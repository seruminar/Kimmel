using System;

using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Parsing.Models
{
    public class RangeParsingState<T> where T : DescribesAccessibleProperty
    {
        public bool Found { get; set; }

        public Action<T, int, int> Describe { get; set; }

        public RangeParsingState(
            bool found,
            Action<T, int, int> describe
            )
        {
            Found = found;
            Describe = describe;
        }
    }
}