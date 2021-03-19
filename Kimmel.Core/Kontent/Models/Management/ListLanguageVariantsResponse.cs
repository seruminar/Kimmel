using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.Items;

namespace Kimmel.Core.Kontent.Models.Management
{
    public class ListLanguageVariantsResponse : AbstractListingResponse
    {
        public IEnumerable<LanguageVariant>? Variants { get; set; }
    }
}