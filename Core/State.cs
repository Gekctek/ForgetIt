using Automerge;
using Automerge.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Automerge.Core
{
	public class State
	{
		public ObjectId Id { get; }
		public IReadOnlyList<Operation> Operations => this._operations;

		private readonly List<Operation> _operations;

		//private JsonObject? _snapshotCache = null;

		public State(ObjectId id, List<Operation>? operations = null)
		{
			Id = id;
			_operations = operations ?? new List<Operation>();
		}

		public Operation Set(Key key, ScalarValue value, bool insert = false)
		{
			var pred = new List<OperationId>(); // TODO
			var operation = Operation.Set(value, this.Id, key, pred, insert);
			this._operations.Add(operation);
			return operation;
		}

		public Change BuildChange(ActorId actorId, long logicalTime, string message)
		{
			(ChangeHash hash, List<ChangeHash> dependencies) = BuildChangeHash();
			byte[] extraBytes = null; // TODO
			long seq = 0; // TODO
			ulong startOp = 0; // TODO
			return new Change(
				operations: this._operations.ToList(),
				actorId: actorId,
				changeHash: hash,
				seq: seq,
				startOp: startOp,
				logicalTime: logicalTime,
				message: message,
				dependencies: dependencies,
				extraBytes: extraBytes
			);
		}

		private (ChangeHash Hash, List<ChangeHash> Dependencies) BuildChangeHash()
		{
			//TODO
			byte[] hash = new byte[32];
			return (new ChangeHash(hash), new List<ChangeHash>());
		}

		//public Operation Update<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr, TProperty value)
		//{
		//	MemberExpression memberExpression = propertyExpr.Body as MemberExpression ?? throw new InvalidOperationException("Only member expressions can be used for state updates");

		//	// TODO nested property name?
		//	string propertyName = memberExpression.Member.Name;
		//	// TODO key format?
		//	Key key = new MapKey(propertyName);
		//	ScalarValue scalarValue = ;
		//	return Operation.Set(scalarValue, this.Id, key, pred, insert: false);
		//}

		//public List<Operation> Update<T>(T obj)
		//{
		//	JsonObject currentObj = GetSnapshot();

		//	JsonDocument newDoc = JsonSerializer.SerializeToDocument(obj);
		//	JsonObject newObj = JsonObject.Create(newDoc.RootElement)!;

		//	List<Operation> newOperations = BuildPatch(currentObj, newObj, JsonPath.Empty()).ToList();
		//	if (newOperations.Any())
		//	{
		//		this._operations.AddRange(newOperations);
		//		this._snapshotCache = null;
		//	}
		//	return newOperations;
		//}

		public void Add(Operation operation)
		{
			this._operations.Add(operation);
			//this._snapshotCache = null;
		}

		//public T Build<T>()
		//{
		//	JsonObject snapshot = GetSnapshot();

		//	return snapshot.Deserialize<T>()!;
		//}

		//private IEnumerable<Operation> BuildPatch(JsonNode currentNode, JsonNode newNode, JsonPath path)
		//{
		//	ScalarValue value;
		//	if (currentNode is JsonObject currentObj)
		//	{
		//		if (newNode is JsonObject newObj)
		//		{
		//			IEnumerable<Operation> operations = BuildPatch(currentObj, newObj, path);
		//			foreach (Operation operation in operations)
		//			{
		//				yield return operation;
		//			}
		//			yield break;
		//		}
		//	}
		//	else if (currentNode is JsonArray currentArray)
		//	{
		//		if (newNode is JsonArray newArray)
		//		{
		//			IEnumerable<Operation> operations = BuildPatch(currentArray, newArray, path);
		//			foreach (Operation operation in operations)
		//			{
		//				yield return operation;
		//			}
		//		}
		//	}
		//	else
		//	{
		//		JsonValue currentValue = (JsonValue)currentNode;
		//		if (newNode is JsonValue newValue)
		//		{
		//			// TODO
		//			if(currentValue.ToJsonString() == newValue.ToJsonString())
		//			{
		//				// Dont update if the same
		//				yield break;
		//			}
		//		}
		//		value = currentValue
		//	}

		//	// Catch all, current != new and both are specified
		//	yield return Operation.Set(value, this.Id, key, pred, insert: false);

		//}
		//private IEnumerable<Operation> BuildPatch(JsonObject currentObj, JsonObject newObj, JsonPath path)
		//{
		//	List<string> currentProperties = GetProperties(currentObj);
		//	List<string> newProperties = GetProperties(newObj);
		//	foreach (string property in currentProperties.Union(newProperties))
		//	{
		//		IEnumerable<Operation> operations = PatchProperty(property, currentObj, newObj, path);
		//		foreach (Operation operation in operations)
		//		{
		//			yield return operation;
		//		}
		//	}
		//}

		//private IEnumerable<Operation> PatchProperty(string property, JsonObject currentObj, JsonObject newObj, JsonPath path)
		//{
		//	bool existsCurrent = currentObj.TryGetPropertyValue(property, out JsonNode? currentProperty) && currentProperty != null;
		//	bool existsNew = newObj.TryGetPropertyValue(property, out JsonNode? newProperty);
		//	if (existsCurrent)
		//	{
		//		if (!existsNew)
		//		{
		//			yield break;
		//		}
		//		else
		//		{
		//			IEnumerable<Operation> operations = BuildPatch(currentProperty!, newProperty!, path.Add(property));
		//			foreach (Operation operation in operations)
		//			{
		//				yield return operation;
		//			}
		//			yield break;
		//		}
		//	}
		//	else
		//	{
		//		if (existsNew && newProperty != null)
		//		{
		//			yield return Operation.Add(path.Add(property), newProperty);
		//			yield break;
		//		}
		//		// TODO equality
		//		bool changed = currentProperty != newProperty;
		//		if (changed)
		//		{
		//			yield return Operation.Replace(path.Add(property), newProperty);
		//			yield break;
		//		}
		//		// No change
		//		yield break;
		//	}
		//}


		//private List<string> GetProperties(JsonObject currentObj)
		//{
		//	return currentObj
		//		.Select(f => f.Key)
		//		.ToList();
		//}

		//private JsonObject GetSnapshot()
		//{
		//	if (_snapshotCache == null)
		//	{
		//		_snapshotCache = BuildSnapshot();
		//	}
		//	return _snapshotCache;
		//}

		//private JsonObject BuildSnapshot()
		//{
		//	var snapshot = new JsonObject();
		//	foreach (Operation operation in this.Operations)
		//	{
		//		operation.Apply(snapshot);
		//	}
		//	return snapshot;
		//}
	}
}
