using System;
using System.Collections.Generic;

using Kimmel.Core.Models;
using Kimmel.Core.Parsing;
using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Parsing
{
    public class TypeDescriber : ITypeDescriber
    {
        public ReadOnlyOptions Options { get; }

        public TypeDescriber(
            Options options
            )
        {
            Options = new ReadOnlyOptions(options);
        }

        public TypeDescription DescribeType(
            DescribesTypedProperty[] activators,
            int count,
            ICollection<string> typeLinkedTypeIds,
            ICollection<string> typeSnippetTypeIds
            )
        {
            if (activators[0] is not TypePropertyDescription typePropertyDescription)
            {
                throw new ParsingException($"Description is not of type '{nameof(TypePropertyDescription)}'.");
            }

            if (count == 1)
            {
                return new TypeDescription(typePropertyDescription, Array.Empty<DescribesTypedProperty>())
                {
                    SnippetTypeIds = typeSnippetTypeIds
                };
            }
            else
            {
                return new TypeDescription(typePropertyDescription, activators[1..count])
                {
                    SnippetTypeIds = typeSnippetTypeIds
                };
            }
        }

        public TypeDescription CloneType(TypeDescription typeDescription)
        {
            return new TypeDescription(
                    new TypePropertyDescription(typeDescription.Label, $"{typeDescription.Id}-{Guid.NewGuid()}"),
                    typeDescription.PropertyDescriptions
                );
        }

        public TypeDescription DescribeEmptyType(string typeId)
        {
            return new TypeDescription(
                new TypePropertyDescription(null, typeId),
                Array.Empty<DescribesTypedProperty>()
                );
        }

        public void SetTypeLinks(TypeDescription typeDescription, IDictionary<string, string>? linkedTypeIdsMap = null)
        {
            var linkedTypeIds = new HashSet<string>();

            foreach (var propertyDescription in typeDescription.PropertyDescriptions)
            {
                switch (propertyDescription)
                {
                    case LinkedItemsPropertyDescription linkedItemsPropertyDescription:
                        for (var linkedTypeIdIndex = 0; linkedTypeIdIndex < linkedItemsPropertyDescription.LinkedTypeIds.Length; linkedTypeIdIndex++)
                        {
                            if (linkedTypeIdsMap is not null && linkedTypeIdsMap.TryGetValue(
                                linkedItemsPropertyDescription.LinkedTypeIds[linkedTypeIdIndex],
                                out var linkedTypeId
                                ))
                            {
                                linkedItemsPropertyDescription.LinkedTypeIds[linkedTypeIdIndex] = linkedTypeId;
                                linkedTypeIds.Add(linkedTypeId);
                            }
                            else
                            {
                                linkedTypeIds.Add(linkedItemsPropertyDescription.LinkedTypeIds[linkedTypeIdIndex]);
                            }
                        }
                        break;

                    case RichTextPropertyDescription richTextPropertyDescription:
                        for (var linkedTypeIdIndex = 0; linkedTypeIdIndex < richTextPropertyDescription.ComponentTypeIds.Length; linkedTypeIdIndex++)
                        {
                            if (linkedTypeIdsMap is not null && linkedTypeIdsMap.TryGetValue(
                                richTextPropertyDescription.ComponentTypeIds[linkedTypeIdIndex],
                                out var linkedTypeId
                                ))
                            {
                                richTextPropertyDescription.ComponentTypeIds[linkedTypeIdIndex] = linkedTypeId;
                                linkedTypeIds.Add(linkedTypeId);
                            }
                            else
                            {
                                linkedTypeIds.Add(richTextPropertyDescription.ComponentTypeIds[linkedTypeIdIndex]);
                            }
                        }
                        break;
                }
            }

            typeDescription.LinkedTypeIds = linkedTypeIds;
        }
    }
}