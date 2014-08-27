
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ApiFactoryContext
    {
        public ApiFactoryContext()
        {
            this.Methods = new List<MethodDescription>();
            this.ArgumentEnums = new Dictionary<string, EntityEnumField>();
        }

        public List<MethodDescription> Methods { get; set; }

        public Dictionary<string, Response> ResponseFormats { get; set; }

        public Dictionary<string, EntityEnumField> ArgumentEnums { get; set; }

        public System.Xml.Linq.XDocument Transforms { get; set; }
    }
}
