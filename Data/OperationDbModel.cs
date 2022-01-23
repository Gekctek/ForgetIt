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
		public string ObjectId { get; set; }
		[Indexed]
		public string ObjectType { get; set; }
		public string Json { get; set; }

		public static OperationDbModel FromCommon(string id, string type, PatchOperation operation)
		{
			return new OperationDbModel
			{
				ObjectId = id,
				ObjectType = type,
				Json = operation.ToJson()
			};
		}

		public PatchOperation ToCommon()
		{
			return JsonSerializer.Deserialize<PatchOperation>(this.Json) ?? throw new Exception($"Invalid json '{this.Json}'");
		}
	}
}