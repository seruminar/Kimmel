namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public abstract class DescribesAccessibleProperty : DescribesTypedProperty
    {
        public string Label { get; }

        public bool Required { get; set; }

        protected DescribesAccessibleProperty(string? label)
        {
            Label = label ?? string.Empty;
        }
    }
}