using ForgetIt.Core;

namespace App
{
	public partial class MainPage : ContentPage
	{
		public Note Note { get; set; }
		private readonly State<Note> state;


		public MainPage()
		{
			Note = new Note("1", "me", DateTime.UtcNow, null, null);
			state = new State<Note>();
			state.Update(Note);

			state.Update(new Note("1", "you", DateTime.UtcNow, "Test Text", null));

			Note note = state.Build();
			InitializeComponent();
		}

		private void OnSaveClicked(object sender, EventArgs e)
		{
			state.Update(Note);
		}
	}
}