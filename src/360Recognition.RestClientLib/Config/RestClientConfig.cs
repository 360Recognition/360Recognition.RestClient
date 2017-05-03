using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Terryberry.Http
{
    public class RestClientConfig
    {
        private string _scheme;
        public string Host = "api.360recognition.com";

        public RestClientConfig()
        {
            BackoffFactor = 25;
            SerializerSettings = JsonSerializerSettingsManager.DefaultSettings;
        }

        public double BackoffFactor { get; set; }

        public string ContentEncoding { get; set; }

        public int? Port { get; set; }

        public string Scheme
        {
            get
            {
                if (string.IsNullOrEmpty(_scheme))
                {
                    _scheme = Uri.UriSchemeHttps;
                }
                return _scheme;
            }
            set => _scheme = value;
        }
        public JsonSerializerSettings SerializerSettings { get; set; }

        public TimeSpan Timeout { get; set; }

        public string Token { get; set; }

        public RestClientConfig CreateCopy()
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                return bf.Deserialize(ms) as RestClientConfig;
            }
        }
    }
}