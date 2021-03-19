using System;

using Kimmel.Core.Parsing.Models;

namespace Functions.Models
{
    public class ParseRequest
    {
        public string? Kml { get; set; }

        public KmlParserMode Mode { get; set; }

        internal void Deconstruct(out string kml, out KmlParserMode mode)
        {
            kml = Kml ?? throw new ArgumentNullException(nameof(Kml));
            mode = Mode;
        }
    }
}