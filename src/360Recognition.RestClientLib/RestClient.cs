using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using Terryberry.Http.Compression;
using Terryberry.Http.Internal;

namespace Terryberry.Http
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

        /// <summary>
        /// Sends a DELETE <param name="payload"></param> message to the specified <param name="endPoint"></param>
        /// </summary>
        /// <typeparam name="TResponseMessage">The strongly typed response message</typeparam>
        /// <param name="config">The configuration.</param>
        /// <param name="endPoint">The path for the API (example: /api/userprofile)</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="payload">The payload request message</param>
        public static RestResponseMessage<TResponseMessage> Delete<TResponseMessage>(
            RestClientConfig config,
            string endPoint,
            NameValueCollection query = null,
            Object payload = null) where TResponseMessage : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Delete
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload, config.ContentEncoding);
            }

            return new RestResponseMessage<TResponseMessage>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }

        /// <summary>
        /// Executes a raw HTTP client request
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="request">The request parameters.</param>
        public static Task<HttpResponseMessage> Execute(RestClientConfig config, IRestClientRequest request)
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
                return client.SendAsync(httpRequest);
            }
        }

        /// <summary>
        /// Gets an API response from the specified <param name="endPoint"></param>
        /// </summary>
        /// <typeparam name="TResponseMessage">The strongly typed response message</typeparam>
        /// <param name="config">The configuration.</param>
        /// <param name="endPoint">The path for the API (example: /api/userprofile)</param>
        /// <param name="query">Optional query parameters</param>
        public static RestResponseMessage<TResponseMessage> Get<TResponseMessage>(
            RestClientConfig config,
            string endPoint,
            NameValueCollection query = null) where TResponseMessage : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Get
            });

            return new RestResponseMessage<TResponseMessage>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }

        /// <summary>
        /// Posts a gzip compressed <param name="payload"></param> message to the specified <param name="endPoint"></param>
        /// </summary>
        /// <typeparam name="TResponseMessage">The strongly typed response message</typeparam>
        /// <param name="config">The configuration.</param>
        /// <param name="endPoint">The path for the API (example: /api/userprofile)</param>
        /// <param name="payload">The payload request message</param>
        /// <param name="query">Optional query parameters</param>
        public static RestResponseMessage<TResponseMessage> PostCompressed<TResponseMessage>(
            RestClientConfig config,
            string endPoint,
            object payload = null,
            NameValueCollection query = null) where TResponseMessage : class
        {
            var modifiedConfig = config.CreateCopy();

            modifiedConfig.ContentEncoding = GZipCompressor.GZipEncoding;

            return Post<TResponseMessage>(config, endPoint, payload, query);
        }

        /// <summary>
        /// Posts a <param name="payload"></param> message to the specified <param name="endPoint"></param>
        /// </summary>
        /// <typeparam name="TResponseMessage">The strongly typed response message</typeparam>
        /// <param name="config">The configuration.</param>
        /// <param name="endPoint">The path for the API (example: /api/userprofile)</param>
        /// <param name="payload">The payload request message</param>
        /// <param name="query">Optional query parameters</param>
        public static RestResponseMessage<TResponseMessage> Post<TResponseMessage>(
            RestClientConfig config,
            string endPoint,
            object payload = null,
            NameValueCollection query = null) where TResponseMessage : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Post
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload, config.ContentEncoding);
            }

            return new RestResponseMessage<TResponseMessage>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }

        /// <summary>
        /// Puts a gzip compressed <param name="payload"></param> message to the specified <param name="endPoint"></param>
        /// </summary>
        /// <typeparam name="TResponseMessage">The strongly typed response message</typeparam>
        /// <param name="config">The configuration.</param>
        /// <param name="endPoint">The path for the API (example: /api/userprofile)</param>
        /// <param name="payload">The payload request message</param>
        /// <param name="query">Optional query parameters</param>
        public static RestResponseMessage<TResponseMessage> PutCompressed<TResponseMessage>(
            RestClientConfig config,
            string endPoint,
            object payload = null,
            NameValueCollection query = null) where TResponseMessage : class
        {
            var modifiedConfig = config.CreateCopy();

            modifiedConfig.ContentEncoding = GZipCompressor.GZipEncoding;

            return Put<TResponseMessage>(config, endPoint, payload, query);
        }

        /// <summary>
        /// Puts a <param name="payload"></param> message to the specified <param name="endPoint"></param>
        /// </summary>
        /// <typeparam name="TResponseMessage">The strongly typed response message</typeparam>
        /// <param name="config">The configuration.</param>
        /// <param name="endPoint">The path for the API (example: /api/userprofile)</param>
        /// <param name="payload">The payload request message</param>
        /// <param name="query">Optional query parameters</param>
        public static RestResponseMessage<TResponseMessage> Put<TResponseMessage>(
            RestClientConfig config,
            string endPoint,
            object payload = null,
            NameValueCollection query = null) where TResponseMessage : class
        {
            HttpRequestMessage request = RestClientUtil.BuildRequest(config, new RestClientRequest
            {
                EndPoint = endPoint,
                Query = query,
                Method = HttpMethod.Put
            });

            if (payload != null)
            {
                request.Content = new JsonContent(payload, config.ContentEncoding);
            }

            return new RestResponseMessage<TResponseMessage>(RestRequestExecuter.AttemptRequestAsync(config, request));
        }
    }
}