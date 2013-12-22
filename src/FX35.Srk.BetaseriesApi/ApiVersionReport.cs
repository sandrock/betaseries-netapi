
namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Permits to obtain a report of implemented API methods for a client object.
    /// </summary>
    public class ApiVersionReport
    {
        private readonly IBetaseriesSyncApi client;

        public ApiVersionReport(IBetaseriesSyncApi client)
        {
            this.client = client;
        }

        public IDictionary<string, int> GetReport(object obj)
        {
            var real = obj as IMethodVersionReport;
            if (real == null)
                throw new ArgumentException("Object does not support API method version reporting.", "obj");

            return real.MethodsVersion;
        }
    }
}
