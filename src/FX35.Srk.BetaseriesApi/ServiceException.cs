
namespace Srk.BetaseriesApi
{
    using System;

    /// <summary>
    /// Exception to be used if the service returns an internal error.
    /// </summary>
    public partial class ServiceException : Exception
    {
        public ServiceException()
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
