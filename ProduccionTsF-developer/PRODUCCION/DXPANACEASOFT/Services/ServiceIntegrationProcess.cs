using AccesoDatos.MSSQL;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Auxiliares.IntegrationProcessService;
using DXPANACEASOFT.Models;
using EntidadesAuxiliares.SQL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;


namespace DXPANACEASOFT.Services
{
    public class ServiceIntegrationProcess
    {


        public static int[] GetIdsDocument(int idLote)
        {


            int[] IdsDocument = new int[0];


            return IdsDocument;

        }
        public static List<IntegrationProcess> FindLotes
                                                (
                                                    DBContext db,
                                                    int id_ActiveUser,
                                                    int id_Company,
                                                    int? id_DocumentType,
                                                    DateTime? dateInitCreate,
                                                    DateTime? dateEndCreate,
                                                    DateTime? dateInitExec,
                                                    DateTime? dateEndExec,
                                                    DateTime? dateInitFullIntegration,
                                                    DateTime? dateEndFullIntegration,
                                                    int? id_IntegrationProcessState,
                                                    int? id_Lote,
                                                    string description
                                                )
        {

            int countParameters = 0;
            List<IntegrationProcess> returnFindLotes = new List<IntegrationProcess>();

            try
            {

                // Tipo de Documento
                if (id_DocumentType != null && id_DocumentType != 0)
                {
                    countParameters++;
                    returnFindLotes = (returnFindLotes == null || returnFindLotes.Count() == 0)
                                      ? db.IntegrationProcess.Where(r => r.id_DocumentType == id_DocumentType).ToList()
                                      : returnFindLotes.Where(r => r.id_DocumentType == id_DocumentType).ToList();
                }

                // Fecha Inicio Creacion
                if (dateInitCreate != null)
                {
                    countParameters++;
                    if (returnFindLotes == null || returnFindLotes.Count() == 0)
                    {
                        List<IntegrationProcess> returnFindLotes_Date = db.IntegrationProcess.Where(r => r.dateCreate != null).ToList();
                        returnFindLotes = returnFindLotes_Date.Where(o => DateTime.Compare(dateInitCreate.Value.Date, o.dateCreate.Date) <= 0).ToList();
                    }
                    else
                    {

                        List<IntegrationProcess> returnFindLotes_Date = returnFindLotes.Where(r => r.dateCreate != null).ToList();
                        returnFindLotes = returnFindLotes_Date.Where(o => DateTime.Compare(dateInitCreate.Value.Date, o.dateCreate.Date) <= 0).ToList();
                    }

                }

                // Fecha Fin Creacion
                if (dateEndCreate != null)
                {
                    countParameters++;
                    if (returnFindLotes == null || returnFindLotes.Count() == 0)
                    {
                        List<IntegrationProcess> returnFindLotes_Date = db.IntegrationProcess.Where(r => r.dateCreate != null).ToList();
                        returnFindLotes = returnFindLotes_Date.Where(o => DateTime.Compare(o.dateCreate.Date, dateEndCreate.Value.Date) <= 0).ToList();
                    }
                    else
                    {
                        List<IntegrationProcess> returnFindLotes_Date = returnFindLotes.Where(r => r.dateCreate != null).ToList();
                        returnFindLotes = returnFindLotes_Date.Where(o => DateTime.Compare(o.dateCreate.Date, dateEndCreate.Value.Date) <= 0).ToList();
                    }


                }

                // Fecha Inicio Trasmision dateInitExec
                if (dateInitExec != null)
                {
                    countParameters++;
                    if (returnFindLotes == null || returnFindLotes.Count() == 0)
                    {                        // 1 obtener del log Documentos que se han ejecutado y la fecha comparar dateInitExec
                        List<IntegrationProcessLog> _integrationProcessLog = db.IntegrationProcessLog.Where(r => r.IntegrationState.code == EnumIntegrationProcess.States.Transmitted && r.dateAction != null).ToList();
                        returnFindLotes = _integrationProcessLog.Where(r => DateTime.Compare(dateInitExec.Value.Date, r.dateAction.Date) <= 0).Select(s => s.IntegrationProcess).ToList();

                    }
                    else
                    {

                        List<IntegrationProcess> _integrationProcess = new List<IntegrationProcess>();
                        // returnFindLotes = returnFindLotes.Where(r =>   DateTime.Compare(dateInitCreate.Value.Date, r.dateCreate.Date) <= 0).ToList();
                        returnFindLotes.ForEach(r =>
                        {
                            r.IntegrationProcessLog
                                .ToList()
                                .ForEach(s1 =>
                                {
                                    IntegrationProcess _localIntegrationProcess = new IntegrationProcess();
                                    if (s1.IntegrationState.code == EnumIntegrationProcess.States.Transmitted
                                                                && DateTime.Compare(dateInitExec.Value.Date, s1.dateAction.Date) <= 0)
                                    {
                                        _integrationProcess.Add(r);
                                    }

                                });

                        });
                        returnFindLotes = _integrationProcess;

                    }

                }

                // Fecha Fin Transmision  dateEndExec
                if (dateEndExec != null)
                {
                    countParameters++;
                    if (returnFindLotes == null || returnFindLotes.Count() == 0)
                    {                        // 1 obtener del log Documentos que se han ejecutado y la fecha comparar dateInitExec
                        List<IntegrationProcessLog> _integrationProcessLog = db.IntegrationProcessLog.Where(r => r.IntegrationState.code == EnumIntegrationProcess.States.Transmitted && r.dateAction != null).ToList();
                        returnFindLotes = _integrationProcessLog.Where(r => DateTime.Compare(r.dateAction.Date, dateEndExec.Value.Date) <= 0).Select(s => s.IntegrationProcess).ToList();

                    }
                    else
                    {

                        List<IntegrationProcess> _integrationProcess = new List<IntegrationProcess>();
                        // returnFindLotes = returnFindLotes.Where(r =>   DateTime.Compare(dateInitCreate.Value.Date, r.dateCreate.Date) <= 0).ToList();
                        returnFindLotes.ForEach(r =>
                        {
                            r.IntegrationProcessLog
                                .ToList()
                                .ForEach(s1 =>
                                {
                                    IntegrationProcess _localIntegrationProcess = new IntegrationProcess();
                                    if (s1.IntegrationState.code == EnumIntegrationProcess.States.Transmitted
                                                                && DateTime.Compare(s1.dateAction.Date, dateEndExec.Value.Date) <= 0)
                                    {
                                        _integrationProcess.Add(r);
                                    }

                                });

                        });
                        returnFindLotes = _integrationProcess;

                    }

                }

                // DateTime? dateInitFullIntegration
                if (dateInitFullIntegration != null)
                {
                    countParameters++;
                    if (returnFindLotes == null || returnFindLotes.Count() == 0)
                    {                        // 1 obtener del log Documentos que se han ejecutado y la fecha comparar dateInitExec
                        List<IntegrationProcessLog> _integrationProcessLog = db.IntegrationProcessLog.Where(r => r.IntegrationState.code == EnumIntegrationProcess.States.Process && r.dateAction != null).ToList();
                        returnFindLotes = _integrationProcessLog.Where(r => DateTime.Compare(dateInitFullIntegration.Value.Date, r.dateAction.Date) <= 0).Select(s => s.IntegrationProcess).ToList();

                    }
                    else
                    {

                        List<IntegrationProcess> _integrationProcess = new List<IntegrationProcess>();
                        // returnFindLotes = returnFindLotes.Where(r =>   DateTime.Compare(dateInitCreate.Value.Date, r.dateCreate.Date) <= 0).ToList();
                        returnFindLotes.ForEach(r =>
                        {
                            r.IntegrationProcessLog
                                .ToList()
                                .ForEach(s1 =>
                                {
                                    IntegrationProcess _localIntegrationProcess = new IntegrationProcess();
                                    if (s1.IntegrationState.code == EnumIntegrationProcess.States.Process
                                                                && DateTime.Compare(dateInitFullIntegration.Value.Date, s1.dateAction.Date) <= 0)
                                    {
                                        _integrationProcess.Add(r);
                                    }

                                });

                        });
                        returnFindLotes = _integrationProcess;

                    }

                }


                // DateTime? dateEndFullIntegration
                if (dateEndFullIntegration != null)
                {
                    countParameters++;
                    if (returnFindLotes == null || returnFindLotes.Count() == 0)
                    {                        // 1 obtener del log Documentos que se han ejecutado y la fecha comparar dateInitExec
                        List<IntegrationProcessLog> _integrationProcessLog = db.IntegrationProcessLog.Where(r => r.IntegrationState.code == EnumIntegrationProcess.States.Process && r.dateAction != null).ToList();
                        returnFindLotes = _integrationProcessLog.Where(r => DateTime.Compare(r.dateAction.Date, dateEndFullIntegration.Value.Date) <= 0).Select(s => s.IntegrationProcess).ToList();

                    }
                    else
                    {

                        List<IntegrationProcess> _integrationProcess = new List<IntegrationProcess>();
                        // returnFindLotes = returnFindLotes.Where(r =>   DateTime.Compare(dateInitCreate.Value.Date, r.dateCreate.Date) <= 0).ToList();
                        returnFindLotes.ForEach(r =>
                        {
                            r.IntegrationProcessLog
                                .ToList()
                                .ForEach(s1 =>
                                {
                                    IntegrationProcess _localIntegrationProcess = new IntegrationProcess();
                                    if (s1.IntegrationState.code == EnumIntegrationProcess.States.Process
                                                                && DateTime.Compare(s1.dateAction.Date, dateEndFullIntegration.Value.Date) <= 0)
                                    {
                                        _integrationProcess.Add(r);
                                    }

                                });

                        });
                        returnFindLotes = _integrationProcess;

                    }

                }

                // int? id_IntegrationProcessState
                if (id_IntegrationProcessState != null && id_IntegrationProcessState != 0)
                {
                    countParameters++;
                    returnFindLotes = (returnFindLotes == null || returnFindLotes.Count() == 0)
                                      ? db.IntegrationProcess.Where(r => r.id_StatusLote == id_IntegrationProcessState).ToList()
                                      : returnFindLotes.Where(r => r.id_StatusLote == id_IntegrationProcessState).ToList();
                }


                // int? id_Lote
                if (id_Lote != null && id_Lote != 0)
                {
                    countParameters++;
                    returnFindLotes = (returnFindLotes == null || returnFindLotes.Count() == 0)
                                      ? db.IntegrationProcess.Where(r => r.id == id_Lote).ToList()
                                      : returnFindLotes.Where(r => r.id == id_Lote).ToList();
                }

                // string description
                /*
				if (!string.IsNullOrEmpty(description) )
				{
					countParameters++;
					returnFindLotes = (returnFindLotes == null || returnFindLotes.Count() == 0)
									  ? db.IntegrationProcess.Where(r => r.description  == description).ToList()
									  : returnFindLotes.Where(r => r.description == description).ToList();
				}
				*/


            }
            catch //(Exception e)
            {


            }


            if ((returnFindLotes == null || returnFindLotes.Count() == 0) && countParameters == 0)
            {
                returnFindLotes = db.IntegrationProcess.ToList();
            }
            else if ((returnFindLotes == null || returnFindLotes.Count() == 0) && countParameters > 0)
            {
                returnFindLotes = new List<IntegrationProcess>();

            }

            return returnFindLotes;


        }



