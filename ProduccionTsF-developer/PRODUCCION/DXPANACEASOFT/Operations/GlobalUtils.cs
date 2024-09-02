using System;
using System.Globalization;

namespace DXPANACEASOFT.Operations
{
    public static class GlobalUtils
    {
        public const string IsoDateFormat = "yyyy-MM-dd";
        public const string IsoDateTimeFormat = "yyyy-MM-dd HH:mm";
        public const string DateFormat = "dd/MM/yyyy";
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
        public const string DateTimeSecondsFormat = "dd/MM/yyyy HH:mm:ss";
        public const string MonthYearFormat = "MM/yyyy";
        public const string TimeFormat = "HH:mm";
        public const string TimeSpanFormat = "hh\\:mm";
        public const string MonthYearPeriodoFormat = "yyyyMM";

        public const string EnglishDateFormat = "dd-MMM-yyyy";
        public const string SpanishDateFormat = "dd-MMM-yyyy";
        public const string SriDateFormat = "dd/MM/yyyy";
        public const string CutOffDateFormat = "dddd, d - HH:mm";

        public const string GeneralFormat = "G";
        public const string IntegerFormat = "N0";
        public const string DecimalFormat = "N2";
        public const string Decimal4Format = "N4";
        public const string Decimal6Format = "N6";
        public const string CurrencyFormat = "C2";
        public const string Currency4Format = "C4";
        public const string Currency6Format = "C6";

        #region Configuración regional del sistema

        private static readonly CultureInfo m_SystemCultureInfo = new CultureInfo("es-MX", false);
        private static readonly CultureInfo m_EnglishCultureInfo = new CultureInfo("en-US", false);
        private static readonly CultureInfo m_SpanishCultureInfo = new CultureInfo("es-MX", false);

        public static CultureInfo SystemCultureInfo => m_SystemCultureInfo;
        public static CultureInfo EnglishCultureInfo => m_EnglishCultureInfo;
        public static CultureInfo SpanishCultureInfo => m_SpanishCultureInfo;

        #endregion Configuración regional del sistema


        public static DateTime DateFromDayYear(int year,int day)
        { 
            return new DateTime(year, 1, 1).AddDays(day - 1);
        }


        public static Tuple<DateTime, DateTime> DatesInitEndFromPeriod(int year, int month)
        {
            DateTime dateInit = new DateTime(year, month, 1);
            DateTime dateEnd = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

            return new  Tuple<DateTime, DateTime>(dateInit, dateEnd);
        }



        public static DateTime DateLastDayFromMonth(int year, int month)
        {
            return new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        }


        public static string ConvertToMonthName(int monthCardinal)
        {
            string result = string.Empty;
            switch (monthCardinal)
            {
                case 1:
                    result = "ENERO";
                    break;
                case 2:
                    result = "FEBRERO";
                    break;
                case 3:
                    result = "MARZO";
                    break;
                case 4:
                    result = "ABRIL";
                    break;
                case 5:
                    result = "MAYO";
                    break;
                case 6:
                    result = "JUNIO";
                    break;
                case 7:
                    result = "JULIO";
                    break;
                case 8:
                    result = "AGOSTO";
                    break;
                case 9:
                    result = "SEPTIEMBRE";
                    break;
                case 10:
                    result = "OCTUBRE";
                    break;
                case 11:
                    result = "NOVIEMBRE";
                    break;
                case 12:
                    result = "DICIEMBRE";
                    break;
            }
            return result;
        }

        public static string ToYearMonthPeriod(this DateTime dateTime)
        {
            return dateTime.ToString(MonthYearPeriodoFormat, m_SystemCultureInfo);
        }

        public static string ToIsoDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(IsoDateFormat, m_SystemCultureInfo);
        }

        public static string ToIsoDateFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(IsoDateFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToIsoDateTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToString(IsoDateTimeFormat, m_SystemCultureInfo);
        }

        public static string ToIsoDateTimeFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(IsoDateTimeFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(DateFormat, m_SystemCultureInfo);
        }

