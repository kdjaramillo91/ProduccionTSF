using MigracionProduccionCIWebApi.Models;
using Newtonsoft.Json;
using ProductionApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MigracionProduccionCIWebApi.Controllers
{
	public class HomeController : Controller
	{
		private const string GuidKey = "Guid_Key";
		public ActionResult Index()
		{
			this.ViewBag.Title = "Home Page";
			return View();
		}

		public async Task<JsonResult> Migrar(string aMigrar)
		{
			try {
                var list = await AsyncAwait_GetSomeDataAsync(aMigrar);
                var result = new
                {
                    Data = JsonConvert.SerializeObject(list)
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
			{
                return Json(new { Error = ex.InnerException?.Message ?? ex.Message });
            }
			
		}

		public async Task<JsonResult> DeleteDataToMigrate(string aMigrar)
		{
			var list = await AsyncAwait_DeleteSomeDataAsync(aMigrar);
			var result = new
			{
				Data = JsonConvert.SerializeObject(list)
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		private async Task<JsonResult> AsyncAwait_GetSomeDataAsync(string aMigrar)
		{
			List<AnswerPersonProvider> resultList = null;

			try
			{
				var uri = String.Concat(
					ConfigurationManager.AppSettings["URIProduccion"],
					"/AddGeneral?sMigrar=",
					aMigrar);

				using (var httpClient = new HttpClient())
				{
					using (var response = await httpClient.PostAsync(uri, null))
					{
						response.EnsureSuccessStatusCode();

						var responseContent = await response
							.Content.ReadAsStringAsync();

						resultList = JsonConvert
							.DeserializeObject<List<AnswerPersonProvider>>(responseContent);
					}
				}
			}
			catch (Exception e)
			{
                return Json(new { Error = e.InnerException?.Message ?? e.Message });
                //System.Diagnostics.Debug.WriteLine($"ERROR en respuesta ADDGENERAL: {e}");
			}

			if (resultList[0].personProvider == null)
			{
                return Json(new { Error = resultList[0].message });
            }
			// Preparación de los resultados...
			var numeroCorrecto = 0;
			var numeroIncorrecto = 0;

			if ((resultList != null) && (resultList.Count > 0))
			{
				numeroCorrecto = resultList
					.Where(r => r.message.IndexOf("Migrado satisfactoriamente") > 0)
					.Count();

				numeroIncorrecto = resultList
					.Where(r => r.message.IndexOf("Error no esperado") > 0)
					.Count();
			}

			var result = new
			{
				listAnswer = resultList,
				numeroCor = numeroCorrecto,
				numeroInc = numeroIncorrecto,
			};

			var key = Guid.NewGuid().ToString("N");
			this.TempData[GuidKey] = key;

			var fallidosKey = $"documentos-fallidos-{key}";
			var correctosKey = $"documentos-importados-{key}";
			int ordenFallo = 1;
			int ordenCorrecto = 1;

			// Datos temporales de Migración
			this.TempData[correctosKey] = resultList
				.Where(r => r.message.IndexOf("Migrado satisfactoriamente") > 0)
				.Select(e => new ImportResult.DocumentoImportado()
				{
					Filas = (ordenCorrecto++).ToString(),
					Descripcion = e.message,
				})
				.ToArray();

			this.TempData[fallidosKey] = resultList
				.Where(r => r.message.IndexOf("Error no esperado") > 0)
				.Select(e => new ImportResult.DocumentoFallido() {
					Filas = (ordenFallo++).ToString(),
					Descripcion = e.message,
				})
				.ToArray();

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		private async Task<List<AnswerPersonProvider>> AsyncAwait_DeleteSomeDataAsync(string aMigrar)
		{
			List<AnswerPersonProvider> resultList = null;

			try
			{
				var uri = String.Concat(
					ConfigurationManager.AppSettings["URIProduccion"],
					"/DeleteGeneral?sMigrar=",
					aMigrar);

				using (var httpClient = new HttpClient())
				{
					using (var response = await httpClient.PostAsync(uri, null))
					{
						response.EnsureSuccessStatusCode();

						var responseContent = await response
							.Content.ReadAsStringAsync();

						resultList = JsonConvert
							.DeserializeObject<List<AnswerPersonProvider>>(responseContent);
					}
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine($"ERROR en respuesta DELETE: {e}");
			}

			return resultList;
		}

		#region Métodos por resultados de la importación

		[HttpGet]
		public ViewResult DownloadDocumentosFallidosImportacion()
        {
			this.ViewBag.ReportCommand = "export";
			this.ViewBag.ReportTitle = "Errores en la Migración de Información";
			this.ViewBag.ExcelFileName = $"ErroresMigracion_{DateTime.Now:yyyyMMdd HHmm}.xls";

			var guid = this.TempData[GuidKey] as string;
			var key = $"documentos-fallidos-{guid}";

			var documentosFallidos = this.TempData.ContainsKey(key)
				? this.TempData[key] as ImportResult.DocumentoFallido[]
				: new ImportResult.DocumentoFallido[] { };

			return this.View("_DocumentosFallidosImportacionReportPartial", documentosFallidos);
		}

		[HttpGet]
		public ViewResult DownloadDocumentosImportadosImportacion()
        {
			this.ViewBag.ReportCommand = "export";
			this.ViewBag.ReportTitle = "Resultados de Migración";
			this.ViewBag.ExcelFileName = $"ResultadosMigracion_{DateTime.Now:yyyyMMdd HHmm}.xls";

			var guid = this.TempData[GuidKey] as string;
			var key = $"documentos-fallidos-{guid}";

			var documentosImportados = this.TempData.ContainsKey(key)
				? this.TempData[key] as ImportResult.DocumentoImportado[]
				: new ImportResult.DocumentoImportado[] { };

			return this.View("_DocumentosImportadosImportacionReportPartial", documentosImportados);
		}

		#endregion
	}
}
