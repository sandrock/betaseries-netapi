
namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web;

    /// <summary>
    /// Real implementation of <see cref="IHttpRequestWrapper"/>.
    /// </summary>
    partial class HttpRequestWrapper
    {
        /// <summary>
        /// Base HTTP url for queries. 
        /// This will permit to use a different base adresse (for HTTPS, different port or domain name...).
        /// Default is http://api.betaseries.com/.
        /// </summary>
        /// <remarks>
        /// Value must be setted from .ctor.
        /// </remarks>
        protected readonly string BaseUrl;

        /// <summary>
        /// Formating string for query string.
        /// Must be set from sub-class.
        /// </summary>
        protected readonly string UrlFormat;

        /// <summary>
        /// Formating string for query string.
        /// Must be set from sub-class.
        /// </summary>
        protected readonly string UserAgent;

        /// <summary>
        /// Default .ctor.
        /// </summary>
        /// <param name="urlFormat"></param>
        /// <param name="baseUrl"></param>
        /// <param name="userAgent"></param>
        public HttpRequestWrapper(string urlFormat, string baseUrl, string userAgent)
        {
            UrlFormat = urlFormat;
            BaseUrl = baseUrl;
            UserAgent = userAgent;
        }

        private static void HandleHttpCodes(HttpStatusCode code)
        {
            switch ((int)code)
            {
                // Good statuses
                case (int)HttpStatusCode.OK:
                    break;

                // Redirections (3xx)
                case (int)HttpStatusCode.MultipleChoices:
                case (int)HttpStatusCode.MovedPermanently:
                case (int)HttpStatusCode.Redirect:
                case (int)HttpStatusCode.SeeOther:
                case (int)HttpStatusCode.TemporaryRedirect:
                    throw new ServiceException(
                        "Service did not respond correctly (redirection) (HTTP code: " + (int)code + "). ");

                // Dev errors
                case (int)HttpStatusCode.NotModified:
                case (int)HttpStatusCode.BadRequest:
                case (int)HttpStatusCode.Forbidden:
                case (int)HttpStatusCode.NotFound:
                case (int)HttpStatusCode.MethodNotAllowed:
                case (int)HttpStatusCode.NotAcceptable:
                case (int)HttpStatusCode.RequestTimeout:
                case (int)HttpStatusCode.Gone:
                case (int)HttpStatusCode.LengthRequired:
                case (int)HttpStatusCode.PreconditionFailed:
                case (int)HttpStatusCode.RequestEntityTooLarge:
                case (int)HttpStatusCode.RequestUriTooLong:
                case (int)HttpStatusCode.UnsupportedMediaType:
                case (int)HttpStatusCode.ExpectationFailed:
                case (int)HttpStatusCode.HttpVersionNotSupported:
                    throw new InvalidOperationException(
                        "Service returned an error. The cause seems to be a bad request. " +
                        "Update your application or contact support. ");

                // LOL
                case 418:
                    throw new ServiceException(
                        "Service made a joke (HTTP code: " + (int)code +
                        "). You might want to try again. ");

                // Computer/LAN issue
                case (int)HttpStatusCode.UseProxy:
                case 306:
                case 450:
                    throw new InvalidOperationException(
                        "Something on your computer or network prevents the website from being contacted " +
                        "(HTTP code: " + (int)code + "). ");

                // Server errors 
                case (int)HttpStatusCode.InternalServerError:
                case (int)HttpStatusCode.NotImplemented:
                case (int)HttpStatusCode.BadGateway:
                case (int)HttpStatusCode.ServiceUnavailable:
                case (int)HttpStatusCode.GatewayTimeout:
                    throw new ServiceException(
                        "Service seems to be unavailable (maintenance?), please try again later " +
                        "(HTTP code: " + (int)code + "). ");

                // Other error
                default:
                    throw new ServiceException(
                        "Service did not respond correctly (HTTP code: " + (int)code + "). ");
            }
        }

        private static string PostEncode(string value)
        {
            return value.Replace("=", "%3D").Replace("&", "%26");
        }

        /// <summary>
        /// Create a HTTP query string from a dictionary of parameters.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string GetQueryString(string action, Dictionary<string, string> parameters)
        {
            parameters = parameters ?? new Dictionary<string, string>();

            var querystring = parameters.GetQueryString();

            string str = string.Format(UrlFormat, BaseUrl, action, querystring);
            return str;
        }
    }
}
