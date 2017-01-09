using System;

namespace AlexSandros.DemoApps.Rates
{
	public static class LogHelper
	{
		public static Action WrapWithDebugLogging(Action action, string description, ILog log, bool hideErrors = false)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			if (string.IsNullOrWhiteSpace(description))
				throw new ArgumentException("Description should be informative", nameof(description));
			return
				() =>
				{
					if (log?.IsDebugEnabled ?? false)
						log.Debug($"Started {description}");
					try
					{
						action();
						if (log?.IsDebugEnabled ?? false)
							log.Debug($"Finished successfully {description}");
					}
					catch (Exception e)
					{
						if (!log?.IsErrorEnabled ?? false)
							log.Error($"Error while {description}: {e.Message}");
						if (log == null || !log.IsErrorEnabled || !hideErrors)
							throw;
					}
				};
		}

		public static void Error(this ILog log, Exception e)
		{
			if (e == null)
				return;
			if (log == null || !log.IsErrorEnabled)
				throw e;
			var ae = (e as AggregateException)?.Flatten();
			if (ae != null)
			{
				if (ae.InnerExceptions.Count > 0)
				{
					foreach (var innerException in ae.InnerExceptions)
					{
						log.Error(innerException.Message, innerException);
					}
				}
				else
				{
					log.Error(ae.Message, ae);
				}
			}
			else
			{
				log.Error(e.Message, e);
			}
		}
	}
}