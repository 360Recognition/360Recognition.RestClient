using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Terryberry.Http
{
    public class RestResponseMessage<T> where T : class
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Task<HttpResponseMessage> _task;
        private HttpResponseMessage _responseMessage;

        public RestResponseMessage(HttpResponseMessage responseMessage)
            : this(responseMessage, null)
        {
        }

        public RestResponseMessage(HttpResponseMessage responseMessage, JsonSerializerSettings serializerSettings)
        {
            _responseMessage = responseMessage;
            _serializerSettings = serializerSettings;
        }

        public RestResponseMessage(Task<HttpResponseMessage> responseTask) : this(responseTask, null)
        {
        }

        public RestResponseMessage(Task<HttpResponseMessage> responseTask, JsonSerializerSettings serializerSettings)
        {
            _task = responseTask;
            _serializerSettings = serializerSettings;
        }

        private Task<HttpResponseMessage> Task
        {
            get { return _task; }
        }

        public async Task<HttpResponseMessage> ReadHttpMessage()
        {
            return _responseMessage ?? (_responseMessage = await Task);
        }

        public async Task<T> ReadResult()
        {
            HttpResponseMessage responseMessage = await ReadHttpMessage();

            MediaTypeFormatter formatter = CreateMediaTypeFormatter();

            if (ResponseCanBeRead(responseMessage, formatter))
            {
                return await responseMessage.Content.ReadAsAsync<T>(new[] {formatter});
            }

            throw new RestResponseException("Unable to read result", responseMessage);
        }

        private static bool ResponseCanBeRead(HttpResponseMessage responseMessage, MediaTypeFormatter formatter)
        {
            return responseMessage.IsSuccessStatusCode && formatter.SupportedMediaTypes.Contains(responseMessage.Content.Headers.ContentType);
        }

        private JsonMediaTypeFormatter CreateMediaTypeFormatter()
        {
            if (_serializerSettings == null)
            {
                return new JsonMediaTypeFormatter();
            }

            return new JsonMediaTypeFormatter
            {
                SerializerSettings = _serializerSettings
            };
        }
    }
}