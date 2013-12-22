using System;
using System.Collections.Generic;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Represents the betaseries.com API.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IBetaseriesApi : IBetaseriesBaseApi, IBetaseriesAsyncApi, IDisposable
    {
    }
}
