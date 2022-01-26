using Automerge;
using System;
using System.Collections.Generic;
using System.Text;

namespace Automerge
{
	public enum ScalarValueType
	{
		// Null
		Null,
		// Bytes(Vec<u8>)
		Bytes,
		// Str(SmolStr)
		String,
		// Int(i64)
		Integer,
		// Uint(u64)
		UnsignedInteger,
		// F64(f64),
		Float,
		// Counter(i64),
		Counter,
		// Timestamp(i64),
		Timestamp,
		// Cursor(OpId),
		Cursor,
		// Boolean(bool)
		Boolean
	}

	public class ScalarValue
	{
		public ScalarValueType Type { get; }

		private readonly object? _value;

		private ScalarValue(ScalarValueType type, object? value)
		{
			this.Type = type;
			this._value = value;
		}

		public byte[] AsBytes() => GetValue<byte[]>(ScalarValueType.Bytes);

		public string AsString() => GetValue<string>(ScalarValueType.String);

		public long AsInteger() => GetValue<long>(ScalarValueType.Integer);

		public ulong AsUnsignedInteger() => GetValue<ulong>(ScalarValueType.UnsignedInteger);

		public double AsFloat() => GetValue<double>(ScalarValueType.Float);

		public long AsCounter() => GetValue<long>(ScalarValueType.Counter);

		public long AsTimestamp() => GetValue<long>(ScalarValueType.Timestamp);

		public OperationId AsCursor() => GetValue<OperationId>(ScalarValueType.Cursor);

		public bool AsBoolean() => GetValue<bool>(ScalarValueType.Boolean);

		private T GetValue<T>(ScalarValueType type)
		{
			if (this.Type != type)
			{
				throw new InvalidOperationException($"Can not convert scalar value type '{this.Type}' to '{type}'");
			}
			return (T)this._value!;
		}

		public static ScalarValue Null()
		{
			return new ScalarValue(ScalarValueType.Null, null);
		}

		public static ScalarValue Bytes(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			return new ScalarValue(ScalarValueType.Bytes, value);
		}

		public static ScalarValue String(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			return new ScalarValue(ScalarValueType.String, value);
		}

		public static ScalarValue Integer(long value)
		{
			return new ScalarValue(ScalarValueType.Integer, value);
		}

		public static ScalarValue UnsignedInteger(ulong value)
		{
			return new ScalarValue(ScalarValueType.UnsignedInteger, value);
		}

		public static ScalarValue Float(double value)
		{
			return new ScalarValue(ScalarValueType.Float, value);
		}

		public static ScalarValue Counter(long value)
		{
			return new ScalarValue(ScalarValueType.Counter, value);
		}

		public static ScalarValue Timestamp(long value)
		{
			return new ScalarValue(ScalarValueType.Timestamp, value);
		}

		public static ScalarValue Cursor(OperationId value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			return new ScalarValue(ScalarValueType.Cursor, value);
		}

		public static ScalarValue Boolean(bool value)
		{
			return new ScalarValue(ScalarValueType.Boolean, value);
		}
	}
}
