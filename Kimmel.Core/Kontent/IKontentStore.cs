using System.Threading.Tasks;

using Kimmel.Core.Kontent.Models.Management.References;
using Kimmel.Core.Kontent.Models.Management.Types.ContentTypes;

namespace Kimmel.Core.Kontent
{
    public interface IKontentStore
    {
        ExternalIdReference NewExternalIdReference();

        void Configure(string managementApiKey);

        Task AddContentType(ContentType contentType);

        Task AddContentTypeSnippet(ContentType contentType);
    }
}