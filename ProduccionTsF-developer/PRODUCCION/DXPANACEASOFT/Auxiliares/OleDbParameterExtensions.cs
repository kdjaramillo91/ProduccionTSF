using System;
using System.Data.OleDb;

namespace DXPANACEASOFT.Auxiliares
{
	internal static class OleDbParameterExtensions
	{
		internal static void SetIntegerValue(this OleDbParameter parameter, int? value)
		{
			if (value.HasValue)
			{
				parameter.Value = value.Value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}

		internal static void SetDecimalValue(this OleDbParameter parameter, decimal? value)
		{
			if (value.HasValue)
			{
				parameter.Value = value.Value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}

		internal static void SetStringValue(this OleDbParameter parameter, string value)
		{
			if (value != null)
			{
				parameter.Value = value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}

		internal static void SetDateValue(this OleDbParameter parameter, DateTime? value)
		{
			if (value.HasValue)
			{
				parameter.Value = value.Value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}

		internal static void SetTimeValue(this OleDbParameter parameter, TimeSpan? value)
		{
			if (value.HasValue)
			{
				parameter.Value = value.Value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}

		internal static void SetBooleanValue(this OleDbParameter parameter, bool? value)
		{
			if (value.HasValue)
			{
				parameter.Value = value.Value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}
	}
}