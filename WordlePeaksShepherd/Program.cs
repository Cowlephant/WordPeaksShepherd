using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WordlePeaksShepherd;
using WordlePeaksShepherd.Services;
using WordlePeaksShepherd.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IWordService, WordService>(
	serviceProvider =>
	{
		using Stream wordsResourceStream = typeof(Program).Assembly.GetManifestResourceStream(
			"WordlePeaksShepherd.Data.words-potential.txt")!;

		return new WordService(wordsResourceStream);
	});
builder.Services.AddScoped<ILetterService, LetterService>();
builder.Services.AddScoped<IShepherdService, ShepherdService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
