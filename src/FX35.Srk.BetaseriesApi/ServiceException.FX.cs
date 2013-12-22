using System;

namespace Srk.BetaseriesApi {

    [Serializable]
    partial class ServiceException : Exception {

        protected ServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
