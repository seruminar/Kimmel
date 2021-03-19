//using System.Threading.Tasks;

//using Kimmel.Core.Exceptions;

//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Toolkit.HighPerformance;

//namespace Kimmel.Core.Functions
//{
//    public static class HttpRequestDataExtensions
//    {
//        /// <exception cref="PropertyNullException"/>
//        /// <exception cref="InvalidTypeException"/>
//        public static async Task<T> ReadBodyAs<T>(this HttpRequestData request)
//        {
//            if (request.Body == null)
//            {
//                throw new PropertyNullException(nameof(request.Body));
//            }

//            using var stream = request.Body.Value.AsStream();

//            var data = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream);

//            if (data == null)
//            {
//                throw new InvalidTypeException(typeof(T).Name);
//            }

//            return data;
//        }
//    }
//}