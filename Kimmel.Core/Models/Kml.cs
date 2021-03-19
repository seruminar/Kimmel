using System.Collections.Generic;

using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Models
{
    public class Kml
    {
        public IList<TypeDescription> TypeDescriptions { get; }

        public IList<TypeDescription> SnippetTypeDescriptions { get; }

        public KmlStatistics Stats { get; }

        public Kml(
            IList<TypeDescription> typeDescriptions,
            IList<TypeDescription> snippetTypeDescriptions,
            KmlStatistics stats
            )
        {
            TypeDescriptions = typeDescriptions;
            SnippetTypeDescriptions = snippetTypeDescriptions;
            Stats = stats;
        }
    }
}