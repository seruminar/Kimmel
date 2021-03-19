using Kimmel.Core.Kontent.Models.Management.References;

using Newtonsoft.Json;

namespace Kimmel.Core.Kontent.Models.Management.Items
{
    public class ContentItem
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Codename { get; set; }

        [JsonProperty("type")]
        public Reference? TypeReference { get; set; }

        [JsonProperty("external_id")]
        public string? ExternalId { get; set; }
    }
}