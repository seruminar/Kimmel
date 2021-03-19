using System;
using System.Collections.Generic;
using System.Linq;

using Kimmel.Core.Kontent.Models.Management.Elements;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kimmel.Core.Kontent.Models
{
    public class ElementTypeResolver : JsonConverter<IElement>
    {
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

        public override IElement? ReadJson(JsonReader reader, Type objectType, IElement? existingValue, bool hasExistingValue, JsonSerializer serializer)
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            serializer.ContractResolver = new SubclassResolver<IElement>();

            var element = JObject.Load(reader);

            if (element.ContainsKey(nameof(RichTextElement.Components).ToLower()))
            {
                return element.ToObject<RichTextElement>(serializer);
            }

            if (element.ContainsKey(nameof(CustomElement.Searchable_Value).ToLower()))
            {
                return element.ToObject<CustomElement>(serializer);
            }

            if (element.ContainsKey(nameof(UrlSlugElement.Mode).ToLower()))
            {
                return element.ToObject<UrlSlugElement>(serializer);
            }

            return (element["value"]?.Type) switch
            {
                JTokenType.Float => element.ToObject<NumberElement>(serializer),
                JTokenType.Date => element.ToObject<DateTimeElement>(serializer),
                JTokenType.Array => element.ToObject<AbstractReferenceListElement>(serializer),
                _ => element.ToObject<TextElement>(serializer),
            };
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, IElement? value, JsonSerializer serializer) => throw new NotImplementedException();

        public static AbstractReferenceListElement ResolveAbstractReferenceListElement(AbstractReferenceListElement listElement, HashSet<Management.Types.Elements.Typed> elementTypes)
        {
            var elementType = elementTypes.First(typeElement => typeElement.Id == listElement.Element?.Value);

            return elementType.Type switch
            {
                AssetElement.Type => new AssetElement(listElement),
                MultipleChoiceElement.Type => new MultipleChoiceElement(listElement),
                TaxonomyElement.Type => new TaxonomyElement(listElement),
                LinkedItemsElement.Type => new LinkedItemsElement(listElement),
                _ => throw new NotImplementedException($"Element type '{elementType.Type}' is not supported."),
            };
        }
    }
}