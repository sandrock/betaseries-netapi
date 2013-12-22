using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Srk.BetaseriesApi {

    [Serializable]
    partial class BetaError : ISerializable {

        protected BetaError(SerializationInfo info, StreamingContext context) {
            this.Code = info.GetString("Code");
            this.IntCode = info.GetUInt16("IntCode");
            this.Message = info.GetString("Message");
        }

        #region ISerializable Members

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Code", this.Code, typeof(string));
            info.AddValue("IntCode", this.IntCode, typeof(ushort));
            info.AddValue("Message", this.Message, typeof(string));
        }

        #endregion

    }
}
