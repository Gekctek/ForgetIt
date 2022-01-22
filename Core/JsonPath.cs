using System.Text;

namespace ForgetIt.Core
{
	public class JsonPath
	{
		public JsonPathSegment[] Segments { get; }

		public JsonPath(JsonPathSegment[] segments)
		{
			this.Segments = segments ?? throw new ArgumentNullException(nameof(segments));
		}

		public JsonPath Parse(string path)
		{
			string[] pathSegments = path.Split('/');
			return Parse(pathSegments);
		}

		public JsonPath Parse(string[] pathSegments)
		{
			JsonPathSegment[] segments = pathSegments
				.Select(JsonPathSegment.Parse)
				.ToArray();
			return new JsonPath(segments);
		}

		public override string ToString()
		{
			var b = new StringBuilder();
			foreach(JsonPathSegment segment in this.Segments)
			{
				b.Append(segment.ToString());
			}
			return b.ToString();
		}

		public JsonPath Add(string property)
		{
			var segments = new JsonPathSegment[this.Segments.Length + 1];
			segments[this.Segments.Length] = JsonPathSegment.Property(property);
			return new JsonPath(segments);
		}

		public static JsonPath Empty()
		{
			return new JsonPath(new JsonPathSegment[0]);
		}
	}

	public class JsonPathSegment
	{
		public bool IsIndex { get; }
		private object _value;

		private JsonPathSegment(bool isIndex, object value)
		{
			this.IsIndex = isIndex;
			this._value = value;
		}

		public int AsIndex
		{
			get
			{
				if (this.IsIndex)
				{
					return (int)_value;
				}
				throw new InvalidOperationException("Json path segment is a property, not an index");
			}
		}

		public string AsProperty
		{
			get
			{
				if (!this.IsIndex)
				{
					return (string)_value;
				}
				throw new InvalidOperationException("Json path segment is an index, not a property");
			}
		}

		public override string ToString()
		{
			if (this.IsIndex)
			{
				return this.AsIndex.ToString();
			}
			return this.AsProperty;
		}

		public static JsonPathSegment Index(int index)
		{
			return new JsonPathSegment(true, index);
		}

		public static JsonPathSegment Property(string property)
		{
			if (property.Contains('/'))
			{
				throw new InvalidOperationException("Path properties can't contain '/'");
			}
			return new JsonPathSegment(false, property);
		}

		public static JsonPathSegment Parse(string segment)
		{
			if (int.TryParse(segment, out int index))
			{
				return JsonPathSegment.Index(index);
			}
			return JsonPathSegment.Property(segment);
		}
	}
}