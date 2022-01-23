using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ForgetIt.Core;

public class Note : StatefulObject
{
	public string Id { get; set; }
	public string Owner { get; set; }
	public DateTime CreateDate { get; set; }
	public string? Text { get; set; }
	public DateTime? DueDate { get; set; }

	public Note(string id, string owner, DateTime createDate, string? text, DateTime? dueDate)
	{
		Id = id ?? throw new ArgumentNullException(nameof(id));
		Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		CreateDate = createDate;
		Text = text;
		DueDate = dueDate;
	}

	protected override JsonDocument BuildSnapshot()
	{
		return JsonSerializer.SerializeToDocument(this);
	}
}
