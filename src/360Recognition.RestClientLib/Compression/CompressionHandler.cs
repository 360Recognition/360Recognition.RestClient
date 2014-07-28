using System.Linq;
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
            if (request.Content == null || request.Content.Headers.ContentEncoding == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            string encoding = request.Content.Headers.ContentEncoding.FirstOrDefault();

            ICompressor compressor = Compressors.GetCompressor(encoding);

            if (compressor != null)
            {
                request.Content = new CompressedContent(request.Content, compressor);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}