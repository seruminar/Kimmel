using System;
using System.Runtime.Serialization;

namespace Kimmel.Core.Parsing
{
    [Serializable]
    public class ParsingException : Exception
    {
        public ParsingException(string? message, char current, int index) : this(message, current.ToString(), index)
        {
        }

        public ParsingException(string? message, string? current = null, int? index = null) : base(Format(message, current, index))
        {
        }

        private static string Format(string? message, string? current, int? index)
        {
            if (current is not null && index is not null)
            {
                return $"[{current}, {index}] {message}";
            }

            return message ?? string.Empty;
        }

        public ParsingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}