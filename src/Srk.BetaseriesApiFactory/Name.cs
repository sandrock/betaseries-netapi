
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Name
    {
        public static string ToPublicName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;
            if (name.Length == 1)
                return name.ToUpperInvariant();
            return name[0].ToString().ToUpperInvariant() + name.Substring(1);
        }

        public static string ToPrivateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;
            if (name.Length == 1)
                return name.ToLowerInvariant();
            return name[0].ToString().ToLowerInvariant() + name.Substring(1);
        }

        internal static string ToSingular(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;
            if (name.EndsWith("ies"))
                return name.Substring(0, name.Length - 3) + "y";
            if (name.EndsWith("uses"))
                return name.Substring(0, name.Length - 4) + "s";
            if (name.EndsWith("s"))
                return name.Substring(0, name.Length - 1);
            return name;
        }
    }
}
