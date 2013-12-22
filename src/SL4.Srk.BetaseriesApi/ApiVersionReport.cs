using System;
using System.Collections.Generic;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Helper to generate an API implementation report.
    /// </summary>
    public class ApiVersionReport {

        private readonly IBetaseriesApi client;

        /// <summary>
        /// Default .ctor.
        /// </summary>
        /// <param name="client"></param>
        public ApiVersionReport(IBetaseriesApi client) {
            this.client = client;
        }

        /// <summary>
        /// Create a report from a <see cref="IMethodVersionReport"/> object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IDictionary<string, int> GetReport(object obj) {
            var real = obj as IMethodVersionReport;
            if (real == null)
                throw new ArgumentException("Object does not support API method version reporting.", "obj");

            return real.MethodsVersion;
        }

    }
}