        public static string ToDateFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(DateFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToDateTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormat, m_SystemCultureInfo);
        }

        public static string ToDateTimeFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(DateTimeFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToDateTimeSecondsFormat(this DateTime dateTime)
        {
            return dateTime.ToString(DateTimeSecondsFormat, m_SystemCultureInfo);
        }

        public static string ToDateTimeSecondsFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(DateTimeSecondsFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToMonthYearFormat(this DateTime dateTime)
        {
            return dateTime.ToString(MonthYearFormat, m_SystemCultureInfo);
        }

        public static string ToMonthYearFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(MonthYearFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToString(TimeFormat, m_SystemCultureInfo);
        }

        public static string ToTimeFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(TimeFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToTimeFormat(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(TimeSpanFormat, m_SystemCultureInfo);
        }

        public static string ToTimeFormat(this TimeSpan? timeSpan)
        {
            return (timeSpan.HasValue)
                ? timeSpan.Value.ToString(TimeSpanFormat, m_SystemCultureInfo)
                : null;
        }

        public static int ToIntegerDate(this DateTime dateTime)
        {
            return (dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day);
        }

        public static int? ToIntegerDate(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? (int?)dateTime.Value.ToIntegerDate()
                : null;
        }

        public static int ToPeriodoDate(this DateTime dateTime)
        {
            return (dateTime.Year * 100 + dateTime.Month);
        }

        public static DateTime ToDateInteger(this int dateTime)
        {
            return new DateTime(dateTime / 10000, dateTime / 100 % 100, dateTime % 100);
        }

        public static DateTime? ToDateInteger(this int? dateTime)
        {
            return (dateTime.HasValue)
                ? (DateTime?)dateTime.Value.ToDateInteger()
                : null;
        }

        public static string ToEnglishDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(EnglishDateFormat, m_EnglishCultureInfo);
        }

        public static string ToEnglishDateFormat(this DateTime? dateTime)
        {
            return dateTime.HasValue
                ? dateTime.Value.ToString(EnglishDateFormat, m_EnglishCultureInfo)
                : null;
        }

        public static string ToSpanishDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(SpanishDateFormat, m_SpanishCultureInfo);
        }

        public static string ToSpanishDateFormat(this DateTime? dateTime)
        {
            return dateTime.HasValue
                ? dateTime.Value.ToString(SpanishDateFormat, m_SpanishCultureInfo)
                : null;
        }

        public static string ToSriDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(SriDateFormat, m_SystemCultureInfo);
        }

        public static string ToSriDateFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(SriDateFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToCutOffDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(CutOffDateFormat, m_SystemCultureInfo).ToUpper();
        }

        public static string ToCutOffDateFormat(this DateTime? dateTime)
        {
            return (dateTime.HasValue)
                ? dateTime.Value.ToString(CutOffDateFormat, m_SystemCultureInfo).ToUpper()
                : null;
        }

        public static string ToGeneralFormat(this int value)
        {
            return value.ToString(GeneralFormat, m_SystemCultureInfo);
        }

        public static string ToGeneralFormat(this int? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(GeneralFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToIntegerFormat(this int value)
        {
            return value.ToString(IntegerFormat, m_SystemCultureInfo);
        }

        public static string ToIntegerFormat(this int? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(IntegerFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToIntegerFormat(this decimal value)
        {
            return GlobalCalculator.RedondearEntero(value).ToString(IntegerFormat, m_SystemCultureInfo);
        }

        public static string ToIntegerFormat(this decimal? value)
        {
            return (value.HasValue)
                ? GlobalCalculator.RedondearEntero(value.Value).ToString(IntegerFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToDecimalFormat(this decimal value)
        {
            return value.ToString(DecimalFormat, m_SystemCultureInfo);
        }

        public static string ToDecimalFormat(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(DecimalFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToDecimal4Format(this decimal value)
        {
            return value.ToString(Decimal4Format, m_SystemCultureInfo);
        }

        public static string ToDecimal4Format(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(Decimal4Format, m_SystemCultureInfo)
                : null;
        }

        public static string ToDecimal6Format(this decimal value)
        {
            return value.ToString(Decimal6Format, m_SystemCultureInfo);
        }

        public static string ToDecimal6Format(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(Decimal6Format, m_SystemCultureInfo)
                : null;
        }

        public static string ToCurrencyFormat(this decimal value)
        {
            return value.ToString(CurrencyFormat, m_SystemCultureInfo);
        }

        public static string ToCurrencyFormat(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(CurrencyFormat, m_SystemCultureInfo)
                : null;
        }

        public static string ToCurrency4Format(this decimal value)
        {
            return value.ToString(Currency4Format, m_SystemCultureInfo);
        }

        public static string ToCurrency4Format(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(Currency4Format, m_SystemCultureInfo)
                : null;
        }

        public static string ToCurrency6Format(this decimal value)
        {
            return value.ToString(Currency6Format, m_SystemCultureInfo);
        }

        public static string ToCurrency6Format(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToString(Currency6Format, m_SystemCultureInfo)
                : null;
        }

        public static string ToVariableDecimalFormat(this decimal value)
        {
            return value.ToVariableDecimalFormat(Decimal6Format);
        }

        public static string ToVariableDecimalFormat(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToVariableDecimalFormat(Decimal6Format)
                : null;
        }

        public static string ToVariableCurrencyFormat(this decimal value)
        {
            return value.ToVariableDecimalFormat(Currency6Format);
        }

        public static string ToVariableCurrencyFormat(this decimal? value)
        {
            return (value.HasValue)
                ? value.Value.ToVariableDecimalFormat(Currency6Format)
                : null;
        }

        private static string ToVariableDecimalFormat(this decimal value, string format)
        {
            // Generamos el valor decimal formateado con el formato indicado
            var decimalValueChars = value
                .ToString(format, m_SystemCultureInfo)
                .ToCharArray();
            var decimalValueCharsLength = decimalValueChars.Length;

            // Reducimos los decimales en pares, si son ceros
            while (decimalValueCharsLength > 2)
            {
                if ((decimalValueChars[decimalValueCharsLength - 2] == '0')
                    && (decimalValueChars[decimalValueCharsLength - 1] == '0'))
                {
                    // Si los dos últimos decimales son cero, los omitimos...
                    decimalValueCharsLength -= 2;
                }
                else
                {
                    // Si alguno de los dos últimos decimales NO son cero, terminamos...
                    break;
                }
            }

            // Si el último elemento es el punto decimal, tomamos los últimos 2 ceros...
            if ((decimalValueCharsLength > 1)
                && (decimalValueChars[decimalValueCharsLength - 1] == '.'))
            {
                decimalValueCharsLength += 2;
            }

            // Retornamos el valor reducido
            return new String(decimalValueChars, 0, decimalValueCharsLength);
        }
    }
}