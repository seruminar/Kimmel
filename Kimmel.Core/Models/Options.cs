using System.Collections.Generic;

namespace Kimmel.Core.Models
{
    public class Options
    {
        public string? Mode { get; set; }

        public string? Comments { get; set; }

        public char? Space { get; set; }

        public char? ArrayDelimiter { get; set; }

        public char? OptionsStart { get; set; }

        public char? OptionsEnd { get; set; }

        public char? OptionDetailStart { get; set; }

        public char? OptionDetailEnd { get; set; }

        public char? MoreOrLess { get; set; }

        public char? Range { get; set; }

        public char? Required { get; set; }

        public string? PropertyDelimiter { get; set; }

        public string? SnippetStart { get; set; }

        public List<Property>? Properties { get; set; }

        public string? NoPropertyFallback { get; set; }

        public string? NoOptionsFallback { get; set; }

        public string? SnippetType { get; set; }

        public string? TypeStart { get; set; }

        public string? Skip { get; set; }
    }
}