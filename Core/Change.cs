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
		public List<Operation> Operations { get; init; }

		[JsonPropertyName("actor")]
		public ActorId ActorId { get; init; }

		[JsonPropertyName("hash")]
		// TODO ignore if null
		public ChangeHash? ChangeHash { get; init; }

		[JsonPropertyName("seq")]
		public long Seq { get; init; } // TODO better name

		[JsonPropertyName("startOp")]
		public ulong StartOp { get; init; } // TODO better name

		[JsonPropertyName("time")]
		public long LogicalTime { get; init; }

		[JsonPropertyName("message")]
		public string? Message { get; init; }

		[JsonPropertyName("deps")]
		public List<ChangeHash> Deps { get; init; } // TODO better name

		[JsonPropertyName("extra_bytes")]
		// TODO skip if empty
		public byte[] ExtraBytes { get; init; }
	}

	public class ChangeHash
	{
		public byte[] Hash { get; init; }

		public ChangeHash(byte[] hash)
		{
			if (hash.Length != 32)
			{
				throw new ArgumentException("Hash length must be 32 bytes");
			}
			this.Hash = hash;
		}
	}

	public class ActorId
	{
		public byte[] Value { get; init; }

		public ActorId(byte[] value)
		{
			if (value.Length != 16)
			{
				throw new ArgumentException("Actor id length must be 16 bytes");
			}
			this.Value = value;
		}
	}

	public class ObjectId
	{
		// If null, then it is pointing at the root
		public OperationId? OperationId { get; init; }

		public ObjectId(OperationId? operationId)
		{
			this.OperationId = operationId;
		}

		public static ObjectId Root()
		{
			return new ObjectId(null);
		}
	}

	public class OperationId
	{
		public ulong Seq { get; init; }
		public ActorId ActorId { get; init; }

		public OperationId(ulong seq, ActorId actorId)
		{
			this.Seq = seq;
			this.ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
		}
	}
}