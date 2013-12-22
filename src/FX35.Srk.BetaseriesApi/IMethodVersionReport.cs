using System.Collections.Generic;

namespace Srk.BetaseriesApi {
    internal interface IMethodVersionReport {

        IDictionary<string, int> MethodsVersion { get; }

    }
}
