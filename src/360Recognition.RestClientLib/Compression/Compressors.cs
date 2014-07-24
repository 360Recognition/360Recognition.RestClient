using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

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

        public static ICompressor FindCompressor(HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptEncoding)
        {
            // As per RFC2616.14.3:
            // Ignores encodings with quality == 0
            // If multiple content-codings are acceptable, then the acceptable content-coding with the highest non-zero qvalue is preferred.

            string preferredEncoding = GetNonZeroHeaders(acceptEncoding).
                OrderByDescending(x => x.Quality).
                Select(x => x.Encoding).
                FirstOrDefault();

            return GetCompressor(preferredEncoding);
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