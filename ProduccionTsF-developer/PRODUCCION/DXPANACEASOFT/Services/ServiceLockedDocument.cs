using System;
using System.Data.Entity;
using System.Linq;
using DXPANACEASOFT.Models;


namespace DXPANACEASOFT.Services
{
    public class ServiceLockedDocument
    {
        public static string LockedDocument(DBContext db, User ActiveUser, int id_document, string nameDocument, string code_sourceLockedDocument, string namesourceLockedDocument)
        {

            //using (DbContextTransaction trans = db.Database.BeginTransaction())
            //{
                try
                {
                    SourceLockedDocument aSourceLockedDocument = null;
                    LockedDocument aLockedDocument = db.LockedDocument.FirstOrDefault(fod => fod.id_document == id_document);
                    if (aLockedDocument != null)
                    {
                        if (aLockedDocument.id_lockedUser == ActiveUser.id && aLockedDocument.SourceLockedDocument.code == code_sourceLockedDocument)
                        {
                            aLockedDocument.lockedDateTime = DateTime.Now;
                            db.LockedDocument.Attach(aLockedDocument);
                            db.Entry(aLockedDocument).State = EntityState.Modified;
                        }
                        else
                        {
                            int SettingMEDD = 1;
                            try
                            {
                                SettingMEDD = Int32.Parse(db.Setting.FirstOrDefault(fod => fod.code == "MEDD").value);
                            }
                            catch (Exception)
                            {
                                SettingMEDD = 1;
                            }
                            
                            if (DateTime.Now > aLockedDocument.lockedDateTime.AddMinutes(SettingMEDD))
                            {
                                aSourceLockedDocument = GetSourceLockedDocument(db, ActiveUser, code_sourceLockedDocument, namesourceLockedDocument);
                                aLockedDocument.id_lockedUser = ActiveUser.id;
                                aLockedDocument.id_sourceLockedDocument = aSourceLockedDocument.id;
                                aLockedDocument.SourceLockedDocument = aSourceLockedDocument;
                                aLockedDocument.lockedDateTime = DateTime.Now;
                                db.LockedDocument.Attach(aLockedDocument);
                                db.Entry(aLockedDocument).State = EntityState.Modified;
                            }
                            else
                            {
                                return (nameDocument + ", con número: " +
                                       aLockedDocument.Document.number + ". Esta bloqueado por el usuario: " +
                                       aLockedDocument.User.Employee.Person.fullname_businessName + ", desde " +
                                       aLockedDocument.SourceLockedDocument.name);
                            }
                        }
                    }
                    else
                    {
                        aSourceLockedDocument = GetSourceLockedDocument(db, ActiveUser, code_sourceLockedDocument, namesourceLockedDocument);
                        aLockedDocument = new LockedDocument();
                        aLockedDocument.id_document = id_document;
                        aLockedDocument.id_lockedUser = ActiveUser.id;
                        aLockedDocument.id_sourceLockedDocument = aSourceLockedDocument.id;
                        aLockedDocument.SourceLockedDocument = aSourceLockedDocument;
                        aLockedDocument.lockedDateTime = DateTime.Now;
                        db.LockedDocument.Add(aLockedDocument);
                    }

                    //db.SaveChanges();
                    //trans.Commit();
                }
                catch (Exception ex)
                {
                    //trans.Rollback();
                    return (nameDocument + ", con numero: " + db.Document.FirstOrDefault(fod => fod.id == id_document).number + ". No se pudo verificar si esta bloqueada por el sieguiente error: " + ex.Message);
                }
            //}

            return "OK";
        }

        public static string UnlockedDocument(DBContext db, User ActiveUser, int id_document, string nameDocument, string code_sourceLockedDocument)
        {

            //using (DbContextTransaction trans = db.Database.BeginTransaction())
            //{
                try
                {
                    LockedDocument aLockedDocument = db.LockedDocument.FirstOrDefault(fod => fod.id_document == id_document);
                    if (aLockedDocument != null && aLockedDocument.id_lockedUser == ActiveUser.id && aLockedDocument.SourceLockedDocument.code == code_sourceLockedDocument)
                    {
                        db.LockedDocument.Remove(aLockedDocument);
                        db.LockedDocument.Attach(aLockedDocument);
                        db.Entry(aLockedDocument).State = EntityState.Deleted;
                    }

                    
                }
                catch (Exception ex)
                {
                    //trans.Rollback();
                    return (nameDocument + ", con numero: " + db.Document.FirstOrDefault(fod => fod.id == id_document).number + ". No se pudo desbloquear por el sieguiente error: " + ex.Message);
                }
            //}

            return "OK";
        }

        #region Auxiliar

        private static SourceLockedDocument GetSourceLockedDocument(DBContext db, User activeUser, string code_sourceLockedDocument, string namesourceLockedDocument)
        {
            SourceLockedDocument aSourceLockedDocument = db.SourceLockedDocument.FirstOrDefault(fod => fod.code == code_sourceLockedDocument);
            if (aSourceLockedDocument == null)
            {
                aSourceLockedDocument = new SourceLockedDocument();
                aSourceLockedDocument.code = code_sourceLockedDocument;
                aSourceLockedDocument.name = namesourceLockedDocument;
                aSourceLockedDocument.description = namesourceLockedDocument;
                aSourceLockedDocument.id_userCreate = activeUser.id;
                aSourceLockedDocument.dateCreate = DateTime.Now;
                aSourceLockedDocument.id_userUpdate = activeUser.id;
                aSourceLockedDocument.dateUpdate = DateTime.Now;
                db.SourceLockedDocument.Add(aSourceLockedDocument);
            }
            return aSourceLockedDocument;
        }

        #endregion

    }
}