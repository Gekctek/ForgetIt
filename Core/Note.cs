using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ForgetIt.Core;

public class Note : AutoStatefulObject
{
	public string Id { get; set; }
	public string Type { get; set; }
	public int Version { get; set; }
	public string Owner { get; set; }
	public DateTime CreateDate { get; set; }
	public string Text { get; set; }
	public DateTime? DueDate { get; set; }

	public Note(string id, string type, int version, string owner, DateTime createDate, string text, DateTime? dueDate)
	{
		Id = id ?? throw new ArgumentNullException(nameof(id));
		Type = type ?? throw new ArgumentNullException(nameof(type));
		Version = version;
		Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		CreateDate = createDate;
		Text = text ?? throw new ArgumentNullException(nameof(text));
		DueDate = dueDate;
	}
}

public abstract class AutoStatefulObject : StatefulObject
{
	private Dictionary<string, PropertyInfo>? _propertyCache = null;

	private Dictionary<string, PropertyInfo> GetProperties()
	{
		if (_propertyCache == null)
		{
			_propertyCache = ReflectionUtil.GetClassProperties(this.GetType());
		}
		return _propertyCache;
	}


	protected override JsonDocument BuildSnapshot()
	{
		return JsonSerializer.SerializeToDocument(this);
	}
}

public abstract class StatefulObject
{
	private JsonDocument? _snapshotCache = null;

	protected abstract JsonDocument BuildSnapshot();

	public JsonDocument GetSnapshot()
	{
		if (_snapshotCache == null)
		{
			_snapshotCache = BuildSnapshot();
		}
		return _snapshotCache;
	}
}

public class State<T> where T : StatefulObject, new()
{
	public List<PatchOperation> Operations { get; }
	private JsonObject? _snapshotCache = null;

	public State(List<PatchOperation> operations)
	{
		Operations = operations;
	}

	public void Update(T obj)
	{
		JsonObject currentObj = GetSnapshot();

		JsonDocument newDoc = obj.GetSnapshot();
		JsonObject newObj = JsonObject.Create(newDoc.RootElement)!;

		List<PatchOperation> newOperations = BuildPatch(currentObj, newObj, JsonPath.Empty()).ToList();
		if (newOperations.Any())
		{
			this.Operations.AddRange(newOperations);
			JsonObject snapshot = GetSnapshot();

			foreach (PatchOperation operation in newOperations)
			{
				operation.Apply(snapshot);
			}
		}
	}

	private IEnumerable<PatchOperation> BuildPatch(JsonNode currentNode, JsonNode newNode, JsonPath path)
	{
		if (currentNode is JsonObject currentObj)
		{
			if (newNode is JsonObject newObj)
			{
				return BuildPatch(currentObj, newObj, path);
			}
		}
		else if (currentNode is JsonArray currentArray)
		{
			if (newNode is JsonArray newArray)
			{
				return BuildPatch(currentArray, newArray, path);
			}
		}
		else
		{
			JsonValue currentValue = (JsonValue)currentNode;
			if (newNode is JsonValue newValue)
			{
				return BuildPatch(currentValue, newValue, path);
			}
		}

		// Catch all, current != new and both are specified
		return new List<PatchOperation>
		{
			PatchOperation.Replace(path, newNode)
		};

	}
	private IEnumerable<PatchOperation> BuildPatch(JsonObject currentObj, JsonObject newObj, JsonPath path)
	{
		List<string> currentProperties = GetProperties(currentObj);
		List<string> newProperties = GetProperties(newObj);
		foreach (string property in currentProperties.Union(newProperties))
		{
			IEnumerable<PatchOperation> operations = PatchProperty(property, currentObj, newObj, path);
			foreach(PatchOperation operation in operations)
			{
				yield return operation;
			}
		}
	}

	private IEnumerable<PatchOperation> PatchProperty(string property, JsonObject currentObj, JsonObject newObj, JsonPath path)
	{
		bool existsCurrent = currentObj.TryGetPropertyValue(property, out JsonNode? currentProperty);
		bool existsNew = newObj.TryGetPropertyValue(property, out JsonNode? newProperty);
		if (existsCurrent)
		{
			if (!existsNew)
			{
				yield break;
			}
			else
			{
				IEnumerable<PatchOperation> operations = BuildPatch(currentProperty!, newProperty!, path.Add(property));
				foreach(PatchOperation operation in operations)
				{
					yield return operation;
				}
				yield break;
			}
		}
		else
		{
			if (existsNew)
			{
				yield return PatchOperation.Add(path.Add(property), newProperty!);
				yield break;
			}
			// TODO equality
			bool changed = currentProperty! != newProperty!;
			if (changed)
			{
				yield return PatchOperation.Replace(path.Add(property), newProperty!);
				yield break;
			}
			// No change
			yield break;
		}
	}


	private List<string> GetProperties(JsonObject currentObj)
	{
		return currentObj
			.Select(f => f.Key)
			.ToList();
	}

	private JsonObject GetSnapshot()
	{
		if (_snapshotCache == null)
		{
			_snapshotCache = BuildSnapshot();
		}
		return _snapshotCache;
	}

	private JsonObject BuildSnapshot()
	{
		var snapshot = new JsonObject();
		foreach (PatchOperation operation in this.Operations)
		{
			operation.Apply(snapshot);
		}
		return snapshot;
	}
}
