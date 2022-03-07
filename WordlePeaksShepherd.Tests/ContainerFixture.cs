using Microsoft.Extensions.DependencyInjection;
using WordlePeaksShepherd.Services;
using WordlePeaksShepherd.Services.Interfaces;

namespace WordlePeaksShepherd.Tests;

//ncrunch: no coverage start
public sealed class ContainerFixture
{
	public ServiceProvider ServiceProvider { get; private set; }

	public ContainerFixture()
	{
		var services = new ServiceCollection();
		services.AddTransient<IWordService, WordService>(
			serviceProvider =>
			{
				using var testWordsFile = new StreamReader(@"Data\test-potential-words.txt");

				return new WordService(testWordsFile.BaseStream);
			});
		services.AddTransient<ILetterService, LetterService>();
		services.AddTransient<IShepherdService, ShepherdService>();

		ServiceProvider = services.BuildServiceProvider();
	}
}
//ncrunch: no coverage end
