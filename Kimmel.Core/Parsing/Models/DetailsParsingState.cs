using System;

using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Parsing.Models
{
    public class DetailsParsingState<T> where T : DescribesAccessibleProperty
    {
        public bool Found { get; set; }

        public ReadOnlyMemory<char> Option { get; set; }

        public Action<T, Memory<char>[]?> Describe { get; set; }

        public DetailsParsingState(
            bool found,
            ReadOnlyMemory<char> option,
            Action<T, Memory<char>[]?> describe
            )
        {
            Found = found;
            Option = option;
            Describe = describe;
        }
    }
}