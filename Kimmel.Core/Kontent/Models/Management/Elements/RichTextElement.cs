using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.Items;

namespace Kimmel.Core.Kontent.Models.Management.Elements
{
    public class RichTextElement : AbstractElement<string>
    {
        public const string Type = "rich_text";

        public IEnumerable<Component>? Components { get; set; }
    }
}