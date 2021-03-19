using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Functions.Functions;
using Functions.Models;

using Kimmel.Core.Activation.Kontent;
using Kimmel.Core.Kontent;
using Kimmel.Core.Kontent.Models.Management.Types.ContentTypes;
using Kimmel.Core.Parsing;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.Export
{
    public class ExportKontentFunction : BaseFunction
    {
        private readonly IKmlParser kmlParser;
        private readonly ITypeActivator typeActivator;
        private readonly IKontentStore kontentStore;

        public ExportKontentFunction(
            ILogger<ExportKontentFunction> logger,
            IKmlParser kmlParser,
            ITypeActivator typeActivator,
            IKontentStore kontentStore
            ) : base(logger)
        {
            this.kmlParser = kmlParser;
            this.typeActivator = typeActivator;
            this.kontentStore = kontentStore;
        }

        [FunctionName(nameof(ExportKontentFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                "post",
                Route = Routes.ExportKontent
            )] ExportKontentRequest exportKontentRequest
            )
        {
            try
            {
                var (kml, managementApiKey, mode) = exportKontentRequest;

                kontentStore.Configure(managementApiKey);
                kmlParser.Options.Mode = mode;

                var parsedKml = kmlParser.ParseKml(kml);

                var globalGuid = Guid.NewGuid();
                var activatedTypes = new Dictionary<string, ContentType>();
                var activatedTypeSnippets = new Dictionary<string, ContentType>();

                foreach (var typeDescription in parsedKml.TypeDescriptions)
                {
                    var contentType = typeActivator.ActivateType(typeDescription, globalGuid);

                    if (contentType.External_id is null)
                    {
                        throw new ArgumentNullException(nameof(contentType.External_id));
                    }

                    activatedTypes.Add(contentType.External_id, contentType);
                }

                foreach (var typeDescription in parsedKml.SnippetTypeDescriptions)
                {
                    var contentType = typeActivator.ActivateTypeSnippet(typeDescription, globalGuid);

                    if (contentType.External_id is null)
                    {
                        throw new ArgumentNullException(nameof(contentType.External_id));
                    }

                    activatedTypeSnippets.Add(contentType.External_id, contentType);
                }

                foreach (var activatedTypeSnippet in activatedTypeSnippets)
                {
                    await kontentStore.AddContentTypeSnippet(activatedTypeSnippet.Value);
                }

                foreach (var activatedType in activatedTypes)
                {
                    await kontentStore.AddContentType(activatedType.Value);
                }

                return LogOk();
            }
            catch (Exception ex)
            {
                return LogException(ex);
            }
        }
    }
}