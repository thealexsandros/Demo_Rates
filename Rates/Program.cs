using System;
using System.IO;
using System.Net;

namespace AlexSandros.DemoApps.Rates
{
	class Program
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException +=
				(source, eargs) =>
				{
					LogHelper.Error(LogProvider.Logger, eargs.ExceptionObject as Exception);
					UiHelper.Finish(1);
				};
			UiHelper.WriteInfo();

			var config = XmlConfiguration.Instance;
			var log = LogProvider.Logger;
			var parser = new Parser(config);
			var storage = Storage.Instance;

			do
			{
				LoadFromWeb(
					UiHelper.SelectYear(args, parser),
					storage,
					parser,
					config,
					log);
			} while (UiHelper.YNCycle());

			do
			{
				var result = storage[UiHelper.SelectDate(args, config, parser)];
				if (result == null)
					Console.WriteLine("No rates found for requested date.");
				else
					foreach (var pair in result)
						Console.WriteLine($"{pair.Key} = {pair.Value}");
			} while (UiHelper.YNCycle());

			UiHelper.Finish();
		}

		private static void LoadFromWeb(short year, IYearRatesStorage storage, IParser parser, IConfiguration config, ILog log)
		{
			using (var client = new WebClient())
			{
				storage.TryAddYear(
					year,
					EnumerableHelper.ReadLines(
							() =>
							{
								var uri = $"{config.RatesUri}?{config.RatesUriYearParamName}={year}";
								if (log?.IsInfoEnabled ?? false)
									log.Info($"Fetching data from {uri}...");
								return new System.IO.StreamReader(client.OpenRead(uri));
							}
							),
					parser,
					config,
					log);
			}
		}
	}
}
