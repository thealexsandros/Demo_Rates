using System;
using System.Globalization;
using System.Text.RegularExpressions;
using CM = System.Configuration.ConfigurationManager;

namespace AlexSandros.DemoApps.Rates
{
	public class XmlConfiguration: IConfiguration
	{
		private XmlConfiguration() { }

		private static readonly Lazy<IConfiguration> m_Instance = new Lazy<IConfiguration>(() => new XmlConfiguration());
		public static IConfiguration Instance => m_Instance.Value;

		/// <summary>
		/// Ссылка для получения данных о курсах
		/// </summary>
		public string RatesUri { get; } = CheckPattern(CM.AppSettings[nameof(RatesUri)], ".+");
		/// <summary>
		/// Имя параметра "год"
		/// </summary>
		public string RatesUriYearParamName {get; } = CheckPattern(CM.AppSettings[nameof(RatesUriYearParamName)], "^[a-z A-Z]+$");
		/// <summary>
		/// Разделитель значений
		/// </summary>
		public string Separator { get; } = CheckPattern(CM.AppSettings[nameof(Separator)], ".+");
		/// <summary>
		/// Формат даты в загружаемом файле
		/// </summary>
		public string FileDateFormat { get; } = CheckPattern(CM.AppSettings[nameof(FileDateFormat)], ".+");
		/// <summary>
		/// Формат даты в интерфейсе пользователя
		/// </summary>
		public string UiDateFormat { get; } = CheckPattern(CM.AppSettings[nameof(UiDateFormat)], ".+");
		/// <summary>
		/// Культура используемых в файле значений
		/// </summary>
		public CultureInfo ValuesCulture { get; } = CultureInfo.GetCultureInfo(CheckPattern(CM.AppSettings[nameof(ValuesCulture)], ".+"));

		private static string CheckPattern(string input, string pattern)
		{
			if (!Regex.IsMatch(input, pattern))
				throw new ArgumentException("Parameter is invalid", nameof(input));
			return input;
		}

	}
}