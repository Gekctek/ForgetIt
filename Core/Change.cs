using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Automerge
{
    public class Change
    {
        [JsonPropertyName("ops")]
        public List<Operation> Operations { get; }

        [JsonPropertyName("actor")]
        public ActorId ActorId { get; }

        [JsonPropertyName("hash")]
        // TODO ignore if null
        public ChangeHash? ChangeHash { get; }

        [JsonPropertyName("seq")]
        public long Seq { get; } // TODO better name

        [JsonPropertyName("startOp")]
        public ulong StartOp { get; } // TODO better name

        [JsonPropertyName("time")]
        public long LogicalTime { get; }

        [JsonPropertyName("message")]
        public string? Message { get; }
        
        [JsonPropertyName("deps")]
        public List<ChangeHash> Deps { get; } // TODO better name
        
        [JsonPropertyName("extra_bytes")]
        // TODO skip if empty
        public byte[] ExtraBytes { get; }

        public Change(
            List<Operation> operations,
            ActorId actorId,
            ChangeHash? changeHash,
            long seq,
            ulong startOp,
            long logicalTime,
            string? message,
            List<ChangeHash> deps,
            byte[] extraBytes)
        {
            this.Operations = operations;
            this.ActorId = actorId;
            this.ChangeHash = changeHash;
            this.Seq = seq;
            this.StartOp = startOp;
            this.LogicalTime = logicalTime;
            this.Message = message;
            this.Deps = deps;
            this.ExtraBytes = extraBytes;
        }
    }

    public class ChangeHash
    {
        public byte[] Hash { get; }

        public ChangeHash(byte[] hash)
        {
            if(hash.Length != 32)
            {
                throw new ArgumentException("Hash length must be 32 bytes");
            }
            this.Hash = hash;
        }
    }

    public class ActorId
    {
        public byte[] Value { get; }

        public ActorId(byte[] value)
        {
            if(value.Length != 16)
            {
                throw new ArgumentException("Actor id length must be 16 bytes");
            }
            this.Value = value;
        }
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

        public ScalarValue AsSetValue => GetValue<>(OperationType.Set);
        

        private T GetValue<T>(OperationType type)
        {
            if(this.Type != type)
            {
                throw new InvalidOperationException($"Cannot convert operation type '{this.Type}' to '{type}'");
            }
            return (T)this._value;
        }

		Make(ObjType),
		/// Perform a deletion, expanding the operation to cover `n` deletions (multiOp).
		Del(NonZeroU32),
		Inc(i64),
		Set(ScalarValue),
		MultiSet(ScalarValues),
    }

    public class ObjectId
    {
        // If null, then it is pointing at the root
        public OperationId? OperationId { get; }
    }

    public class OperationId
    {
        public ulong Seq {get;}
        public ActorId ActorId {get;}
    }

    public abstract class Key
    {

    }

    public class MapKey : Key
    {
        public string Value {get;}
    }

    public class SeqKey : Key
    {
        // If null, then it is pointing at the head
        public OperationId? OperationId { get; }
    }

    public enum ObjectType
    {
        Map,
        Table,
        List,
        Text,
    }

	public enum OperationType
	{
		Make,
		Delete,
		Increment,
		Set,
		MultiSet
	}

    public class ScalarValue
    {

    }
    
    public class ScalarValueType
    {
        Null,
        Bytes(Vec<u8>),
        Str(SmolStr),
        Int(i64),
        Uint(u64),
        F64(f64),
        Counter(i64),
        Timestamp(i64),
        Cursor(OpId),
        Boolean(bool)
    }
}