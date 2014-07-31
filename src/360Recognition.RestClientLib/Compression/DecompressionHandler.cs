using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Terryberry.Http.Compression
{
    public class DecompressionHandler : DelegatingHandler
    {  
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.Content == null || response.Content.Headers.ContentEncoding == null)
            {
                return response;
            }

            string encoding = response.Content.Headers.ContentEncoding.FirstOrDefault();

            ICompressor compressor = Compressors.GetCompressor(encoding);

            if (compressor != null)
            {
                response.Content = await DecompressContentAsync(response.Content, compressor);
            }
            
            return response;
        }

        private static async Task<HttpContent> DecompressContentAsync(
            HttpContent compressedContent,
            ICompressor compressor)
        {
            using (compressedContent)
            {
                var decompressed = new MemoryStream();

                await compressor.Decompress(await compressedContent.ReadAsStreamAsync(), decompressed);

                // set position back to 0 so it can be read again
                decompressed.Position = 0;

                var newContent = new StreamContent(decompressed);

                // copy content type so we know how to load correct formatter
                newContent.Headers.ContentType = compressedContent.Headers.ContentType;

                return newContent;
            }
        }
    }
}