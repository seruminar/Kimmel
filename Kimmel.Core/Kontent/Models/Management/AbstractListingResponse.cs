﻿using Newtonsoft.Json;

namespace Kimmel.Core.Kontent.Models.Management
{
    public abstract class AbstractListingResponse
    {
        public Pagination? Pagination { get; set; }
    }

    public class Pagination
    {
        [JsonProperty("continuation_token")]
        public string? ContinuationToken { get; set; }
    }
}