using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Utilitarios.General
{
    public static class GeneralStr
    {
        public static string GetRandomStr(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

        }
        public static string InnerXML(XElement el)
        {
            var reader = el.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        public static string GetDateFormatStringFromDatetime(DateTime dt)
        {
            int _year = dt.Year;
            int _month = dt.Month;
            int _day = dt.Day;
            int _hour = dt.Hour;
            int _minute = dt.Minute;
            int _second = dt.Second;

            return
                ((_day >= 10) ? _day.ToString() : _day.ToString().PadLeft(2, '0')) + '/'
                + ((_month >= 10) ? _month.ToString() : _month.ToString().PadLeft(2, '0')) + '/'
                + _year.ToString() ;
        }
        public static string GetTimeFormatStringFromDatetime(DateTime dt)
        {
            int _year = dt.Year;
            int _month = dt.Month;
            int _day = dt.Day;
            int _hour = dt.Hour;
            int _minute = dt.Minute;
            int _second = dt.Second;

            return ((_hour >= 10) ? _hour.ToString() : _hour.ToString().PadLeft(2, '0')) + ':'
                + ((_minute >= 10) ? _minute.ToString() : _minute.ToString().PadLeft(2, '0')) + ':'
                + ((_second >= 10) ? _second.ToString() : _second.ToString().PadLeft(2, '0'));
        }
    }
    public static class GeneralDataCollection
    {
        public static DataTable ConvertToDataTable<T>(IList<T> data, string name)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable(name);
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
    }

}
