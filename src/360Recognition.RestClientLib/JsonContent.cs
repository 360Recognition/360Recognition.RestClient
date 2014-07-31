using System.Net.Http;
using System.Net.Http.Headers;

namespace Terryberry.Http
{
    public class JsonContent : StringContent
    {
        public JsonContent(object content, string contentEncoding = null)
            : base(JSON.Generate(content))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (!string.IsNullOrEmpty(contentEncoding))
            {
                Headers.ContentEncoding.Add(contentEncoding);
            }
        }
    }
}