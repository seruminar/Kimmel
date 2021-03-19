namespace Kimmel.Core.Kontent.Models.Management.Elements
{
    public class TaxonomyElement : AbstractReferenceListElement
    {
        public const string Type = "taxonomy";

        internal TaxonomyElement(AbstractReferenceListElement element)
        {
            Element = element.Element;
            Value = element.Value;
        }
    }
}