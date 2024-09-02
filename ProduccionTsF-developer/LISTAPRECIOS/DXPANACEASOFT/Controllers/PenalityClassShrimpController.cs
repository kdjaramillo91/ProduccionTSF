using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class PenalityClassShrimpController : DefaultController
    {
        // GET: PenalityClass
        public ActionResult Index()
        {
            return View(GetPenalityListDTO());
        }

        private PenalityClassShrimpDTO GetPenalityClassShrimpDTO()
        {
            if (!(Session["PenalityClassShrimpDTO"] is PenalityClassShrimpDTO penalityClassShrimpDTO))
                penalityClassShrimpDTO = new PenalityClassShrimpDTO();
            return penalityClassShrimpDTO;
        }

        private void SetPenalityClassShrimpDTO(PenalityClassShrimpDTO penalityClassShrimpDTO)
        {
            Session["PenalityClassShrimpDTO"] = penalityClassShrimpDTO;
        }

        private List<PenalityClassShrimpDTO> GetPenalityListDTO()
        {
            using (var db = new DBContext())
            {
                var query = db.PenalityClassShrimp.Select(p => new PenalityClassShrimpDTO
                {
                    id = p.id,
                    byProvider = p.byProvider,
                    byGroup = !p.byProvider,
                    nameDestination = p.byProvider ? p.Provider.Person.fullname_businessName : p.GroupPersonByRol.name,
                    id_provider = p.id_provider,
                    id_groupPersonByRol = p.id_groupPersonByRol,
                }).ToList();

                return query;
            }
        }

        [ValidateInput(false)]
        public ActionResult GridViewPenalityList()
        {
            return PartialView("_GridViewPenality", GetPenalityListDTO());
        }

        public ActionResult ComboBoxPenalityProveedores(PenalityClassShrimpDTO penalityClassShrimpDTO = null, bool enabled = true)
        {
            var proveedores = new List<SelectListItem>();

            using (DBContext db = new DBContext())
            {
                proveedores = db.Provider
                .Where(g => db.PenalityClassShrimp.FirstOrDefault(p => p.id_provider == g.id) == null)
                .Select(s => new SelectListItem
                {
                    Text = s.Person.fullname_businessName,
                    Value = s.id.ToString()
                }).ToList();

                if (penalityClassShrimpDTO != null && penalityClassShrimpDTO.id != 0)
                {
                    var proveedor = db.Provider.FirstOrDefault(p => p.id == penalityClassShrimpDTO.id_provider);
                    proveedores.Insert(0, new SelectListItem
                    {
                        Text = proveedor?.Person.fullname_businessName ?? "",
                        Value = proveedor?.id.ToString() ?? "0"
                    });
                }
            }

            ViewData["Proveedores"] = proveedores;
            ViewBag.enabled = enabled;
            return PartialView("_ComboBoxProveedores");
        }

        public ActionResult ComboBoxPenalityGrupos(PenalityClassShrimpDTO penalityClassShrimpDTO = null, bool enabled = true)
        {
            var grupos = new List<SelectListItem>();
            using (DBContext db = new DBContext())
            {
                grupos = db.GroupPersonByRol
                .Where(g => db.PenalityClassShrimp.FirstOrDefault(p => p.id_groupPersonByRol == g.id) == null)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();

                if (penalityClassShrimpDTO != null && penalityClassShrimpDTO.id != 0)
                {
                    var grupo = db.GroupPersonByRol.FirstOrDefault(p => p.id == penalityClassShrimpDTO.id_groupPersonByRol);
                    grupos.Insert(0, new SelectListItem
                    {
                        Text = grupo?.name ?? "",
                        Value = grupo?.id.ToString() ?? "0"
                    });
                }
            }

            ViewData["Grupos"] = grupos;
            ViewBag.enabled = enabled;
            return PartialView("_ComboBoxGrupos");
        }

        private void BuildViewData(PenalityClassShrimpDTO penalityClassShrimpDTO = null, bool enabled = true)
        {
            ComboBoxPenalityProveedores(penalityClassShrimpDTO, enabled);
            ComboBoxPenalityGrupos(penalityClassShrimpDTO, enabled);

            ViewBag.enabled = enabled;
        }

        private static void BuildClassShrimpPenalityDTO(PenalityClassShrimpDTO penalityClassShrimpDTO)
        {
            using (var db = new DBContext())
            {

                penalityClassShrimpDTO.ListPriceListPenaltyDTO = new List<PriceListPenaltyDTO>();
                if (penalityClassShrimpDTO.id != 0)
                {
                    penalityClassShrimpDTO.ListPriceListPenaltyDTO = db.PenalityClassShrimpDetails
                    .Where(d => d.id_penalityClassShrimp == penalityClassShrimpDTO.id)
                    .Select(i => new PriceListPenaltyDTO
                    {
                        id_classShrimp = i.id_classShrimp,
                        classShrimp = i.ClassShrimp.name,
                        value = i.value,
                        order = i.ClassShrimp.order,
                    }).ToList();
                }

                var listClassShrimp = new List<PriceListPenaltyDTO>();
                listClassShrimp = db.ClassShrimp.Where(p => p.isActive).Select(i => new PriceListPenaltyDTO
                {
                    id_classShrimp = i.id,
                    classShrimp = i.name,
                    value = 0,
                    order = i.order,
                }).ToList();

                foreach (var classShrimp in listClassShrimp)
                {
                    bool isIn = false;
                    foreach (var detailClassShrimp in penalityClassShrimpDTO.ListPriceListPenaltyDTO)
                    {
                        if (classShrimp.id_classShrimp == detailClassShrimp.id_classShrimp)
                        {
                            isIn = true;
                            break;
                        }
                    }
                    if (!isIn)
                        penalityClassShrimpDTO.ListPriceListPenaltyDTO.Add(classShrimp);
                }
            }
        }

        public ActionResult GridViewPenaltyDetails(bool enabled = true)
        {
            ViewBag.enabled = enabled;
            var penalityClassShrimpDTO = GetPenalityClassShrimpDTO();
            return PartialView("_GridViewPenaltyDetails", penalityClassShrimpDTO.ListPriceListPenaltyDTO.OrderBy(ob => ob.order));
        }

        [HttpPost]
        public ActionResult Edit(int id, bool enabled = true)
        {
            using (var db = new DBContext())
            {
                PenalityClassShrimpDTO penalityClassShrimpDTO;
                var penality = db.PenalityClassShrimp.FirstOrDefault(p => p.id == id);
                if (penality == null)
                {
                    penalityClassShrimpDTO = new PenalityClassShrimpDTO
                    {
                        byGroup = true,
                        byProvider = false,
                    };
                }
                else
                {
                    penalityClassShrimpDTO = new PenalityClassShrimpDTO
                    {
                        id = penality.id,
                        byGroup = !penality.byProvider,
                        byProvider = penality.byProvider,
                        id_groupPersonByRol = penality.id_groupPersonByRol,
                        id_provider = penality.id_provider
                    };
                }

                BuildClassShrimpPenalityDTO(penalityClassShrimpDTO);
                BuildViewData(penalityClassShrimpDTO, enabled);

                SetPenalityClassShrimpDTO(penalityClassShrimpDTO);
                return PartialView(penalityClassShrimpDTO);
            }
        }

        [HttpPost]
        public JsonResult Save(string jsonPenality)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();
                    try
                    {
                        JToken tokenPenality = JsonConvert.DeserializeObject<JToken>(jsonPenality);
                        var newObject = false;
                        var id = tokenPenality.Value<int>("id");

                        var penality = db.PenalityClassShrimp.FirstOrDefault(p => p.id == id);
                        if (penality == null)
                        {
                            newObject = true;

                            penality = new PenalityClassShrimp();
                        }

                        penality.byProvider = tokenPenality.Value<bool>("byProvider");
                        penality.id_provider = tokenPenality.Value<int?>("id_provider");
                        penality.id_groupPersonByRol = tokenPenality.Value<int?>("id_groupPersonByRol");

                        //Penalizacion
                        var penaltyAux = db.PenalityClassShrimpDetails.Where(i => i.id_penalityClassShrimp == penality.id);
                        foreach (var penalty in penaltyAux)
                        {
                            db.PenalityClassShrimpDetails.Remove(penalty);
                            db.PenalityClassShrimpDetails.Attach(penalty);
                            db.Entry(penalty).State = EntityState.Deleted;
                        }

                        JArray priceListPenalty = tokenPenality.Value<JArray>("priceListPenalty");
                        foreach (JToken penalty in priceListPenalty)
                        {
                            string penalidad = db.Setting.FirstOrDefault(r => r.code == "PPC")?.value;

                            if (!string.IsNullOrEmpty(penalidad) && penalidad == "NO" && penalty.Value<decimal>("value") == 0)
                            {
                                throw new Exception("No puede registrar penalidad en cero, verifique cofiguración de parámetro PPC");
                            }

                            penality.PenalityClassShrimpDetails.Add(new PenalityClassShrimpDetails
                            {
                                id_classShrimp = penalty.Value<int>("id_classShrimp"),
                                value = penalty.Value<decimal>("value")
                            });
                        }

                        if (newObject)
                        {
                            db.PenalityClassShrimp.Add(penality);
                            db.Entry(penality).State = EntityState.Added;
                        }
                        else
                        {
                            db.PenalityClassShrimp.Attach(penality);
                            db.Entry(penality).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();

                        result.Data = penality.id.ToString();
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