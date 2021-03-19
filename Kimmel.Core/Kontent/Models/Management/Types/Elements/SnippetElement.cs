using Kimmel.Core.Kontent.Models.Management.References;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class SnippetElement : Typed
    {
        public Reference? Snippet { get; set; }

        public SnippetElement()
        {
            Type = "snippet";
        }
    }
}