using System.IO;
using System.Threading.Tasks;

namespace Kimmel.Core.Export
{
    public interface IZipper
    {
        Task<Stream> Zip((string, string)[] files);
    }
}