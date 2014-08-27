
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Response
    {
        public Response()
        {
            this.Warnings = new List<string>();
        }

        public Entity Entity { get; set; }

        public List<string> Warnings { get; set; }

        public string Key { get; set; }

        public string MethodName { get; set; }

        public string ClassName { get; set; }
    }
}
