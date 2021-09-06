using Newtonsoft.Json;

namespace DotNetFive.Core.Caching
{
    internal class JsonConverter<T> : IConverter<T>
    {
        public T Deserialize(string value)
        {
            T result = default;

            if (value != null)
            {
                result = JsonConvert.DeserializeObject<T>(value);
            }

            return result;
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
