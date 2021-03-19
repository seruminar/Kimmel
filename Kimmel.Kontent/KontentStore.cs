using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Kimmel.Core.Kontent;
using Kimmel.Core.Kontent.Models.Management;
using Kimmel.Core.Kontent.Models.Management.References;
using Kimmel.Core.Kontent.Models.Management.Types.ContentTypes;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Kimmel.Kontent
{
    public class KontentStore : IKontentStore
    {
        private readonly IKontentRateLimiter kontentRateLimiter;
        private readonly HttpClient httpClient;
        private string projectId = string.Empty;

        public IDictionary<string, object> CacheDictionary { get; } = new ConcurrentDictionary<string, object>();

        public KontentStore(
            IKontentRateLimiter kontentRateLimiter,
            HttpClient httpClient
            )
        {
            this.kontentRateLimiter = kontentRateLimiter;
            this.httpClient = httpClient;
        }

        public ExternalIdReference NewExternalIdReference() => new ExternalIdReference(Guid.NewGuid().ToString());

        public void Configure(string managementApiKey)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", managementApiKey);

            if (GetClaims(managementApiKey).TryGetValue("project_id", out var projectIdObject))
            {
                projectId = Guid.Parse(projectIdObject.Value<string>()).ToString();
            }
        }

        public async Task AddContentType(ContentType contentType)
        {
            var response = await kontentRateLimiter.WithRetry(() => Post($"types", contentType));

            await ThrowIfNotSuccessStatusCode(response);
        }

        public async Task AddContentTypeSnippet(ContentType contentType)
        {
            var response = await kontentRateLimiter.WithRetry(() => Post($"snippets", contentType));

            await ThrowIfNotSuccessStatusCode(response);
        }

        private async Task EnumerateListing<T>(Func<Task<HttpResponseMessage>> doRequest, Action<T> getItems) where T : AbstractListingResponse
        {
            var continuationToken = "";

            do
            {
                httpClient.DefaultRequestHeaders.Add("x-continuation", continuationToken);

                var response = await kontentRateLimiter.WithRetry(doRequest);

                await ThrowIfNotSuccessStatusCode(response);

                var responseObject = await response.Content.ReadAsAsync<T>();

                getItems(responseObject);

                continuationToken = responseObject.Pagination?.ContinuationToken;

                httpClient.DefaultRequestHeaders.Remove("x-continuation");
            } while (!string.IsNullOrWhiteSpace(continuationToken));
        }

        private async Task<TOut> Cache<TIn, TOut>(Func<Task<TIn>> doRequest, string key, Func<TIn, Task<TOut>> getItem) where TOut : class?
        {
            if (CacheDictionary.TryGetValue(key, out var item) && item is TOut tItem)
            {
                return tItem;
            }

            var newItem = await getItem(await doRequest());

            if (newItem != null)
            {
                CacheDictionary.Add(key, newItem);
            }

            return newItem;
        }

        private async Task<HttpResponseMessage> Get(string endpoint) => await httpClient.GetAsync(GetEndpoint(endpoint));

        private async Task<HttpResponseMessage> Put(string endpoint, object? value = default)
        {
            var response = await httpClient.PutAsync(GetEndpoint(endpoint), value, new JsonMediaTypeFormatter()
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            return response;
        }

        private async Task<HttpResponseMessage> Post(string endpoint, object? value = default)
        {
            var response = await httpClient.PostAsync(GetEndpoint(endpoint), value, new JsonMediaTypeFormatter()
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            return response;
        }

        private string GetEndpoint(string? endpoint = default) => $@"https://manage.kontent.ai/v2/projects/{projectId}/{endpoint}";

        private static async Task ThrowIfNotSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsAsync<APIErrorResponse>();
                throw errorContent.GetException();
            }
        }

        private static JObject GetClaims(string jwt)
        {
            var base64UrlClaimsSet = GetBase64UrlClaimsSet(jwt);
            var claimsSet = DecodeBase64Url(base64UrlClaimsSet);

            try
            {
                return JObject.Parse(claimsSet);
            }
            catch (JsonReaderException exception)
            {
                throw new FormatException(exception.Message, exception);
            }
        }

        private static string GetBase64UrlClaimsSet(string jwt)
        {
            var firstDotIndex = jwt.IndexOf('.');
            var lastDotIndex = jwt.LastIndexOf('.');

            if (firstDotIndex == -1 || lastDotIndex <= firstDotIndex)
            {
                throw new FormatException("The JWT should contain two periods.");
            }

            return jwt.Substring(firstDotIndex + 1, lastDotIndex - firstDotIndex - 1);
        }

        private static string DecodeBase64Url(string base64Url)
        {
            var base64 = base64Url
                .Replace('-', '+')
                .Replace('_', '/')
                .PadRight(base64Url.Length + ((4 - (base64Url.Length % 4)) % 4), '=');

            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }
    }
}