using Data;
using ForgetIt.Core;
using SQLite;
using System.Text.Json;

var store = new SQLiteOperationStore(new SQLiteConnection("Operations.db"));


ObjectInfo objectInfo = store.Get("1", "note");

State state;
if (!objectInfo.Operations.Any())
{
	var newNote = new Note("me", DateTime.UtcNow, "Hello World!", DateTime.UtcNow);
	state = new State("1", "note");
	List<PatchOperation> operations = state.Update(newNote);
	if (operations.Any())
	{
		var info = new ObjectInfo(objectInfo.Id, objectInfo.Type, objectInfo.Operations);
		store.Add(info);
	}
}
else
{
	state = new State(objectInfo.Id, objectInfo.Type, objectInfo.Operations);
}


Note note = state.Build<Note>();



foreach (PatchOperation operation in state.Operations)
{
	Console.WriteLine(operation.ToString());
}
Console.WriteLine(JsonSerializer.Serialize(note));