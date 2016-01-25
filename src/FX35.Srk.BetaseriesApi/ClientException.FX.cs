
namespace Srk.BetaseriesApi
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    partial class ClientException : Exception
    {

        protected ClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
