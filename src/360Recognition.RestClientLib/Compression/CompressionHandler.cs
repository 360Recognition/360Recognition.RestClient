using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Recognition360.RestClientLib.Compression
{
    public class CompressionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.Content == null || request.Headers.AcceptEncoding == null)
            {
                return response;
            }

            ICompressor compressor = FindCompressor(request);

            if (compressor != null)
            {
                response.Content = new CompressedContent(response.Content, compressor);
            }

            return response;
        }

        private static ICompressor FindCompressor(HttpRequestMessage request)
        {
            // As per RFC2616.14.3:
            // Ignores encodings with quality == 0
            // If multiple content-codings are acceptable, then the acceptable content-coding with the highest non-zero qvalue is preferred.

            HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptEncoding = request.Headers.AcceptEncoding;

            string preferredEncoding = GetNonZeroHeaders(acceptEncoding).
                OrderByDescending(x => x.Quality).
                Select(x => x.Encoding).
                FirstOrDefault();

            return Compressors.GetCompressor(preferredEncoding);
        }

        private static IEnumerable<AcceptEncodingHeader> GetNonZeroHeaders(
            IEnumerable<StringWithQualityHeaderValue> acceptEncoding)
        {
            return acceptEncoding.
                Select(x => new AcceptEncodingHeader
                {
                    Encoding = x.Value,
                    Quality = x.Quality.GetValueOrDefault(1)
                }).
                Where(x => x.Quality > 0);
        }

        private class AcceptEncodingHeader
        {
            public string Encoding { get; set; }

            public double Quality { get; set; }
        }
    }
}