using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.References;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class CustomElement : Accessible
    {
        public string Source_url { get; set; } = string.Empty;

        public string? Json_parameters { get; set; }

        public IList<Reference>? Allowed_elements { get; set; }

        public CustomElement()
        {
            Type = "custom";
        }
    }
}