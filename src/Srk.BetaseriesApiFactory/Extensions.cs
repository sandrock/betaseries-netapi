
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
    }
}
