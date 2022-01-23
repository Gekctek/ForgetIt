using Data;
using ForgetIt.Core;

namespace App
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		private void OnSaveClicked(object sender, EventArgs e)
		{
		}
	}

	public class OperationModel
	{
		public OperationType Type { get; set; }
		public string Path { get; set; }
		public string Value { get; set; }
		public string From { get; set; }
	}
}