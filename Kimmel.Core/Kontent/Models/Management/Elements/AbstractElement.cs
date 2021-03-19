using Kimmel.Core.Kontent.Models.Management.References;

namespace Kimmel.Core.Kontent.Models.Management.Elements
{
    public abstract class AbstractElement<T> : IElement
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public T Value { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Reference? Element { get; set; }
    }
}