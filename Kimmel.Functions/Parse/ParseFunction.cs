using System;

using Functions.Functions;
using Functions.Models;

using Kimmel.Core.Parsing;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.Parse
{
    public class ParseFunction : BaseFunction
    {
        private readonly IKmlParser kmlParser;

        public ParseFunction(
            ILogger<ParseFunction> logger,
            IKmlParser kmlParser
            ) : base(logger)
        {
            this.kmlParser = kmlParser;
        }

        [FunctionName(nameof(ParseFunction))]
        public IActionResult Run(
            [HttpTrigger(
                "post",
                Route = Routes.Parse
            )] ParseRequest data
            )
        {
            try
            {
                var (kml, mode) = data;

                kmlParser.Options.Mode = mode;

                return LogOkObject(new
                {
                    ParsedKml = kmlParser.ParseKml(kml)
                });
            }
            catch (Exception ex)
            {
                return LogException(ex);
            }
        }
    }
}