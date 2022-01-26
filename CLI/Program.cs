using Automerge;
using Automerge.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;


namespace CLI
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			using (AutomergeBackend automerge = AutomergeBackend.Init())
			{
				var id = ObjectId.Root();
				var state = new State(id);
				state.Set("Test", ScalarValue.Boolean(true));

				Change change = state.BuildChange();

				automerge.ApplyLocalChange(change);

				Console.WriteLine();
				Console.ReadLine();
			}
		}
	}
}
// var store = new SQLiteOperationStore(new SQLiteConnection("Operations.db"));


// ObjectInfo objectInfo = store.Get("1", "note");

// State state;
// if (!objectInfo.Operations.Any())
// {
// 	var newNote = new Note("me", DateTime.UtcNow, "Hello World!", DateTime.UtcNow);
// 	state = new State("1", "note");
// 	List<PatchOperation> operations = state.Update(newNote);
// 	if (operations.Any())
// 	{
// 		var info = new ObjectInfo(objectInfo.Id, objectInfo.Type, operations);
// 		store.Add(info);
// 	}
// }
// else
// {
// 	state = new State(objectInfo.Id, objectInfo.Type, objectInfo.Operations);
// }


// Note note = state.Build<Note>();



// foreach (PatchOperation operation in state.Operations)
// {
// 	Console.WriteLine(operation.ToString());
// }
// Console.WriteLine(JsonSerializer.Serialize(note));