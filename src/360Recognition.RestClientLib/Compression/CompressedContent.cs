using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Recognition360.RestClientLib.Compression
{
    public class CompressedContent : HttpContent
    {
        private readonly ICompressor _compressor;
        private readonly HttpContent _content;

        public CompressedContent(HttpContent content, ICompressor compressor)
        {
            _content = content;
            _compressor = compressor;

            AddHeaders();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (_content)
            {
                Stream contentStream = await _content.ReadAsStreamAsync();
                await _compressor.Compress(contentStream, stream);
            }
        }

        private void AddHeaders()
        {
            foreach (var header in _content.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add(_compressor.EncodingType);
        }
    }
}