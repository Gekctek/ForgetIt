using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgetIt.Core
{
	public class ObjectInfo
	{
		public string Id { get; }
		public string Type { get; }
		public List<PatchOperation> Operations { get; }

		public ObjectInfo(string id, string type, List<PatchOperation> operations)
		{
			this.Id = id ?? throw new ArgumentNullException(nameof(id));
			this.Type = type ?? throw new ArgumentNullException(nameof(type));
			this.Operations = operations ?? throw new ArgumentNullException(nameof(operations));
		}
	}
}
