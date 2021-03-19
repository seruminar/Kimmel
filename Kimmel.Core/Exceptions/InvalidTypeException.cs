using System;
using System.Runtime.Serialization;

namespace Kimmel.Core.Exceptions
{
    [Serializable]
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string? message) : base($"Type {message} is invalid.")
        {
        }

        public InvalidTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}