using System.Text.Json;
using System.Text.Json.Serialization;
using ForgetIt.Core;
using System;
using System.Text.Json.Nodes;

namespace Automerge.Core.JsonConverters
{
    public class OperationJsonConverter : JsonConverter<Operation>
    {
        public override Operation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			JsonNode? obj = JsonNode.Parse(ref reader);
			if (obj == null)
			{
				return null;
			}
			if (!(obj is JsonObject jObj))
			{
				throw new JsonException("Invalid operation json");
			}
			JsonNode typeNode = GetRequiredValue("type");
			OperationType type = typeNode.GetValue<OperationType>();

			JsonNode objectIdNode = GetRequiredValue("objectId");
			ObjectId objectId = objectIdNode.Deserialize<ObjectId>(options) ?? throw new JsonException("Missing object id");

			switch(type)
			{
				case OperationType.Make:
					JsonNode objTypeNode = GetRequiredValue("objType");
					ObjectType objType = objTypeNode.GetValue<ObjectType>();
					return Operation.Make(objType, objectId, key, pred, insert);
				case OperationType.Delete:
					return Operation.Delete(deleteCount, objectId, key, pred, insert);
				case OperationType.Increment:
					return Operation.Increment(incrementValue, objectId, key, pred, insert);
				case OperationType.Set:
					return Operation.Set(setValue, objectId, key, pred, insert);
				case OperationType.MultiSet:
					return Operation.Set(multiSetValue, objectId, key, pred, insert);
				default:
					throw new NotImplementedException();
			};

			JsonNode GetRequiredValue(string propertyName)
			{
				if (!jObj.TryGetPropertyValue(propertyName, out JsonNode? node) || node == null)
				{
					throw new JsonException("Missing value 'type'");
				}
				return node;
			}
		}

		public override void Write(Utf8JsonWriter writer, Operation value, JsonSerializerOptions options)
        {
            writer.WriteBase64String("hash", value.Hash);
        }
    }
}