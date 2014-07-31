using System.IO;
using System.Threading.Tasks;

namespace Terryberry.Http.Compression
{
    public abstract class Compressor : ICompressor
    {
        public abstract string EncodingType { get; }

        public virtual Task Compress(Stream source, Stream destination)
        {
            var compressed = CreateCompressionStream(destination);

            return source.CopyToAsync(compressed).ContinueWith(task => compressed.Dispose());
        }

        public abstract Stream CreateCompressionStream(Stream output);
        
        public abstract Stream CreateDecompressionStream(Stream input);
        
        public virtual Task Decompress(Stream source, Stream destination)
        {
            var decompressed = CreateDecompressionStream(source);

            return decompressed.CopyToAsync(destination).ContinueWith(task => decompressed.Dispose());
        }
    }
}
