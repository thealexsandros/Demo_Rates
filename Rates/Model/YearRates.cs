using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlexSandros.DemoApps.Rates
{
	public class YearRates: IYearRates
	{
		#region ctor
		/// <summary>
		/// Инициализировать хранилище данных за год
		/// </summary>
		/// <param name="year">Год</param>
		/// <param name="dataLines">Строки файла</param>
		/// <param name="config">Настройки</param>
		/// <param name="parser">Парсер</param>
		/// <param name="log">Лог</param>
		/// <exception cref="ArgumentNullException"><paramref name="dataLines"/> or <paramref name="parser"/> is null</exception>
		public YearRates(short year, IEnumerable<string> dataLines, IConfiguration config, IParser parser, ILog log)
		{
			if (dataLines == null)
				throw new ArgumentNullException(nameof(dataLines));
			if (parser == null)
				throw new ArgumentNullException(nameof(parser));
			Year = year;
			m_DaysInYear = new DateTime(Year, 12, 31).DayOfYear;
			var slotCount = 0;
			Task.WaitAll(
				dataLines.Select(
					(s, idx) =>
					{
						if (idx == 0) // обработка заголовка
						{
							ProcessHeader(s, config);
							InitData();
							return Task.CompletedTask;
						}
						return
							Task.Run(
								LogHelper.WrapWithDebugLogging(
									action: () => ProcessLineParallel(s, ref slotCount, config, parser, log),
									description: $"processing line #{idx}",
									log: log,
									hideErrors: true));
					}
					).ToArray());
			if (m_CurrencySpec == null)
				throw new ArgumentException("No currencies were provided.", nameof(dataLines));
			if (slotCount == 0)
				throw new ArgumentException("No rates were provided.", nameof(dataLines));
			TrimData(slotCount);
		}
		#endregion

		/// <summary>
		/// Год
		/// </summary>
		public short Year { get; }
		/// <summary>
		/// Число дней в году
		/// </summary>
		private readonly int m_DaysInYear;

		#region FirstDate
		private bool m_FirstDateSet = false;
		private DateTime? m_FirstDate;
		/// <summary>
		/// Первая дата, на которую есть курсы
		/// </summary>
		public DateTime? FirstDate
		{
			get
			{
				// новые даты динамичеки добавлять нельзя, поэтому будем считать это поле 1 раз лениво
				if (m_FirstDateSet)
					return m_FirstDate;
				
				for(var i = 0; i < m_DaysInYear; i++)
					if (BitConverter.ToUInt16(m_Data, i*sizeof (ushort)) != 0)
					{
						m_FirstDate = new DateTime(Year, 1, 1).AddDays(i);
						break;
					}
				m_FirstDateSet = true;
				return m_FirstDate;
			}
		}
		#endregion

		#region m_Data init helpers
		/// <summary>
		/// Инициализация внутреннего буфера исходя из количества дней в году и валют
		/// </summary>
		private void InitData()
		{
			if (m_CurrencySpec == null)
				throw new Exception("Currency list must be initialized before initializing data.");
			m_Data = new byte[m_DaysInYear * (sizeof(ushort) + m_CurrencySpec.Length * sizeof(float))];
		}

		/// <summary>
		/// Убрать неиспользуемые слоты курсов из буфера
		/// </summary>
		/// <param name="slotCount">Количество задействованных слотов</param>
		private void TrimData(int slotCount)
		{
			Array.Resize(ref m_Data, m_DaysInYear * sizeof(ushort) + slotCount * m_CurrencySpec.Length * sizeof(float));
		}
		#endregion

		#region Processing
		/// <summary>
		/// Разбор заголовка (инициализация списка валют)
		/// </summary>
		/// <exception cref="ArgumentException"><paramref name="headerString"/> не удалось разобрать</exception>
		private void ProcessHeader(string headerString, IConfiguration config)
		{
			m_CurrencySpec = headerString.SplitStringByConfig(config)?.Skip(1).ToArray();
			if (0 == (m_CurrencySpec?.Length ?? 0))
				throw new ArgumentException($"Provided string [{headerString}] does not contain currency info.");
		}

		/// <summary>
		/// Обработать строку курсов (с возможностью параллельного вызова)
		/// </summary>
		/// <param name="lineString">Строка с курсами</param>
		/// <param name="slotCount">Текущее количество слотов</param>
		/// <exception cref="ArgumentNullException"><paramref name="parser"/> is null</exception>
		private void ProcessLineParallel(string lineString, ref int slotCount, IConfiguration config, IParser parser, ILog log)
		{
			if (parser == null)
				throw new ArgumentNullException(nameof(parser));
			var idx = 0;
			using (var ms = new MemoryStream(m_Data))
			using (var bw = new BinaryWriter(ms))
			{
				foreach (var s in lineString.SplitStringByConfig(config))
				{
					if (idx == 0) // обработка даты, подготовка к записи значений
					{
						var date = parser.ParseFileDateTime(s);
						if (date == null)
						{
							if (log?.IsWarnEnabled ?? false)
								log.Warn($"Unable to parse date from string [{s}]. Line will be ignored.");
							return;
						}
						if (date.Value.Year != Year)
						{
							if (log?.IsWarnEnabled ?? false)
								log.Warn($"Date from string [{s}] does not belong to year [{Year}]. Line will be ignored.");
							return;
						}
						int slotId = BitConverter.ToUInt16(m_Data, (date.Value.DayOfYear - 1) * sizeof(ushort));
						if (slotId != 0)
						{
							if (log?.IsWarnEnabled ?? false)
								log.Warn($"Rates for this date {date.Value.ToString(config.UiDateFormat)} are already filled. This line will be ignored.");
							return;
						}
						slotId = Interlocked.Increment(ref slotCount);
						bw.Seek((date.Value.DayOfYear - 1)*sizeof (ushort), SeekOrigin.Begin);
						bw.Write(slotId);
						bw.Seek(new DateTime(date.Value.Year, 12, 31).DayOfYear * sizeof(ushort)
							+ (slotId - 1) * m_CurrencySpec.Length * sizeof(float),
							SeekOrigin.Begin);
					}
					else if (idx > m_CurrencySpec.Length)
					{
						if (log?.IsWarnEnabled ?? false)
							log.Warn($"Record with index {idx}(0-based) has no matching currency in header and will be ignored.");
					}
					else
					{
						var rate = parser.ParseFloat(s);
						if (rate == null)
						{
							if (log?.IsWarnEnabled ?? false)
								log.Warn($"Unable to parse float from string [{s}]. NaN value will be used.");
							rate = float.NaN;
						}
						bw.Write(rate.Value);
					}
					idx++;
				}
				while (idx++ <= m_CurrencySpec.Length) // idx > 0 чтобы не писать пустые строки
				{
					if (log?.IsWarnEnabled ?? false)
						log.Warn($"No value exist for currency #{idx - 1}. NaN value will be used.");
					bw.Write(float.NaN);
				}
			}
		}
		#endregion

		/// <summary>
		/// Получить курсы на дату. Если курсов на дату нет, но есть раньше, вернутся последние актуальные
		/// </summary>
		public IEnumerable<KeyValuePair<string, float>> this[DateTime date]
		{
			get
			{
				var defaultResult = m_CurrencySpec.Select(item => new KeyValuePair<string, float>(item, float.NaN));
				if (date.Year < Year)
					return null;
				if (FirstDate == null || date < FirstDate)
					return defaultResult;
				// идём по всем слотам с конца
				for (var i = (date.Year == Year ? date.DayOfYear : m_DaysInYear) - 1; i >= 0; i--)
				{
					var slotId = BitConverter.ToUInt16(m_Data, i * sizeof(ushort));
					if (slotId > 0)
					{
						var start = m_DaysInYear*sizeof (ushort) + (slotId - 1)*m_CurrencySpec.Length*sizeof (float);
						return
							m_CurrencySpec.Select(
								(item, idx) =>
									new KeyValuePair<string, float>(
										item,
										BitConverter.ToSingle(m_Data, start + idx*sizeof (float))));
					}
				}
				return defaultResult;
			}
		}

		/// <summary>
		/// Список валют
		/// </summary>
		private string[] m_CurrencySpec;

		/// <summary>
		/// Сериализованные данные по курсам валют
		/// </summary>
		private byte[] m_Data;
	}
}