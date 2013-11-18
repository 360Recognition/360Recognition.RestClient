using System;
using System.Threading;

namespace Recognition360.RestClientLib
{
    internal static class ExponentialBackoff
    {
        public static void Sleep(double backoffFactor, int attempt)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(Math.Pow(backoffFactor, attempt)));
        }
    }
}