using System;
using System.Collections.Generic;
using System.IO;

namespace AlexSandros.DemoApps.Rates
{
	/// <summary>
	/// Утилиты для загрузки файла
	/// </summary>
	public static class EnumerableHelper
	{
		/// <summary>
		/// Пребразовать поток в набор строк
		/// </summary>
		/// <param name="readerProvider">Поставщик читателя</param>
		/// <returns></returns>
		public static IEnumerable<string> ReadLines(Func<StreamReader> readerProvider)
		{
			if (readerProvider == null)
				yield break;
			using (var reader = readerProvider())
			{
				if (reader == null)
					yield break;
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					yield return line;
				}
			}
		}

		/// <summary>
		/// Разбить строку по подстрокам
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="config"/> is null</exception>
		public static IEnumerable<string> SplitStringByConfig(this string s, IConfiguration config)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (s == null)
				yield break;
			var startIdx = 0;
			while (startIdx > -1)
			{
				var endIdx = s.IndexOf(config.Separator, startIdx, StringComparison.OrdinalIgnoreCase);
				yield return
					endIdx > -1
						? s.Substring(startIdx, endIdx - startIdx)
						: s.Substring(startIdx);
				startIdx = endIdx +
					(endIdx > -1
						? config.Separator.Length
						: 0);
			}
		} 
	}
}
