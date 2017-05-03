using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Terryberry.Http
{
    public class RestResponse<T> where T : class
    {
        private T _result;

        public RestResponse(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage ?? throw new ArgumentNullException("responseMessage");
        }

        public HttpContent Content => ResponseMessage.Content;

        public HttpResponseMessage ResponseMessage { get; set; }

        public T Result
        {
            get
            {
                SetResult();
                return _result;
            }
        }

        public static implicit operator bool(RestResponse<T> value)
        {
            return value.ResponseMessage != null && value.ResponseMessage.IsSuccessStatusCode;
        }

        public static implicit operator T(RestResponse<T> value)
        {
            return value.Result;
        }

        public bool CanReadResult()
        {
            try
            {
                SetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> ReadResultAsync()
        {
            return await Content.ReadAsAsync<T>();
        }

        private void SetResult()
        {
            LazyInitializer.EnsureInitialized(ref _result, () => ReadResultAsync().Result);
        }
    }
}