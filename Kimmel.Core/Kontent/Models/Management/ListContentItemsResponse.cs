using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.Items;

namespace Kimmel.Core.Kontent.Models.Management
{
    public class ListContentItemsResponse : AbstractListingResponse
    {
        public IEnumerable<ContentItem>? Items { get; set; }
    }
}