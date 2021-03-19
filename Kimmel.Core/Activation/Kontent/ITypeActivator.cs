using System;
using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.References;
using Kimmel.Core.Kontent.Models.Management.Types.ContentTypes;
using Kimmel.Core.Kontent.Models.Management.Types.Elements;
using Kimmel.Core.Models;
using Kimmel.Core.Parsing.Models.Descriptions;

namespace Kimmel.Core.Activation.Kontent
{
    public interface ITypeActivator
    {
        ReadOnlyOptions Options { get; }

        ContentType ActivateType(TypeDescription typeDescription, Guid globalGuid);

        ContentType ActivateTypeSnippet(TypeDescription typeDescription, Guid globalGuid);

        Limit? ParseRange(DescribesList listPropertyDescription);

        IList<Reference> ParseLinks(string[] linkedTypeIds, Guid globalGuid);

        IList<MultipleChoiceElement.Option> ParseMultipleOptions(string[] options);
    }
}