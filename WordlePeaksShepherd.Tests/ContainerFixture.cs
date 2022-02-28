using Microsoft.Extensions.DependencyInjection;
using WordlePeaksShepherd.Services;

namespace WordlePeaksShepherd.Tests;

//ncrunch: no coverage start
public sealed class ContainerFixture
{
	public ServiceProvider ServiceProvider { get; private set; }

	public ContainerFixture()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddTransient<IWordService, WordlePeaksWordService>(
			serviceProvider =>
			{
				using var testWordsFile = new StreamReader(@"Data\test-potential-words.txt");

				return new WordlePeaksWordService(testWordsFile.BaseStream);
			});
		serviceCollection.AddTransient<ILetterService, ShepherdLetterService>();
		serviceCollection.AddTransient<IShepherdService, ShepherdService>();

		ServiceProvider = serviceCollection.BuildServiceProvider();
	}
}
//ncrunch: no coverage end
