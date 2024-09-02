using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using DevExpress.Data.Utils;
using DevExpress.Web;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Services;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ForeignCustomerController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region ForeignCustomers

        public ActionResult PopupControlRolForeignCustomerPartial()
        {
            try
            {

                Person person = (TempData["person"] as Person);
                if (TempData["foreignCustomer"] != null)
                {

                    TempData.Keep("foreignCustomer");
                }
                //person = person ?? db.Person.FirstOrDefault(fod => fod.id == id_person);
                person = person ?? new Person();

                TempData.Keep();
                return PartialView("_PopupControlRolForeignCustomerPartial", person);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //[ActionName("PopupComentarioPospuesto")]
        [HttpPost, ValidateInput(false)]
        public ActionResult PopupControlRolForeignCustomer(int? id_person)
        {
            try
            {
                var person = db.Person.FirstOrDefault(fod => fod.id == id_person);
                person = person ?? new Person();
                var modelAux = db.ForeignCustomer.FirstOrDefault(fod => fod.id == person.id);
                modelAux = modelAux ?? new ForeignCustomer
                {
                    Person = person,
                    id = person.id
                };
                TempData["foreignCustomer"] = modelAux;

                //ViewBag.rowCountRequired = db.Country_IdentificationType.Where(w => w.id_country == modelAux.id_country).ToList().Count();

                TempData.Keep();
                return PartialView("_FormEditPopupControlRolForeignCustomerPartial", modelAux);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ValidateInput(false)]
        public ActionResult FormEditForeignCustomerIdentificationPartial()
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);

            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            TempData["foreignCustomer"] = /*TempData["foreignCustomer"] ??*/ foreignCustomer;
            TempData.Keep();
            return PartialView("_FormEditForeignCustomerIdentificationPartial", foreignCustomer);
        }

        [ValidateInput(false)]
        public ActionResult FormEditDetailForeignCustomerIdentificationPartial(int? id_countryForeignCustomer)
        {
            if (TempData["foreignCustomer"] != null)
            {
                TempData.Keep("foreignCustomer");
            }
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);

            foreignCustomer = foreignCustomer ?? new ForeignCustomer();
            if (foreignCustomer.Person.ForeignCustomerIdentification.Count() == 0)
            {
                foreignCustomer.Person.ForeignCustomerIdentification = new List<ForeignCustomerIdentification>();
                ViewBag.Direccion = foreignCustomer.Person.address;
            }

            //for (int i = foreignCustomer.Person.ForeignCustomerIdentification.Count - 1; i >= 0; i--)
            //{
            //    ForeignCustomerIdentification foreignCustomerIdentification = foreignCustomer.Person.ForeignCustomerIdentification.ElementAt(i);
            //    if (foreignCustomerIdentification.Country_IdentificationType.id_country != id_countryForeignCustomer)
            //        foreignCustomer.Person.ForeignCustomerIdentification.Remove(foreignCustomerIdentification);
            //}

            var model = foreignCustomer.Person.ForeignCustomerIdentification.ToList();

            ViewBag.id_countryForeignCustomerCurrent = id_countryForeignCustomer;
            TempData["foreignCustomer"] =/* TempData["foreignCustomer"] ??*/ foreignCustomer;
            TempData.Keep();
            return PartialView("_FormEditDetailForeignCustomerIdentificationPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditDetailForeignCustomerIdentificationAddNew(/*int id_person,*/ ForeignCustomerIdentification foreignCustomerIdentification)
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }


            //Person person = (TempData["person"] as Person);
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            //person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            if (ModelState.IsValid)
            {
                try
                {
                    foreignCustomerIdentification.id = foreignCustomer.Person.ForeignCustomerIdentification.Count() > 0 ? foreignCustomer.Person.ForeignCustomerIdentification.Max(pld => pld.id) + 1 : 1;
                    foreignCustomerIdentification.id_ForeignCustomer = foreignCustomer.id;
                    foreignCustomerIdentification.Person = foreignCustomer.Person;
                    foreignCustomerIdentification.Country = db.Country.FirstOrDefault(fod => fod.id == foreignCustomerIdentification.id_country);
                    foreignCustomerIdentification.City = db.City.FirstOrDefault(fod => fod.id == foreignCustomerIdentification.id_city);
                    foreignCustomerIdentification.Country_IdentificationType = db.Country_IdentificationType.FirstOrDefault(fod => fod.id == foreignCustomerIdentification.id_Country_IdentificationType);
                    foreignCustomer.Person.ForeignCustomerIdentification.Add(foreignCustomerIdentification);
                    TempData["foreignCustomer"] = foreignCustomer;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            ViewBag.id_countryForeignCustomerCurrent = foreignCustomerIdentification.Country_IdentificationType?.id_country;
            TempData.Keep();

            var model = foreignCustomer.Person.ForeignCustomerIdentification?.ToList() ?? new List<ForeignCustomerIdentification>();
            return PartialView("_FormEditDetailForeignCustomerIdentificationPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditDetailForeignCustomerIdentificationUpdate(ForeignCustomerIdentification foreignCustomerIdentification)
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }
            //Person person = (TempData["person"] as Person);
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            //person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            if (ModelState.IsValid && foreignCustomer != null)
            {
                try
                {
                    // Si no existe en base, busco en temporal
                    var modelForeignCustomerIdentification = db.ForeignCustomerIdentification.FirstOrDefault(i => i.id == foreignCustomerIdentification.id);
                    if (modelForeignCustomerIdentification != null)
                    {
                        modelForeignCustomerIdentification.Country_IdentificationType = db.Country_IdentificationType.FirstOrDefault(fod => fod.id == foreignCustomerIdentification.id_Country_IdentificationType);
                        modelForeignCustomerIdentification.id_invoiceLanguage = foreignCustomerIdentification.id_invoiceLanguage;
                        modelForeignCustomerIdentification.id_country = foreignCustomerIdentification.id_country;
                        modelForeignCustomerIdentification.id_city = foreignCustomerIdentification.id_city;
                        modelForeignCustomerIdentification.emailInterno = foreignCustomerIdentification.emailInterno;
                        modelForeignCustomerIdentification.emailInternoCC = foreignCustomerIdentification.emailInternoCC;
                        modelForeignCustomerIdentification.phone1FC = foreignCustomerIdentification.phone1FC;
                        modelForeignCustomerIdentification.AddressType = foreignCustomerIdentification.AddressType;
                        modelForeignCustomerIdentification.addressForeign = foreignCustomerIdentification.addressForeign;
                        modelForeignCustomerIdentification.printInInvoice = foreignCustomerIdentification.printInInvoice;
                        this.UpdateModel(modelForeignCustomerIdentification);

                        foreignCustomer = modelForeignCustomerIdentification.Person.ForeignCustomer;

                        TempData["foreignCustomer"] = foreignCustomer;
                        ViewBag.id_countryForeignCustomerCurrent = modelForeignCustomerIdentification.Country_IdentificationType.id_country;
                        ViewBag.id_identificationTypeCurrent = modelForeignCustomerIdentification.Country_IdentificationType.id_identificationType;
                    }
                    else
                    {
                        var modelForeignCustomerIdentificationAux1 = foreignCustomer.Person.ForeignCustomerIdentification.FirstOrDefault(fod => fod.id == foreignCustomerIdentification.id);
                        if (modelForeignCustomerIdentificationAux1 != null)
                        {
                            modelForeignCustomerIdentificationAux1.Country_IdentificationType = db.Country_IdentificationType.FirstOrDefault(fod => fod.id == foreignCustomerIdentification.id_Country_IdentificationType);
                            modelForeignCustomerIdentificationAux1.id_invoiceLanguage = foreignCustomerIdentification.id_invoiceLanguage;
                            modelForeignCustomerIdentificationAux1.id_country = foreignCustomerIdentification.id_country;
                            modelForeignCustomerIdentificationAux1.id_city = foreignCustomerIdentification.id_city;
                            modelForeignCustomerIdentificationAux1.emailInterno = foreignCustomerIdentification.emailInterno;
                            modelForeignCustomerIdentificationAux1.emailInternoCC = foreignCustomerIdentification.emailInternoCC;
                            modelForeignCustomerIdentificationAux1.phone1FC = foreignCustomerIdentification.phone1FC;
                            modelForeignCustomerIdentificationAux1.AddressType = foreignCustomerIdentification.AddressType;
                            modelForeignCustomerIdentificationAux1.addressForeign = foreignCustomerIdentification.addressForeign;
                            modelForeignCustomerIdentificationAux1.printInInvoice = foreignCustomerIdentification.printInInvoice;
                        }
                        this.UpdateModel(modelForeignCustomerIdentificationAux1);

                        foreignCustomer = modelForeignCustomerIdentificationAux1.Person.ForeignCustomer;

                        TempData["foreignCustomer"] = foreignCustomer;
                        ViewBag.id_countryForeignCustomerCurrent = modelForeignCustomerIdentificationAux1.Country_IdentificationType.id_country;
                        ViewBag.id_identificationTypeCurrent = modelForeignCustomerIdentificationAux1.Country_IdentificationType.id_identificationType;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep();


            var model = foreignCustomer.Person.ForeignCustomerIdentification?.ToList() ?? new List<ForeignCustomerIdentification>();
            return PartialView("_FormEditDetailForeignCustomerIdentificationPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditDetailForeignCustomerIdentificationDelete(/*int id_person,*/ int id)
        {

            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }
            //Person person = (TempData["person"] as Person);
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            //person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            try
            {
                var foreignCustomerIdentification = foreignCustomer.Person.ForeignCustomerIdentification.FirstOrDefault(it => it.id == id);
                ViewBag.id_countryForeignCustomerCurrent = foreignCustomerIdentification.Country_IdentificationType.id_country;

                if (foreignCustomerIdentification != null)
                    foreignCustomer.Person.ForeignCustomerIdentification.Remove(foreignCustomerIdentification);

                TempData["foreignCustomer"] = foreignCustomer;


            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep();

            var model = foreignCustomer.Person.ForeignCustomerIdentification?.ToList() ?? new List<ForeignCustomerIdentification>();
            return PartialView("_FormEditDetailForeignCustomerIdentificationPartial", model);
        }

        public Boolean EvaluateExistDocumentType(List<ForeignCustomerIdentification> customerIdentification, int Country_IdentificationTypeId)
        {

            return (customerIdentification.FirstOrDefault(fod => fod.id_Country_IdentificationType == Country_IdentificationTypeId) == null);
        }

        [HttpPost]
        public ActionResult GetCountry_IdentificationType(int? id_Country_IdentificationType, int? id_countryForeignCustomer)
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            //person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            int numCountryIdentificationType = db.Country_IdentificationType.Count(r => r.id_country == id_countryForeignCustomer);
            if (numCountryIdentificationType == 0 && id_countryForeignCustomer != null) // no existe difinido pais agregar tipo de documento pasaporte
            {

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {

                    try
                    {
                        IdentificationType identificationType = db.IdentificationType.FirstOrDefault(r => r.code == "03");
                        Country_IdentificationType icountry_IdentificationType = new Country_IdentificationType
                        {
                            id_identificationType = identificationType.id,
                            id_country = (int)id_countryForeignCustomer,
                            dateCreate = DateTime.Now,
                            id_userCreate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            isActive = true
                        };
                        db.Country_IdentificationType.Add(icountry_IdentificationType);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch //(Exception e)
                    {

                    }
                }

            }


            try
            {
                /*
                 var resultQuery = dataContext.Customers
                .Where (c => c.Name == "Alugili")
                .AsEnumerable()
                .Where (c => SalesCount(c.CustomerId) < 100);
                 */

                var country_IdentificationTypes
                                    = db.Country_IdentificationType.Where(w =>
                                    (w.id_country == id_countryForeignCustomer)).AsEnumerable()
                                    .Where
                                    (w2 =>
                                      EvaluateExistDocumentType(foreignCustomer.Person.ForeignCustomerIdentification.ToList(), w2.id)
                                      || w2.id == id_Country_IdentificationType
                                     ).Select(s => new { s.id, s.IdentificationType.code, s.IdentificationType.name }).ToList();

                if (country_IdentificationTypes == null || country_IdentificationTypes.Count() == 0) country_IdentificationTypes = db.IdentificationType.AsEnumerable().Where(r => r.code == "03").Select(r2 => new { r2.id, r2.code, r2.name }).ToList();

                TempData.Keep();
                //ViewData["id_person"] = id_person;
                //var dataProvider = GetDataProvider(dataSource);
                return GridViewExtension.GetComboBoxCallbackResult(p =>
                {
                    //settings.Name = "id_person";
                    p.ClientInstanceName = "id_Country_IdentificationType";
                    p.Width = Unit.Percentage(100);

                    //p.DropDownStyle = DropDownStyle.DropDownList;
                    p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                    //p.EnableSynchronization = DefaultBoolean.False;
                    p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                    //p.NullDisplayText = "Todo";
                    //p.NullText = "Todo";
                    //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                    p.CallbackRouteValues = new { Controller = "ForeignCustomer", Action = "GetCountry_IdentificationType"/*, TextField = "CityName"*/ };
                    p.ClientSideEvents.BeginCallback = "PersonCountry_IdentificationType_BeginCallback";
                    p.ClientSideEvents.EndCallback = "PersonCountry_IdentificationType_EndCallback";
                    p.CallbackPageSize = 5;

                    p.DropDownStyle = DropDownStyle.DropDownList;

                    //p.ValueField = "id";
                    //p.TextField = "name";
                    p.TextFormatString = "{1}";
                    p.ValueField = "id";
                    p.ValueType = typeof(int);

                    p.Columns.Add("code", "Cod.", 50);
                    p.Columns.Add("name", "Nombre", 100);
                    //settings.ReadOnly = codeState != "01";//Pendiente
                    //p.ShowModelErrors = true;
                    //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                    p.ClientSideEvents.Validation = "OnCountry_IdentificationTypeComboValidation";
                    p.ClientSideEvents.Init = "Country_IdentificationTypeCombo_Init";
                    //p.TextField = textField;
                    p.BindList(country_IdentificationTypes);//.Bind(id_person);

                });
            }
            catch (Exception e)
            {
                ViewBag.error = e.Message;

            }
            return new ContentResult();

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost]
        public ActionResult GetCountry_City(int? id_City, int? id_countryForeignCustomer)
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();


            try
            {

                var citys
                        = db.City.Where(w =>
                        (w.id_country == id_countryForeignCustomer)).AsEnumerable()
                        .Where
                        (w2 => w2.id == id_City
                            ).Select(s => new { s.id, s.code, s.name }).ToList();

                if (citys == null || citys.Count() == 0) citys = db.City.AsEnumerable().Where(r => r.code == "03").Select(r2 => new { r2.id, r2.code, r2.name }).ToList();

                TempData.Keep();

                return GridViewExtension.GetComboBoxCallbackResult(p =>
                {
                    p.ClientInstanceName = "id_City";
                    p.Width = Unit.Percentage(100);

                    p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                    p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;


                    p.CallbackRouteValues = new { Controller = "ForeignCustomer", Action = "GetCountry_City" };
                    p.ClientSideEvents.BeginCallback = "CityCountry_BeginCallback";
                    p.ClientSideEvents.Validation = "Country_CityCombo_Validation";
                    p.ClientSideEvents.EndCallback = "CityCountry_EndCallback";
                    p.CallbackPageSize = 5;

                    p.DropDownStyle = DropDownStyle.DropDownList;

                    //p.ValueField = "id";
                    //p.TextField = "name";
                    p.TextFormatString = "{1}";
                    p.ValueField = "id";
                    p.ValueType = typeof(int);

                    p.Columns.Add("code", "Cod.", 50);
                    p.Columns.Add("name", "Nombre", 100);
                    //p.ClientSideEvents.Validation = "OnCountry_IdentificationTypeComboValidation";
                    p.ClientSideEvents.Init = "Country_CityCombo_Init";

                    p.BindList(citys);//.Bind(id_person);

                });
            }
            catch (Exception e)
            {
                ViewBag.error = e.Message;

            }
            return new ContentResult();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateCodeCountry_IdentificationType(int? id_Country_IdentificationTypeCurrent)
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }
            //Person person = (TempData["person"] as Person);
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            //person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            //var codeCountry = "";
            //var codeIdentificationType = "";
            //var nameIdentificationType = "";
            var country_IdentificationTypeCurrentAux = db.Country_IdentificationType.Where(fod => fod.id_country == id_Country_IdentificationTypeCurrent).Select(s => new
            {
                id = s.id,
                code = s.IdentificationType.code,
                name = s.IdentificationType.name
            }).ToList();
            var result = new
            {
                identificationTypes = country_IdentificationTypeCurrentAux
            };


            TempData.Keep("foreignCustomer");

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateCityCountry(int? id_Country_CityCurrent)
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }

            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            var country_CityCurrentAux = db.City.Where(fod => fod.id_country == id_Country_CityCurrent).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();
            var result = new
            {
                cityTypes = country_CityCurrentAux
            };


            TempData.Keep("foreignCustomer");

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CountryDetailData(int? id_country)
        {

            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            if (foreignCustomer != null)
            {
                TempData["foreignCustomer"] = foreignCustomer;
                TempData.Keep("foreignCustomer");
            }
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            Country country = db.Country.FirstOrDefault(fod => fod.id == id_country);

            var cities = db.City.Where(w => (w.id_country == id_country && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                cities = cities
            };


            TempData.Keep("foreignCustomer");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateRowCountRequired(int? id_countryCurrent)
        {
            if (TempData["foreignCustomer"] != null)
            {
                TempData.Keep("foreignCustomer");
            }
            //Person person = (TempData["person"] as Person);
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);
            //person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            var rowCountRequired = db.Country_IdentificationType.Where(w => w.id_country == id_countryCurrent).ToList().Count();

            var result = new
            {
                rowCountRequired = rowCountRequired

            };

            TempData.Keep();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PersonForeignCustomerUpdate(int id,
                                                      string bankRefForeignCustomer, int id_PaymentTermForeignCustomer,
                                                      long? IFFC, long? RCFC, long? patenteFC, long? CNSSFC, string ICEFC)
        {
            if (TempData["foreignCustomer"] != null)
            {

                TempData.Keep("foreignCustomer");
            }
            var result = new
            {
                message = "OK"

            };
            //bool recordedOk = false;

            PaymentTerm customPaymentTerm = db.PaymentTerm.FirstOrDefault(r => r.code == "AV");
            id_PaymentTermForeignCustomer = customPaymentTerm.id;
            ForeignCustomer foreignCustomer = (TempData["foreignCustomer"] as ForeignCustomer);

            foreignCustomer = foreignCustomer ?? new ForeignCustomer();

            var personAux = db.Person.FirstOrDefault(p => p.id == id);
            var modelItem = db.ForeignCustomer.FirstOrDefault(p => p.id == id);

            if (ModelState.IsValid)
            {
                #region"TRANSACCION INSERTAR FOREIGN CUSTOMER"
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (modelItem != null)
                        {
                            modelItem.bankRef = bankRefForeignCustomer;
                            modelItem.id_PaymentTerm = id_PaymentTermForeignCustomer;

                            modelItem.IFFC = IFFC;
                            modelItem.RCFC = RCFC;
                            modelItem.patenteFC = patenteFC;
                            modelItem.CNSSFC = CNSSFC;
                            modelItem.ICEFC = ICEFC;

                            modelItem.isActive = true;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            for (int i = modelItem.Person.ForeignCustomerIdentification.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.Person.ForeignCustomerIdentification.ElementAt(i);
                                if (detail.SalesQuotationExterior.Count() == 0 && detail.InvoiceExterior.Count() == 0)
                                {
                                    modelItem.Person.ForeignCustomerIdentification.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }

                            }

                            foreach (var foreignCustomerIdentification in foreignCustomer.Person.ForeignCustomerIdentification)
                            {
                                var _ForeignCustomerIdentification = modelItem.Person.ForeignCustomerIdentification.FirstOrDefault(p => p.id == foreignCustomerIdentification.id);
                                if (_ForeignCustomerIdentification != null)
                                {
                                    _ForeignCustomerIdentification.id_ForeignCustomer = personAux.id;
                                    _ForeignCustomerIdentification.id_Country_IdentificationType = foreignCustomerIdentification.id_Country_IdentificationType;
                                    _ForeignCustomerIdentification.numberIdentification = foreignCustomerIdentification.numberIdentification;
                                    _ForeignCustomerIdentification.id_country = foreignCustomerIdentification.id_country;
                                    _ForeignCustomerIdentification.id_city = foreignCustomerIdentification.id_city;
                                    _ForeignCustomerIdentification.id_invoiceLanguage = foreignCustomerIdentification.id_invoiceLanguage;
                                    _ForeignCustomerIdentification.AddressType = foreignCustomerIdentification.AddressType;
                                    _ForeignCustomerIdentification.addressForeign = foreignCustomerIdentification.addressForeign;
                                    _ForeignCustomerIdentification.emailInterno = foreignCustomerIdentification.emailInterno;
                                    _ForeignCustomerIdentification.emailInternoCC = foreignCustomerIdentification.emailInternoCC;
                                    _ForeignCustomerIdentification.phone1FC = foreignCustomerIdentification.phone1FC;
                                    _ForeignCustomerIdentification.fax1FC = foreignCustomerIdentification.fax1FC;
                                    _ForeignCustomerIdentification.isActive = true;
                                    _ForeignCustomerIdentification.id_userUpdate = ActiveUser.id;
                                    _ForeignCustomerIdentification.dateUpdate = DateTime.Now;
                                    _ForeignCustomerIdentification.printInInvoice = foreignCustomerIdentification.printInInvoice;
                                }
                                else
                                {
                                    //if(foreignCustomerIdentification.SalesQuotationExterior.Count() == 0)
                                    //{
                                    var newForeignCustomerIdentification = new ForeignCustomerIdentification
                                    {
                                        id_ForeignCustomer = personAux.id,
                                        id_Country_IdentificationType = foreignCustomerIdentification.id_Country_IdentificationType,
                                        numberIdentification = foreignCustomerIdentification.numberIdentification,
                                        id_country = foreignCustomerIdentification.id_country,
                                        id_city = foreignCustomerIdentification.id_city,
                                        id_invoiceLanguage = foreignCustomerIdentification.id_invoiceLanguage,
                                        AddressType = foreignCustomerIdentification.AddressType,
                                        addressForeign = foreignCustomerIdentification.addressForeign,
                                        emailInterno = foreignCustomerIdentification.emailInterno,
                                        emailInternoCC = foreignCustomerIdentification.emailInternoCC,
                                        phone1FC = foreignCustomerIdentification.phone1FC,
                                        fax1FC = foreignCustomerIdentification.fax1FC,
                                        printInInvoice = foreignCustomerIdentification.printInInvoice,
                                        isActive = true,
                                        id_userCreate = ActiveUser.id,
                                        dateCreate = DateTime.Now,
                                        id_userUpdate = ActiveUser.id,
                                        dateUpdate = DateTime.Now
                                    };
                                    modelItem.Person.ForeignCustomerIdentification.Add(newForeignCustomerIdentification);
                                }
                            }
                            db.ForeignCustomer.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;
                        }
                        else
                        {
                            modelItem = new ForeignCustomer
                            {
                                id = personAux.id,
                                code = personAux.identification_number,
                                name = personAux.fullname_businessName,
                                bankRef = bankRefForeignCustomer,
                                id_PaymentTerm = id_PaymentTermForeignCustomer,
                                isActive = true,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now,
                                id_userUpdate = ActiveUser.id,
                                dateUpdate = DateTime.Now,
                                Person = personAux,

                                IFFC = IFFC,
                                RCFC = RCFC,
                                patenteFC = patenteFC,
                                CNSSFC = CNSSFC,
                                ICEFC = ICEFC
                            };
                            modelItem.Person.ForeignCustomerIdentification = new List<ForeignCustomerIdentification>();
                            foreach (var foreignCustomerIdentification in foreignCustomer.Person.ForeignCustomerIdentification)
                            {
                                var newForeignCustomerIdentification = new ForeignCustomerIdentification
                                {
                                    id_ForeignCustomer = personAux.id,
                                    id_Country_IdentificationType = foreignCustomerIdentification.id_Country_IdentificationType,
                                    numberIdentification = foreignCustomerIdentification.numberIdentification,
                                    printInInvoice = foreignCustomerIdentification.printInInvoice,
                                    id_country = foreignCustomerIdentification.id_country,
                                    id_city = foreignCustomerIdentification.id_city,
                                    id_invoiceLanguage = foreignCustomerIdentification.id_invoiceLanguage,
                                    AddressType = foreignCustomerIdentification.AddressType,
                                    addressForeign = foreignCustomerIdentification.addressForeign,
                                    emailInterno = foreignCustomerIdentification.emailInterno,
                                    emailInternoCC = foreignCustomerIdentification.emailInternoCC,
                                    phone1FC = foreignCustomerIdentification.phone1FC,
                                    fax1FC = foreignCustomerIdentification.fax1FC,
                                    isActive = true,
                                    id_userCreate = ActiveUser.id,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = ActiveUser.id,
                                    dateUpdate = DateTime.Now
                                };
                                modelItem.Person.ForeignCustomerIdentification.Add(newForeignCustomerIdentification);
                            }
                            db.ForeignCustomer.Add(modelItem);
                        }

                        #region"AGREGAR EL ROL A LA PERSONA"
                        if (personAux != null)
                        {
                            var par = personAux.Rol.FirstOrDefault(fod => fod.name == "Cliente Exterior");
                            if (par == null)
                            {
                                Rol r = db.Rol.FirstOrDefault(fod => fod.name == "Cliente Exterior");
                                personAux.Rol.Add(r);

                                db.Person.Attach(personAux);
                                db.Entry(personAux).State = EntityState.Modified;

                            }
                        }
                        #endregion

                        db.SaveChanges();
                        trans.Commit();
                        //recordedOk = true;
                    }
                    catch (Exception e)
                    {
                        TempData["foreignCustomer"] = foreignCustomer;
                        TempData.Keep();
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        result = new
                        {
                            message = ErrorMessage(e.Message)

                        };
                    }
                }
                #endregion
            }

            TempData.Keep();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}