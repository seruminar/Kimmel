using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.Elements;
using Kimmel.Core.Kontent.Models.Management.References;

using Newtonsoft.Json;

namespace Kimmel.Core.Kontent.Models.Management.Items
{
    public class LanguageVariant
    {
        [JsonProperty("item")]
        public Reference? ItemReference { get; set; }

        [JsonProperty("workflow_step")]
        public Reference? WorkflowStep { get; set; }

        public IList<IElement>? Elements { get; set; }
    }
}