        public static List<IntegrationProcessDetail> FindDocuments
                                               (
                                                   DBContext db,
                                                   int id_ActiveUser,
                                                   int id_Company,
                                                   int? id_DocumentType,
                                                   DateTime? dateInitEmission,
                                                   DateTime? dateEndEmission,
                                                   int?[] id_EmissionPoint,
                                                   string numberDocument,
                                                   int[] id_integrationProcessDetailList,
                                                   int id_IntegrationProcessLote
                                               )
        {


            List<IntegrationProcessDetail> returnIntegrationProcessDetail = new List<IntegrationProcessDetail>();
            tbsysIntegrationDocumentConfig integrationDocumentConfig = new tbsysIntegrationDocumentConfig();
            IIntegrationProcessActionOutput integrationImplementation;
            IEnumerable<Document> returnDocuments = new List<Document>();
            DocumentType documentType = new DocumentType();
            int countParameters = 0;
            string msgGeneral = "";

            try
            {
                // - Documentos no pueden estar Agregados en otro lote Creado, Procesando o Procesado
                // - Tienen que ser del tipo de documento indicado
                // - Deben estar en el estado final para el tipo de documento				
                // Obtener el tipo de documento + y el estado requerido para poder ser elegible

                documentType = db.DocumentType
                                    .AsNoTracking()
                                    .FirstOrDefault(r => r.id == id_DocumentType);

                if (documentType == null)
                {
                    throw new Exception("Tipo de Documento No Definido");
                }

                integrationDocumentConfig = db.tbsysIntegrationDocumentConfig
                                                    .FirstOrDefault(r => r.codeDocumentType == documentType.code);

                if (integrationDocumentConfig == null)
                {
                    throw new Exception("Configuración de Integracion No Definida");
                }

                integrationImplementation = (IIntegrationProcessActionOutput)IntegrationProcessDefinition(db, (int)id_DocumentType);


                // Obtener Status Agregado
                int id_IntegrationState_Added = db.IntegrationState
                                                       .AsNoTracking()
                                                       .FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Added)?.id ?? 0;

                // Obtener Status Eliminado
                int id_IntegrationState_Deleted = db.IntegrationState
                                                       .AsNoTracking()
                                                       .FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted)?.id ?? 0;

