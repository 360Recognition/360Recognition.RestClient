using System;
using System.Net.Http;

namespace Terryberry.Http
{
    public class MaximumRetryAttemptsExceededException : ApplicationException
    {
        public MaximumRetryAttemptsExceededException(HttpRequestMessage request, int maxAttempts)
            : base($"The maximum number of retry attempts ({maxAttempts}) has been exceeded.")
        {
            Request = request;
        }

        public HttpRequestMessage Request { get; }
    }
}