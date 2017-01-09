using System;

namespace AlexSandros.DemoApps.Rates
{
	public interface ILog
	{
		void Debug(object message);

		void Info(object message);

		void Warn(object message);

		void Error(object message);

		void Fatal(object message);

		bool IsDebugEnabled { get; }

		bool IsInfoEnabled { get; }

		bool IsWarnEnabled { get; }

		bool IsErrorEnabled { get; }

		bool IsFatalEnabled { get; }

		void Fatal(object message, Exception exception);

		void Error(object message, Exception exception);

		void Warn(object message, Exception exception);

		void Info(object message, Exception exception);

		void Debug(object message, Exception exception);
	}
}