using System;
using System.Collections.Generic;

using Kimmel.Core.Activation;
using Kimmel.Core.Activation.Kontent;
using Kimmel.Core.Kontent.Models.Management.References;
using Kimmel.Core.Kontent.Models.Management.Types.ContentTypes;
using Kimmel.Core.Kontent.Models.Management.Types.Elements;
using Kimmel.Core.Models;
using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Activation.Kontent
{
    public class KontentTypeActivator : ITypeActivator
    {
        public ReadOnlyOptions Options { get; }

        public KontentTypeActivator(
            Options options
            )
        {
            Options = new ReadOnlyOptions(options);
        }

        public ContentType ActivateType(TypeDescription typeDescription, Guid globalGuid)
        {
            var elements = ParseElements(typeDescription, globalGuid);

            return new ContentType(typeDescription.Label, elements)
            {
                External_id = $"{typeDescription.Id}-{globalGuid}"
            };
        }

        public ContentType ActivateTypeSnippet(TypeDescription typeDescription, Guid globalGuid)
        {
            var elements = ParseElements(typeDescription, globalGuid);

            return new ContentType(typeDescription.Label, elements)
            {
                External_id = $"{typeDescription.Id}-{globalGuid}"
            };
        }

        private IList<Typed> ParseElements(TypeDescription typeDescription, Guid globalGuid)
        {
            var elements = new List<Typed>(typeDescription.PropertyDescriptions.Length);

            foreach (var propertyDescription in typeDescription.PropertyDescriptions)
            {
                switch (propertyDescription)
                {
                    case LinkedItemsPropertyDescription linkedItemsPropertyDescription:
                        {
                            elements.Add(new LinkedItemsElement()
                            {
                                Allowed_content_types = ParseLinks(linkedItemsPropertyDescription.LinkedTypeIds, globalGuid),
                                Item_count_limit = ParseRange(linkedItemsPropertyDescription),
                                Is_required = linkedItemsPropertyDescription.Required,
                                Name = linkedItemsPropertyDescription.Label
                            });
                            break;
                        }
                    case SnippetTypePropertyDescription snippetTypePropertyDescription:
                        {
                            elements.Add(new SnippetElement()
                            {
                                Snippet = new ExternalIdReference($"{snippetTypePropertyDescription.Id}-{globalGuid}")
                            });
                            break;
                        }
                    case AssetPropertyDescription assetPropertyDescription:
                        {
                            var allowedFileTypes = assetPropertyDescription.AssetMode switch
                            {
                                AssetMode.Images => AllowedTypes.Adjustable,
                                _ => AllowedTypes.Any,
                            };

                            elements.Add(new AssetElement()
                            {
                                Allowed_file_types = allowedFileTypes,
                                Asset_count_limit = ParseRange(assetPropertyDescription),
                                Is_required = assetPropertyDescription.Required,
                                Name = assetPropertyDescription.Label
                            });
                            break;
                        }
                    case TextPropertyDescription textPropertyDescription:
                        {
                            MaximumTextLength? maximumTextLength = null;

                            if (textPropertyDescription.Words > 0)
                            {
                                maximumTextLength = new MaximumTextLength()
                                {
                                    Applies_to = MaximumTextLength.ValueAppliesTo.Words,
                                    Value = textPropertyDescription.Words.Value
                                };
                            }

                            if (textPropertyDescription.Words > 0 && textPropertyDescription.Characters > 0)
                            {
                                throw new ActivationException($"'{textPropertyDescription.Label}' cannot have both words and characters options.");
                            }

                            if (textPropertyDescription.Characters > 0)
                            {
                                maximumTextLength = new MaximumTextLength()
                                {
                                    Applies_to = MaximumTextLength.ValueAppliesTo.Characters,
                                    Value = textPropertyDescription.Characters.Value
                                };
                            }

                            elements.Add(new TextElement()
                            {
                                Maximum_text_length = maximumTextLength,
                                Is_required = textPropertyDescription.Required,
                                Name = textPropertyDescription.Label
                            });
                            break;
                        }
                    case DatePropertyDescription datePropertyDescription:
                        {
                            elements.Add(new DateTimeElement()
                            {
                                Is_required = datePropertyDescription.Required,
                                Name = datePropertyDescription.Label
                            });
                            break;
                        }
                    case NumberPropertyDescription numberPropertyDescription:
                        {
                            elements.Add(new NumberElement()
                            {
                                Is_required = numberPropertyDescription.Required,
                                Name = numberPropertyDescription.Label
                            });
                            break;
                        }
                    case SingleChoicePropertyDescription singleChoicePropertyDescription:
                        {
                            elements.Add(new MultipleChoiceElement()
                            {
                                Mode = MultipleChoiceElement.ChoiceMode.Single,
                                Options = ParseMultipleOptions(singleChoicePropertyDescription.Options),
                                Is_required = singleChoicePropertyDescription.Required,
                                Name = singleChoicePropertyDescription.Label
                            });
                            break;
                        }
                    case MultipleChoicePropertyDescription multipleChoicePropertyDescription:
                        {
                            elements.Add(new MultipleChoiceElement()
                            {
                                Mode = MultipleChoiceElement.ChoiceMode.Multiple,
                                Options = ParseMultipleOptions(multipleChoicePropertyDescription.Options),
                                Is_required = multipleChoicePropertyDescription.Required,
                                Name = multipleChoicePropertyDescription.Label
                            });
                            break;
                        }
                    case RichTextPropertyDescription richTextPropertyDescription:
                        {
                            MaximumTextLength? maximumTextLength = null;

                            if (richTextPropertyDescription.Words > 0)
                            {
                                maximumTextLength = new MaximumTextLength()
                                {
                                    Applies_to = MaximumTextLength.ValueAppliesTo.Words,
                                    Value = richTextPropertyDescription.Words.Value
                                };
                            }

                            if (richTextPropertyDescription.Words > 0 && richTextPropertyDescription.Characters > 0)
                            {
                                throw new ActivationException($"'{richTextPropertyDescription.Label}' cannot have both words and characters options.");
                            }

                            if (richTextPropertyDescription.Characters > 0)
                            {
                                maximumTextLength = new MaximumTextLength()
                                {
                                    Applies_to = MaximumTextLength.ValueAppliesTo.Characters,
                                    Value = richTextPropertyDescription.Characters.Value
                                };
                            }

                            var allowedBlocks = new List<RichTextElement.Block>(4);

                            if (richTextPropertyDescription.Images)
                            {
                                allowedBlocks.Add(RichTextElement.Block.Images);
                            }

                            if (richTextPropertyDescription.P)
                            {
                                allowedBlocks.Add(RichTextElement.Block.Text);
                            }

                            if (richTextPropertyDescription.Tables)
                            {
                                allowedBlocks.Add(RichTextElement.Block.Tables);
                            }

                            if (richTextPropertyDescription.Components)
                            {
                                if (allowedBlocks.Count == 3)
                                {
                                    allowedBlocks.Clear();
                                }
                                else
                                {
                                    allowedBlocks.Add(RichTextElement.Block.ComponentsAndItems);
                                }
                            }

                            var allowedTextBlocks = new List<RichTextElement.TextBlock>(7);

                            if (richTextPropertyDescription.P)
                            {
                                allowedTextBlocks.Add(RichTextElement.TextBlock.Paragraph);
                            }

                            if (richTextPropertyDescription.H1)
                            {
                                allowedTextBlocks.Add(RichTextElement.TextBlock.Heading1);
                            }

                            if (richTextPropertyDescription.H2)
                            {
                                allowedTextBlocks.Add(RichTextElement.TextBlock.Heading2);
                            }

                            if (richTextPropertyDescription.H3)
                            {
                                allowedTextBlocks.Add(RichTextElement.TextBlock.Heading3);
                            }

                            if (richTextPropertyDescription.H4)
                            {
                                allowedTextBlocks.Add(RichTextElement.TextBlock.Heading4);
                            }

                            if (richTextPropertyDescription.Ol)
                            {
                                allowedTextBlocks.Add(RichTextElement.TextBlock.OrderedList);
                            }

                            if (richTextPropertyDescription.Ul)
                            {
                                if (allowedTextBlocks.Count == 6)
                                {
                                    allowedTextBlocks.Clear();
                                }
                                else
                                {
                                    allowedTextBlocks.Add(RichTextElement.TextBlock.UnorderedList);
                                }
                            }

                            var allowedFormatting = new List<RichTextElement.Formatting>(7) { RichTextElement.Formatting.Unstyled };

                            if (richTextPropertyDescription.B)
                            {
                                allowedFormatting.Add(RichTextElement.Formatting.Bold);
                            }

                            if (richTextPropertyDescription.I)
                            {
                                allowedFormatting.Add(RichTextElement.Formatting.Italic);
                            }

                            if (richTextPropertyDescription.A)
                            {
                                allowedFormatting.Add(RichTextElement.Formatting.Link);
                            }

                            if (richTextPropertyDescription.Sub)
                            {
                                allowedFormatting.Add(RichTextElement.Formatting.Subscript);
                            }

                            if (richTextPropertyDescription.Sup)
                            {
                                allowedFormatting.Add(RichTextElement.Formatting.Superscript);
                            }

                            var allowedTableBlocks = new List<RichTextElement.TableBlock>(7) { RichTextElement.TableBlock.Text };

                            if (richTextPropertyDescription.TablesImages)
                            {
                                allowedTableBlocks.Clear();
                            }

                            var allowedTableTextBlocks = new List<RichTextElement.TextBlock>(7);

                            if (richTextPropertyDescription.TablesP)
                            {
                                allowedTableTextBlocks.Add(RichTextElement.TextBlock.Paragraph);
                            }

                            if (richTextPropertyDescription.TablesH1)
                            {
                                allowedTableTextBlocks.Add(RichTextElement.TextBlock.Heading1);
                            }

                            if (richTextPropertyDescription.TablesH2)
                            {
                                allowedTableTextBlocks.Add(RichTextElement.TextBlock.Heading2);
                            }

                            if (richTextPropertyDescription.TablesH3)
                            {
                                allowedTableTextBlocks.Add(RichTextElement.TextBlock.Heading3);
                            }

                            if (richTextPropertyDescription.TablesH4)
                            {
                                allowedTableTextBlocks.Add(RichTextElement.TextBlock.Heading4);
                            }

                            if (richTextPropertyDescription.TablesOl)
                            {
                                allowedTableTextBlocks.Add(RichTextElement.TextBlock.OrderedList);
                            }

                            if (richTextPropertyDescription.TablesUl)
                            {
                                if (allowedTableTextBlocks.Count == 6)
                                {
                                    allowedTableTextBlocks.Clear();
                                }
                                else
                                {
                                    allowedTableTextBlocks.Add(RichTextElement.TextBlock.UnorderedList);
                                }
                            }

                            var allowedTableFormatting = new List<RichTextElement.Formatting>(7) { RichTextElement.Formatting.Unstyled };

                            if (richTextPropertyDescription.TablesB)
                            {
                                allowedTableFormatting.Add(RichTextElement.Formatting.Bold);
                            }

                            if (richTextPropertyDescription.TablesI)
                            {
                                allowedTableFormatting.Add(RichTextElement.Formatting.Italic);
                            }

                            if (richTextPropertyDescription.TablesA)
                            {
                                allowedTableFormatting.Add(RichTextElement.Formatting.Link);
                            }

                            if (richTextPropertyDescription.TablesSub)
                            {
                                allowedTableFormatting.Add(RichTextElement.Formatting.Subscript);
                            }

                            if (richTextPropertyDescription.TablesSup)
                            {
                                allowedTableFormatting.Add(RichTextElement.Formatting.Superscript);
                            }

                            elements.Add(new RichTextElement()
                            {
                                Maximum_text_length = maximumTextLength,
                                Allowed_content_types = ParseLinks(richTextPropertyDescription.ComponentTypeIds, globalGuid),
                                Allowed_blocks = allowedBlocks.ToArray(),
                                Allowed_text_blocks = allowedTextBlocks.ToArray(),
                                Allowed_formatting = allowedFormatting.ToArray(),
                                Allowed_table_blocks = allowedTableBlocks.ToArray(),
                                Allowed_table_text_blocks = allowedTableTextBlocks.ToArray(),
                                Allowed_table_formatting = allowedTableFormatting.ToArray(),
                                Allowed_image_types = richTextPropertyDescription.Images ? AllowedTypes.Adjustable : AllowedTypes.Any,
                                Is_required = richTextPropertyDescription.Required,
                                Name = richTextPropertyDescription.Label
                            });
                            break;
                        }
                    case CustomPropertyDescription customPropertyDescription:
                        {
                            elements.Add(new CustomElement()
                            {
                                Source_url = "https://custom",
                                Is_required = customPropertyDescription.Required,
                                Name = customPropertyDescription.Label
                            });
                            break;
                        }
                }
            }

            return elements;
        }

        public IList<MultipleChoiceElement.Option> ParseMultipleOptions(string[]? options)
        {
            if (options is null)
            {
                return new List<MultipleChoiceElement.Option>();
            }

            var parsedOptions = new List<MultipleChoiceElement.Option>(options.Length);

            foreach (var option in options)
            {
                parsedOptions.Add(new MultipleChoiceElement.Option()
                {
                    Name = option
                });
            }

            return parsedOptions;
        }

        public IList<Reference> ParseLinks(string[] linkedTypeIds, Guid globalGuid)
        {
            var links = new List<Reference>(linkedTypeIds.Length);

            foreach (var linkedTypeId in linkedTypeIds)
            {
                links.Add(new ExternalIdReference($"{linkedTypeId}-{globalGuid}"));
            }

            return links;
        }

        public Limit? ParseRange(DescribesList listPropertyDescription)
        {
            if (listPropertyDescription.Minimum is not null && listPropertyDescription.Maximum is null)
            {
                return new Limit(listPropertyDescription.Minimum.Value, Limit.LimitCondition.AtLeast);
            }
            else if (listPropertyDescription.Minimum is null && listPropertyDescription.Maximum is not null)
            {
                return new Limit(listPropertyDescription.Maximum.Value, Limit.LimitCondition.AtMost);
            }
            else if (listPropertyDescription.Minimum is not null && listPropertyDescription.Maximum is not null)
            {
                return new Limit(listPropertyDescription.Minimum.Value, Limit.LimitCondition.Exactly);
            }

            return null;
        }
    }
}