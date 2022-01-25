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

		public void Add(int id, string type, IEnumerable<PatchOperation> operations)
		{
			List<OperationDbModel> opEntities = operations
				.Select(o => OperationDbModel.FromCommon(id, type, o))
				.ToList();
			this._db.InsertAll(opEntities, runInTransaction: true);
		}

		public void Add(int id, string type, params PatchOperation[] operations)
		{
			Add(id, type, (IEnumerable<PatchOperation>)operations);
		}

		public ObjectInfo Get(int id, string type)
		{
			List<PatchOperation> operations = this._db.Table<OperationDbModel>()
				.Where(m => m.Type == type && m.Id == id)
				.Select(o => o.ToCommon())
				.ToList();

			return new ObjectInfo(id, type, operations);
		}
	}
}
