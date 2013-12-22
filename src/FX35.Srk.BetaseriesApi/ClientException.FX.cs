using System;

namespace Srk.BetaseriesApi {

    [Serializable]
    partial class ClientException : Exception {

        protected ClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
