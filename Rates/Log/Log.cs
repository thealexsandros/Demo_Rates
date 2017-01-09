using System;

namespace AlexSandros.DemoApps.Rates
{
	public class Log: ILog
	{
		private log4net.ILog m_Log;

		public Log(log4net.ILog log)
		{
			if (log == null)
				throw new ArgumentNullException(nameof(log));
			m_Log = log;
		}
		public void Debug(object message)
		{
			m_Log.Debug(message);
		}

		public void Info(object message)
		{
			m_Log.Info(message);
		}

		public void Warn(object message)
		{
			m_Log.Warn(message);
		}

		public void Error(object message)
		{
			m_Log.Error(message);
		}

		public void Fatal(object message)
		{
			m_Log.Fatal(message);
		}

		public bool IsDebugEnabled => m_Log.IsDebugEnabled;

		public bool IsInfoEnabled => m_Log.IsInfoEnabled;

		public bool IsWarnEnabled => m_Log.IsWarnEnabled;

		public bool IsErrorEnabled => m_Log.IsErrorEnabled;

		public bool IsFatalEnabled => m_Log.IsFatalEnabled;

		public void Fatal(object message, Exception exception)
		{
			m_Log.Fatal(message, exception);
		}

		public void Error(object message, Exception exception)
		{
			m_Log.Error(message, exception);
		}

		public void Warn(object message, Exception exception)
		{
			m_Log.Warn(message, exception);
		}

		public void Info(object message, Exception exception)
		{
			m_Log.Info(message, exception);
		}

		public void Debug(object message, Exception exception)
		{
			m_Log.Debug(message, exception);
		}
	}
}