                if (id_IntegrationState_Added == 0)
                {
                    throw new Exception("Estado  de Integración Agregado No Definido");
                }

                if (id_IntegrationState_Deleted == 0)
                {
                    throw new Exception("Estado  de Integración Eliminado No Definido");
                }

                // Filtro Permisos de Usuario
                // Listado de Documentos Procesados o incluidos en Lotes
                // Exluir Documentos Anulados
                // Excluir Lotes Anulados
                int[] _id_IntegrationProcessDetailList = db.IntegrationProcessDetail
                                                                .AsNoTracking()
                                                                .Where(r =>
                                                                            r.id_StatusDocument != id_IntegrationState_Deleted
                                                                            && r.Document.id_documentType == id_DocumentType
                                                                            // && r.id_Lote != id_IntegrationProcessLote
                                                                            && r.IntegrationProcess.id_StatusLote != id_IntegrationState_Deleted
                                                                        )
                                                                        .Select(s => s.id_Document)
                                                                        .Distinct().ToArray();



                // Obtener Documentos Procesados
                // Filtro Tipo de Documento + Estado Documento
                if (id_DocumentType != null && id_DocumentType != 0)
                {

                    if (_id_IntegrationProcessDetailList == null || _id_IntegrationProcessDetailList.Count() == 0)
                    {

                        returnDocuments = db.Document
                            .AsNoTracking()
                            .Where(r => r.id_documentType == id_DocumentType);
                    }
                    else
                    {
                        returnDocuments = db.Document.AsNoTracking()
                                                .Where(r => r.id_documentType == id_DocumentType
                                                            && !_id_IntegrationProcessDetailList.Contains(r.id));

                    }
                }

                // Filtro Fecha inicio Obtencion Tipo de Documento
                if (integrationDocumentConfig.dateInitFindDocument != null && (returnDocuments.Any()))
                {
                    returnDocuments = returnDocuments
                        .Where(r => DateTime.Compare(integrationDocumentConfig.dateInitFindDocument.Value, r.emissionDate.Date) <= 0);
                }

