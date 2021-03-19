using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using Kimmel.Core.Export;

namespace Kimmel.Export.Zip
{
    public class Zipper : IZipper
    {
        public async Task<Stream> Zip((string, string)[] files)
        {
            var memoryStream = new MemoryStream();
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

            foreach (var (fileContents, fileName) in files)
            {
                await AddFile(fileContents, fileName, archive);
            }

            return memoryStream;
        }

        private static async Task AddFile(string fileContents, string fileName, ZipArchive zipStream)
        {
            var entry = zipStream.CreateEntry(fileName);

            using var writer = new StreamWriter(entry.Open());

            await writer.WriteAsync(fileContents);
        }
    }
}