using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            AddHeaders(this, content, compressor);
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

        private static void AddHeaders(HttpContent target, HttpContent source, ICompressor compressor)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in source.Headers)
            {
                target.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            ICollection<string> encodingHeaders = target.Headers.ContentEncoding;

            string encodingType = compressor.EncodingType;

            if (HeadersAlreadyContainsEncodingHeader(encodingHeaders, encodingType))
            {
                return;
            }

            encodingHeaders.Add(encodingType);
        }

        private static bool HeadersAlreadyContainsEncodingHeader(IEnumerable<string> headers, string encodingType)
        {
            return headers.Any(x => string.Equals(encodingType, x, StringComparison.OrdinalIgnoreCase));
        }
    }
}