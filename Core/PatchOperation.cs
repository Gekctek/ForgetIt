using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ForgetIt.Core
{

	public class PatchOperation
	{
		public OperationType Type { get; set; }
		public JsonPath Path { get; set; }
		public JsonNode? Value { get; set; }
		public JsonPath? From { get; set; }

		private PatchOperation(OperationType type, JsonPath path, JsonNode? value, JsonPath? from)
		{
			Type = type;
			Path = path ?? throw new ArgumentNullException(nameof(path));
			Value = value;
			From = from;
		}

		/// <summary>
		/// If the target location specifies an array index, a new value is inserted into the array at the specified index.
		/// If the target location specifies an object member that does not already exist, a new member is added to the object.
		/// If the target location specifies an object member that does exist, that member's value is replaced.
		/// </summary>
		public static PatchOperation Add(JsonPath path, JsonNode value)
		{
			return new PatchOperation(OperationType.Add, path, value, null);
		}

		/// <summary>
		///  The "remove" operation removes the value at the target location.
		///  The target location MUST exist for the operation to be successful.
		/// </summary>
		public static PatchOperation Remove(JsonPath path)
		{
			return new PatchOperation(OperationType.Remove, path, null, null);
		}
		/// <summary>
		/// The "replace" operation replaces the value at the target location with a new value.The operation object MUST contain a "value" member whose content specifies the replacement value.
		/// The target location MUST exist for the operation to be successful.
		/// </summary>
		public static PatchOperation Replace(JsonPath path, JsonNode value)
		{
			return new PatchOperation(OperationType.Replace, path, value, null);
		}

		/// <summary>
		///  The "copy" operation copies the value at a specified location to the target location.
		///  The operation object MUST contain a "from" member, which is a string containing a JSON Pointer value that references the location in the target document to copy the value from.
		///  The "from" location MUST exist for the operation to be successful.
		/// </summary>
		public static PatchOperation Copy(JsonPath path, JsonPath from)
		{
			return new PatchOperation(OperationType.Copy, path, null, from);
		}

		/// <summary>
		///  The "move" operation removes the value at a specified location and adds it to the target location.
		///  The operation object MUST contain a "from" member, which is a string containing a JSON Pointer value that references the location in the target document to move the value from.
		///  The "from" location MUST exist for the operation to be successful.
		/// </summary>
		public static PatchOperation Move(JsonPath path, JsonPath from)
		{
			return new PatchOperation(OperationType.Move, path, null, from);
		}

		/// <summary>
		/// The "test" operation tests that a value at the target location is equal to a specified value.
		/// The target location MUST be equal to the "value" value for the operation to be considered successful.
		/// </summary>
		public static PatchOperation Test(JsonPath path, JsonNode value)
		{
			return new PatchOperation(OperationType.Move, path, value, null);
		}

		public static GetNodeResult GetBaseNode(JsonNode node, Span<JsonPathSegment> path)
		{
			JsonPathSegment pathSegment = path[0];
			if (path.Length == 1)
			{
				return new GetNodeResult(node, path.ToArray());
			}
			JsonNode innerNode;
			switch (node)
			{
				case JsonObject jObj:
					if (pathSegment.IsIndex)
					{
						throw new InvalidOperationException("Can't use an index on an object");
					}
					JsonNode? n = jObj[pathSegment.AsProperty];
					if (n == null)
					{
						return new GetNodeResult(node, path.ToArray());
					}
					innerNode = n;
					break;
				case JsonArray jArray:
					if (!pathSegment.IsIndex)
					{
						throw new InvalidOperationException("Arrays require an index, not a property");
					}
					JsonNode? a = jArray[pathSegment.AsIndex];
					if (a == null)
					{
						return new GetNodeResult(node, path.ToArray());
					}
					innerNode = a;
					break;
				case JsonValue:
					throw new InvalidOperationException($"Can't add {(pathSegment.IsIndex ? "an array item" : "a property")} to a value");
				default:
					throw new NotImplementedException();
			}
			return GetBaseNode(innerNode, path[1..]);
		}

		public void Apply(JsonObject snapshot)
		{
			// TODO https://datatracker.ietf.org/doc/html/rfc6902#section-4.1
			GetNodeResult result = GetBaseNode(snapshot, this.Path.Segments);

			switch (this.Type)
			{
				case OperationType.Add:
					{
						SetValue(this.Value);
					}
					break;
				case OperationType.Remove:
					{
						Remove(result);
						break;
					}
				case OperationType.Replace:
					{
						if (!result.Found)
						{
							throw new Exception($"Operation can't replace '{this.Path}' because it does not exist");
						}
						SetValue(this.Value);
						break;
					}
				case OperationType.Copy:
					{
						if (this.From == null)
						{
							throw new InvalidOperationException($"Can't {this.Type} without a 'from' being specified");
						}
						GetNodeResult fromResult = GetBaseNode(snapshot, this.From.Segments);
						CopyValue(fromResult);
						break;
					}
				case OperationType.Move:
					{
						if (this.From == null)
						{
							throw new InvalidOperationException($"Can't {this.Type} without a 'from' being specified");
						}
						GetNodeResult fromResult = GetBaseNode(snapshot, this.From.Segments);
						CopyValue(fromResult);
						Remove(fromResult);
						break;
					}
				case OperationType.Test:
					{
						if (!result.Found)
						{
							throw new Exception($"Operation can't {this.Type} from '{this.Path}' because it does not exist");
						}
						if(this.Value == null)
						{
							throw new InvalidOperationException($"Can't {this.Type} with unspecified value");
						}
						if (result.GetValueOrDefault() != this.Value)
						{
							throw new Exception($"Test failed. Values are not equal");
						}
						break;
					}
			}

			void CopyValue(GetNodeResult fromResult)
			{
				if (!fromResult.Found)
				{
					throw new Exception($"Operation can't {this.Type} from '{this.Path}' because it does not exist");
				}
				(JsonNode fromLeaf, JsonPathSegment fromLastSegment) = fromResult.GetOrBuildRemainingNodes();
				JsonNode fromValue;
				if (fromLastSegment.IsIndex)
				{
					fromValue = fromLeaf[fromLastSegment.AsIndex]!;
				}
				else
				{
					fromValue = fromLeaf[fromLastSegment.AsIndex]!;
				}
				SetValue(fromValue);
			}

			void SetValue(JsonNode? value)
			{
				if (value == null)
				{
					throw new InvalidOperationException($"Can't {this.Type} with unspecified value");
				}
				(JsonNode leaf, JsonPathSegment lastSegment) = result.GetOrBuildRemainingNodes();
				if (lastSegment.IsIndex)
				{
					leaf[lastSegment.AsIndex] = value;
				}
				else
				{
					leaf[lastSegment.AsProperty] = value;
				}
			}

			void Remove(GetNodeResult result)
			{
				if (!result.Found)
				{
					throw new Exception($"Operation can't {this.Type} '{this.Path}' because it does not exist");
				}
				(JsonNode leaf, JsonPathSegment lastSegment) = result.GetOrBuildRemainingNodes();
				if (lastSegment.IsIndex)
				{
					leaf.AsArray().RemoveAt(lastSegment.AsIndex);
				}
				else
				{
					leaf.AsObject().Remove(lastSegment.AsProperty);
				}
			}
		}
	}

	public class GetNodeResult
	{
		public bool Found => this.RemainingPath.Length == 1;
		public JsonNode LastNode { get; }
		public JsonPathSegment[] RemainingPath { get; }

		public GetNodeResult(JsonNode lastNode, JsonPathSegment[] remainingPath)
		{
			LastNode = lastNode;
			RemainingPath = remainingPath;
		}

		public (JsonNode Leaf, JsonPathSegment LastSegment) GetOrBuildRemainingNodes()
		{
			JsonPathSegment nextSegment = this.RemainingPath[0];
			if (this.RemainingPath.Length == 1)
			{
				return (this.LastNode, nextSegment);
			}
			return BuildNode(this.LastNode, this.RemainingPath);
		}

		public JsonNode? GetValueOrDefault()
		{
			if (this.RemainingPath.Length == 1)
			{
				JsonPathSegment lastSegment = this.RemainingPath[0];
				if (lastSegment.IsIndex)
				{
					return this.LastNode[lastSegment.AsIndex];
				}
				return this.LastNode[lastSegment.AsProperty];
			}
			return null;
		}

		private static (JsonNode Leaf, JsonPathSegment LastSegment) BuildNode(JsonNode node, Span<JsonPathSegment> path)
		{
			JsonPathSegment segment = path[0];
			if (path.Length == 1)
			{
				// Base case, return the leaf/last segment
				return (node, segment);
			}

			node = node[segment.AsProperty] = segment.IsIndex
					? new JsonArray()
					: new JsonObject();

			return BuildNode(node, path[1..]);
		}
	}

	public enum OperationType
	{
		Add,
		Remove,
		Replace,
		Copy,
		Move,
		Test
	}
}
