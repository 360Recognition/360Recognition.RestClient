using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using Recognition360.RestClientLib.Internal;

namespace Recognition360.RestClientLib
{
    public class RestClient
    {
        /// <summary>
        /// Generates the Uri for the specified request.
        /// </summary>
        /// <param name="config">The project id and other config values</param>
        /// <param name="request">The request endpoint and query parameters</param>
        /// <param name="token">
        /// (optional) The token to use for the building the request uri if different than the Token specified
        /// in the config.
        /// </param>
        public static Uri BuildRequestUri(RestClientConfig config, IRestClientRequest request, string token = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = config.Token;
            }

            RestClientUtil.SetOathQueryParameterIfRequired(request, token);

            return RestClientUtil.BuildUri(config, request.EndPoint, request.Query);
        }

        public static RestResponseMessage<T> Delete<T>(
            RestClientConfig config,
            string endPoint,
            NameValueCollection query = null,
            Object payload = null) where T : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Delete
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload);
            }

            return new RestResponseMessage<T>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }

        public static async Task<HttpResponseMessage> Execute(RestClientConfig config, IRestClientRequest request)
        {
            HttpRequestMessage httpRequest = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = request.EndPoint,
                Query = request.Query,
                Method = request.Method,
                Content = request.Content
            });

            using (HttpClient client = RestClientUtil.CreateHttpClient())
            {
                return await client.SendAsync(httpRequest);
            }
        }

        public static RestResponseMessage<T> Get<T>(
            RestClientConfig config,
            string endPoint,
            NameValueCollection query = null) where T : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Get
            });

            return new RestResponseMessage<T>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }

        public static RestResponseMessage<T> Post<T>(
            RestClientConfig config,
            string endPoint,
            object payload = null,
            NameValueCollection query = null) where T : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Post
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload);
            }

            return new RestResponseMessage<T>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }

        public static RestResponseMessage<T> Put<T>(
            RestClientConfig config,
            string endPoint,
            object payload = null,
            NameValueCollection query = null) where T : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Put
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload);
            }

            return new RestResponseMessage<T>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }
    }
}