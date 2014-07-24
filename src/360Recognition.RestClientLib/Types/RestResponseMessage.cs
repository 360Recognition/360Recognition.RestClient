using System.Net.Http;
using System.Threading.Tasks;

namespace Recognition360.RestClientLib
{
    public class RestResponseMessage<T> where T : class
    {
        private readonly Task<HttpResponseMessage> _task;

        public RestResponseMessage(Task<HttpResponseMessage> responseTask)
        {
            _task = responseTask;
        }

        public Task<HttpResponseMessage> Task
        {
            get { return _task; }
        }

        public async Task<HttpResponseMessage> ReadHttpMessage()
        {
            return await Task;
        }

        public async Task<T> ReadResult()
        {
            HttpResponseMessage responseMessage = await Task;

            return await responseMessage.Content.ReadAsAsync<T>();
        }
    }
}