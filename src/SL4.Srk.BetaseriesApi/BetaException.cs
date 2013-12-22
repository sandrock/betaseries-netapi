using System;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Exception containing a betaseries API error.
    /// </summary>
    public class BetaException : Exception {

        /// <summary>
        /// Error from the service.
        /// </summary>
        public BetaError BetaError { get; set; }

#pragma warning disable 1591
        public BetaException(BetaError betaError) : base(betaError.Message) {
            BetaError = betaError;
        }
        public BetaException(BetaError betaError, string message)
            : base(message) {
            BetaError = betaError;
        }
        public BetaException(BetaError betaError, string message, Exception inner)
            : base(message, inner) {
            BetaError = betaError;
        }

        public BetaException() { }
        public BetaException(string message) : base(message) { }
        public BetaException(string message, Exception inner) : base(message, inner) { }

#pragma warning restore 1591
    }
}
