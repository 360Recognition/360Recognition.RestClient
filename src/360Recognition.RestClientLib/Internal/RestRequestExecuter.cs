using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Logging;

namespace Terryberry.Http.Internal
{
    public static class RestRequestExecuter
    {
        public static async Task<HttpResponseMessage> AttemptRequestAsync(RestClientConfig config, HttpRequestMessage request, int attempt = 0)
        {
            if (attempt > HttpClientOptions.RetryLimit)
            {
                throw new MaximumRetryAttemptsExceededException(request, HttpClientOptions.RetryLimit);
            }

            ILog logger = LogManager.GetLogger<RestClient>();

            using (HttpClient client = RestClientUtil.CreateHttpClient())
            {
                if (logger.IsDebugEnabled)
                {
                    await LogRequest(request, logger);
                }

                HttpResponseMessage response = await client.SendAsync(request);

                if (logger.IsDebugEnabled)
                {
                    await LogResponse(response, logger);
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                if (HttpClientOptions.EnableRetry && IsRetriableStatusCode(response))
                {
                    return await RetryRequest(config, request, attempt);
                }

                return response;
            }
        }

        public static bool IsRetriableStatusCode(HttpResponseMessage response)
        {
            return response != null && response.StatusCode == HttpStatusCode.ServiceUnavailable;
        }

        private static async Task LogRequest(HttpRequestMessage request, ILog logger)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("{0} {1}", request.Method, request.RequestUri);
                if (request.Content != null && !request.Content.IsMimeMultipartContent())
                {
                    sw.WriteLine(await request.Content.ReadAsStringAsync());
                }
                logger.Debug(sw.ToString());
            }
        }

        private static async Task LogResponse(HttpResponseMessage response, ILog logger)
        {
            if (response.Content != null)
            {
                logger.Debug(await response.Content.ReadAsStringAsync());
            }
        }

        private static async Task<HttpResponseMessage> RetryRequest(RestClientConfig config, HttpRequestMessage request, int attempt)
        {
            attempt++;

            await ExponentialBackoff.Sleep(config.BackoffFactor, attempt);

            return await AttemptRequestAsync(config, request, attempt);
        }
    }
}