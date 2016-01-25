
namespace Srk.BetaseriesApi
{
    using System;
    using System.Xml.Linq;
    using System.Globalization;

    internal static class XmlTools
    {
        private static CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        private static CultureInfo apiCulture = new CultureInfo("fr-fr");

        internal static bool ElementValue(this XElement element, XName name, bool defaultValue)
        {
            var e = element.Element(name);
            //short val = 0;
            //return e != null && !string.IsNullOrEmpty(e.Value) && short.TryParse(e.Value, out val) ? val : defaultValue;
            return e != null && !string.IsNullOrEmpty(e.Value) ? bool.Parse(e.Value) : defaultValue;
        }

        internal static short ElementValue(this XElement element, XName name, short defaultValue)
        {
            var e = element.Element(name);
            //short val = 0;
            //return e != null && !string.IsNullOrEmpty(e.Value) && short.TryParse(e.Value, out val) ? val : defaultValue;
            return e != null && !string.IsNullOrEmpty(e.Value) ? short.Parse(e.Value) : defaultValue;
        }

        internal static ushort ElementValue(this XElement element, XName name, ushort defaultValue)
        {
            var e = element.Element(name);
            //ushort val = 0;
            //return e != null && !string.IsNullOrEmpty(e.Value) && ushort.TryParse(e.Value, out val) ? val : defaultValue;
            return e != null && !string.IsNullOrEmpty(e.Value) ? ushort.Parse(e.Value) : defaultValue;
        }

        internal static int ElementValue(this XElement element, XName name, int defaultValue)
        {
            var e = element.Element(name);
            //int val = 0;
            //return e != null && !string.IsNullOrEmpty(e.Value) && int.TryParse(e.Value, out val) ? val : defaultValue;
            return e != null && !string.IsNullOrEmpty(e.Value) ? int.Parse(e.Value) : defaultValue;
        }

        internal static uint ElementValue(this XElement element, XName name, uint defaultValue)
        {
            var e = element.Element(name);
            //uint val = 0;
            //return e != null && !string.IsNullOrEmpty(e.Value) && uint.TryParse(e.Value, out val) ? val : defaultValue;
            return e != null && !string.IsNullOrEmpty(e.Value) ? uint.Parse(e.Value) : defaultValue;
        }

        internal static long ElementValue(this XElement element, XName name, long defaultValue)
        {
            var e = element.Element(name);
            //int val = 0;
            //return e != null && !string.IsNullOrEmpty(e.Value) && int.TryParse(e.Value, out val) ? val : defaultValue;
            return e != null && !string.IsNullOrEmpty(e.Value) ? long.Parse(e.Value) : defaultValue;
        }

        internal static ulong ElementValue(this XElement element, XName name, ulong defaultValue)
        {
            var e = element.Element(name);
            //uint val = 0;
            //return e != null && !string.IsNullOrEmpty(e.Value) && uint.TryParse(e.Value, out val) ? val : defaultValue;
            return e != null && !string.IsNullOrEmpty(e.Value) ? ulong.Parse(e.Value) : defaultValue;
        }

        internal static DateTime ElementValue(this XElement element, XName name, DateTime defaultValue)
        {
            var e = element.Element(name);
            return e != null && !string.IsNullOrEmpty(e.Value) ? DateTime.Parse(e.Value) : defaultValue;
        }

        internal static DateTime? ElementValueTimestampToDatetime(this XElement element, XName name)
        {
            var e = element.Element(name);
            long timestamp = 0;
            if (string.IsNullOrEmpty(e.Value) || !long.TryParse(e.Value, out timestamp) || timestamp <= 100)
                return null;
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dt.AddSeconds(timestamp);
        }

        internal static TimeSpan ElementValueTimespanFromSeconds(this XElement element, XName name, TimeSpan defaultValue)
        {
            var e = element.Element(name);
            return e != null && !string.IsNullOrEmpty(e.Value) ? TimeSpan.FromSeconds(int.Parse(e.Value)) : defaultValue;
        }

        internal static TimeSpan? ElementValueTimespanFromMinutes(this XElement element, XName name)
        {
            var e = element.Element(name);
            int val = 0;
            if (e != null && !string.IsNullOrEmpty(e.Value) && int.TryParse(e.Value, out val))
            {
                return TimeSpan.FromMinutes(val);
            }
            return null;
        }

        internal static TimeSpan? ElementValueTimespanFromSeconds(this XElement element, XName name)
        {
            var e = element.Element(name);
            int val = 0;
            if (e != null && !string.IsNullOrEmpty(e.Value) && int.TryParse(e.Value, out val))
            {
                return TimeSpan.FromSeconds(val);
            }
            return null;
        }

        internal static string ElementValue(this XElement element, XName name, string defaultValue)
        {
            var e = element.Element(name);
            return e != null && !string.IsNullOrEmpty(e.Value) ? e.Value : defaultValue;
        }

        internal static string ElementValueString(this XElement element, XName name)
        {
            var e = element.Element(name);
            return e != null ? e.Value : null;
        }

        internal static bool? ElementValueNbool(this XElement element, XName name)
        {
            var e = element.Element(name);
            bool val = false;
            if (e != null && !string.IsNullOrEmpty(e.Value))
            {
                string v = e.Value;
                if (v == "1")
                    return true;
                else if (v == "0")
                    return false;
                else if (bool.TryParse(v, out val))
                    return val;
            }
            return null;
        }

        internal static float? ElementValueNfloat(this XElement element, XName name)
        {
            var e = element.Element(name);
            float val = 0;
            if (e != null && !string.IsNullOrEmpty(e.Value) && float.TryParse(e.Value, NumberStyles.AllowDecimalPoint, apiCulture.NumberFormat, out val))
            {
                return val;
            }
            return null;
        }

        internal static int? ElementValueNint(this XElement element, XName name)
        {
            var e = element.Element(name);
            int val = 0;
            if (e != null && !string.IsNullOrEmpty(e.Value) && int.TryParse(e.Value, out val))
            {
                return val;
            }
            return null;
        }
    }
}
