using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;
using DXPANACEASOFT.Models.FE;
using Utilitarios.Logs;
using DXPANACEASOFT.Auxiliares;
using System.Collections;

namespace DXPANACEASOFT.Models
{
    public partial class InventoryMove
    {
        private DBContext db = new DBContext();

        public string numberReq { get; set; }


        public void FillDocumentSourceInformation()
        {
            var lstDocSource = db.DocumentSource.Where(w => w.id_document == this.id).ToList();

            if (lstDocSource != null)
            {
                // Se obtienen las Guías de Remisión
                var DocSourceRG = lstDocSource.Where(w => w.Document1.DocumentType.code.Equals("08") 
                                                    && !(w.Document1.DocumentState.code.Equals("01") 
                                                    || w.Document1.DocumentState.code.Equals("05")) ).FirstOrDefault();

                if (DocSourceRG != null)
                {
                    numberRemGuide = db.RemissionGuide
                                        .FirstOrDefault(fod => fod.id == DocSourceRG.id_documentOrigin)?
                                        .Document?
                                        .sequential
                                        .ToString() ?? "";

                    numberReq = db.DispatchMaterialSequential
                                    .FirstOrDefault(fod => fod.id_RemissionGuide == DocSourceRG.id_documentOrigin
                                    && fod.id_Warehouse == this.idWarehouse)?.sequential.ToString() ?? "";

                    if (numberReq == "")
                    {
                        var lstDocSourceRg = db
                                                .DocumentSource
                                                .Where(w => w.id_documentOrigin == DocSourceRG.id_documentOrigin).ToList();


                        var lstInvMoveWarehouse = db.InventoryMove
                                                    .Where(w => w.InventoryReason.code.Equals("EPTAMDL") && w.Document.DocumentState.code.Equals("03"))
                                                    .Select(s => new { idInventoryMove = s.id, idWarehouse = s.idWarehouse })
                                                    .ToList();

                        // var lstInvMoveWarehouse = lstInvMove.Select(s => new { idInventoryMove = s.id, idWarehouse = s.idWarehouse }).ToList();

                        var lstInventoryMoveEPTAMDL = (from a in lstDocSourceRg
                                             join b in lstInvMoveWarehouse on a.id_document equals b.idInventoryMove
                                             select new
                                             {
                                                 Id_Warehouse = b.idWarehouse
                                             }).FirstOrDefault();

                        if (lstInventoryMoveEPTAMDL != null)
                        {
                            numberReq = db.DispatchMaterialSequential
                                    .FirstOrDefault(fod => fod.id_RemissionGuide == DocSourceRG.id_documentOrigin
                                    && fod.id_Warehouse == lstInventoryMoveEPTAMDL.Id_Warehouse)?.sequential.ToString() ?? "";
                            
                        }

                    }
                }

            }
        }
    }
}
