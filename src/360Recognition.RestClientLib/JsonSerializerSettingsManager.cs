using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Terryberry.Http
{
    public static class JsonSerializerSettingsManager
    {
        private static JsonSerializerSettings _jsonSerializerSettings;

        public static JsonSerializerSettings DefaultSettings
        {
            get => LazyInitializer.EnsureInitialized(ref _jsonSerializerSettings, GetDefaultSerializerSettings);
            set => _jsonSerializerSettings = value;
        }

        private static JsonSerializerSettings GetDefaultSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                }
            };
        }
    }
}