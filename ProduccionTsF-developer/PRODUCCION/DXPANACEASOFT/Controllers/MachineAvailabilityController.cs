using DevExpress.Data.ODataLinq.Helpers;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class MachineAvailabilityController : DefaultController
    {
        private MachineAvailabilityResultConsultDTO GetaMachineAvailabilityResultConsultDTO()
        {
            if (!(Session["aMachineAvailabilityResultConsultDTO"] is MachineAvailabilityResultConsultDTO machineAvailabilityResultConsult))
                machineAvailabilityResultConsult = new MachineAvailabilityResultConsultDTO();
            return machineAvailabilityResultConsult;
        }

        private List<MachineAvailabilityResultConsultDTO> GetMachineAvailabilityResultConsultDTO()
        {
            if (!(Session["MachineAvailabilityResultConsultDTO"] is List<MachineAvailabilityResultConsultDTO> machineAvailabilityResultConsult))
                machineAvailabilityResultConsult = new List<MachineAvailabilityResultConsultDTO>();
            return machineAvailabilityResultConsult;
        }

        private void SetaMachineAvailabilityResultConsultDTO(MachineAvailabilityResultConsultDTO machineAvailabilityResultConsultDTO)
        {
            Session["aMachineAvailabilityResultConsultDTO"] = machineAvailabilityResultConsultDTO;
        }

        private void SetMachineAvailabilityResultConsultDTO(List<MachineAvailabilityResultConsultDTO> machineAvailabilityResultConsult)
        {
            Session["MachineAvailabilityResultConsultDTO"] = machineAvailabilityResultConsult;
        }

        // GET: MachineAvailability
        public ActionResult Index()
        {
            var result = GetListsConsultDto();
            SetMachineAvailabilityResultConsultDTO(result);
            return PartialView("Index", result);
            //return View();
        }

        [HttpPost]
        public ActionResult GridViewMachineAvailability()
        {
            return PartialView("_GridViewIndex", GetMachineAvailabilityResultConsultDTO());
        }

        private List<MachineAvailabilityResultConsultDTO> GetListsConsultDto()
        {
            using (var db = new DBContext())
            {
                var consultResult = db.tbsysTypeMachineForProd.Where(w => w.isActive).AsEnumerable().Where(w => ActiveUser.Employee.Person.Rol.FirstOrDefault(fod => fod.id == w.id_Rol) != null);
                var query = consultResult.Select(t => new MachineAvailabilityResultConsultDTO
                {
                    id = t.id,
                    code = t.code,
                    nameTbsysTypeMachineForProd = t.name,
                    nameRol = t.Rol.name
                    //MachineAvailabilityDetails = new List<MachineAvailabilityDTO>()
                }).OrderBy(ob => ob.id).ToList();

                bool updateDetail = false;
                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                if (entityObjectPermissions != null)
                {
                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                    if (entityPermissions != null)
                    {
                        foreach (var item in query)
                        {
                            item.MachineAvailabilityDetails = db.MachineForProd.Where(w => w.id_tbsysTypeMachineForProd == item.id &&
                                                                                           (entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == w.id && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) != null))
                                .Select(t => new MachineAvailabilityDTO
                                {
                                    id = t.id,
                                    code = t.code,
                                    nameMachineForProd = t.name,
                                    nameTbsysTypeMachineForProd = t.tbsysTypeMachineForProd.name,
                                    namePersonProcessPlant = db.Person.FirstOrDefault(fod=> fod.id == t.id_personProcessPlant).processPlant,
                                    isActive = t.isActive,
                                    available = t.available,
                                    reason = t.reason
                                }).OrderBy(ob => ob.id).ToList();
                        }
                        var tempModel = new List<MachineAvailabilityResultConsultDTO>();
                        foreach (var item in query)
                        {
                            if (item.MachineAvailabilityDetails.Count > 0)
                            {
                                tempModel.Add(item);
                            }
                        }
                        query = tempModel;
                        updateDetail = true;
                    }
                }

                if (!updateDetail) {
                    var tempModel = new List<MachineAvailabilityResultConsultDTO>();
                    foreach (var item in query)
                    {
                        item.MachineAvailabilityDetails = db.MachineForProd.Where(w => w.id_tbsysTypeMachineForProd == item.id)
                            .Select(t => new MachineAvailabilityDTO
                            {
                                id = t.id,
                                code = t.code,
                                nameMachineForProd = t.name,
                                nameTbsysTypeMachineForProd = t.tbsysTypeMachineForProd.name,
                                namePersonProcessPlant = db.Person.FirstOrDefault(fod => fod.id == t.id_personProcessPlant).processPlant,
                                isActive = t.isActive,
                                available = t.available,
                                reason = t.reason
                            }).OrderBy(ob => ob.id).ToList();
                        if (item.MachineAvailabilityDetails.Count > 0)
                        {
                            tempModel.Add(item);
                        }
                    }
                        query = tempModel;
                }

                return query;
            }
        }

        //[HttpPost]
        //public ActionResult PendingNew()
        //{
        //    return View(GetMachineAvailabilityPendingNewDto());
        //}

        //[ValidateInput(false)]
        //public ActionResult GridViewPendingNew()
        //{
        //    return PartialView("_GridViewPendingNew", GetMachineAvailabilityPendingNewDto());
        //}

        //private MachineAvailabilityDTO Create(int id_MachineProdOpeningDetail)
        //{
        //    using (var db = new DBContext())
        //    {
        //        var machineProdOpeningDetail =
        //            db.MachineProdOpeningDetail.FirstOrDefault(r => r.id == id_MachineProdOpeningDetail);
        //        if (machineProdOpeningDetail == null)
        //            return new MachineAvailabilityDTO();

        //        var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoMachineAvailability));
        //        var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

        //        var machineAvailabilityDTO = new MachineAvailabilityDTO
        //        {
        //            id_machineProdOpeningDetail = id_MachineProdOpeningDetail,
        //            description = "",
        //            number = "",//GetDocumentNumber(documentType?.id ?? 0),
        //            documentType = documentType?.name ?? "",
        //            id_documentType = documentType?.id ?? 0,
        //            reference = "",
        //            dateTimeEmision = machineProdOpeningDetail.MachineProdOpening.Document.emissionDate,
        //            dateTimeEmisionStr = machineProdOpeningDetail.MachineProdOpening.Document.emissionDate.ToString("dd-MM-yyyy"),
        //            idSate = documentState?.id ?? 0,
        //            state = documentState?.name ?? "",
        //            id_machineForProd = machineProdOpeningDetail.id_MachineForProd,
        //            machineForProd = machineProdOpeningDetail.MachineForProd.name,
        //            id_turn = machineProdOpeningDetail.MachineProdOpening.id_Turn,
        //            turn = machineProdOpeningDetail.MachineProdOpening.Turn.name,
        //            noOfPerson = (int)machineProdOpeningDetail.numPerson,
        //            poundsRemitted = 0.00M,
        //            noOfBox = 0,
        //            idPerson = machineProdOpeningDetail.id_Person,
        //            person = machineProdOpeningDetail.Person.fullname_businessName,
        //            poundsProcessed = 0.00M,
        //            poundsTailProcessed = 0.00M,
        //            poundsWholeProcessed = 0.00M,
        //            poundsCooling = 0.00M,
        //            poundsTailCooling = 0.00M,
        //            poundsWholeCooling = 0.00M,
        //            MachineAvailabilityDetails = new List<MachineAvailabilityDetailDTO>()
        //        };

        //        FillMachineAvailabilityDetails(machineAvailabilityDTO, machineProdOpeningDetail);

        //        return machineAvailabilityDTO;
        //    }
        //}

        //private MachineAvailabilityDTO ConvertToDto(MachineAvailability machineAvailability)
        //{
        //    //var reception = machineAvailability.ResultProdLotMachineAvailability;//.FirstOrDefault(r => r.idMachineAvailability == machineAvailability.id);
        //    //if (reception == null)
        //    //    return null;

        //    var machineAvailabilityDto = new MachineAvailabilityDTO
        //    {
        //        id = machineAvailability.id,
        //        id_machineProdOpeningDetail = machineAvailability.id_MachineProdOpeningDetail,
        //        description = machineAvailability.Document.description,
        //        id_documentType = machineAvailability.Document.id_documentType,
        //        number = machineAvailability.Document.number,
        //        state = machineAvailability.Document.DocumentState.name,
        //        documentType = machineAvailability.Document.DocumentType.name,
        //        dateTimeEmision = machineAvailability.Document.emissionDate,//.ToString("d"),
        //        dateTimeEmisionStr = machineAvailability.Document.emissionDate.ToString("dd-MM-yyyy"),
        //        idSate = machineAvailability.Document.id_documentState,
        //        reference = machineAvailability.Document.reference,
        //        id_machineForProd = machineAvailability.MachineProdOpeningDetail.id_MachineForProd,
        //        machineForProd = machineAvailability.MachineProdOpeningDetail.MachineForProd.name,
        //        id_turn = machineAvailability.MachineProdOpeningDetail.MachineProdOpening.id_Turn,
        //        turn = machineAvailability.MachineProdOpeningDetail.MachineProdOpening.Turn.name,
        //        noOfPerson = (int)machineAvailability.MachineProdOpeningDetail.numPerson,
        //        poundsRemitted = 0.00M,
        //        noOfBox = 0,
        //        idPerson = machineAvailability.MachineProdOpeningDetail.id_Person,
        //        person = machineAvailability.MachineProdOpeningDetail.Person.fullname_businessName,
        //        poundsProcessed = 0.00M,
        //        poundsTailProcessed = 0.00M,
        //        poundsWholeProcessed = 0.00M,
        //        poundsCooling = 0.00M,
        //        poundsTailCooling = 0.00M,
        //        poundsWholeCooling = 0.00M,
        //        MachineAvailabilityDetails = new List<MachineAvailabilityDetailDTO>()
        //    };

        //    FillMachineAvailabilityDetails(machineAvailabilityDto, machineAvailability.MachineProdOpeningDetail);

        //    return machineAvailabilityDto;
        //}

        [HttpPost]
        public ActionResult Edit(int id = 0, bool enabled = true)
        {
            var model = new MachineAvailabilityResultConsultDTO();
            var machineAvailabilityResultConsult = GetMachineAvailabilityResultConsultDTO();
            model = machineAvailabilityResultConsult.FirstOrDefault(d => d.id == id);
            ViewBag.enabled = enabled;

            SetaMachineAvailabilityResultConsultDTO(model);

            return PartialView(model);
        }

        //private void BuilViewBag(bool enabled)
        //{
        //    var machineAvailabilityDTO = GetMachineAvailabilityDTO();
        //    ViewBag.enabled = enabled;
        //    ViewBag.canNew = machineAvailabilityDTO.id != 0;
        //    ViewBag.canEdit = !enabled &&
        //                      (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == machineAvailabilityDTO.idSate)
        //                           ?.code.Equals("01") ?? false);
        //    ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == machineAvailabilityDTO.idSate)
        //                             ?.code.Equals("01") ?? false) && machineAvailabilityDTO.id != 0;
        //    ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == machineAvailabilityDTO.idSate)
        //                             ?.code.Equals("03") ?? false) && !enabled;
        //    ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == machineAvailabilityDTO.idSate)
        //                              ?.code.Equals("01") ?? false) && machineAvailabilityDTO.id != 0;
        //}

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetails(bool? enabled)
        {
            var machineAvailabilityDetails = GetaMachineAvailabilityResultConsultDTO().MachineAvailabilityDetails;

            ViewBag.enabled = enabled;

            return PartialView("_GridViewDetails", machineAvailabilityDetails);
        }

        [HttpPost]
        public JsonResult Save(string jsonMachineAvailability)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var machineAvailabilityResultConsultDTO = GetaMachineAvailabilityResultConsultDTO();
                        
                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        JArray listArray = JsonConvert.DeserializeObject<JArray>(jsonMachineAvailability);
                        foreach (var item in listArray)
                        {
                            #region Validación Permiso

                            var aId = item.Value<int>("id");
                            if (entityObjectPermissions != null)
                            {
                                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                                if (entityPermissions != null)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == aId && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new Exception("No tiene Permiso para editar y guardar la disponibilidad para máquina. Máquina: " + item.Value<string>("nameMachineForProd"));
                                    }
                                }
                            }

                            #endregion
                            var aMachineForProd = db.MachineForProd.FirstOrDefault(d => d.id == aId);
                            aMachineForProd.available = item.Value<bool>("available");
                            aMachineForProd.reason = item.Value<string>("reason");
                            aMachineForProd.id_userUpdateAvailable = ActiveUser.id;
                            aMachineForProd.dateUpdateAvailable = DateTime.Now;
                            db.MachineForProd.Attach(aMachineForProd);
                            db.Entry(aMachineForProd).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = machineAvailabilityResultConsultDTO.id.ToString();

                        SetMachineAvailabilityResultConsultDTO(GetListsConsultDto());
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}