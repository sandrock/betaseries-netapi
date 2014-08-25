
namespace Srk.BetaseriesApi
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
        [DataMember(Order = 0)]
        public T Data { get; set; }

        [DataMember(Name = "errors", Order = 1)]
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
