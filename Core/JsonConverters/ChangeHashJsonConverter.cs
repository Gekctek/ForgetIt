using System.Text.Json;
using System.Text.Json.Serialization;
using ForgetIt.Core;
using System;

namespace Automerge.Core.JsonConverters
{
    public class ChangeHashJsonConverter : JsonConverter<ChangeHash>
    {
        public override ChangeHash Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            byte[] hash = reader.GetBytesFromBase64();
            return new ChangeHash(hash);
        }

        public override void Write(Utf8JsonWriter writer, ChangeHash value, JsonSerializerOptions options)
        {
            writer.WriteBase64String("hash", value.Hash);
        }
    }
}