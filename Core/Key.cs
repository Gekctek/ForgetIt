using System;
using System.Collections.Generic;
using System.Text;

namespace Automerge.Core
{
	public abstract class Key
	{
		public static implicit operator Key(string key) => new MapKey(key);
		public static explicit operator Key(OperationId operationId) => new SeqKey(operationId);
	}

	public class MapKey : Key
	{
		public string Value { get; }

		public MapKey(string value)
		{
			this.Value = value ?? throw new ArgumentNullException(nameof(value));
		}

		public static implicit operator MapKey(string key) => new MapKey(key);
	}

	public class SeqKey : Key
	{
		// If null, then it is pointing at the head
		public OperationId? OperationId { get; }

		public SeqKey(OperationId? operationId)
		{
			this.OperationId = operationId;
		}

		public static SeqKey Head()
		{
			return new SeqKey(null);
		}

		public static implicit operator SeqKey(OperationId operationId) => new SeqKey(operationId);
	}
}
