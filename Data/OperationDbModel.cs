using ForgetIt.Core;
using SQLite;
using System.Text.Json;

namespace Data
{
	public class OperationDbModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[Indexed]
		public string Type { get; set; }
		public string Json { get; set; }

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