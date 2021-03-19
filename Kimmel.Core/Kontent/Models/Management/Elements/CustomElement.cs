using Newtonsoft.Json;

namespace Kimmel.Core.Kontent.Models.Management.Elements
{
    public class CustomElement : AbstractElement<string>
    {
        public const string Type = "custom";

        [JsonProperty("searchable_value")]
        public string? Searchable_Value { get; set; }
    }
}