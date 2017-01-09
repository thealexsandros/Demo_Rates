using System;
using System.Collections.Generic;
using System.Linq;

namespace AlexSandros.DemoApps.Rates
{
	public class Storage: IYearRatesStorage
	{
		private Storage() {}
		private static readonly Lazy<IYearRatesStorage> m_Instance = new Lazy<IYearRatesStorage>(() => new Storage());
		public static IYearRatesStorage Instance => m_Instance.Value;

		private readonly List<IYearRates> m_Years = new List<IYearRates>();

		/// <summary>
		/// Курсы на дату
		/// </summary>
		public IEnumerable<KeyValuePair<string, float>> this[DateTime date]
		{
			get
			{
				var items =
					m_Years.Where(item => item.Year <= date.Year && item.FirstDate != null)
						.OrderByDescending(item => item.Year);
				return
					(items.FirstOrDefault(item => item.FirstDate <= date) 
					?? items.FirstOrDefault(item => item.Year <= date.Year))?[date];
			}
		}

		/// <summary>
		/// Загрузить в хранилище данные на год
		/// </summary>
		/// <returns>Удалось ли загрузить данные на год</returns>
		public bool TryAddYear(short year, IEnumerable<string> lines, IParser parser, IConfiguration config, ILog log)
		{
			if (m_Years.Any(item => item.Year == year))
			{
				if (log?.IsWarnEnabled ?? false)
					log.Warn($"Entry for year {year} already exists.");
				return false;
			}

			IYearRates newItem;
			try
			{
				newItem =
					new YearRates(
						year,
						lines,
						config,
						parser,
						log);
			}
			catch (Exception e)
			{
				if (log?.IsErrorEnabled ?? false)
					log?.Error($"Error while trying to add entry for year {year}: {e.Message}");
				return false;
			}
			m_Years.Add(newItem);
			return true;
		}
	}
}