using System;

namespace Recognition360.RestClientLib.Compression
{
    internal static class Compressors
    {
        public static ICompressor GetCompressor(string encodingName)
        {
            if (string.Equals(encodingName, GZipCompressor.GZipEncoding, StringComparison.OrdinalIgnoreCase))
            {
                return new GZipCompressor();
            }
            if (string.Equals(encodingName, DeflateCompressor.DeflateEncoding, StringComparison.OrdinalIgnoreCase))
            {
                return new DeflateCompressor();
            }
            return null;
        }
    }
}