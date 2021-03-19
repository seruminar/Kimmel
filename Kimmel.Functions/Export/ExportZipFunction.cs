using System;
using System.Threading.Tasks;

using Functions.Functions;
using Functions.Models;

using Kimmel.Core.Export;
using Kimmel.Core.Parsing;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

namespace Functions.Export
{
    public class ExportZipFunction : BaseFunction
    {
        private readonly IKmlParser kmlParser;
        private readonly IZipper zipper;

        public ExportZipFunction(
            ILogger<ExportZipFunction> logger,
            IKmlParser kmlParser,
            IZipper zipper
            ) : base(logger)
        {
            this.kmlParser = kmlParser;
            this.zipper = zipper;
        }

        [FunctionName(nameof(ExportZipFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                "post",
                Route = Routes.ExportZip
            )] ParseRequest data
            )
        {
            try
            {
                var (kml, mode) = data;

                kmlParser.Options.Mode = mode;

                var parsedKml = kmlParser.ParseKml(kml);
                var exportStream = await zipper.Zip(
                    new[] {
                        (kml, "kml.kml"),
                        (JsonConvert.SerializeObject(parsedKml, Formatting.Indented), "kml.json")
                    });

                exportStream.Position = 0;

                return new FileStreamResult(exportStream, new MediaTypeHeaderValue("application/zip"));
            }
            catch (Exception ex)
            {
                return LogException(ex);
            }
        }
    }
}