
namespace Srk.BetaseriesApi {

    /// <summary>
    /// Represents the betaseries.com API.
    /// This interface requires synchronous and asynchronous implementation.
    /// </summary>
    public interface IBetaseriesApi : IBetaseriesBaseApi, IBetaseriesSyncApi, IBetaseriesAsyncApi, IBetaseriesTaskAsyncApi { }
}
