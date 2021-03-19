using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.References;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class LinkedItemsElement : Accessible
    {
        public Limit? Item_count_limit { get; set; }

        public IList<Reference>? Allowed_content_types { get; set; }

        public LinkedItemsElement()
        {
            Type = "modular_content";
        }
    }
}