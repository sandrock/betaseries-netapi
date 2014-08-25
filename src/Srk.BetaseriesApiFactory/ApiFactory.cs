
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Generates API client code based on the website's documentation and custom rules.
    /// </summary>
    public class ApiFactory
    {
        private static readonly string BaseDocUrl = @"http://www.betaseries.com/api/methodes/";
        private static readonly string[] DocCategories = new string[]
        {
            "comments",
            "episodes",
            "friends",
            "members",
            "messages",
            "movies",
            "pictures",
            "planning",
            "shows",
            "subtitles",
            "timeline",
        };
        private static readonly string[] validHttpMethods = new string[] { "GET", "POST", "DELETE", };
        private static readonly Regex linkTagRegex = new Regex("<link[^<>]+>");
        private static readonly Regex badHeaderRegex = new Regex("<div class=\"caption\"><p>[^<>]+</p></div>");


        public ApiFactory()
        {
            this.EntitiesNamespace = "Srk.BetaseriesApi";
            this.ServicesNamespace = "Srk.BetaseriesApi";
            this.ServicesName = "BetaseriesClient";
            
        }

        public string EntitiesNamespace { get; set; }

        public string ServicesNamespace { get; set; }

        public string ServicesName { get; set; }

        public void Run(TextWriter text)
        {
            var context = new ApiFactoryContext();
            this.FetchDocumentation(context);
            this.ReviewDocumentation(context);
            this.ApplyTransforms(context, text);
            this.WriteEntities(context, text);
            this.WriteArgumentEnums(context, text);
            this.WriteService(context, text);
        }

        private void ReviewDocumentation(ApiFactoryContext context)
        {
            // merge response formats
            var formats = context.Methods
                .Where(m => m.ResponseFormat != null)
                .Select(m => new
                {
                    Method = m,
                    Format = m.ResponseFormat,
                })
                .GroupBy(x => x.Method.UrlPath)
                .ToArray();

            var mergedFormats = new Dictionary<string, Response>(formats.Count());
            context.ResponseFormats = mergedFormats;
            foreach (var pathFormats in formats)
            {
                var path = pathFormats.Key;
                var merged = new Response()
                {
                    Entity = new Entity(),
                };
                merged.MethodName = pathFormats.Key;
                mergedFormats.Add(path, merged);
                foreach (var format in pathFormats)
                {
                    if (merged.Entity.Name == null && format.Format.Entity.Name != null)
                    {
                        merged.Entity.Name = format.Format.Entity.Name;
                    }

                    merged.ClassName = merged.ClassName ?? format.Format.ClassName;

                    foreach (var field in format.Format.Entity.Fields)
                    {
                        if (merged.Entity.Fields.ContainsKey(field.Key))
                        {
                            var mergedField = merged.Entity.Fields[field.Key];
                            if (mergedField.Type != field.Value.Type)
                                merged.Warnings.Add("Field conflict on '" + field.Key + "'. Type differts between '" + mergedField.Type + "' and '" + field.Value.Type + "'.");
                        }
                        else
                        {
                            merged.Entity.Fields.Add(field.Key, field.Value);
                        }
                    }

                    format.Method.ResponseFormat = merged;
                }
            }

            // rename ArgumentEnums
            foreach (var argEnum in context.ArgumentEnums)
            {
                if (argEnum.Value.Name.Contains('|'))
                {
                    // changes     title|popularity
                    // to          Title_Popularity
                    argEnum.Value.Name = this.GetResultTypeName(argEnum.Value.Name);
                }
            }
        }

        private void ApplyTransforms(ApiFactoryContext context, TextWriter text)
        {
            /*
            var stream = this.GetType().Assembly.GetManifestResourceNames();
            text.WriteLine("// ");
            text.WriteLine("// Assembly.GetManifestResourceNames();");
            text.WriteLine("// ");
            foreach (var s in stream)
            {
                text.WriteLine("//  - " + s);
            }
            text.WriteLine("// ");
            */
            var xmlStream = typeof(ApiFactory).Assembly.GetManifestResourceStream("Srk.BetaseriesApiFactory.Api2.xml");
            if (xmlStream == null)
            {
                text.WriteLine("// ");
                text.WriteLine("// ApplyTransforms: no XML file found");
                text.WriteLine("// ");
                return;
            }

            XDocument doc;
            try
            {
                doc = XDocument.Load(xmlStream);
            }
            finally
            {
                xmlStream.Dispose();
            }

            foreach (var transform in doc.Root.Elements("Transform"))
            {
                var target = transform.Attribute("Target").Value;
                if (target == "Method")
                {
                    ApplyMethodTransform(context, transform);
                }
                else if (target == "ArgumentEnum")
                {
                    ApplyArgumentEnumTransform(context, transform);
                }
                else if (target == "ResponseFormat")
                {
                    ApplyResponseFormatTransform(context, transform);
                }
            }
        }

        private void ApplyMethodTransform(ApiFactoryContext context, XElement transform)
        {
            var matchElement = transform.Element("Match");
            var method = matchElement.Element("Method");
            var urlPathElement = matchElement.Element("UrlPath");

            foreach (var format in context.Methods.ToArray())
            {
                // match
                bool match = true;

                if (urlPathElement != null)
                {
                    match &= urlPathElement.Value == format.UrlPath;
                }

                if (method != null)
                {
                    match &= method.Value == format.Method;
                }

                if (!match)
                    continue;

                var removeElement = transform.Element("Remove");
                if (removeElement != null)
                {
                    context.Methods.Remove(format);
                }

                var setReponseFormat = transform.Element("SetReponseFormat");
                if (setReponseFormat != null)
                {
                    format.ResponseFormat = this.ParseReponseFormat(setReponseFormat.Value);
                }

                var setMethodName = transform.Element("SetMethodName");
                if (setMethodName != null)
                {
                    format.MethodName = setMethodName.Value;
                }
            }
        }

        private static void ApplyArgumentEnumTransform(ApiFactoryContext context, XElement transform)
        {
            var matcher = transform.Element("ArgumentEnumMatch");
            var valuesToMatch = matcher.Elements("Value").ToArray();
            if (valuesToMatch.Length > 0)
            {
                foreach (var argEnum in context.ArgumentEnums)
                {
                    var valueMatches = new List<string>(argEnum.Value.Values.Count);
                    foreach (var valueToMatch in valuesToMatch)
                    {
                        string val = valueToMatch.Value;
                        bool isRequired = true;

                        var isPresentElement = valueToMatch.Attribute("IsPresent");
                        if (isPresentElement != null && bool.TryParse(isPresentElement.Value, out isRequired))
                        {
                        }
                        else
                        {
                            isRequired = true;
                        }

                        if (argEnum.Value.Values.Contains(val) && !valueMatches.Contains(val) && isRequired)
                            valueMatches.Add(val);

                        if (argEnum.Value.Values.Contains(val) && !valueMatches.Contains(val) && !isRequired)
                            break;
                    }

                    if (valueMatches.Count == valuesToMatch.Length)
                    {
                        var setNameElement = transform.Element("SetName");
                        if (setNameElement != null)
                        {
                            argEnum.Value.Name = setNameElement.Value;
                        }
                    }
                }
            }
        }

        private static void ApplyResponseFormatTransform(ApiFactoryContext context, XElement transform)
        {
            var matcher = transform.Element("ResponseFormatMatch");
            var urlPathElement = matcher.Element("UrlPath");

            foreach (var format in context.ResponseFormats.ToArray())
            {
                // match
                bool match = true;

                if (urlPathElement != null)
                {
                    match &= urlPathElement.Value == format.Key;
                }

                if (!match)
                    continue;

                var removeElement = transform.Element("Remove");
                if (removeElement != null)
                {
                    context.ResponseFormats.Remove(format.Key);
                }

                var setClassNameElement = transform.Element("SetClassName");
                if (setClassNameElement != null)
                {
                    format.Value.ClassName = setClassNameElement.Value;
                }
            }
        }

        protected void WriteService(ApiFactoryContext context, TextWriter text)
        {
            int indent = 0;
            text.WriteLine();
            text.WriteLine("#region Services");
            text.WriteLine();
            text.WriteLine(indent++, "namespace " + this.ServicesNamespace + " {");
            text.WriteLine(indent, "using System;");
            text.WriteLine(indent, "using System.Collections.Generic;");
            text.WriteLine(indent, "using Newtonsoft.Json;");
            text.WriteLine();
            text.WriteLine(indent++, "public partial class " + this.ServicesName + " {");

            var categories = context.Methods.GroupBy(m => m.Category);
            foreach (var category in categories)
            {
                var categoryName = Name.ToPublicName(category.Key);
                var categoryPrivateName = Name.ToPrivateName(category.Key);

                text.WriteLine();
                text.WriteLine(indent, "private Betaseries" + categoryName + "Client " + categoryPrivateName + "Client;");
                text.WriteLine(indent++, "public Betaseries" + categoryName + "Client " + categoryName + " {");
                text.WriteLine(indent, "get { return this." + categoryPrivateName + "Client ?? (this." + categoryPrivateName + "Client = new Betaseries" + categoryName + "Client(this)); }");
                text.WriteLine(--indent, "}");
            }

            text.WriteLine(--indent, "}");
            text.WriteLine();

            foreach (var category in categories)
            {
                var categoryName = Name.ToPublicName(category.Key);
                var categoryPrivateName = Name.ToPrivateName(category.Key);

                text.WriteLine();
                text.WriteLine(indent++, "public partial class Betaseries" + categoryName + "Client {");
                text.WriteLine(indent, "private readonly BetaseriesClient client;");
                text.WriteLine();
                text.WriteLine(indent++, "public Betaseries" + categoryName + "Client(BetaseriesClient client) {");
                text.WriteLine(indent, "this.client = client;");
                text.WriteLine(--indent, "}");
                text.WriteLine();

                foreach (var item in context.Methods.Where(m => m.Category == category.Key))
                {
                    text.WriteLine();
                    text.WriteLine(indent, "/// <summary>");
                    text.WriteLine(indent, "/// " + item.Description);
                    text.WriteLine(indent, "/// Call for " + item.Method + " '" + item.UrlPath + "'.");
                    text.WriteLine(indent, "/// </summary>");

                    foreach (var arg in item.Arguments)
                    {
                        text.WriteLine(indent, "/// <param name=\"" + arg.Name + "\">" + arg.Description.ProperHtmlEscape() + "</param>");
                    }

                    // method return type
                    text.Write(indent, "public ");
                    string resultType = null, resultTypeFull = null;
                    if (item.ResponseFormat != null)
                    {
                        if (item.ResponseFormat.ClassName != null)
                        {
                            resultType = item.ResponseFormat.ClassName;
                            resultTypeFull = this.EntitiesNamespace + "." + resultType;
                        }
                        else
                        {
                            resultType = this.GetResultTypeName(item.UrlPath);
                            resultTypeFull = this.EntitiesNamespace + "." + resultType;
                        }
                        text.Write(resultTypeFull + " ");
                    }
                    else
                    {
                        text.Write("void ");
                    }

                    // method name
                    if (item.MethodName != null)
                    {
                        text.Write(item.MethodName);
                    }
                    else
                    {
                        if (item.Method == "DELETE")
                            text.Write("Delete");
                        text.Write(this.GetResultTypeName(item.UrlPath));
                    }

                    // method args
                    text.Write("(");
                    string sep = "";
                    foreach (var arg in item.Arguments)
                    {
                        text.Write(sep);
                        if (arg.EnumField != null)
                        {
                            text.Write(arg.EnumField.Name + " ");
                        }
                        else if (arg.IsArray)
                        {
                            text.Write("string[] ");
                        }
                        else
                        {
                            text.Write("string ");
                        }

                        text.Write(arg.Name);
                        sep = ", ";
                    }

                    text.WriteLine(") {");
                    indent++;

                    // method code
                    text.WriteLine(indent, "var parameters = new List<KVP<string, string>>(" + item.Arguments.Count + ");");
                    foreach (var arg in item.Arguments)
                    {
                        if (arg.EnumField != null)
                        {
                            text.WriteLine(indent, "parameters.Add(new KVP<string, string>(\"" + arg.Name + "\", " + arg.Name + ".ToString()));"); 
                        }
                        else if (arg.IsArray)
                        {
                            text.WriteLine(indent++, "foreach (var argValue in " + arg.Name + ") {");
                            text.WriteLine(indent, "parameters.Add(new KVP<string, string>(\"" + arg.Name + "[]\", argValue));");
                            text.WriteLine(--indent, "}"); 
                        }
                        else
                        {
                            text.WriteLine(indent, "parameters.Add(new KVP<string, string>(\"" + arg.Name + "\", " + arg.Name + "));");
                        }
                    }

                    text.WriteLine(indent, "var response = this.client.ExecuteQuery(\"" + item.Method + "\", \"" + item.UrlPath + "\", parameters);");

                    text.WriteLine(indent, "");
                    if (item.ResponseFormat != null)
                    {
                        text.WriteLine(indent, "var result = JsonConvert.DeserializeObject<BaseResponse<" + resultTypeFull + ">>(response);");
                    }
                    else
                    {
                        text.WriteLine(indent, "var result = JsonConvert.DeserializeObject<BaseResponse>(response);");
                    }

                    text.WriteLine(indent, "this.client.HandleErrors(result);");

                    if (item.ResponseFormat != null)
                    {
                        text.WriteLine(indent, "return result.Data;");
                    }

                    text.WriteLine(--indent, "}");
                }

                text.WriteLine(--indent, "}");
            }

            text.WriteLine("}");
            text.WriteLine();
            text.WriteLine("#endregion");
            text.WriteLine();
            text.Flush();
        }

        protected void WriteEntities(ApiFactoryContext context, TextWriter text)
        {
            int indent = 1;
            text.WriteLine();
            text.WriteLine("#region Entities (merged)");
            text.WriteLine();
            text.WriteLine("namespace " + this.EntitiesNamespace + " {");
            text.WriteLine(indent, "using System;");
            
            foreach (var item in context.ResponseFormats)
            {
                var className = item.Value.ClassName ?? this.GetResultTypeName(item.Key);
                text.WriteLine(indent, "");
                text.WriteLine(indent, "/// <summary>");
                text.WriteLine(indent, "/// Response format for '" + item.Key + "'.");
                text.WriteLine(indent, "/// </summary>");
                text.WriteLine(indent++, "public class " + className + " {");
                foreach (var warning in item.Value.Warnings)
                {
                    text.WriteLine(indent, "#warning " + warning);
                }

                foreach (var field in item.Value.Entity.Fields.Values)
                {
                    this.WriteCSharpProperty(ref indent, text, field);
                }

                text.WriteLine(--indent, "}");
            }

            text.WriteLine("}");
            text.WriteLine();
            text.WriteLine("#endregion");
            text.WriteLine();
            text.Flush();
        }

        protected void WriteArgumentEnums(ApiFactoryContext context, TextWriter text)
        {
            int indent = 1;
            text.WriteLine();
            text.WriteLine("#region ArgumentEnums (merged)");
            text.WriteLine();
            text.WriteLine("namespace " + this.EntitiesNamespace + " {");
            text.WriteLine(indent, "using System;");
            
            foreach (var item in context.ArgumentEnums)
            {
                var argEnum = item.Value;
                var className = argEnum.Name;
                text.WriteLine(indent, "");
                text.WriteLine(indent, "/// <summary>");
                text.WriteLine(indent, "/// Response format id '" + item.Key.ProperHtmlEscape() + "'.");
                text.WriteLine(indent, "/// </summary>");
                text.WriteLine(indent, "/// <remark>");
                foreach (var name in argEnum.Names)
                {
                    text.WriteLine(indent, "/// " + name.ProperHtmlEscape() + ""); 
                }

                text.WriteLine(indent, "/// </remark>");
                text.WriteLine(indent++, "public enum " + className + " {");

                foreach (var field in item.Value)
                {
                    text.WriteLine(indent, field + ",");
                }

                text.WriteLine(--indent, "}");
            }

            text.WriteLine("}");
            text.WriteLine();
            text.WriteLine("#endregion");
            text.WriteLine();
            text.Flush();
        }

        protected void WriteCSharpProperty(ref int indent, TextWriter text, EntityField field)
        {
            text.WriteLine(indent, "");
            text.WriteLine(indent, "/// <summary>");
            text.WriteLine(indent, "/// Gets or sets the " + field.Name + ".");
            text.WriteLine(indent, "/// </summary>");
            switch (field.Type)
            {
                case EntityFieldType.String:
                    text.WriteLine(indent, "public string " + field.Name + " { get; set; }");
                    break;

                case EntityFieldType.Url:
                    text.WriteLine(indent, "public string " + field.Name + " { get; set; }");
                    break;

                case EntityFieldType.DateTime:
                    text.WriteLine(indent, "public DateTime " + field.Name + " { get; set; }");
                    break;

                case EntityFieldType.Integer:
                    text.WriteLine(indent, "public int " + field.Name + " { get; set; }");
                    break;

                case EntityFieldType.Boolean:
                    text.WriteLine(indent, "public bool " + field.Name + " { get; set; }");
                    break;

                case EntityFieldType.Enum:
                    var enumName = field.Name + "Enum";
                    text.WriteLine(indent, "public " + enumName + " " + field.Name + " { get; set; }");
                    text.WriteLine(indent, "");
                    text.WriteLine(indent++, "public enum " + enumName + " {");
                    foreach (var enumValue in field.EnumField.Values)
                    {
                        text.WriteLine(indent, enumValue + ",");
                    }

                    text.WriteLine(--indent, "}");
                    break;

                case EntityFieldType.Entity:
                    var subClassName = Name.ToPublicName(field.Name + "Class");
                    text.WriteLine(indent, "public " + subClassName + " " + field.Name + " { get; set; }");
                    text.WriteLine(indent, "");
                    text.WriteLine(indent++, "public class " + subClassName + " {");
                    foreach (var subField in field.Entity.Fields)
                    {
                        this.WriteCSharpProperty(ref indent, text, subField.Value);
                    }

                    text.WriteLine(--indent, "}");
                    break;

                default:
                    text.WriteLine(indent, "#warning Unknown field type");
                    text.WriteLine(indent, "public " + field.Type + " " + field.Name + " { get; set; }");
                    break;
            }

        }

        private string GetResultTypeName(MethodDescription item)
        {
            return Name.ToPublicName(item.FullPath.Replace('/', '_'));
        }

        private string GetResultTypeName(string methodName)
        {
            return string.Join("", methodName.Split('_', '/', '-', ' ', '|').Select(n => Name.ToSingular(Name.ToPublicName(n))));
        }

        private void FetchDocumentation(ApiFactoryContext context)
        {
            ////Parallel.ForEach(
            ////    DocCategories,
            ////    category =>
            ////    {
            ////        this.FetchDocumentation(BaseDocUrl + category, category, context);
            ////    });
            foreach (var category in DocCategories)
            {
                this.FetchDocumentation(BaseDocUrl + category, category, context);
            }
        }

        private void FetchDocumentation(string documentationUrl, string category, ApiFactoryContext context)
        {
            // fetch documentation page and parse as XML
            XElement[] documentation;
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(documentationUrl);
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream);
                    var html1 = reader.ReadToEnd();
                    var html = this.CleanHtml(html1);
                    try
                    {
                        var xdoc = XDocument.Parse(html);
                        var contentElement = xdoc
                            .Descendants("div")
                            .Where(e =>
                            {
                                var attr = e.Attribute("id");
                                return attr != null && attr.Value == "right";
                            })
                            .Single();
                        documentation = contentElement.Elements().ToArray();

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }

            // read elements
            MethodDescription method = null;
            HtmlDocumentationField readField = HtmlDocumentationField.None;
            for (int i = 0; i < documentation.Length; i++)
            {
                var element = documentation[i];

                if (element.Name == "h1")
                    continue;

                if (element.Name == "h2")
                {
                    readField = HtmlDocumentationField.TitleH2;
                    method = new MethodDescription();
                    method.UrlPath = element.Value.Trim();
                    method.Category = category;
                    context.Methods.Add(method);
                    continue;
                }

                if (readField == HtmlDocumentationField.TitleH2 && element.Name == "p")
                {
                    if (element.Name != "p")
                        Debug.Fail("Expected <p>DescriptionParagraph element after <h2>TitleH2");

                    method.Description = element.Value.Trim();
                    readField = HtmlDocumentationField.DescriptionParagraph;
                    continue;
                }

                if (readField <= HtmlDocumentationField.DescriptionParagraph && element.Name == "pre")
                {
                    if (element.Name != "pre")
                        Debug.Fail("Expected <pre>FirstRequestLine element after <p>DescriptionParagraph");

                    var value = element.Value.Trim();
                    var firstWordPos = value.IndexOf(' ');
                    var firstWord = value.Substring(0, firstWordPos);
                    method.Method = firstWord;
                    if (!validHttpMethods.Contains(method.Method))
                        Debug.Fail("Invalid HTTP method '"+method.Method+"' in <pre>FirstRequestLine");

                    method.FullPath = value.Substring(firstWordPos + 1);
                    
                    readField = HtmlDocumentationField.FirstRequestLine;
                    continue;
                }

                if (readField <= HtmlDocumentationField.FirstRequestLine && element.Name == "ul")
                {
                    foreach (var li in element.Elements())
                    {
                        if (li.Name != "li")
                            Debug.Fail("Expected <li> in <ul>ArgumentList");

                        var arg = new MethodArgumentDescription();
                        arg.Name = li.Elements("strong").Single().Value.Trim();
                        if (arg.Name.EndsWith("[]"))
                        {
                            arg.Name = arg.Name.Substring(0, arg.Name.Length - 2);
                            arg.IsArray = true;
                        }

                        arg.Description = li.Nodes().Skip(1).First().ToString();
                        if (arg.Description.StartsWith(" : "))
                            arg.Description = arg.Description.Substring(3);

                        var argumentEnumRegex = new Regex(@"([a-zA-Z0-9]+)(\|([a-zA-Z0-9]+))+");
                        var argumentEnumMatch = argumentEnumRegex.Match(arg.Description);
                        if (argumentEnumMatch.Success)
                        {
                            var id = argumentEnumMatch.Groups[0].Value;
                            if (context.ArgumentEnums.ContainsKey(id))
                            {
                                arg.EnumField = context.ArgumentEnums[id];
                            }
                            else
                            {
                                arg.EnumField = new EntityEnumField();
                                arg.EnumField.Name = id;
                                arg.EnumField.Names.Add("ARGID:" + id);
                                arg.EnumField.Names.Add("ARGDESC:" + arg.Description);
                                arg.EnumField.Add(argumentEnumMatch.Groups[1].Captures[0].Value);
                                foreach (Capture part in argumentEnumMatch.Groups[3].Captures)
                                {
                                    arg.EnumField.Add(part.Value);
                                }

                                context.ArgumentEnums.Add(id, arg.EnumField);
                            }
                        }

                        method.Arguments.Add(arg);
                    }

                    readField = HtmlDocumentationField.ArgumentList;
                    continue;
                }

                if (readField <= HtmlDocumentationField.ArgumentList && element.Name == "p" && element.Value.Contains("Réponse"))
                {
                    if (element.Name != "p")
                        Debug.Fail("Expected <p>ResponseHeaderParagraph element after <ul>ArgumentList");

                    if (!element.Value.Contains("Réponse"))
                        Debug.Fail("Expected <p>ResponseHeaderParagraph to contain 'Réponse'");

                    readField = HtmlDocumentationField.ResponseHeaderParagraph;
                    continue;
                }

                if (readField <= HtmlDocumentationField.ResponseHeaderParagraph && element.Name == "pre")
                {
                    if (element.Name != "pre")
                        Debug.Fail("Expected <pre>ResponseFormat element after <p>ResponseHeaderParagraph");

                    try
                    {
                        method.ResponseFormat = this.ParseReponseFormat(element.Value);
                    }
                    catch (NotSupportedException ex)
                    {
                        var error = new NotSupportedException("Documented method '" + method.Category + "/" + method.UrlPath + "' is not supported", ex);
                        error.Data.Add("HtmlDocumentationField", HtmlDocumentationField.ResponseFormat);
                        error.Data.Add("XElement.Value", element.Value);
                    }

                    readField = HtmlDocumentationField.ResponseFormat;
                    continue;
                }

                if (readField <= HtmlDocumentationField.ResponseFormat && element.Name == "p")
                {
                    if (element.Name != "p")
                        Debug.Fail("Expected <p>LastUpdateParagraph element after <pre>ResponseFormat");

                    method.LastUpdateString = element.Value;

                    readField = HtmlDocumentationField.LastUpdateParagraph;
                    continue;
                }

                Debug.WriteLine("Hmm...");
            }
            
            // the end
        }

        protected Response ParseReponseFormat(string pre)
        {
            var format = new Response();
            var lines = pre.Split(new string[] { "\n", }, StringSplitOptions.None);

            var firstLine = lines[0].Trim();
            var formats = this.ParseReponseFormatFormats(lines.Skip(1).ToArray());
            format.Entity = this.ParseResponseFormatDeclaration(firstLine);

            if (format.Entity.ItemName != null && formats.ContainsKey(format.Entity.ItemName))
            {
                format.Entity.ItemEntity = formats[format.Entity.ItemName];
            }

            if (formats.ContainsKey(format.Entity.Name))
            {
                format.Entity = formats[format.Entity.Name];
            }

            return format;
        }

        private Dictionary<string, Entity> ParseReponseFormatFormats(string[] lines)
        {
            var list = new Dictionary<string, Entity>();

            Entity entity = null;
            EntityField field2 = null;
            int level = 0;
            int l = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                level = line.TakeWhile(c => c == ' ').Count();
                line = line.Trim();
                var colon = line.IndexOf(':');

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (colon > 0 && line[0] != '-')
                {
                    entity = new Entity();
                    entity.Name = line.Substring(0, colon);
                    list.Add(entity.Name, entity);
                    continue;
                }

                var field = new EntityField();
                var parts = line.Split('-', ':');
                Debug.Assert(string.IsNullOrWhiteSpace(parts[0]));
                field.Name = parts[1].Trim();
                var type = parts[2].Trim();
                // integer, string, ...
                // string # comment
                var sharpPosition = type.IndexOf('#');
                if (sharpPosition > 0)
                {
                    type = type.Substring(0, sharpPosition).TrimEnd();
                }

                if (level <= 2)
                {
                    field2 = field;
                    if (!string.IsNullOrWhiteSpace(type))
                        field.SetType(type);
                    entity.Fields.Add(field.Name, field);
                }
                else if (level <= 4)
                {
                    field.SetType(type);
                    field2.Type = EntityFieldType.Entity;
                    field2.Entity = field2.Entity ?? new Entity
                    {
                        Name = field2.Name,
                    };
                    field2.Entity.Fields.Add(field.Name, field);
                }
                else
                {
                    throw new NotSupportedException("Response formats of depth > 2 are not supported");
                }
            }

            return list;
        }

        private Entity ParseResponseFormatDeclaration(string line)
        {
            // comment
            // characters: character*
            // comments: comment*

            var entity = new Entity();
            if (line.Contains(':'))
            {
                var colon = line.IndexOf(':');
                entity.Name = line.Substring(0, colon);
                entity.ItemName = line.Substring(colon + 2).Trim();
                if (entity.ItemName.EndsWith("*"))
                {
                    entity.ItemName = entity.ItemName.Substring(0, entity.ItemName.Length - 1);
                    entity.HasMultipleChild = true;
                }
                else
                {
                    entity.HasOneChild = true;
                }
            }
            else
            {
                entity.Name = line.Trim();
            }

            return entity;
        }

        private string CleanHtml(string html)
        {
            // html tag has a attribute without value
            html = html.Replace("itemscope itemtype=", "itemtype=");

            // link tags are not closed correctly
            html = linkTagRegex.Replace(html, "");

            // duplicate attribute 
            html = html.Replace("<li class=\"inscription\" class=\"\">", "<li class=\"inscription\">");

            // bad entities
            html = html.Replace("Séries & Films", "Séries &amp; Films");
            html = html.Replace("&mdash;", "");
            html = html.Replace("&rarr;", "");
            html = html.Replace("&copy;", "");

            // a div has no end tag...
            html = html.Replace("    </div></div></div>", "    </div></div>");
            html = html.Replace("</body>", "</div></body>");

            // xss issue in <div id="header"> (random & char)
            html = badHeaderRegex.Replace(html, "");

            return html;
        }

        private enum HtmlDocumentationField
        {
            None,
            TitleH2,
            DescriptionParagraph,
            FirstRequestLine,
            ArgumentList,
            ResponseHeaderParagraph,
            ResponseFormat,
            LastUpdateParagraph,
        }
    }
}
