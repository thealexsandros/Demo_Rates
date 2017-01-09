using System.Globalization;

namespace AlexSandros.DemoApps.Rates
{
	public interface IConfiguration
	{
		/// <summary>
		/// Ссылка для получения данных о курсах
		/// </summary>
		string RatesUri { get; }
		/// <summary>
		/// Имя параметра "год"
		/// </summary>
		string RatesUriYearParamName { get; }
		/// <summary>
		/// Разделитель значений
		/// </summary>
		string Separator { get; }
		/// <summary>
		/// Формат даты в загружаемом файле
		/// </summary>
		string FileDateFormat { get; }
		/// <summary>
		/// Формат даты в интерфейсе пользователя
		/// </summary>
		string UiDateFormat { get; }
		/// <summary>
		/// Культура используемых в файле значений
		/// </summary>
		CultureInfo ValuesCulture { get; }
	}
}