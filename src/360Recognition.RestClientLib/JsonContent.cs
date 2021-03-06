﻿using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Terryberry.Http
{
    public class JsonContent : StringContent
    {
        public JsonContent(object content, string contentEncoding = null, JsonSerializerSettings serializerSettings = null)
            : base(JSON.Generate(content, serializerSettings))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (!string.IsNullOrEmpty(contentEncoding))
            {
                Headers.ContentEncoding.Add(contentEncoding);
            }
        }
    }
}