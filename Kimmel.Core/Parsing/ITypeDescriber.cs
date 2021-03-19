using System.Collections.Generic;

using Kimmel.Core.Models;
using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Parsing
{
    public interface ITypeDescriber
    {
        ReadOnlyOptions Options { get; }

        TypeDescription DescribeType(
            DescribesTypedProperty[] activators,
            int count,
            ICollection<string> typeLinkedTypeIds,
            ICollection<string> typeSnippetTypeIds
            );

        TypeDescription CloneType(TypeDescription typeDescription);

        TypeDescription DescribeEmptyType(string typeId);

        void SetTypeLinks(TypeDescription typeDescription, IDictionary<string, string>? linkedTypeIdsMap = null);
    }
}