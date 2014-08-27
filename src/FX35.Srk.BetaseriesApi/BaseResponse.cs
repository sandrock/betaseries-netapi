
namespace Srk.BetaseriesApi2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class BaseResponse
    {
        [DataMember(Name = "errors", Order = 1)]
        public ResponseError[] Errors { get; set; }
    }

    [DataContract]
    public class BaseResponse<T>
    {
        [DataMember]
        public T Data { get; set; }

        [DataMember(IsRequired = false, Name = "hash")]
        public string Hash { get; set; }

        [DataMember(IsRequired = false, Name = "user")]
        public object User { get; set; }

        [DataMember(Name = "errors")]
        public ResponseError[] Errors { get; set; }
    }

    [DataContract]
    public class ResponseError
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}
