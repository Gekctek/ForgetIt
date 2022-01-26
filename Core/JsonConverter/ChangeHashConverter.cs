using System.Text.Json;
using System.Text.Json.Serialization;
using ForgetIt.Core;
using System;

namespace Automerge
{
    public class ChangeHashConverter : JsonConverter<ChangeHash>
    {
        public override ChangeHash? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            byte[] hash = Convert.FromBase64String(value);
            return new ChangeHash(hash);
        }

        public override void Write(Utf8JsonWriter writer, ChangeHash value, JsonSerializerOptions options)
        {
            writer.WriteBase64String("hash", value.Hash);
        }
    }
}