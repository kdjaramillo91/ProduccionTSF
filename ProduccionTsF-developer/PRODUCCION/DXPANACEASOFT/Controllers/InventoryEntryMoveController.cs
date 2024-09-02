using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.Services;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryEntryMoveController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            var model = new InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("03")) // Ingreso
                }
            };

            return PartialView("Index", model);
        }

        #region INVENTORY Entry MOVE EDIT FORM

        [HttpPost]
        public ActionResult InventoryEntryMoveEditFormPartial(int id)
        {
            var inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id);

            if (inventoryMove == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals("03"));// Ingreso
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code.Equals("01")); //PENDIENTE

                 inventoryMove = new InventoryMove
                {
                    Document = new Document
                    {
                        id_documentType = documentType?.id ?? 0,
                        DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    }
                };

            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveEntryEditFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult InventoryMoveCopy(int id)
        {
            var inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            InventoryMove copyInventoryMove = null;
            if(inventoryMove.id != 0)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals("03"));// Ingreso
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code.Equals("01")); //PENDIENTE

                copyInventoryMove = new InventoryMove
                {
                    Document = new Document
                    {
                        id_documentType = documentType?.id ?? 0,
                        DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    }
                };

                    copyInventoryMove.InventoryEntryMove = new InventoryEntryMove
                    {
                        id_warehouseEntry = inventoryMove.InventoryEntryMove.id_warehouseEntry,
                        id_warehouseLocationEntry = inventoryMove.InventoryEntryMove.id_warehouseLocationEntry,
                        id_receiver = inventoryMove.InventoryEntryMove.id_receiver,
                        dateEntry = DateTime.Now
                    };

                foreach (var detail in inventoryMove.InventoryMoveDetail)
                {
                    var tempDetail = new InventoryMoveDetail
                    {
                        id_item = detail.id_item,
                        Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item),
                        id_lot = detail.id_lot,
                        Lot = db.Lot.FirstOrDefault(fod => fod.id == detail.id_lot),
                        entryAmount = detail.entryAmount,
                        entryAmountCost = detail.entryAmountCost,
                        exitAmount = 0,//detail.exitAmount,
                        exitAmountCost = 0,//detail.exitAmountCost
                        id_warehouse = detail.id_warehouse,
                        Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == detail.id_warehouse),
                        id_warehouseLocation = detail.id_warehouseLocation,
                        WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == detail.id_warehouseLocation),
                        id_warehouseEntry = null,
                        id_inventoryMoveDetailExit = null,
                        id_inventoryMoveDetailPrevious = null,
                        id_inventoryMoveDetailNext = null,
                        unitPrice = detail.unitPrice,
                        balance = 0,
                        averagePrice = 0,
                        balanceCost = 0,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                    };
                    
                    copyInventoryMove.InventoryMoveDetail.Add(tempDetail);
                }
            }

            TempData["inventoryMove"] = copyInventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryEntryMoveEditFormPartial", copyInventoryMove);
        }

        #endregion

        #region INVENTORY MOVE FILTER RESULTS

        [HttpPost]
        public ActionResult InventoryResults(InventoryMove inventoryMove,
                                             InventoryEntryMove entryMove,
                                             Document document,
                                             DateTime? startEmissionDate, DateTime? endEmissionDate,
                                             int[] items)
        {
            var model = db.InventoryMove.Where(w=> (w.InventoryReason == null ? "" : w.InventoryReason.code) == "ING").ToList();

            //if (document.id_documentType != 0)
            //{
            //    model = model.Where(o => o.Document.DocumentType.id == document.id_documentType).ToList();
            //}
            

            #region DOCUMENT FILTERS

            if (document.id_documentState != 0)
            {
                model = model.Where(o => o.Document.id_documentState == document.id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(document.number))
            {
                model = model.Where(o => o.Document.number.Contains(document.number)).ToList();
            }

            if (!string.IsNullOrEmpty(document.reference))
            {
                model = model.Where(o => o.Document.reference.Contains(document.reference)).ToList();
            }

            if (startEmissionDate != null && endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            #endregion

            # region InventoryEntryMove
            //if (entryMove.id_warehouseEntry != 0)
            //{
            //    model = model.Where(o => o.InventoryEntryMove?.id_warehouseEntry == entryMove.id_warehouseEntry).ToList();
            //}

            //if (entryMove.id_warehouseLocationEntry != 0)
            //{
            //    model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseLocationEntry == entryMove.id_warehouseLocationEntry).ToList();
            //}

            if (entryMove.id_receiver != 0)
            {
                model = model.Where(o => o.InventoryEntryMove?.id_receiver == entryMove.id_receiver).ToList();
            }
            #endregion

            # region InventoryMoveDetail
            //if (entryMove.id_warehouseEntry != 0)
            //{
            //    model = model.Where(o => o.InventoryEntryMove?.id_warehouseEntry == entryMove.id_warehouseEntry).ToList();
            //}

            //if (entryMove.id_warehouseLocationEntry != 0)
            //{
            //    model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseLocationEntry == entryMove.id_warehouseLocationEntry).ToList();
            //}
            if (entryMove.id_warehouseEntry != 0)
            {
                var tempModel = new List<InventoryMove>();
                foreach (var im in model)
                {
                    if (im.InventoryMoveDetail.Any(a=> a.id_warehouse == entryMove.id_warehouseEntry))
                    {
                        tempModel.Add(im);
                    }
                }

                model = tempModel;
            }

            if (entryMove.id_warehouseLocationEntry != 0)
            {
                var tempModel = new List<InventoryMove>();
                foreach (var im in model)
                {
                    if (im.InventoryMoveDetail.Any(a => a.id_warehouseLocation == entryMove.id_warehouseLocationEntry))
                    {
                        tempModel.Add(im);
                    }
                }

                model = tempModel;

                if (items != null && items.Length > 0)
                {
                    tempModel = new List<InventoryMove>();
                    foreach (var inv in model)
                    {
                        foreach (var invd in inv.InventoryMoveDetail)
                        {
                            if (items.Contains(invd.id_item))
                            {
                                tempModel.Add(inv);
                                break;
                            }
                        }
                    }

                    model = tempModel;
                }
            }
            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_InventoryEntryResults", model.OrderByDescending(i => i.id).ToList());
        }

        #endregion

        #region INVENTORY MOVE HEADER

        [ValidateInput(false)]
        public ActionResult InventoryEntryMovesPartial()
        {
            var model = TempData["model"] as List<InventoryMove>;
            model = model ?? new List<InventoryMove>();

            TempData.Keep("model");

            return PartialView("_InventoryEntryMovesPartial", model.OrderByDescending(i => i.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryEntryMovesPartialAddNew(bool approve, InventoryMove item, Document document, InventoryEntryMove entryMove)
        {

            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {

                    #region Document

                    document.id_userCreate = ActiveUser.id;
                    document.dateCreate = DateTime.Now;
                    document.id_userUpdate = ActiveUser.id;
                    document.dateUpdate = DateTime.Now;
                    document.sequential = GetDocumentSequential(document.id_documentType);
                    document.number = GetDocumentNumber(document.id_documentType);

                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == document.id_documentType);//Debe ser 03 el de Ingreso
                    document.DocumentType = documentType;

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == document.id_documentState);//Debe ser 01 el Pendiente
                    document.DocumentState = documentState;
                        
                    document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
                    document.id_emissionPoint = ActiveEmissionPoint.id;

                    //Actualiza Secuencial
                    if (documentType != null)
                    {
                        documentType.currentNumber = documentType.currentNumber + 1;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }

                    item.Document = document;

                    #endregion

                    #region InventoryMove

                    item.InventoryReason = db.InventoryReason.FirstOrDefault(e => e.id == item.id_inventoryReason);
                    item.id_productionLot = null;
                    item.ProductionLot = null;
                    item.id_inventoryMoveToReverse = null;
                    item.InventoryMove2 = null;

                    //if (item.Document.DocumentType.code.Equals("03") || item.Document.DocumentType.code.Equals("04"))
                    //{
                    item.InventoryEntryMove = new InventoryEntryMove
                    {
                        id_warehouseEntry = entryMove.id_warehouseEntry == 0 ? null : entryMove.id_warehouseEntry,
                        id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry == 0 ? null : entryMove.id_warehouseLocationEntry,
                        id_receiver = entryMove.id_receiver,
                        dateEntry = DateTime.Now

                    };
                    //}
                    //else if (item.Document.DocumentType.code.Equals("05"))
                    //{
                    //    item.InventoryExitMove = new InventoryExitMove
                    //    {
                    //        id_warehouseExit = exitMove.id_warehouseExit,
                    //        id_warehouseLocationExit = exitMove.id_warehouseLocationExit,
                    //        id_dispatcher = exitMove.id_dispatcher,
                    //        dateExit = DateTime.Now
                    //    };
                    //}
                    //else if (item.Document.DocumentType.code.Equals("06"))
                    //{
                    //    exitMove.dateExit = DateTime.Now;
                    //    entryMove.dateEntry = DateTime.Now;

                    //    item.InventoryExitMove = new InventoryExitMove
                    //    {
                    //        id_warehouseExit = exitMove.id_warehouseExit,
                    //        id_warehouseLocationExit = exitMove.id_warehouseLocationExit,
                    //        id_dispatcher = exitMove.id_dispatcher,
                    //        dateExit = DateTime.Now
                    //    };

                    //    item.InventoryEntryMove = new InventoryEntryMove
                    //    {
                    //        id_warehouseEntry = entryMove.id_warehouseEntry,
                    //        id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry,
                    //        id_receiver = entryMove.id_receiver,
                    //        dateEntry = DateTime.Now

                    //    };

                    //    item.InventoryTransferMove = new InventoryTransferMove
                    //    {
                    //        InventoryExitMove = item.InventoryExitMove,
                    //        InventoryEntryMove = item.InventoryEntryMove
                    //    };
                    //}

                    #endregion

                    #region Details
                    item.InventoryMoveDetail = new List<InventoryMoveDetail>();
                    if (tempInventoryMove?.InventoryMoveDetail != null)
                    {

                        var itemDetail = tempInventoryMove.InventoryMoveDetail.ToList();

                        foreach (var i in itemDetail)
                        {
                            //List<InventoryMoveDetail> lastsMoveDetails =
                            //    db.InventoryMoveDetail.Where(d =>
                            //                                 d.id_productionLote == i.id_productionLote &&
                            //                                 d.id_item == i.id_item &&
                            //                                 d.id_warehouse == i.id_warehouse &&
                            //                                 d.id_warehouseLocation == i.id_warehouseLocation).ToList();

                            //lastsMoveDetails = lastsMoveDetails.OrderByDescending(d => d.dateUpdate).ToList();

                            //InventoryMoveDetail lastInventoryMove = (lastsMoveDetails.Count > 0)
                            //                                        ? lastsMoveDetails.First()
                            //                                        : null;

                            var tempDetail = new InventoryMoveDetail
                            {
                                id_item = i.id_item,
                                entryAmount = i.entryAmount,
                                exitAmount = i.exitAmount,
                                id_warehouse = i.id_warehouse,
                                id_warehouseLocation = i.id_warehouseLocation,
                                InventoryMoveDetailPurchaseOrder = i.InventoryMoveDetailPurchaseOrder,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now,
                                id_userUpdate = ActiveUser.id,
                                dateUpdate = DateTime.Now,
                                //balance = (lastInventoryMove?.balance ?? 0) + (i.entryAmount - i.exitAmount)
                            };

                            db.InventoryMoveDetail.Add(tempDetail);
                            item.InventoryMoveDetail.Add(tempDetail);
                        }
                    }
                    #endregion

                    if (item.InventoryMoveDetail.Count == 0)
                    {
                        TempData.Keep("inventoryMove");
                        ViewData["EditMessage"] = ErrorMessage("No se puede guardar un movimiento de inventario sin detalles");
                        return PartialView("_InventoryMoveMainFormPartial", tempInventoryMove);
                    }

                    ServiceInventoryMove.Save(item, db);

                    //model.Add(item);
                    //db.SaveChanges();
                    trans.Commit();

                    TempData["inventoryMove"] = item;
                    TempData.Keep("inventoryMove");

                    ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + item.Document.number + " guardado exitosamente");
                }
                catch (Exception)
                {
                    TempData.Keep("inventoryMove");
                    ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                }
            }
            
            
            return PartialView("_InventoryMoveMainFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialUpdate(bool approve, int id_inventoryMove, InventoryMove item, Document document, InventoryEntryMove entryMove, InventoryExitMove exitMove)
        {
            var model = db.InventoryMove;

            var modelItem = new InventoryMove();
            //if (ModelState.IsValid)
            //{
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        modelItem = model.FirstOrDefault(it => it.id == id_inventoryMove);
                        if (modelItem != null)
                        {
                            #region DOCUMENT

                            modelItem.Document.description = document.description;
                            modelItem.Document.id_userUpdate = ActiveUser.id;
                            modelItem.Document.dateUpdate = DateTime.Now;

                            #endregion

                            #region INVENTORYMOVE

                            if (modelItem.Document.DocumentType.code.Equals("03") || modelItem.Document.DocumentType.code.Equals("04"))
                            {
                                modelItem.InventoryEntryMove.id_warehouseEntry = entryMove.id_warehouseEntry;
                                modelItem.InventoryEntryMove.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
                                modelItem.InventoryEntryMove.id_receiver = entryMove.id_receiver;
                                modelItem.InventoryEntryMove.dateEntry = DateTime.Now;
                            }
                            else if (modelItem.Document.DocumentType.code.Equals("05"))
                            {
                                modelItem.InventoryExitMove.id_warehouseExit = exitMove.id_warehouseExit;
                                modelItem.InventoryExitMove.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
                                modelItem.InventoryExitMove.id_dispatcher = exitMove.id_dispatcher;
                                modelItem.InventoryExitMove.dateExit = DateTime.Now;
                            }
                            else if (modelItem.Document.DocumentType.code.Equals("06"))
                            {
                                exitMove.dateExit = DateTime.Now;
                                entryMove.dateEntry = DateTime.Now;

                                modelItem.InventoryExitMove.id_warehouseExit = exitMove.id_warehouseExit;
                                modelItem.InventoryExitMove.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
                                modelItem.InventoryExitMove.id_dispatcher = exitMove.id_dispatcher;
                                modelItem.InventoryExitMove.dateExit = DateTime.Now;

                                modelItem.InventoryEntryMove.id_warehouseEntry = entryMove.id_warehouseEntry;
                                modelItem.InventoryEntryMove.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
                                modelItem.InventoryEntryMove.id_receiver = entryMove.id_receiver;
                                modelItem.InventoryEntryMove.dateEntry = DateTime.Now;

                                //item.InventoryTransferMove = new InventoryTransferMove
                                //{
                                //    InventoryExitMove = item.InventoryExitMove,
                                //    InventoryEntryMove = item.InventoryEntryMove
                                //};
                            }

                            #endregion

                            #region DETAILS

                            var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_inventoryMove == id_inventoryMove);

                            foreach (var detail in inventoryMoveDetails)
                            {
                                db.InventoryMoveDetail.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);

                            if (tempInventoryMove?.InventoryMoveDetail != null)
                            {
                                var itemDetail = tempInventoryMove.InventoryMoveDetail.ToList();

                                foreach (var i in itemDetail)
                                {
                                    //List<InventoryMoveDetail> lastsMoveDetails =
                                    //                        db.InventoryMoveDetail.Where(d =>
                                    //                                                     d.id_productionLote == i.id_productionLote &&
                                    //                                                     d.id_item == i.id_item &&
                                    //                                                     d.id_warehouse == i.id_warehouse &&
                                    //                                                     d.id_warehouseLocation == i.id_warehouseLocation).ToList();

                                    //lastsMoveDetails = lastsMoveDetails.OrderByDescending(d => d.dateUpdate).ToList();

                                    //InventoryMoveDetail lastInventoryMove = (lastsMoveDetails.Count > 0)
                                    //                                        ? inventoryMoveDetails.First()
                                    //                                        : null;

                                    var tempDetail = new InventoryMoveDetail
                                    {
                                        id_item = i.id_item,
                                        entryAmount = i.entryAmount,
                                        exitAmount = i.exitAmount,
                                        id_warehouse = i.id_warehouse,
                                        id_warehouseLocation = i.id_warehouseLocation,
                                        InventoryMoveDetailPurchaseOrder = i.InventoryMoveDetailPurchaseOrder,
                                        id_userCreate = ActiveUser.id,
                                        dateCreate = DateTime.Now,
                                        id_userUpdate = ActiveUser.id,
                                        dateUpdate = DateTime.Now,
                                        //balance = (lastInventoryMove?.balance ?? 0) + (i.entryAmount - i.exitAmount)
                                    };

                                    modelItem.InventoryMoveDetail.Add(tempDetail);
                                }
                            }

                            #endregion

                            if (modelItem.InventoryMoveDetail.Count == 0)
                            {
                                TempData.Keep("inventoryMove");
                                ViewData["EditMessage"] = ErrorMessage("No se puede guardar un movimiento de inventario sin detalles");
                                return PartialView("_InventoryMoveMainFormPartial", tempInventoryMove);
                            }

                            db.InventoryMove.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            TempData["inventoryMove"] = modelItem;
                        }

                    }
                    catch (Exception)
                    {
                        TempData.Keep("inventoryMove");
                        ViewData["EditError"] = ErrorMessage();
                        trans.Rollback();
                        //ViewData["EditError"] = e.Message;
                        //trans.Rollback();
                    }
                }
            //}
            //else
            //    ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveEditFormPartial", modelItem);
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialDelete(System.Int32 id)
        {
            var model = db.InventoryMove;
            if (id >= 0){
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                    {
                        //var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_inventoryMove == id_inventoryMove);

                        foreach (var detail in item.InventoryMoveDetail)
                        {
                            db.InventoryMoveDetail.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }
                        
                        model.Remove(item);
                    }
                        
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_InventoryMovesPartial", model.ToList());
        }

        #endregion

        #region DETAILS

        [ValidateInput(false)]
        public ActionResult InventoryMoveDetails(int id_inventoryMove)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id_inventoryMove);
            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            ViewData["id_inventoryMove"] = id_inventoryMove;
            ViewData["code"] = inventoryMove.Document.DocumentType.code;

            return PartialView("_InventoryMoveDetailsPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartial()
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            TempData["inventoryMove"] = TempData["inventoryMove"] ?? inventoryMove;
            TempData.Keep("inventoryMove");

            ViewData["id_inventoryMove"] = inventoryMove?.id ?? 0;
            ViewData["code"] = inventoryMove?.Document?.DocumentType?.code ?? "";

            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialAddNew(DXPANACEASOFT.Models.InventoryMoveDetail item)
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            if (ModelState.IsValid)
            {
                try
                {

                    item.id_userCreate = ActiveUser.id;
                    item.dateCreate = DateTime.Now;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;

                    inventoryMove.InventoryMoveDetail.Add(item);
                    TempData["inventoryMove"] = inventoryMove;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("inventoryMove");

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialUpdate(DXPANACEASOFT.Models.InventoryMoveDetail item)
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = inventoryMove.InventoryMoveDetail.FirstOrDefault(it => it.id_inventoryMove == inventoryMove.id && it.id_item == item.id_item);
                    if (modelItem != null)
                    {

                        modelItem.entryAmount = item.entryAmount;
                        modelItem.exitAmount = item.exitAmount;

                        modelItem.id_warehouse = item.id_warehouse;
                        modelItem.id_warehouseLocation = item.id_warehouseLocation;

                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["inventoryMove"] = inventoryMove;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("inventoryMove");

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialDelete(System.Int32 id_item)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            if (id_item >= 0)
            {
                try
                {
                    var item = inventoryMove.InventoryMoveDetail.FirstOrDefault(it => it.id_item == id_item);
                    if (item != null)
                        inventoryMove.InventoryMoveDetail.Remove(item);
                    
                    TempData["inventoryMove"] = inventoryMove;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("inventoryMove");

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();
            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        #endregion
        
        #region PURCHASE ORDERS

        [HttpPost]
        public ActionResult PurchaseOrdersResult()
        {   
            return PartialView("_PurchaseOrdersResult");
        }

        [ValidateInput(false)]
        public ActionResult PurchaseOrdersDetailsPartial()
        {
            var model = db.PurchaseOrderDetail.Where(o => (o.PurchaseOrder.Document.id_documentState == 6 || o.PurchaseOrder.Document.id_documentState == 3)
                                                       && (o.quantityApproved - o.quantityReceived > 0)).OrderByDescending(o => o.id);
            return PartialView("_PurchaseOrdersDetailsPartial", model.ToList());
        }

        #endregion

        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                    if (inventoryMove != null && documentState != null)
                    {
                        inventoryMove.Document.id_documentState = documentState.id;
                        inventoryMove.Document.DocumentState = documentState;

                        ServiceInventoryMove.Commit(inventoryMove, db);
                        
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                    if (inventoryMove != null && documentState != null)
                    {
                        DocumentState currentState = inventoryMove.Document.DocumentState;

                        inventoryMove.Document.id_documentState = documentState.id;
                        inventoryMove.Document.DocumentState = documentState;

                        if(currentState.id != 3)
                        {
                            ServiceInventoryMove.Commit(inventoryMove, db);
                        }
                        else
                        {
                            db.InventoryMove.Attach(inventoryMove);
                            db.Entry(inventoryMove).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                    if (inventoryMove != null && documentState != null)
                    {
                        inventoryMove.Document.id_documentState = documentState.id;
                        inventoryMove.Document.DocumentState = documentState;

                        db.InventoryMove.Attach(inventoryMove);
                        db.Entry(inventoryMove).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            //using (DbContextTransaction trans = db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

            //        if (purchaseOrder != null && documentState != null)
            //        {
            //            purchaseOrder.Document.id_documentState = documentState.id;
            //            purchaseOrder.Document.DocumentState = documentState;

            //            db.PurchaseOrder.Attach(purchaseOrder);
            //            db.Entry(purchaseOrder).State = EntityState.Modified;

            //            db.SaveChanges();
            //            trans.Commit();
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        ViewData["EditError"] = e.Message;
            //        trans.Rollback();
            //    }
            //}

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            //using (DbContextTransaction trans = db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

            //        if (purchaseOrder != null && documentState != null)
            //        {
            //            purchaseOrder.Document.id_documentState = documentState.id;
            //            purchaseOrder.Document.DocumentState = documentState;

            //            foreach (var details in purchaseOrder.PurchaseOrderDetail)
            //            {
            //                details.quantityApproved = 0.0M;

            //                db.PurchaseOrderDetail.Attach(details);
            //                db.Entry(details).State = EntityState.Modified;
            //            }

            //            db.PurchaseOrder.Attach(purchaseOrder);
            //            db.Entry(purchaseOrder).State = EntityState.Modified;

            //            db.SaveChanges();
            //            trans.Commit();
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        ViewData["EditError"] = e.Message;
            //        trans.Rollback();
            //    }
            //}

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        #endregion

        #region SELECTED DOCUMENT STATE CHANGE

        [HttpPost, ValidateInput(false)]
        public void ApproveDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                            if (inventoryMove != null && documentState != null)
                            {
                                ServiceInventoryMove.Commit(inventoryMove, db);

                                inventoryMove.Document.id_documentState = documentState.id;
                                inventoryMove.Document.DocumentState = documentState;
                            }

                            db.InventoryMove.Attach(inventoryMove);
                            db.Entry(inventoryMove).State = EntityState.Modified;
                            
                        }

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void AutorizeDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                            if (inventoryMove != null && documentState != null)
                            {
                                DocumentState currentState = inventoryMove.Document.DocumentState;

                                if (currentState.id != 3)
                                {
                                    ServiceInventoryMove.Commit(inventoryMove, db);
                                }

                                inventoryMove.Document.id_documentState = documentState.id;
                                inventoryMove.Document.DocumentState = documentState;
                                
                                db.InventoryMove.Attach(inventoryMove);
                                db.Entry(inventoryMove).State = EntityState.Modified;
                            }
                        }

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void ProtectDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                            if (inventoryMove != null && documentState != null)
                            {
                                inventoryMove.Document.id_documentState = documentState.id;
                                inventoryMove.Document.DocumentState = documentState;

                                db.InventoryMove.Attach(inventoryMove);
                                db.Entry(inventoryMove).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void CancelDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            //if (purchaseOrder != null && documentState != null)
                            //{
                            //    purchaseOrder.Document.id_documentState = documentState.id;
                            //    purchaseOrder.Document.DocumentState = documentState;

                            //    db.PurchaseOrder.Attach(purchaseOrder);
                            //    db.Entry(purchaseOrder).State = EntityState.Modified;
                            //}
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void RevertDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            //if (purchaseOrder != null && documentState != null)
                            //{
                            //    purchaseOrder.Document.id_documentState = documentState.id;
                            //    purchaseOrder.Document.DocumentState = documentState;

                            //    foreach (var details in purchaseOrder.PurchaseOrderDetail)
                            //    {
                            //        details.quantityApproved = 0.0M;

                            //        db.PurchaseOrderDetail.Attach(details);
                            //        db.Entry(details).State = EntityState.Modified;
                            //    }

                            //    db.PurchaseOrder.Attach(purchaseOrder);
                            //    db.Entry(purchaseOrder).State = EntityState.Modified;
                            //}
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_inventoryMove)
        {
            TempData.Keep("inventoryMove");

            int index = db.InventoryMove.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_inventoryMove);

            var result = new
            {
                maximunPages = db.InventoryMove.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            InventoryMove inventoryMove = db.InventoryMove.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (inventoryMove != null)
            {
                TempData["inventoryMove"] = inventoryMove;
                TempData.Keep("inventoryMove");
                return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
            }

            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", new InventoryMove());
        }

        #endregion

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            var actions = new
            {
                btnApprove = false,
                btnAutorize = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
            int state = inventoryMove?.Document.DocumentState.id ?? 0;

            if (state == 1) // PENDIENTE
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = true,
                    btnProtect = true,
                    btnCancel = true,
                    btnRevert = false,
                };
            }
            else if (state == 2 || state == 3) // APROBADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = true,
                    btnProtect = true,
                    btnCancel = true,
                    btnRevert = true,
                };
            }
            else if (state == 4 || state == 5) // CERRADA O ANULADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                };
            }
            else if (state == 6) // AUTORIZADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = true,
                    btnCancel = true,
                    btnRevert = true,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORTS

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveReportList()
        {
            return PartialView("_InventoryMoveListReport");
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult InventoryMoveDetails()
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

            TempData.Keep("inventoryMove");

            return Json(inventoryMove.InventoryMoveDetail.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult WarehouseChangeData(int id_warehouse)
        {
            Warehouse warehouse = db.Warehouse.FirstOrDefault(i => i.id == id_warehouse);

            if (warehouse == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);


            if (inventoryMove?.InventoryMoveDetail != null)
            {
                var itemDetail = inventoryMove.InventoryMoveDetail.ToList();

                foreach (var i in itemDetail)
                {
                    i.id_warehouse = id_warehouse;
                    UpdateModel(i);
                }
            }

            var result = new
            {
               id_warehouse,
               warehouse.name
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult WarehouseLocationChangeData(int id_warehouseLocation)
        {
            WarehouseLocation warehouseLocation = db.WarehouseLocation.FirstOrDefault(i => i.id == id_warehouseLocation);

            if (warehouseLocation == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);


            if (inventoryMove?.InventoryMoveDetail != null)
            {
                var itemDetail = inventoryMove.InventoryMoveDetail.ToList();

                foreach (var i in itemDetail)
                {
                    i.id_warehouseLocation = id_warehouseLocation;
                    UpdateModel(i);
                }
            }

            var result = new
            {
                id_warehouseLocation,
                warehouseLocation.name
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}