                // Filtro Especifico del Tipo de Documento
                msgGeneral = integrationImplementation.FindDocument(db, integrationDocumentConfig, ref returnDocuments);

                if (!string.IsNullOrEmpty(msgGeneral))
                {
                    throw new Exception(msgGeneral);
                }

                // Simulacion Filtrado Anticipo  Viaticos
                //returnDocuments = returnDocuments.Where(r =>  r.RemissionGuideCustomizedViaticPersonalAssigned.Any(s=> s.tbsysCatalogState.codeState =="01")  ).ToList();

                if (returnDocuments == null || returnDocuments.Count() == 0)
                {
                    // No existen Documentos
                    return new List<IntegrationProcessDetail>();
                }

                // Pendiente 
                // Filtro Puntos de Emision
                if (id_EmissionPoint != null && id_EmissionPoint.Count() != 0)
                {
                    int emissionPointErr = id_EmissionPoint.Count(r => r != null);
                    if (emissionPointErr > 0)
                    {
                        countParameters++;
                        if (returnDocuments.Any())
                        {
                            returnDocuments.Where(r => id_EmissionPoint.Contains(r.id_emissionPoint));
                        }
                    }
                }

                // Filtro  numberDocument
                if (!string.IsNullOrEmpty(numberDocument))
                {
                    countParameters++;
                    if (returnDocuments.Any())
                    {
                        returnDocuments = returnDocuments.Where(r => r.number.Contains(numberDocument));
                    }
                }

                //dateInitEmission
                if (dateInitEmission != null)
                {
                    countParameters++;
                    //returnDocuments = db.Document.Where(r => DbFunctions.TruncateTime(r.emissionDate) <= dateInitEmission).ToList();
                    if (returnDocuments.Any())
                    {
                        returnDocuments = returnDocuments.Where(o => DateTime.Compare(dateInitEmission.Value.Date, o.emissionDate.Date) <= 0).ToList();
                    }
                }

                //dateEndEmission
                if (dateEndEmission != null)
                {
                    countParameters++;
                    if (returnDocuments.Any())
                    {
                        returnDocuments = returnDocuments.Where(o => DateTime.Compare(o.emissionDate.Date, dateEndEmission.Value.Date) <= 0).ToList();
                    }
                }

                if ((!returnDocuments.Any()) && countParameters > 0)
                {
                    returnIntegrationProcessDetail = new List<IntegrationProcessDetail>();

                }
                else
                {
                    returnIntegrationProcessDetail
                        = returnDocuments //.Where(e =>!id_integrationProcessDetailList.Contains(e.id))
                            .Select(r =>
                                new IntegrationProcessDetail()
                                {
                                    id_Document = r.id,
                                    Document = r,
                                    dateCreate = DateTime.Now,
                                    dateUpdate = DateTime.Now,
                                    id_StatusDocument = 1,
                                    id_UserCreate = id_ActiveUser,
                                    id_UserUpdate = id_ActiveUser,
                                    glossData = GetXGlosa(db, r.id, documentType.code,
                                        integrationImplementation.GetGlossX, 3),
                                    totalValueDocument = integrationImplementation.GetTotalValue(r)
                                }).ToList();
                    // Filtrar documentos con Valor mayor que cero
                    if (integrationDocumentConfig.code != null && integrationDocumentConfig.code == "PPC")
                    {
                        returnIntegrationProcessDetail = returnIntegrationProcessDetail.OrderBy(o => o.Document.Lot.ProductionLot.sequentialLiquidation).ToList(); ;
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return returnIntegrationProcessDetail;
        }

        internal static IIntegrationProcessActionOutput IntegrationProcessDefinition(DBContext db, int id_DocumentType)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.id == id_DocumentType);
            if (documentType == null)
            {
                throw new Exception("Configuracion de tipo de Documento para el tipo de documento seleccionado, no definido.");
            }

            tbsysIntegrationDocumentConfig integrationParameters = db.tbsysIntegrationDocumentConfig
                                                                              .FirstOrDefault(r => r.codeDocumentType == documentType.code);


            if (integrationParameters == null)
            {
                throw new Exception("Configuración de Integración para el Tipo de Documento  no está definido.");
            }


            //Type typeOfIntegrationOuput = Type.GetType("DXPANACEASOFT.Auxiliares.IntegrationProcessService.IntegrationProcessActionOutputAdvanceProviderShrimp", true);
            Type typeOfIntegrationImplementation = Type.GetType(integrationParameters.implementingIntegration, true);
            IIntegrationProcessActionOutput integrationImplementation = (IIntegrationProcessActionOutput)Activator.CreateInstance(typeOfIntegrationImplementation);

            return integrationImplementation;

        }



