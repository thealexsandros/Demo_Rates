using System.Globalization;

namespace AlexSandros.DemoApps.Rates.Test
{
	public class ConfigurationStub: IConfiguration
	{
		public string RatesUri
			=> "https://www.cnb.cz/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/year.txt";

		public string RatesUriYearParamName => "year";

		public string Separator => "|";

		public string FileDateFormat => "dd.MMM yyyy";

		public string UiDateFormat => "dd.MM.yyyy";

		public CultureInfo ValuesCulture => CultureInfo.GetCultureInfo("en-US");
	}
}