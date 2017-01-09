using System;

namespace AlexSandros.DemoApps.Rates
{
	public static class LogProvider
	{
		private static readonly Lazy<ILog> m_Logger =
			new Lazy<ILog>(() => new Log(log4net.LogManager.GetLogger(typeof (LogProvider))));
		/// <summary>
		/// Общий лог
		/// </summary>
		public static ILog Logger => m_Logger.Value;
	}
}