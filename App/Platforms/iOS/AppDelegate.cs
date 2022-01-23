using Foundation;
using Microsoft.Maui.Hosting;

namespace ForgetIt.App;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => App.CreateMauiApp();
}

