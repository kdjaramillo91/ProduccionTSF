using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace DXPANACEASOFT.Auxiliares
{
	internal static class Logging
	{
		private const string m_LogDirectory = "~/App_Data/LogFiles/";
		private static readonly string m_CurrentLogFilePathKeyName = Guid.NewGuid().ToString("N");

		internal static void LogRequest()
		{
			if (IsLogEnabled())
			{
				try
				{
					var context = HttpContext.Current;

					var logFilePath = GetNewLogFilePath(context);
					context.Request.SaveAs(logFilePath, true);
					context.Session[m_CurrentLogFilePathKeyName] = logFilePath;
				}
				catch
				{
				}
			}
		}

		internal static void LogException(Exception exception)
		{
			if (IsLogEnabled())
			{
				try
				{
					var context = HttpContext.Current;

					var logFilePath = (string)context.Session[m_CurrentLogFilePathKeyName];

					if (logFilePath.EndsWith(".log") && File.Exists(logFilePath))
					{
						logFilePath = String.Concat(
							logFilePath.Substring(0, logFilePath.Length - 4),
							"_Exception.log");

						File.WriteAllText(logFilePath, exception.ToString());
					}
				}
				catch
				{
				}
			}
		}

		private static bool IsLogEnabled()
		{
			return (Boolean.TryParse(ConfigurationManager.AppSettings["EnableRequestLogging"], out bool isLogEnabled))
				? isLogEnabled
				: false;
		}

		private static string GetNewLogFilePath(HttpContext context)
		{
			var auditDate = DateTime.Now;

			// Nos aseguramos que exista el directorio del Log
			var logDirectoryName = Path.Combine(
				context.Server.MapPath(m_LogDirectory),
				auditDate.ToString("yyyyMMdd"),
				context.Request.UserHostAddress);

			Directory.CreateDirectory(logDirectoryName);

			// Generamos el nombre del archivo de Log
			var logFileName = String.Format(
				"{0:HHmmss} {1} {2}.log",
				auditDate, context.Request.HttpMethod, Guid.NewGuid().ToString("N"));

			return Path.Combine(logDirectoryName, logFileName);
		}
	}
}