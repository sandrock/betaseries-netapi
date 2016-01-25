
namespace Srk.BetaseriesApi
{
    using System.Collections.Generic;

    internal interface IMethodVersionReport
    {
        IDictionary<string, int> MethodsVersion { get; }
    }
}
