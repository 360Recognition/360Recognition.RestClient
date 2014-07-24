using System.IO;
using System.IO.Compression;

namespace Recognition360.RestClientLib.Compression
{
    public class DeflateCompressor : Compressor
    {
        public const string DeflateEncoding = "deflate";

        public override string EncodingType
        {
            get { return DeflateEncoding; }
        }

        public override Stream CreateCompressionStream(Stream output)
        {
            return new DeflateStream(output, CompressionMode.Compress, leaveOpen: true);
        }

        public override Stream CreateDecompressionStream(Stream input)
        {
            return new DeflateStream(input, CompressionMode.Decompress, leaveOpen: true);
        }
    }
}
