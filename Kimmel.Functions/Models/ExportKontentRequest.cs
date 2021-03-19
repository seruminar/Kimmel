using System;

using Kimmel.Core.Parsing.Models;

namespace Functions.Models
{
    public class ExportKontentRequest
    {
        public string Kml { get; set; } = string.Empty;

        public string ManagementApiKey { get; set; } = string.Empty;

        public KmlParserMode Mode { get; set; }

        internal void Deconstruct(
            out string kml,
            out string managementApiKey,
            out KmlParserMode mode
            )
        {
            kml = Kml ?? throw new ArgumentNullException(nameof(Kml));
            managementApiKey = ManagementApiKey ?? throw new ArgumentNullException(nameof(ManagementApiKey));
            mode = Mode;
        }
    }
}