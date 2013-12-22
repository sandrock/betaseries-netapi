using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Srk.BetaseriesApi {

    [Serializable]
    partial class BetaException : ISerializable {

        protected BetaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("BetaError", this.BetaError, typeof(BetaError));
        }

    }
}
