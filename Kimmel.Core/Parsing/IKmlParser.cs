using System.Collections.Generic;

using Kimmel.Core.Models;
using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Parsing
{
    public interface IKmlParser
    {
        ReadOnlyOptions Options { get; }

        Kml ParseKml(string kml);

        KmlStatistics GetStatistics(ICollection<TypeDescription> typeDescriptions);
    }
}