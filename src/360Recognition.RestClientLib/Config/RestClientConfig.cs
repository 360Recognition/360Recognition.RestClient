using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Terryberry.Http
{
    public class RestClientConfig
    {
        private string _scheme;
        private string _host;

        public RestClientConfig()
        {
            BackoffFactor = 25;
        }

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
            set { _scheme = value; }
        }

        public string Host
        {
            get
            {
                if (string.IsNullOrEmpty(_host))
                {
                    _host = "api.360recognition.com";
                }
                return _host;
            }
            set { _host = value; }
        }

        public string ContentEncoding { get; set; }

        public string Token { get; set; }

        public double BackoffFactor { get; set; }

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