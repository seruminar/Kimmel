using System.Collections.Generic;

namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public class TypeDescription
    {
        public string Id { get; }

        public string Label { get; }

        public DescribesTypedProperty[] PropertyDescriptions { get; }

        public ICollection<string> LinkedTypeIds { get; set; } = new HashSet<string>();

        public ICollection<string> SnippetTypeIds { get; set; } = new HashSet<string>();

        public TypeDescription(
            TypePropertyDescription typePropertyDescription,
            DescribesTypedProperty[] propertyDescriptions
            )
        {
            Label = typePropertyDescription.Label;
            Id = typePropertyDescription.Id;
            PropertyDescriptions = propertyDescriptions;
        }

        public override bool Equals(object? other)
        {
            if (other is TypeDescription otherTypeDescription)
            {
                return Id == otherTypeDescription.Id && Label == otherTypeDescription.Label;
            }

            return base.Equals(other);
        }

        public override int GetHashCode() => Id.GetHashCode() ^ Label.GetHashCode();
    }
}