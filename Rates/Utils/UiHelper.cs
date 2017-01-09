using System;
using System.Diagnostics;
using System.Reflection;

namespace AlexSandros.DemoApps.Rates
{
	internal static class UiHelper
	{
		internal static void WriteInfo()
		{
			var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
			Console.WriteLine($"{versionInfo.ProductName} {versionInfo.ProductVersion} by {versionInfo.CompanyName}");
		}
		internal static void Finish(int exitCode = 0)
		{
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
			Environment.Exit(exitCode);
		}

		private const string NOW = "NOW";
		private const string GET_NOW = "GET:NOW";
		private const string LOAD_NOW = "LOAD:NOW";


		/// <exception cref="ArgumentNullException"><paramref name="parser"/> is null</exception>
		internal static short SelectYear(string[] args, IParser parser)
		{
			if (parser == null)
				throw new ArgumentNullException(nameof(parser));
			for (var i = 0; i < (args?.Length ?? 0); i++)
			{
				if (args[i] == null) continue;
				var val = parser.ParseUiYear(args[i], LOAD_NOW);
				if (val.HasValue)
				{
					args[i] = null;
					return val.Value;
				}
			}
			while (true)
			{
				Console.Write($"Input year for loading rates or {NOW} for current year: ");
				var val = parser.ParseUiYear(Console.ReadLine(), NOW);
				if (val.HasValue)
					return val.Value;
			}
		}

		/// <exception cref="ArgumentNullException"><paramref name="parser"/> is null</exception>
		internal static DateTime SelectDate(string[] args, IConfiguration config, IParser parser)
		{
			if (parser == null)
				throw new ArgumentNullException(nameof(parser));
			for (var i = 0; i < (args?.Length ?? 0); i++)
			{
				if (args[i] == null) continue;
				var val = parser.ParseUiDateTime(args[i], GET_NOW);
				if (val.HasValue)
				{
					args[i] = null;
					return val.Value;
				}
			}
			while (true)
			{
				Console.Write($"Input date for querying rates or {NOW} for today (format is {config.UiDateFormat}): ");
				var val = parser.ParseUiDateTime(Console.ReadLine(), NOW);
				if (val.HasValue)
					return val.Value;
			}
		}

		internal static bool YNCycle()
		{
			while (true)
			{
				Console.Write("Repeat this action again (Y/N)? ");
				switch ((Console.ReadLine() ?? string.Empty).ToUpper())
				{
					case "Y":
						return true;
					case "N":
						return false;
				}
			}
		}

	}
}