
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
            this.WriteEntities(context, text);
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

                foreach (var item in context.Methods)
                {
                    text.WriteLine();
                    text.WriteLine(indent, "/// <summary>");
                    text.WriteLine(indent, "/// Response for '" + item.UrlPath + "'.");
                    text.WriteLine(indent, "/// " + item.Description);
                    text.WriteLine(indent, "/// </summary>");

                    // method return type
                    text.Write(indent, "public ");
                    if (item.ResponseFormat != null)
                    {
                        var resultType = this.GetResultTypeName(item.UrlPath);
                        text.Write(this.EntitiesNamespace + "." + resultType + " ");
                    }
                    else
                    {
                        text.Write("void ");
                    }

                    // method name
                    if (item.Method == "DELETE")
                        text.Write("Delete");
                    text.Write(this.GetResultTypeName(item.UrlPath));

                    // method args
                    text.Write("(");
                    string sep = "";
                    foreach (var arg in item.Arguments)
                    {
                        text.Write(sep);
                        text.Write("string ");
                        text.Write(arg.Name);
                        sep = ", ";
                    }

                    text.WriteLine(") {");
                    indent++;

                    text.WriteLine(indent, "// call http " + item.UrlPath);
                    text.WriteLine(indent, "throw new NotImplementedException();");
                    

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
            /*
            text.WriteLine();
            text.WriteLine("#region Entities (from methods)");
            text.WriteLine();
            text.WriteLine("namespace " + this.EntitiesNamespace + " {");
            foreach (var item in context.Methods)
            {
                if (item.ResponseFormat == null)
                    continue;

                var className = this.GetResultTypeName(item.UrlPath);
                text.WriteLine(indent, "");
                text.WriteLine(indent, "/// <summary>");
                text.WriteLine(indent, "/// Response for '" + item.UrlPath + "'.");
                text.WriteLine(indent, "/// " + item.Description);
                text.WriteLine(indent, "/// </summary>");
                text.WriteLine(indent++, "public class " + className + " {");
                foreach (var field in item.ResponseFormat.Entity.Fields.Values)
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
                        default:
                            break;
                    }
                }

                text.WriteLine(--indent, "}");
            }

            text.WriteLine("}");
            text.WriteLine();
            text.WriteLine("#endregion");
            text.WriteLine();
            */
            text.WriteLine();
            text.WriteLine("#region Entities (merged)");
            text.WriteLine();
            text.WriteLine("namespace " + this.EntitiesNamespace + " {");
            text.WriteLine(indent, "using System;");
            
            foreach (var item in context.ResponseFormats)
            {
                var className = this.GetResultTypeName(item.Key);
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
            return string.Join("", methodName.Split('_', '/', '-', ' ').Select(n => Name.ToSingular(Name.ToPublicName(n))));
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
                        arg.Description = li.Nodes().Skip(1).First().ToString();
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
