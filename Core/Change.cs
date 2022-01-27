using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Automerge
{
	public class Change
	{
		public IReadOnlyList<Operation> Operations { get; }
		public ActorId ActorId { get; }
		public ChangeHash? ChangeHash { get; }
		public long Seq { get; } // TODO better name
		public ulong StartOp { get;} // TODO better name
		public long LogicalTime { get; }
		public string? Message { get; }
		public IReadOnlyList<ChangeHash> Dependencies { get; } // TODO better name
		public byte[]? ExtraBytes { get; }

		public Change(
			IEnumerable<Operation> operations,
			ActorId actorId,
			ChangeHash? changeHash,
			long seq,
			ulong startOp,
			long logicalTime,
			string? message,
			IEnumerable<ChangeHash> dependencies,
			byte[]? extraBytes = null)
		{
			this.Operations = operations?.ToList() ?? throw new ArgumentNullException(nameof(operations));
			this.ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
			this.ChangeHash = changeHash;
			this.Seq = seq;
			this.StartOp = startOp;
			this.LogicalTime = logicalTime;
			this.Message = message;
			this.Dependencies = dependencies?.ToList() ?? throw new ArgumentNullException(nameof(dependencies));
			this.ExtraBytes = extraBytes;
		}
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