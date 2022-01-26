using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Automerge
{
	public class JsonUtil
	{
		public static JsonNode Clone(JsonNode value)
		{
			return JsonNode.Parse(value.ToJsonString()) ?? throw new Exception("Couldn't clone json");
		}
	}
}
