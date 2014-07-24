using Common.Logging;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Recognition360.RestClientLib.Compression;

namespace Recognition360.RestClientLib
{
    public class RestClient
    {
        /// <summary>
        /// Generates the Uri for the specified request.
        /// </summary>
        /// <param name="config">The project id and other config values</param>
        /// <param name="request">The request endpoint and query parameters</param>
        /// <param name="token">(optional) The token to use for the building the request uri if different than the Token specified in the config.</param>
        public static Uri BuildRequestUri(RestClientConfig config, IRestClientRequest request, string token = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = config.Token;
            }
            SetOathQueryParameterIfRequired(request, token);
            return BuildUri(config, request.EndPoint, request.Query);
        }

        public static RestResponseMessage<T> Delete<T>(RestClientConfig config, string endPoint, NameValueCollection query = null, Object payload = null) where T : class
        {
            HttpRequestMessage request = BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Delete
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload);
            }

            return new RestResponseMessage<T>(AttemptRequestAsync(config, request));
        }

        public static async Task<HttpResponseMessage> Execute(RestClientConfig config, IRestClientRequest request)
        {
            HttpRequestMessage httpRequest = BuildRequest(config, new RestClientRequest
            {
                EndPoint = request.EndPoint,
                Query = request.Query,
                Method = request.Method,
                Content = request.Content
            });

            using (var client = CreateHttpClient())
            {
                return await client.SendAsync(httpRequest);
            }
        }

        public static  RestResponseMessage<T> Get<T>(RestClientConfig config, string endPoint, NameValueCollection query = null) where T : class
        {
            HttpRequestMessage request = BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Get
            });

            return new RestResponseMessage<T>(AttemptRequestAsync(config, request));
        }

        public static RestResponseMessage<T> Post<T>(RestClientConfig config, string endPoint, object payload = null, NameValueCollection query = null) where T : class
        {
            HttpRequestMessage request = BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Post
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload);
            }

            return new RestResponseMessage<T>(AttemptRequestAsync(config, request));
        }

        public static RestResponseMessage<T> Put<T>(RestClientConfig config, string endPoint, object payload, NameValueCollection query = null) where T : class
        {
            HttpRequestMessage request = BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Put
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload);
            }

            return new RestResponseMessage<T>(AttemptRequestAsync(config, request));
        }

        private static HttpClient CreateHttpClient()
        {
            return HttpClientFactory.Create(new DecompressionHandler(), new CompressionHandler());
        }

        private static async Task<HttpResponseMessage> AttemptRequestAsync(RestClientConfig config, HttpRequestMessage request, int attempt = 0)
        {
            if (attempt > HttpClientOptions.RetryLimit)
            {
                throw new MaximumRetryAttemptsExceededException(request, HttpClientOptions.RetryLimit);
            }

            ILog logger = LogManager.GetLogger<RestClient>();

            using (var client = CreateHttpClient())
            {
                if (logger.IsDebugEnabled)
                {
                    using (var sw = new StringWriter())
                    {
                        sw.WriteLine("{0} {1}", request.Method, request.RequestUri);
                        if (request.Content != null)
                        {
                            sw.WriteLine(await request.Content.ReadAsStringAsync());
                        }
                        logger.Debug(sw.ToString());
                    }
                }

                HttpResponseMessage response = await client.SendAsync(request);

                if (logger.IsDebugEnabled)
                {
                    if (response.Content != null)
                    {
                        logger.Debug(await response.Content.ReadAsStringAsync());
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                if (HttpClientOptions.EnableRetry && IsRetriableStatusCode(response))
                {
                    attempt++;

                    await ExponentialBackoff.Sleep(config.BackoffFactor, attempt);

                    return await AttemptRequestAsync(config, request, attempt);
                }

                return response;
            }
        }

        private static HttpRequestMessage BuildRequest(RestClientConfig config, IRestClientRequest request)
        {
            SetOathQueryParameterIfRequired(request, config.Token);

            var httpRequest = new HttpRequestMessage
            {
                Content = request.Content,
                RequestUri = BuildUri(config, request.EndPoint, request.Query),
                Method = request.Method
            };

            HttpRequestHeaders headers = httpRequest.Headers;

            SetOauthHeaderIfRequired(config, request, headers);
            
            headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(request.Accept));

            return httpRequest;
        }

        private static Uri BuildUri(RestClientConfig config, string path, NameValueCollection query)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            string queryString = "";

            if (query != null && query.Count > 0)
            {
                NameValueCollection httpValueCollection = HttpUtility.ParseQueryString("");

                httpValueCollection.Add(query);

                queryString = httpValueCollection.ToString();
            }

            var uriBuilder = new UriBuilder(config.Scheme, config.Host)
            {
                Path = path,
                Query = queryString
            };

            if (config.Port.HasValue)
            {
                uriBuilder.Port = config.Port.Value;
            }

            return uriBuilder.Uri;
        }

        private static bool IsRetriableStatusCode(HttpResponseMessage response)
        {
            return response != null && response.StatusCode == HttpStatusCode.ServiceUnavailable;
        }

        private static void SetOathQueryParameterIfRequired(IRestClientRequest request, string token)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Querystring)
            {
                request.Query = request.Query ?? new NameValueCollection();
                request.Query["oauth_token"] = token;
            }
        }

        private static void SetOauthHeaderIfRequired(RestClientConfig config, IRestClientRequest request, HttpRequestHeaders headers)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Header)
            {
                headers.Authorization = new AuthenticationHeaderValue("OAuth", config.Token);
            }
        }
    }
}