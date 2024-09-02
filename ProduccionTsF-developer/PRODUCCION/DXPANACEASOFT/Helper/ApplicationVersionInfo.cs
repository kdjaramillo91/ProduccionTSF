using System;
using System.Reflection;
using System.Threading;

namespace DXPANACEASOFT.Helper
{
	public static class ApplicationVersionInfo
	{
		private const string m_EmptyVersion = "0.0.0.0";
		private static string m_ApplicationVersionTextInfo;

		public static string GetApplicationVersionInfo()
		{
			if (m_ApplicationVersionTextInfo == null)
			{
				Interlocked.CompareExchange(
					ref m_ApplicationVersionTextInfo, GetApplicationVersionInfoPrivate(), null);
			}

			return m_ApplicationVersionTextInfo;
		}

		private static string GetApplicationVersionInfoPrivate()
		{
			return String.Format(
				"v{0} [Build {1}]",
				GetApplicationSystemVersion(),
				GetApplicationBuildVersion());
		}

		private static string GetApplicationSystemVersion()
		{
			return GetApplicationAssembly()
				.GetName()
				.Version
				.ToString();
		}

		private static string GetApplicationBuildVersion()
		{
			var attribute = GetApplicationAssembly()
				.GetCustomAttribute<AssemblyFileVersionAttribute>();

			if (attribute != null)
			{
				if (Version.TryParse(attribute.Version, out var version))
				{
					return String.Concat(version.Build, ".", version.Revision);
				}
				else
				{
					return attribute.Version;
				}
			}
			else
			{
				return m_EmptyVersion;
			}
		}

		private static Assembly GetApplicationAssembly()
		{
			return typeof(ApplicationVersionInfo).Assembly;
		}
	}
}