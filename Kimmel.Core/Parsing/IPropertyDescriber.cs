using System;
using System.Collections.Generic;

using Kimmel.Core.Models;
using Kimmel.Core.Parsing.Models;
using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Parsing
{
    public interface IPropertyDescriber
    {
        ReadOnlyOptions Options { get; }

        DescribesTypedProperty Describe(bool snippet, string identifier, Memory<char>[]? options, string? label);

        Property MatchProperty(bool snippet, string identifier, Memory<char>[]? options);

        void ParseOptions<T>(Memory<char>[]? options,
            T description,
            IList<OptionParsingState<T>>? targetOptions = null,
            IList<DetailsParsingState<T>>? targetDetailedOptions = null,
            RangeParsingState<DescribesList>? targetRangeOption = null
            ) where T : DescribesAccessibleProperty;

        bool ParseOption<T>(T description, Memory<char> option, IList<OptionParsingState<T>> targetOptions) where T : DescribesAccessibleProperty;

        void ParseDetailedOption<T>(T description, Memory<char> option, IList<DetailsParsingState<T>> targetDetailedOptions) where T : DescribesAccessibleProperty;

        void ParseRangeOption(DescribesList description, Memory<char> option, RangeParsingState<DescribesList> targetRangeOption);

        string[] ParseMultipleOptions<T>(Memory<char>[]? options, T description, IList<OptionParsingState<T>> targetOptions) where T : DescribesAccessibleProperty;
    }
}