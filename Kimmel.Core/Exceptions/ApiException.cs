using System;
using System.Runtime.Serialization;

namespace Kimmel.Core.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException(string? message) : base(message)
        {
        }

        public ApiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}