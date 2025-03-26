/*
using System;
using Newtonsoft.Json;

namespace _Project.Scripts.Tools.Observable
{
    public class ObservableVariableConverter<T> : JsonConverter<ObservableVariable<T>>
    {
        public override void WriteJson(JsonWriter writer, ObservableVariable<T> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Value);
        }

        public override ObservableVariable<T> ReadJson(JsonReader reader, Type objectType,
            ObservableVariable<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            T value = serializer.Deserialize<T>(reader);
            ObservableVariable<T> observableVariable = new ObservableVariable<T>(value);

            return observableVariable;
        }
    }
}
*/