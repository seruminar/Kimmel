namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public class SnippetTypePropertyDescription : DescribesTypedProperty
    {
        public string Id { get; }

        public SnippetTypePropertyDescription(
            string identifier
            )
        {
            Id = identifier;
        }
    }
}