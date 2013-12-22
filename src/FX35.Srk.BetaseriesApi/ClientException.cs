using System;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Exception to be used if the service returns an input error.
    /// </summary>
    public partial class ClientException : Exception {

        public ClientException() { }
        public ClientException(string message) : base(message) { }
        public ClientException(string message, Exception inner) : base(message, inner) { }

    }
}
