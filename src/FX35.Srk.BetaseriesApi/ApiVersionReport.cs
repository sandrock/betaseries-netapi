using System;
using System.Collections.Generic;

namespace Srk.BetaseriesApi {
    public class ApiVersionReport {

        private readonly IBetaseriesSyncApi client;

        public ApiVersionReport(IBetaseriesSyncApi client) {
            this.client = client;
        }

        public IDictionary<string, int> GetReport(object obj) {
            var real = obj as IMethodVersionReport;
            if (real == null)
                throw new ArgumentException("Object does not support API method version reporting.", "obj");

            return real.MethodsVersion;
        }


    }
}
