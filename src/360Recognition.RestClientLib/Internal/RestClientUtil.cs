using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Terryberry.Http.Compression;

namespace Terryberry.Http.Internal
{
    public static class RestClientUtil
    {
        public static HttpRequestMessage BuildRequest(RestClientConfig config, IRestClientRequest request)
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

        public static Uri BuildUri(RestClientConfig config, string path, NameValueCollection query)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            string queryString = "";

            if (query != null && query.Count > 0)
            {
                queryString = BuildQueryString(query);
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

        public static HttpClient CreateHttpClient()
        {
            return HttpClientFactory.Create(new CompressionHandler(), new DecompressionHandler());
        }

        public static void SetOathQueryParameterIfRequired(IRestClientRequest request, string token)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Querystring)
            {
                request.Query = request.Query ?? new NameValueCollection();
                request.Query["oauth_token"] = token;
            }
        }

        public static void SetOauthHeaderIfRequired(RestClientConfig config, IRestClientRequest request, HttpRequestHeaders headers)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Header)
            {
                headers.Authorization = new AuthenticationHeaderValue("OAuth", config.Token);
            }
        }

        private static string BuildQueryString(NameValueCollection query)
        {
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString("");

            httpValueCollection.Add(query);

            return httpValueCollection.ToString();
        }
    }
}