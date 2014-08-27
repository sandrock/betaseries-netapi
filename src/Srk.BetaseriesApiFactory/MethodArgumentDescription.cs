
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MethodArgumentDescription
    {
        public MethodArgumentDescription()
        {
            this.Extras = new Dictionary<string, object>();
            this.BlitableType = "string";
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public ArgumentType ArgumentType { get; set; }

        public EntityEnumField EnumField { get; set; }

        public bool IsArray { get; set; }

        public Dictionary<string, object> Extras { get; set; }

        public string BlitableType { get; set; }

        public override string ToString()
        {
            if (this.EnumField != null)
                return "MethodArgument " + this.ArgumentType + " '" + this.Name + "' enum " + (this.EnumField.Name) + (this.IsArray ? "[]" : "");
            else
                return "MethodArgument " + this.ArgumentType + " '" + this.Name + "' string" + (this.IsArray ? "[]" : "");
        }
    }

    public enum ArgumentType
    {
        UrlQueryString,
        PostQueryString,
    }
}
