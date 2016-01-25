
namespace Srk.BetaseriesApi
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    partial class ServiceException : Exception
    {

        protected ServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
