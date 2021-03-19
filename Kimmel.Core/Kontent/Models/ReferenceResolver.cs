using System;

using Kimmel.Core.Kontent.Models.Management.References;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kimmel.Core.Kontent.Models
{
    internal class ReferenceResolver : JsonConverter<Reference>
    {
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

        public override Reference? ReadJson(JsonReader reader, Type objectType, Reference? existingValue, bool hasExistingValue, JsonSerializer serializer)
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            serializer.ContractResolver = new SubclassResolver<Reference>();

            var rawObject = JObject.Load(reader);

            if (rawObject.ContainsKey("id"))
            {
                return rawObject.ToObject<IdReference>(serializer);
            }

            if (rawObject.ContainsKey("codename"))
            {
                return rawObject.ToObject<CodenameReference>(serializer);
            }

            if (rawObject.ContainsKey("external_id"))
            {
                return rawObject.ToObject<ExternalIdReference>(serializer);
            }

            throw new NotImplementedException("Reference format not supported.");
        }

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, Reference? value, JsonSerializer serializer)
        {
            switch (value as object)
            {
                case IdReference idReference:
                    JToken.FromObject(new { id = idReference.Value }).WriteTo(writer);
                    break;

                case CodenameReference codenameReference:
                    JToken.FromObject(new { codename = codenameReference.Value }).WriteTo(writer);
                    break;

                case ExternalIdReference externalIdReference:
                    JToken.FromObject(new { external_id = externalIdReference.Value }).WriteTo(writer);
                    break;
            }
        }
    }
}