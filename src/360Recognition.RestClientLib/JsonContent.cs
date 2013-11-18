using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Recognition360.RestClientLib
{
    public class JsonContent : StringContent
    {
        public JsonContent(Object content)
            : base(JSON.Generate(content))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}