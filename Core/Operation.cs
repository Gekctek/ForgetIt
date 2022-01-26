using Automerge.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Automerge
{
	public enum OperationType
	{
		Make,
		Delete,
		Increment,
		Set,
		MultiSet
	}
	public enum ObjectType
	{
		Map,
		Table,
		List,
		Text,
	}
	public class Operation
	{
		private readonly object _value;

		public OperationType Type { get; }
		public ObjectId ObjectId { get; }
		public Key Key { get; }
		public List<OperationId> Pred { get; }
		public bool Insert { get; }

		private Operation(
			OperationType type,
			object value,
			ObjectId objectId,
			Key key,
			List<OperationId> pred,
			bool insert)
		{
			this.Type = type;
			this._value = value;
			this.ObjectId = objectId;
			this.Key = key;
			this.Pred = pred;
			this.Insert = insert;
		}

		public ObjectType AsMakeObjectType => GetValue<ObjectType>(OperationType.Make);

		public uint AsDeletionCount => GetValue<uint>(OperationType.Delete);

		public long AsIncrementCount => GetValue<long>(OperationType.Increment);

		public ScalarValue AsSetValue => GetValue<ScalarValue>(OperationType.Set);

		public List<ScalarValue> AsMultiSetValue => GetValue<List<ScalarValue>>(OperationType.MultiSet);


		private T GetValue<T>(OperationType type)
		{
			if (this.Type != type)
			{
				throw new InvalidOperationException($"Cannot convert operation type '{this.Type}' to '{type}'");
			}
			return (T)this._value;
		}

		public static Operation Make(ObjectType type, ObjectId objectId, Key key, List<OperationId> pred, bool insert)
		{
			return new Operation(OperationType.Make, type, objectId, key, pred, insert);
		}

		public static Operation Delete(int count, ObjectId objectId, Key key, List<OperationId> pred, bool insert)
		{
			return new Operation(OperationType.Delete, count, objectId, key, pred, insert);
		}

		public static Operation Increment(long value, ObjectId objectId, Key key, List<OperationId> pred, bool insert)
		{
			return new Operation(OperationType.Increment, value, objectId, key, pred, insert);
		}

		public static Operation Set(ScalarValue value, ObjectId objectId, Key key, List<OperationId> pred, bool insert)
		{
			if(value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			return new Operation(OperationType.Set, value, objectId, key, pred, insert);
		}

		public static Operation Set(List<ScalarValue> values, ObjectId objectId, Key key, List<OperationId> pred, bool insert)
		{
			if (values?.Any() != true)
			{
				throw new ArgumentException("At least one value must be specified");
			}
			if(values.Count == 1)
			{
				return Set(values[0], objectId, key, pred, insert);
			}
			return new Operation(OperationType.MultiSet, values, objectId, key, pred, insert);
		}
	}

}
