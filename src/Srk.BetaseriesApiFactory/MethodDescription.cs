
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class MethodDescription
    {
        public MethodDescription()
        {
            this.Arguments = new List<MethodArgumentDescription>();
        }

        public string Category { get; set; }
        public string Method { get; set; }
        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        public string FullPath { get; set; }

        public Response ResponseFormat { get; set; }

        public string LastUpdateString { get; set; }

        public List<MethodArgumentDescription> Arguments { get; set; }

        public string MethodName { get; set; }

        public override string ToString()
        {
            return "Method " + this.Method + " " + this.UrlPath;
        }

        public bool ReturnRawResult { get; set; }
    }
}
