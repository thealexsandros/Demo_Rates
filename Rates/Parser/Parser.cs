using System;
using System.Globalization;

namespace AlexSandros.DemoApps.Rates
{
	public class Parser: IParser
	{
		private readonly IConfiguration m_Config;

		public Parser(IConfiguration config)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			m_Config = config;
		}

		/// <summary>
		/// Разобрать дату (в формате файла) согласно параметрам
		/// </summary>
		/// <exception cref="InvalidOperationException">Parser is not configured</exception>
		public DateTime? ParseFileDateTime(string s)
		{
			DateTime date;
			if (DateTime.TryParseExact(
				s,
				m_Config.FileDateFormat,
				m_Config.ValuesCulture,
				DateTimeStyles.None,
				out date
				))
			{
				return date;
			}
			return null;
		}

		/// <summary>
		/// Разобрать курс согласно параметрам
		/// </summary>
		/// <exception cref="InvalidOperationException">Parser is not configured</exception>
		public float? ParseFloat(string s)
		{
			float parseResult;
			if (float.TryParse(
				s,
				NumberStyles.Float,
				m_Config.ValuesCulture,
				out parseResult))
			{
				return parseResult;
			}

			return null;
		}

		/// <summary>
		/// Разобрать дату (в формате UI) согласно параметрам
		/// </summary>
		/// <exception cref="InvalidOperationException">Parser is not configured</exception>
		public DateTime? ParseUiDateTime(string stringToParse, string nowString)
		{
			if (string.IsNullOrWhiteSpace(nowString))
				throw new ArgumentNullException(nameof(nowString));
			if (nowString.Equals(stringToParse, StringComparison.OrdinalIgnoreCase))
			{
				return DateTime.Today;
			}
			DateTime res;
			if (DateTime.TryParseExact(
				stringToParse,
				m_Config.UiDateFormat,
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out res))
			{
				return res;
			}
			return null;

		}

		/// <summary>
		/// Разобрать год
		/// </summary>
		public short? ParseUiYear(string stringToParse, string nowString)
		{
			if (string.IsNullOrWhiteSpace(nowString))
				throw new ArgumentNullException(nameof(nowString));
			if (nowString.Equals(stringToParse, StringComparison.OrdinalIgnoreCase))
			{
				return (short)DateTime.Today.Year;
			}
			int res;
			if (int.TryParse(
				stringToParse,
				out res) && res >= DateTime.MinValue.Year && res <= DateTime.MaxValue.Year)
			{
				return (short)new DateTime(res, 1, 1).Year;
			}
			return null;
		}
	}
}