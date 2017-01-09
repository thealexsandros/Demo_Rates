using System;
using System.Collections.Generic;

namespace AlexSandros.DemoApps.Rates
{
	public interface IYearRatesStorage
	{
		IEnumerable<KeyValuePair<string, float>> this[DateTime date] { get; }
		bool TryAddYear(short year, IEnumerable<string> lines, IParser parser, IConfiguration config, ILog log);
	}
}