namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class TextElement : Accessible
    {
        public MaximumTextLength? Maximum_text_length { get; set; }

        public TextElement()
        {
            Type = "text";
        }
    }
}