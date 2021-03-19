using System;
using System.Runtime.Serialization;

namespace Kimmel.Core.Activation
{
    [Serializable]
    public class ActivationException : Exception
    {
        public ActivationException(string? message, char? current = null, int? index = null) : base(Format(message, current, index))
        {
        }

        private static string Format(string? message, char? current, int? index)
        {
            if (current is not null && index is not null)
            {
                return $"[{current}, {index}] {message}";
            }

            return message ?? string.Empty;
        }

        public ActivationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}