        public static void SaveLog
                           (
                             DBContext db,
                             int id_ActiveUser,
                             ref IntegrationProcess integrationProcess,
                             int id_StatusDocument
                           )
        {
            try
            {
                IntegrationProcessLog integrationProcessLog = new IntegrationProcessLog();
                integrationProcessLog.id_StatusLote = id_StatusDocument;
                integrationProcessLog.id_userAction = id_ActiveUser;
                integrationProcessLog.dateAction = DateTime.Now;
                integrationProcess.IntegrationProcessLog.Add(integrationProcessLog);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void SaveDetailLog
                            (
                              DBContext db,
                              int id_ActiveUser,
                              ref IntegrationProcessDetail integrationProcessDetail,
                              int id_StatusDocument
                            )
        {

            try
            {
                IntegrationProcessDetailLog integrationProcessDetailLog = new IntegrationProcessDetailLog();
                integrationProcessDetailLog.id_StatusDocument = id_StatusDocument;
                integrationProcessDetailLog.id_userAction = id_ActiveUser;
                integrationProcessDetailLog.dateAction = DateTime.Now;
                integrationProcessDetail.IntegrationProcessDetailLog.Add(integrationProcessDetailLog);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static int SaveMessageError
                            (
                                DBContext db,
                                int id_ActiveUser,
                                int id_Referencia,
                                string messageErr,
                                string sourceReferenceCode,
                                string sourceMessageCode,
                                List<AdvanceParametersDetail> _parametersDetail
                            )
        {

            List<AdvanceParameters> _advanceParametersList = new List<AdvanceParameters>();
            //IntegrationProcessMessage _integrationProcessMessage = null;


            #region Variables SQLCOMMAND


            string _cadenaConexion = "";
            string _rutaLog = "";
            string _controllerName = "IntegrationProcess";
            string _sentencia = ResourceSqlScript.InsertIntegrationProcessMessage; //"insert into IntegrationProcessMessage(id_SourceReference,id_Reference,messageProcess,dateMessage,id_sourceMessage) values(@id_SourceReference,@id_Reference,@messageProcess,@dateMessage,@id_sourceMessage)";
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();

            #endregion

            try
            {


                int _idSourceReference = _parametersDetail
                                              .FirstOrDefault(r => (r.valueCode.Equals(sourceReferenceCode)))?.id ?? 0;

                int _idSourceMessage = _parametersDetail
                                              .FirstOrDefault(r => (r.valueCode.Equals(sourceMessageCode)))?.id ?? 0;
                if (_idSourceReference == 0 || _idSourceMessage == 0)
                {

                    throw new Exception("Parametros de Origen Referencia u Origen Mensaje no definido(s)");
                }

                #region SQLCOMMAND Prepare

                _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
                _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

                // Parametros  : id_SourceReference
                ParamSQL _param = new ParamSQL();
                _param.Nombre = "@id_SourceReference";
                _param.TipoDato = DbType.Int32;
                _param.Valor = _idSourceReference;
                lstParametersSql.Add(_param);

                // Parametros  : id_Reference
                _param = new ParamSQL();
                _param.Nombre = "@id_Reference";
                _param.TipoDato = DbType.Int32;
                _param.Valor = id_Referencia;
                lstParametersSql.Add(_param);

                // Parametros  : messageProcess
                _param = new ParamSQL();
                _param.Nombre = "@messageProcess";
                _param.TipoDato = DbType.String;
                _param.Valor = messageErr;
                lstParametersSql.Add(_param);

                // Parametros  : messageProcess
                _param = new ParamSQL();
                _param.Nombre = "@dateMessage";
                _param.TipoDato = DbType.Date;
                _param.Valor = DateTime.Now;
                lstParametersSql.Add(_param);

                // Parametros  : messageProcess
                _param = new ParamSQL();
                _param.Nombre = "@id_sourceMessage";
                _param.TipoDato = DbType.Int32;
                _param.Valor = _idSourceMessage;
                lstParametersSql.Add(_param);

                #endregion

                #region TRANSACCION

                AccesoDatos.MSSQL.MetodosDatos2.ExecuteNoQuery
                                              (
                                                _cadenaConexion
                                                , _sentencia
                                                , _rutaLog
                                                , _controllerName
                                                , "PROD"
                                                , lstParametersSql, true
                                               );
                /*
				_integrationProcessMessage = new IntegrationProcessMessage
				{
					id_Reference = id_Referencia,
					id_sourceMessage = _idSourceMessage,
					id_SourceReference = _idSourceReference,
					messageProcess = messageErr,
					dateMessage = DateTime.Now
				};
				 */

                #endregion


            }
            catch (Exception e)
            {

                throw e;
            }

            return 0;

        }

        public static string GetXGlosa
            (
                                DBContext db,
                                int id_document,
                                string code_documentType,
                                Func<DBContext, int, string, Dictionary<string, string>> getglosa,
                                int breakLine
            )
        {

            string returnData = "";
            int countBreakLine = 1;
            Dictionary<string, string> glosa = getglosa(db, id_document, code_documentType);
            //string lineBreak = "";
            string tagLineBreak = "";

            returnData = "<div class='ip_wrapperGloss'>";
            foreach (KeyValuePair<string, string> reg in glosa)
            {
                // classLineBreak = (countBreakLine == breakLine) ? "class='line'"  :"";
                if (countBreakLine == breakLine)
                {
                    tagLineBreak = "div";
                    countBreakLine = 0;
                }
                else
                {
                    tagLineBreak = "span";
                }

                returnData += "<" + tagLineBreak + "><span class='ip_title'>" + reg.Key + ":</span><span class='ip_text'>" + reg.Value + "</span></" + tagLineBreak + ">";
                countBreakLine++;
            }

            returnData += "</div>";

            return returnData;

        }


        public static void GetXGlosaDetail
            (
                                ref IntegrationProcessDetail integrationProcessDetail,
                                int breakLine
            )
        {

            string returnData = "";
            int countBreakLine = 1;
            string[] delimiterField = new string[] { "{|}" };
            string[] delimiterKeyValue = new string[] { "{:}" };
            string[] glosaDetail = integrationProcessDetail.glossData.Split(delimiterField, StringSplitOptions.RemoveEmptyEntries);

            //string lineBreak = "";
            string tagLineBreak = "";


            returnData = "<div class='ip_wrapperGloss'>";

            foreach (string reg in glosaDetail)
            {
                string fieldName = "";
                string valueName = "";

                string[] glosaField = reg.Split(delimiterKeyValue, StringSplitOptions.RemoveEmptyEntries);
                // classLineBreak = (countBreakLine == breakLine) ? "class='line'"  :"";
                if (countBreakLine == breakLine)
                {
                    tagLineBreak = "div";
                    countBreakLine = 0;
                }
                else
                {
                    tagLineBreak = "span";
                }


                switch (glosaField.Length)
                {

                    case 1:
                        fieldName = glosaField[0];
                        break;
                    case 2:
                        fieldName = glosaField[0];
                        valueName = glosaField[1];
                        break;

                }


                returnData += "<" + tagLineBreak + "><span class='ip_title'>" + fieldName + ":</span><span class='ip_text'>" + valueName + "</span></" + tagLineBreak + ">";
                countBreakLine++;
            }

            returnData += "</div>";



            integrationProcessDetail.glossData = returnData;

        }


        public static void PreparePrintGroup
            (
                 DBContext db,
                 ref IntegrationProcess integrationProcessLote
            )
        {
            try
            {

                int _id_DocumentType = integrationProcessLote.id_DocumentType;


                DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.id == _id_DocumentType);
                tbsysIntegrationDocumentConfig integrationParameters = db.tbsysIntegrationDocumentConfig
                                                                                  .FirstOrDefault(r => r.codeDocumentType == documentType.code);


                if (!integrationParameters.isGroupValue)
                {
                    return;
                }


                IIntegrationProcessActionOutput _actionOutput = IntegrationProcessDefinition(db, _id_DocumentType);


                IntegrationState StateDelete = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted);
                if (StateDelete == null)
                {

                    throw new Exception("Estado Eliminado, Proceso de Integración no definido.");
                }


                integrationProcessLote
                        .IntegrationProcessPrintGroup = _actionOutput
                                                            .PrintGroup(db, integrationProcessLote
                                                                                .IntegrationProcessDetail
                                                                                .Where(r => r.id_StatusDocument != StateDelete.id)
                                                                                .ToList());



            }
            catch (Exception e)
            {
                throw e;
            }


        }


        public static void GetXGlosaForView
            (
                                ref IntegrationProcess integrationProcessLote,
                                int id_deletedState,
                                int breakLine
            )
        {


            //IntegrationState deletedState = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted);
            //if (deletedState == null) throw new Exception("Estado Eliminado para proceso de Integracion No Definido");

            integrationProcessLote
                .IntegrationProcessDetail
                .Where(r => r.id_StatusDocument != id_deletedState)
                .ToList()
                .ForEach(c =>
                {
                    GetXGlosaDetail(ref c, breakLine);
                });

        }


        public static string GetXGlosaForSave
            (
                                DBContext db,
                                int id_document,
                                string code_documentType,
                                Func<DBContext, int, string, Dictionary<string, string>> getglosa
            )
        {

            string returnData = "";
            Dictionary<string, string> glosa = getglosa(db, id_document, code_documentType);


            foreach (KeyValuePair<string, string> reg in glosa)
            {
                returnData += reg.Key + "{:}" + reg.Value + "{|}";

            }


            return returnData;

        }


        public static decimal GetTotalValue
            (
             Document document,
             Func<Document, decimal> getTotalValue
            )
        {


            //Type typeOfIntegrationOuput = Type.GetType("DXPANACEASOFT.Auxiliares.IntegrationProcessService.IntegrationProcessActionOutputAdvanceProviderShrimp", true);
            // IIntegrationProcessActionOutput ActionOutput = null;
            //IIntegrationProcessActionOutput ActionOutput = (IIntegrationProcessActionOutput)Activator.CreateInstance(typeOfIntegrationOuput);

            decimal totalValue = getTotalValue(document);
            return totalValue;



        }
        public static String ProcesingQueue
                              (
                                  DBContext db,
                                  int id_ActiveUser,
                                  ref IntegrationProcess integrationProcessLote
                              )
        {

            //int ierr = 0;
            string msgGeneral = "";
            //string msgErrorProcesados = "";
            //string msgErrorOutState = "";
            List<int> ids_IntegrationProcessDetail = new List<int>();
            tbsysIntegrationDocumentConfig _integrationDocumentConfig = new tbsysIntegrationDocumentConfig();
            List<AdvanceParametersDetail> typesMessageError = new List<AdvanceParametersDetail>();


            // Obtener el tipo de documento
            // Obtener el estado en el que debe estar el documento para procesarse
            // Validar que cada documento del lote no haya sido procesado antes
            // Validar que el documento este en el estado requerido para procesarse
            // Si un documento no cumple las validaciones generales de Document se almacena en string que retorna
            //       a la pantalla y se almacena en IntegrationProcessMessage      
            // Obtener el Tipo de Documento

            try
            {

                DocumentType documentType = integrationProcessLote.DocumentType;



                typesMessageError = db.AdvanceParametersDetail
                                            .Where(r =>
                                                    r.AdvanceParameters.code.Equals(EnumIntegrationProcess.ParametersIntegration.SourceMessage)
                                                    ||
                                                     r.AdvanceParameters.code.Equals(EnumIntegrationProcess.ParametersIntegration.SourceReference)
                                                    ).ToList();




                IntegrationState stateDelete = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted);
                if (stateDelete == null)
                {
                    throw new Exception("Estado de Integración Eliminado, No Definido.");
                }


                // Validaciones Generales del Document
                List<IntegrationProcessDetail> integrationProcessDetailList = integrationProcessLote
                                                                                   .IntegrationProcessDetail
                                                                                   .Where(r => r.IntegrationState.code != EnumIntegrationProcess.States.Deleted)
                                                                                   .ToList();
                //se agrega la secuencia de la proforma en caso que una Factura Fiscal nazca de ella
                if (documentType.id == 7)
                {

                    var query = (from docF in integrationProcessDetailList
                                 where docF.Document.DocumentType.id == 7
                                 select new { docF.Document.id_documentOrigen, docF.id, docF.id_Document})
                                 .Join(db.Document.Where(docP => docP.id_documentType == 1147),
                                       docF => docF.id_documentOrigen,
                                       docP => docP.id,
                                       (docF, docP) => new { docF, docP })
                                 .Select(x => new
                                 {
                                     x.docF.id_Document,
                                     x.docF.id_documentOrigen,
                                     x.docP.number
                                })
                                 .ToList();


                    foreach (var docs in query )
                    {
                        var integrationTem = integrationProcessDetailList.Where(x => x.id_Document == docs.id_Document).FirstOrDefault();
                        integrationTem.relatedDocument = docs?.number;
                    }


                }
 


                ///Type typeOfIntegrationOuput = Type.GetType(_integrationDocumentConfig.implementingIntegration , true);
                IIntegrationProcessActionOutput ActionOutput = IntegrationProcessDefinition(db, documentType.id);//  (IIntegrationProcessActionOutput)Activator.CreateInstance(typeOfIntegrationOuput);



                int countOutput = integrationProcessLote
                                        .IntegrationProcessOutput
                                        .Where(r => r.idStatusOutput != stateDelete.id)?.Count() ?? 0;

                if (countOutput > 0)
                {
                    integrationProcessLote
                            .IntegrationProcessOutput
                            .ToList()
                            .ForEach(r =>
                            {

                                r.idStatusOutput = stateDelete.id;
                                r.userUpdate = id_ActiveUser;
                                r.dateUpdate = DateTime.Now;

                            });
                }

                // Validaciones Particulares al tipo de documento
                msgGeneral = ActionOutput
                                .Validate(db,
                                            id_ActiveUser,
                                            documentType,
                                            integrationProcessLote,
                                            integrationProcessDetailList,
                                            typesMessageError,
                                            SaveMessageError
                                            );

                if (!string.IsNullOrEmpty(msgGeneral))
                {
                    throw new Exception(msgGeneral);
                }

                // Proceso Group By DocumentType Parameters
                msgGeneral = ActionOutput
                                .SaveOutput(db,
                                                id_ActiveUser,
                                                documentType,
                                                ref integrationProcessLote,
                                                integrationProcessDetailList,
                                                typesMessageError,
                                                SaveMessageError
                                            );

                if (!string.IsNullOrEmpty(msgGeneral))
                {
                    throw new Exception(msgGeneral);
                }


            }

            catch (Exception e)
            {
                msgGeneral = e.Message;
            }

            return msgGeneral;

        }


        public static string TransferData
                             (
                                 DBContext db,
                                 int id_ActiveUser,
                                 List<IntegrationProcessOutput> _integrationProcessOutputs,
                                 int id_IntegrationLote,
                                 int id_DocumentType,
                                 Func<DBContext, int, int, List<IntegrationProcessOutput>, Func<DBContext, int, int, string, string, string, List<AdvanceParametersDetail>, int>, string> transferData
                             )
        {

            string msgGeneral = "";

            // Validaciones Particulares al tipo de documento
            msgGeneral = transferData
                            (
                                db,
                                id_ActiveUser,
                                id_IntegrationLote,
                                _integrationProcessOutputs,
                                ServiceIntegrationProcess.SaveMessageError
                            );


            return msgGeneral;

        }



        public static void GetLogView(ref IntegrationProcess integrationProcessLote)
        {

            List<IntegrationProcessLogView> _integrationProcessLogViewList = new List<IntegrationProcessLogView>();
            IntegrationProcessLogView _integrationProcessLogView = new IntegrationProcessLogView();


            foreach (IntegrationProcessLog _integrationProcessLog in integrationProcessLote.IntegrationProcessLog.ToList())
            {
                _integrationProcessLogView = new IntegrationProcessLogView();
                _integrationProcessLogView.FechaHora = _integrationProcessLog.dateAction;
                _integrationProcessLogView.Accion = _integrationProcessLog.IntegrationState.code;
                _integrationProcessLogView.Description = "Se ha " + _integrationProcessLog.IntegrationState.description + " el Lote: " + integrationProcessLote.codeLote;
                _integrationProcessLogViewList.Add(_integrationProcessLogView);

            }

            foreach (IntegrationProcessDetail integrationProcessDetail in integrationProcessLote.IntegrationProcessDetail.ToList())
            {
                integrationProcessDetail.IntegrationProcessDetailLog.ToList().ForEach(r =>
                {
                    _integrationProcessLogView = new IntegrationProcessLogView();
                    _integrationProcessLogView.FechaHora = r.dateAction;
                    _integrationProcessLogView.Accion = r.IntegrationState.code;
                    _integrationProcessLogView.Description = "Se ha " + r.IntegrationState.description + " el Documento: " + integrationProcessDetail.Document.number;
                    _integrationProcessLogViewList.Add(_integrationProcessLogView);

                });
            }


            integrationProcessLote.IntegrationProcessLogViewList = _integrationProcessLogViewList.OrderBy(r => r.FechaHora).ToList();

        }

        public static Dictionary<string, Boolean> GetRequeridDate(DBContext db)
        {

            db = new DBContext();
            Dictionary<string, Boolean> requieridDateDic = new Dictionary<string, bool>();




            List<tbsysIntegrationDocumentConfig> _integrationDocumentConfig =
                                                                            db.tbsysIntegrationDocumentConfig
                                                                                    .Where(r => r.isActive)
                                                                                    .ToList();

            string[] infoIntegrationProcessCodes =
                                                    db.AdvanceParameters
                                                            .FirstOrDefault(r => r.code == "INTD")
                                                            .AdvanceParametersDetail
                                                            .Select(r => r.valueCode.Trim())
                                                            .ToArray();



            foreach (tbsysIntegrationDocumentConfig config in _integrationDocumentConfig)
            {

                int countRecord = infoIntegrationProcessCodes.Count(r => r == config.code);
                Boolean isRequered = (countRecord == 0) ? false : true;

                requieridDateDic.Add(config.code, isRequered);

            }
            //var resultList = (from integrationConfig in _integrationDocumentConfig
            //				 join integrationParameter in infoIntegrationProcess on
            //				 integrationConfig.code equals integrationParameter.code into g
            //				 from result in g.DefaultIfEmpty(new { isRequered =false} )
            //				 select new
            //						{
            //						 integrationConfig.code,
            //						 isRequered = result?.isRequered ?? false
            //						}).ToList();
            //person.FirstName, PetName = subpet?.Name ?? String.Empty 
            //requieridDateDic = resultList.ToDictionary(r => r.code, r => r.valueCode);

            //			(from l1 in myFirstDataSet
            //			 join l2 in mySecondDataSet on l1.< join_val > equals l2.< join_val > into leftJ
            //			 from lj in leftJ.DefaultIfEmpty()
            //			 where < your_where_clause >
            //select < something >).ToList();

            /*  _documentType.code equals _documentTypeIntegration.codeDocumentType
			  select new
			  {
				  _documentType.id,
				  _documentType.code,
				  _documentTypeIntegration.description
			  };*/
            /*

			 from f in Foo
				join b in Bar on f.Foo_Id equals b.Foo_Id into g
				from result in g.DefaultIfEmpty()
				select new { Foo = f, Bar = result }
			 */

            //         if (infoIntegrationProcess == null)
            //{

            //	throw new Exception("Proceso de Integracion: No se ha definido grupo de parámetros para fecha predeterminada");
            //}

            //int countParametersDate = db.AdvanceParametersDetail
            //										 .Count(r => r.valueCode == _integrationDocumentConfig.code);

            //if (countParametersDate > 0)
            //{
            //	isFormDateSource = true;
            //}

            //integrationProcessLote.isRequeridDate = isFormDateSource;
            return requieridDateDic;


        }

        public static Boolean GetRequeridDate(DBContext db, int idDocumentType)
        {

            db = new DBContext();
            Boolean isRequieridDate = false;


            string codeDocumentType = db.DocumentType.FirstOrDefault(r => r.id == idDocumentType)?.code ?? null;
            if (codeDocumentType == null) return false;


            List<tbsysIntegrationDocumentConfig> _integrationDocumentConfig =
                                                                        db.tbsysIntegrationDocumentConfig
                                                                                .Where(r => r.isActive)
                                                                                .ToList();

            string[] infoIntegrationProcessCodes =
                                                    db.AdvanceParameters
                                                            .FirstOrDefault(r => r.code == "INTD")
                                                            .AdvanceParametersDetail
                                                            .Select(r => r.valueCode)
                                                            .ToArray();



            int existConfig = infoIntegrationProcessCodes.Count(r => r == codeDocumentType);
            if (existConfig > 0)
            {
                isRequieridDate = true;
            }


            return isRequieridDate;


        }

    }
}