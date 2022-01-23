using Comet;
using Data;
using ForgetIt.Core;
using SQLite;
using System.Collections.Generic;
using View = Comet.View;

namespace App;


public class MainPage : View
{

	[State]
	readonly CometRide comet = new();

	private State _state;
	private SQLiteOperationStore _store;

	public MainPage()
	{
		_store = new SQLiteOperationStore(new SQLiteConnection("Operations.db"));

		ObjectInfo objectInfo = _store.Get("1", "note");

		_state = new State(objectInfo.Id, objectInfo.Type, objectInfo.Operations);
	}

	[Body]
	View body()
		=> new VStack {
				new Text(()=> comet.Text)
					.Frame(width:300)
					.LineBreakMode(LineBreakMode.CharacterWrap),

				new Comet.CheckBox(() => comet.HasDueDate),

				new Comet.DatePicker(()=> comet.DueDate)
					.Enabled(() => comet.HasDueDate)
					.Frame(width:300)
					.LineBreakMode(LineBreakMode.CharacterWrap),

				new Comet.Button("Save", Save)
					.Frame(height:44)
					.Margin(8)
					.Color(Colors.White)
					.Background(Colors.Green)
				.RoundedBorder(color:Colors.Blue)
				.Shadow(Colors.Grey,4,2,2),
		};

	private void Save()
	{
		PatchOperation operation1 = _state.Update<Note, string>(n => n.Text, comet.Text);
		PatchOperation operation2 = _state.Update<Note, DateTime?>(n => n.DueDate, comet.HasDueDate ? comet.DueDate : null);

		
		_store.Add(new ObjectInfo(_state.Id, _state.Type, new List<PatchOperation> { operation1, operation2 }));
	}

	public class CometRide : BindingObject
	{
		public string? Text { get; set; }
		public bool HasDueDate { get; set; }
		public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(1);
	}
}


