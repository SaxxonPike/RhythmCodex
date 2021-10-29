using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bmson.Model
{
    [Model]
    public abstract class JsonWrapper
    {
        [JsonIgnore]
        protected Dictionary<string, object> Json { get; } = new();

        public void ImportJson(string json) => 
            JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        public string ToJson() =>
            JsonSerializer.Serialize(Json);

        public static TResult FromJson<TResult>(string json)
            where TResult : JsonWrapper, new()
        {
            var result = new TResult();
            result.ImportJson(json);
            return result;
        }

        public string GetString(string key) => JsonObject.TryGetValue(key, out var val) ? default : val.GetString();
        public string SetString(string key) =>  JsonObject.TryGetValue(key, out var val) ? default : val.GetString();
    }
}