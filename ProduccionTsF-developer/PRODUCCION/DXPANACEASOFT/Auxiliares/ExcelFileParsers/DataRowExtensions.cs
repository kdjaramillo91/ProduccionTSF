using System;
using System.Data;

namespace DXPANACEASOFT.Auxiliares.ExcelFileParsers
{
    internal static class DataRowExtensions
    {
		internal static string GetString(this DataRow dataRow, string columnName)
		{
			try
			{
				return Convert.ToString(dataRow[columnName]);
			}
			catch (Exception exception)
			{
				throw new Exception(
					$"Error al leer el valor de la columna {columnName} [Tabla {dataRow?.Table?.TableName}].",
					exception);
			}
		}

		internal static int GetInteger(this DataRow dataRow, string columnName)
		{
			try
			{
				return Convert.ToInt32(dataRow[columnName]);
			}
			catch (Exception exception)
			{
				throw new Exception(
					$"Error al leer el valor entero de la columna {columnName} [Tabla {dataRow?.Table?.TableName}].",
					exception);
			}
		}

		internal static int? GetIntegerNullable(this DataRow dataRow, string columnName)
		{
			var value = GetString(dataRow, columnName);

			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				return GetInteger(dataRow, columnName);
			}
		}

		internal static decimal GetDecimal(this DataRow dataRow, string columnName)
		{
			try
			{
				return Convert.ToDecimal(dataRow[columnName]);
			}
			catch (Exception exception)
			{
				throw new Exception(
					$"Error al leer el valor numérico de la columna {columnName} [Tabla {dataRow?.Table?.TableName}].",
					exception);
			}
		}

		internal static decimal? GetDecimalNullable(this DataRow dataRow, string columnName)
		{
			var value = GetString(dataRow, columnName);

			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				return GetDecimal(dataRow, columnName);
			}
		}


		internal static DateTime GetDate(this DataRow dataRow, string columnName)
		{
			try
			{
				return Convert.ToDateTime(dataRow[columnName]).Date;
			}
			catch (Exception exception)
			{
				throw new Exception(
					$"Error al leer la fecha de la columna {columnName} [Tabla {dataRow?.Table?.TableName}].",
					exception);
			}
		}
		internal static DateTime? GetDateNullable(this DataRow dataRow, string columnName)
		{
			var value = GetString(dataRow, columnName);

			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				return GetDate(dataRow, columnName);
			}
		}


		internal static TimeSpan GetTime(this DataRow dataRow, string columnName)
		{
			try
			{
				return Convert.ToDateTime(dataRow[columnName]).TimeOfDay;
			}
			catch (Exception exception)
			{
				throw new Exception(
					$"Error al leer la hora de la columna {columnName} [Tabla {dataRow?.Table?.TableName}].",
					exception);
			}
		}
		internal static TimeSpan? GetTimeNullable(this DataRow dataRow, string columnName)
		{
			var value = GetString(dataRow, columnName);

			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				return GetTime(dataRow, columnName);
			}
		}


		internal static bool GetBoolean(this DataRow dataRow, string columnName)
		{
			var value = GetString(dataRow, columnName);

			if (String.Equals(value, "SI", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			if (String.Equals(value, "NO", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			throw new Exception(
				$"Valor lógico es incorrecto en la columna {columnName} [Tabla {dataRow?.Table?.TableName}].");
		}
		internal static bool? GetBooleanNullable(this DataRow dataRow, string columnName)
		{
			var value = GetString(dataRow, columnName);

			if (String.IsNullOrEmpty(value))
			{
				return null;
			}

			if (String.Equals(value, "SI", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			if (String.Equals(value, "NO", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			throw new Exception(
				$"Valor lógico es incorrecto en la columna {columnName} [Tabla {dataRow?.Table?.TableName}].");
		}

	}
}