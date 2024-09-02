using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Webservices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DXPANACEASOFT.Controllers
{
    public class ElectronicDocumentController : DefaultController
    {
        [HttpPost, ValidateInput(false)]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ELECTRONIC DOCUMENT FILTER RESULT

        [HttpPost, ValidateInput(false)]
        public ActionResult ElectronicDocumentResults(ElectronicDocument electronicDocument,
                                                      Document document,
                                                      DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                      DateTime? startAuthorizationDate, DateTime? endAuthorizationDate)
        {
            List<Document> model = db.Document.Where(d => d.DocumentType.isElectronic && d.ElectronicDocument != null).OrderByDescending(d => d.id).ToList();

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_ElectronicDocumentResultsPartial", model.ToList());
        }

        #endregion

        #region ELECTRONIC DOCUMENTS GRIDVIEW

        [ValidateInput(false)]
        public ActionResult ElectronicDocumentsPartial()
        {
            var model = (TempData["model"] as List<Document>);
            model = model ?? new List<Document>();

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_ElectronicDocumentsPartial", model.ToList());
        }

        #endregion

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult SendXmlToSri(int id)
        {
            Document document = db.Document.FirstOrDefault(d => d.id == id);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(document.ElectronicDocument.xml);

            // SAVE XML DOCUMENT
            string path = Server.MapPath("~/App_Data/Xmls");
            string filename = $"{path}/{document.number}.xml";
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
            xml.Save(filename);


            // SIGN XML
            string certificate = Server.MapPath("~/App_Data/Certificates/") + ActiveCompany.CompanyElectronicFacturation.certificate;
            string password = ActiveCompany.CompanyElectronicFacturation.password;

            #if !DEBUG
                SignXmlServiceReference.SignXmlPortTypeClient client = new SignXmlServiceReference.SignXmlPortTypeClient();
            #else 
                TestServiceReference.SignXmlPortTypeClient client = new TestServiceReference.SignXmlPortTypeClient();
            #endif

            string strOutput = null;
            try
            {
                strOutput = client.sign(path, document.number + ".xml", path, certificate, password);
            }
            catch (Exception)
            {
                TempData.Keep("model");
                return Json(new { isValid = false, errorText = "No se pudo firmar digitalmente el documento" });
            }
            

            JToken result = JsonConvert.DeserializeObject<JToken>(strOutput);
            int code = result.Value<int>("code");
            if(code == 0)
            {
                XmlDocument signedXml = new XmlDocument();
                signedXml.Load(filename);
                document.ElectronicDocument.signedXml = signedXml.OuterXml;
                
                try
                {
                    string respuestaEnvio = SRIWebServices.EnviarComprobante(filename);
                    SRIWebServices.RespuestaSRI respuestaSRI = JsonConvert.DeserializeObject<SRIWebServices.RespuestaSRI>(respuestaEnvio);

                    JToken respuesta = JsonConvert.DeserializeObject<JToken>(respuestaEnvio);
                    string stateName = respuesta.Value<string>("Estado");

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }

                    ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == this.ActiveCompanyId && e.name.Equals(stateName));

                    if (electronicState != null)
                    {
                        document.ElectronicDocument.id_electronicDocumentState = electronicState.id;
                    }
                }
                catch (Exception /*e*/)
                {
                    TempData.Keep("model");
                    return Json(new { isValid = false, errorText = "No hay conexion con el servicio del SRI" });
                }
            }
            else
            {
                TempData.Keep("model");
                return Json(new { isValid = false, errorText = "No se pudo firmar digitalmente el documento" });
            }

            db.Document.Attach(document);
            db.Entry(document).State = EntityState.Modified;

            db.SaveChanges();

            List<Document> model = db.Document.Where(d => d.DocumentType.isElectronic && d.ElectronicDocument != null).OrderByDescending(d => d.id).ToList();

            TempData["model"] = model;
            TempData.Keep("model");

            return Json(new { isValid = true, errorText = "Documento: " + document.number + " estado " + document.ElectronicDocument.ElectronicDocumentState.name });
        }

        public JsonResult CheckXmlAuthorization(int id)
        {
            ElectronicDocument document = db.ElectronicDocument.FirstOrDefault(d => d.id == id);

            if(document != null)
            {
                string respuestaEnvio = SRIWebServices.AutorizarComprobante(document.Document.accessKey);
                SRIWebServices.RespuestaSRI respuestaSRI = JsonConvert.DeserializeObject<SRIWebServices.RespuestaSRI>(respuestaEnvio);

                bool isValid = respuestaSRI.Estado.Equals("AUTORIZADA");

                ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == this.ActiveCompanyId && e.name.Equals(respuestaSRI.Estado));

                if (electronicState != null)
                {
                    document.id_electronicDocumentState = electronicState.id;

                    document.Document.authorizationNumber = (isValid) ? respuestaSRI.NumeroAutorizacion
                                                                      : $"{respuestaSRI.ErrorIdentificador}-{respuestaSRI.ErrorMensaje}";


                    db.ElectronicDocument.Attach(document);
                    db.Entry(document).State = EntityState.Modified;

                    db.SaveChanges();
                }

                List<Document> model = db.Document.Where(d => d.DocumentType.isElectronic && d.ElectronicDocument != null).OrderByDescending(d => d.id).ToList();
                TempData["model"] = model;
                TempData.Keep("model");

                string errorText = $"Documento: {document.Document.number} AUTORIZADO";
                errorText = (!isValid) ? $"Documento: {document.Document.number} NO AUTORIZADO <br/> Error: {respuestaSRI.ErrorIdentificador} - {respuestaSRI.ErrorMensaje}"
                                       : errorText; 

                return Json(new { isValid = isValid, errorText = errorText });
            }


            TempData.Keep("model");
            return Json(new { isValid = false, errorText = "Documento no autorizado" });
        }

        [HttpGet, ValidateInput(false)]
        public ActionResult ViewXml(int id)
        {
            Document document = db.Document.FirstOrDefault(d => d.id == id);
            TempData.Keep("model");
            return View(document);
        }

#endregion
    }
}