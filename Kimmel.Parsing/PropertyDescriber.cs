using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

using Kimmel.Core.Models;
using Kimmel.Core.Parsing;
using Kimmel.Core.Parsing.Models;
using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Parsing
{
    public class PropertyDescriber : IPropertyDescriber
    {
        private readonly Property noPropertyFallback;
        private readonly Property noOptionsFallback;
        private readonly Property snippetType;

        private readonly Func<IList<OptionParsingState<DescribesAccessibleProperty>>> requiredOptions;
        private readonly Func<RangeParsingState<DescribesList>> rangeOption;

        private readonly Func<IList<OptionParsingState<AssetPropertyDescription>>> assetOptions;

        private readonly Func<IList<OptionParsingState<TextPropertyDescription>>> textOptions;
        private readonly Func<IList<DetailsParsingState<TextPropertyDescription>>> textDetailedOptions;

        private readonly Func<IList<OptionParsingState<RichTextPropertyDescription>>> richTextOptions;
        private readonly Func<IList<DetailsParsingState<RichTextPropertyDescription>>> richTextDetailedOptions;

        public ReadOnlyOptions Options { get; }

        public PropertyDescriber(Options options)
        {
            Options = new ReadOnlyOptions(options);

            foreach (var property in Options.Properties)
            {
                if (noPropertyFallback is null && property.Description == Options.NoPropertyFallback)
                {
                    noPropertyFallback = property;
                }

                if (noOptionsFallback is null && property.Description == Options.NoOptionsFallback)
                {
                    noOptionsFallback = property;
                }

                if (snippetType is null && property.Description == Options.SnippetType)
                {
                    snippetType = property;
                }
            }

            if (noPropertyFallback is null)
            {
                throw new ParsingException($"'{nameof(noPropertyFallback)}' does not have a value.");
            }

            if (noOptionsFallback is null)
            {
                throw new ParsingException($"'{nameof(noOptionsFallback)}' does not have a value.");
            }

            if (snippetType is null)
            {
                throw new ParsingException($"'{nameof(snippetType)}' does not have a value.");
            }

            requiredOptions = () => new List<OptionParsingState<DescribesAccessibleProperty>>()
            {
                { new OptionParsingState<DescribesAccessibleProperty>(false, Options.Required.ToString().AsMemory(), description => description.Required = true) }
            };

            rangeOption = () => new RangeParsingState<DescribesList>(false, (description, minimum, maximum) => { description.Minimum = minimum; description.Maximum = maximum; });

            assetOptions = () => new List<OptionParsingState<AssetPropertyDescription>>()
            {
                { new OptionParsingState<AssetPropertyDescription>(false, "images".AsMemory(), description => description.AssetMode = AssetMode.Images) },
                { new OptionParsingState<AssetPropertyDescription>(false, Options.Required.ToString().AsMemory(), description => description.Required = true) }
            };

            textOptions = () => new List<OptionParsingState<TextPropertyDescription>>()
            {
                { new OptionParsingState<TextPropertyDescription>(false, Options.Required.ToString().AsMemory(), description => description.Required = true) }
            };

            textDetailedOptions = () => new List<DetailsParsingState<TextPropertyDescription>>()
            {
                { new DetailsParsingState<TextPropertyDescription>(false, "words".AsMemory(), (description, details) => {
                    if (details is not null)
                    {
                        var detailFound = false;

                        foreach (var detail in details)
                        {
                            if (detailFound)
                            {
                                throw new ParsingException($"Option 'words' can only have one value.");
                            }

                            var detailSpan = detail.Span;

                            if (int.TryParse(detailSpan, out var parsedDetail))
                            {
                                detailFound = true;
                                description.Words = parsedDetail;
                            }
                        }
                    }
                }) },
                { new DetailsParsingState<TextPropertyDescription>(false, "characters".AsMemory(), (description, details) => {
                    if (details is not null)
                    {
                        var detailFound = false;

                        foreach (var detail in details)
                        {
                            if (detailFound)
                            {
                                throw new ParsingException($"Option 'characters' can only have one value.");
                            }

                            var detailSpan = detail.Span;

                            if (int.TryParse(detailSpan, out var parsedDetail))
                            {
                                detailFound = true;
                                description.Characters = parsedDetail;
                            }
                        }
                    }
                }) },
            };

            richTextOptions = () => new List<OptionParsingState<RichTextPropertyDescription>>()
            {
                { new OptionParsingState<RichTextPropertyDescription>(false, "p".AsMemory(), description => description.P = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "b".AsMemory(), description => description.B = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "i".AsMemory(), description => description.I = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "a".AsMemory(), description => description.A = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h1".AsMemory(), description => description.H1 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h2".AsMemory(), description => description.H2 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h3".AsMemory(), description => description.H3 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h4".AsMemory(), description => description.H4 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "ul".AsMemory(), description => description.Ul = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "ol".AsMemory(), description => description.Ol = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "sup".AsMemory(), description => description.Sup = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "sub".AsMemory(), description => description.Sub = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "images".AsMemory(), description => description.Images = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, Options.Required.ToString().AsMemory(), description => description.Required = true) }
            };

            var richTextTablesDetails = new List<OptionParsingState<RichTextPropertyDescription>>()
            {
                { new OptionParsingState<RichTextPropertyDescription>(false, "p".AsMemory(), description => description.TablesP = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "b".AsMemory(), description => description.TablesB = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "i".AsMemory(), description => description.TablesI = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "a".AsMemory(), description => description.TablesA = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h1".AsMemory(), description => description.TablesH1 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h2".AsMemory(), description => description.TablesH2 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h3".AsMemory(), description => description.TablesH3 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "h4".AsMemory(), description => description.TablesH4 = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "ul".AsMemory(), description => description.TablesUl = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "ol".AsMemory(), description => description.TablesOl = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "sup".AsMemory(), description => description.TablesSup = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "sub".AsMemory(), description => description.TablesSub = true) },
                { new OptionParsingState<RichTextPropertyDescription>(false, "images".AsMemory(), description => description.TablesImages = true) },
            };

            richTextDetailedOptions = () => new List<DetailsParsingState<RichTextPropertyDescription>>()
            {
                { new DetailsParsingState<RichTextPropertyDescription>(false, "tables".AsMemory(), (description, details) => {
                    if (details is not null)
                    {
                        foreach (var detail in details)
                        {
                            var detailSpan = detail.Span;
                            var detailFound = false;

                            for (var detailIndex = 0; detailIndex < richTextTablesDetails.Count; detailIndex++)
                            {
                                var targetDetail = richTextTablesDetails[detailIndex];

                                if (detailSpan.SequenceEqual(targetDetail.Option.Span))
                                {
                                    if (targetDetail.Found)
                                    {
                                        throw new ParsingException($"Detail '{targetDetail.Option}' already has a value.");
                                    }

                                    targetDetail.Found = true;
                                    detailFound = true;
                                    targetDetail.Describe(description);
                                }

                                if (detailFound)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }) },
                { new DetailsParsingState<RichTextPropertyDescription>(false, "words".AsMemory(), (description, details) => {
                    if (details is not null)
                    {
                        var detailFound = false;

                        foreach (var detail in details)
                        {
                            if (detailFound)
                            {
                                throw new ParsingException($"Option 'words' can only have one value.");
                            }

                            var detailSpan = detail.Span;

                            if (int.TryParse(detailSpan, out var parsedDetail))
                            {
                                detailFound = true;
                                description.Words = parsedDetail;
                            }
                        }
                    }
                }) },
                { new DetailsParsingState<RichTextPropertyDescription>(false, "characters".AsMemory(), (description, details) => {
                    if (details is not null)
                    {
                        var detailFound = false;

                        foreach (var detail in details)
                        {
                            if (detailFound)
                            {
                                throw new ParsingException($"Option 'characters' can only have one value.");
                            }

                            var detailSpan = detail.Span;

                            if (int.TryParse(detailSpan, out var parsedDetail))
                            {
                                detailFound = true;
                                description.Characters = parsedDetail;
                            }
                        }
                    }
                }) },
            };
        }

        private enum ParseDetailPhase
        {
            Label,
            Details
        }

        private enum ParseRangePhase
        {
            Minimum,
            Maximum
        }

        public DescribesTypedProperty Describe(bool snippet, string identifier, Memory<char>[]? options, string? label)
        {
            var matchedProperty = MatchProperty(snippet, identifier, options);

            switch (matchedProperty.Description)
            {
                case nameof(TypePropertyDescription):
                    {
                        return new TypePropertyDescription(label, identifier);
                    }
                case nameof(SnippetTypePropertyDescription):
                    {
                        return new SnippetTypePropertyDescription(identifier);
                    }
                case nameof(LinkedItemsPropertyDescription):
                    {
                        var required = false;
                        var linkedTypeIds = new List<string>();

                        var linkedTypeIdBuffer = ArrayPool<char>.Shared.Rent(identifier.Length);
                        var linkedTypeIdBufferPointer = 0;

                        foreach (var character in identifier.AsSpan())
                        {
                            if (character == Options.ArrayDelimiter)
                            {
                                linkedTypeIds.Add(new string(linkedTypeIdBuffer, 0, linkedTypeIdBufferPointer));

                                linkedTypeIdBufferPointer = 0;
                            }
                            else
                            {
                                linkedTypeIdBuffer[linkedTypeIdBufferPointer++] = character;
                            }
                        }

                        linkedTypeIds.Add(new string(linkedTypeIdBuffer, 0, linkedTypeIdBufferPointer));

                        ArrayPool<char>.Shared.Return(linkedTypeIdBuffer);

                        var linkedItemsPropertyDescription = new LinkedItemsPropertyDescription(label)
                        {
                            Required = required,
                            LinkedTypeIds = linkedTypeIds.ToArray()
                        };

                        ParseOptions(options, linkedItemsPropertyDescription, targetRangeOption: rangeOption());

                        return linkedItemsPropertyDescription;
                    }
                case nameof(AssetPropertyDescription):
                    {
                        var assetPropertyDescription = new AssetPropertyDescription(label);

                        ParseOptions(options, assetPropertyDescription, assetOptions(), targetRangeOption: rangeOption());

                        return assetPropertyDescription;
                    }
                case nameof(TextPropertyDescription):
                    {
                        var textPropertyDescription = new TextPropertyDescription(label);

                        ParseOptions(options, textPropertyDescription, textOptions(), textDetailedOptions());

                        return textPropertyDescription;
                    }
                case nameof(DatePropertyDescription):
                    {
                        var datePropertyDescription = new DatePropertyDescription(label);

                        ParseOptions(options, datePropertyDescription, requiredOptions());

                        return datePropertyDescription;
                    }
                case nameof(NumberPropertyDescription):
                    {
                        var numberPropertyDescription = new NumberPropertyDescription(label);

                        ParseOptions(options, numberPropertyDescription, requiredOptions());

                        return numberPropertyDescription;
                    }
                case nameof(SingleChoicePropertyDescription):
                    {
                        var singleChoicePropertyDescription = new SingleChoicePropertyDescription(label);

                        var choiceOptions = ParseMultipleOptions(options, singleChoicePropertyDescription, requiredOptions());

                        singleChoicePropertyDescription.Options = choiceOptions;

                        return singleChoicePropertyDescription;
                    }
                case nameof(MultipleChoicePropertyDescription):
                    {
                        var multipleChoicePropertyDescription = new MultipleChoicePropertyDescription(label);

                        var choiceOptions = ParseMultipleOptions(options, multipleChoicePropertyDescription, requiredOptions());

                        multipleChoicePropertyDescription.Options = choiceOptions;

                        return multipleChoicePropertyDescription;
                    }
                case nameof(RichTextPropertyDescription):
                    {
                        var richTextPropertyDescription = new RichTextPropertyDescription(label);

                        ParseOptions(options, richTextPropertyDescription, richTextOptions(), richTextDetailedOptions());

                        return richTextPropertyDescription;
                    }
                case nameof(CustomPropertyDescription):
                    {
                        var customPropertyDescription = new CustomPropertyDescription(label);

                        ParseOptions(options, customPropertyDescription, requiredOptions());

                        return customPropertyDescription;
                    }
                default:
                    throw new ParsingException($"Description '{matchedProperty.Description}' not found.");
            }
        }

        public Property MatchProperty(bool snippet, string identifier, Memory<char>[]? options)
        {
            if (snippet)
            {
                return snippetType;
            }

            foreach (var property in Options.Properties)
            {
                if (property.Identifier == identifier)
                {
                    return property;
                }
            }

            if (options is null)
            {
                return noOptionsFallback;
            }
            else
            {
                return noPropertyFallback;
            }
        }

        public void ParseOptions<T>(
            Memory<char>[]? options,
            T description,
            IList<OptionParsingState<T>>? targetOptions = null,
            IList<DetailsParsingState<T>>? targetDetailedOptions = null,
            RangeParsingState<DescribesList>? targetRangeOption = null
            ) where T : DescribesAccessibleProperty
        {
            if (options is not null)
            {
                foreach (var option in options)
                {
                    switch (targetOptions, targetDetailedOptions, targetRangeOption)
                    {
                        case (not null, null, null):
                            {
                                ParseOption(description, option, targetOptions!);
                            }
                            break;

                        case (not null, null, not null) when description is DescribesList describesList:
                            {
                                if (ParseOption(description, option, targetOptions!))
                                {
                                    continue;
                                }

                                ParseRangeOption(describesList, option, targetRangeOption!);
                            }
                            break;

                        case (not null, not null, null):
                            {
                                if (ParseOption(description, option, targetOptions!))
                                {
                                    continue;
                                }

                                ParseDetailedOption(description, option, targetDetailedOptions!);
                            }
                            break;
                    }
                }
            }
        }

        public bool ParseOption<T>(
            T description,
            Memory<char> option,
            IList<OptionParsingState<T>> targetOptions
            ) where T : DescribesAccessibleProperty
        {
            var optionFound = false;

            for (var targetOptionIndex = 0; targetOptionIndex < targetOptions.Count; targetOptionIndex++)
            {
                var targetOption = targetOptions[targetOptionIndex];

                if (option.Span.SequenceEqual(targetOption.Option.Span))
                {
                    if (targetOption.Found)
                    {
                        throw new ParsingException($"Option '{targetOption.Option}' already has a value.");
                    }

                    targetOption.Found = true;
                    optionFound = true;
                    targetOption.Describe(description);
                }
            }

            return optionFound;
        }

        public void ParseDetailedOption<T>(
            T description,
            Memory<char> option,
            IList<DetailsParsingState<T>> targetDetailedOptions
            ) where T : DescribesAccessibleProperty
        {
            for (var targetDetailedOptionIndex = 0; targetDetailedOptionIndex < targetDetailedOptions.Count; targetDetailedOptionIndex++)
            {
                var targetDetailedOption = targetDetailedOptions[targetDetailedOptionIndex];

                var phase = ParseDetailPhase.Label;

                var breakFind = targetDetailedOption.Found;
                var continueFind = false;
                var labelBuffer = ArrayPool<char>.Shared.Rent(50);
                var labelBufferPointer = 0;

                var detailBuffer = ArrayPool<char>.Shared.Rent(100);
                var detailBufferPointer = 0;

                Memory<char> finalizeDetail()
                {
                    var optionArray = Array.CreateInstance(typeof(char), detailBufferPointer);

                    Array.Copy(detailBuffer, optionArray, detailBufferPointer);

                    return ((char[])optionArray).AsMemory();
                }

                var detailsBuffer = ArrayPool<Memory<char>>.Shared.Rent(50);
                var detailsBufferPointer = 0;
                Memory<char>[]? details = null;

                void finalizeDetailsArray()
                {
                    details = (Memory<char>[])Array.CreateInstance(typeof(Memory<char>), detailsBufferPointer);

                    for (var detailIndex = 0; detailIndex < detailsBufferPointer; detailIndex++)
                    {
                        details[detailIndex] = detailsBuffer[detailIndex];
                    }
                }

                var optionSpan = option.Span;
                var length = optionSpan.Length;

                for (var index = 0; index < length; index++)
                {
                    var current = optionSpan[index];

                    switch (phase)
                    {
                        case ParseDetailPhase.Label:
                            {
                                if (current == Options.OptionDetailStart)
                                {
                                    if (index == 0)
                                    {
                                        throw new ParsingException($"Found '{current}' during '{phase}' but not yet a label.", current, index);
                                    }
                                    else if (labelBuffer[..labelBufferPointer].AsSpan().SequenceEqual(targetDetailedOption.Option.Span))
                                    {
                                        if (targetDetailedOption.Found)
                                        {
                                            throw new ParsingException($"Option '{targetDetailedOption.Option}' already has a value.");
                                        }

                                        targetDetailedOption.Found = true;

                                        phase = ParseDetailPhase.Details;
                                    }
                                    else
                                    {
                                        continueFind = true;

                                        break;
                                    }

                                    continue;
                                }
                                else if (current == Options.OptionDetailEnd)
                                {
                                    throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                                }
                                else
                                {
                                    labelBuffer[labelBufferPointer++] = current;

                                    continue;
                                }
                            }
                        case ParseDetailPhase.Details:
                            {
                                if (current == Options.ArrayDelimiter)
                                {
                                    if (detailBufferPointer == 0)
                                    {
                                        throw new ParsingException($"Found '{current}' but yet no detail.", current, index);
                                    }
                                    else
                                    {
                                        detailsBuffer[detailsBufferPointer++] = finalizeDetail();
                                        detailBufferPointer = 0;

                                        continue;
                                    }
                                }
                                else if (current == Options.OptionDetailStart)
                                {
                                    throw new ParsingException($"Found '{current}' inside detail.", current, index);
                                }
                                else if (current == Options.OptionDetailEnd)
                                {
                                    if (detailsBufferPointer == 0)
                                    {
                                        if (detailBufferPointer == 0)
                                        {
                                            details = Array.Empty<Memory<char>>();
                                        }
                                        else
                                        {
                                            detailsBuffer[detailsBufferPointer++] = finalizeDetail();

                                            finalizeDetailsArray();
                                        }
                                    }
                                    else
                                    {
                                        detailsBuffer[detailsBufferPointer++] = finalizeDetail();

                                        finalizeDetailsArray();
                                    }

                                    detailBufferPointer = 0;
                                    detailsBufferPointer = 0;
                                    breakFind = true;

                                    break;
                                }
                                else
                                {
                                    if (breakFind)
                                    {
                                        throw new ParsingException($"Found '{current}' after detail.", current, index);
                                    }

                                    detailBuffer[detailBufferPointer++] = current;

                                    continue;
                                }
                            }
                    }

                    if (continueFind)
                    {
                        break;
                    }
                }

                targetDetailedOption.Describe(description, details);

                ArrayPool<char>.Shared.Return(labelBuffer);
                ArrayPool<char>.Shared.Return(detailBuffer);
                ArrayPool<Memory<char>>.Shared.Return(detailsBuffer);

                if (continueFind)
                {
                    continue;
                }

                if (breakFind)
                {
                    break;
                }
            }
        }

        public string[] ParseMultipleOptions<T>(
            Memory<char>[]? options,
            T description,
            IList<OptionParsingState<T>> targetOptions
            ) where T : DescribesAccessibleProperty
        {
            var parsedOptions = Array.Empty<string>();

            if (options is not null)
            {
                var optionsBuffer = ArrayPool<Memory<char>>.Shared.Rent(options.Length);
                var optionsBufferPointer = 0;

                foreach (var option in options)
                {
                    var optionFound = ParseOption(description, option, targetOptions);

                    if (optionFound)
                    {
                        continue;
                    }

                    optionsBuffer[optionsBufferPointer++] = option;
                }

                parsedOptions = (string[])Array.CreateInstance(typeof(string), optionsBufferPointer);

                for (var optionIndex = 0; optionIndex < optionsBufferPointer; optionIndex++)
                {
                    parsedOptions[optionIndex] = optionsBuffer[optionIndex].ToString();
                }

                ArrayPool<Memory<char>>.Shared.Return(optionsBuffer);
            }

            return parsedOptions;
        }

        public void ParseRangeOption(
            DescribesList description,
            Memory<char> option,
            RangeParsingState<DescribesList> targetRangeOption
            )
        {
            if (ParseRange(!targetRangeOption.Found, option, out var parsedMinimum, out var parsedMaximum))
            {
                targetRangeOption.Found = true;

                description.Minimum = parsedMinimum;
                description.Maximum = parsedMaximum;
            }
        }

        private bool ParseRange(bool valid, Memory<char> option, out int? minimum, out int? maximum)
        {
            var phase = ParseRangePhase.Minimum;

            var optionSpan = option.Span;
            var length = optionSpan.Length;

            var minimumBuffer = ArrayPool<char>.Shared.Rent(length);
            var minimumBufferPointer = 0;

            var maximumBuffer = ArrayPool<char>.Shared.Rent(length);
            var maximumBufferPointer = 0;

            minimum = null;
            maximum = null;

            for (var index = 0; index < length; index++)
            {
                var current = optionSpan[index];

                switch (phase)
                {
                    case ParseRangePhase.Minimum:
                        {
                            if (current == Options.MoreOrLess)
                            {
                                if (minimumBufferPointer == 0)
                                {
                                    phase = ParseRangePhase.Maximum;

                                    continue;
                                }
                                else
                                {
                                    if (int.TryParse(minimumBuffer.AsSpan()[0..minimumBufferPointer], out var parsedMininum))
                                    {
                                        if (!valid)
                                        {
                                            throw new ParsingException($"Option '{option}' is already set.");
                                        }

                                        minimum = parsedMininum;

                                        return true;
                                    }

                                    return false;
                                }
                            }
                            else if (current == Options.Range)
                            {
                                if (minimumBufferPointer == 0)
                                {
                                    throw new ParsingException($"Found '{current}' before minimum during '{nameof(ParseRangePhase.Minimum)}'.", current, index);
                                }
                                else
                                {
                                    if (int.TryParse(minimumBuffer.AsSpan()[0..minimumBufferPointer], out var parsedMininum))
                                    {
                                        minimum = parsedMininum;
                                    }

                                    phase = ParseRangePhase.Maximum;

                                    continue;
                                }
                            }
                            else
                            {
                                minimumBuffer[minimumBufferPointer++] = current;

                                if (index + 1 == length)
                                {
                                    if (int.TryParse(minimumBuffer.AsSpan()[0..minimumBufferPointer], out var parsedMininum))
                                    {
                                        if (!valid)
                                        {
                                            throw new ParsingException($"Option '{option}' is already set.");
                                        }

                                        minimum = parsedMininum;
                                        maximum = minimum;

                                        return true;
                                    }

                                    return false;
                                }

                                continue;
                            }
                        }
                    case ParseRangePhase.Maximum:
                        {
                            maximumBuffer[maximumBufferPointer++] = current;

                            if (index + 1 == length)
                            {
                                if (int.TryParse(maximumBuffer.AsSpan()[0..maximumBufferPointer], out var parsedMaximum))
                                {
                                    if (!valid)
                                    {
                                        throw new ParsingException($"Option '{option}' is already set.");
                                    }

                                    maximum = parsedMaximum;

                                    return true;
                                }

                                return false;
                            }

                            continue;
                        }
                }
            }

            ArrayPool<char>.Shared.Return(minimumBuffer);
            ArrayPool<char>.Shared.Return(maximumBuffer);

            return false;
        }
    }
}