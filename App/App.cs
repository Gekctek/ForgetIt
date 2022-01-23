using Comet;
using Microsoft.Maui.Hosting;
using View = Comet.View;

namespace ForgetIt.App;
public class App : CometApp
{
	[Body]
	View view() => new MainPage();

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseCometApp<App>()
			.ConfigureFonts(fonts => {
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
		//-:cnd
#if DEBUG
			builder.EnableHotReload();
#endif
		//+:cnd
		return builder.Build();
	}
}
