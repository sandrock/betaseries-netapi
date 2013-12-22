using System;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Exception to be used if the service returns an internal error.
    /// </summary>
    public class ServiceException : Exception {
#pragma warning disable 1591
        public ServiceException() { }
        public ServiceException(string message) : base(message) { }
        public ServiceException(string message, Exception inner) : base(message, inner) { }
#pragma warning restore 1591
    }
}
