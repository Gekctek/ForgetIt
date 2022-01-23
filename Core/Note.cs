using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ForgetIt.Core;

public class Note : StatefulObject
{
	public string Owner { get; set; }
	public DateTime CreateDate { get; set; }
	public string? Text { get; set; }
	public DateTime? DueDate { get; set; }

	[JsonConstructor]
	public Note()
	{

	}

	public Note(string owner, DateTime createDate, string? text, DateTime? dueDate)
	{
		Owner = owner;
		CreateDate = createDate;
		Text = text;
		DueDate = dueDate;
	}

	protected override JsonDocument BuildSnapshot()
	{
		return JsonSerializer.SerializeToDocument(this);
	}
}
