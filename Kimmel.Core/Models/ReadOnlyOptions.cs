using System;
using System.Collections.Generic;

using Kimmel.Core.Exceptions;
using Kimmel.Core.Parsing.Models;

namespace Kimmel.Core.Models
{
    public record ReadOnlyOptions
    {
        public KmlParserMode Mode { get; set; }

        public Memory<char> Comments { get; }

        public char Space { get; set; }

        public char ArrayDelimiter { get; }

        public char OptionsStart { get; }

        public char OptionsEnd { get; }

        public char OptionDetailStart { get; set; }

        public char OptionDetailEnd { get; set; }

        public char MoreOrLess { get; }

        public char Range { get; }

        public char Required { get; }

        public Memory<char> PropertyDelimiter { get; }

        public Memory<char> SnippetStart { get; }

        public List<Property> Properties { get; }

        public string NoPropertyFallback { get; }

        public string NoOptionsFallback { get; }

        public string SnippetType { get; }

        public string TypeStart { get; }
        public Memory<char> Skip { get; }

        /// <exception cref="PropertyNullException"/>
        public ReadOnlyOptions(Options options)
        {
            if (Enum.TryParse<KmlParserMode>(options.Mode, out var mode))
            {
                Mode = mode;
            }
            else
            {
                throw new PropertyNullException(nameof(options.Mode));
            }

            Comments = options.Comments?.ToCharArray().AsMemory() ?? throw new PropertyNullException(nameof(options.Comments));
            Space = options.Space ?? throw new PropertyNullException(nameof(options.Space));
            ArrayDelimiter = options.ArrayDelimiter ?? throw new PropertyNullException(nameof(options.ArrayDelimiter));
            OptionsStart = options.OptionsStart ?? throw new PropertyNullException(nameof(options.OptionsStart));
            OptionsEnd = options.OptionsEnd ?? throw new PropertyNullException(nameof(options.OptionsEnd));
            OptionDetailStart = options.OptionDetailStart ?? throw new PropertyNullException(nameof(options.OptionDetailStart));
            OptionDetailEnd = options.OptionDetailEnd ?? throw new PropertyNullException(nameof(options.OptionDetailEnd));
            MoreOrLess = options.MoreOrLess ?? throw new PropertyNullException(nameof(options.MoreOrLess));
            Range = options.Range ?? throw new PropertyNullException(nameof(options.Range));
            Required = options.Required ?? throw new PropertyNullException(nameof(options.Required));
            PropertyDelimiter = options.PropertyDelimiter?.ToCharArray().AsMemory() ?? throw new PropertyNullException(nameof(options.PropertyDelimiter));
            SnippetStart = options.SnippetStart?.ToCharArray().AsMemory() ?? throw new PropertyNullException(nameof(options.SnippetStart));
            Properties = options.Properties ?? throw new PropertyNullException(nameof(options.Properties));
            NoPropertyFallback = options.NoPropertyFallback ?? throw new PropertyNullException(nameof(options.NoPropertyFallback));
            NoOptionsFallback = options.NoOptionsFallback ?? throw new PropertyNullException(nameof(options.NoOptionsFallback));
            SnippetType = options.SnippetType ?? throw new PropertyNullException(nameof(options.SnippetType));
            TypeStart = options.TypeStart ?? throw new PropertyNullException(nameof(options.TypeStart));
            Skip = options.Skip?.ToCharArray().AsMemory() ?? throw new PropertyNullException(nameof(options.Skip));
        }
    }
}