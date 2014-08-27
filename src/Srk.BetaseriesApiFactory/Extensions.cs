
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using System.Xml.Linq;

    public static class Extensions
    {
        /// <summary>
        /// Escapes XML/HTML characters that needs to be escaped when outputing HTML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <remarks>
        /// Code based on http://wonko.com/post/html-escaping and https://www.owasp.org/index.php/XSS_%28Cross_Site_Scripting%29_Prevention_Cheat_Sheet.
        /// </remarks>
        public static string ProperHtmlEscape(this string content)
        {
            if (content == null)
                return null;

            var builder = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                switch (c)
                {
                    case '<':
                        builder.Append("&lt;");
                        break;

                    case '>':
                        builder.Append("&gt;");
                        break;

                    case '&':
                        builder.Append("&amp;");
                        break;

                    case '"':
                        builder.Append("&quot;");
                        break;

                    case '\'':
                        builder.Append("&#x27;");
                        break;

                    ////case '/':
                    ////    builder.Append("&#x2F;");
                    ////    break;

                    default:
                        builder.Append(content[i]);
                        break;
                }
            }

            return builder.ToString();
        }

        public static string AttributeOrValue(this XElement element, XName name)
        {
            var attribute = element.Attribute(name);
            if (attribute != null)
                return attribute.Value;
            return element.Value;
        }

        public static string AttributeValue(this XElement element, XName name)
        {
            var attribute = element.Attribute(name);
            if (attribute != null)
                return attribute.Value;
            return null;
        }

        public static string AttributeOrElementValue(this XElement element, string name)
        {
            var attribute = element.Attribute(name);
            if (attribute != null)
                return attribute.Value;

            return element.ElementValue(name);
        }

        public static string ElementValue(this XElement element, string name)
        {
            var names = name.Split('.');
            XElement child = element;
            foreach (var subname in names)
            {
                child = child.Element(subname);
                if (child == null)
                    return null;
            }

            if (child != element)
                return child.Value;
            return null;
        }
    }
}
