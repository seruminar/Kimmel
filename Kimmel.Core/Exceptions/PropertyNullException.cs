using System;
using System.Runtime.Serialization;

namespace Kimmel.Core.Exceptions
{
    [Serializable]
    public class PropertyNullException : Exception
    {
        public PropertyNullException(string? message) : base($"Property {message} is null.")
        {
        }

        public PropertyNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PropertyNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}