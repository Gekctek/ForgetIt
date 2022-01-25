using ForgetIt.Core;
using SQLite;
using System.Text.Json;

namespace Data
{
	public class OperationDbModel
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[Indexed]
		public string Type { get; set; }
		public string Json { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public static OperationDbModel FromCommon(int? id, string type, PatchOperation operation)
		{
			return new OperationDbModel
			{
				Id = id ?? 0,
				Type = type,
				Json = operation.ToJson()
			};
		}

		public PatchOperation ToCommon()
		{
			return PatchOperation.ParseJson(this.Json);
		}
	}
}