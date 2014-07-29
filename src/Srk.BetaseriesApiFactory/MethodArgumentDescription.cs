
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MethodArgumentDescription
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return "MethodArgument " + this.Name;
        }
    }
}
