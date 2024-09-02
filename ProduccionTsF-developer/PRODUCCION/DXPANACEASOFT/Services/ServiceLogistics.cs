using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Utilitarios.Logs;
using System.Configuration;
using DXPANACEASOFT.Models.FE;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.DataProviders;
using XmlGuiaRemision = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.GuiaRemision2;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using DXPANACEASOFT.Models.RemGuide;
using DXPANACEASOFT.Services;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using EntidadesAuxiliares.SQL;
using System.Data;

namespace DXPANACEASOFT.Services
{

    public class ServiceLogistics
    {
        const int LOGON_TYPE_NEW_CREDENTIALS = 9;
        const int LOGON32_PROVIDER_WINNT50 = 3;

        public static string AutorizeRemissionGuide(int id_RemissionGuide, int id_company, string pathFileXml, string pathA1FirmarFileXml, string driverNameThird = "", string carRegistrationThird = "")
        {
            DBContext db2 = new DBContext();

            string answer = "OK";
            string ruta = ConfigurationManager.AppSettings["rutalog"];

            RemissionGuide remissionGuide = db2.RemissionGuide.FirstOrDefault(r => r.id == id_RemissionGuide);

            using (DbContextTransaction trans = db2.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db2.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        #region Regeneramos la clave de acceso
                        var document = remissionGuide.Document;
                        string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                        Company vrCompa = db2.Company.Where(h => h.id == id_company).FirstOrDefault();
                        var enviromentCode = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI.ToString();
                        remissionGuide.Document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                                document.DocumentType.codeSRI, document.EmissionPoint.BranchOffice.Division.Company.ruc, enviromentCode,
                                document.EmissionPoint.BranchOffice.code.PadLeft(3, '0') + document.EmissionPoint.code.ToString("D3"),
                                document.sequential.ToString("D9"),
                                document.sequential.ToString("D8"),
                                "1");
                        #endregion

                        string varPar = db2.Setting.FirstOrDefault(fod => fod.code == "GXP")?.value;

                        if (varPar == "1")
                        {
                            documentState = db2.DocumentState.FirstOrDefault(s => s.code == "09"); //Pre - Autorizada
                            if (remissionGuide != null && documentState != null)
                            {
                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;
                                GeneraGuiaElectronicaXmlPers(db2, id_company, remissionGuide, pathFileXml, pathA1FirmarFileXml, driverNameThird, carRegistrationThird);
                            }
                        }

                        db2.RemissionGuide.Attach(remissionGuide);
                        db2.Entry(remissionGuide).State = EntityState.Modified;

                        db2.SaveChanges();
                        trans.Commit();

                    }
                }
                catch (Exception e)
                {
                    answer = "ERROR";
                    MetodosEscrituraLogs.EscribeExcepcionLog(e, ruta, "SERV_LOGIS_AUTORIZA_GR", "PROD");
                    //trans.Rollback();
                }
            }
            return answer;
        }

        public static string AutorizeRemissionGuideRiver(DBContext db2, int id_RemissionGuideRiver, int id_company, string pathFileXml, string pathA1FirmarFileXml)
        {
            string answer = "OK";
            string ruta = ConfigurationManager.AppSettings["rutalog"];

            RemissionGuideRiver remissionGuideRiver = db2.RemissionGuideRiver.FirstOrDefault(r => r.id == id_RemissionGuideRiver);

            DocumentState documentState = db2.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

            if (remissionGuideRiver != null && documentState != null)
            {
                remissionGuideRiver.Document.id_documentState = documentState.id;
                remissionGuideRiver.Document.DocumentState = documentState;

                #region Regeneramos la clave de acceso
                var document = remissionGuideRiver.Document;
                string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                Company vrCompa = db2.Company.Where(h => h.id == id_company).FirstOrDefault();
                var enviromentCode = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI.ToString();
                remissionGuideRiver.Document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                        document.DocumentType.codeSRI, document.EmissionPoint.BranchOffice.Division.Company.ruc, enviromentCode,
                        document.EmissionPoint.BranchOffice.code.PadLeft(3, '0') + document.EmissionPoint.code.ToString("D3"),
                        document.sequential.ToString("D9"),
                        document.sequential.ToString("D8"),
                        "1");
                #endregion

                string varPar = db2.Setting.FirstOrDefault(fod => fod.code == "GXP")?.value;

                if (varPar == "1")
                {
                    documentState = db2.DocumentState.FirstOrDefault(s => s.code == "09"); //Pre - Autorizada
                    if (remissionGuideRiver != null && documentState != null)
                    {
                        remissionGuideRiver.Document.id_documentState = documentState.id;
                        remissionGuideRiver.Document.DocumentState = documentState;
                        GeneraGuiaElectronicaFluvialXmlPers(db2, id_company, remissionGuideRiver, pathFileXml, pathA1FirmarFileXml);
                    }
                }

                db2.RemissionGuideRiver.Attach(remissionGuideRiver);
                db2.Entry(remissionGuideRiver).State = EntityState.Modified;
            }
            return answer;
        }

        public static void GeneraGuiaElectronicaXmlPers(DBContext db, int id_company, RemissionGuide pRemissionGuide, string pathFileXml, string pathA1FirmarFileXml, string driverNameThird = "", string carRegistrationThird = "")
        {

            //try
            //{
            ElectronicDocument pElectronicDocument = new ElectronicDocument();
            bool isNew = false;
            if (pRemissionGuide != null)
            {
                // FACTURACION ELECTRONICA - GENERACION DE XML
                if (pRemissionGuide.Document.DocumentType.isElectronic)
                {
                    ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == id_company && e.sriCode.Equals("01"));

                    if (electronicState != null)
                    {
                        if (pRemissionGuide.Document.ElectronicDocument == null)
                        {
                            pElectronicDocument = new ElectronicDocument
                            {
                                id_electronicDocumentState = electronicState.id
                            };
                            isNew = true;
                        }
                        else
                        {
                            pElectronicDocument = pRemissionGuide.Document.ElectronicDocument;
                        }

                        pElectronicDocument.isGenerated = false;

                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
                        XmlGuiaRemision xmlGuiaRemision = DB2XML.GuiaRemision3Xml(pRemissionGuide.id, driverNameThird, carRegistrationThird);
                        XmlDocument xmlout1 = SerializeToXml(xmlGuiaRemision, ns);


                        String xml = xmlout1.OuterXml;
                        xml = xml.Replace(@"p2:nil=""true""", "");
                        xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
                        xml = xml.Replace("<infoAdicional  />", "");

                        xmlout1.LoadXml(xml);

                        try
                        {
                            if (pathFileXml != "")
                            {
                                //User token that represents the authorized user account
                                IntPtr token = IntPtr.Zero;
                                string USER_rutaXmlFEX = ConfigurationManager.AppSettings["USER_rutaXmlFEX"];
                                USER_rutaXmlFEX = string.IsNullOrEmpty(USER_rutaXmlFEX) ? "admin" : USER_rutaXmlFEX;
                                string PASS_rutaXmlFEX = ConfigurationManager.AppSettings["PASS_rutaXmlFEX"];
                                PASS_rutaXmlFEX = string.IsNullOrEmpty(PASS_rutaXmlFEX) ? "admin" : PASS_rutaXmlFEX;
                                string DOMAIN_rutaXmlFEX = ConfigurationManager.AppSettings["DOMAIN_rutaXmlFEX"];
                                DOMAIN_rutaXmlFEX = string.IsNullOrEmpty(DOMAIN_rutaXmlFEX) ? "" : DOMAIN_rutaXmlFEX;

                                bool result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                if (result == true)
                                {
                                    //Use token to setup a WindowsImpersonationContext 
                                    using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                    {

                                        if (!string.IsNullOrEmpty(pathFileXml))
                                        {
                                            if (!(Directory.Exists(pathFileXml)))
                                            {
                                                Directory.CreateDirectory(pathFileXml);
                                                LogController.WriteLog(pathFileXml + ": Creado Exitosamente");
                                            }
                                            if (Directory.Exists(pathFileXml))
                                            {
                                                string pathFileXmlFileName = pathFileXml + "\\" + pRemissionGuide.Document.accessKey + ".xml";
                                                xmlout1.Save(pathFileXmlFileName);
                                                LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");
                                            }
                                        }
                                        else
                                        {
                                            LogController.WriteLog("No existe la ruta de XML de FEX usado en Guía.");
                                        }

                                        ctx.Undo();
                                        CloseHandle(token);
                                    }
                                }

                                //User token that represents the authorized user account
                                //IntPtr token = IntPtr.Zero;
                                string USER_rutaXmlA1Firmar = ConfigurationManager.AppSettings["USER_rutaXmlA1Firmar"];
                                USER_rutaXmlA1Firmar = string.IsNullOrEmpty(USER_rutaXmlA1Firmar) ? "admin" : USER_rutaXmlA1Firmar;
                                string PASS_rutaXmlA1Firmar = ConfigurationManager.AppSettings["PASS_rutaXmlA1Firmar"];
                                PASS_rutaXmlA1Firmar = string.IsNullOrEmpty(PASS_rutaXmlA1Firmar) ? "admin" : PASS_rutaXmlA1Firmar;
                                string DOMAIN_rutaXmlA1Firmar = ConfigurationManager.AppSettings["DOMAIN_rutaXmlA1Firmar"];
                                DOMAIN_rutaXmlA1Firmar = string.IsNullOrEmpty(DOMAIN_rutaXmlA1Firmar) ? "" : DOMAIN_rutaXmlA1Firmar;

                                result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                if (result == true)
                                {
                                    //Use token to setup a WindowsImpersonationContext 
                                    using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                    {
                                        if (!string.IsNullOrEmpty(pathA1FirmarFileXml))
                                        {
                                            if (!(Directory.Exists(pathA1FirmarFileXml)))
                                            {
                                                Directory.CreateDirectory(pathA1FirmarFileXml);
                                                LogController.WriteLog(pathA1FirmarFileXml + ": Creado Exitosamente");
                                            }
                                            if (Directory.Exists(pathA1FirmarFileXml))
                                            {
                                                string pathFileXmlFileName = pathA1FirmarFileXml + "\\" + pRemissionGuide.Document.accessKey + ".xml";
                                                xmlout1.Save(pathFileXmlFileName);

                                                LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");

                                                if (!string.IsNullOrEmpty(pathFileXml))
                                                {
                                                    if (Directory.Exists(pathFileXml))
                                                    {
                                                        string pathFileXmlFileName2 = pathFileXml + "\\" + pRemissionGuide.Document.accessKey + ".xml";
                                                        System.IO.File.Delete(pathFileXmlFileName2);
                                                        LogController.WriteLog(pathFileXmlFileName2 + ": Eliminado Exitosamente");
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            LogController.WriteLog("No existe la ruta de A1.Firmar.");
                                        }
                                        ctx.Undo();
                                        CloseHandle(token);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogController.WriteLog(ex.Message);
                        }
                        pElectronicDocument.xml = xmlout1.OuterXml;

                    }
                }
                else
                {
                    return;
                }



                try
                {
                    if (isNew)
                    {
                        db.ElectronicDocument.Add(pElectronicDocument);
                    }
                    else
                    {
                        db.ElectronicDocument.Attach(pElectronicDocument);
                        db.Entry(pElectronicDocument).State = EntityState.Modified;
                    }

                    pElectronicDocument.id = pRemissionGuide.id;


                }

                catch //(Exception ex)
                {
                }

            }
            //}
            //catch (Exception ep)
            //{


            //}

        }

        //Impersonation functionality
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        //Disconnection after file operations
        [DllImport("kernel32.dll")]
        private static extern Boolean CloseHandle(IntPtr hObject);

        public static void GeneraGuiaElectronicaFluvialXmlPers(DBContext db, int id_company, RemissionGuideRiver pRemissionGuide, string pathFileXml, string pathA1FirmarFileXml)
        {
            XmlDocument xmlout1 = null;
            ElectronicDocument pElectronicDocument = new ElectronicDocument();
            bool isNew = false;
            if (pRemissionGuide != null)
            {
                // FACTURACION ELECTRONICA - GENERACION DE XML
                if (pRemissionGuide.Document.DocumentType.isElectronic)
                {
                    ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == id_company && e.sriCode.Equals("01"));

                    if (electronicState != null)
                    {
                        try
                        {
                            #region Procesamiento del XML
                            if (pRemissionGuide.Document.ElectronicDocument == null)
                            {
                                pElectronicDocument = new ElectronicDocument
                                {
                                    id_electronicDocumentState = electronicState.id
                                };
                                isNew = true;
                            }
                            else
                            {
                                pElectronicDocument = pRemissionGuide.Document.ElectronicDocument;
                            }

                            pElectronicDocument.isGenerated = false;

                            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
                            XmlGuiaRemision xmlGuiaRemision = DB2XML.GuiaRemision4Xml(pRemissionGuide.id);
                            xmlout1 = SerializeToXml(xmlGuiaRemision, ns);


                            String xml = xmlout1.OuterXml;
                            xml = xml.Replace(@"p2:nil=""true""", "");
                            xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
                            xml = xml.Replace("<infoAdicional  />", "");

                            xmlout1.LoadXml(xml);
                            #endregion

                            if (pathFileXml != "")
                            {
                                //User token that represents the authorized user account
                                IntPtr token = IntPtr.Zero;
                                string USER_rutaXmlFEX = ConfigurationManager.AppSettings["USER_rutaXmlFEX"];
                                USER_rutaXmlFEX = string.IsNullOrEmpty(USER_rutaXmlFEX) ? "admin" : USER_rutaXmlFEX;
                                string PASS_rutaXmlFEX = ConfigurationManager.AppSettings["PASS_rutaXmlFEX"];
                                PASS_rutaXmlFEX = string.IsNullOrEmpty(PASS_rutaXmlFEX) ? "admin" : PASS_rutaXmlFEX;
                                string DOMAIN_rutaXmlFEX = ConfigurationManager.AppSettings["DOMAIN_rutaXmlFEX"];
                                DOMAIN_rutaXmlFEX = string.IsNullOrEmpty(DOMAIN_rutaXmlFEX) ? "" : DOMAIN_rutaXmlFEX;

                                bool result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                if (result == true)
                                {
                                    //Use token to setup a WindowsImpersonationContext 
                                    using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                    {

                                        if (!string.IsNullOrEmpty(pathFileXml))
                                        {
                                            if (!(Directory.Exists(pathFileXml)))
                                            {
                                                Directory.CreateDirectory(pathFileXml);
                                                LogController.WriteLog(pathFileXml + ": Creado Exitosamente");
                                            }
                                            if (Directory.Exists(pathFileXml))
                                            {
                                                string pathFileXmlFileName = pathFileXml + "\\" + pRemissionGuide.Document.accessKey + ".xml";
                                                xmlout1.Save(pathFileXmlFileName);
                                                LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");
                                            }
                                        }
                                        else
                                        {
                                            LogController.WriteLog("No existe la ruta de XML de FEX usado en Guía.");
                                        }

                                        ctx.Undo();
                                        CloseHandle(token);
                                    }
                                }

                                //User token that represents the authorized user account
                                //IntPtr token = IntPtr.Zero;
                                string USER_rutaXmlA1Firmar = ConfigurationManager.AppSettings["USER_rutaXmlA1Firmar"];
                                USER_rutaXmlA1Firmar = string.IsNullOrEmpty(USER_rutaXmlA1Firmar) ? "admin" : USER_rutaXmlA1Firmar;
                                string PASS_rutaXmlA1Firmar = ConfigurationManager.AppSettings["PASS_rutaXmlA1Firmar"];
                                PASS_rutaXmlA1Firmar = string.IsNullOrEmpty(PASS_rutaXmlA1Firmar) ? "admin" : PASS_rutaXmlA1Firmar;
                                string DOMAIN_rutaXmlA1Firmar = ConfigurationManager.AppSettings["DOMAIN_rutaXmlA1Firmar"];
                                DOMAIN_rutaXmlA1Firmar = string.IsNullOrEmpty(DOMAIN_rutaXmlA1Firmar) ? "" : DOMAIN_rutaXmlA1Firmar;

                                result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                if (result == true)
                                {
                                    //Use token to setup a WindowsImpersonationContext 
                                    using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                    {
                                        if (!string.IsNullOrEmpty(pathA1FirmarFileXml))
                                        {
                                            if (!(Directory.Exists(pathA1FirmarFileXml)))
                                            {
                                                Directory.CreateDirectory(pathA1FirmarFileXml);
                                                LogController.WriteLog(pathA1FirmarFileXml + ": Creado Exitosamente");
                                            }
                                            if (Directory.Exists(pathA1FirmarFileXml))
                                            {
                                                string pathFileXmlFileName = pathA1FirmarFileXml + "\\" + pRemissionGuide.Document.accessKey + ".xml";
                                                xmlout1.Save(pathFileXmlFileName);

                                                LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");

                                                if (!string.IsNullOrEmpty(pathFileXml))
                                                {
                                                    if (Directory.Exists(pathFileXml))
                                                    {
                                                        string pathFileXmlFileName2 = pathFileXml + "\\" + pRemissionGuide.Document.accessKey + ".xml";
                                                        System.IO.File.Delete(pathFileXmlFileName2);
                                                        LogController.WriteLog(pathFileXmlFileName2 + ": Eliminado Exitosamente");
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            LogController.WriteLog("No existe la ruta de A1.Firmar.");
                                        }
                                        ctx.Undo();
                                        CloseHandle(token);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogController.WriteLog(ex.Message);
                        }

                        pElectronicDocument.xml = xmlout1.OuterXml;

                    }
                }
                else
                {
                    return;
                }



                try
                {
                    if (isNew)
                    {
                        db.ElectronicDocument.Add(pElectronicDocument);
                    }
                    else
                    {
                        db.ElectronicDocument.Attach(pElectronicDocument);
                        db.Entry(pElectronicDocument).State = EntityState.Modified;
                    }

                    pElectronicDocument.id = pRemissionGuide.id;


                }
                catch
                {
                }

            }
        }

        private static XmlDocument SerializeToXml<T>(T source, XmlSerializerNamespaces ns)
        {
            var document = new XmlDocument();
            var navigator = document.CreateNavigator();

            using (var writer = navigator.AppendChild())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, source, ns);
            }
            return document;
        }

        public static AnswerRGService GenerateRequestToWarehouseFromRemissionGuide(ParamRGRequestWarehouse _rgRequestWarehouse)
        {
            AnswerRGService _ans = new AnswerRGService();

            #region Variables
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            List<ObjItemDispMaterial> _lstItem;
            List<ObjItemDispMaterial> _lstDetailOjbItDisMat;
            int id_reqHeader;
            int iCont;
            int id_emissionPoint = _rgRequestWarehouse.rgTmp.Document.id_emissionPoint;
            EmissionPoint _epTmp = _rgRequestWarehouse.dbTmp.EmissionPoint.FirstOrDefault(fod => fod.id == id_emissionPoint);
            Company _coTmp = _rgRequestWarehouse.dbTmp.Company.FirstOrDefault(fod => fod.id == _epTmp.id_company);
            //bool isNew = false;
            int idDa = 0;
            int idUs = 0;
            #endregion

            #region DATA REGISTER
            //GET IF THERE IS ANY REQUEST (NOT ANY WITH CANCELED[anulado] STATE)
            var lstRequestWithDocumentSource = _rgRequestWarehouse
                                                            .dbTmp
                                                            .DocumentSource
                                                            .Where(w => w.Document.DocumentType.code == "79"
                                                               && !w.Document.DocumentState.code.Equals("05")
                                                               && w.id_documentOrigin == _rgRequestWarehouse.id_RemissionGuide)
                                                            .Select(s => s.id_document)
                                                            .ToList();

            //GET ALL THE WAREHOUSE FROM REMISSION GUIDE 
            var lstWarehouse = _rgRequestWarehouse
                                    .rgTmp
                                    .RemissionGuideDispatchMaterial
                                    .Select(s => s.id_warehouse)
                                    .Distinct()
                                    .ToList();

            string varParFecha = _rgRequestWarehouse.dbTmp.Setting.FirstOrDefault(fod => fod.code == "GXP")?.value;
            var document = _rgRequestWarehouse.dbTmp.Document.FirstOrDefault(fod => fod.id == _rgRequestWarehouse.id_RemissionGuide);
            //Request is made by Warehouse
            //Case There is Not Request To Warehouse For This Remission Guide
            #region CREATE REQUEST
            if (lstRequestWithDocumentSource != null && lstRequestWithDocumentSource.Count == 0)
            {
                //isNew = true;
                if (lstWarehouse != null && lstWarehouse.Count > 0)
                {
                    foreach (var i in lstWarehouse)
                    {
                        //Verify For an Item wich has isActive = True
                        iCont = _rgRequestWarehouse
                                    .rgTmp
                                    .RemissionGuideDispatchMaterial
                                    .Where(w => w.isActive && w.id_warehouse == i).Count();
                        if (iCont > 0)
                        {
                            RequestInventoryMove rim = new RequestInventoryMove();
                            rim.id_PersonRequest = _rgRequestWarehouse.rgTmp.id_reciver;
                            rim.id_Warehouse = i;
                            //This is For Nature
                            rim.id_NatureMove = _rgRequestWarehouse
                                                                    .dbTmp.AdvanceParametersDetail
                                                                    .FirstOrDefault(fod => fod.AdvanceParameters.code == "NMMGI"
                                                                    && fod.valueCode == "E").id;

                            #region DOCUMENT CREATION AND ASSIGNATION
                            Document docRequestInventory = new Document();

                            var user_reciver = _rgRequestWarehouse.dbTmp.User.FirstOrDefault(fod => fod.id_employee == _rgRequestWarehouse.rgTmp.id_reciver);
                            if(user_reciver == null)
                            {
                                var nombre = _rgRequestWarehouse.dbTmp.Person.FirstOrDefault(e => e.id == _rgRequestWarehouse.rgTmp.id_reciver)?.fullname_businessName;
                                throw new Exception($"No se ha encontrado un usuario del sistema relacionado a quien recibe: {nombre}.");
                            }

                            docRequestInventory.id_userCreate = user_reciver.id;
                            docRequestInventory.dateCreate = DateTime.Now;
                            docRequestInventory.id_userUpdate = user_reciver.id;
                            docRequestInventory.dateUpdate = DateTime.Now;

                            if (varParFecha == "1")
                            {
                                docRequestInventory.emissionDate = document.emissionDate;
                            }
                            else
                            {
                                docRequestInventory.emissionDate = DateTime.Now;
                            }


                            DocumentType docTypeRequestInventory = _rgRequestWarehouse.dbTmp.DocumentType.FirstOrDefault(fod => fod.code == "79");

                            docRequestInventory.sequential = docTypeRequestInventory?.currentNumber ?? 0;
                            docRequestInventory.number = ServiceDocument.GetDocumentNumber(docTypeRequestInventory.id, _rgRequestWarehouse.dbTmp, _coTmp, _epTmp);
                            docRequestInventory.DocumentType = docTypeRequestInventory;

                            DocumentState docStateBuyIce = _rgRequestWarehouse.dbTmp.DocumentState
                                                                        .FirstOrDefault(s => s.code == "01");

                            docRequestInventory.DocumentState = docStateBuyIce;
                            docRequestInventory.id_documentState = docStateBuyIce.id;

                            docRequestInventory.id_emissionPoint = _epTmp?.id ?? 0;
                            docRequestInventory.EmissionPoint = _epTmp;

                            //Update Sequential For Document Type 79 REQUEST INVENTORY
                            if (docTypeRequestInventory != null)
                            {
                                docTypeRequestInventory.currentNumber = docTypeRequestInventory.currentNumber + 1;
                                _rgRequestWarehouse.dbTmp.DocumentType.Attach(docTypeRequestInventory);
                                _rgRequestWarehouse.dbTmp.Entry(docTypeRequestInventory).State = EntityState.Modified;
                            }
                            rim.Document = docRequestInventory;
                            #endregion

                            //Now Consider all Item from dispatchMaterials
                            #region DETAILS
                            rim.RequestInventoryMoveDetail = new List<RequestInventoryMoveDetail>();
                            _lstItem = _rgRequestWarehouse
                                        .rgTmp
                                        .RemissionGuideDispatchMaterial
                                        .Where(w => w.isActive && w.id_warehouse == i)
                                        .Select(s =>
                                            new ObjItemDispMaterial
                                            {
                                                id_item = s.id_item,
                                                quantityDispatch = s.sourceExitQuantity,
                                                isActive = s.isActive,
                                                id_warehouseLocation = s.id_warehouselocation
                                            })
                                        .ToList();

                            foreach (ObjItemDispMaterial _objItemDisMat in _lstItem)
                            {
                                RequestInventoryMoveDetail rimDetail = new RequestInventoryMoveDetail();
                                rimDetail.id_item = _objItemDisMat.id_item;
                                rimDetail.isActive = _objItemDisMat.isActive;
                                rimDetail.quantityRequest = _objItemDisMat.quantityDispatch;
                                rimDetail.idWarehouseLocation = _objItemDisMat.id_warehouseLocation;
                                rim.RequestInventoryMoveDetail.Add(rimDetail);
                            }
                            _rgRequestWarehouse.dbTmp.RequestInventoryMove.Add(rim);


                            #endregion

                            ServiceDocument.UpdateDocumentSource(rim.Document, _rgRequestWarehouse.rgTmp.Document, _rgRequestWarehouse.dbTmp);

                            idDa = _rgRequestWarehouse.dbTmp.tbsysActionOnDocument.FirstOrDefault(fod => fod.code.Equals("CD")).id;
                            idUs = _rgRequestWarehouse.dbTmp.User.FirstOrDefault(fod => fod.id_employee == _rgRequestWarehouse.rgTmp.id_reciver).id;
                            ServiceDocument.SaveTrackState(rim.Document, idDa, idUs, _rgRequestWarehouse.dbTmp);
                        }
                    }
                }
            }
            #endregion
            //Case There is any Request To Warehouse For this Remission Guide 
            //Then you have to change quantities if only there is not any InventoryMove
            #region UPDATE REQUEST CREATE WHEN NOT EXIST
            else
            {
                List<ObjItemDispMaterial> _lstOjbItDisMat = _rgRequestWarehouse
                                                                .dbTmp
                                                                .RequestInventoryMoveDetail
                                                                .Where(w => lstRequestWithDocumentSource.Contains(w.RequestInventoryMove.id))
                                                                .Select(s =>
                                                                    new ObjItemDispMaterial
                                                                    {
                                                                        id_ReqHeader = s.id_RequestInventoryMove,
                                                                        id_ReqDetail = s.id,
                                                                        id_item = s.id_item,
                                                                        id_warehouse = s.RequestInventoryMove.id_Warehouse,
                                                                        id_warehouseLocation = s.idWarehouseLocation,
                                                                        quantityDispatch = s.quantityRequest,
                                                                        isActive = s.isActive
                                                                    })
                                                                .ToList();
                if (lstWarehouse != null && lstWarehouse.Count > 0)
                {
                    foreach (var i in lstWarehouse)
                    {
                        _lstItem = _rgRequestWarehouse
                                    .rgTmp
                                    .RemissionGuideDispatchMaterial
                                    .Where(w => w.id_warehouse == i)
                                    .Select(s =>
                                        new ObjItemDispMaterial
                                        {
                                            id_item = s.id_item,
                                            quantityDispatch = s.sourceExitQuantity,
                                            id_warehouseLocation = s.id_warehouselocation,
                                            isActive = s.isActive
                                        })
                                    .ToList();
                        _lstDetailOjbItDisMat = _lstOjbItDisMat.Where(w => w.id_warehouse == i).ToList();
                        id_reqHeader = _lstOjbItDisMat.FirstOrDefault(fod => fod.id_warehouse == i)?.id_ReqHeader ?? 0;

                        #region DOCUMENT FOR THIS WAREHOUSE EXISTS
                        if (id_reqHeader > 0)
                        {
                            idDa = _rgRequestWarehouse.dbTmp.tbsysActionOnDocument.FirstOrDefault(fod => fod.code.Equals("MD")).id;
                            idUs = _rgRequestWarehouse.dbTmp.User.FirstOrDefault(fod => fod.id_employee == _rgRequestWarehouse.rgTmp.id_reciver).id;
                            ServiceDocument.SaveTrackState(_rgRequestWarehouse.dbTmp.Document.FirstOrDefault(fod => fod.id == id_reqHeader), idDa, idUs, _rgRequestWarehouse.dbTmp);

                            foreach (ObjItemDispMaterial _obj in _lstItem)
                            {

                                ObjItemDispMaterial _objRequestDetail = _lstDetailOjbItDisMat.FirstOrDefault(fod => fod.id_item == _obj.id_item);
                                if (_objRequestDetail == null)
                                {
                                    //Case 1: Look up for a Document wich has a Request to Warehouse (i) with this RemissionGuide if I is New then Insert
                                    #region INSERT DETAIL 
                                    RequestInventoryMoveDetail rqd = new RequestInventoryMoveDetail();
                                    rqd.id_RequestInventoryMove = id_reqHeader;
                                    rqd.quantityRequest = _obj.quantityDispatch;
                                    rqd.id_item = _obj.id_item;
                                    rqd.isActive = _obj.isActive;
                                    rqd.idWarehouseLocation = _obj.id_warehouseLocation;

                                    _rgRequestWarehouse.dbTmp.RequestInventoryMoveDetail.Add(rqd);
                                    _rgRequestWarehouse.dbTmp.Entry(rqd).State = EntityState.Added;
                                    #endregion
                                }
                                else
                                {
                                    //Case 2, 3: If it has Some Details I have to Update values if is necessary; If it has Some Details I have to Delete values if is necessary
                                    #region UPDATE OR DELETE(INACTIVE) DETAIL
                                    RequestInventoryMoveDetail rimdTmp = _rgRequestWarehouse
                                                                            .dbTmp
                                                                            .RequestInventoryMoveDetail
                                                                            .FirstOrDefault(fod => fod.id == _objRequestDetail.id_ReqDetail);
                                    rimdTmp.quantityRequest = _obj.quantityDispatch;
                                    rimdTmp.idWarehouseLocation = _obj.id_warehouseLocation;
                                    rimdTmp.isActive = _obj.isActive;
                                    _rgRequestWarehouse.dbTmp.RequestInventoryMoveDetail.Attach(rimdTmp);
                                    _rgRequestWarehouse.dbTmp.Entry(rimdTmp).State = EntityState.Modified;
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        #region DOCUMENT FOR THIS WAREHOUSE DOES NOT EXIST
                        else
                        {
                            //Verify For an Item wich has isActive = True
                            iCont = _rgRequestWarehouse
                                        .rgTmp
                                        .RemissionGuideDispatchMaterial
                                        .Where(w => w.isActive && w.id_warehouse == i).Count();

                            if (iCont > 0)
                            {
                                RequestInventoryMove rim = new RequestInventoryMove();
                                rim.id_PersonRequest = _rgRequestWarehouse.rgTmp.id_reciver;
                                rim.id_Warehouse = i;
                                //This is For Nature
                                rim.id_NatureMove = _rgRequestWarehouse
                                                                        .dbTmp.AdvanceParametersDetail
                                                                        .FirstOrDefault(fod => fod.AdvanceParameters.code == "NMMGI"
                                                                        && fod.valueCode == "E").id;

                                #region DOCUMENT CREATION AND ASSIGNATION
                                Document docRequestInventory = new Document();

                                docRequestInventory.id_userCreate = _rgRequestWarehouse.dbTmp.User.FirstOrDefault(fod => fod.id_employee == _rgRequestWarehouse.rgTmp.id_reciver).id;
                                docRequestInventory.dateCreate = DateTime.Now;
                                docRequestInventory.id_userUpdate = _rgRequestWarehouse.dbTmp.User.FirstOrDefault(fod => fod.id_employee == _rgRequestWarehouse.rgTmp.id_reciver).id;
                                docRequestInventory.dateUpdate = DateTime.Now;

                                docRequestInventory.emissionDate = DateTime.Now;

                                DocumentType docTypeRequestInventory = _rgRequestWarehouse.dbTmp.DocumentType.FirstOrDefault(fod => fod.code == "79");

                                docRequestInventory.sequential = docTypeRequestInventory?.currentNumber ?? 0;
                                docRequestInventory.number = ServiceDocument.GetDocumentNumber(docTypeRequestInventory.id, _rgRequestWarehouse.dbTmp, _coTmp, _epTmp);
                                docRequestInventory.DocumentType = docTypeRequestInventory;

                                DocumentState docStateBuyIce = _rgRequestWarehouse.dbTmp.DocumentState
                                                                            .FirstOrDefault(s => s.code == "01");

                                docRequestInventory.DocumentState = docStateBuyIce;
                                docRequestInventory.id_documentState = docStateBuyIce.id;


                                docRequestInventory.id_emissionPoint = _epTmp?.id ?? 0;
                                docRequestInventory.EmissionPoint = _epTmp;

                                //Update Sequential For Document Type 79 REQUEST INVENTORY
                                if (docTypeRequestInventory != null)
                                {
                                    docTypeRequestInventory.currentNumber = docTypeRequestInventory.currentNumber + 1;
                                    _rgRequestWarehouse.dbTmp.DocumentType.Attach(docTypeRequestInventory);
                                    _rgRequestWarehouse.dbTmp.Entry(docTypeRequestInventory).State = EntityState.Modified;
                                }
                                rim.Document = docRequestInventory;
                                #endregion

                                //Now Consider all Item from dispatchMaterials
                                #region DETAILS
                                rim.RequestInventoryMoveDetail = new List<RequestInventoryMoveDetail>();
                                _lstItem = _rgRequestWarehouse
                                            .rgTmp
                                            .RemissionGuideDispatchMaterial
                                            .Where(w => w.isActive && w.id_warehouse == i)
                                            .Select(s =>
                                                new ObjItemDispMaterial
                                                {
                                                    id_item = s.id_item,
                                                    quantityDispatch = s.sourceExitQuantity,
                                                    id_warehouseLocation = s.id_warehouselocation,
                                                    isActive = s.isActive
                                                })
                                            .ToList();

                                foreach (ObjItemDispMaterial _objItemDisMat in _lstItem)
                                {
                                    RequestInventoryMoveDetail rimDetail = new RequestInventoryMoveDetail();
                                    rimDetail.id_item = _objItemDisMat.id_item;
                                    rimDetail.isActive = true;
                                    rimDetail.idWarehouseLocation = _objItemDisMat.id_warehouseLocation;
                                    rimDetail.quantityRequest = _objItemDisMat.quantityDispatch;
                                    rim.RequestInventoryMoveDetail.Add(rimDetail);
                                }
                                #endregion

                                _rgRequestWarehouse.dbTmp.RequestInventoryMove.Add(rim);
                                ServiceDocument.UpdateDocumentSource(rim.Document, _rgRequestWarehouse.rgTmp.Document, _rgRequestWarehouse.dbTmp);
                                idDa = _rgRequestWarehouse.dbTmp.tbsysActionOnDocument.FirstOrDefault(fod => fod.code.Equals("CD")).id;
                                idUs = _rgRequestWarehouse.dbTmp.User.FirstOrDefault(fod => fod.id_employee == _rgRequestWarehouse.rgTmp.id_reciver).id;
                                ServiceDocument.SaveTrackState(rim.Document, idDa, idUs, _rgRequestWarehouse.dbTmp);
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #endregion

            return _ans;
        }

        public static async Task CallXML(DBContext db,
                                         List<RemissionGuide> remissionGuideList,
                                         List<RemissionGuideRiver> remissionGuideRiverList,
                                         int idCompany,
                                         string routePath,
                                         string routePathA1Firmar,
                                         int delayInSeconds)
        {

            await Task.Run(() => generateGuiaXML(db,
                                                    remissionGuideList,
                                                    remissionGuideRiverList,
                                                    routePath,
                                                    routePathA1Firmar,
                                                    idCompany,
                                                    delayInSeconds
                                                    ));

        }

        public static async Task generateGuiaXML(DBContext db,
                                                        List<RemissionGuide> remissionGuideList,
                                                        List<RemissionGuideRiver> remissionGuideRiverList,
                                                       string routePath,
                                                       string routePathA1Firmar,
                                                       int idCompany,
                                                       int delayInSeconds
                                                     )
        {

            XmlDocument xmlFEX = new XmlDocument();
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(routePath))
                    {
                        bool aExistsRoutePath = true;

                        IntPtr token = IntPtr.Zero;

                        string USER_rutaXmlFEX = ConfigurationManager.AppSettings["USER_rutaXmlFEX"];
                        USER_rutaXmlFEX = string.IsNullOrEmpty(USER_rutaXmlFEX) ? "admin" : USER_rutaXmlFEX;
                        string PASS_rutaXmlFEX = ConfigurationManager.AppSettings["PASS_rutaXmlFEX"];
                        PASS_rutaXmlFEX = string.IsNullOrEmpty(PASS_rutaXmlFEX) ? "admin" : PASS_rutaXmlFEX;
                        string DOMAIN_rutaXmlFEX = ConfigurationManager.AppSettings["DOMAIN_rutaXmlFEX"];
                        DOMAIN_rutaXmlFEX = string.IsNullOrEmpty(DOMAIN_rutaXmlFEX) ? "" : DOMAIN_rutaXmlFEX;

                        bool result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                        if (result == true)
                        {
                            //Use token to setup a WindowsImpersonationContext 
                            using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                            {
                                aExistsRoutePath = Directory.Exists(routePath);
                                if (!(aExistsRoutePath))
                                {
                                    Directory.CreateDirectory(routePath);
                                    LogController.WriteLog(routePath + ": Creado Exitosamente");
                                }

                                ctx.Undo();
                                CloseHandle(token);
                            }
                        }

                        if (aExistsRoutePath)
                        {
                            if (remissionGuideList != null)
                            {
                                foreach (RemissionGuide _guia in remissionGuideList)
                                {
                                    result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                    if (result == true)
                                    {
                                        //Use token to setup a WindowsImpersonationContext 
                                        using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                        {
                                            Task.Delay(delayInSeconds).Wait();
                                            xmlFEX = GenerateXML(db, _guia, idCompany);
                                            string pathFileXmlFileName = routePath + "\\" + _guia.Document.accessKey + ".xml";
                                            xmlFEX.Save(pathFileXmlFileName);
                                            LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");

                                            ctx.Undo();
                                            CloseHandle(token);
                                        }
                                    }

                                    bool aExistsRoutePathA1Firmar = true;

                                    if (!string.IsNullOrEmpty(routePathA1Firmar))
                                    {
                                        string USER_rutaXmlA1Firmar = ConfigurationManager.AppSettings["USER_rutaXmlA1Firmar"];
                                        USER_rutaXmlA1Firmar = string.IsNullOrEmpty(USER_rutaXmlA1Firmar) ? "admin" : USER_rutaXmlA1Firmar;
                                        string PASS_rutaXmlA1Firmar = ConfigurationManager.AppSettings["PASS_rutaXmlA1Firmar"];
                                        PASS_rutaXmlA1Firmar = string.IsNullOrEmpty(PASS_rutaXmlA1Firmar) ? "admin" : PASS_rutaXmlA1Firmar;
                                        string DOMAIN_rutaXmlA1Firmar = ConfigurationManager.AppSettings["DOMAIN_rutaXmlA1Firmar"];
                                        DOMAIN_rutaXmlA1Firmar = string.IsNullOrEmpty(DOMAIN_rutaXmlA1Firmar) ? "" : DOMAIN_rutaXmlA1Firmar;

                                        result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                        if (result == true)
                                        {
                                            //Use token to setup a WindowsImpersonationContext 
                                            using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                            {
                                                aExistsRoutePathA1Firmar = Directory.Exists(routePathA1Firmar);

                                                if (!(aExistsRoutePathA1Firmar))
                                                {
                                                    Directory.CreateDirectory(routePathA1Firmar);
                                                    LogController.WriteLog(routePathA1Firmar + ": Creado Exitosamente");
                                                }

                                                ctx.Undo();
                                                CloseHandle(token);
                                            }
                                        }


                                        if (aExistsRoutePathA1Firmar)
                                        {
                                            result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                            if (result == true)
                                            {
                                                //Use token to setup a WindowsImpersonationContext 
                                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                                {
                                                    //Task.Delay(delayInSeconds).Wait();
                                                    //xmlFEX = _invoiceExterior.GenerateXML(idCompany);
                                                    string pathFileXmlFileName2 = routePathA1Firmar + "\\" + _guia.Document.accessKey + ".xml";
                                                    xmlFEX.Save(pathFileXmlFileName2);
                                                    LogController.WriteLog(pathFileXmlFileName2 + ": Salvado Exitosamente");

                                                    ctx.Undo();
                                                    CloseHandle(token);
                                                }
                                            }


                                            if (!string.IsNullOrEmpty(routePath))
                                            {
                                                result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                                if (result == true)
                                                {
                                                    //Use token to setup a WindowsImpersonationContext 
                                                    using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                                    {
                                                        if (Directory.Exists(routePath))
                                                        {
                                                            //Task.Delay(delayInSeconds).Wait();
                                                            string pathFileXmlFileName3 = routePath + "\\" + _guia.Document.accessKey + ".xml";
                                                            System.IO.File.Delete(pathFileXmlFileName3);
                                                            LogController.WriteLog(pathFileXmlFileName3 + ": Eliminado Exitosamente");
                                                        }

                                                        ctx.Undo();
                                                        CloseHandle(token);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LogController.WriteLog("No existe la ruta de A1.Firmar.");
                                    }

                                }
                            }

                            if (remissionGuideRiverList != null)
                            {
                                foreach (RemissionGuideRiver _guia in remissionGuideRiverList)
                                {
                                    result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                    if (result == true)
                                    {
                                        //Use token to setup a WindowsImpersonationContext 
                                        using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                        {
                                            Task.Delay(delayInSeconds).Wait();
                                            xmlFEX = GenerateXML(db, _guia, idCompany);
                                            string pathFileXmlFileName = routePath + "\\" + _guia.Document.accessKey + ".xml";
                                            xmlFEX.Save(pathFileXmlFileName);
                                            LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");

                                            ctx.Undo();
                                            CloseHandle(token);
                                        }
                                    }

                                    bool aExistsRoutePathA1Firmar = true;

                                    if (!string.IsNullOrEmpty(routePathA1Firmar))
                                    {
                                        string USER_rutaXmlA1Firmar = ConfigurationManager.AppSettings["USER_rutaXmlA1Firmar"];
                                        USER_rutaXmlA1Firmar = string.IsNullOrEmpty(USER_rutaXmlA1Firmar) ? "admin" : USER_rutaXmlA1Firmar;
                                        string PASS_rutaXmlA1Firmar = ConfigurationManager.AppSettings["PASS_rutaXmlA1Firmar"];
                                        PASS_rutaXmlA1Firmar = string.IsNullOrEmpty(PASS_rutaXmlA1Firmar) ? "admin" : PASS_rutaXmlA1Firmar;
                                        string DOMAIN_rutaXmlA1Firmar = ConfigurationManager.AppSettings["DOMAIN_rutaXmlA1Firmar"];
                                        DOMAIN_rutaXmlA1Firmar = string.IsNullOrEmpty(DOMAIN_rutaXmlA1Firmar) ? "" : DOMAIN_rutaXmlA1Firmar;

                                        result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                        if (result == true)
                                        {
                                            //Use token to setup a WindowsImpersonationContext 
                                            using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                            {
                                                aExistsRoutePathA1Firmar = Directory.Exists(routePathA1Firmar);

                                                if (!(aExistsRoutePathA1Firmar))
                                                {
                                                    Directory.CreateDirectory(routePathA1Firmar);
                                                    LogController.WriteLog(routePathA1Firmar + ": Creado Exitosamente");
                                                }

                                                ctx.Undo();
                                                CloseHandle(token);
                                            }
                                        }


                                        if (aExistsRoutePathA1Firmar)
                                        {
                                            result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                            if (result == true)
                                            {
                                                //Use token to setup a WindowsImpersonationContext 
                                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                                {
                                                    //Task.Delay(delayInSeconds).Wait();
                                                    //xmlFEX = _invoiceExterior.GenerateXML(idCompany);
                                                    string pathFileXmlFileName2 = routePathA1Firmar + "\\" + _guia.Document.accessKey + ".xml";
                                                    xmlFEX.Save(pathFileXmlFileName2);
                                                    LogController.WriteLog(pathFileXmlFileName2 + ": Salvado Exitosamente");

                                                    ctx.Undo();
                                                    CloseHandle(token);
                                                }
                                            }


                                            if (!string.IsNullOrEmpty(routePath))
                                            {
                                                result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                                if (result == true)
                                                {
                                                    //Use token to setup a WindowsImpersonationContext 
                                                    using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                                    {
                                                        if (Directory.Exists(routePath))
                                                        {
                                                            //Task.Delay(delayInSeconds).Wait();
                                                            string pathFileXmlFileName3 = routePath + "\\" + _guia.Document.accessKey + ".xml";
                                                            System.IO.File.Delete(pathFileXmlFileName3);
                                                            LogController.WriteLog(pathFileXmlFileName3 + ": Eliminado Exitosamente");
                                                        }

                                                        ctx.Undo();
                                                        CloseHandle(token);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LogController.WriteLog("No existe la ruta de A1.Firmar.");
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        LogController.WriteLog("No existe la ruta de XML de FEX.");
                    }

                }
                catch (Exception ex)
                {
                    LogController.WriteLog(ex.Message);
                }

            });


            Task.Delay(delayInSeconds).Wait();


            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    if (remissionGuideList != null)
                    {
                        foreach (RemissionGuide _guia in remissionGuideList)
                        {


                            db.RemissionGuide.Attach(_guia);
                            db.Entry(_guia).State = EntityState.Modified;
                        }
                    }

                    if (remissionGuideRiverList != null)
                    {
                        foreach (RemissionGuideRiver _guia in remissionGuideRiverList)
                        {


                            db.RemissionGuideRiver.Attach(_guia);
                            db.Entry(_guia).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();
                    trans.Commit();

                }
                catch //(Exception e)
                {

                    trans.Rollback();
                }


            }


        }

        public static XmlDocument GenerateXML(DBContext db, RemissionGuide pRemissionGuide, int company)
        {
            XmlDocument xmlout1 = new XmlDocument();
            //Boolean returnResult = false;


            // id documentState
            int? id_documentState = pRemissionGuide.Document.id_documentState;
            string code_documentState = db.DocumentState.FirstOrDefault(r => r.id == id_documentState)?.code ?? "";

            if (code_documentState != "09") return null;
            //if (this.InvoiceExterior == null) return null;
            //if (this.InvoiceDetail.Count() == 0) return null;
            if (pRemissionGuide.Document?.DocumentType?.isElectronic == false) return null;



            ElectronicDocument _electronicDocument = pRemissionGuide.Document.ElectronicDocument ?? new ElectronicDocument();
            ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == company && e.sriCode.Equals("01"));
            if (electronicState == null) return null;

            _electronicDocument = new ElectronicDocument
            {
                id_electronicDocumentState = electronicState.id
            };

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            var xmlGuiaRemision = DB2XML.GuiaRemision2Xml(pRemissionGuide);
            xmlout1 = SerializeToXml(xmlGuiaRemision, ns);


            String xml = xmlout1.OuterXml;
            xml = xml.Replace(@"p2:nil=""true""", "");
            xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
            xml = xml.Replace("<infoAdicional  />", "");

            xmlout1.LoadXml(xml);

            /* if (!string.IsNullOrEmpty( fullPathXML))
			 {
				 string pathFileXmlFileName = fullPathXML + "\\" + this.Document.accessKey + ".xml";
				 xmlout1.Save(pathFileXmlFileName);
			 }*/

            _electronicDocument.xml = xmlout1.OuterXml;

            pRemissionGuide.Document.ElectronicDocument = _electronicDocument;

            //db.Invoice.Attach(this);
            //db.Entry(this).State = EntityState.Modified;



            return xmlout1;
        }

        public static XmlDocument GenerateXML(DBContext db, RemissionGuideRiver pRemissionGuide, int company)
        {
            XmlDocument xmlout1 = new XmlDocument();
            //Boolean returnResult = false;


            // id documentState
            int? id_documentState = pRemissionGuide.Document.id_documentState;
            string code_documentState = db.DocumentState.FirstOrDefault(r => r.id == id_documentState)?.code ?? "";

            if (code_documentState != "09") return null;
            //if (this.InvoiceExterior == null) return null;
            //if (this.InvoiceDetail.Count() == 0) return null;
            if (pRemissionGuide.Document?.DocumentType?.isElectronic == false) return null;



            ElectronicDocument _electronicDocument = pRemissionGuide.Document.ElectronicDocument ?? new ElectronicDocument();
            ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == company && e.sriCode.Equals("01"));
            if (electronicState == null) return null;

            _electronicDocument = new ElectronicDocument
            {
                id_electronicDocumentState = electronicState.id
            };

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            var xmlGuiaRemision = DB2XML.GuiaRemision4Xml(pRemissionGuide.id);
            xmlout1 = SerializeToXml(xmlGuiaRemision, ns);


            String xml = xmlout1.OuterXml;
            xml = xml.Replace(@"p2:nil=""true""", "");
            xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
            xml = xml.Replace("<infoAdicional  />", "");

            xmlout1.LoadXml(xml);

            /* if (!string.IsNullOrEmpty( fullPathXML))
			 {
				 string pathFileXmlFileName = fullPathXML + "\\" + this.Document.accessKey + ".xml";
				 xmlout1.Save(pathFileXmlFileName);
			 }*/

            _electronicDocument.xml = xmlout1.OuterXml;

            pRemissionGuide.Document.ElectronicDocument = _electronicDocument;

            //db.Invoice.Attach(this);
            //db.Entry(this).State = EntityState.Modified;



            return xmlout1;
        }

        public static List<RGResultsQuery> GetAllRemissionGuide(RemissionGuide remissionGuide,
                                                  Document document,
                                                  string carRegistration,
                                                  DateTime? startEmissionDate,
                                                  DateTime? endEmissionDate,
                                                  DateTime? startAuthorizationDate,
                                                  DateTime? endAuthorizationDate,
                                                  DateTime? startDespachureDate, DateTime? endDespachureDate,
                                                  DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                                  DateTime? startentranceDateProductionBuilding, DateTime? endentranceDateProductionBuilding,
                                                  int[] items, int[] businessOportunities,
                                                  int? id_DocumentState_2,
                                                  int? id_provider_remission = null,
                                                  int? id_driver = null,
                                                  bool filterCustodianIncome = false
                                                  )
        {
            List<RGResultsQuery> lstRemissionGuides = new List<RGResultsQuery>();

            List<ParamSQL> lstParametersSql = new List<ParamSQL>();

            #region Armo Parametros de Consulta
            int id_docState = 0;
            string str_numberDoc = string.Empty;
            string str_referenceDoc = string.Empty;
            string str_startEmissionDate = string.Empty;
            string str_endEmissionDate = string.Empty;
            string str_startAuthDate = string.Empty;
            string str_endAuthDate = string.Empty;
            string str_accesKey = string.Empty;
            string str_authNumber = string.Empty;
            string str_startDespDate = string.Empty;
            string str_endDespDate = string.Empty;
            string str_startExitPlanctDate = string.Empty;
            string str_endExitPlanctDate = string.Empty;
            string str_startEntrancePlanctDate = string.Empty;
            string str_endEntrancePlanctDate = string.Empty;
            string str_isExport = string.Empty;
            string str_items = string.Empty;
            string str_carRegistration = string.Empty;
            string str_gruia_externa = string.Empty;
            int r_id_DocumentState_2 = 0;

            int r_id_provider_remission = 0;
            int r_id_driver = 0;
            bool r_filterCustodianIncome = false;

            if (document.id_documentState > 0) { id_docState = document.id_documentState; }
            if (!string.IsNullOrEmpty(remissionGuide.Guia_Externa)) { str_gruia_externa = remissionGuide.Guia_Externa; }
            if (!string.IsNullOrEmpty(document.number)) { str_numberDoc = document.number; }
            if (!string.IsNullOrEmpty(document.reference)) { str_referenceDoc = document.reference; }
            if (startEmissionDate != null) { str_startEmissionDate = startEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (endEmissionDate != null) { str_endEmissionDate = endEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (startAuthorizationDate != null) { str_startAuthDate = startAuthorizationDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (endAuthorizationDate != null) { str_endAuthDate = endAuthorizationDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (!string.IsNullOrEmpty(document.accessKey)) { str_accesKey = document.accessKey; }
            if (!string.IsNullOrEmpty(document.authorizationNumber)) { str_authNumber = document.authorizationNumber; }
            if (startDespachureDate != null) { str_startDespDate = startDespachureDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (endDespachureDate != null) { str_endDespDate = endDespachureDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (startexitDateProductionBuilding != null) { str_startExitPlanctDate = startexitDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (endexitDateProductionBuilding != null) { str_endExitPlanctDate = endexitDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (startentranceDateProductionBuilding != null) { str_startEntrancePlanctDate = startentranceDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (endentranceDateProductionBuilding != null) { str_endEntrancePlanctDate = endentranceDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (!string.IsNullOrEmpty(carRegistration)) { str_carRegistration = carRegistration; }
            if ((id_DocumentState_2 ?? 0) > 0) { r_id_DocumentState_2 = id_DocumentState_2.Value; }
            if ((id_provider_remission ?? 0) > 0) { r_id_provider_remission = id_provider_remission.Value; }
            if ((id_driver ?? 0) > 0) { r_id_driver = id_driver.Value; }
            if (filterCustodianIncome) { r_filterCustodianIncome = filterCustodianIncome; }
            #endregion

            XElement xe_parameter = new XElement("Root",
                                            new XElement("RGQ",
                                                new XAttribute("id_docState", id_docState),
                                                new XAttribute("str_numberDoc", str_numberDoc),
                                                new XAttribute("str_referenceDoc", str_referenceDoc),
                                                new XAttribute("str_startEmissionDate", str_startEmissionDate),
                                                new XAttribute("str_endEmissionDate", str_endEmissionDate),
                                                new XAttribute("str_startAuthDate", str_startAuthDate),
                                                new XAttribute("str_endAuthDate", str_endAuthDate),
                                                new XAttribute("str_accesKey", str_accesKey),
                                                new XAttribute("str_authNumber", str_authNumber),
                                                new XAttribute("str_startDespDate", str_startDespDate),
                                                new XAttribute("str_endDespDate", str_endDespDate),
                                                new XAttribute("str_startExitPlanctDate", str_startExitPlanctDate),
                                                new XAttribute("str_endExitPlanctDate", str_endExitPlanctDate),
                                                new XAttribute("str_startEntrancePlanctDate", str_startEntrancePlanctDate),
                                                new XAttribute("str_endEntrancePlanctDate", str_endEntrancePlanctDate),
                                                new XAttribute("str_carRegistration", str_carRegistration),
                                                new XAttribute("str_GuiaExterna", str_gruia_externa),
                                                new XAttribute("id_provider_remission", r_id_provider_remission),
                                                new XAttribute("id_driver", r_id_driver),
                                                new XAttribute("filterCustodianIncome", r_filterCustodianIncome)

                                            ));

            ParamSQL _param = new ParamSQL();
            _param.Nombre = "@P_xml";
            _param.TipoDato = DbType.Xml;
            _param.Valor = Utilitarios.General.GeneralStr.InnerXML(xe_parameter);
            lstParametersSql.Add(_param);

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];
            DataSet ds = AccesoDatos.MSSQL.MetodosDatos2.ObtieneDatos(_cadenaConexion
                                                , "pac_Guia_Remision_Resultado"
                                                , _rutaLog
                                                , "Logistics"
                                                , "PROD"
                                                , lstParametersSql);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                lstRemissionGuides = dt.AsEnumerable().Select(s => new RGResultsQuery()
                {
                    id = s.Field<Int32>("id"),
                    numberDoc = s.Field<String>("NumeroDocumento"),
                    numberDocPurchaseOrder = s.Field<String>("NumeroOrdenCompra"),
                    emissionDateDoc = s.Field<DateTime>("FechaEmision"),
                    providerName = s.Field<String>("NombreProveedor"),
                    productionUnitProviderName = s.Field<String>("NombreUnidadProd"),
                    despachureDateDoc = s.Field<DateTime>("FechaDespacho"),
                    certificadoName = s.Field<String>("NombreCertificado"),
                    exitTimePlanctDoc = s.Field<DateTime?>("SalidaPlanta"),
                    entranceTimePlanctDoc = s.Field<DateTime?>("EntradaPlanta"),
                    isThird = s.Field<Boolean>("LogisticaPropia"),
                    stateDoc = s.Field<String>("EstadoDocumento"),
                    stateDocElectronic = s.Field<String>("EstadoElectronico"),
                    personProcesPlant = s.Field<String>("personProcesPlant"),
                    guia_externa = s.Field<String>("GuiaExterna"),
                }).ToList();
            }
            return lstRemissionGuides;
        }


    }

}