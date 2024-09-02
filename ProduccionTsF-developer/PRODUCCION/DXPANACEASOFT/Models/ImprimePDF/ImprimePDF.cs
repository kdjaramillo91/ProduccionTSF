using System;
using System.Drawing.Printing;
using System.Linq;
using Utilitarios.Logs;
using PdfDocument = Spire.Pdf.PdfDocument;

namespace DXPANACEASOFT.Models.ImprimePDF
{
	public class ImprimePDF
	{
		#region PRIVATE ATTRIBUTES
		private string _printerName { get; set; }

		private string _formatPrint { get; set; }

		private string _pathFileTotal { get; set; }

		#endregion

		#region GLOBAL VARIABLES
		//static Timer TTimer = null;
		PdfDocument pdf;

		PrintDocument printDoc;
		#endregion

		#region CONSTRUCTORES
		public ImprimePDF()
		{
		}

		public ImprimePDF(string printName, string formatPrint, string fileTotalName)
		{
			this._printerName = printName;
			this._formatPrint = formatPrint;
			this._pathFileTotal = fileTotalName;

		}

		#endregion

		#region PUBLIC METHODS
		public void Print()
		{
			ValidateInfo();
			PrintDoc();

		}

		#endregion

		#region PRIVATE METHODS
		private void ValidateInfo()
		{
			try
			{
				if (string.IsNullOrEmpty(_printerName))
				{
					throw new Exception("Nombre de la impresora no se definido");
				}

				if (string.IsNullOrEmpty(_formatPrint))
				{
					throw new Exception("Formato de Tipo de página no se definido");
				}

				if (string.IsNullOrEmpty(_pathFileTotal))
				{
					throw new Exception("Nombre de la impresora no se definido");
				}
			}
			catch (Exception ex)
			{
				MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, "C:\\PrintDirect", "IMPRESION DIRECTA", "PROD");
			}
		}
		private void PrintDoc()
		{
			pdf = new PdfDocument();
			//string ruta = ConfigurationManager.AppSettings["rutaLog"];
			//_pathFileTotal = "C:\\ReportesGenerados\\1542401.pdf";
			//string _rutaTotal = pathReport + "\\" + this._folderPrint + "\\" + this._fileName + ".pdf";
			pdf.LoadFromFile(_pathFileTotal);

			//string _machineNew = "";

			//if (this._printerName == "VIATICOS")
			//{
			//    _machineNew = this._maqName1;
			//}
			//else
			//{
			//    _machineNew = this._maqName;
			//}
			pdf.PrinterName = /*_machineNew + "\\" + this.*/_printerName;


			printDoc = pdf.PrintDocument;

			var papersSizes = "";
			foreach (PaperSize _psize in printDoc.PrinterSettings.PaperSizes)
			{
				if (_psize.PaperName == _formatPrint)
				{
					printDoc.DefaultPageSettings.PaperSize = _psize;
					papersSizes += papersSizes != "" ? ", " + _psize.PaperName + " = " + _formatPrint : _psize.PaperName + " = " + _formatPrint;
				}
				else
				{
					papersSizes += papersSizes != "" ? ", " + _psize.PaperName : _psize.PaperName;
				}
			}

			MetodosEscrituraLogs.EscribeMensajeLog("PapersSizes, PapersNames: " + papersSizes, "C:\\PrintDirect", "IMPRESION DIRECTA", "PROD");

			//Font myFont;
			//if (this._printerName == "GUIA")
			//{
			//    myFont = new Font("Tahoma (Occidental)", 14);
			//}
			//else
			//{
			//    myFont = new Font("Tahoma (Occidental)", 18);
			//}

			printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);



			var maxResolution = printDoc.PrinterSettings
										.PrinterResolutions.OfType<PrinterResolution>()
										.OrderByDescending(r => r.X)
										.ThenByDescending(r => r.Y)
										.First();

			printDoc.DefaultPageSettings.PrinterResolution.X = maxResolution.X;
			printDoc.DefaultPageSettings.PrinterResolution.Y = maxResolution.Y;

			try
			{

				printDoc.Print();
			}
			catch (Exception ex)
			{
				MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, "C:\\PrintDirect", "IMPRESION DIRECTA", "PROD");
				//MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "ImpresionDirecta", this._fileName);
				//printDoc.Dispose();
				//pdf.Close();
				//pdf.Dispose();
			}
		}
		//private void CopyDoc()
		//{

		//}


		#endregion
	}
}