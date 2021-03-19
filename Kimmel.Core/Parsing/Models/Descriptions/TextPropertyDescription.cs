namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public class TextPropertyDescription : DescribesAccessibleProperty
    {
        public int? Words { get; set; }

        public int? Characters { get; set; }

        public TextPropertyDescription(
            string? label
            ) : base(label)
        {
        }
    }
}