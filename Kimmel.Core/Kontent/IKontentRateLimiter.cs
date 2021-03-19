using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kimmel.Core.Kontent
{
    public interface IKontentRateLimiter
    {
        Task<HttpResponseMessage> WithRetry(Func<Task<HttpResponseMessage>> doRequest);
    }
}