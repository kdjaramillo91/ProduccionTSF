
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares
{
	internal abstract class ExcelFileParserBase
	{
		#region Constantes

		protected const string ExcelXlsMime = "application/vnd.ms-excel";
		protected const string ExcelXlsxMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
		protected const string ExcelXlsmMime = "application/vnd.ms-excel.sheet.macroEnabled.12";

		private const string m_ExcelConnectionStringPatternReadOnly = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties=""{1};HDR=YES;IMEX=1"";Persist Security Info=False;";
		private const string m_ExcelConnectionStringPattern = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties=""{1};HDR=YES"";Persist Security Info=False;";

		private const string m_ExcelXlsType = "Excel 8.0";
		private const string m_ExcelXlsxType = "Excel 12.0 Xml";
		private const string m_ExcelXlsmType = "Excel 12.0 Xml";

		#endregion

		#region Propiedades

		internal string ContentFilePath { get; private set; }
		internal string ContentFileMime { get; private set; }
		internal string ConnectionString { get; private set; }

		#endregion

		#region Constructores

		internal ExcelFileParserBase(string contentFilePath, string contentFileMime, bool readOnly)
		{
			this.ContentFilePath = contentFilePath;
			this.ContentFileMime = contentFileMime;
			this.ConnectionString = GenerateConnectionString(contentFilePath, contentFileMime, readOnly);
		}

		#endregion

		#region Métodos

		protected static string CopyContentFile(string contentFilePath, string contentMime)
		{
			var contentFileName = Path.GetFileName(contentFilePath);
			var content = File.ReadAllBytes(contentFilePath);

			return FileExcelUploadHelper.WriteFileUpload(contentFileName, contentMime, content);
		}

		protected DataTable[] GetDataTables(string[][] tables)
		{
			// Verificamos el tamaño del argumento de entrada...
			if (tables == null)
			{
				return null;
			}

			if (tables.Length == 0)
			{
				return new DataTable[] { };
			}

			// Ejecutamos el proceso
			var dataTables = new List<DataTable>();

			using (var cnn = new OleDbConnection(this.ConnectionString))
			{
				cnn.Open();

				// Procesamos cada una de las tablas
				var validTableNames = GetTableNames(cnn);

				foreach (var table in tables)
				{
					var tableName = table[0];
					var tableColumns = table[1];

					// Verificamos si la tabla es válida
					if (!validTableNames.Contains(tableName, StringComparer.OrdinalIgnoreCase))
					{
						throw new Exception($"Archivo no contiene tabla de datos: {tableName}.");
					}

					// Recuperamos los datos de la tabla indicada
					var dataTable = GetDataTable(cnn, tableName, tableColumns);
					RemoveInvalidDataRows(dataTable);
					dataTables.Add(dataTable);
				}

				cnn.Close();
			}

			return dataTables.ToArray();
		}

		private static string GenerateConnectionString(string contentFilePath, string contentFileMime, bool readOnly)
		{
			var excelConnectionStringPattern = (readOnly)
				? m_ExcelConnectionStringPatternReadOnly
				: m_ExcelConnectionStringPattern;

			// Preparar la cadena de conexión, de acuerdo al tipo de MIME
			if (contentFileMime == ExcelXlsMime)
			{
				return String.Format(excelConnectionStringPattern, contentFilePath, m_ExcelXlsType);
			}
			else if (contentFileMime == ExcelXlsxMime)
			{
				return String.Format(excelConnectionStringPattern, contentFilePath, m_ExcelXlsxType);
			}
			else if (contentFileMime == ExcelXlsmMime)
			{
				return String.Format(excelConnectionStringPattern, contentFilePath, m_ExcelXlsmType);
			}
			else
			{
				throw new Exception(
					"Tipo de archivo no soportado para el procesamiento de archivos Excel: " + contentFileMime);
			}
		}

		private static string[] GetTableNames(OleDbConnection cnn)
		{
			var tableNames = new List<string>();

			using (var dtSchema = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" }))
			{
				if (dtSchema != null)
				{
					foreach (DataRow drSchema in dtSchema.Rows)
					{
						var tableName = Convert.ToString(drSchema["TABLE_NAME"]);
						tableNames.Add(tableName);
					}
				}
			}

			return tableNames.ToArray();
		}

		private static DataTable GetDataTable(OleDbConnection cnn, string tableName, string tableColumns)
		{
			DataTable dtResult = null;

			if (!String.IsNullOrEmpty(tableName))
			{
				var selectScript = $"SELECT {tableColumns} FROM [{tableName}]";
				var dsResults = new DataSet("ResultadosDataSet");

				using (var da = new OleDbDataAdapter(selectScript, cnn))
				{
					da.Fill(dsResults);
				}

				if (dsResults.Tables.Count > 0)
				{
					dtResult = dsResults.Tables[0];
					dtResult.TableName = tableName;
				}
			}

			return dtResult;
		}

		private static void RemoveInvalidDataRows(DataTable dataTable)
		{
			var numColumns = dataTable.Columns.Count;
			var numRows = dataTable.Rows.Count;
			var rowIndex = 0;

			while (rowIndex < numRows)
			{
				var row = dataTable.Rows[rowIndex];
				var anyData = false;

				for (var columnIndex = 0; columnIndex < numColumns; columnIndex++)
				{
					var value = row[columnIndex];

					if ((value != null) && !Convert.IsDBNull(value))
					{
						if ((value is String) && String.IsNullOrEmpty((string)value))
						{
							continue;
						}

						anyData = true;
						break;
					}
				}

				if (anyData)
				{
					rowIndex++;
				}
				else
				{
					dataTable.Rows.RemoveAt(rowIndex);
					numRows--;
				}
			}
		}

		#endregion
	}
}