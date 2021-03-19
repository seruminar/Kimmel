using System;
using System.Collections.Generic;
using System.Linq;

using Kimmel.Core.Exceptions;

using Newtonsoft.Json;

namespace Kimmel.Core.Kontent.Models.Management
{
    public class APIErrorResponse
    {
        public string? Message { get; set; }

        [JsonProperty("validation_errors")]
        public IEnumerable<ValidationError>? ValidationErrors { get; set; }

        public Exception GetException()
        {
            if (ValidationErrors != default && ValidationErrors.Any())
            {
                return new ApiException($"{Message} {string.Join(", ", ValidationErrors.Select(error => $"{error.Path}: {error.Message}"))}");
            }

            return new ApiException($"{Message}");
        }
    }

    public class ValidationError
    {
        public string? Message { get; set; }

        public string? Path { get; set; }
    }
}