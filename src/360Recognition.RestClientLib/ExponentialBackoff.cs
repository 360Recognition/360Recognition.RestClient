using System;
using System.Threading.Tasks;

namespace Recognition360.RestClientLib
{
    internal static class ExponentialBackoff
    {
        public static Task Sleep(double backoffFactor, int attempt)
        {
            return Task.Delay(TimeSpan.FromMilliseconds(Math.Pow(backoffFactor, attempt)));
        }
    }
}