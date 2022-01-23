using ForgetIt.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
	public class SQLiteOperationStore
	{
		private readonly SQLiteConnection _db;
		public SQLiteOperationStore(SQLiteConnection db)
		{
			this._db = db;
			this._db.CreateTable<OperationDbModel>();
		}

		public void Add(ObjectInfo info)
		{
			List<OperationDbModel> opEntities = info.Operations
				.Select(o => OperationDbModel.FromCommon(info.Id, info.Type, o))
				.ToList();
			this._db.InsertAll(opEntities, runInTransaction: true);
		}

		public ObjectInfo Get(string id, string type)
		{
			List<PatchOperation> operations = this._db.Table<OperationDbModel>()
				.Where(m => m.ObjectType == type && m.ObjectId == id)
				.Select(o => o.ToCommon())
				.ToList();

			return new ObjectInfo(id, type, operations);
		}
	}
}
