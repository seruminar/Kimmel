namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public class TypePropertyDescription : DescribesAccessibleProperty
    {
        public string Id { get; }

        public TypePropertyDescription(
            string? label,
            string identifier
            ) : base(label ?? identifier)
        {
            Id = identifier;
        }
    }
}