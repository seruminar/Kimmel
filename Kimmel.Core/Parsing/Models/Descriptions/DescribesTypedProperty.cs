namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public abstract class DescribesTypedProperty
    {
        public string Type => GetType().Name[..^"PropertyDescription".Length];
    }
}