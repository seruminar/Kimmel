using System;

using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Parsing.Models
{
    public class OptionParsingState<T> where T : DescribesAccessibleProperty
    {
        public bool Found { get; set; }

        public ReadOnlyMemory<char> Option { get; set; }

        public Action<T> Describe { get; set; }

        public OptionParsingState(
            bool found,
            ReadOnlyMemory<char> option,
            Action<T> describe
            )
        {
            Found = found;
            Option = option;
            Describe = describe;
        }
    }
}