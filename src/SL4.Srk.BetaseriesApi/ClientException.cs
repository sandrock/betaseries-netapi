using System;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Exception to be used if the service returns an input error.
    /// </summary>
    public class ClientException : Exception {
#pragma warning disable 1591
        public ClientException() { }
        public ClientException(string message) : base(message) { }
        public ClientException(string message, Exception inner) : base(message, inner) { }
#pragma warning restore 1591
    }
}
