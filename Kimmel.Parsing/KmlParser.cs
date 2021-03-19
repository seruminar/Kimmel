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
    public class KmlParser : IKmlParser
    {
        private readonly IPropertyDescriber propertyDescriber;
        private readonly ITypeDescriber typeDescriber;
        private readonly string typeStart;

        private enum ParseKmlPhase
        {
            LeadingWhiteSpace,
            Comments,
            SnippetStart,
            Identifier,
            Options,
            TrailingWhiteSpace,
            Label,
            Property,
            PropertyDelimiter
        }

        public ReadOnlyOptions Options { get; }

        public KmlParser(
            Options options,
            IPropertyDescriber propertyDescriber,
            ITypeDescriber typeDescriber
            )
        {
            Options = new ReadOnlyOptions(options);

            this.propertyDescriber = propertyDescriber;
            this.typeDescriber = typeDescriber;

            typeStart = Options.TypeStart;
        }

        public Kml ParseKml(string kml)
        {
            var phase = ParseKmlPhase.LeadingWhiteSpace;

            var commentsPointer = 0;
            var snippetStartPointer = 0;
            var propertyDelimiterPointer = 0;
            var spaceCounter = 0;
            var finished = false;

            var identifierBuffer = ArrayPool<char>.Shared.Rent(50);
            var identifierBufferPointer = 0;
            string? identifier = null;

            void finalizeIdentifier() => identifier = new string(identifierBuffer, 0, identifierBufferPointer);

            var optionBuffer = ArrayPool<char>.Shared.Rent(100);
            var optionBufferPointer = 0;
            var inOptionDetail = false;

            Memory<char> finalizeOption()
            {
                var optionArray = Array.CreateInstance(typeof(char), optionBufferPointer - spaceCounter);

                Array.Copy(optionBuffer, optionArray, optionBufferPointer - spaceCounter);

                spaceCounter = 0;

                return ((char[])optionArray).AsMemory();
            }

            var optionsBuffer = ArrayPool<Memory<char>>.Shared.Rent(50);
            var optionsBufferPointer = 0;
            var inOptions = false;
            Memory<char>[]? options = null;

            void finalizeOptionsArray()
            {
                options = (Memory<char>[])Array.CreateInstance(typeof(Memory<char>), optionsBufferPointer);

                for (var optionIndex = 0; optionIndex < optionsBufferPointer; optionIndex++)
                {
                    options[optionIndex] = optionsBuffer[optionIndex];
                }
            }

            var labelBuffer = ArrayPool<char>.Shared.Rent(50);
            var labelBufferPointer = 0;
            string? label = null;

            void finalizeLabel()
            {
                label = new string(labelBuffer, 0, labelBufferPointer - spaceCounter);

                spaceCounter = 0;
            }

            var propertyDescriptions = new List<DescribesTypedProperty>();

            RichTextPropertyDescription? richTextParent = null;
            var componentBlacklist = new HashSet<string>();
            var componentTypeIds = ArrayPool<string>.Shared.Rent(100);
            var componentTypeIdsPointer = 0;

            var typeDescriptions = new Dictionary<string, TypeDescription>();

            var linkedTypeIds = new HashSet<string>();
            var snippetTypeIds = new HashSet<string>();

            void tryAddType(TypeDescription typeDescription)
            {
                if (!typeDescriptions.TryAdd(typeDescription.Id, typeDescription))
                {
                    switch (Options.Mode)
                    {
                        case KmlParserMode.Strict:
                            throw new ParsingException($"'{typeDescription.Id}' is already described.");
                        case KmlParserMode.Loose:
                            var clonedTypeDescription = typeDescriber.CloneType(typeDescription);

                            typeDescriptions.Add(clonedTypeDescription.Id, clonedTypeDescription);
                            break;
                    }
                }
            }

            void finalizeRichTextParent()
            {
                if (richTextParent is not null && componentTypeIdsPointer > 0)
                {
                    richTextParent.Components = true;
                    richTextParent.ComponentTypeIds = (string[])Array.CreateInstance(typeof(string), componentTypeIdsPointer);

                    Array.Copy(componentTypeIds, richTextParent.ComponentTypeIds, componentTypeIdsPointer);

                    richTextParent = null;
                    componentTypeIdsPointer = 0;
                }
            }

            var kmlSpan = kml.AsMemory().Span;
            var length = kmlSpan.Length;

            for (var index = 0; index < length; index++)
            {
                var current = kmlSpan[index];
                var skip = false;

                foreach (var skipCharacter in Options.Skip.Span)
                {
                    if (current == skipCharacter)
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }

                switch (phase)
                {
                    case ParseKmlPhase.LeadingWhiteSpace:
                        {
                            if (commentsPointer == 0 && current == Options.Comments.Span[commentsPointer])
                            {
                                phase = ParseKmlPhase.Comments;

                                goto case ParseKmlPhase.Comments;
                            }
                            else if (current == Options.Space)
                            {
                                continue;
                            }
                            else if (current == Options.ArrayDelimiter)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionsStart)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionsEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailStart)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (snippetStartPointer == 0 && current == Options.SnippetStart.Span[snippetStartPointer])
                            {
                                finalizeRichTextParent();

                                phase = ParseKmlPhase.SnippetStart;

                                goto case ParseKmlPhase.SnippetStart;
                            }
                            else if (propertyDelimiterPointer == 0 && current == Options.PropertyDelimiter.Span[propertyDelimiterPointer])
                            {
                                finalizeRichTextParent();

                                phase = ParseKmlPhase.PropertyDelimiter;

                                goto case ParseKmlPhase.PropertyDelimiter;
                            }
                            else
                            {
                                phase = ParseKmlPhase.Identifier;

                                goto case ParseKmlPhase.Identifier;
                            }
                        }
                    case ParseKmlPhase.Comments:
                        {
                            if (propertyDelimiterPointer == 0 && current == Options.PropertyDelimiter.Span[propertyDelimiterPointer])
                            {
                                commentsPointer = 0;
                                phase = ParseKmlPhase.PropertyDelimiter;

                                goto case ParseKmlPhase.PropertyDelimiter;
                            }
                            else if (commentsPointer == Options.Comments.Span.Length - 1)
                            {
                                continue;
                            }
                            else if (commentsPointer > 0
                                && commentsPointer < Options.Comments.Span.Length
                                && current != Options.Comments.Span[commentsPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else
                            {
                                commentsPointer++;

                                continue;
                            }
                        }
                    case ParseKmlPhase.SnippetStart:
                        {
                            if (snippetStartPointer == Options.SnippetStart.Span.Length - 1)
                            {
                                phase = ParseKmlPhase.Identifier;

                                continue;
                            }
                            else if (snippetStartPointer > 0
                                && snippetStartPointer < Options.SnippetStart.Span.Length
                                && current != Options.SnippetStart.Span[snippetStartPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else
                            {
                                snippetStartPointer++;

                                continue;
                            }
                        }
                    case ParseKmlPhase.Identifier:
                        {
                            if (commentsPointer == 0 && current == Options.Comments.Span[commentsPointer])
                            {
                                finalizeIdentifier();

                                phase = ParseKmlPhase.Comments;

                                goto case ParseKmlPhase.Comments;
                            }
                            else if (current == Options.Space)
                            {
                                finalizeIdentifier();

                                phase = ParseKmlPhase.TrailingWhiteSpace;

                                if (index + 1 == length)
                                {
                                    finished = true;

                                    goto case ParseKmlPhase.Property;
                                }

                                continue;
                            }
                            else if (current == Options.OptionsStart)
                            {
                                finalizeIdentifier();

                                phase = ParseKmlPhase.Options;

                                continue;
                            }
                            else if (current == Options.ArrayDelimiter)
                            {
                                identifierBuffer[identifierBufferPointer++] = current;

                                continue;
                            }
                            else if (current == Options.OptionsEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailStart)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (snippetStartPointer == 0 && current == Options.SnippetStart.Span[snippetStartPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (propertyDelimiterPointer == 0 && current == Options.PropertyDelimiter.Span[propertyDelimiterPointer])
                            {
                                finalizeIdentifier();

                                phase = ParseKmlPhase.PropertyDelimiter;

                                goto case ParseKmlPhase.PropertyDelimiter;
                            }
                            else if (index + 1 == length)
                            {
                                identifierBuffer[identifierBufferPointer++] = current;

                                finalizeIdentifier();

                                finished = true;

                                goto case ParseKmlPhase.Property;
                            }
                            else
                            {
                                identifierBuffer[identifierBufferPointer++] = current;

                                continue;
                            }
                        }
                    case ParseKmlPhase.Options:
                        {
                            if (commentsPointer == 0 && current == Options.Comments.Span[commentsPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.Space)
                            {
                                spaceCounter++;
                                optionBuffer[optionBufferPointer++] = current;

                                continue;
                            }
                            else if (current == Options.ArrayDelimiter)
                            {
                                if (optionBufferPointer == 0)
                                {
                                    throw new ParsingException($"Found '{current}' but yet no option.", current, index);
                                }
                                else
                                {
                                    if (inOptionDetail)
                                    {
                                        optionBuffer[optionBufferPointer++] = current;
                                        spaceCounter = 0;

                                        continue;
                                    }

                                    optionsBuffer[optionsBufferPointer++] = finalizeOption();
                                    optionBufferPointer = 0;

                                    continue;
                                }
                            }
                            else if (current == Options.OptionsStart)
                            {
                                if (inOptions)
                                {
                                    throw new ParsingException($"Found '{current}' inside option detail.", current, index);
                                }
                                else
                                {
                                    inOptions = true;
                                    continue;
                                }
                            }
                            else if (current == Options.OptionsEnd)
                            {
                                if (optionsBufferPointer == 0)
                                {
                                    if (optionBufferPointer == 0)
                                    {
                                        options = Array.Empty<Memory<char>>();
                                    }
                                    else
                                    {
                                        optionsBuffer[optionsBufferPointer++] = finalizeOption();

                                        finalizeOptionsArray();
                                    }
                                }
                                else
                                {
                                    optionsBuffer[optionsBufferPointer++] = finalizeOption();

                                    finalizeOptionsArray();
                                }

                                optionBufferPointer = 0;
                                optionsBufferPointer = 0;
                                inOptions = false;
                                phase = ParseKmlPhase.TrailingWhiteSpace;

                                continue;
                            }
                            else if (current == Options.OptionDetailStart)
                            {
                                if (inOptionDetail)
                                {
                                    throw new ParsingException($"Found '{current}' inside option detail.", current, index);
                                }
                                else
                                {
                                    inOptionDetail = true;
                                    optionBuffer[optionBufferPointer++] = current;
                                    spaceCounter = 0;
                                    continue;
                                }
                            }
                            else if (current == Options.OptionDetailEnd)
                            {
                                if (inOptionDetail)
                                {
                                    inOptionDetail = false;
                                    optionBuffer[optionBufferPointer++] = current;
                                    spaceCounter = 0;
                                    continue;
                                }
                                else
                                {
                                    throw new ParsingException($"Found '{current}' outside option detail.", current, index);
                                }
                            }
                            else if (snippetStartPointer == 0 && current == Options.SnippetStart.Span[snippetStartPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (propertyDelimiterPointer == 0 && current == Options.PropertyDelimiter.Span[propertyDelimiterPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (index + 1 == length)
                            {
                                if (inOptions)
                                {
                                    throw new ParsingException($"Reached end during '{phase}'.", current, index);
                                }

                                finished = true;

                                goto case ParseKmlPhase.Property;
                            }
                            else
                            {
                                if (!inOptions)
                                {
                                    inOptions = true;
                                }

                                optionBuffer[optionBufferPointer++] = current;
                                spaceCounter = 0;

                                continue;
                            }
                        }
                    case ParseKmlPhase.TrailingWhiteSpace:
                        {
                            if (commentsPointer == 0 && current == Options.Comments.Span[commentsPointer])
                            {
                                phase = ParseKmlPhase.Comments;

                                goto case ParseKmlPhase.Comments;
                            }
                            else if (index + 1 == length)
                            {
                                finished = true;

                                goto case ParseKmlPhase.Property;
                            }
                            else if (current == Options.Space)
                            {
                                continue;
                            }
                            else if (current == Options.ArrayDelimiter)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionsStart)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionsEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailStart)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (snippetStartPointer == 0 && current == Options.SnippetStart.Span[snippetStartPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (propertyDelimiterPointer == 0 && current == Options.PropertyDelimiter.Span[propertyDelimiterPointer])
                            {
                                phase = ParseKmlPhase.PropertyDelimiter;

                                goto case ParseKmlPhase.PropertyDelimiter;
                            }
                            else if (snippetStartPointer > 0)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}' after snippet identified.", current, index);
                            }
                            else
                            {
                                phase = ParseKmlPhase.Label;

                                goto case ParseKmlPhase.Label;
                            }
                        }
                    case ParseKmlPhase.Label:
                        {
                            if (commentsPointer == 0 && current == Options.Comments.Span[commentsPointer])
                            {
                                if (labelBufferPointer > 0)
                                {
                                    finalizeLabel();
                                }

                                phase = ParseKmlPhase.Comments;

                                goto case ParseKmlPhase.Comments;
                            }
                            else if (current == Options.Space)
                            {
                                spaceCounter++;
                                labelBuffer[labelBufferPointer++] = current;

                                continue;
                            }
                            else if (current == Options.ArrayDelimiter)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionsStart)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionsEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailStart)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (current == Options.OptionDetailEnd)
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else if (propertyDelimiterPointer == 0 && current == Options.PropertyDelimiter.Span[propertyDelimiterPointer])
                            {
                                if (labelBufferPointer > 0)
                                {
                                    finalizeLabel();
                                }

                                phase = ParseKmlPhase.PropertyDelimiter;

                                goto case ParseKmlPhase.PropertyDelimiter;
                            }
                            else if (index + 1 == length)
                            {
                                labelBuffer[labelBufferPointer++] = current;

                                finalizeLabel();

                                finished = true;

                                goto case ParseKmlPhase.Property;
                            }
                            else
                            {
                                labelBuffer[labelBufferPointer++] = current;

                                spaceCounter = 0;

                                continue;
                            }
                        }
                    case ParseKmlPhase.PropertyDelimiter:
                        {
                            if (propertyDelimiterPointer == Options.PropertyDelimiter.Span.Length - 1)
                            {
                                propertyDelimiterPointer = 0;

                                if (identifierBufferPointer > 0)
                                {
                                    phase = ParseKmlPhase.Property;

                                    goto case ParseKmlPhase.Property;
                                }
                                else
                                {
                                    phase = ParseKmlPhase.LeadingWhiteSpace;

                                    continue;
                                }
                            }
                            else if (propertyDelimiterPointer > 0
                                && propertyDelimiterPointer < Options.PropertyDelimiter.Span.Length
                                && current != Options.PropertyDelimiter.Span[propertyDelimiterPointer])
                            {
                                throw new ParsingException($"Found '{current}' during '{phase}'.", current, index);
                            }
                            else
                            {
                                propertyDelimiterPointer++;

                                continue;
                            }
                        }
                    case ParseKmlPhase.Property:
                        {
                            if (identifier is null)
                            {
                                continue;
                            }

                            var propertyDescription = propertyDescriber.Describe(snippetStartPointer > 0, identifier, options, label);

                            switch (propertyDescription)
                            {
                                case LinkedItemsPropertyDescription linkedItemsPropertyDescription:
                                    foreach (var linkedTypeId in linkedItemsPropertyDescription.LinkedTypeIds)
                                    {
                                        linkedTypeIds.Add(linkedTypeId);
                                    }

                                    goto default;

                                case TypePropertyDescription typePropertyDescription:
                                    if (richTextParent is not null)
                                    {
                                        if (string.IsNullOrWhiteSpace(label))
                                        {
                                            componentTypeIds[componentTypeIdsPointer++] = typePropertyDescription.Id;
                                            componentBlacklist.Add(typePropertyDescription.Id);
                                            linkedTypeIds.Add(typePropertyDescription.Id);
                                        }
                                        else
                                        {
                                            goto default;
                                        }
                                    }

                                    break;

                                case SnippetTypePropertyDescription snippetTypePropertyDescription:
                                    snippetTypeIds.Add(snippetTypePropertyDescription.Id);

                                    break;

                                default:
                                    finalizeRichTextParent();

                                    if (propertyDescription is RichTextPropertyDescription richTextPropertyDescription)
                                    {
                                        richTextParent = richTextPropertyDescription;
                                    }

                                    break;
                            }

                            propertyDescriptions.Add(propertyDescription);

                            if (finished)
                            {
                                finalizeRichTextParent();

                                break;
                            }

                            snippetStartPointer = 0;
                            identifier = null;
                            identifierBufferPointer = 0;
                            options = null;
                            optionsBufferPointer = 0;
                            label = null;
                            labelBufferPointer = 0;

                            phase = ParseKmlPhase.LeadingWhiteSpace;

                            continue;
                        }
                }

                if (finished)
                {
                    break;
                }

                continue;
            }

            if (propertyDescriptions.Count == 0)
            {
                return new Kml(
                    Array.Empty<TypeDescription>(),
                    Array.Empty<TypeDescription>(),
                    GetStatistics(Array.Empty<TypeDescription>())
                    );
            }

            var typeBuffer = ArrayPool<DescribesTypedProperty>.Shared.Rent(propertyDescriptions.Count);
            var typeBufferPointer = 0;
            var typeLinkedTypeIds = new HashSet<string>();
            var typeSnippetTypeIds = new HashSet<string>();
            var richTextStart = false;

            foreach (var propertyDescription in propertyDescriptions)
            {
                var addProperty = true;

                switch (propertyDescription)
                {
                    case TypePropertyDescription typePropertyDescription:
                        if (typeBufferPointer == 0)
                        {
                            break;
                        }
                        else
                        {
                            if (richTextStart && componentBlacklist.Contains(typePropertyDescription.Id))
                            {
                                addProperty = false;
                                break;
                            }

                            var typeDescription = typeDescriber.DescribeType(
                                typeBuffer,
                                typeBufferPointer,
                                typeLinkedTypeIds,
                                typeSnippetTypeIds
                                );

                            tryAddType(typeDescription);

                            typeBufferPointer = 0;
                            typeLinkedTypeIds = new HashSet<string>();
                            typeSnippetTypeIds = new HashSet<string>();
                            richTextStart = false;

                            break;
                        }
                    case SnippetTypePropertyDescription snippetTypePropertyDescription:
                        typeSnippetTypeIds.Add(snippetTypePropertyDescription.Id);

                        break;

                    case RichTextPropertyDescription richTextPropertyDescription:
                        richTextStart = true;

                        break;

                    case LinkedItemsPropertyDescription linkedItemsPropertyDescription:
                        foreach (var linkedTypeId in linkedItemsPropertyDescription.LinkedTypeIds)
                        {
                            typeLinkedTypeIds.Add(linkedTypeId);
                        }

                        goto default;

                    default:
                        richTextStart = false;

                        if (typeBufferPointer == 0)
                        {
                            throw new ParsingException($"Description with name '{propertyDescription.Type}' is not the {nameof(typeStart)} '{typeStart}'.");
                        }

                        break;
                }

                if (addProperty)
                {
                    typeBuffer[typeBufferPointer++] = propertyDescription;
                }
            }

            {
                var typeDescription = typeDescriber.DescribeType(
                    typeBuffer,
                    typeBufferPointer,
                    typeLinkedTypeIds,
                    typeSnippetTypeIds
                    );

                tryAddType(typeDescription);
            }

            var finalTypeDescriptions = new List<TypeDescription>(typeDescriptions.Count);
            var finalSnippetTypeDescriptions = new List<TypeDescription>(typeDescriptions.Count);

            switch (Options.Mode)
            {
                case KmlParserMode.Strict:
                    {
                        foreach (var linkedTypeId in linkedTypeIds)
                        {
                            if (snippetTypeIds.Contains(linkedTypeId))
                            {
                                throw new ParsingException($"Link '{linkedTypeId}' is already described as a snippet.");
                            }

                            if (!typeDescriptions.ContainsKey(linkedTypeId))
                            {
                                throw new ParsingException($"Link '{linkedTypeId}' is undescribed.");
                            }
                        }

                        foreach (var snippetTypeId in snippetTypeIds)
                        {
                            if (!typeDescriptions.ContainsKey(snippetTypeId))
                            {
                                throw new ParsingException($"Snippet '{snippetTypeId}' is undescribed.");
                            }
                        }

                        foreach (var typeDescription in typeDescriptions.Values)
                        {
                            typeDescriber.SetTypeLinks(typeDescription);

                            if (snippetTypeIds.Contains(typeDescription.Id))
                            {
                                foreach (var propertyDescription in typeDescription.PropertyDescriptions)
                                {
                                    switch (propertyDescription)
                                    {
                                        case SnippetTypePropertyDescription snippetTypePropertyDescription:
                                            throw new ParsingException($"Snippet with ID '{snippetTypePropertyDescription.Id}' cannot be in snippet type '{typeDescription.Id}'.");
                                    }
                                }

                                finalSnippetTypeDescriptions.Add(typeDescription);
                            }
                            else
                            {
                                finalTypeDescriptions.Add(typeDescription);
                            }
                        }

                        break;
                    }
                case KmlParserMode.Loose:
                    {
                        var updateLinkedReferenceMap = new Dictionary<string, string>();

                        foreach (var linkedTypeId in linkedTypeIds)
                        {
                            var existsAsSnippet = snippetTypeIds.Contains(linkedTypeId);

                            if (typeDescriptions.TryGetValue(linkedTypeId, out var typeDescription))
                            {
                                if (existsAsSnippet)
                                {
                                    var clonedTypeDescription = typeDescriber.CloneType(typeDescription);

                                    tryAddType(clonedTypeDescription);

                                    updateLinkedReferenceMap.Add(linkedTypeId, clonedTypeDescription.Id);
                                }
                            }
                            else
                            {
                                if (existsAsSnippet)
                                {
                                    var clonedNewTypeDescription = typeDescriber.CloneType(typeDescriber.DescribeEmptyType(linkedTypeId));

                                    tryAddType(clonedNewTypeDescription);

                                    updateLinkedReferenceMap.Add(linkedTypeId, clonedNewTypeDescription.Id);
                                }
                                else
                                {
                                    tryAddType(typeDescriber.DescribeEmptyType(linkedTypeId));
                                }
                            }
                        }

                        foreach (var snippetTypeId in snippetTypeIds)
                        {
                            if (!typeDescriptions.ContainsKey(snippetTypeId))
                            {
                                tryAddType(typeDescriber.DescribeEmptyType(snippetTypeId));
                            }
                        }

                        foreach (var typeDescription in typeDescriptions.Values)
                        {
                            typeDescriber.SetTypeLinks(typeDescription, updateLinkedReferenceMap);

                            if (snippetTypeIds.Contains(typeDescription.Id))
                            {
                                foreach (var propertyDescription in typeDescription.PropertyDescriptions)
                                {
                                    switch (propertyDescription)
                                    {
                                        case SnippetTypePropertyDescription snippetTypePropertyDescription:
                                            throw new ParsingException($"Snippet with ID '{snippetTypePropertyDescription.Id}' cannot be in snippet type '{typeDescription.Id}'.");
                                    }
                                }

                                finalSnippetTypeDescriptions.Add(typeDescription);
                            }
                            else
                            {
                                finalTypeDescriptions.Add(typeDescription);
                            }
                        }

                        break;
                    }
            }

            ArrayPool<char>.Shared.Return(identifierBuffer);
            ArrayPool<char>.Shared.Return(optionBuffer);
            ArrayPool<Memory<char>>.Shared.Return(optionsBuffer);
            ArrayPool<char>.Shared.Return(labelBuffer);
            ArrayPool<string>.Shared.Return(componentTypeIds);
            ArrayPool<DescribesTypedProperty>.Shared.Return(typeBuffer);

            return new Kml(
                finalTypeDescriptions,
                finalSnippetTypeDescriptions,
                GetStatistics(finalTypeDescriptions)
                );
        }

        public KmlStatistics GetStatistics(ICollection<TypeDescription> typeDescriptions)
        {
            var chains = new List<ICollection<string>>(typeDescriptions.Count);
            var closedChains = new List<ICollection<string>>(typeDescriptions.Count);
            var chainQueue = new Queue<(ICollection<string> chain, TypeDescription type)>(typeDescriptions.Count);

            foreach (var type in typeDescriptions)
            {
                var chain = new List<string>(type.LinkedTypeIds.Count + 1)
                {
                    type.Id
                };

                chains.Add(chain);
                chainQueue.Enqueue((chain, type));
            }

            while (chainQueue.TryDequeue(out var chain))
            {
                foreach (var link in chain.type.LinkedTypeIds)
                {
                    var closedChain = false;

                    foreach (var chainLink in chain.chain)
                    {
                        if (chainLink == link)
                        {
                            closedChain = true;

                            break;
                        }
                    }

                    var clone = new List<string>(chain.chain)
                    {
                        link
                    };

                    if (closedChain)
                    {
                        closedChains.Add(clone);

                        break;
                    }
                    else
                    {
                        chains.Add(clone);

                        foreach (var type in typeDescriptions)
                        {
                            if (type.Id == link)
                            {
                                chainQueue.Enqueue((clone, type));

                                break;
                            }
                        }
                    }
                }
            }

            var maxDepth = 0;

            foreach (var chain in chains)
            {
                if (chain.Count > maxDepth)
                {
                    maxDepth = chain.Count;
                }
            }

            return new KmlStatistics()
            {
                TotalTypes = typeDescriptions.Count,
                Chains = chains.Select(chain => string.Join('>', chain)),
                ClosedChains = closedChains.Select(chain => string.Join('>', chain)),
                MaxChainDepth = maxDepth
            };
        }
    }
}