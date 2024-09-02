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

using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class PersonController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonsPartial()
        {
            Person tempPerson = (TempData["person"] as Person);
            var errorMessagePersonTmp = TempData["ErrorMessagePerson"] != null ? TempData["ErrorMessagePerson"] as string : ""; 
            if (tempPerson != null)
            {
                TempData.Remove("person");
            }
            if (TempData["ErrorMessagePerson"] != null)
            {
                if (errorMessagePersonTmp != "")
                {
                    ViewData["EditError"] = errorMessagePersonTmp;
                }
                TempData.Remove("ErrorMessagePerson");
            }

            var model = db.Person.OrderByDescending(p => p.id);
            //foreach (var v in model)
            //{
            //    v.nameRols = string.Join(",", v.Rol.Select(x => x.name).ToArray());
            //}

            return PartialView("_PersonsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonDetailPartial(Person person)
        {
            var model = db.Person.FirstOrDefault(i => i.id == person.id);
            return PartialView("_PersonDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonsPartialAddNew(DXPANACEASOFT.Models.Person item, Provider provider, ProviderGeneralData providerGeneralData, 
                                                 
                                                 ProviderGeneralDataEP providerGeneralDataEP, ProviderGeneralDataRise providerGeneralDataRise,
                                                 Employee employee 
                                                , int [] id_roles)
        {
            var model = db.Person;
            
            int idTmp = 0;

            //if (ModelState.IsValid)
            //{
            //ProviderType ptMI = null;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {

                Person tempPerson = (TempData["person"] as Person);

                // ROLES

                if (id_roles != null)
                    {
                        item.Rol = new List<Rol>();
                        foreach (var r in id_roles)
                        {
                            Rol rol = db.Rol.FirstOrDefault(i => i.id == r);
                            if (rol != null)
                            {
                                if(rol.name == "Proveedor")
                                {
                                    item.Provider = new Provider();

                                    #region ADD Provider
                                    //Provider
                                    item.Provider.electronicDocumentIssuance = provider.electronicDocumentIssuance;
                                    //item.Provider.id_paymentTerm = provider.id_paymentTerm;

                                    #endregion

                                    #region ADD ProviderGeneralData
                                    //ProviderGeneralData
                                    item.Provider.ProviderGeneralData = new ProviderGeneralData();
                                    item.Provider.ProviderGeneralData.cod_alternate = providerGeneralData.cod_alternate;
                                    //item.Provider.ProviderGeneralData.tradeName = providerGeneralData.tradeName;
                                    item.Provider.ProviderGeneralData.id_providerType = providerGeneralData.id_providerType;
                                    item.Provider.ProviderGeneralData.accountExecutive = providerGeneralData.accountExecutive;
                                    item.Provider.ProviderGeneralData.observation = providerGeneralData.observation;
                                    item.Provider.ProviderGeneralData.reference = providerGeneralData.reference;
                                    item.Provider.ProviderGeneralData.id_origin = providerGeneralData.id_origin;
                                    item.Provider.ProviderGeneralData.id_country = providerGeneralData.id_country;
                                    item.Provider.ProviderGeneralData.id_city = providerGeneralData.id_city;
                                    //item.Provider.ProviderGeneralData.id_refCountry = providerGeneralData.id_refCountry;
                                    item.Provider.ProviderGeneralData.id_stateOfContry = providerGeneralData.id_stateOfContry;
                                    item.Provider.ProviderGeneralData.id_economicGroup = providerGeneralData.id_economicGroup;
                                    item.Provider.ProviderGeneralData.noFax = providerGeneralData.noFax;
                                    item.Provider.ProviderGeneralData.phoneNumber1 = providerGeneralData.phoneNumber1;
                                    item.Provider.ProviderGeneralData.phoneNumber2 = providerGeneralData.phoneNumber2;
                                    item.Provider.ProviderGeneralData.cellPhoneNumber = providerGeneralData.cellPhoneNumber;
                                    item.Provider.ProviderGeneralData.contactName = providerGeneralData.contactName;
                                    item.Provider.ProviderGeneralData.contactPhoneNumber = providerGeneralData.contactPhoneNumber;
                                    item.Provider.ProviderGeneralData.contactNoFax = providerGeneralData.contactNoFax;
                                    item.Provider.ProviderGeneralData.electronicPayment = providerGeneralData.electronicPayment;
                                    item.Provider.ProviderGeneralData.isCRAFTSMAN = providerGeneralData.isCRAFTSMAN;
                                    if (providerGeneralData.electronicPayment)
                                    {
                                        #region ADD ProviderGeneralDataEP
                                        //ProviderGeneralDataEP
                                        item.Provider.ProviderGeneralDataEP = new ProviderGeneralDataEP();

                                        item.Provider.ProviderGeneralDataEP.id_identificationTypeEP = providerGeneralDataEP.id_identificationTypeEP;
                                        item.Provider.ProviderGeneralDataEP.id_paymentMethodEP = providerGeneralDataEP.id_paymentMethodEP;
                                        item.Provider.ProviderGeneralDataEP.id_bankToBelieve = providerGeneralDataEP.id_bankToBelieve;
                                        item.Provider.ProviderGeneralDataEP.id_accountTypeGeneralEP = providerGeneralDataEP.id_accountTypeGeneralEP;
                                        item.Provider.ProviderGeneralDataEP.noAccountEP = providerGeneralDataEP.noAccountEP;
                                        item.Provider.ProviderGeneralDataEP.noRefTransfer = providerGeneralDataEP.noRefTransfer;
                                        item.Provider.ProviderGeneralDataEP.id_rtInternational = providerGeneralDataEP.id_rtInternational;
                                        item.Provider.ProviderGeneralDataEP.noRoute = providerGeneralDataEP.noRoute;
                                        item.Provider.ProviderGeneralDataEP.emailEP = providerGeneralDataEP.emailEP;

                                        #endregion
                                    }
                                    item.Provider.ProviderGeneralData.rise = providerGeneralData.rise;
                                    if (providerGeneralData.rise)
                                    {
                                        #region ADD Rise
                                        //Rise
                                        item.Provider.ProviderGeneralDataRise = new ProviderGeneralDataRise();

                                        item.Provider.ProviderGeneralDataRise.id_categoryRise = providerGeneralDataRise.id_categoryRise;
                                        item.Provider.ProviderGeneralDataRise.id_activityRise = providerGeneralDataRise.id_activityRise;
                                        item.Provider.ProviderGeneralDataRise.invoiceAmountRise = providerGeneralDataRise.invoiceAmountRise;

                                        #endregion
                                    }
                                    item.Provider.ProviderGeneralData.specialTaxPayer = providerGeneralData.specialTaxPayer;
                                    item.Provider.ProviderGeneralData.forcedToKeepAccounts = providerGeneralData.forcedToKeepAccounts;
                                    item.Provider.ProviderGeneralData.habitualExporter = providerGeneralData.habitualExporter;
                                    item.Provider.ProviderGeneralData.taxHavenExporter = providerGeneralData.taxHavenExporter;
                                    item.Provider.ProviderGeneralData.sponsoredLinks = providerGeneralData.sponsoredLinks;
                                    
                                    item.Provider.ProviderGeneralData.id_typeBoxCardAndBankAD = providerGeneralData.id_typeBoxCardAndBankAD;
                                    item.Provider.ProviderGeneralData.id_boxCardAndBankAD = providerGeneralData.id_boxCardAndBankAD;

                                    
                                    #endregion

                                    #region ADD ProviderPaymentTerms

                                    if (tempPerson?.Provider?.ProviderPaymentTerm != null)
                                    {
                                        item.Provider.ProviderPaymentTerm = new List<ProviderPaymentTerm>();

                                        var providerPaymentTerms = tempPerson.Provider.ProviderPaymentTerm.ToList();

                                        foreach (var providerPaymentTerm in providerPaymentTerms)
                                        {
                                            var tempProviderPaymentTerm = new ProviderPaymentTerm
                                            {
                                                id_company = providerPaymentTerm.id_company,
                                                id_division = providerPaymentTerm.id_division,
                                                id_branchOffice = providerPaymentTerm.id_branchOffice,
                                                id_paymentTerm = providerPaymentTerm.id_paymentTerm,
                                                isPredetermined = providerPaymentTerm.isPredetermined,
                                                isActive = providerPaymentTerm.isActive
                                            };
                                            item.Provider.ProviderPaymentTerm.Add(tempProviderPaymentTerm);
                                        }
                                    }

                                    #endregion


                                    #region ADD ProviderPaymentMethods

                                    if (tempPerson?.Provider?.ProviderPaymentMethod != null)
                                    {
                                        item.Provider.ProviderPaymentMethod = new List<ProviderPaymentMethod>();

                                        var providerPaymentMethods = tempPerson.Provider.ProviderPaymentMethod.ToList();

                                        foreach (var providerPaymentMethod in providerPaymentMethods)
                                        {
                                            var tempProviderPaymentMethod = new ProviderPaymentMethod
                                            {
                                                id_company = providerPaymentMethod.id_company,
                                                id_division = providerPaymentMethod.id_division,
                                                id_branchOffice = providerPaymentMethod.id_branchOffice,
                                                id_paymentMethod = providerPaymentMethod.id_paymentMethod,
                                                isPredetermined = providerPaymentMethod.isPredetermined,
                                                isActive = providerPaymentMethod.isActive
                                            };
                                            item.Provider.ProviderPaymentMethod.Add(tempProviderPaymentMethod);
                                        }
                                    }

                                    #endregion

                                    #region ADD ProviderSeriesForDocumentss

                                    if (tempPerson?.Provider?.ProviderSeriesForDocuments != null)
                                    {
                                        item.Provider.ProviderSeriesForDocuments = new List<ProviderSeriesForDocuments>();

                                        var providerSeriesForDocumentss = tempPerson.Provider.ProviderSeriesForDocuments.ToList();

                                        foreach (var providerSeriesForDocuments in providerSeriesForDocumentss)
                                        {
                                            var tempProviderSeriesForDocuments = new ProviderSeriesForDocuments
                                            {
                                                id_documentType = providerSeriesForDocuments.id_documentType,
                                                serialNumber = providerSeriesForDocuments.serialNumber,
                                                authorizationNumber = providerSeriesForDocuments.authorizationNumber,
                                                initialNumber = providerSeriesForDocuments.initialNumber,
                                                finalNumber = providerSeriesForDocuments.finalNumber,
                                                dateOfExpiry = providerSeriesForDocuments.dateOfExpiry,
                                                currentNumber = providerSeriesForDocuments.currentNumber,
                                                id_retentionSeriesForDocumentsType = providerSeriesForDocuments.id_retentionSeriesForDocumentsType,
                                                isActive = providerSeriesForDocuments.isActive
                                            };
                                            item.Provider.ProviderSeriesForDocuments.Add(tempProviderSeriesForDocuments);
                                        }
                                    }

                                    #endregion

                                    #region ADD ProviderRetentions

                                    if (tempPerson?.Provider?.ProviderRetention != null)
                                    {
                                        item.Provider.ProviderRetention = new List<ProviderRetention>();

                                        var providerRetentions = tempPerson.Provider.ProviderRetention.ToList();

                                        foreach (var providerRetention in providerRetentions)
                                        {
                                            var tempProviderRetention = new ProviderRetention
                                            {
                                                id_retentionType = providerRetention.id_retentionType,
                                                id_retentionGroup = providerRetention.id_retentionGroup,
                                                id_retention = providerRetention.id_retention,
                                                percentRetencion = providerRetention.percentRetencion
                                            };
                                            item.Provider.ProviderRetention.Add(tempProviderRetention);
                                        }
                                    }

                                    #endregion

                                    #region ADD ProviderPersonAuthorizedToPayTheBills

                                    if (tempPerson?.Provider?.ProviderPersonAuthorizedToPayTheBill != null)
                                    {
                                        item.Provider.ProviderPersonAuthorizedToPayTheBill = new List<ProviderPersonAuthorizedToPayTheBill>();

                                        var providerPersonAuthorizedToPayTheBills = tempPerson.Provider.ProviderPersonAuthorizedToPayTheBill.ToList();

                                        foreach (var providerPersonAuthorizedToPayTheBill in providerPersonAuthorizedToPayTheBills)
                                        {
                                            var tempProviderPersonAuthorizedToPayTheBill = new ProviderPersonAuthorizedToPayTheBill
                                            {
                                                id_identificationType = providerPersonAuthorizedToPayTheBill.id_identificationType,
                                                identification_number = providerPersonAuthorizedToPayTheBill.identification_number,
                                                name = providerPersonAuthorizedToPayTheBill.name,
                                                address = providerPersonAuthorizedToPayTheBill.address,
                                                phoneNumber1 = providerPersonAuthorizedToPayTheBill.phoneNumber1,
                                                phoneNumber2 = providerPersonAuthorizedToPayTheBill.phoneNumber2,
                                                typeReg = providerPersonAuthorizedToPayTheBill.typeReg,
                                                codeProd = providerPersonAuthorizedToPayTheBill.codeProd,
                                                codeEmpr = providerPersonAuthorizedToPayTheBill.codeEmpr,
                                                type = providerPersonAuthorizedToPayTheBill.type,
                                                id_country = providerPersonAuthorizedToPayTheBill.id_country,
                                                id_bank = providerPersonAuthorizedToPayTheBill.id_bank,
                                                id_accountType = providerPersonAuthorizedToPayTheBill.id_accountType,
                                                noAccount = providerPersonAuthorizedToPayTheBill.noAccount,
                                                amount = providerPersonAuthorizedToPayTheBill.amount,
                                                noPayments = providerPersonAuthorizedToPayTheBill.noPayments,
                                                date = providerPersonAuthorizedToPayTheBill.date
                                            };
                                            item.Provider.ProviderPersonAuthorizedToPayTheBill.Add(tempProviderPersonAuthorizedToPayTheBill);
                                        }
                                    }

                                    #endregion

                                    #region ADD ProviderRelatedCompanys

                                    if (tempPerson?.Provider?.ProviderRelatedCompany != null)
                                    {
                                        item.Provider.ProviderRelatedCompany = new List<ProviderRelatedCompany>();

                                        var providerRelatedCompanys = tempPerson.Provider.ProviderRelatedCompany.ToList();

                                        foreach (var providerRelatedCompany in providerRelatedCompanys)
                                        {
                                            var tempProviderRelatedCompany = new ProviderRelatedCompany
                                            {
                                                id_company = providerRelatedCompany.id_company,
                                                id_division = providerRelatedCompany.id_division,
                                                id_branchOffice = providerRelatedCompany.id_branchOffice
                                            };
                                            item.Provider.ProviderRelatedCompany.Add(tempProviderRelatedCompany);
                                        }
                                    }

                                    #endregion

                                    #region ADD ProviderAccountingAccountss

                                    if (tempPerson?.Provider?.ProviderAccountingAccounts != null)
                                    {
                                        item.Provider.ProviderAccountingAccounts = new List<ProviderAccountingAccounts>();

                                        var providerAccountingAccountss = tempPerson.Provider.ProviderAccountingAccounts.ToList();

                                        foreach (var providerAccountingAccounts in providerAccountingAccountss)
                                        {
                                            var tempProviderAccountingAccounts = new ProviderAccountingAccounts
                                            {
                                                id_company = providerAccountingAccounts.id_company,
                                                id_division = providerAccountingAccounts.id_division,
                                                id_branchOffice = providerAccountingAccounts.id_branchOffice,
                                                id_accountFor = providerAccountingAccounts.id_accountFor,
                                                id_accountPlan = providerAccountingAccounts.id_accountPlan,
                                                id_account = providerAccountingAccounts.id_account,
                                                id_accountingAssistantDetailType = providerAccountingAccounts.id_accountingAssistantDetailType
                                            };
                                            item.Provider.ProviderAccountingAccounts.Add(tempProviderAccountingAccounts);
                                        }
                                    }

                                    #endregion

                                    #region ADD ProviderMailByComDivBras

                                    if (tempPerson?.Provider?.ProviderMailByComDivBra != null)
                                    {
                                        item.Provider.ProviderMailByComDivBra = new List<ProviderMailByComDivBra>();

                                        var providerMailByComDivBras = tempPerson.Provider.ProviderMailByComDivBra.ToList();

                                        foreach (var providerMailByComDivBra in providerMailByComDivBras)
                                        {
                                            var tempProviderMailByComDivBra = new ProviderMailByComDivBra
                                            {
                                                id_company = providerMailByComDivBra.id_company,
                                                id_division = providerMailByComDivBra.id_division,
                                                id_branchOffice = providerMailByComDivBra.id_branchOffice,
                                                email = providerMailByComDivBra.email
                                            };
                                            item.Provider.ProviderMailByComDivBra.Add(tempProviderMailByComDivBra);
                                        }
                                    }

                                #endregion


                                    
                                  
                                }

                                if (rol.name == "Cliente Local")
                                {
                                    
                                    

                                  }

                                  item.Rol.Add(rol);
                            }
                        }
                        // TODO:
                        if(employee != null && employee.id_department != 0)
                        {
                            item.Employee = employee;    
                        }                            
                            
                    }
                            
                    // CAMPOS DE AUDITORIA 
                    item.id_userCreate = ActiveUser.id;
                    item.dateCreate = DateTime.Now;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;
                    
                    // FORMA DE QUEMAR EL ID DEL LA COMPANIA
                    item.id_company = ActiveCompany.id;

                    model.Add(item);
                    db.SaveChanges();
                    trans.Commit();
                    
                    idTmp = item.id;
                    
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }
            var result = new
            {
                idPerson = idTmp
            };
            return Json(result, JsonRequestBehavior.AllowGet);
            //return PartialView("_PersonsPartial", model.OrderByDescending(p => p.id).ToList());
        }

        
        [HttpPost, ValidateInput(false)]
        public ActionResult PersonsPartialUpdate(int id_person, DXPANACEASOFT.Models.Person item, Provider provider, ProviderGeneralData providerGeneralData, 
                                                
                                                 ProviderGeneralDataEP providerGeneralDataEP, ProviderGeneralDataRise providerGeneralDataRise,
                                                 Employee employee,
                                                 int[] id_roles)
        {

            var model = db.Person;
            var modelItem = model.FirstOrDefault(it => it.id == id_person);

            bool hadRolProvider = false;
            hadRolProvider = (modelItem?.Rol.FirstOrDefault(fod => fod.name == "Proveedor") != null) ? true : false;
            bool hadRolCustomerL = false;
            hadRolCustomerL = (modelItem?.Rol.FirstOrDefault(fod => fod.name == "Cliente Local") != null) ? true : false;

            int idTmp = 0;
           

            //ProviderType ptMI = null;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    
                    if (modelItem != null)
                    {
                        Person tempPerson = (TempData["person"] as Person);
                        // UPDATE ROLES

                        if (id_roles != null)
                        {
                            for (int i = modelItem.Rol.Count - 1; i >= 0; i--)
                            {
                                Rol rol = modelItem.Rol.ElementAt(i);
                                if (!id_roles.Contains(rol.id))
                                    modelItem.Rol.Remove(rol);
                            }

                            foreach (var id in id_roles)
                            {
                                var _rol = db.Rol.FirstOrDefault(r => r.id == id);
                                var rol = modelItem.Rol.FirstOrDefault(r => r.id == id);
                                if (rol == null)
                                {

                                    modelItem.Rol.Add(_rol);
                                    
                                }

                                if (_rol.name == "Proveedor")
                                {
                                    //Provider _provider = db.Provider.FirstOrDefault(p => p.id == modelItem.id);

                                    //Provider
                                    if (modelItem.Provider == null)
                                    {
                                        modelItem.Provider = new Provider();
                                        modelItem.Provider.id = modelItem.id;
                                        db.Provider.Attach(modelItem.Provider);
                                        db.Entry(modelItem.Provider).State = EntityState.Added;
                                    }

                                    //ProviderGeneralData
                                    if (modelItem.Provider.ProviderGeneralData == null)
                                    {
                                        modelItem.Provider.ProviderGeneralData = new ProviderGeneralData();
                                        modelItem.Provider.ProviderGeneralData.id_provider = modelItem.Provider.id;
                                        db.ProviderGeneralData.Attach(modelItem.Provider.ProviderGeneralData);
                                        db.Entry(modelItem.Provider.ProviderGeneralData).State = EntityState.Added;
                                    }
                                    //ProviderTypeShrimp


                                    //ProviderGeneralDataEP
                                    var providerGeneralDataEPAux = new ProviderGeneralDataEP();
                                    if (providerGeneralData.electronicPayment)
                                    {
                                        if (modelItem.Provider.ProviderGeneralDataEP == null)
                                        {
                                            providerGeneralDataEPAux.id_provider = modelItem.Provider.id;
                                            db.ProviderGeneralDataEP.Add(providerGeneralDataEPAux);

                                        }
                                        else
                                        {
                                            providerGeneralDataEPAux = modelItem.Provider.ProviderGeneralDataEP;
                                            db.ProviderGeneralDataEP.Attach(providerGeneralDataEPAux);
                                            db.Entry(providerGeneralDataEPAux).State = EntityState.Modified;
                                        }
                                    }

                                    //ProviderGeneralDataRise
                                    var providerGeneralDataRiseAux = new ProviderGeneralDataRise();
                                    if (providerGeneralData.rise)
                                    {
                                        if (modelItem.Provider.ProviderGeneralDataRise == null)
                                        {
                                            providerGeneralDataRiseAux.id_provider = modelItem.Provider.id;
                                            db.ProviderGeneralDataRise.Add(providerGeneralDataRiseAux);

                                        }
                                        else
                                        {
                                            providerGeneralDataRiseAux = modelItem.Provider.ProviderGeneralDataRise;
                                            db.ProviderGeneralDataRise.Attach(providerGeneralDataRiseAux);
                                            db.Entry(providerGeneralDataRiseAux).State = EntityState.Modified;
                                        }
                                    }


                                    #region UPDATE Provider
                                    //Provider
                                    modelItem.Provider.electronicDocumentIssuance = provider.electronicDocumentIssuance;

                                    #endregion

                                    #region UPDATE ProviderGeneralData
                                    //ProviderGeneralData
                                    modelItem.Provider.ProviderGeneralData.cod_alternate = providerGeneralData.cod_alternate;
                                    //modelItem.Provider.ProviderGeneralData.tradeName = providerGeneralData.tradeName;
                                    modelItem.Provider.ProviderGeneralData.id_providerType = providerGeneralData.id_providerType;
                                    modelItem.Provider.ProviderGeneralData.accountExecutive = providerGeneralData.accountExecutive;
                                    modelItem.Provider.ProviderGeneralData.observation = providerGeneralData.observation;
                                    modelItem.Provider.ProviderGeneralData.reference = providerGeneralData.reference;
                                    modelItem.Provider.ProviderGeneralData.id_origin = providerGeneralData.id_origin;
                                    modelItem.Provider.ProviderGeneralData.id_country = providerGeneralData.id_country;
                                    modelItem.Provider.ProviderGeneralData.id_city = providerGeneralData.id_city;
                                    modelItem.Provider.ProviderGeneralData.cellPhoneNumber = providerGeneralData.cellPhoneNumber;
                                    //modelItem.Provider.ProviderGeneralData.id_refCountry = providerGeneralData.id_refCountry;
                                    modelItem.Provider.ProviderGeneralData.id_stateOfContry = providerGeneralData.id_stateOfContry;
                                    modelItem.Provider.ProviderGeneralData.noFax = providerGeneralData.noFax;
                                    modelItem.Provider.ProviderGeneralData.id_economicGroup = providerGeneralData.id_economicGroup;
                                    modelItem.Provider.ProviderGeneralData.phoneNumber1 = providerGeneralData.phoneNumber1;
                                    modelItem.Provider.ProviderGeneralData.phoneNumber2 = providerGeneralData.phoneNumber2;
                                    modelItem.Provider.ProviderGeneralData.contactName = providerGeneralData.contactName;
                                    modelItem.Provider.ProviderGeneralData.contactPhoneNumber = providerGeneralData.contactPhoneNumber;
                                    modelItem.Provider.ProviderGeneralData.contactNoFax = providerGeneralData.contactNoFax;
                                    modelItem.Provider.ProviderGeneralData.electronicPayment = providerGeneralData.electronicPayment;
                                    modelItem.Provider.ProviderGeneralData.isCRAFTSMAN = providerGeneralData.isCRAFTSMAN;
                                    if (providerGeneralData.electronicPayment)
                                    {
                                        #region UPDATE ProviderGeneralDataEP
                                        //ProviderGeneralDataEP


                                        providerGeneralDataEPAux.id_identificationTypeEP = providerGeneralDataEP.id_identificationTypeEP;
                                        providerGeneralDataEPAux.id_paymentMethodEP = providerGeneralDataEP.id_paymentMethodEP;
                                        providerGeneralDataEPAux.id_bankToBelieve = providerGeneralDataEP.id_bankToBelieve;
                                        providerGeneralDataEPAux.id_accountTypeGeneralEP = providerGeneralDataEP.id_accountTypeGeneralEP;
                                        providerGeneralDataEPAux.noAccountEP = providerGeneralDataEP.noAccountEP;
                                        providerGeneralDataEPAux.noRefTransfer = providerGeneralDataEP.noRefTransfer;
                                        providerGeneralDataEPAux.id_rtInternational = providerGeneralDataEP.id_rtInternational;
                                        providerGeneralDataEPAux.noRoute = providerGeneralDataEP.noRoute;
                                        providerGeneralDataEPAux.emailEP = providerGeneralDataEP.emailEP;

                                        //if (modelItem.Provider.ProviderGeneralDataEP != null)
                                        //{
                                        //    db.ProviderGeneralDataEP.Attach(providerGeneralDataEPAux);
                                        //    db.Entry(providerGeneralDataEPAux).State = EntityState.Modified;
                                        //}

                                        #endregion
                                    }
                                    modelItem.Provider.ProviderGeneralData.rise = providerGeneralData.rise;
                                    if (providerGeneralData.rise)
                                    {
                                        #region ADD Rise
                                        //Rise
                                        providerGeneralDataRiseAux.id_categoryRise = providerGeneralDataRise.id_categoryRise;
                                        providerGeneralDataRiseAux.id_activityRise = providerGeneralDataRise.id_activityRise;
                                        providerGeneralDataRiseAux.invoiceAmountRise = providerGeneralDataRise.invoiceAmountRise;

                                        #endregion
                                    }
                                    modelItem.Provider.ProviderGeneralData.specialTaxPayer = providerGeneralData.specialTaxPayer;
                                    modelItem.Provider.ProviderGeneralData.forcedToKeepAccounts = providerGeneralData.forcedToKeepAccounts;
                                    modelItem.Provider.ProviderGeneralData.habitualExporter = providerGeneralData.habitualExporter;
                                    modelItem.Provider.ProviderGeneralData.taxHavenExporter = providerGeneralData.taxHavenExporter;
                                    modelItem.Provider.ProviderGeneralData.sponsoredLinks = providerGeneralData.sponsoredLinks;
                                    modelItem.Provider.ProviderGeneralData.id_typeBoxCardAndBankAD = providerGeneralData.id_typeBoxCardAndBankAD;
                                    modelItem.Provider.ProviderGeneralData.id_boxCardAndBankAD = providerGeneralData.id_boxCardAndBankAD;
                                    #endregion

                                    #region UPDATE ProviderPaymentTerms

                                    if (tempPerson?.Provider?.ProviderPaymentTerm != null)
                                    {
                                        if (modelItem.Provider.ProviderPaymentTerm != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderPaymentTerm.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderPaymentTerm.ElementAt(i);
                                                modelItem.Provider.ProviderPaymentTerm.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderPaymentTerm = new List<ProviderPaymentTerm>();
                                        }

                                        var providerPaymentTerms = tempPerson.Provider.ProviderPaymentTerm.ToList();

                                        foreach (var providerPaymentTerm in providerPaymentTerms)
                                        {
                                            var tempProviderPaymentTerm = new ProviderPaymentTerm
                                            {
                                                id_company = providerPaymentTerm.id_company,
                                                id_division = providerPaymentTerm.id_division,
                                                id_branchOffice = providerPaymentTerm.id_branchOffice,
                                                id_paymentTerm = providerPaymentTerm.id_paymentTerm,
                                                isPredetermined = providerPaymentTerm.isPredetermined,
                                                isActive = providerPaymentTerm.isActive
                                            };
                                            modelItem.Provider.ProviderPaymentTerm.Add(tempProviderPaymentTerm);
                                        }
                                    }

                                    #endregion

                                    #region UPDATE ProviderPaymentMethods

                                    if (tempPerson?.Provider?.ProviderPaymentMethod != null)
                                    {
                                        if (modelItem.Provider.ProviderPaymentMethod != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderPaymentMethod.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderPaymentMethod.ElementAt(i);
                                                modelItem.Provider.ProviderPaymentMethod.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderPaymentMethod = new List<ProviderPaymentMethod>();
                                        }

                                        var providerPaymentMethods = tempPerson.Provider.ProviderPaymentMethod.ToList();

                                        foreach (var providerPaymentMethod in providerPaymentMethods)
                                        {
                                            var tempProviderPaymentMethod = new ProviderPaymentMethod
                                            {
                                                id_company = providerPaymentMethod.id_company,
                                                id_division = providerPaymentMethod.id_division,
                                                id_branchOffice = providerPaymentMethod.id_branchOffice,
                                                id_paymentMethod = providerPaymentMethod.id_paymentMethod,
                                                isPredetermined = providerPaymentMethod.isPredetermined,
                                                isActive = providerPaymentMethod.isActive
                                            };
                                            modelItem.Provider.ProviderPaymentMethod.Add(tempProviderPaymentMethod);
                                        }
                                    }

                                    #endregion

                                    #region UPDATE ProviderSeriesForDocumentss

                                    if (tempPerson?.Provider?.ProviderSeriesForDocuments != null)
                                    {
                                        if (modelItem.Provider.ProviderSeriesForDocuments != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderSeriesForDocuments.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderSeriesForDocuments.ElementAt(i);
                                                modelItem.Provider.ProviderSeriesForDocuments.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderSeriesForDocuments = new List<ProviderSeriesForDocuments>();
                                        }

                                        var providerSeriesForDocumentss = tempPerson.Provider.ProviderSeriesForDocuments.ToList();

                                        foreach (var providerSeriesForDocuments in providerSeriesForDocumentss)
                                        {
                                            var tempProviderSeriesForDocuments = new ProviderSeriesForDocuments
                                            {
                                                id_documentType = providerSeriesForDocuments.id_documentType,
                                                serialNumber = providerSeriesForDocuments.serialNumber,
                                                authorizationNumber = providerSeriesForDocuments.authorizationNumber,
                                                initialNumber = providerSeriesForDocuments.initialNumber,
                                                finalNumber = providerSeriesForDocuments.finalNumber,
                                                dateOfExpiry = providerSeriesForDocuments.dateOfExpiry,
                                                currentNumber = providerSeriesForDocuments.currentNumber,
                                                id_retentionSeriesForDocumentsType = providerSeriesForDocuments.id_retentionSeriesForDocumentsType,
                                                isActive = providerSeriesForDocuments.isActive
                                            };
                                            modelItem.Provider.ProviderSeriesForDocuments.Add(tempProviderSeriesForDocuments);
                                        }
                                    }

                                    #endregion

                                    #region UPDATE ProviderRetentions

                                    if (tempPerson?.Provider?.ProviderRetention != null)
                                    {
                                        if (modelItem.Provider.ProviderRetention != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderRetention.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderRetention.ElementAt(i);
                                                modelItem.Provider.ProviderRetention.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderRetention = new List<ProviderRetention>();
                                        }


                                        var providerRetentions = tempPerson.Provider.ProviderRetention.ToList();

                                        foreach (var providerRetention in providerRetentions)
                                        {
                                            var tempProviderRetention = new ProviderRetention
                                            {
                                                id_retentionType = providerRetention.id_retentionType,
                                                id_retentionGroup = providerRetention.id_retentionGroup,
                                                id_retention = providerRetention.id_retention,
                                                percentRetencion = providerRetention.percentRetencion
                                            };
                                            modelItem.Provider.ProviderRetention.Add(tempProviderRetention);
                                        }
                                    }

                                    #endregion

                                    #region UPDATE ProviderPersonAuthorizedToPayTheBills

                                    if (tempPerson?.Provider?.ProviderPersonAuthorizedToPayTheBill != null)
                                    {
                                        if (modelItem.Provider.ProviderPersonAuthorizedToPayTheBill != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderPersonAuthorizedToPayTheBill.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderPersonAuthorizedToPayTheBill.ElementAt(i);
                                                modelItem.Provider.ProviderPersonAuthorizedToPayTheBill.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderPersonAuthorizedToPayTheBill = new List<ProviderPersonAuthorizedToPayTheBill>();
                                        }


                                        var providerPersonAuthorizedToPayTheBills = tempPerson.Provider.ProviderPersonAuthorizedToPayTheBill.ToList();

                                        foreach (var providerPersonAuthorizedToPayTheBill in providerPersonAuthorizedToPayTheBills)
                                        {
                                            var tempProviderPersonAuthorizedToPayTheBill = new ProviderPersonAuthorizedToPayTheBill
                                            {
                                                id_identificationType = providerPersonAuthorizedToPayTheBill.id_identificationType,
                                                identification_number = providerPersonAuthorizedToPayTheBill.identification_number,
                                                name = providerPersonAuthorizedToPayTheBill.name,
                                                address = providerPersonAuthorizedToPayTheBill.address,
                                                phoneNumber1 = providerPersonAuthorizedToPayTheBill.phoneNumber1,
                                                phoneNumber2 = providerPersonAuthorizedToPayTheBill.phoneNumber2,
                                                typeReg = providerPersonAuthorizedToPayTheBill.typeReg,
                                                codeProd = providerPersonAuthorizedToPayTheBill.codeProd,
                                                codeEmpr = providerPersonAuthorizedToPayTheBill.codeEmpr,
                                                type = providerPersonAuthorizedToPayTheBill.type,
                                                id_country = providerPersonAuthorizedToPayTheBill.id_country,
                                                id_bank = providerPersonAuthorizedToPayTheBill.id_bank,
                                                id_accountType = providerPersonAuthorizedToPayTheBill.id_accountType,
                                                noAccount = providerPersonAuthorizedToPayTheBill.noAccount,
                                                amount = providerPersonAuthorizedToPayTheBill.amount,
                                                noPayments = providerPersonAuthorizedToPayTheBill.noPayments,
                                                date = providerPersonAuthorizedToPayTheBill.date
                                            };
                                            modelItem.Provider.ProviderPersonAuthorizedToPayTheBill.Add(tempProviderPersonAuthorizedToPayTheBill);
                                        }
                                    }

                                    #endregion

                                    #region UPDATE ProviderRelatedCompanys

                                    if (tempPerson?.Provider?.ProviderRelatedCompany != null)
                                    {
                                        if (modelItem.Provider.ProviderRelatedCompany != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderRelatedCompany.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderRelatedCompany.ElementAt(i);
                                                modelItem.Provider.ProviderRelatedCompany.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderRelatedCompany = new List<ProviderRelatedCompany>();
                                        }


                                        var providerRelatedCompanys = tempPerson.Provider.ProviderRelatedCompany.ToList();

                                        foreach (var providerRelatedCompany in providerRelatedCompanys)
                                        {
                                            var tempProviderRelatedCompany = new ProviderRelatedCompany
                                            {
                                                id_company = providerRelatedCompany.id_company,
                                                id_division = providerRelatedCompany.id_division,
                                                id_branchOffice = providerRelatedCompany.id_branchOffice
                                            };
                                            modelItem.Provider.ProviderRelatedCompany.Add(tempProviderRelatedCompany);
                                        }
                                    }

                                    #endregion

                                    #region UPDATE ProviderAccountingAccountss

                                    if (tempPerson?.Provider?.ProviderAccountingAccounts != null)
                                    {
                                        if (modelItem.Provider.ProviderAccountingAccounts != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderAccountingAccounts.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderAccountingAccounts.ElementAt(i);
                                                modelItem.Provider.ProviderAccountingAccounts.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderAccountingAccounts = new List<ProviderAccountingAccounts>();
                                        }


                                        var providerAccountingAccountss = tempPerson.Provider.ProviderAccountingAccounts.ToList();

                                        foreach (var providerAccountingAccounts in providerAccountingAccountss)
                                        {
                                            var tempProviderAccountingAccounts = new ProviderAccountingAccounts
                                            {
                                                id_company = providerAccountingAccounts.id_company,
                                                id_division = providerAccountingAccounts.id_division,
                                                id_branchOffice = providerAccountingAccounts.id_branchOffice,
                                                id_accountFor = providerAccountingAccounts.id_accountFor,
                                                id_accountPlan = providerAccountingAccounts.id_accountPlan,
                                                id_account = providerAccountingAccounts.id_account,
                                                id_accountingAssistantDetailType = providerAccountingAccounts.id_accountingAssistantDetailType
                                            };
                                            modelItem.Provider.ProviderAccountingAccounts.Add(tempProviderAccountingAccounts);
                                        }
                                    }

                                    #endregion

                                    #region UPDATE ProviderMailByComDivBras

                                    if (tempPerson?.Provider?.ProviderMailByComDivBra != null)
                                    {
                                        if (modelItem.Provider.ProviderMailByComDivBra != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderMailByComDivBra.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderMailByComDivBra.ElementAt(i);
                                                modelItem.Provider.ProviderMailByComDivBra.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderMailByComDivBra = new List<ProviderMailByComDivBra>();
                                        }


                                        var providerMailByComDivBras = tempPerson.Provider.ProviderMailByComDivBra.ToList();

                                        foreach (var providerMailByComDivBra in providerMailByComDivBras)
                                        {
                                            var tempProviderMailByComDivBra = new ProviderMailByComDivBra
                                            {
                                                id_company = providerMailByComDivBra.id_company,
                                                id_division = providerMailByComDivBra.id_division,
                                                id_branchOffice = providerMailByComDivBra.id_branchOffice,
                                                email = providerMailByComDivBra.email
                                            };
                                            modelItem.Provider.ProviderMailByComDivBra.Add(tempProviderMailByComDivBra);
                                        }
                                    }

                                    #endregion

                                }
                                else if (_rol.name == "Cliente Local")
                                {
                                    
                                }
                                
                            }

                        }
                        
                        // UPDATE EMPLOYEE

                        Employee _employee = db.Employee.FirstOrDefault(p => p.id == modelItem.id);

                        if (employee.id_department != 0)
                        {
                            if (_employee != null)
                            {
                                _employee.id_department = employee.id_department;

                                db.Employee.Attach(_employee);
                                db.Entry(_employee).State = EntityState.Modified;
                            }
                            else
                            {
                                modelItem.Employee = modelItem.Employee ?? new Employee();
                                modelItem.Employee.id_department = employee.id_department;
                            }
                        }
                        else
                        {
                            if (_employee != null)
                            {
                                db.Employee.Remove(_employee);
                            }
                        }

                        modelItem.id_personType = item.id_personType;
                        modelItem.id_identificationType = item.id_identificationType;
                        modelItem.identification_number = item.identification_number;
                        modelItem.fullname_businessName = item.fullname_businessName;
                        modelItem.tradeName = item.tradeName; // {RA}
                        modelItem.cellPhoneNumberPerson = item.cellPhoneNumberPerson;
                        modelItem.photo = item.photo;
                        modelItem.address = item.address;
                        modelItem.email = item.email;
                        modelItem.isActive = item.isActive;
                        modelItem.bCC = item.bCC;

                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        db.Person.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        idTmp = id_person;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    TempData["ErrorMessagePerson"] = ErrorMessage("Error al modificar la persona: "+ e.Message);
                    trans.Rollback();
                }
            }
            if (modelItem == null)
            {
                return PersonsPartialAddNew(item, provider, providerGeneralData,  providerGeneralDataEP, providerGeneralDataRise, employee,
                    id_roles);
            }
            
            var result = new
            {
                idPerson = idTmp
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonsPartialDelete(System.Int32 id)
        {
            var model = db.Person;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)

                        item.isActive = false;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                    //model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_PersonsPartial", model.OrderByDescending(m => m.id).ToList());
        }

        [HttpPost]
        public void DeleteSelectedPersons(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var persons = db.Person.Where(i => ids.Contains(i.id));
                        foreach (var person in persons)
                        {
                            person.isActive = false;
                            db.Entry(person).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                }
            }
        }

        #region ProviderPaymentTerm

        [ValidateInput(false)]
        public ActionResult ProviderPaymentTerm(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderPaymentTerm?.ToList() ?? new List<ProviderPaymentTerm>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderPaymentTerm", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPaymentTermAddNew(int id_person, ProviderPaymentTerm providerPaymentTerm)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerPaymentTerm.id = person.Provider.ProviderPaymentTerm.Count() > 0 ? person.Provider.ProviderPaymentTerm.Max(pld => pld.id) + 1 : 1;
                    if (!providerPaymentTerm.isActive) providerPaymentTerm.isPredetermined = false;
                    person.Provider.ProviderPaymentTerm.Add(providerPaymentTerm);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderPaymentTerm?.ToList() ?? new List<ProviderPaymentTerm>();
            return PartialView("_FormEditProviderPaymentTerm", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPaymentTermUpdate(ProviderPaymentTerm providerPaymentTerm)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerPaymentTerm.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderPaymentTerm.FirstOrDefault(i => i.id == providerPaymentTerm.id);
                    if (modelPerson != null)
                    {
                        

                        this.UpdateModel(modelPerson);

                        if (!providerPaymentTerm.isActive)
                        {
                            //providerPaymentTerm.isPredetermined = false;
                            modelPerson.isPredetermined = false;
                        }
                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderPaymentTerm?.ToList() ?? new List<ProviderPaymentTerm>();
            return PartialView("_FormEditProviderPaymentTerm", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPaymentTermDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerPaymentTerm = person.Provider.ProviderPaymentTerm.FirstOrDefault(it => it.id == id);
                if (providerPaymentTerm != null)
                    person.Provider.ProviderPaymentTerm.Remove(providerPaymentTerm);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderPaymentTerm?.ToList() ?? new List<ProviderPaymentTerm>();
            return PartialView("_FormEditProviderPaymentTerm", model);
        }


        #endregion

        #region ProviderPaymentMethod

        [ValidateInput(false)]
        public ActionResult ProviderPaymentMethod(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderPaymentMethod?.ToList() ?? new List<ProviderPaymentMethod>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderPaymentMethod", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPaymentMethodAddNew(int id_person, ProviderPaymentMethod providerPaymentMethod)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerPaymentMethod.id = person.Provider.ProviderPaymentMethod.Count() > 0 ? person.Provider.ProviderPaymentMethod.Max(pld => pld.id) + 1 : 1;
                    if (!providerPaymentMethod.isActive) providerPaymentMethod.isPredetermined = false;
                    person.Provider.ProviderPaymentMethod.Add(providerPaymentMethod);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderPaymentMethod?.ToList() ?? new List<ProviderPaymentMethod>();
            return PartialView("_FormEditProviderPaymentMethod", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPaymentMethodUpdate(ProviderPaymentMethod providerPaymentMethod)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerPaymentMethod.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderPaymentMethod.FirstOrDefault(i => i.id == providerPaymentMethod.id);
                    if (modelPerson != null)
                    {


                        this.UpdateModel(modelPerson);

                        if (!providerPaymentMethod.isActive)
                        {
                            //providerPaymentMethod.isPredetermined = false;
                            modelPerson.isPredetermined = false;
                        }
                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderPaymentMethod?.ToList() ?? new List<ProviderPaymentMethod>();
            return PartialView("_FormEditProviderPaymentMethod", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPaymentMethodDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerPaymentMethod = person.Provider.ProviderPaymentMethod.FirstOrDefault(it => it.id == id);
                if (providerPaymentMethod != null)
                    person.Provider.ProviderPaymentMethod.Remove(providerPaymentMethod);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderPaymentMethod?.ToList() ?? new List<ProviderPaymentMethod>();
            return PartialView("_FormEditProviderPaymentMethod", model);
        }


        #endregion

        #region ProviderSeriesForDocuments

        [ValidateInput(false)]
        public ActionResult ProviderSeriesForDocuments(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderSeriesForDocuments?.ToList() ?? new List<ProviderSeriesForDocuments>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderSeriesForDocuments", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderSeriesForDocumentsAddNew(int id_person, ProviderSeriesForDocuments providerSeriesForDocuments)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerSeriesForDocuments.id = person.Provider.ProviderSeriesForDocuments.Count() > 0 ? person.Provider.ProviderSeriesForDocuments.Max(pld => pld.id) + 1 : 1;
                    person.Provider.ProviderSeriesForDocuments.Add(providerSeriesForDocuments);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderSeriesForDocuments?.ToList() ?? new List<ProviderSeriesForDocuments>();
            return PartialView("_FormEditProviderSeriesForDocuments", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderSeriesForDocumentsUpdate(ProviderSeriesForDocuments providerSeriesForDocuments)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerSeriesForDocuments.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderSeriesForDocuments.FirstOrDefault(i => i.id == providerSeriesForDocuments.id);
                    if (modelPerson != null)
                    {


                        this.UpdateModel(modelPerson);

                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderSeriesForDocuments?.ToList() ?? new List<ProviderSeriesForDocuments>();
            return PartialView("_FormEditProviderSeriesForDocuments", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderSeriesForDocumentsDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerSeriesForDocuments = person.Provider.ProviderSeriesForDocuments.FirstOrDefault(it => it.id == id);
                if (providerSeriesForDocuments != null)
                    person.Provider.ProviderSeriesForDocuments.Remove(providerSeriesForDocuments);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderSeriesForDocuments?.ToList() ?? new List<ProviderSeriesForDocuments>();
            return PartialView("_FormEditProviderSeriesForDocuments", model);
        }


        #endregion

        #region ProviderRetention

        [ValidateInput(false)]
        public ActionResult ProviderRetention(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderRetention?.ToList() ?? new List<ProviderRetention>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderRetention", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderRetentionAddNew(int id_person, ProviderRetention providerRetention)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerRetention.id = person.Provider.ProviderRetention.Count() > 0 ? person.Provider.ProviderRetention.Max(pld => pld.id) + 1 : 1;
                    person.Provider.ProviderRetention.Add(providerRetention);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderRetention?.ToList() ?? new List<ProviderRetention>();
            return PartialView("_FormEditProviderRetention", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderRetentionUpdate(ProviderRetention providerRetention)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerRetention.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderRetention.FirstOrDefault(i => i.id == providerRetention.id);
                    if (modelPerson != null)
                    {


                        this.UpdateModel(modelPerson);

                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderRetention?.ToList() ?? new List<ProviderRetention>();
            return PartialView("_FormEditProviderRetention", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderRetentionDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerRetention = person.Provider.ProviderRetention.FirstOrDefault(it => it.id == id);
                if (providerRetention != null)
                    person.Provider.ProviderRetention.Remove(providerRetention);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderRetention?.ToList() ?? new List<ProviderRetention>();
            return PartialView("_FormEditProviderRetention", model);
        }


        #endregion

        #region ProviderPersonAuthorizedToPayTheBill

        [ValidateInput(false)]
        public ActionResult ProviderPersonAuthorizedToPayTheBill(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderPersonAuthorizedToPayTheBill?.ToList() ?? new List<ProviderPersonAuthorizedToPayTheBill>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderPersonAuthorizedToPayTheBill", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPersonAuthorizedToPayTheBillAddNew(int id_person, ProviderPersonAuthorizedToPayTheBill providerPersonAuthorizedToPayTheBill)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerPersonAuthorizedToPayTheBill.id = person.Provider.ProviderPersonAuthorizedToPayTheBill.Count() > 0 ? person.Provider.ProviderPersonAuthorizedToPayTheBill.Max(pld => pld.id) + 1 : 1;
                    person.Provider.ProviderPersonAuthorizedToPayTheBill.Add(providerPersonAuthorizedToPayTheBill);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderPersonAuthorizedToPayTheBill?.ToList() ?? new List<ProviderPersonAuthorizedToPayTheBill>();
            return PartialView("_FormEditProviderPersonAuthorizedToPayTheBill", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPersonAuthorizedToPayTheBillUpdate(ProviderPersonAuthorizedToPayTheBill providerPersonAuthorizedToPayTheBill)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerPersonAuthorizedToPayTheBill.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderPersonAuthorizedToPayTheBill.FirstOrDefault(i => i.id == providerPersonAuthorizedToPayTheBill.id);
                    if (modelPerson != null)
                    {


                        this.UpdateModel(modelPerson);

                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderPersonAuthorizedToPayTheBill?.ToList() ?? new List<ProviderPersonAuthorizedToPayTheBill>();
            return PartialView("_FormEditProviderPersonAuthorizedToPayTheBill", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderPersonAuthorizedToPayTheBillDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerPersonAuthorizedToPayTheBill = person.Provider.ProviderPersonAuthorizedToPayTheBill.FirstOrDefault(it => it.id == id);
                if (providerPersonAuthorizedToPayTheBill != null)
                    person.Provider.ProviderPersonAuthorizedToPayTheBill.Remove(providerPersonAuthorizedToPayTheBill);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderPersonAuthorizedToPayTheBill?.ToList() ?? new List<ProviderPersonAuthorizedToPayTheBill>();
            return PartialView("_FormEditProviderPersonAuthorizedToPayTheBill", model);
        }

        #endregion

        #region ProviderRelatedCompany

        [ValidateInput(false)]
        public ActionResult ProviderRelatedCompany(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderRelatedCompany?.ToList() ?? new List<ProviderRelatedCompany>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderRelatedCompany", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderRelatedCompanyAddNew(int id_person, ProviderRelatedCompany providerRelatedCompany)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerRelatedCompany.id = person.Provider.ProviderRelatedCompany.Count() > 0 ? person.Provider.ProviderRelatedCompany.Max(pld => pld.id) + 1 : 1;
                    person.Provider.ProviderRelatedCompany.Add(providerRelatedCompany);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderRelatedCompany?.ToList() ?? new List<ProviderRelatedCompany>();
            return PartialView("_FormEditProviderRelatedCompany", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderRelatedCompanyUpdate(ProviderRelatedCompany providerRelatedCompany)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerRelatedCompany.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderRelatedCompany.FirstOrDefault(i => i.id == providerRelatedCompany.id);
                    if (modelPerson != null)
                    {


                        this.UpdateModel(modelPerson);

                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderRelatedCompany?.ToList() ?? new List<ProviderRelatedCompany>();
            return PartialView("_FormEditProviderRelatedCompany", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderRelatedCompanyDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerRelatedCompany = person.Provider.ProviderRelatedCompany.FirstOrDefault(it => it.id == id);
                if (providerRelatedCompany != null)
                    person.Provider.ProviderRelatedCompany.Remove(providerRelatedCompany);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderRelatedCompany?.ToList() ?? new List<ProviderRelatedCompany>();
            return PartialView("_FormEditProviderRelatedCompany", model);
        }

        #endregion

        #region ProviderAccountingAccounts

        [ValidateInput(false)]
        public ActionResult ProviderAccountingAccounts(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderAccountingAccounts?.ToList() ?? new List<ProviderAccountingAccounts>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderAccountingAccounts", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderAccountingAccountsAddNew(int id_person, ProviderAccountingAccounts providerAccountingAccounts)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerAccountingAccounts.id = person.Provider.ProviderAccountingAccounts.Count() > 0 ? person.Provider.ProviderAccountingAccounts.Max(pld => pld.id) + 1 : 1;
                    person.Provider.ProviderAccountingAccounts.Add(providerAccountingAccounts);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderAccountingAccounts?.ToList() ?? new List<ProviderAccountingAccounts>();
            return PartialView("_FormEditProviderAccountingAccounts", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderAccountingAccountsUpdate(ProviderAccountingAccounts providerAccountingAccounts)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerAccountingAccounts.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderAccountingAccounts.FirstOrDefault(i => i.id == providerAccountingAccounts.id);
                    if (modelPerson != null)
                    {


                        this.UpdateModel(modelPerson);

                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderAccountingAccounts?.ToList() ?? new List<ProviderAccountingAccounts>();
            return PartialView("_FormEditProviderAccountingAccounts", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderAccountingAccountsDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerAccountingAccounts = person.Provider.ProviderAccountingAccounts.FirstOrDefault(it => it.id == id);
                if (providerAccountingAccounts != null)
                    person.Provider.ProviderAccountingAccounts.Remove(providerAccountingAccounts);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderAccountingAccounts?.ToList() ?? new List<ProviderAccountingAccounts>();
            return PartialView("_FormEditProviderAccountingAccounts", model);
        }

        #endregion

        #region ProviderMailByComDivBra

        [ValidateInput(false)]
        public ActionResult ProviderMailByComDivBra(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderMailByComDivBra?.ToList() ?? new List<ProviderMailByComDivBra>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderMailByComDivBra", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderMailByComDivBraAddNew(int id_person, ProviderMailByComDivBra providerMailByComDivBra)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerMailByComDivBra.id = person.Provider.ProviderMailByComDivBra.Count() > 0 ? person.Provider.ProviderMailByComDivBra.Max(pld => pld.id) + 1 : 1;
                    person.Provider.ProviderMailByComDivBra.Add(providerMailByComDivBra);
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderMailByComDivBra?.ToList() ?? new List<ProviderMailByComDivBra>();
            return PartialView("_FormEditProviderMailByComDivBra", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderMailByComDivBraUpdate(ProviderMailByComDivBra providerMailByComDivBra)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerMailByComDivBra.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderMailByComDivBra.FirstOrDefault(i => i.id == providerMailByComDivBra.id);
                    if (modelPerson != null)
                    {


                        this.UpdateModel(modelPerson);

                    }
                    TempData["person"] = person;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProviderMailByComDivBra?.ToList() ?? new List<ProviderMailByComDivBra>();
            return PartialView("_FormEditProviderMailByComDivBra", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderMailByComDivBraDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerMailByComDivBra = person.Provider.ProviderMailByComDivBra.FirstOrDefault(it => it.id == id);
                if (providerMailByComDivBra != null)
                    person.Provider.ProviderMailByComDivBra.Remove(providerMailByComDivBra);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderMailByComDivBra?.ToList() ?? new List<ProviderMailByComDivBra>();
            return PartialView("_FormEditProviderMailByComDivBra", model);
        }

        #endregion

        #region AXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public string IdentificationTypeCode(int id_identificationType)
        {
            IdentificationType identificationType = db.IdentificationType.FirstOrDefault(i => i.id == id_identificationType);
            return identificationType?.code ?? "";
        }

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            return BinaryImageEditExtension.GetCallbackResult();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult VerifyIdentificationCode(string id_code, int id_person)
        {
            Person person = null;
            if (id_person == 0)
            {
                person = db.Person.FirstOrDefault(fod => fod.identification_number == id_code && fod.isActive);
            }
            else
            {
                person = db.Person.FirstOrDefault(fod => fod.identification_number == id_code && fod.id != id_person && fod.isActive);
            }
             

            var result = new
            {
                isRepeated = person == null ? "NO" : "SI",
                personName = person == null ? string.Empty : person.fullname_businessName
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PersonsGetCodSRIIdentificationType(int? id_identificationType)
        {
            var identificationTypeAux = db.IdentificationType.FirstOrDefault(fod => fod.id == id_identificationType);

            var result = new
            {
                codSRI_IdentificationType = identificationTypeAux?.codeSRI ?? ""

            };

            //TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProviderTypeWhatProviderIs(int? id_providerType)
        {
            var providerType = db.ProviderType.FirstOrDefault(fod => fod.id == id_providerType);
            bool isShrimpPerson = false; 
            bool isTransportist = false;
            if (providerType != null)
            {
                isShrimpPerson = (bool)providerType.isShrimpPerson;
                isTransportist = (bool)providerType.isTransportist;
            }
            var result = new
            {
                isShrimpProvider = isShrimpPerson ? "SI" : "NO",
                isTransportist = isTransportist ? "SI": "NO"
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitComboOrigin(int? id_origin, int? id_country, int? id_city, int? id_stateOfContry)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();

            //var originAux = db.Origin.FirstOrDefault(fod => fod.id == id_origin);

            var countries = db.Country.Where(w => (w.id_origin == id_origin && w.isActive) || w.id == id_country).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var cities = db.City.Where(w => (w.id_country == id_country && w.isActive) || w.id == id_city).Select(s => new
            {
                id = s.id,
                //code = s.code,
                name = s.name
            }).ToList();

            var stateOfContries = db.StateOfContry.Where(w => (w.id_country == id_country && w.isActive) || w.id == id_stateOfContry).Select(s => new
            {
                id = s.id,
                //code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                countries = countries,
                cities = cities,
                stateOfContries = stateOfContries
            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult OriginDetailData(int? id_origin)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();

            //var originAux = db.Origin.FirstOrDefault(fod => fod.id == id_origin);


            var countries = db.Country.Where(w => (w.id_origin == id_origin && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                countries = countries
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CountryDetailData(int? id_country)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();

            Country country = db.Country.FirstOrDefault(fod => fod.id == id_country);

            var cities = db.City.Where(w => (w.id_country == id_country && w.isActive)).Select(s => new
            {
                id = s.id,
                name = s.name
            }).ToList();

            var stateOfContries = db.StateOfContry.Where(w => (w.id_country == id_country && w.isActive)).Select(s => new
            {
                id = s.id,
                name = s.name
            }).ToList();
            

            var result = new
            {
                cities = cities,
                stateOfContries = stateOfContries,
                codeCountry = country?.code ?? string.Empty
            };
            
            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInvoiceAmountRise(int? id_categoryRise, int? id_activityRise)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();

            var categoryActivityRiseAux = db.CategoryActivityRise.FirstOrDefault(fod => fod.id_categoryRise == id_categoryRise && fod.id_activityRise == id_activityRise);


            var result = new
            {
                invoiceAmountRise = categoryActivityRiseAux?.invoiceAmountRise ?? 0
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitComboTypeBoxCardAndBankAD(int? id_typeBoxCardAndBankAD, int? id_boxCardAndBankAD)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();

            //var originAux = db.Origin.FirstOrDefault(fod => fod.id == id_origin);

            var boxCardAndBankADs = db.BoxCardAndBank.Where(w => (w.id_typeBoxCardAndBank == id_typeBoxCardAndBankAD && w.isActive) || w.id == id_boxCardAndBankAD).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                boxCardAndBankADs = boxCardAndBankADs
            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult TypeBoxCardAndBankADDetailData(int? id_typeBoxCardAndBankAD)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();

            //var originAux = db.Origin.FirstOrDefault(fod => fod.id == id_origin);


            var boxCardAndBankADs = db.BoxCardAndBank.Where(w => (w.id_typeBoxCardAndBank == id_typeBoxCardAndBankAD && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                boxCardAndBankADs = boxCardAndBankADs
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedPaymentTerm(int? id_companyNew, int? id_divisionNew, int? id_branchOfficeNew, int? id_paymentTermNew, bool? isPredeterminedNew, bool? onlyBecauseIsPredetermined)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };
            if (isPredeterminedNew ?? false)
            {
                var providerPredeterminedAux = person.Provider.ProviderPaymentTerm.FirstOrDefault(fod => fod.id_company == id_companyNew &&
                                                                                fod.id_division == id_divisionNew &&
                                                                                fod.id_branchOffice == id_branchOfficeNew &&
                                                                                fod.isPredetermined);
                if (providerPredeterminedAux != null)
                {
                    var paymentTermAux = db.PaymentTerm.FirstOrDefault(fod => fod.id == id_paymentTermNew);
                    var companyAux = db.Company.FirstOrDefault(fod => fod.id == id_companyNew);
                    var divisionAux = db.Division.FirstOrDefault(fod => fod.id == id_divisionNew);
                    var branchOfficeAux = db.BranchOffice.FirstOrDefault(fod => fod.id == id_branchOfficeNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede predeterminar dos Plazos de Pago para igual" +
                                ",  compañía: " + companyAux.businessName +
                                ", división: " + divisionAux.name +
                                ", sucursal: " + divisionAux.name + ",  en los detalles."

                    };
                    TempData["person"] = person;
                    TempData.Keep("person");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            if (!(onlyBecauseIsPredetermined ?? false))
            {
                var providerPaymentTermAux = person.Provider.ProviderPaymentTerm.FirstOrDefault(fod => fod.id_company == id_companyNew &&
                                                                                fod.id_division == id_divisionNew &&
                                                                                fod.id_branchOffice == id_branchOfficeNew &&
                                                                                fod.id_paymentTerm == id_paymentTermNew);
                if (providerPaymentTermAux != null)
                {
                    var paymentTermAux = db.PaymentTerm.FirstOrDefault(fod => fod.id == id_paymentTermNew);
                    var companyAux = db.Company.FirstOrDefault(fod => fod.id == id_companyNew);
                    var divisionAux = db.Division.FirstOrDefault(fod => fod.id == id_divisionNew);
                    var branchOfficeAux = db.BranchOffice.FirstOrDefault(fod => fod.id == id_branchOfficeNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir el Plazo de Pago: " + paymentTermAux.name +
                                ",  para la misma compañía: " + companyAux.businessName +
                                ", división: " + divisionAux.name +
                                ", sucursal: " + divisionAux.name + ",  en los detalles."

                    };

                }
            }
            


            TempData["person"] = person;
            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateInput(false)]
        public JsonResult InitProviderPaymentTermCompanyCombo(int? id_company, int? id_division, int? id_branchOffice, int? id_paymentTerm)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var companies = db.Company.Where(w => (w.isActive) || w.id == id_company).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.businessName
            }).ToList();

            var divisions = db.Division.Where(w => (w.id_company == id_company && w.isActive) || w.id == id_division).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var branchOffices = db.BranchOffice.Where(w => (w.id_company == id_company && w.id_division == id_division && w.isActive) || w.id == id_branchOffice).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var paymentTerms = db.PaymentTerm.Where(w => (w.id_company == id_company && w.isActive) || w.id == id_paymentTerm).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                companies = companies,
                divisions = divisions,
                branchOffices = branchOffices,
                paymentTerms = paymentTerms

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult ProviderPaymentTermCompanyDetailData(int? id_company)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var divisions = db.Division.Where(w => (w.id_company == id_company && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var paymentTerms = db.PaymentTerm.Where(w => (w.id_company == id_company && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                divisions = divisions,
                paymentTerms = paymentTerms
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ProviderPaymentMethodDivisionDetailData(int? id_company, int? id_division)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var branchOffices = db.BranchOffice.Where(w => (w.id_company == id_company && w.id_division == id_division && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                branchOffices = branchOffices
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ProviderPaymentTermDivisionDetailData(int? id_company, int? id_division)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var branchOffices = db.BranchOffice.Where(w => (w.id_company == id_company && w.id_division == id_division && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                branchOffices = branchOffices
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedPaymentMethod(int? id_companyNew, int? id_divisionNew, int? id_branchOfficeNew, int? id_paymentMethodNew, bool? isPredeterminedNew, bool? onlyBecauseIsPredetermined)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };
            if (isPredeterminedNew ?? false)
            {
                var providerPredeterminedAux = person.Provider.ProviderPaymentMethod.FirstOrDefault(fod => fod.id_company == id_companyNew &&
                                                                                fod.id_division == id_divisionNew &&
                                                                                fod.id_branchOffice == id_branchOfficeNew &&
                                                                                fod.isPredetermined);
                if (providerPredeterminedAux != null)
                {
                    var paymentMethodAux = db.PaymentMethod.FirstOrDefault(fod => fod.id == id_paymentMethodNew);
                    var companyAux = db.Company.FirstOrDefault(fod => fod.id == id_companyNew);
                    var divisionAux = db.Division.FirstOrDefault(fod => fod.id == id_divisionNew);
                    var branchOfficeAux = db.BranchOffice.FirstOrDefault(fod => fod.id == id_branchOfficeNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede predeterminar dos Formas de Pago para igual" +
                                ",  compañía: " + companyAux.businessName +
                                ", división: " + divisionAux.name +
                                ", sucursal: " + divisionAux.name + ",  en los detalles."

                    };
                    TempData["person"] = person;
                    TempData.Keep("person");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            if (!(onlyBecauseIsPredetermined ?? false))
            {
                var providerPaymentMethodAux = person.Provider.ProviderPaymentMethod.FirstOrDefault(fod => fod.id_company == id_companyNew &&
                                                                                fod.id_division == id_divisionNew &&
                                                                                fod.id_branchOffice == id_branchOfficeNew &&
                                                                                fod.id_paymentMethod == id_paymentMethodNew);
                if (providerPaymentMethodAux != null)
                {
                    var paymentMethodAux = db.PaymentMethod.FirstOrDefault(fod => fod.id == id_paymentMethodNew);
                    var companyAux = db.Company.FirstOrDefault(fod => fod.id == id_companyNew);
                    var divisionAux = db.Division.FirstOrDefault(fod => fod.id == id_divisionNew);
                    var branchOfficeAux = db.BranchOffice.FirstOrDefault(fod => fod.id == id_branchOfficeNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir la Forma de Pago: " + paymentMethodAux.name +
                                ",  para la misma compañía: " + companyAux.businessName +
                                ", división: " + divisionAux.name +
                                ", sucursal: " + divisionAux.name + ",  en los detalles."

                    };

                }
            }



            TempData["person"] = person;
            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitProviderPaymentMethodCompanyCombo(int? id_company, int? id_division, int? id_branchOffice, int? id_paymentMethod)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var companies = db.Company.Where(w => (w.isActive) || w.id == id_company).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.businessName
            }).ToList();

            var divisions = db.Division.Where(w => (w.id_company == id_company && w.isActive) || w.id == id_division).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var branchOffices = db.BranchOffice.Where(w => (w.id_company == id_company && w.id_division == id_division && w.isActive) || w.id == id_branchOffice).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var paymentMethods = db.PaymentMethod.Where(w => (w.id_company == id_company && w.isActive) || w.id == id_paymentMethod).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                companies = companies,
                divisions = divisions,
                branchOffices = branchOffices,
                paymentMethods = paymentMethods

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult ProviderPaymentMethodCompanyDetailData(int? id_company)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var divisions = db.Division.Where(w => (w.id_company == id_company && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var paymentMethods = db.PaymentMethod.Where(w => (w.id_company == id_company && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                divisions = divisions,
                paymentMethods = paymentMethods
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult InitProviderSeriesForDocumentsDocumentTypeCombo(int? id_documentType, int? id_retentionSeriesForDocumentsType)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var documentTypes = db.DocumentType.Where(w => (w.isActive && w.isElectronic) || w.id == id_documentType).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var retentionSeriesForDocumentsTypes = db.RetentionSeriesForDocumentsType.Where(w => (w.isActive) || w.id == id_retentionSeriesForDocumentsType).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                documentTypes = documentTypes,
                retentionSeriesForDocumentsTypes = retentionSeriesForDocumentsTypes

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }


        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedRetention(int? id_retentionNew)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var providerRetentionAux = person.Provider.ProviderRetention.FirstOrDefault(fod => fod.id_retention == id_retentionNew);
            if (providerRetentionAux != null)
            {
                var retentionAux = db.Retention.FirstOrDefault(fod => fod.id == id_retentionNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir la Retención: " + retentionAux.name +
                            ",  en los detalles."

                };

            }



            TempData["person"] = person;
            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitProviderRetentionRetentionTypeCombo(int? id_retentionType, int? id_retentionGroup, int? id_retention)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var retentionTypes = db.RetentionType.Where(w => (w.isActive) || w.id == id_retentionType).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var retentionGroups = db.RetentionGroup.Where(w => (w.isActive) || w.id == id_retentionGroup).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var retentions = db.Retention.Where(w => (w.id_retentionType == id_retentionType && w.id_retentionGroup == id_retentionGroup) || w.id == id_retention).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();


            var result = new
            {
                retentionTypes = retentionTypes,
                retentionGroups = retentionGroups,
                retentions = retentions

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProviderRetentionRetentionCombo(int? id_retentionType, int? id_retentionGroup)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var retentions = db.Retention.Where(w => (w.id_retentionType == id_retentionType && w.id_retentionGroup == id_retentionGroup)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();


            var result = new
            {
                retentions = retentions

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult ProviderRetentionRetentionDetailData(int? id_retention)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var retentionAux = db.Retention.FirstOrDefault(w => (w.id == id_retention));


            var result = new
            {
                percentRetencion = retentionAux?.percentRetencion ?? 0
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitProviderPersonAuthorizedToPayTheBillIdentificationTypeCombo(int? id_identificationType, int? id_country, int? id_bank, int? id_accountType)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var identificationTypes = db.IdentificationType.Where(w => (w.is_Active) || w.id == id_identificationType).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var countries = db.Country.Where(w => (w.isActive) || w.id == id_country).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var banks = db.BoxCardAndBank.Where(w => (w.isActive && w.TypeBoxCardAndBank.code.Equals("BAN")) || w.id == id_bank).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var accountTypes = db.AccountType.Where(w => (w.isActive) || w.id == id_accountType).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var result = new
            {
                identificationTypes = identificationTypes,
                countries = countries,
                banks = banks,
                accountTypes = accountTypes

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedRelatedCompany(int? id_companyNew, int? id_divisionNew, int? id_branchOfficeNew)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

           
            var providerRelatedCompanyAux = person.Provider.ProviderRelatedCompany.FirstOrDefault(fod => fod.id_company == id_companyNew &&
                                                                            fod.id_division == id_divisionNew &&
                                                                            fod.id_branchOffice == id_branchOfficeNew);
            if (providerRelatedCompanyAux != null)
            {
                var companyAux = db.Company.FirstOrDefault(fod => fod.id == id_companyNew);
                var divisionAux = db.Division.FirstOrDefault(fod => fod.id == id_divisionNew);
                var branchOfficeAux = db.BranchOffice.FirstOrDefault(fod => fod.id == id_branchOfficeNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir la Sucursal: " + branchOfficeAux.name +
                            ",  para la misma compañía: " + companyAux.businessName +
                            ", división: " + divisionAux.name + ",  en los detalles."

                };

            }



            TempData["person"] = person;
            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitProviderAccountingAccountsCompanyCombo(int? id_company, int? id_division, int? id_branchOffice, int? id_accountFor, int? id_accountPlan, int? id_account, int? id_accountingAssistantDetailType)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var companies = db.Company.Where(w => (w.isActive) || w.id == id_company).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.businessName
            }).ToList();

            var divisions = db.Division.Where(w => (w.id_company == id_company && w.isActive) || w.id == id_division).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var branchOffices = db.BranchOffice.Where(w => (w.id_company == id_company && w.id_division == id_division && w.isActive) || w.id == id_branchOffice).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var accountFors = db.AccountFor.Where(w => (w.isActive) || w.id == id_accountFor).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var accountPlans = db.AccountPlan.Where(w => (w.isActive) || w.id == id_accountPlan).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();

            var accounts = db.Account.Where(w => (w.id_account_plan == id_accountPlan && w.isActive && w.isMovement) || w.id == id_account).Select(s => new
            {
                id = s.id,
                number = s.number,
                name = s.name
            }).ToList();

            var accountingAssistantDetailTypes = db.AccountingAssistantDetailType.Where(w => (w.isActive && db.AccountDetailAssistantType.FirstOrDefault(fod=> fod.id_account == id_account && fod.id_assistantType == w.id_assistantType) != null) || w.id == id_accountingAssistantDetailType).Select(s => new
            {
                assistantTypeCode = s.AssistantType.code,
                accountingAssistantCode = s.AccountingAssistant.code,
                accountingAssistantName = s.AccountingAssistant.name
            }).ToList();

            var result = new
            {
                companies = companies,
                divisions = divisions,
                branchOffices = branchOffices,
                accountFors = accountFors,
                accountPlans = accountPlans,
                accounts = accounts,
                accountingAssistantDetailTypes = accountingAssistantDetailTypes

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult ProviderAccountingAccountsAccountPlanDetailData(int? id_accountPlan)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var accounts = db.Account.Where(w => (w.id_account_plan == id_accountPlan && w.isActive && w.isMovement)).Select(s => new
            {
                id = s.id,
                number = s.number,
                name = s.name
            }).ToList();

            var result = new
            {
                accounts = accounts
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProviderAccountingAccountsAccountDetailData(int? id_account)
        {

            Person person = (TempData["person"] as Person);

            person = person ?? new Person();


            var accountingAssistantDetailTypes = db.AccountingAssistantDetailType.Where(w => (w.isActive && db.AccountDetailAssistantType.FirstOrDefault(fod => fod.id_account == id_account && fod.id_assistantType == w.id_assistantType) != null)).Select(s => new
            {
                assistantTypeCode = s.AssistantType.code,
                accountingAssistantCode = s.AccountingAssistant.code,
                accountingAssistantName = s.AccountingAssistant.name
            }).ToList();

            var result = new
            {
                accountingAssistantDetailTypes = accountingAssistantDetailTypes
            };


            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}