using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentLogDTO;
using Utilitarios.Logs;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;

namespace DXPANACEASOFT.Services
{
    public class ServiceDocument
    {
        public static Document CreateDocument(DBContext db, User ActiveUser, Company ActiveCompany, EmissionPoint ActiveEmissionPoint, string code_documentType, string name_documentType)
        {
            string result = "";
            Document document = new Document();
            try
            {
                #region Document

                //Document document = new Document();
                document.id_userCreate = ActiveUser.id;
                document.dateCreate = DateTime.Now;
                document.id_userUpdate = ActiveUser.id;
                document.dateUpdate = DateTime.Now;

                DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == code_documentType);//code_documentType
                if (documentType == null)
                {
                    throw new Exception("No se puedo Crear el Documento porque no existe el Tipo de Documento: " + name_documentType + " con Código: " + code_documentType + ", configúrelo e inténtelo de nuevo");
                }
                document.id_documentType = documentType.id;
                document.DocumentType = documentType;
                document.sequential = GetDocumentSequential(document.id_documentType, db, ActiveCompany);
                document.number = GetDocumentNumber(document.id_documentType, db, ActiveCompany, ActiveEmissionPoint);


                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE
                if (documentState == null)
                {
                    throw new Exception("No se puedo Crear el Documento Lote porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                }
                document.id_documentState = documentState.id;
                document.DocumentState = documentState;

                document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
                document.id_emissionPoint = ActiveEmissionPoint.id;

                document.emissionDate = DateTime.Now;
                document.description = null;

                //Actualiza Secuencial
                if (documentType != null)
                {
                    documentType.currentNumber = documentType.currentNumber + 1;
                    db.DocumentType.Attach(documentType);
                    db.Entry(documentType).State = EntityState.Modified;
                }

                #endregion

            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            return document;
        }

        public static int GenerateDocumentLog(DBContext db, DocumentLogDTO _docLog, string ruta)
        {
            int _answer = 0;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentLog _docLogNew = new DocumentLog();
                    _docLogNew.id_Document = _docLog.id;
                    _docLogNew.description = _docLog.description;
                    _docLogNew.id_user = _docLog.id_User;
                    _docLogNew.id_ActionOnDocument = db.tbsysActionOnDocument
                                                        .FirstOrDefault(fod => fod.code == _docLog.code_Action)?
                                                        .id ?? 0;
                    _docLogNew.dateUser = DateTime.Now;

                    db.DocumentLog.Add(_docLogNew);

                    db.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "Apertura Documentos", "PROD");
                    trans.Rollback();
                }
            }

                

            return _answer;
        }

        public static DocumentLog GenerateDocumentLog(DocumentLogModelP dlm)
        {
            return new DocumentLog
            {
                id_Document = dlm.idDocumentModelP,
                id_ActionOnDocument = dlm.idActionOnDocumentModelP,
                description = dlm.descriptionModelP,
                id_user = dlm.idUserModelP,
                dateUser = dlm.dtDateModelP
            };
        }
        
        #region Auxiliar

        public static int GetDocumentSequential(int id_documentType, DBContext db, Company activeCompany)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id_documentType && d.id_company == activeCompany.id);
            return documentType?.currentNumber ?? 0;
        }

        public static string GetDocumentNumber(int id_documentType, DBContext db, Company ActiveCompany, EmissionPoint ActiveEmissionPoint)
        {
            string number = GetDocumentSequential(id_documentType, db, ActiveCompany).ToString().PadLeft(9, '0');
            string documentNumber = string.Empty;
            documentNumber = $"{ActiveEmissionPoint.BranchOffice.code.ToString().PadLeft(3, '0')}-{ActiveEmissionPoint.code.ToString().PadLeft(3, '0')}-{number}";
            return documentNumber;
        }

        public static void UpdateDocumentSource(Document destinationDocument, Document sourceDocument, DBContext db)
        {
            if (destinationDocument != null && sourceDocument != null)
            {
                if (db.DocumentSource.FirstOrDefault(fod => fod.id_document == destinationDocument.id && fod.id_documentOrigin == sourceDocument.id) == null)
                {
                    db.DocumentSource.Add(new DocumentSource()
                    {
                        id_document = destinationDocument.id,
                        Document = destinationDocument,
                        id_documentOrigin = sourceDocument.id,
                        Document1 = sourceDocument
                    });
                }

            }
        }
        public static void SaveTrackState(Document _docToLog, int idDa, int idUs, DBContext db)
        {
            if (_docToLog != null)
            {
                db.DocumentActionTrack.Add(new DocumentActionTrack()
                {
                    idDocument = _docToLog.id,
                    idActionOnDocument = idDa,
                    description ="EJECUCION MANUAL",
                    idUser= idUs,
                    dateUser = DateTime.Now,
                    Document = _docToLog,
                    tbsysActionOnDocument = db.tbsysActionOnDocument.FirstOrDefault(fod => fod.id == idDa)
                });
            }
        }
        #endregion

    }
}