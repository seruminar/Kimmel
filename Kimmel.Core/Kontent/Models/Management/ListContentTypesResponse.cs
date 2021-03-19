using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.Types.ContentTypes;

namespace Kimmel.Core.Kontent.Models.Management
{
    public class ListContentTypesResponse : AbstractListingResponse
    {
        public IEnumerable<ContentType>? Types { get; set; }
    }
}