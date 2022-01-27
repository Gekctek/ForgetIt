using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Automerge.Core.JsonConverters
{
	public class ChangeJsonConverter : JsonConverter<Change>
	{
		public override Change? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			List<Operation>? operations = null;
			ActorId? actorId = null;
			ChangeHash? changeHash = null;
			long? seq = null;
			ulong? startOp = null;
			long? logicalTime = null;
			string? message = null;
			List<ChangeHash>? dependencies = null;
			byte[]? extraBytes = null;
			switch (reader.TokenType)
			{
				case JsonTokenType.PropertyName:
					string propertyName = reader.GetString()!;
					if (!reader.Read())
					{
						throw new Exception($"Invalid json. Could not read value of '{propertyName}'");
					}
					switch (propertyName)
					{
						case "ops":
							operations = JsonSerializer.Deserialize<List<Operation>>(ref reader, options);
							break;
						case "actor":
							actorId = JsonSerializer.Deserialize<ActorId>(ref reader, options);
							break;
						case "hash":
							changeHash = JsonSerializer.Deserialize<ChangeHash>(ref reader, options);
							break;
						case "seq":
							seq = reader.GetInt64();
							break;
						case "startOp":
							startOp = reader.GetUInt64();
							break;
						case "time":
							logicalTime = reader.GetInt64();
							break;
						case "message":
							message = reader.GetString();
							break;
						case "deps":
							dependencies = JsonSerializer.Deserialize<List<ChangeHash>>(ref reader, options);
							break;
						case "extra_bytes":
							extraBytes = reader.GetBytesFromBase64();
							break;

					}
					break;
			}
			List<string>? missingProperties = null; // Only allocate list if there are any

			CheckRequiredProperty("ops", operations);
			CheckRequiredProperty("actor", actorId);
			CheckRequiredProperty("seq", seq);
			CheckRequiredProperty("startOp", startOp);
			CheckRequiredProperty("time", logicalTime);
			CheckRequiredProperty("deps", dependencies);

			if (missingProperties != null)
			{
				string missingPropertiesString = string.Join(", ", missingProperties);
				throw new Exception($"Invalid json. Missing properties for Change object: [{missingPropertiesString}]");
			}

			return new Change(operations!, actorId!, changeHash, seq!.Value, startOp!.Value, logicalTime!.Value, message, dependencies!, extraBytes);


			void CheckRequiredProperty(string name, object? value)
			{
				if (value == null)
				{
					if (missingProperties == null)
					{
						missingProperties = new List<string>();
					}
					missingProperties.Add(name);
				}
			}
		}

		public override void Write(Utf8JsonWriter writer, Change value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			writer.WritePropertyName("ops");
			JsonSerializer.Serialize(writer, value.Operations, options);

			writer.WriteBase64String("actor", value.ActorId.Value); // TODO base64?

			if (value.ChangeHash != null)
			{
				writer.WriteBase64String("hash", value.ChangeHash.Hash); // TODO base64?
			}

			writer.WriteNumber("seq", value.Seq);

			writer.WriteNumber("startOp", value.StartOp);

			writer.WriteNumber("time", value.LogicalTime);

			writer.WriteString("message", value.Message);

			writer.WritePropertyName("deps");
			JsonSerializer.Serialize(writer, value.Dependencies, options);

			if (value.ExtraBytes?.Any() == true)
			{
				writer.WriteBase64String("extra_bytes", value.ExtraBytes); // TODO base64?
			}


			writer.WriteEndObject();
		}
	}
}
