using System;
using System.Collections.Generic;

namespace AlexSandros.DemoApps.Rates
{
	public interface IYearRates
	{
		short Year { get; }
		DateTime? FirstDate { get; }
		IEnumerable<KeyValuePair<string, float>> this[DateTime date] { get; }
	}
}