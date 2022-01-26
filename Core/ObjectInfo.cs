using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automerge
{
	public class ObjectInfo
	{
		public int Id { get; }
		public string Type { get; }
		public List<Operation> Operations { get; }

		public ObjectInfo(int id, string type, List<Operation> operations)
		{
			this.Id = id;
			this.Type = type ?? throw new ArgumentNullException(nameof(type));
			this.Operations = operations ?? throw new ArgumentNullException(nameof(operations));
		}
	}
}
