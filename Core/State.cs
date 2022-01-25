﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ForgetIt.Core
{
	public class State
	{
		public int Id { get; }
		public string Type { get; }
		public IReadOnlyList<PatchOperation> Operations => this._operations;
		private readonly List<PatchOperation> _operations;
		private JsonObject? _snapshotCache = null;

		public State(int id, string type, List<PatchOperation>? operations = null)
		{
			Id = id;
			Type = type;
			_operations = operations ?? new List<PatchOperation>();
		}

		public PatchOperation Update<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr, TProperty value)
		{
			MemberExpression memberExpression = propertyExpr.Body as MemberExpression ?? throw new InvalidOperationException("Only member expressions can be used for state updates");

			string propertyName = memberExpression.Member.Name;
			propertyName = JsonNamingPolicy.CamelCase.ConvertName(propertyName);
			var path = new JsonPath(new JsonPathSegment[1] { JsonPathSegment.Property(propertyName) });
			JsonNode? node = JsonValue.Create(value);
			return PatchOperation.Add(path, node);
		}

		public List<PatchOperation> Update<T>(T obj)
		{
			JsonObject currentObj = GetSnapshot();

			JsonDocument newDoc = JsonSerializer.SerializeToDocument(obj);
			JsonObject newObj = JsonObject.Create(newDoc.RootElement)!;

			List<PatchOperation> newOperations = BuildPatch(currentObj, newObj, JsonPath.Empty()).ToList();
			if (newOperations.Any())
			{
				this._operations.AddRange(newOperations);
				this._snapshotCache = null;
			}
			return newOperations;
		}

		public void Add(PatchOperation operation)
		{
			this._operations.Add(operation);
			this._snapshotCache = null;
		}

		public T Build<T>()
		{
			JsonObject snapshot = GetSnapshot();

			return snapshot.Deserialize<T>()!;
		}

		private IEnumerable<PatchOperation> BuildPatch(JsonNode currentNode, JsonNode newNode, JsonPath path)
		{
			if (currentNode is JsonObject currentObj)
			{
				if (newNode is JsonObject newObj)
				{
					IEnumerable<PatchOperation> operations = BuildPatch(currentObj, newObj, path);
					foreach (PatchOperation operation in operations)
					{
						yield return operation;
					}
				}
			}
			else if (currentNode is JsonArray currentArray)
			{
				if (newNode is JsonArray newArray)
				{
					IEnumerable<PatchOperation> operations = BuildPatch(currentArray, newArray, path);
					foreach (PatchOperation operation in operations)
					{
						yield return operation;
					}
				}
			}
			else
			{
				JsonValue currentValue = (JsonValue)currentNode;
				if (newNode is JsonValue newValue)
				{
					// TODO
					if(currentValue.ToJsonString() == newValue.ToJsonString())
					{
						// Dont update if the same
						yield break;
					}
				}
			}

			// Catch all, current != new and both are specified
			yield return PatchOperation.Replace(path, newNode);

		}
		private IEnumerable<PatchOperation> BuildPatch(JsonObject currentObj, JsonObject newObj, JsonPath path)
		{
			List<string> currentProperties = GetProperties(currentObj);
			List<string> newProperties = GetProperties(newObj);
			foreach (string property in currentProperties.Union(newProperties))
			{
				IEnumerable<PatchOperation> operations = PatchProperty(property, currentObj, newObj, path);
				foreach (PatchOperation operation in operations)
				{
					yield return operation;
				}
			}
		}

		private IEnumerable<PatchOperation> PatchProperty(string property, JsonObject currentObj, JsonObject newObj, JsonPath path)
		{
			bool existsCurrent = currentObj.TryGetPropertyValue(property, out JsonNode? currentProperty) && currentProperty != null;
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
					foreach (PatchOperation operation in operations)
					{
						yield return operation;
					}
					yield break;
				}
			}
			else
			{
				if (existsNew && newProperty != null)
				{
					yield return PatchOperation.Add(path.Add(property), newProperty);
					yield break;
				}
				// TODO equality
				bool changed = currentProperty != newProperty;
				if (changed)
				{
					yield return PatchOperation.Replace(path.Add(property), newProperty);
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
}
