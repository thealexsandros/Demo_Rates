using System;

namespace AlexSandros.DemoApps.Rates
{
	public interface IParser
	{
		/// <summary>
		/// Разобрать дату (в формате файла) согласно параметрам
		/// </summary>
		DateTime? ParseFileDateTime(string s);

		/// <summary>
		/// Разобрать курс согласно параметрам
		/// </summary>
		float? ParseFloat(string s);

		/// <summary>
		/// Разобрать дату (в формате UI) согласно параметрам
		/// </summary>
		DateTime? ParseUiDateTime(string stringToParse, string nowString);

		/// <summary>
		/// Разобрать год
		/// </summary>
		short? ParseUiYear(string stringToParse, string nowString);
	}
}