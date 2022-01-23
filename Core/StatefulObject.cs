using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForgetIt.Core
{
	public abstract class StatefulObject
	{
		[JsonIgnore]
		private JsonDocument? _snapshotCache = null;

		protected abstract JsonDocument BuildSnapshot();

		public JsonDocument GetSnapshot()
		{
			if (_snapshotCache == null)
			{
				_snapshotCache = BuildSnapshot();
			}
			return _snapshotCache;
		}
	}
}
