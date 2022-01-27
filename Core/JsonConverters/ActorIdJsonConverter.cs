using System.Text.Json;
using System.Text.Json.Serialization;
using ForgetIt.Core;
using System;

namespace Automerge.Core.JsonConverters
{
    public class ActorIdJsonConverter : JsonConverter<ActorId>
    {
        public override ActorId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            byte[] hash = reader.GetBytesFromBase64();
            return new ActorId(hash);
        }

        public override void Write(Utf8JsonWriter writer, ActorId value, JsonSerializerOptions options)
        {
            writer.WriteBase64String("hash", value.Value);
        }
    }
}