using System.Collections.Generic;
using System.Linq;

namespace Kimmel.Core.Models
{
    public class KmlStatistics
    {
        public int TotalTypes { get; set; }

        public IEnumerable<string> Chains { get; set; } = Enumerable.Empty<string>();

        public IEnumerable<string> ClosedChains { get; set; } = Enumerable.Empty<string>();

        public int? MaxChainDepth { get; set; }
    }
}