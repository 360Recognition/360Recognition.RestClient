using System.Net.Http;
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
            if (request.Content == null || request.Headers.AcceptEncoding == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            ICompressor compressor = Compressors.FindCompressor(request.Headers.AcceptEncoding);

            if (compressor != null)
            {
                request.Content = new CompressedContent(request.Content, compressor);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}