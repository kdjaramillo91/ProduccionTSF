using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.Logs;

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

            var model = db.Person.OrderByDescending(p => p.id).ToList();
            //foreach (var v in model)
            //{
            //    v.nameRols = string.Join(",", v.Rol.Select(x => x.name).ToArray());
            //}
            return PartialView("_PersonsPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonDetailPartial(Person person)
        {
            var model = db.Person.FirstOrDefault(i => i.id == person.id);
            return PartialView("_PersonDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonsPartialAddNew(DXPANACEASOFT.Models.Person item, Provider provider, ProviderGeneralData providerGeneralData,
                                                 ProviderTypeShrimp providerTypeShrimp, ProviderTransportist providerTransportist,
                                                 ProviderGeneralDataEP providerGeneralDataEP, ProviderGeneralDataRise providerGeneralDataRise,
                                                 Employee employee,
                                                 Customer customer, CustomerContact customerContact, CustomerCreditInfo customerCreditInfo,
                                                 ProviderRawMaterial providerRawMaterial, int[] id_roles)
        {
            var model = db.Person;

            item.isCopacking = false;
            int idTmp = 0;
            DBContextCI dbCI = new DBContextCI();
            TblGeSecuencia tblGeSecuencia = dbCI.TblGeSecuencia.FirstOrDefault(fod => fod.CCiTipoSecu == "PERS");
            tblGeSecuencia = tblGeSecuencia ?? new TblGeSecuencia();
            bool isSaved = false;
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];

            int iSecInicial = 0;
            int iSecFinal = 0;
            //Calculo Secuencia error
            var setErrSH = db.Setting.FirstOrDefault(fod => fod.code == "ESSP" && fod.value == "1");
            if (setErrSH != null)
            {
                var setDetErrSH = setErrSH.SettingDetail.FirstOrDefault(fod => Convert.ToInt32(fod.value) > 0);
                if (setDetErrSH != null)
                {
                    iSecInicial = Convert.ToInt32(setDetErrSH.value);
                    iSecFinal = Convert.ToInt32(setDetErrSH.valueAux);
                }
            }


            decimal NNuSecuenciaMax = tblGeSecuencia.NNuSecuencia;
            NNuSecuenciaMax = NNuSecuenciaMax + 1;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];

            if (NNuSecuenciaMax > 0 && iSecFinal > 0 && iSecInicial > 0)
            {
                if (NNuSecuenciaMax == iSecInicial)
                {
                    NNuSecuenciaMax = iSecFinal;
                }
            }

            //if (ModelState.IsValid)
            //{
            ProviderType ptMI = null;
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
                                if (rol.name == "Proveedor")
                                {
                                    item.Provider = new Provider();

                                    #region ADD Provider
                                    //Provider
                                    item.Provider.electronicDocumentIssuance = provider.electronicDocumentIssuance;
                                    //item.Provider.id_paymentTerm = provider.id_paymentTerm;

                                    #endregion

                                    #region ADD ProviderRawMaterial
                                    item.isCopacking = item.isCopacking ?? false;

                                    if ((bool)item.isCopacking || validarRolExistente("Planta Proceso", id_roles))
                                    {
                                        item.Provider.ProviderRawMaterial =
                                                item.Provider.ProviderRawMaterial ?? new ProviderRawMaterial();

                                        item.Provider.ProviderRawMaterial.id_provider = providerRawMaterial.id_provider;
                                        item.Provider.ProviderRawMaterial.id_Warehouse = providerRawMaterial.id_Warehouse;
                                        item.Provider.ProviderRawMaterial.id_WarehouseLocation = providerRawMaterial.id_WarehouseLocation;
                                        item.Provider.ProviderRawMaterial.id_CostCenter = providerRawMaterial.id_CostCenter;
                                        item.Provider.ProviderRawMaterial.id_SubCostCenter = providerRawMaterial.id_SubCostCenter;

                                        item.Provider.ProviderRawMaterial.id_WarehouseCarton = providerRawMaterial.id_WarehouseCarton;
                                        item.Provider.ProviderRawMaterial.id_warehouseLocationCarton = providerRawMaterial.id_warehouseLocationCarton;
                                        item.Provider.ProviderRawMaterial.id_CostCenterCarton = providerRawMaterial.id_CostCenterCarton;
                                        item.Provider.ProviderRawMaterial.id_SubCostCenterCarton = providerRawMaterial.id_SubCostCenterCarton;

                                        item.Provider.ProviderRawMaterial.id_WarehouseFreeze = providerRawMaterial.id_WarehouseFreeze;
                                        item.Provider.ProviderRawMaterial.id_warehouseLocationFreeze = providerRawMaterial.id_warehouseLocationFreeze;
                                        item.Provider.ProviderRawMaterial.id_CostCenterFreeze = providerRawMaterial.id_CostCenterFreeze;
                                        item.Provider.ProviderRawMaterial.id_SubCostCenterFreeze = providerRawMaterial.id_SubCostCenterFreeze;

                                        item.Provider.ProviderRawMaterial.id_WarehouseLooseCarton = providerRawMaterial.id_WarehouseLooseCarton;
                                        item.Provider.ProviderRawMaterial.id_WarehouseLocationLooseCarton = providerRawMaterial.id_WarehouseLocationLooseCarton;
                                        item.Provider.ProviderRawMaterial.id_CostCenterLooseCarton = providerRawMaterial.id_CostCenterLooseCarton;
                                        item.Provider.ProviderRawMaterial.id_SubCostCenterLooseCarton = providerRawMaterial.id_SubCostCenterLooseCarton;

                                        db.ProviderRawMaterial.Attach(item.Provider.ProviderRawMaterial);
                                        db.Entry(item.Provider.ProviderRawMaterial).State = EntityState.Added;
                                    }

                                    this.ViewBag.rolExistente = validarRolExistente("Planta Proceso", id_roles);
                                    #endregion

                                    #region ADD ProviderGeneralData
                                    //ProviderGeneralData
                                    item.Provider.ProviderGeneralData = new ProviderGeneralData();
                                    item.Provider.ProviderGeneralData.cod_alternate = providerGeneralData.cod_alternate;
                                    item.Provider.ProviderGeneralData.buyerLabel = providerGeneralData.buyerLabel;
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
                                    item.Provider.ProviderGeneralData.plantCode = providerGeneralData.plantCode;
                                    item.Provider.ProviderGeneralData.fda = providerGeneralData.fda;
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

                                    //Shrimp Type Person
                                    ptMI = db.ProviderType.FirstOrDefault(fod => fod.id == providerGeneralData.id_providerType);
                                    ptMI = ptMI ?? new ProviderType();

                                    if ((bool)ptMI.isShrimpPerson)
                                    {
                                        if (providerTypeShrimp != null)
                                        {
                                            item.Provider.ProviderTypeShrimp = new ProviderTypeShrimp();
                                            item.Provider.ProviderTypeShrimp.totalHectareasPool = providerTypeShrimp.totalHectareasPool;
                                            item.Provider.ProviderTypeShrimp.grammageFrom = providerTypeShrimp.grammageFrom;
                                            item.Provider.ProviderTypeShrimp.grammageUpto = providerTypeShrimp.grammageUpto;
                                            item.Provider.ProviderTypeShrimp.totalHectareasPool = providerTypeShrimp.totalHectareasPool;
                                            if (providerTypeShrimp.id_protectiveProvider != null)
                                            {
                                                if (providerTypeShrimp.id_protectiveProvider != 0)
                                                {
                                                    item.Provider.ProviderTypeShrimp.id_protectiveProvider = providerTypeShrimp.id_protectiveProvider;
                                                }
                                                else
                                                {
                                                    item.Provider.ProviderTypeShrimp.id_protectiveProvider = item.id;
                                                }

                                            }
                                            else
                                            {
                                                item.Provider.ProviderTypeShrimp.id_protectiveProvider = item.id;
                                            }
                                            if (providerTypeShrimp.id_buyerProvider != null)
                                            {
                                                if (providerTypeShrimp.id_buyerProvider != 0)
                                                {
                                                    item.Provider.ProviderTypeShrimp.id_buyerProvider = providerTypeShrimp.id_buyerProvider;
                                                }
                                                else
                                                {
                                                    item.Provider.ProviderTypeShrimp.id_buyerProvider = null;
                                                }

                                            }
                                            else
                                            {
                                                item.Provider.ProviderTypeShrimp.id_buyerProvider = null;
                                            }
                                        }
                                    }
                                    if ((bool)ptMI.isTransportist)
                                    {
                                        if (providerTransportist != null)
                                        {
                                            item.Provider.ProviderTransportist = new ProviderTransportist();
                                            item.Provider.ProviderTransportist.ANTEspecification = providerTransportist.ANTEspecification;
                                        }
                                    }
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

                                    #region ADD ProductionUnitProvider

                                    if ((bool)ptMI.isShrimpPerson)
                                    {
                                        if (tempPerson?.Provider?.ProductionUnitProvider != null)
                                        {
                                            item.Provider.ProductionUnitProvider = new List<ProductionUnitProvider>();

                                            var productionUnitProviders = tempPerson.Provider.ProductionUnitProvider.ToList();

                                            foreach (var productionUnitProvider in productionUnitProviders)
                                            {
                                                var tempProductioUnitProvider = new ProductionUnitProvider
                                                {
                                                    code = productionUnitProvider.code,
                                                    address = productionUnitProvider.address,
                                                    SiteUnitProvider = productionUnitProvider.SiteUnitProvider,
                                                    name = productionUnitProvider.name,
                                                    poolNumber = productionUnitProvider.poolNumber,
                                                    INPnumber = productionUnitProvider.INPnumber,
                                                    tramitNumber = productionUnitProvider.tramitNumber,
                                                    ministerialAgreement = productionUnitProvider.ministerialAgreement,
                                                    isActive = productionUnitProvider.isActive,
                                                    id_userCreate = ActiveUser.id,

                                                    dateCreate = DateTime.Now,
                                                    id_userUpdate = ActiveUser.id,
                                                    dateUpdate = DateTime.Now,
                                                    id_FishingSite = productionUnitProvider.id_FishingSite,
                                                    id_FishingZone = productionUnitProvider.id_FishingZone,
                                                    id_shippingType = productionUnitProvider.id_shippingType,
                                                    poolprefix = productionUnitProvider.poolprefix,
                                                    poolsuffix = productionUnitProvider.poolsuffix
                                                };
                                                item.Provider.ProductionUnitProvider.Add(tempProductioUnitProvider);
                                            }
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

                                    #region ADD ProviderItems

                                    if (tempPerson?.Provider?.ProviderItem != null)
                                    {
                                        item.Provider.ProviderItem = new List<ProviderItem>();

                                        var providerItems = tempPerson.Provider.ProviderItem.ToList();

                                        foreach (var providerItem in providerItems)
                                        {
                                            var tempProviderItem = new ProviderItem
                                            {
                                                id_item = providerItem.id_item
                                            };
                                            item.Provider.ProviderItem.Add(tempProviderItem);
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

                                    #region ADD FrameworkContract

                                    if (tempPerson?.FrameworkContract != null)
                                    {
                                        item.FrameworkContract = new List<FrameworkContract>();

                                        var frameworkContract = tempPerson.FrameworkContract.ToList();

                                        //FrameworkContract
                                        foreach (var detailFrameworkContract in frameworkContract)
                                        {
                                            #region Document
                                            Document document = new Document();
                                            document = ServiceDocument.CreateDocument(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, "62", "Contrato Marco");
                                            //Actualizando Estado del Documento FrameworkContract
                                            var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                                            document.id_documentState = documentState.id;
                                            document.DocumentState = documentState;
                                            #endregion

                                            var tempFrameworkContract = new FrameworkContract();
                                            tempFrameworkContract.id = document.id;
                                            tempFrameworkContract.Document = document;
                                            tempFrameworkContract.id_person = item.id;
                                            tempFrameworkContract.Person = item;
                                            tempFrameworkContract.id_typeContractFramework = detailFrameworkContract.id_typeContractFramework;
                                            tempFrameworkContract.TypeContractFramework = db.TypeContractFramework.FirstOrDefault(fod => fod.id == detailFrameworkContract.id_typeContractFramework);
                                            tempFrameworkContract.id_company = detailFrameworkContract.id_company;
                                            tempFrameworkContract.id_rol = detailFrameworkContract.id_rol;
                                            tempFrameworkContract.Rol = db.Rol.FirstOrDefault(fod => fod.id == detailFrameworkContract.id_rol);
                                            tempFrameworkContract.FrameworkContractItem = new List<FrameworkContractItem>();
                                            tempFrameworkContract.FrameworkContractExtension = new List<FrameworkContractExtension>();

                                            //FrameworkContractItem
                                            if (detailFrameworkContract.FrameworkContractItem != null)
                                            {
                                                foreach (var detailFrameworkContractItem in detailFrameworkContract.FrameworkContractItem)
                                                {
                                                    var tempFrameworkContractItem = new FrameworkContractItem();
                                                    tempFrameworkContractItem.FrameworkContract = tempFrameworkContract;
                                                    tempFrameworkContractItem.id_item = detailFrameworkContractItem.id_item;
                                                    tempFrameworkContractItem.Item = db.Item.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_item);
                                                    tempFrameworkContractItem.startDate = detailFrameworkContractItem.startDate;
                                                    tempFrameworkContractItem.endDate = detailFrameworkContractItem.endDate;
                                                    tempFrameworkContractItem.value = detailFrameworkContractItem.value;
                                                    tempFrameworkContractItem.amout = detailFrameworkContractItem.amout;
                                                    tempFrameworkContractItem.id_metricUnit = detailFrameworkContractItem.id_metricUnit;
                                                    tempFrameworkContractItem.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_metricUnit);
                                                    tempFrameworkContractItem.FrameworkContractDeliveryPlan = new List<FrameworkContractDeliveryPlan>();
                                                    tempFrameworkContractItem.FrameworkContractExtension = new List<FrameworkContractExtension>();
                                                    //FrameworkContractDeliveryPlan
                                                    if (detailFrameworkContractItem.FrameworkContractDeliveryPlan != null)
                                                    {
                                                        foreach (var detailFrameworkContractDeliveryPlan in detailFrameworkContractItem.FrameworkContractDeliveryPlan)
                                                        {
                                                            var tempFrameworkContractDeliveryPlan = new FrameworkContractDeliveryPlan();
                                                            tempFrameworkContractDeliveryPlan.FrameworkContractItem = tempFrameworkContractItem;
                                                            tempFrameworkContractDeliveryPlan.deliveryPlanDate = detailFrameworkContractDeliveryPlan.deliveryPlanDate;
                                                            tempFrameworkContractDeliveryPlan.amout = detailFrameworkContractDeliveryPlan.amout;

                                                            tempFrameworkContractItem.FrameworkContractDeliveryPlan.Add(tempFrameworkContractDeliveryPlan);
                                                        }
                                                    }
                                                    //FrameworkContractExtension
                                                    if (detailFrameworkContractItem.FrameworkContractExtension != null)
                                                    {
                                                        foreach (var detailFrameworkContractExtension in detailFrameworkContractItem.FrameworkContractExtension)
                                                        {
                                                            var tempFrameworkContractExtension = new FrameworkContractExtension();
                                                            tempFrameworkContractExtension.FrameworkContract = tempFrameworkContract;
                                                            tempFrameworkContractExtension.FrameworkContractItem = tempFrameworkContractItem;
                                                            tempFrameworkContractExtension.value = detailFrameworkContractExtension.value;
                                                            tempFrameworkContractExtension.amout = detailFrameworkContractExtension.amout;

                                                            tempFrameworkContractExtension.isSave = true;
                                                            tempFrameworkContract.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                            tempFrameworkContractItem.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                        }
                                                    }

                                                    tempFrameworkContract.FrameworkContractItem.Add(tempFrameworkContractItem);
                                                }
                                            }
                                            item.FrameworkContract.Add(tempFrameworkContract);
                                        }
                                    }

                                    #endregion

                                    item.codeCI = Convert.ToString(NNuSecuenciaMax);
                                    tblGeSecuencia.NNuSecuencia = NNuSecuenciaMax;
                                    dbCI.Entry(tblGeSecuencia).State = EntityState.Modified;

                                    MigrationPerson migrationPerson = db.MigrationPerson.FirstOrDefault(fod => fod.id_person == item.id && fod.id_rol == r);
                                    if (migrationPerson == null)
                                    {
                                        migrationPerson = new MigrationPerson
                                        {
                                            id_person = item.id,
                                            id_rol = r,
                                            id_user_replicate = ActiveUser.id,
                                            id_userCreate = ActiveUser.id,
                                            isNewPerson = true,
                                            dateCreate = DateTime.Now
                                        };
                                        db.MigrationPerson.Add(migrationPerson);
                                    }
                                }

                                if (rol.name == "Cliente Local")
                                {
                                    item.Customer = new Customer();

                                    #region Customer
                                    item.Customer.id_customerType = customer.id_customerType;
                                    item.Customer.name_customerType = getSubstr(DataProviderCustomerType.CustomerTypeById(customer.id_customerType).name, 99);
                                    item.Customer.forceToKeepAccountsCusm = customer.forceToKeepAccountsCusm;
                                    item.Customer.specialTaxPayerCusm = customer.specialTaxPayerCusm;
                                    item.Customer.id_vendorAssigned = customer.id_vendorAssigned;
                                    item.Customer.name_vendorAssigned = (customer.id_vendorAssigned == null) ? null : getSubstr(DataProviderPerson.PersonById((int)customer.id_vendorAssigned).fullname_businessName, 99);
                                    item.Customer.id_economicGroupCusm = customer.id_economicGroupCusm;
                                    item.Customer.name_economicGroup = (customer.id_economicGroupCusm == null) ? null : getSubstr(DataProviderEconomicGroup.EconomicGroupById(customer.id_economicGroupCusm).name, 99);
                                    item.Customer.applyIva = customer.applyIva;
                                    item.Customer.id_commissionAgent = customer.id_commissionAgent;
                                    item.Customer.id_customerState = customer.id_customerState;
                                    item.Customer.id_clientCategory = customer.id_clientCategory;
                                    item.Customer.id_businessLine = customer.id_businessLine;

                                    item.Customer.isActive = true;
                                    item.Customer.id_userCreate = ActiveUser.id;
                                    item.Customer.dateCreate = DateTime.Now;
                                    item.Customer.id_userUpdate = ActiveUser.id;
                                    item.Customer.dateUpdate = DateTime.Now;
                                    #endregion

                                    #region CustomerContact

                                    item.Customer.CustomerContact = new CustomerContact();
                                    item.Customer.CustomerContact.phoneCellular = customerContact.phoneCellular;
                                    item.Customer.CustomerContact.phoneNumber = customerContact.phoneNumber;
                                    item.Customer.CustomerContact.emailGeneralBilling = customerContact.emailGeneralBilling;
                                    item.Customer.CustomerContact.emailGeneralBillingBc = customerContact.emailGeneralBillingBc;
                                    item.Customer.CustomerContact.emailNC = customerContact.emailNC;
                                    item.Customer.CustomerContact.emailNCBc = customerContact.emailNCBc;
                                    item.Customer.CustomerContact.emailND = customerContact.emailND;
                                    item.Customer.CustomerContact.emailNDBc = customerContact.emailNDBc;
                                    item.Customer.CustomerContact.website = customerContact.website;
                                    item.Customer.CustomerContact.id_userCreate = ActiveUser.id;
                                    item.Customer.CustomerContact.dateCreate = DateTime.Now;
                                    item.Customer.CustomerContact.id_userUpdate = ActiveUser.id;
                                    item.Customer.CustomerContact.dateUpdate = DateTime.Now;
                                    #endregion

                                    #region CustomerCreditInfo
                                    item.Customer.CustomerCreditInfo = new CustomerCreditInfo();
                                    item.Customer.CustomerCreditInfo.creditDays = customerCreditInfo.creditDays;
                                    item.Customer.CustomerCreditInfo.creditQuota = customerCreditInfo.creditQuota;
                                    item.Customer.CustomerCreditInfo.discountRate = customerCreditInfo.discountRate;
                                    item.Customer.CustomerCreditInfo.id_paymentMethodCusm = customerCreditInfo.id_paymentMethodCusm;
                                    item.Customer.CustomerCreditInfo.name_paymentMethod = (customerCreditInfo.id_paymentMethodCusm == null) ? null : getSubstr(DataProviderPaymentMethod.PaymentMethodById(customerCreditInfo.id_paymentMethodCusm).name, 99);
                                    item.Customer.CustomerCreditInfo.id_paymentTerm = customerCreditInfo.id_paymentTerm;
                                    item.Customer.CustomerCreditInfo.name_paymentTerm = (customerCreditInfo.id_paymentTerm == null) ? null : getSubstr(DataProviderPaymentTerm.PaymentTermById(customerCreditInfo.id_paymentTerm).name, 99);
                                    item.Customer.CustomerCreditInfo.id_userCreate = ActiveUser.id;
                                    item.Customer.CustomerCreditInfo.dateCreate = DateTime.Now;
                                    item.Customer.CustomerCreditInfo.id_userUpdate = ActiveUser.id;
                                    item.Customer.CustomerCreditInfo.dateUpdate = DateTime.Now;


                                    #endregion

                                    #region CustomerAddress
                                    if (tempPerson?.Customer?.CustomerAddress != null)
                                    {
                                        item.Customer.CustomerAddress = new List<CustomerAddress>();

                                        var customerAddresses = tempPerson.Customer.CustomerAddress.ToList();

                                        foreach (var customerAddress in customerAddresses)
                                        {

                                            var tempcustomerAddress = new CustomerAddress
                                            {


                                                id_addressType = customerAddress.id_addressType,
                                                name_addressType = customerAddress.name_addressType,
                                                addressdescription = customerAddress.addressdescription,
                                                isActive = customerAddress.isActive,
                                                id_userCreate = ActiveUser.id,
                                                dateCreate = DateTime.Now,
                                                id_userUpdate = ActiveUser.id,
                                                dateUpdate = DateTime.Now,
                                            };
                                            item.Customer.CustomerAddress.Add(tempcustomerAddress);
                                        }
                                    }
                                    #endregion

                                    #region CustomerPriceList
                                    //if (tempPerson?.Customer.CustomerPriceList != null)
                                    //{
                                    //    item.Customer.CustomerPriceList = new List<CustomerPriceList>();

                                    //    var customerPriceLists = tempPerson.Customer.CustomerPriceList.ToList();

                                    //    foreach (var customerPriceList in customerPriceLists)
                                    //    {


                                    //        PriceList _priceList = db.PriceList.FirstOrDefault(reg => reg.id == customerPriceList.id_priceList);

                                    //        var tempcustomerPriceList = new CustomerPriceList
                                    //        {

                                    //            id_priceList = customerPriceList.id_priceList,
                                    //            name_priceList = customerPriceList.name_priceList,
                                    //            isActive = customerPriceList.isActive,
                                    //            id_userCreate = ActiveUser.id,
                                    //            dateCreate = DateTime.Now,
                                    //            id_userUpdate = ActiveUser.id,
                                    //            dateUpdate = DateTime.Now,
                                    //        };
                                    //        item.Customer.CustomerPriceList.Add(tempcustomerPriceList);
                                    //    }
                                    //}
                                    #endregion

                                    item.codeCI = Convert.ToString(NNuSecuenciaMax);
                                    tblGeSecuencia.NNuSecuencia = NNuSecuenciaMax;
                                    dbCI.Entry(tblGeSecuencia).State = EntityState.Modified;

                                    MigrationPerson migrationPerson = db.MigrationPerson.FirstOrDefault(fod => fod.id_person == item.id && fod.id_rol == r);
                                    if (migrationPerson == null)
                                    {
                                        migrationPerson = new MigrationPerson
                                        {
                                            id_person = item.id,
                                            id_rol = r,
                                            id_user_replicate = ActiveUser.id,
                                            id_userCreate = ActiveUser.id,
                                            isNewPerson = true,
                                            dateCreate = DateTime.Now
                                        };
                                        db.MigrationPerson.Add(migrationPerson);
                                    }
                                }

                                if (rol.name == "Cliente Exterior")
                                {
                                    item.codeCI = Convert.ToString(NNuSecuenciaMax);
                                    tblGeSecuencia.NNuSecuencia = NNuSecuenciaMax;
                                    dbCI.Entry(tblGeSecuencia).State = EntityState.Modified;

                                    MigrationPerson migrationPerson = db.MigrationPerson.FirstOrDefault(fod => fod.id_person == item.id && fod.id_rol == r);
                                    if (migrationPerson == null)
                                    {
                                        migrationPerson = new MigrationPerson
                                        {
                                            id_person = item.id,
                                            id_rol = r,
                                            id_user_replicate = ActiveUser.id,
                                            id_userCreate = ActiveUser.id,
                                            isNewPerson = true,
                                            dateCreate = DateTime.Now
                                        };
                                        db.MigrationPerson.Add(migrationPerson);
                                    }
                                }
                                item.Rol.Add(rol);
                            }
                        }
                        // TODO:
                        if (employee != null && employee.id_department != 0)
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
                    item.id_company = this.ActiveCompanyId;
                    item.address = item.address.Trim();

                    model.Add(item);
                    db.SaveChanges();
                    trans.Commit();

                    dbCI.SaveChanges();
                    idTmp = item.id;
                    isSaved = true;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            try
            {
                #region Replicate Person to PriceList
                if (isSaved)
                {
                    #region Replicate Person to PriceList
                    if (isSaved)
                    {
                        #region 
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = _pathProgramReplication,
                            Arguments = item.id.ToString() + " RPPLP ReplicateInformation",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        Process.Start(startInfo);
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "PersonAddNewReplication", "PROD");
            }

            RefreesProductionUnitProviderPoolBachList(idTmp);
            var result = new
            {
                idPerson = idTmp
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //RefressPooll
        public void RefreesProductionUnitProviderPoolBachList(int id_provider)
        {
            try
            {
                bool vcontinue = false;
                var tempProvider = db.Provider.Where(g => g.id == id_provider).FirstOrDefault();
                if (tempProvider != null && tempProvider.ProductionUnitProvider != null)
                {

                    foreach (var prdunit in tempProvider.ProductionUnitProvider)
                    {
                        int cantcreate = prdunit.poolNumber ?? 0;
                        prdunit.ProductionUnitProviderPool = DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolBachList(prdunit.id, ActiveUser.id, prdunit.poolprefix, prdunit.poolsuffix, cantcreate);
                    }
                    using (var dbsave = new DBContext())
                    {
                        using (DbContextTransaction trans = dbsave.Database.BeginTransaction())
                        {
                            try
                            {
                                foreach (var prdunit in tempProvider.ProductionUnitProvider)
                                {
                                    foreach (var prdunitPool in prdunit.ProductionUnitProviderPool)
                                    {
                                        if (prdunitPool.id == 0)
                                        {
                                            vcontinue = true;
                                            ProductionUnitProviderPool temp = new ProductionUnitProviderPool()
                                            {
                                                code = prdunitPool.code,
                                                id_productionUnitProvider = prdunitPool.id_productionUnitProvider,
                                                dateUpdate = prdunitPool.dateUpdate,
                                                dateCreate = prdunitPool.dateCreate,
                                                id_userCreate = prdunitPool.id_userCreate,
                                                id_userUpdate = prdunitPool.id_userUpdate,
                                                isActive = prdunitPool.isActive,
                                                name = prdunitPool.name
                                            };

                                            dbsave.ProductionUnitProviderPool.Add(temp);
                                            dbsave.Entry(temp).State = EntityState.Added;
                                        }
                                    }
                                }

                                if (vcontinue)
                                {
                                    dbsave.SaveChanges();
                                    trans.Commit();
                                }
                            }
                            catch //(Exception expro)
                            {

                                trans.Rollback();
                            }
                        }
                    }


                }

            }
            catch //(Exception ex)
            {


            }



        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonsPartialUpdate(int id_person, DXPANACEASOFT.Models.Person item, Provider provider, ProviderGeneralData providerGeneralData,
                                                 ProviderTypeShrimp providerTypeShrimp, ProviderTransportist providerTransportist,
                                                 ProviderGeneralDataEP providerGeneralDataEP, ProviderGeneralDataRise providerGeneralDataRise,
                                                 Employee employee, Customer customer, CustomerContact customerContact,
                                                 CustomerCreditInfo customerCreditInfo,
                                                 int[] id_roles, ProviderRawMaterial providerRawMaterial)
        {
            var model = db.Person;
            var modelItem = model.FirstOrDefault(it => it.id == id_person);

            bool hadRolProvider = false;
            hadRolProvider = (modelItem?.Rol.FirstOrDefault(fod => fod.name == "Proveedor") != null) ? true : false;
            bool hadRolCustomerL = false;
            hadRolCustomerL = (modelItem?.Rol.FirstOrDefault(fod => fod.name == "Cliente Local") != null) ? true : false;
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];

            int idTmp = 0;
            DBContextCI dbCI = new DBContextCI();
            TblGeSecuencia tblGeSecuencia = dbCI.TblGeSecuencia.FirstOrDefault(fod => fod.CCiTipoSecu == "PERS");
            tblGeSecuencia = tblGeSecuencia ?? new TblGeSecuencia();

            decimal NNuSecuenciaMax = tblGeSecuencia.NNuSecuencia;
            NNuSecuenciaMax = NNuSecuenciaMax + 1;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            bool isSaved = false;

            ProviderType ptMI = null;
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

                                    #region ADD ProviderRawMaterial
                                    item.isCopacking = item.isCopacking ?? false;
                                    if ((bool)item.isCopacking || validarRolExistente("Planta Proceso", id_roles))
                                    {
                                        modelItem.Provider.ProviderRawMaterial =
                                            modelItem.Provider.ProviderRawMaterial ?? new ProviderRawMaterial();

                                        modelItem.Provider.ProviderRawMaterial.id_provider = providerRawMaterial.id_provider;
                                        modelItem.Provider.ProviderRawMaterial.id_Warehouse = providerRawMaterial.id_Warehouse;
                                        modelItem.Provider.ProviderRawMaterial.id_WarehouseLocation = providerRawMaterial.id_WarehouseLocation;
                                        modelItem.Provider.ProviderRawMaterial.id_CostCenter = providerRawMaterial.id_CostCenter;
                                        modelItem.Provider.ProviderRawMaterial.id_SubCostCenter = providerRawMaterial.id_SubCostCenter;

                                        modelItem.Provider.ProviderRawMaterial.id_WarehouseCarton = providerRawMaterial.id_WarehouseCarton;
                                        modelItem.Provider.ProviderRawMaterial.id_warehouseLocationCarton = providerRawMaterial.id_warehouseLocationCarton;
                                        modelItem.Provider.ProviderRawMaterial.id_CostCenterCarton = providerRawMaterial.id_CostCenterCarton;
                                        modelItem.Provider.ProviderRawMaterial.id_SubCostCenterCarton = providerRawMaterial.id_SubCostCenterCarton;

                                        modelItem.Provider.ProviderRawMaterial.id_WarehouseFreeze = providerRawMaterial.id_WarehouseFreeze;
                                        modelItem.Provider.ProviderRawMaterial.id_warehouseLocationFreeze = providerRawMaterial.id_warehouseLocationFreeze;
                                        modelItem.Provider.ProviderRawMaterial.id_CostCenterFreeze = providerRawMaterial.id_CostCenterFreeze;
                                        modelItem.Provider.ProviderRawMaterial.id_SubCostCenterFreeze = providerRawMaterial.id_SubCostCenterFreeze;

                                        modelItem.Provider.ProviderRawMaterial.id_WarehouseLooseCarton = providerRawMaterial.id_WarehouseLooseCarton;
                                        modelItem.Provider.ProviderRawMaterial.id_WarehouseLocationLooseCarton = providerRawMaterial.id_WarehouseLocationLooseCarton;
                                        modelItem.Provider.ProviderRawMaterial.id_CostCenterLooseCarton = providerRawMaterial.id_CostCenterLooseCarton;
                                        modelItem.Provider.ProviderRawMaterial.id_SubCostCenterLooseCarton = providerRawMaterial.id_SubCostCenterLooseCarton;

                                        if (db.ProviderRawMaterial.Select(e => e.id_provider).Contains(providerRawMaterial.id_provider))
                                        {
                                            db.ProviderRawMaterial.Attach(modelItem.Provider.ProviderRawMaterial);
                                            db.Entry(modelItem.Provider.ProviderRawMaterial).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            db.ProviderRawMaterial.Attach(modelItem.Provider.ProviderRawMaterial);
                                            db.Entry(modelItem.Provider.ProviderRawMaterial).State = EntityState.Added;
                                        }

                                    }

                                    this.ViewBag.rolExistente = validarRolExistente("Planta Proceso", id_roles);
                                    #endregion

                                    //ProviderGeneralData
                                    if (modelItem.Provider.ProviderGeneralData == null)
                                    {
                                        modelItem.Provider.ProviderGeneralData = new ProviderGeneralData();
                                        modelItem.Provider.ProviderGeneralData.id_provider = modelItem.Provider.id;
                                        db.ProviderGeneralData.Attach(modelItem.Provider.ProviderGeneralData);
                                        db.Entry(modelItem.Provider.ProviderGeneralData).State = EntityState.Added;
                                    }
                                    //ProviderTypeShrimp

                                    //ProviderTypeShrimp
                                    ptMI = db.ProviderType.FirstOrDefault(fod => fod.id == providerGeneralData.id_providerType);
                                    ptMI = ptMI ?? new ProviderType();
                                    if (modelItem.Provider.ProviderTypeShrimp == null)
                                    {

                                        if ((bool)ptMI.isShrimpPerson)
                                        {
                                            modelItem.Provider.ProviderTypeShrimp = new ProviderTypeShrimp();
                                            modelItem.Provider.ProviderTypeShrimp.id_provider = modelItem.Provider.id;
                                            db.ProviderTypeShrimp.Attach(modelItem.Provider.ProviderTypeShrimp);
                                            db.Entry(modelItem.Provider.ProviderTypeShrimp).State = EntityState.Added;
                                        }
                                    }
                                    if ((bool)ptMI.isTransportist)
                                    {
                                        if (modelItem.Provider.ProviderTransportist == null)
                                        {
                                            modelItem.Provider.ProviderTransportist = new ProviderTransportist();
                                            modelItem.Provider.ProviderTransportist.id_provider = modelItem.Provider.id;
                                            db.ProviderTransportist.Attach(modelItem.Provider.ProviderTransportist);
                                            db.Entry(modelItem.Provider.ProviderTransportist).State = EntityState.Added;
                                        }
                                    }
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
                                    modelItem.Provider.ProviderGeneralData.buyerLabel = providerGeneralData.buyerLabel;
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
                                    modelItem.Provider.ProviderGeneralData.plantCode = providerGeneralData.plantCode;
                                    modelItem.Provider.ProviderGeneralData.fda = providerGeneralData.fda;
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

                                    // ProviderTypeShrimp
                                    if ((bool)ptMI.isShrimpPerson)
                                    {
                                        if (providerTypeShrimp != null)
                                        {
                                            modelItem.Provider.ProviderTypeShrimp.totalHectareasPool = providerTypeShrimp.totalHectareasPool;
                                            modelItem.Provider.ProviderTypeShrimp.grammageFrom = providerTypeShrimp.grammageFrom;
                                            modelItem.Provider.ProviderTypeShrimp.grammageUpto = providerTypeShrimp.grammageUpto;
                                            modelItem.Provider.ProviderTypeShrimp.totalHectareasPool = providerTypeShrimp.totalHectareasPool;
                                            
                                            if ((providerTypeShrimp?.id_TypeINP ?? 0) == 0)
                                            {
                                                modelItem.Provider.ProviderTypeShrimp.id_TypeINP = null;
                                            }
                                            else
                                            {
                                                modelItem.Provider.ProviderTypeShrimp.id_TypeINP = providerTypeShrimp.id_TypeINP;
                                            }

                                            if (providerTypeShrimp.id_protectiveProvider != null)
                                            {
                                                if ((int)providerTypeShrimp.id_protectiveProvider != 0)
                                                {
                                                    modelItem.Provider.ProviderTypeShrimp.id_protectiveProvider = providerTypeShrimp.id_protectiveProvider;
                                                }
                                                else
                                                {
                                                    modelItem.Provider.ProviderTypeShrimp.id_protectiveProvider = modelItem.id;
                                                }
                                            }
                                            else
                                            {
                                                modelItem.Provider.ProviderTypeShrimp.id_protectiveProvider = modelItem.id;
                                            }
                                            if (providerTypeShrimp.id_buyerProvider != null)
                                            {
                                                if (providerTypeShrimp.id_buyerProvider != 0)
                                                {
                                                    modelItem.Provider.ProviderTypeShrimp.id_buyerProvider = providerTypeShrimp.id_buyerProvider;
                                                }
                                                else
                                                {
                                                    modelItem.Provider.ProviderTypeShrimp.id_buyerProvider = null;
                                                }

                                            }
                                            else
                                            {
                                                modelItem.Provider.ProviderTypeShrimp.id_buyerProvider = null;
                                            }
                                        }
                                    }
                                    if ((bool)ptMI.isTransportist)
                                    {
                                        if (providerTransportist != null)
                                        {
                                            modelItem.Provider.ProviderTransportist.ANTEspecification = providerTransportist.ANTEspecification;
                                        }
                                    }
                                    #endregion

                                    #region UPDATE ProviderUnitProduction
                                    if ((bool)ptMI.isShrimpPerson)
                                    {
                                        if (tempPerson?.Provider?.ProductionUnitProvider != null)
                                        {

                                            var detailsPUP = tempPerson.Provider.ProductionUnitProvider.ToList();

                                            foreach (var detail in detailsPUP)
                                            {
                                                if (detail.isCopackingDetail == null)
                                                    detail.isCopackingDetail = false;

                                                if ((bool)item.isCopacking && (bool)detail.isCopackingDetail)
                                                {
                                                    if (detail.ministerialAgreement == null)
                                                        detail.ministerialAgreement = "";
                                                    if (detail.tramitNumber == null)
                                                        detail.tramitNumber = "";
                                                    if (detail.INPnumber == null)
                                                        detail.INPnumber = "";
                                                }
                                                ProductionUnitProvider pupTemporal = modelItem.Provider.ProductionUnitProvider.FirstOrDefault(fod => fod.id == detail.id);
                                                if (pupTemporal == null)
                                                {
                                                    pupTemporal = new ProductionUnitProvider();
                                                    pupTemporal.code = detail.code;
                                                    pupTemporal.name = detail.name;
                                                    pupTemporal.address = detail.address;
                                                    pupTemporal.SiteUnitProvider = detail.SiteUnitProvider;
                                                    pupTemporal.poolNumber = detail.poolNumber;
                                                    pupTemporal.isActive = detail.isActive;
                                                    pupTemporal.id_userCreate = detail.id_userCreate;
                                                    pupTemporal.dateCreate = DateTime.Now;
                                                    pupTemporal.id_userUpdate = ActiveUser.id;
                                                    pupTemporal.dateUpdate = DateTime.Now;
                                                    pupTemporal.id_shippingType = detail.id_shippingType;
                                                    pupTemporal.id_FishingZone = detail.id_FishingZone;
                                                    pupTemporal.id_FishingSite = detail.id_FishingSite;
                                                    pupTemporal.INPnumber = detail.INPnumber;
                                                    pupTemporal.ministerialAgreement = detail.ministerialAgreement;
                                                    pupTemporal.tramitNumber = detail.tramitNumber;
                                                    pupTemporal.poolprefix = detail.poolprefix;
                                                    pupTemporal.poolsuffix = detail.poolsuffix;
                                                    if ((bool)item.isCopacking)
                                                    {
                                                        pupTemporal.isCopackingDetail = detail.isCopackingDetail;
                                                    }
                                                    else
                                                    {
                                                        pupTemporal.isCopackingDetail = false;
                                                    }

                                                    modelItem.Provider.ProductionUnitProvider.Add(pupTemporal);
                                                    db.Entry(pupTemporal).State = EntityState.Added;
                                                }
                                                else
                                                {
                                                    pupTemporal.id_provider = detail.id_provider;
                                                    pupTemporal.Provider = db.Provider.FirstOrDefault(fod => fod.id == detail.id_provider);

                                                    pupTemporal.code = detail.code;
                                                    pupTemporal.name = detail.name;
                                                    pupTemporal.address = detail.address;
                                                    pupTemporal.SiteUnitProvider = detail.SiteUnitProvider;
                                                    pupTemporal.poolNumber = detail.poolNumber;
                                                    pupTemporal.isActive = detail.isActive;
                                                    pupTemporal.id_shippingType = detail.id_shippingType;
                                                    pupTemporal.id_FishingSite = detail.id_FishingSite;
                                                    pupTemporal.id_FishingZone = detail.id_FishingZone;
                                                    pupTemporal.INPnumber = detail.INPnumber;
                                                    pupTemporal.ministerialAgreement = detail.ministerialAgreement;
                                                    pupTemporal.tramitNumber = detail.tramitNumber;
                                                    pupTemporal.id_userCreate = detail.id_userCreate;
                                                    pupTemporal.dateCreate = DateTime.Now;
                                                    pupTemporal.id_userUpdate = ActiveUser.id;
                                                    pupTemporal.dateUpdate = DateTime.Now;
                                                    pupTemporal.poolprefix = detail.poolprefix;
                                                    pupTemporal.poolsuffix = detail.poolsuffix;
                                                    if ((bool)item.isCopacking)
                                                    {
                                                        pupTemporal.isCopackingDetail = detail.isCopackingDetail;
                                                    }
                                                    else
                                                    {
                                                        pupTemporal.isCopackingDetail = false;
                                                    }




                                                    db.Entry(pupTemporal).State = EntityState.Modified;
                                                }


                                            }
                                        }
                                    }

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

                                    #region UPDATE ProviderItems

                                    if (tempPerson?.Provider?.ProviderItem != null)
                                    {
                                        if (modelItem.Provider.ProviderItem != null)
                                        {
                                            for (int i = modelItem.Provider.ProviderItem.Count - 1; i >= 0; i--)
                                            {
                                                var detail = modelItem.Provider.ProviderItem.ElementAt(i);
                                                modelItem.Provider.ProviderItem.Remove(detail);
                                                db.Entry(detail).State = EntityState.Deleted;
                                            }
                                        }
                                        else
                                        {
                                            modelItem.Provider.ProviderItem = new List<ProviderItem>();
                                        }


                                        var providerItems = tempPerson.Provider.ProviderItem.ToList();

                                        foreach (var providerItem in providerItems)
                                        {
                                            var tempProviderItem = new ProviderItem
                                            {
                                                id_item = providerItem.id_item
                                            };
                                            modelItem.Provider.ProviderItem.Add(tempProviderItem);
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

                                    if (modelItem.codeCI == null || modelItem.codeCI == "")
                                    {
                                        modelItem.codeCI = Convert.ToString(NNuSecuenciaMax);
                                        tblGeSecuencia.NNuSecuencia = NNuSecuenciaMax;
                                        dbCI.Entry(tblGeSecuencia).State = EntityState.Modified;
                                    }
                                    MigrationPerson migrationPerson = db.MigrationPerson.FirstOrDefault(fod => fod.id_person == item.id && fod.id_rol == id);
                                    if (migrationPerson == null)
                                    {

                                        migrationPerson = new MigrationPerson
                                        {
                                            id_person = modelItem.id,
                                            id_rol = id,
                                            id_user_replicate = ActiveUser.id,

                                            isNewPerson = ((!hadRolProvider) ? true : false),
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now
                                        };
                                        db.MigrationPerson.Add(migrationPerson);

                                    }

                                }
                                else if (_rol.name == "Cliente Local")
                                {

                                    #region  {RA} - validate Customer 
                                    if (modelItem.Customer == null)
                                    {
                                        modelItem.Customer = new Customer();
                                        modelItem.Customer.id = modelItem.id;
                                        modelItem.Customer.id_userCreate = ActiveUser.id;
                                        modelItem.Customer.dateCreate = DateTime.Now;
                                        db.Customer.Attach(modelItem.Customer);
                                        db.Entry(modelItem.Customer).State = EntityState.Added;
                                    }
                                    #endregion

                                    #region  {RA} - validate Customer CustomerContact
                                    if (modelItem.Customer.CustomerContact == null)
                                    {
                                        modelItem.Customer.CustomerContact = new CustomerContact();
                                        modelItem.Customer.CustomerContact.id_customer = modelItem.Customer.id;
                                        modelItem.Customer.CustomerContact.id_userCreate = ActiveUser.id;
                                        modelItem.Customer.CustomerContact.dateCreate = DateTime.Now;
                                        db.CustomerContact.Attach(modelItem.Customer.CustomerContact);
                                        db.Entry(modelItem.Customer.CustomerContact).State = EntityState.Added;
                                    }
                                    #endregion

                                    #region  {RA} - validate Customer CustomerCreditInfo
                                    if (modelItem.Customer.CustomerCreditInfo == null)
                                    {
                                        modelItem.Customer.CustomerCreditInfo = new CustomerCreditInfo();
                                        modelItem.Customer.CustomerCreditInfo.id_customer = modelItem.Customer.id;
                                        modelItem.Customer.CustomerCreditInfo.id_userCreate = ActiveUser.id;
                                        modelItem.Customer.CustomerCreditInfo.dateCreate = DateTime.Now;
                                        db.CustomerCreditInfo.Attach(modelItem.Customer.CustomerCreditInfo);
                                        db.Entry(modelItem.Customer.CustomerCreditInfo).State = EntityState.Added;
                                    }
                                    #endregion

                                    #region  {RA} - Customer
                                    modelItem.Customer.id_customerType = customer.id_customerType;

                                    String nameCustomerType = DataProviderCustomerType.CustomerTypeById(customer.id_customerType).name;

                                    modelItem.Customer.name_customerType = getSubstr(nameCustomerType, 99);
                                    modelItem.Customer.forceToKeepAccountsCusm = customer.forceToKeepAccountsCusm;
                                    modelItem.Customer.specialTaxPayerCusm = customer.specialTaxPayerCusm;
                                    modelItem.Customer.id_vendorAssigned = customer.id_vendorAssigned;
                                    modelItem.Customer.name_vendorAssigned = (customer.id_vendorAssigned == null) ? null : getSubstr(DataProviderPerson.PersonById((int)customer.id_vendorAssigned).fullname_businessName, 99);
                                    modelItem.Customer.id_economicGroupCusm = customer.id_economicGroupCusm;
                                    modelItem.Customer.name_economicGroup = (customer.id_economicGroupCusm == null) ? null : getSubstr(DataProviderEconomicGroup.EconomicGroupById((int)customer.id_economicGroupCusm).name, 99);
                                    modelItem.Customer.applyIva = customer.applyIva;
                                    modelItem.Customer.id_commissionAgent = customer.id_commissionAgent;
                                    modelItem.Customer.id_customerState = customer.id_customerState;
                                    modelItem.Customer.id_clientCategory = customer.id_clientCategory;
                                    modelItem.Customer.id_businessLine = customer.id_businessLine;

                                    modelItem.Customer.isActive = customer.isActive;
                                    modelItem.Customer.id_userUpdate = ActiveUser.id;
                                    modelItem.Customer.dateUpdate = DateTime.Now;
                                    #endregion

                                    #region {RA} - CustomerContact
                                    modelItem.Customer.CustomerContact.phoneNumber = customerContact.phoneNumber;
                                    modelItem.Customer.CustomerContact.phoneCellular = customerContact.phoneCellular;
                                    modelItem.Customer.CustomerContact.emailGeneralBilling = customerContact.emailGeneralBilling;
                                    modelItem.Customer.CustomerContact.emailGeneralBillingBc = customerContact.emailGeneralBillingBc;
                                    modelItem.Customer.CustomerContact.emailNC = customerContact.emailNC;
                                    modelItem.Customer.CustomerContact.emailNCBc = customerContact.emailNCBc;
                                    modelItem.Customer.CustomerContact.emailND = customerContact.emailND;
                                    modelItem.Customer.CustomerContact.emailNDBc = customerContact.emailNDBc;
                                    modelItem.Customer.CustomerContact.website = customerContact.website;
                                    modelItem.Customer.CustomerContact.id_userUpdate = ActiveUser.id;
                                    modelItem.Customer.CustomerContact.dateUpdate = DateTime.Now;

                                    #endregion

                                    #region {RA} - CustomerCreditInfo
                                    modelItem.Customer.CustomerCreditInfo.creditDays = customerCreditInfo.creditDays;
                                    modelItem.Customer.CustomerCreditInfo.creditQuota = customerCreditInfo.creditQuota;
                                    modelItem.Customer.CustomerCreditInfo.discountRate = customerCreditInfo.discountRate;
                                    modelItem.Customer.CustomerCreditInfo.id_paymentMethodCusm = customerCreditInfo.id_paymentMethodCusm;
                                    modelItem.Customer.CustomerCreditInfo.name_paymentMethod = (customerCreditInfo.id_paymentMethodCusm == null) ? null : getSubstr(DataProviderPaymentMethod.PaymentMethodById(customerCreditInfo.id_paymentMethodCusm).name, 99);
                                    modelItem.Customer.CustomerCreditInfo.id_paymentTerm = customerCreditInfo.id_paymentTerm;
                                    modelItem.Customer.CustomerCreditInfo.name_paymentTerm = (customerCreditInfo.id_paymentTerm == null) ? null : getSubstr(DataProviderPaymentTerm.PaymentTermById(customerCreditInfo.id_paymentTerm).name, 99);
                                    modelItem.Customer.CustomerCreditInfo.id_userUpdate = ActiveUser.id;
                                    modelItem.Customer.CustomerCreditInfo.dateUpdate = DateTime.Now;
                                    #endregion

                                    #region {RA} - CustomerAddress
                                    if (tempPerson?.Customer?.CustomerAddress != null)
                                    {

                                        var _customerAddresses = tempPerson.Customer.CustomerAddress.ToList();

                                        foreach (var _customerAddress in _customerAddresses)
                                        {

                                            CustomerAddress CurrentCustomerAddress = modelItem.Customer.CustomerAddress
                                                                                            .FirstOrDefault(fod => fod.id == _customerAddress.id);

                                            if (CurrentCustomerAddress == null)
                                            {
                                                CurrentCustomerAddress = new CustomerAddress();
                                                CurrentCustomerAddress.id_addressType = _customerAddress.id_addressType;
                                                CurrentCustomerAddress.name_addressType = _customerAddress.name_addressType;
                                                CurrentCustomerAddress.addressdescription = _customerAddress.addressdescription;
                                                CurrentCustomerAddress.isActive = _customerAddress.isActive;
                                                CurrentCustomerAddress.id_userCreate = _customerAddress.id_userCreate;
                                                CurrentCustomerAddress.dateCreate = DateTime.Now;
                                                CurrentCustomerAddress.id_userUpdate = ActiveUser.id;
                                                CurrentCustomerAddress.dateUpdate = DateTime.Now;
                                                modelItem.Customer.CustomerAddress.Add(CurrentCustomerAddress);
                                                db.Entry(CurrentCustomerAddress).State = EntityState.Added;
                                            }
                                            else
                                            {

                                                CurrentCustomerAddress.id_addressType = _customerAddress.id_addressType;
                                                CurrentCustomerAddress.name_addressType = _customerAddress.name_addressType;
                                                CurrentCustomerAddress.addressdescription = _customerAddress.addressdescription;
                                                CurrentCustomerAddress.isActive = _customerAddress.isActive;
                                                CurrentCustomerAddress.id_userUpdate = ActiveUser.id;
                                                CurrentCustomerAddress.dateUpdate = DateTime.Now;
                                                db.Entry(CurrentCustomerAddress).State = EntityState.Modified;
                                            }


                                        }
                                    }
                                    #endregion

                                    #region {RA} - CustomerPriceList
                                    //if (tempPerson?.Customer?.CustomerPriceList != null)
                                    //{

                                    //    var _customerPriceLists = tempPerson.Customer.CustomerPriceList.ToList();

                                    //    foreach (var _customerPriceList in _customerPriceLists)
                                    //    {

                                    //        CustomerPriceList CurrentCustomerPriceList = modelItem.Customer.CustomerPriceList
                                    //                                                        .FirstOrDefault(fod => fod.id == _customerPriceList.id);

                                    //        if (CurrentCustomerPriceList == null)
                                    //        {
                                    //            CurrentCustomerPriceList = new CustomerPriceList();
                                    //            CurrentCustomerPriceList.id_priceList = _customerPriceList.id_priceList;
                                    //            CurrentCustomerPriceList.name_priceList = _customerPriceList.name_priceList;

                                    //            CurrentCustomerPriceList.isActive = _customerPriceList.isActive;
                                    //            CurrentCustomerPriceList.id_userCreate = _customerPriceList.id_userCreate;
                                    //            CurrentCustomerPriceList.dateCreate = DateTime.Now;
                                    //            CurrentCustomerPriceList.id_userUpdate = ActiveUser.id;
                                    //            CurrentCustomerPriceList.dateUpdate = DateTime.Now;


                                    //            modelItem.Customer.CustomerPriceList.Add(CurrentCustomerPriceList);
                                    //            db.Entry(CurrentCustomerPriceList).State = EntityState.Added;
                                    //        }
                                    //        else
                                    //        {
                                    //            CurrentCustomerPriceList.id_priceList = _customerPriceList.id_priceList;
                                    //            CurrentCustomerPriceList.name_priceList = _customerPriceList.name_priceList;

                                    //            CurrentCustomerPriceList.isActive = _customerPriceList.isActive;
                                    //            CurrentCustomerPriceList.id_userUpdate = ActiveUser.id;
                                    //            CurrentCustomerPriceList.dateUpdate = DateTime.Now;

                                    //            db.Entry(CurrentCustomerPriceList).State = EntityState.Modified;
                                    //        }


                                    //    }
                                    //}
                                    #endregion

                                    if (modelItem.codeCI == null || modelItem.codeCI == "")
                                    {
                                        modelItem.codeCI = Convert.ToString(NNuSecuenciaMax);
                                        tblGeSecuencia.NNuSecuencia = NNuSecuenciaMax;
                                        dbCI.Entry(tblGeSecuencia).State = EntityState.Modified;
                                    }

                                    MigrationPerson migrationPerson = db.MigrationPerson.FirstOrDefault(fod => fod.id_person == item.id && fod.id_rol == id);
                                    if (migrationPerson == null)
                                    {
                                        migrationPerson = new MigrationPerson
                                        {
                                            id_person = modelItem.id,
                                            id_rol = id,
                                            isNewPerson = ((!hadRolCustomerL) ? true : false),
                                            id_user_replicate = ActiveUser.id,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now
                                        };
                                        db.MigrationPerson.Add(migrationPerson);
                                    }
                                    modelItem.Customer = modelItem.Customer ?? new Customer();
                                }

                                else if (_rol.name == "Cliente Exterior")
                                {
                                    if (modelItem.codeCI == null || modelItem.codeCI == "")
                                    {
                                        modelItem.codeCI = Convert.ToString(NNuSecuenciaMax);
                                        tblGeSecuencia.NNuSecuencia = NNuSecuenciaMax;
                                        dbCI.Entry(tblGeSecuencia).State = EntityState.Modified;
                                    }

                                    MigrationPerson migrationPerson = db.MigrationPerson.FirstOrDefault(fod => fod.id_person == item.id && fod.id_rol == id);
                                    if (migrationPerson == null)
                                    {
                                        migrationPerson = new MigrationPerson
                                        {
                                            id_person = modelItem.id,
                                            id_rol = id,
                                            isNewPerson = ((!hadRolCustomerL) ? true : false),
                                            id_user_replicate = ActiveUser.id,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now
                                        };
                                        db.MigrationPerson.Add(migrationPerson);
                                    }
                                }

                            }

                            #region UPDATE FrameworkContract

                            if (tempPerson?.FrameworkContract != null)
                            {
                                if (modelItem.FrameworkContract == null)
                                {
                                    modelItem.FrameworkContract = new List<FrameworkContract>();
                                }

                                var frameworkContract = tempPerson.FrameworkContract.ToList();

                                //FrameworkContract
                                foreach (var detailFrameworkContract in frameworkContract)
                                {
                                    var tempFrameworkContract = modelItem.FrameworkContract.FirstOrDefault(fod => fod.id == detailFrameworkContract.id);
                                    if (tempFrameworkContract == null)
                                    {
                                        #region Document
                                        Document document = new Document();
                                        document = ServiceDocument.CreateDocument(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, "62", "Contrato Marco");
                                        //Actualizando Estado del Documento FrameworkContract
                                        var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                                        document.id_documentState = documentState.id;
                                        document.DocumentState = documentState;
                                        #endregion

                                        tempFrameworkContract = new FrameworkContract();
                                        tempFrameworkContract.id = document.id;
                                        tempFrameworkContract.Document = document;
                                        tempFrameworkContract.id_person = modelItem.id;
                                        tempFrameworkContract.Person = modelItem;
                                        tempFrameworkContract.id_typeContractFramework = detailFrameworkContract.id_typeContractFramework;
                                        tempFrameworkContract.TypeContractFramework = db.TypeContractFramework.FirstOrDefault(fod => fod.id == detailFrameworkContract.id_typeContractFramework);
                                        tempFrameworkContract.id_company = detailFrameworkContract.id_company;
                                        tempFrameworkContract.id_rol = detailFrameworkContract.id_rol;
                                        tempFrameworkContract.Rol = db.Rol.FirstOrDefault(fod => fod.id == detailFrameworkContract.id_rol);
                                        tempFrameworkContract.FrameworkContractItem = new List<FrameworkContractItem>();
                                        tempFrameworkContract.FrameworkContractExtension = new List<FrameworkContractExtension>();
                                        //FrameworkContractItem
                                        if (detailFrameworkContract.FrameworkContractItem != null)
                                        {
                                            foreach (var detailFrameworkContractItem in detailFrameworkContract.FrameworkContractItem)
                                            {
                                                var tempFrameworkContractItem = new FrameworkContractItem();
                                                tempFrameworkContractItem.FrameworkContract = tempFrameworkContract;
                                                tempFrameworkContractItem.id_item = detailFrameworkContractItem.id_item;
                                                tempFrameworkContractItem.Item = db.Item.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_item);
                                                tempFrameworkContractItem.startDate = detailFrameworkContractItem.startDate;
                                                tempFrameworkContractItem.endDate = detailFrameworkContractItem.endDate;
                                                tempFrameworkContractItem.value = detailFrameworkContractItem.value;
                                                tempFrameworkContractItem.amout = detailFrameworkContractItem.amout;
                                                tempFrameworkContractItem.id_metricUnit = detailFrameworkContractItem.id_metricUnit;
                                                tempFrameworkContractItem.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_metricUnit);
                                                tempFrameworkContractItem.FrameworkContractDeliveryPlan = new List<FrameworkContractDeliveryPlan>();
                                                tempFrameworkContractItem.FrameworkContractExtension = new List<FrameworkContractExtension>();
                                                //FrameworkContractDeliveryPlan
                                                if (detailFrameworkContractItem.FrameworkContractDeliveryPlan != null)
                                                {
                                                    foreach (var detailFrameworkContractDeliveryPlan in detailFrameworkContractItem.FrameworkContractDeliveryPlan)
                                                    {
                                                        var tempFrameworkContractDeliveryPlan = new FrameworkContractDeliveryPlan();
                                                        tempFrameworkContractDeliveryPlan.FrameworkContractItem = tempFrameworkContractItem;
                                                        tempFrameworkContractDeliveryPlan.deliveryPlanDate = detailFrameworkContractDeliveryPlan.deliveryPlanDate;
                                                        tempFrameworkContractDeliveryPlan.amout = detailFrameworkContractDeliveryPlan.amout;

                                                        tempFrameworkContractItem.FrameworkContractDeliveryPlan.Add(tempFrameworkContractDeliveryPlan);
                                                    }
                                                }
                                                //FrameworkContractExtension
                                                if (detailFrameworkContractItem.FrameworkContractExtension != null)
                                                {
                                                    foreach (var detailFrameworkContractExtension in detailFrameworkContractItem.FrameworkContractExtension)
                                                    {
                                                        var tempFrameworkContractExtension = new FrameworkContractExtension();
                                                        tempFrameworkContractExtension.FrameworkContract = tempFrameworkContract;
                                                        tempFrameworkContractExtension.FrameworkContractItem = tempFrameworkContractItem;
                                                        tempFrameworkContractExtension.value = detailFrameworkContractExtension.value;
                                                        tempFrameworkContractExtension.amout = detailFrameworkContractExtension.amout;

                                                        tempFrameworkContractExtension.isSave = true;

                                                        tempFrameworkContract.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                        tempFrameworkContractItem.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                    }
                                                }

                                                tempFrameworkContract.FrameworkContractItem.Add(tempFrameworkContractItem);
                                            }
                                        }
                                        modelItem.FrameworkContract.Add(tempFrameworkContract);
                                    }
                                    else
                                    {
                                        //#region Document
                                        //Document document = new Document();
                                        //document = ServiceDocument.CreateDocument(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, "62", "Contrato Marco");
                                        ////Actualizando Estado del Documento FrameworkContract
                                        //var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                                        //document.id_documentState = documentState.id;
                                        //document.DocumentState = documentState;
                                        //#endregion

                                        //tempFrameworkContract = new FrameworkContract();
                                        //tempFrameworkContract.id = document.id;
                                        //tempFrameworkContract.Document = document;
                                        //tempFrameworkContract.id_person = modelItem.id;
                                        //tempFrameworkContract.Person = modelItem;
                                        tempFrameworkContract.Document.id_documentState = detailFrameworkContract.Document.id_documentState;
                                        tempFrameworkContract.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.id == detailFrameworkContract.Document.id_documentState);
                                        tempFrameworkContract.id_typeContractFramework = detailFrameworkContract.id_typeContractFramework;
                                        tempFrameworkContract.TypeContractFramework = db.TypeContractFramework.FirstOrDefault(fod => fod.id == detailFrameworkContract.id_typeContractFramework);
                                        tempFrameworkContract.id_company = detailFrameworkContract.id_company;
                                        //tempFrameworkContract.id_rol = detailFrameworkContract.id_rol;
                                        //tempFrameworkContract.Rol = db.Rol.FirstOrDefault(fod => fod.id == detailFrameworkContract.id_rol);
                                        //tempFrameworkContract.FrameworkContractItem = new List<FrameworkContractItem>();
                                        if (tempFrameworkContract.FrameworkContractExtension == null)
                                        {
                                            tempFrameworkContract.FrameworkContractExtension = new List<FrameworkContractExtension>();
                                        }

                                        //FrameworkContractItem
                                        if (tempFrameworkContract.FrameworkContractItem == null)
                                        {
                                            for (int i = tempFrameworkContract.FrameworkContractItem.Count - 1; i >= 0; i--)
                                            {
                                                var detail = tempFrameworkContract.FrameworkContractItem.ElementAt(i);
                                                if (detailFrameworkContract.FrameworkContractItem == null ||
                                                    detailFrameworkContract.FrameworkContractItem.FirstOrDefault(fod => fod.id == detail.id) == null)
                                                {
                                                    //Verificar que el detalle de item no este usado en ningun Requerimiento de Pedido Aprobado o ninguna Orden de compra aprobada
                                                    for (int j = detail.FrameworkContractDeliveryPlan.Count - 1; j >= 0; j--)
                                                    {
                                                        var detailFrameworkContractDeliveryPlan = detail.FrameworkContractDeliveryPlan.ElementAt(j);
                                                        detail.FrameworkContractDeliveryPlan.Remove(detailFrameworkContractDeliveryPlan);
                                                        db.Entry(detailFrameworkContractDeliveryPlan).State = EntityState.Deleted;

                                                    }

                                                    for (int j = detail.FrameworkContractExtension.Count - 1; j >= 0; j--)
                                                    {
                                                        var detailFrameworkContractExtension = detail.FrameworkContractExtension.ElementAt(j);
                                                        detail.FrameworkContractExtension.Remove(detailFrameworkContractExtension);
                                                        tempFrameworkContract.FrameworkContractExtension.Remove(detailFrameworkContractExtension);
                                                        db.Entry(detailFrameworkContractExtension).State = EntityState.Deleted;

                                                    }

                                                    tempFrameworkContract.FrameworkContractItem.Remove(detail);
                                                    db.Entry(detail).State = EntityState.Deleted;
                                                }
                                            }
                                        }

                                        if (detailFrameworkContract.FrameworkContractItem != null)
                                        {
                                            foreach (var detailFrameworkContractItem in detailFrameworkContract.FrameworkContractItem)
                                            {
                                                var tempFrameworkContractItem = tempFrameworkContract.FrameworkContractItem.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id);
                                                if (tempFrameworkContractItem == null)
                                                {
                                                    tempFrameworkContractItem = new FrameworkContractItem();
                                                    tempFrameworkContractItem.FrameworkContract = tempFrameworkContract;
                                                    tempFrameworkContractItem.id_item = detailFrameworkContractItem.id_item;
                                                    tempFrameworkContractItem.Item = db.Item.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_item);
                                                    tempFrameworkContractItem.startDate = detailFrameworkContractItem.startDate;
                                                    tempFrameworkContractItem.endDate = detailFrameworkContractItem.endDate;
                                                    tempFrameworkContractItem.value = detailFrameworkContractItem.value;
                                                    tempFrameworkContractItem.amout = detailFrameworkContractItem.amout;
                                                    tempFrameworkContractItem.id_metricUnit = detailFrameworkContractItem.id_metricUnit;
                                                    tempFrameworkContractItem.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_metricUnit);
                                                    tempFrameworkContractItem.FrameworkContractDeliveryPlan = new List<FrameworkContractDeliveryPlan>();
                                                    tempFrameworkContractItem.FrameworkContractExtension = new List<FrameworkContractExtension>();
                                                    //FrameworkContractDeliveryPlan
                                                    if (detailFrameworkContractItem.FrameworkContractDeliveryPlan != null)
                                                    {
                                                        foreach (var detailFrameworkContractDeliveryPlan in detailFrameworkContractItem.FrameworkContractDeliveryPlan)
                                                        {
                                                            var tempFrameworkContractDeliveryPlan = new FrameworkContractDeliveryPlan();
                                                            tempFrameworkContractDeliveryPlan.FrameworkContractItem = tempFrameworkContractItem;
                                                            tempFrameworkContractDeliveryPlan.deliveryPlanDate = detailFrameworkContractDeliveryPlan.deliveryPlanDate;
                                                            tempFrameworkContractDeliveryPlan.amout = detailFrameworkContractDeliveryPlan.amout;

                                                            tempFrameworkContractItem.FrameworkContractDeliveryPlan.Add(tempFrameworkContractDeliveryPlan);
                                                        }
                                                    }
                                                    //FrameworkContractExtension
                                                    if (detailFrameworkContractItem.FrameworkContractExtension != null)
                                                    {
                                                        foreach (var detailFrameworkContractExtension in detailFrameworkContractItem.FrameworkContractExtension)
                                                        {
                                                            var tempFrameworkContractExtension = new FrameworkContractExtension();
                                                            tempFrameworkContractExtension.FrameworkContract = tempFrameworkContract;
                                                            tempFrameworkContractExtension.FrameworkContractItem = tempFrameworkContractItem;
                                                            tempFrameworkContractExtension.value = detailFrameworkContractExtension.value;
                                                            tempFrameworkContractExtension.amout = detailFrameworkContractExtension.amout;

                                                            tempFrameworkContractExtension.isSave = true;

                                                            tempFrameworkContract.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                            tempFrameworkContractItem.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                        }
                                                    }

                                                    tempFrameworkContract.FrameworkContractItem.Add(tempFrameworkContractItem);
                                                }
                                                else
                                                {
                                                    //tempFrameworkContractItem = new FrameworkContractItem();
                                                    //tempFrameworkContractItem.FrameworkContract = tempFrameworkContract;
                                                    tempFrameworkContractItem.id_item = detailFrameworkContractItem.id_item;
                                                    tempFrameworkContractItem.Item = db.Item.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_item);
                                                    tempFrameworkContractItem.startDate = detailFrameworkContractItem.startDate;
                                                    tempFrameworkContractItem.endDate = detailFrameworkContractItem.endDate;
                                                    tempFrameworkContractItem.value = detailFrameworkContractItem.value;
                                                    tempFrameworkContractItem.amout = detailFrameworkContractItem.amout;
                                                    tempFrameworkContractItem.id_metricUnit = detailFrameworkContractItem.id_metricUnit;
                                                    tempFrameworkContractItem.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == detailFrameworkContractItem.id_metricUnit);
                                                    if (tempFrameworkContractItem.FrameworkContractDeliveryPlan == null)
                                                    {
                                                        tempFrameworkContractItem.FrameworkContractDeliveryPlan = new List<FrameworkContractDeliveryPlan>();
                                                    }
                                                    //tempFrameworkContractItem.FrameworkContractDeliveryPlan = new List<FrameworkContractDeliveryPlan>();
                                                    if (tempFrameworkContractItem.FrameworkContractExtension == null)
                                                    {
                                                        tempFrameworkContractItem.FrameworkContractExtension = new List<FrameworkContractExtension>();
                                                    }
                                                    //tempFrameworkContractItem.FrameworkContractExtension = new List<FrameworkContractExtension>();
                                                    //FrameworkContractDeliveryPlan
                                                    if (tempFrameworkContractItem.FrameworkContractDeliveryPlan == null)
                                                    {
                                                        for (int i = tempFrameworkContractItem.FrameworkContractDeliveryPlan.Count - 1; i >= 0; i--)
                                                        {
                                                            var detail = tempFrameworkContractItem.FrameworkContractDeliveryPlan.ElementAt(i);
                                                            if (detailFrameworkContractItem.FrameworkContractDeliveryPlan == null ||
                                                                detailFrameworkContractItem.FrameworkContractDeliveryPlan.FirstOrDefault(fod => fod.id == detail.id) == null)
                                                            {
                                                                //Verificar que el detalle de item no este usado en ningun Requerimiento de Pedido Aprobado o ninguna Orden de compra aprobada
                                                                tempFrameworkContractItem.FrameworkContractDeliveryPlan.Remove(detail);
                                                                db.Entry(detail).State = EntityState.Deleted;
                                                            }
                                                        }
                                                    }
                                                    if (detailFrameworkContractItem.FrameworkContractDeliveryPlan != null)
                                                    {
                                                        foreach (var detailFrameworkContractDeliveryPlan in detailFrameworkContractItem.FrameworkContractDeliveryPlan)
                                                        {
                                                            var tempFrameworkContractDeliveryPlan = tempFrameworkContractItem.FrameworkContractDeliveryPlan.FirstOrDefault(fod => fod.id == detailFrameworkContractDeliveryPlan.id);
                                                            if (tempFrameworkContractDeliveryPlan == null)
                                                            {
                                                                tempFrameworkContractDeliveryPlan = new FrameworkContractDeliveryPlan();
                                                                tempFrameworkContractDeliveryPlan.FrameworkContractItem = tempFrameworkContractItem;
                                                                tempFrameworkContractDeliveryPlan.deliveryPlanDate = detailFrameworkContractDeliveryPlan.deliveryPlanDate;
                                                                tempFrameworkContractDeliveryPlan.amout = detailFrameworkContractDeliveryPlan.amout;

                                                                tempFrameworkContractItem.FrameworkContractDeliveryPlan.Add(tempFrameworkContractDeliveryPlan);
                                                            }
                                                            else
                                                            {
                                                                //tempFrameworkContractDeliveryPlan = new FrameworkContractDeliveryPlan();
                                                                //tempFrameworkContractDeliveryPlan.FrameworkContractItem = tempFrameworkContractItem;
                                                                tempFrameworkContractDeliveryPlan.deliveryPlanDate = detailFrameworkContractDeliveryPlan.deliveryPlanDate;
                                                                tempFrameworkContractDeliveryPlan.amout = detailFrameworkContractDeliveryPlan.amout;

                                                                db.Entry(tempFrameworkContractDeliveryPlan).State = EntityState.Modified;
                                                                //tempFrameworkContractItem.FrameworkContractDeliveryPlan.Add(tempFrameworkContractDeliveryPlan);
                                                            }

                                                        }
                                                    }
                                                    //FrameworkContractExtension
                                                    if (tempFrameworkContractItem.FrameworkContractExtension == null)
                                                    {
                                                        for (int i = tempFrameworkContractItem.FrameworkContractExtension.Count - 1; i >= 0; i--)
                                                        {
                                                            var detail = tempFrameworkContractItem.FrameworkContractExtension.ElementAt(i);
                                                            if (detailFrameworkContractItem.FrameworkContractExtension == null ||
                                                                detailFrameworkContractItem.FrameworkContractExtension.FirstOrDefault(fod => fod.id == detail.id) == null)
                                                            {
                                                                //Verificar que el detalle de item no este usado en ningun Requerimiento de Pedido Aprobado o ninguna Orden de compra aprobada
                                                                tempFrameworkContractItem.FrameworkContractExtension.Remove(detail);
                                                                db.Entry(detail).State = EntityState.Deleted;
                                                            }
                                                        }
                                                    }
                                                    if (detailFrameworkContractItem.FrameworkContractExtension != null)
                                                    {
                                                        foreach (var detailFrameworkContractExtension in detailFrameworkContractItem.FrameworkContractExtension)
                                                        {
                                                            var tempFrameworkContractExtension = tempFrameworkContractItem.FrameworkContractExtension.FirstOrDefault(fod => fod.id == detailFrameworkContractExtension.id);
                                                            if (tempFrameworkContractExtension == null)
                                                            {
                                                                tempFrameworkContractExtension = new FrameworkContractExtension();
                                                                tempFrameworkContractExtension.FrameworkContract = tempFrameworkContract;
                                                                tempFrameworkContractExtension.FrameworkContractItem = tempFrameworkContractItem;
                                                                tempFrameworkContractExtension.value = detailFrameworkContractExtension.value;
                                                                tempFrameworkContractExtension.amout = detailFrameworkContractExtension.amout;

                                                                tempFrameworkContractExtension.isSave = true;

                                                                tempFrameworkContract.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                                tempFrameworkContractItem.FrameworkContractExtension.Add(tempFrameworkContractExtension);
                                                            }
                                                            else
                                                            {
                                                                //tempFrameworkContractExtension = new FrameworkContractExtension();
                                                                //tempFrameworkContractExtension.FrameworkContract = tempFrameworkContract;
                                                                //tempFrameworkContractExtension.FrameworkContractItem = tempFrameworkContractItem;
                                                                tempFrameworkContractExtension.value = detailFrameworkContractExtension.value;
                                                                tempFrameworkContractExtension.amout = detailFrameworkContractExtension.amout;

                                                                db.Entry(tempFrameworkContractExtension).State = EntityState.Modified;
                                                            }
                                                        }
                                                    }

                                                    db.Entry(tempFrameworkContractItem).State = EntityState.Modified;
                                                    //tempFrameworkContract.FrameworkContractItem.Add(tempFrameworkContractItem);
                                                }

                                            }
                                        }
                                        db.Entry(tempFrameworkContract).State = EntityState.Modified;
                                        //modelItem.FrameworkContract.Add(tempFrameworkContract);
                                    }
                                }
                            }

                            #endregion

                            #region {RA}  -- Comentado Error procedimiento anulacion de rol
                            // Decrecated: 2018-01-03


                            //"Proveedor"
                            //var _rolProvider = db.Rol.FirstOrDefault(r => r.name == "Proveedor");
                            //if (!id_roles.Contains(_rolProvider.id))
                            //{
                            //    #region Delete Provider

                            //    if (modelItem.Provider != null)
                            //    {
                            //        //ProviderGeneralData
                            //        if (modelItem.Provider.ProviderGeneralData != null)
                            //        {
                            //            var detail = modelItem.Provider.ProviderGeneralData;
                            //            db.ProviderGeneralData.Remove(detail);
                            //            db.Entry(detail).State = EntityState.Deleted;
                            //        }

                            //        //ProviderGeneralDataEP
                            //        if (modelItem.Provider.ProviderGeneralDataEP != null)
                            //        {
                            //            var detail = modelItem.Provider.ProviderGeneralDataEP;
                            //            db.ProviderGeneralDataEP.Remove(detail);
                            //            db.Entry(detail).State = EntityState.Deleted;
                            //        }

                            //        //ProviderGeneralDataRise
                            //        if (modelItem.Provider.ProviderGeneralDataRise != null)
                            //        {
                            //            var detail = modelItem.Provider.ProviderGeneralDataRise;
                            //            db.ProviderGeneralDataRise.Remove(detail);
                            //            db.Entry(detail).State = EntityState.Deleted;
                            //        }

                            //        //ProviderPaymentTerm
                            //        if (modelItem.Provider.ProviderPaymentTerm != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderPaymentTerm.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderPaymentTerm.ElementAt(i);
                            //                modelItem.Provider.ProviderPaymentTerm.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderPaymentMethod
                            //        if (modelItem.Provider.ProviderPaymentMethod != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderPaymentMethod.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderPaymentMethod.ElementAt(i);
                            //                modelItem.Provider.ProviderPaymentMethod.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderSeriesForDocuments
                            //        if (modelItem.Provider.ProviderSeriesForDocuments != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderSeriesForDocuments.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderSeriesForDocuments.ElementAt(i);
                            //                modelItem.Provider.ProviderSeriesForDocuments.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderRetention
                            //        if (modelItem.Provider.ProviderRetention != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderRetention.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderRetention.ElementAt(i);
                            //                modelItem.Provider.ProviderRetention.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderPersonAuthorizedToPayTheBill
                            //        if (modelItem.Provider.ProviderPersonAuthorizedToPayTheBill != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderPersonAuthorizedToPayTheBill.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderPersonAuthorizedToPayTheBill.ElementAt(i);
                            //                modelItem.Provider.ProviderPersonAuthorizedToPayTheBill.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderRelatedCompany
                            //        if (modelItem.Provider.ProviderRelatedCompany != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderRelatedCompany.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderRelatedCompany.ElementAt(i);
                            //                modelItem.Provider.ProviderRelatedCompany.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderAccountingAccounts
                            //        if (modelItem.Provider.ProviderAccountingAccounts != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderAccountingAccounts.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderAccountingAccounts.ElementAt(i);
                            //                modelItem.Provider.ProviderAccountingAccounts.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderItem
                            //        if (modelItem.Provider.ProviderItem != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderItem.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderItem.ElementAt(i);
                            //                modelItem.Provider.ProviderItem.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }
                            //        //ProviderMailByComDivBra
                            //        if (modelItem.Provider.ProviderMailByComDivBra != null)
                            //        {
                            //            for (int i = modelItem.Provider.ProviderMailByComDivBra.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.Provider.ProviderMailByComDivBra.ElementAt(i);
                            //                modelItem.Provider.ProviderMailByComDivBra.Remove(detail);
                            //                db.Entry(detail).State = EntityState.Deleted;
                            //            }
                            //        }

                            //        //FrameworkContract
                            //        if (modelItem.FrameworkContract != null)
                            //        {
                            //            for (int i = modelItem.FrameworkContract.Count - 1; i >= 0; i--)
                            //            {
                            //                var detail = modelItem.FrameworkContract.ElementAt(i);
                            //                //Verificar que no este usado el contrato en ninguna orden de Compra o Requerimiento de Pedido o de Venta
                            //                if (detail.Rol.name.Equals(_rolProvider.name))
                            //                {
                            //                    //Actualizando Estado del Documento FrameworkContract
                            //                    var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //ANULADA
                            //                    detail.Document.id_documentState = documentState.id;
                            //                    detail.Document.DocumentState = documentState;

                            //                    db.FrameworkContract.Attach(detail);
                            //                    db.Entry(detail).State = EntityState.Modified;
                            //                }
                            //            }

                            //        }

                            //        db.Provider.Remove(modelItem.Provider);
                            //    }
                            //    #endregion
                            //}
                            #endregion

                            #region Cancel FrameworkContract

                            //"Cliente Local"
                            var _rolCostumerLocal = db.Rol.FirstOrDefault(r => r.name == "Cliente Local");
                            if (!id_roles.Contains(_rolCostumerLocal.id))
                            {
                                //FrameworkContract
                                if (modelItem.FrameworkContract != null)
                                {
                                    for (int i = modelItem.FrameworkContract.Count - 1; i >= 0; i--)
                                    {
                                        var detail = modelItem.FrameworkContract.ElementAt(i);
                                        //Verificar que no este usado el contrato en ninguna orden de Compra o Requerimiento de Pedido o de Venta
                                        if (detail.Rol.name.Equals(_rolCostumerLocal.name))
                                        {
                                            //Actualizando Estado del Documento FrameworkContract
                                            var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //ANULADA
                                            detail.Document.id_documentState = documentState.id;
                                            detail.Document.DocumentState = documentState;

                                            db.FrameworkContract.Attach(detail);
                                            db.Entry(detail).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            //"Cliente Exterior"
                            var _rolCostumerExterior = db.Rol.FirstOrDefault(r => r.name == "Cliente Exterior");
                            if (!id_roles.Contains(_rolCostumerLocal.id))
                            {
                                //FrameworkContract
                                if (modelItem.FrameworkContract != null)
                                {
                                    for (int i = modelItem.FrameworkContract.Count - 1; i >= 0; i--)
                                    {
                                        var detail = modelItem.FrameworkContract.ElementAt(i);
                                        //Verificar que no este usado el contrato en ninguna orden de Compra o Requerimiento de Pedido o de Venta
                                        if (detail.Rol.name.Equals(_rolCostumerExterior.name))
                                        {
                                            //Actualizando Estado del Documento FrameworkContract
                                            var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //ANULADA
                                            detail.Document.id_documentState = documentState.id;
                                            detail.Document.DocumentState = documentState;

                                            db.FrameworkContract.Attach(detail);
                                            db.Entry(detail).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            #endregion

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
                        modelItem.firm = item.firm;
                        modelItem.address = item.address.Trim();
                        modelItem.email = item.email;
                        modelItem.isActive = item.isActive;
                        modelItem.bCC = item.bCC;
                        modelItem.processPlant = item.processPlant;
                        modelItem.isCopacking = item.isCopacking;

                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        db.Person.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        dbCI.SaveChanges();
                        idTmp = id_person;
                        isSaved = true;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    TempData["ErrorMessagePerson"] = ErrorMessage("Error al modificar la persona: " + e.Message);
                    trans.Rollback();
                }
            }
            if (modelItem == null)
            {
                return PersonsPartialAddNew(item, provider, providerGeneralData, providerTypeShrimp, providerTransportist, providerGeneralDataEP, providerGeneralDataRise, employee, customer, customerContact, customerCreditInfo,
                    providerRawMaterial, id_roles);
            }

            #region Replicate Person to PriceList
            try
            {
                if (isSaved)
                {
                    #region Replicate Person to PriceList
                    if (isSaved)
                    {
                        #region 
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = _pathProgramReplication,
                            Arguments = modelItem.id.ToString() + " RPPLP ReplicateInformation",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        Process.Start(startInfo);
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "PersonUpdateReplication", "PROD");
            }
            #endregion

            RefreesProductionUnitProviderPoolBachList(id_person);
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

        #region PROVIDER SHRIMP UNIT PROVIDER

        [ValidateInput(false)]
        public ActionResult ProductionShrimpUnitProvider(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProductionUnitProvider?.ToList() ?? new List<ProductionUnitProvider>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            this.ViewBag.isCopackingPerson = person.isCopacking;
            return PartialView("_ProviderShrimpUnitProvider", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionShrimpUnitProviderAddNew(int id_person, ProductionUnitProvider productionUnitProvider)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();

            if (ModelState.IsValid)
            {
                try
                {
                    productionUnitProvider.id = person.Provider.ProductionUnitProvider.Count() > 0 ? person.Provider.ProductionUnitProvider.Max(pld => pld.id) + 1 : 1;

                    person.Provider.ProductionUnitProvider.Add(productionUnitProvider);
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

            var model = person.Provider.ProductionUnitProvider?.ToList() ?? new List<ProductionUnitProvider>();
            return PartialView("_ProviderShrimpUnitProvider", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionShrimpUnitProviderUpdate(ProductionUnitProvider productionUnitProvider)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == productionUnitProvider.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            //if (ModelState.IsValid && person != null)
            //{
            try
            {
                var modelPerson = person.Provider.ProductionUnitProvider.FirstOrDefault(i => i.id == productionUnitProvider.id);
                if (modelPerson != null)
                {
                    this.UpdateModel(modelPerson);

                    if (!productionUnitProvider.isActive)
                    {
                        modelPerson.isActive = false;
                    }
                    else
                    {
                        modelPerson.isActive = true;
                    }
                }
                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}
            //else
            //    ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");

            var model = person.Provider.ProductionUnitProvider?.ToList() ?? new List<ProductionUnitProvider>();
            return PartialView("_ProviderShrimpUnitProvider", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionShrimpUnitProviderDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();
            if (person.isCopacking == null)
            {
                this.ViewBag.CopackingPerson = false;
            }
            else
            {
                this.ViewBag.CopackingPerson = person.isCopacking;
            }

            try
            {
                var productionUnitProvider = person.Provider.ProductionUnitProvider.FirstOrDefault(it => it.id == id);
                if (productionUnitProvider != null)
                    person.Provider.ProductionUnitProvider.Remove(productionUnitProvider);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProductionUnitProvider?.ToList() ?? new List<ProductionUnitProvider>();
            return PartialView("_ProviderShrimpUnitProvider", model);
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

        #region ProviderItem

        [ValidateInput(false)]
        public ActionResult ProviderItem(int id_person)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var model = person.Provider.ProviderItem?.ToList() ?? new List<ProviderItem>();
            TempData["person"] = TempData["person"] ?? person;
            TempData.Keep("person");
            return PartialView("_FormEditProviderItem", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderItemAddNew(int id_person, ProviderItem providerItem)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid)
            {
                try
                {
                    providerItem.id = person.Provider.ProviderItem.Count() > 0 ? person.Provider.ProviderItem.Max(pld => pld.id) + 1 : 1;
                    person.Provider.ProviderItem.Add(providerItem);
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

            var model = person.Provider.ProviderItem?.ToList() ?? new List<ProviderItem>();
            return PartialView("_FormEditProviderItem", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderItemUpdate(ProviderItem providerItem)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == providerItem.id_provider);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelPerson = person.Provider.ProviderItem.FirstOrDefault(i => i.id == providerItem.id);
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

            var model = person.Provider.ProviderItem?.ToList() ?? new List<ProviderItem>();
            return PartialView("_FormEditProviderItem", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderItemDelete(int id_person, int id)
        {
            Person person = (TempData["person"] as Person);

            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            try
            {
                var providerItem = person.Provider.ProviderItem.FirstOrDefault(it => it.id == id);
                if (providerItem != null)
                    person.Provider.ProviderItem.Remove(providerItem);

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");

            var model = person.Provider.ProviderItem?.ToList() ?? new List<ProviderItem>();
            return PartialView("_FormEditProviderItem", model);
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

        #region PERSONS FRAMEWORK CONTRACT

        [ValidateInput(false)]
        public ActionResult PersonFrameworkContractView(int id_person)
        {
            //ViewData["id_person"] = id_person;
            FrameworkContract tempFrameworkContract = (TempData["frameworkContract"] as FrameworkContract);
            if (tempFrameworkContract != null)
            {
                TempData.Remove("frameworkContract");
            }
            Person person = GetPersonCurrent(id_person);

            //Person person = (TempData["person"] as Person);
            //person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            //person = person ?? new Person();

            //var person = db.Person.FirstOrDefault(p => p.id == id_person);
            var frameworkContract = person.FrameworkContract.ToList();
            var model = frameworkContract ?? new List<FrameworkContract>();

            //var model = person.FrameworkContract.ToList() ?? new List<FrameworkContract>();

            //SetViewData();

            return PartialView("_FrameworkContractViewPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult PersonFrameworkContract(int id_person)
        {
            FrameworkContract tempFrameworkContract = (TempData["frameworkContract"] as FrameworkContract);
            if (tempFrameworkContract != null)
            {
                TempData.Remove("frameworkContract");
            }

            Person person = GetPersonCurrent(id_person);

            var frameworkContract = person.FrameworkContract.ToList();
            var model = frameworkContract ?? new List<FrameworkContract>();

            //TempData.Keep("person");

            return PartialView("_FormEditFrameworkContract", model.OrderByDescending(od => od.id).ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractAddNew(int id_person, FrameworkContract item)
        {

            //Person person = GetPersonCurrent(id_person);
            //FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);

            //var frameworkContractItem = frameworkContract.FrameworkContractItem.ToList();
            //frameworkContract.FrameworkContractItem = frameworkContractItem ?? new List<FrameworkContractItem>();

            Person person = GetPersonCurrent(id_person);

            var frameworkContractCurrent = GetFrameworkContractCurrent(id_person, 0);

            var frameworkContract = person.FrameworkContract.ToList();
            person.FrameworkContract = frameworkContract ?? new List<FrameworkContract>();

            if (ModelState.IsValid)
            {
                try
                {
                    frameworkContractCurrent.id = person.FrameworkContract.Count() > 0 ? person.FrameworkContract.Max(pld => pld.id) + 1 : 1;
                    //var phaseName = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == item.id_businessOportunityDocumentTypePhase)?.Phase.name ?? "";
                    //item.phaseName = phaseName;
                    frameworkContractCurrent.id_person = id_person;
                    frameworkContractCurrent.Person = db.Person.FirstOrDefault(fod => fod.id == id_person);

                    frameworkContractCurrent.id_typeContractFramework = item.id_typeContractFramework;
                    frameworkContractCurrent.TypeContractFramework = db.TypeContractFramework.FirstOrDefault(fod => fod.id == item.id_typeContractFramework);

                    frameworkContractCurrent.id_company = item.id_company;

                    frameworkContractCurrent.id_rol = item.id_rol;
                    frameworkContractCurrent.Rol = db.Rol.FirstOrDefault(fod => fod.id == item.id_rol);

                    Document document = new Document();
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE
                    if (documentState == null)
                    {
                        throw new Exception("No se puedo Crear el Documento Lote porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                    }
                    document.id_documentState = documentState.id;
                    document.DocumentState = documentState;
                    frameworkContractCurrent.Document = document;
                    //item.FrameworkContractItem = (TempData["frameworkContractItem"] as List<FrameworkContractItem>);
                    //item.FrameworkContractItem = item.FrameworkContractItem ?? new List<FrameworkContractItem>();

                    //item.FrameworkContractExtension = (TempData["frameworkContractExtension"] as List<FrameworkContractExtension>);
                    //item.FrameworkContractExtension = item.FrameworkContractExtension ?? new List<FrameworkContractExtension>();

                    person.FrameworkContract.Add(frameworkContractCurrent);
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
            //TempData.Keep("frameworkContractItem");
            //TempData.Keep("frameworkContractExtension");
            var model = person.FrameworkContract;
            //SetViewData();

            return PartialView("_FormEditFrameworkContract", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractUpdate(int id_person, FrameworkContract item)
        {
            Person person = GetPersonCurrent(id_person);

            var frameworkContract = person.FrameworkContract.ToList();
            person.FrameworkContract = frameworkContract ?? new List<FrameworkContract>();

            //var model = db.BusinessOportunityNote;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = person.FrameworkContract.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        //var phaseName = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == item.id_businessOportunityDocumentTypePhase)?.Phase.name ?? "";
                        //modelItem.phaseName = phaseName;
                        //item.id_person = id_person;
                        //item.Person = db.Person.FirstOrDefault(fod => fod.id == id_person);

                        modelItem.id_typeContractFramework = item.id_typeContractFramework;
                        modelItem.TypeContractFramework = db.TypeContractFramework.FirstOrDefault(fod => fod.id == item.id_typeContractFramework);

                        modelItem.id_company = item.id_company;

                        //modelItem.FrameworkContractItem = (TempData["frameworkContractItem"] as List<FrameworkContractItem>);
                        //modelItem.FrameworkContractItem = modelItem.FrameworkContractItem ?? new List<FrameworkContractItem>();

                        //modelItem.FrameworkContractExtension = (TempData["frameworkContractExtension"] as List<FrameworkContractExtension>);
                        //modelItem.FrameworkContractExtension = modelItem.FrameworkContractExtension ?? new List<FrameworkContractExtension>();
                        //item.phaseName = phaseName;
                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
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
            //TempData.Keep("frameworkContractItem");
            //TempData.Keep("frameworkContractExtension");
            var model = person.FrameworkContract;
            //SetViewData();

            return PartialView("_FormEditFrameworkContract", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractDelete(int id_person, System.Int32 id)
        {
            Person person = GetPersonCurrent(id_person);

            var frameworkContract = person.FrameworkContract.ToList();
            person.FrameworkContract = frameworkContract ?? new List<FrameworkContract>();

            try
            {
                var item = person.FrameworkContract.FirstOrDefault(it => it.id == id);
                if (item != null /*Verificar que el contrato no este utilizado en ningun Orden de Pedido o de Venta o ningún Requrimiento de Compra */)
                {
                    if (item.Document.DocumentState.code.Equals("01"))//PENDIENTE
                    {
                        ClearFrameworkContractDetail(id_person, id);
                        person.FrameworkContract.Remove(item);

                    }
                    else
                    {
                        if (item.Document.DocumentState.code.Equals("03"))//APROBADO
                        {
                            Document document = new Document();
                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");//Anulado
                            if (documentState == null)
                            {
                                throw new Exception("No se puedo Crear el Documento Lote porque no existe el Estado de Documento: Anulado con Código: 05, configúrelo e inténtelo de nuevo");
                            }
                            document.id_documentState = documentState.id;
                            document.DocumentState = documentState;
                            item.Document = document;
                            this.UpdateModel(item);
                        }
                        else
                        {
                            throw new Exception("No se puedo eliminar un contrato anulado");
                        }
                    }

                }

                TempData["person"] = person;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");
            //TempData.Keep("frameworkContractItem");
            //TempData.Keep("frameworkContractExtension");
            var model = person.FrameworkContract;
            //SetViewData();

            return PartialView("_FormEditFrameworkContract", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region PERSONS FRAMEWORK CONTRACT ITEM

        [ValidateInput(false)]
        public ActionResult PersonFrameworkContractItem(int id_person, int id_frameworkContract, string code_typeContractFramework, string code_documentState)
        {
            ViewData["code_typeContractFramework"] = code_typeContractFramework;
            ViewData["code_documentState"] = code_documentState;

            FrameworkContractItem tempFrameworkContractItem = (TempData["frameworkContractItem"] as FrameworkContractItem);
            if (tempFrameworkContractItem != null)
            {
                TempData.Remove("frameworkContractItem");
            }

            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);

            var frameworkContractItem = frameworkContract.FrameworkContractItem.ToList();
            var model = frameworkContractItem ?? new List<FrameworkContractItem>();

            return PartialView("_FormEditFrameworkContractItem", model.OrderBy(od => od.id).ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractItemAddNew(int id_person, int id_frameworkContract, FrameworkContractItem item,
                                                        int id_itemFrameworkContractItem, DateTime startDateFrameworkContractItem, DateTime endDateFrameworkContractItem,
                                                        decimal? valueFrameworkContractItem, decimal? amoutFrameworkContractItem, int? id_metricUnitFrameworkContractItem, string code_typeContractFramework)
        {
            ViewData["code_typeContractFramework"] = code_typeContractFramework;

            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);
            item = GetFrameworkContractItemCurrent(id_person, id_frameworkContract, 0);

            var frameworkContractItem = frameworkContract.FrameworkContractItem.ToList();
            frameworkContract.FrameworkContractItem = frameworkContractItem ?? new List<FrameworkContractItem>();

            if (ModelState.IsValid)
            {
                try
                {
                    item.id = frameworkContract.FrameworkContractItem.Count() > 0 ? frameworkContract.FrameworkContractItem.Max(pld => pld.id) + 1 : 1;
                    //var phaseName = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == item.id_businessOportunityDocumentTypePhase)?.Phase.name ?? "";
                    //item.phaseName = phaseName;
                    item.id_frameworkContract = id_frameworkContract;
                    item.FrameworkContract = frameworkContract;
                    item.id_item = id_itemFrameworkContractItem;
                    item.Item = db.Item.FirstOrDefault(fod => fod.id == id_itemFrameworkContractItem);
                    item.startDate = startDateFrameworkContractItem;
                    item.endDate = endDateFrameworkContractItem;
                    item.value = valueFrameworkContractItem;
                    item.amout = amoutFrameworkContractItem;
                    item.id_metricUnit = id_metricUnitFrameworkContractItem;
                    item.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == item.id_metricUnit);

                    frameworkContract.FrameworkContractItem.Add(item);

                    TempData["person"] = person;
                    TempData["frameworkContract"] = frameworkContract;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            var model = frameworkContract.FrameworkContractItem;

            return PartialView("_FormEditFrameworkContractItem", model.OrderBy(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractItemUpdate(int id_person, int id_frameworkContract, FrameworkContractItem item,
                                                        int? id_itemFrameworkContractItem, DateTime? startDateFrameworkContractItem, DateTime? endDateFrameworkContractItem,
                                                        decimal? valueFrameworkContractItem, decimal? amoutFrameworkContractItem, int? id_metricUnitFrameworkContractItem,
                                                        decimal? valueFrameworkContractExtension, decimal? amoutFrameworkContractExtension, string code_typeContractFramework)
        {
            ViewData["code_typeContractFramework"] = code_typeContractFramework;

            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);

            var frameworkContractItem = frameworkContract.FrameworkContractItem.ToList();
            frameworkContract.FrameworkContractItem = frameworkContractItem ?? new List<FrameworkContractItem>();

            //var model = db.BusinessOportunityNote;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = frameworkContract.FrameworkContractItem.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        if (valueFrameworkContractExtension == null && amoutFrameworkContractExtension == null)
                        {
                            modelItem.id_item = id_itemFrameworkContractItem.Value;
                            modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == id_itemFrameworkContractItem);
                            modelItem.startDate = startDateFrameworkContractItem.Value;
                            modelItem.endDate = endDateFrameworkContractItem.Value;
                            modelItem.value = valueFrameworkContractItem;
                            modelItem.amout = amoutFrameworkContractItem;

                            modelItem.id_metricUnit = id_metricUnitFrameworkContractItem;
                            modelItem.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitFrameworkContractItem);
                        }
                        else
                        {
                            if (modelItem.FrameworkContractExtension == null)
                            {
                                modelItem.FrameworkContractExtension = new List<FrameworkContractExtension>();
                            }

                            var frameworkContractExtensionAux = modelItem.FrameworkContractExtension.FirstOrDefault();

                            if (frameworkContractExtensionAux == null && modelItem.FrameworkContractExtension.Count == 0)
                            {
                                frameworkContractExtensionAux = new FrameworkContractExtension();
                                frameworkContractExtensionAux.id_frameworkContract = id_frameworkContract;
                                frameworkContractExtensionAux.FrameworkContract = frameworkContract;
                                frameworkContractExtensionAux.value = valueFrameworkContractExtension;
                                frameworkContractExtensionAux.amout = amoutFrameworkContractExtension;

                                frameworkContractExtensionAux.isSave = false;
                                frameworkContract.FrameworkContractExtension.Add(frameworkContractExtensionAux);
                                modelItem.FrameworkContractExtension.Add(frameworkContractExtensionAux);
                            }
                            else
                            {
                                //frameworkContractExtensionAux.id_frameworkContract = id_frameworkContract;
                                //frameworkContractExtensionAux.FrameworkContract = frameworkContract;
                                frameworkContractExtensionAux.isSave = false;
                                frameworkContractExtensionAux.value = valueFrameworkContractExtension;
                                frameworkContractExtensionAux.amout = amoutFrameworkContractExtension;
                            }

                        }


                        //item.phaseName = phaseName;
                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["person"] = person;
                    TempData["frameworkContract"] = frameworkContract;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            var model = frameworkContract.FrameworkContractItem;

            return PartialView("_FormEditFrameworkContractItem", model.OrderBy(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractItemDelete(int id_person, int id_frameworkContract, System.Int32 id, string code_typeContractFramework)
        {
            ViewData["code_typeContractFramework"] = code_typeContractFramework;

            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);

            var frameworkContractItem = frameworkContract.FrameworkContractItem.ToList();
            frameworkContract.FrameworkContractItem = frameworkContractItem ?? new List<FrameworkContractItem>();

            try
            {
                var item = frameworkContract.FrameworkContractItem.FirstOrDefault(it => it.id == id);
                if (item != null /*Verificar que el contrato no este utilizado en ningun Orden de Pedido o de Venta o ningún Requrimiento de Compra */)
                {
                    for (int j = item.FrameworkContractDeliveryPlan.Count - 1; j >= 0; j--)
                    {
                        var detailFrameworkContractDeliveryPlan = item.FrameworkContractDeliveryPlan.ElementAt(j);

                        item.FrameworkContractDeliveryPlan.Remove(detailFrameworkContractDeliveryPlan);
                    }

                    frameworkContract.FrameworkContractItem.Remove(item);

                }

                TempData["person"] = person;
                TempData["frameworkContract"] = frameworkContract;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            var model = frameworkContract.FrameworkContractItem;

            return PartialView("_FormEditFrameworkContractItem", model.OrderBy(od => od.id).ToList());
        }

        #endregion

        #region PERSONS FRAMEWORK CONTRACT DELIVERY PLAN

        [ValidateInput(false)]
        public ActionResult PersonFrameworkContractDeliveryPlan(int id_person, int id_frameworkContract, int id_frameworkContractItem, string code_documentState)
        {
            ViewData["code_documentState"] = code_documentState;

            FrameworkContractItem frameworkContractItem = GetFrameworkContractItemCurrent(id_person, id_frameworkContract, id_frameworkContractItem);

            var frameworkContractDeliveryPlan = frameworkContractItem.FrameworkContractDeliveryPlan.ToList();
            var model = frameworkContractDeliveryPlan ?? new List<FrameworkContractDeliveryPlan>();

            return PartialView("_FormEditFrameworkContractDeliveryPlan", model.OrderBy(od => od.deliveryPlanDate).ThenBy(tb => tb.id).ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractDeliveryPlanAddNew(int id_person, int id_frameworkContract, int id_frameworkContractItem, FrameworkContractDeliveryPlan item)
        {
            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);
            FrameworkContractItem frameworkContractItem = GetFrameworkContractItemCurrent(id_person, id_frameworkContract, id_frameworkContractItem);

            var frameworkContractDeliveryPlan = frameworkContractItem.FrameworkContractDeliveryPlan.ToList();
            frameworkContractItem.FrameworkContractDeliveryPlan = frameworkContractDeliveryPlan ?? new List<FrameworkContractDeliveryPlan>();

            if (ModelState.IsValid)
            {
                try
                {
                    item.id = frameworkContractItem.FrameworkContractDeliveryPlan.Count() > 0 ? frameworkContractItem.FrameworkContractDeliveryPlan.Max(pld => pld.id) + 1 : 1;
                    //var phaseName = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == item.id_businessOportunityDocumentTypePhase)?.Phase.name ?? "";
                    //item.phaseName = phaseName;
                    item.id_frameworkContractItem = id_frameworkContractItem;
                    item.FrameworkContractItem = frameworkContractItem;

                    frameworkContractItem.FrameworkContractDeliveryPlan.Add(item);

                    TempData["person"] = person;
                    TempData["frameworkContract"] = frameworkContract;
                    TempData["frameworkContractItem"] = frameworkContractItem;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");
            var model = frameworkContractItem.FrameworkContractDeliveryPlan;

            return PartialView("_FormEditFrameworkContractDeliveryPlan", model.OrderBy(od => od.deliveryPlanDate).ThenBy(tb => tb.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractDeliveryPlanUpdate(int id_person, int id_frameworkContract, int id_frameworkContractItem, FrameworkContractDeliveryPlan item)
        {
            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);
            FrameworkContractItem frameworkContractItem = GetFrameworkContractItemCurrent(id_person, id_frameworkContract, id_frameworkContractItem);

            var frameworkContractDeliveryPlan = frameworkContractItem.FrameworkContractDeliveryPlan.ToList();
            frameworkContractItem.FrameworkContractDeliveryPlan = frameworkContractDeliveryPlan ?? new List<FrameworkContractDeliveryPlan>();


            //var model = db.BusinessOportunityNote;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = frameworkContractItem.FrameworkContractDeliveryPlan.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.deliveryPlanDate = item.deliveryPlanDate;
                        modelItem.amout = item.amout;

                        //item.phaseName = phaseName;
                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["person"] = person;
                    TempData["frameworkContract"] = frameworkContract;
                    TempData["frameworkContractItem"] = frameworkContractItem;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");
            var model = frameworkContractItem.FrameworkContractDeliveryPlan;

            return PartialView("_FormEditFrameworkContractDeliveryPlan", model.OrderBy(od => od.deliveryPlanDate).ThenBy(tb => tb.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FrameworkContractDeliveryPlanDelete(int id_person, int id_frameworkContract, int id_frameworkContractItem, System.Int32 id)
        {
            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);
            FrameworkContractItem frameworkContractItem = GetFrameworkContractItemCurrent(id_person, id_frameworkContract, id_frameworkContractItem);

            var frameworkContractDeliveryPlan = frameworkContractItem.FrameworkContractDeliveryPlan.ToList();
            frameworkContractItem.FrameworkContractDeliveryPlan = frameworkContractDeliveryPlan ?? new List<FrameworkContractDeliveryPlan>();

            try
            {
                var item = frameworkContractItem.FrameworkContractDeliveryPlan.FirstOrDefault(it => it.id == id);
                if (item != null /*Verificar que el contrato no este utilizado en ningun Orden de Pedido o de Venta o ningún Requrimiento de Compra */)
                {
                    frameworkContractItem.FrameworkContractDeliveryPlan.Remove(item);

                }

                TempData["person"] = person;
                TempData["frameworkContract"] = frameworkContract;
                TempData["frameworkContractItem"] = frameworkContractItem;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");
            var model = frameworkContractItem.FrameworkContractDeliveryPlan;

            return PartialView("_FormEditFrameworkContractDeliveryPlan", model.OrderBy(od => od.deliveryPlanDate).ThenBy(tb => tb.id).ToList());
        }

        #endregion

        #region PERSONS FRAMEWORK CONTRACT DETAILS

        [ValidateInput(false)]
        public ActionResult PersonFrameworkContractDeliveryPlansViewPartial(int id_person, int id_frameworkContract, int id_frameworkContractItem)
        {
            ViewData["id_frameworkContractItem"] = id_frameworkContractItem;
            ViewData["id_frameworkContract"] = id_frameworkContract;

            Person person = (TempData["person"] as Person);
            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();

            var frameworkContractDeliveryPlan = person.FrameworkContract.FirstOrDefault(p => p.id == id_frameworkContract)
                                              .FrameworkContractItem.FirstOrDefault(p => p.id == id_frameworkContractItem).FrameworkContractDeliveryPlan.ToList();
            var model = frameworkContractDeliveryPlan ?? new List<FrameworkContractDeliveryPlan>();

            //SetViewData();

            TempData["person"] = person;
            TempData.Keep("person");

            return PartialView("_FrameworkContractDeliveryPlanViewPartial", model.OrderBy(od => od.deliveryPlanDate).ThenBy(tb => tb.id).ToList());

        }

        [ValidateInput(false)]
        public ActionResult PersonFrameworkContractItemsViewPartial(int id_person, int id_frameworkContract, string code_typeContractFramework)
        {
            ViewData["id_frameworkContract"] = id_frameworkContract;
            ViewData["code_typeContractFramework"] = code_typeContractFramework;

            Person person = (TempData["person"] as Person);
            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();

            var frameworkContractItem = person.FrameworkContract.FirstOrDefault(p => p.id == id_frameworkContract)
                                              .FrameworkContractItem.ToList();
            var model = frameworkContractItem ?? new List<FrameworkContractItem>();

            //SetViewData();

            TempData["person"] = person;
            TempData.Keep("person");

            return PartialView("_FrameworkContractItemViewPartial", model.OrderBy(od => od.id).ToList());

        }

        #endregion

        #region {RA} - Customer::CustomerAddress
        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerAddress(int id_person)
        {

            Person _person = getPersonCustomer(id_person);

            var model = _person.Customer?.CustomerAddress.Where(r => r.isActive).ToList() ?? new List<CustomerAddress>();

            TempData["person"] = TempData["person"] ?? _person;
            TempData.Keep("person");

            return PartialView("_FormEditCustomerAddress", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerAddressAddNew(int id_person, CustomerAddress customerAddress)
        {

            Person _person = getPersonCustomer(id_person);

            if (ModelState.IsValid)
            {
                try
                {
                    customerAddress.id = _person.Customer.CustomerAddress.Count() > 0 ? _person.Customer.CustomerAddress.Max(pld => pld.id) + 1 : 1;
                    customerAddress.name_addressType = getSubstr(DataProviderAddressType.AddressTypeById(customerAddress.id_addressType).name, 99);
                    customerAddress.dateCreate = DateTime.Now;
                    customerAddress.dateUpdate = DateTime.Now;
                    customerAddress.id_userCreate = ActiveUser.id;
                    customerAddress.id_userUpdate = ActiveUser.id;

                    _person.Customer.CustomerAddress.Add(customerAddress);
                    TempData["person"] = _person;



                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    TempData.Keep("person");
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";



            var model = _person.Customer.CustomerAddress.Where(r => r.isActive).ToList();
            if (model == null)
            {
                model = new List<CustomerAddress>();
            }


            return PartialView("_FormEditCustomerAddress", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerAddresslUpdate(CustomerAddress customerAddress)
        {

            Person _person = getPersonCustomer(customerAddress.id_customer);

            List<CustomerAddress> model = CustomerAddressUpdateDelete(_person, customerAddress.id, false);
            TempData.Keep("person");

            return PartialView("_FormEditCustomerAddress", model);

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerAddressDelete(int id_person, int id)
        {

            Person _person = getPersonCustomer(id_person);

            List<CustomerAddress> model = CustomerAddressUpdateDelete(_person, id, true);

            return PartialView("_FormEditCustomerAddress", model);
        }


        #endregion

        #region {RA} Customer::CustomerPriceList

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerPriceList(int id_person)
        {

            Person _person = getPersonCustomer(id_person);

            var model = _person.Customer.CustomerPriceList.Where(r => r.isActive).ToList() ?? new List<CustomerPriceList>();

            TempData["person"] = TempData["person"] ?? _person;
            TempData.Keep("person");

            return PartialView("_FormEditCustomePriceList", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerPriceListAddNew(int id_person, CustomerPriceList customerPriceList)
        {

            Person _person = getPersonCustomer(id_person);

            if (ModelState.IsValid)
            {
                try
                {
                    customerPriceList.id = _person.Customer.CustomerPriceList.Count() > 0 ? _person.Customer.CustomerPriceList.Max(pld => pld.id) + 1 : 1;
                    customerPriceList.name_priceList = getSubstr(DataProviderPriceList.PriceListById(customerPriceList.id_priceList).name, 99);
                    customerPriceList.dateCreate = DateTime.Now;
                    customerPriceList.dateUpdate = DateTime.Now;
                    customerPriceList.id_userCreate = ActiveUser.id;
                    customerPriceList.id_userUpdate = ActiveUser.id;

                    _person.Customer.CustomerPriceList.Add(customerPriceList);
                    TempData["person"] = _person;
                    TempData.Keep("person");


                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";



            var model = _person.Customer.CustomerPriceList.Where(r => r.isActive).ToList();
            if (model == null)
            {
                model = new List<CustomerPriceList>();
            }


            return PartialView("_FormEditCustomePriceList", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerPriceListUpdate(CustomerPriceList customerPriceList)
        {

            Person _person = getPersonCustomer(customerPriceList.id_customer);

            List<CustomerPriceList> model = CustomerPriceListDelete(_person, customerPriceList.id, false);
            TempData.Keep("person");

            return PartialView("_FormEditCustomePriceList", model);

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerPriceListDelete(int id_person, int id)
        {

            Person _person = getPersonCustomer(id_person);
            List<CustomerPriceList> model = CustomerPriceListDelete(_person, id, true);
            return PartialView("_FormEditCustomePriceList", model);

        }


        #endregion

        #region {RA} - Auxiliar Method Customer
        private Person getPersonCustomer(int? id_person)
        {

            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();
            person.Customer = person.Customer ?? new Customer();

            return person;



        }

        private List<CustomerAddress> CustomerAddressUpdateDelete(Person person, int id_CustomerAddress, Boolean isDelete)
        {

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelCustomerAddress = person.Customer.CustomerAddress.FirstOrDefault(i => i.id == id_CustomerAddress);
                    if (modelCustomerAddress != null)
                    {
                        modelCustomerAddress.name_addressType = getSubstr(DataProviderAddressType.AddressTypeById(modelCustomerAddress.id_addressType).name, 99);
                        modelCustomerAddress.id_userUpdate = ActiveUser.id;
                        modelCustomerAddress.dateUpdate = DateTime.Now;
                        if (isDelete) modelCustomerAddress.isActive = false;
                        this.UpdateModel(modelCustomerAddress);
                    }

                    TempData["person"] = person;
                    TempData.Keep("person");

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";


            TempData.Keep("person");

            List<CustomerAddress> model = person.Customer.CustomerAddress?.Where(x => x.isActive == true).ToList() ?? new List<CustomerAddress>();
            return model;
        }

        private List<CustomerPriceList> CustomerPriceListDelete(Person person, int id_CustomerPriceList, Boolean isDelete)
        {

            if (ModelState.IsValid && person != null)
            {
                try
                {
                    var modelmodelCustomerPriceList = person.Customer.CustomerPriceList.FirstOrDefault(i => i.id == id_CustomerPriceList);
                    if (modelmodelCustomerPriceList != null)
                    {

                        modelmodelCustomerPriceList.name_priceList = getSubstr(DataProviderPriceList.PriceListById(modelmodelCustomerPriceList.id_priceList).name, 99);
                        modelmodelCustomerPriceList.id_userUpdate = ActiveUser.id;
                        modelmodelCustomerPriceList.dateUpdate = DateTime.Now;
                        if (isDelete) modelmodelCustomerPriceList.isActive = false;
                        this.UpdateModel(modelmodelCustomerPriceList);
                    }

                    TempData["person"] = person;
                    TempData.Keep("person");

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";


            TempData.Keep("person");

            List<CustomerPriceList> model = person.Customer.CustomerPriceList?.Where(x => x.isActive == true).ToList() ?? new List<CustomerPriceList>();
            return model;
        }

        private static String getSubstr(String Text, int maxNumText)
        {

            int maxLength = (Text.Length >= maxNumText) ? maxNumText : Text.Length;

            return Text.Substring(0, maxLength);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadCustomerType(int? id_customerType, int? id_clientCategory)
        {
            var aCustomerTypes = DataProviderCustomerType.CustomerTypebyCompanyClientCategoryAndCurrent(ActiveCompany.id, id_clientCategory, id_customerType);

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_customerType";
            p.ValueField = "id";
            p.TextFormatString = "{1}";
            p.ValueType = typeof(int);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            p.Columns.Add("code", "Cod.", 50);
            p.Columns.Add("name", "Nombre", 100);
            //settings.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "OnSelectedIndexChanged_CustomerType";
            p.CallbackRouteValues = new { Controller = "Person", Action = "LoadCustomerType" };
            p.ClientSideEvents.BeginCallback = "CustomerType_BeginCallback";
            p.ClientSideEvents.EndCallback = "CustomerType_EndCallback";
            p.ClientSideEvents.Validation = "OnUpdateImagenWhenRequiredField";
            //p.CustomJSProperties = (s, e) =>
            //{

            //    e.Properties["cpTabContainer"] = "tabCustomer";
            //    e.Properties["cpMessageError"] = "Campo Obligatorio.";
            //    // e.Properties["cpInitialCondition"] = "(  )-   -";
            //    e.Properties["cpIsRequired"] = "true";
            //    e.Properties["cpMessageErrorFormart"] = "Debe seleccionar Tipo de Cliente ";
            //    e.Properties["cpTabControl"] = "tabControl";
            //};

            p.BindList(aCustomerTypes);

            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadBusinessLine(int? id_businessLine, int? id_customerType, int? id_clientCategory)
        {
            var aBusinessLines = DataProviderBusinessLine.BusinessLinebyCompanyClientCategoryCustomerTypeAndCurrent(ActiveCompany.id, id_clientCategory, id_customerType, id_businessLine);

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            //p.Name = "id_businessLine";
            p.ClientInstanceName = "id_businessLine";
            p.ValueField = "id";
            p.TextFormatString = "{1}";
            p.ValueType = typeof(int);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            p.Columns.Add("code", "Cod.", 50);
            p.Columns.Add("description", "Descripción", 100);
            //settings.ShowModelErrors = true;
            p.CallbackRouteValues = new { Controller = "Person", Action = "LoadBusinessLine" };
            p.ClientSideEvents.BeginCallback = "BusinessLine_BeginCallback";
            p.ClientSideEvents.Validation = "OnUpdateImagenWhenRequiredField";
            //p.CustomJSProperties = (s, e) =>
            //{

            //    e.Properties["cpTabContainer"] = "tabCustomer";
            //    e.Properties["cpMessageError"] = "Campo Obligatorio.";
            //    // e.Properties["cpInitialCondition"] = "(  )-   -";
            //    e.Properties["cpIsRequired"] = "true";
            //    e.Properties["cpMessageErrorFormart"] = "Debe seleccionar el Giro de Negocio ";
            //    e.Properties["cpTabControl"] = "tabControl";
            //};

            p.BindList(aBusinessLines);

            return GridViewExtension.GetComboBoxCallbackResult(p);

        }
        #endregion

        #region AXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public string IdentificationTypeCode(int id_identificationType)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            IdentificationType identificationType = db.IdentificationType.FirstOrDefault(i => i.id == id_identificationType);
            return identificationType?.code ?? "";
        }

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            return BinaryImageEditExtension.GetCallbackResult();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult VerifyIdentificationCode(string id_code, int id_person)
        {
            Person personAux = (TempData["person"] as Person);
            if (personAux != null)
            {
                TempData["person"] = personAux;
                TempData.Keep("person");
            }
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
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            var identificationTypeAux = db.IdentificationType.FirstOrDefault(fod => fod.id == id_identificationType);

            var result = new
            {
                codSRI_IdentificationType = identificationTypeAux?.codeSRI ?? ""

            };

            //TempData.Keep("productionLot");
            TempData.Keep("person");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProviderTypeWhatProviderIs(int? id_providerType)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
                isTransportist = isTransportist ? "SI" : "NO"
            };
            TempData.Keep("person");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitComboOrigin(int? id_origin, int? id_country, int? id_city, int? id_stateOfContry)
        {
            Person person = (TempData["person"] as Person);

            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
        public JsonResult ItsRepeatedCodeProduction(string codeProductionUnitProvider, bool isNew, int? id)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            id = id ?? 0;

            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            person.Provider.ProductionUnitProvider = person.Provider.ProductionUnitProvider ?? new List<ProductionUnitProvider>();
            List<ProductionUnitProvider> tmp = person.Provider.ProductionUnitProvider.ToList();
            List<ProductionUnitProvider> tmp2 = person.Provider.ProductionUnitProvider.ToList();
            if (person.Provider.ProductionUnitProvider != null && person.Provider.ProductionUnitProvider.Count > 0)
            {
                if (isNew == true)
                {
                    var productionUnitProvider = person.Provider.ProductionUnitProvider.FirstOrDefault(fod => fod.code == codeProductionUnitProvider);
                    if (productionUnitProvider != null)
                    {
                        result = new
                        {
                            itsRepeated = 1,
                            Error = "El código está repetido."
                        };
                    }
                }
                else
                {
                    tmp2 = tmp.Where(w => w.code != codeProductionUnitProvider).ToList();
                    var productionUnitProvider2 = tmp2.FirstOrDefault(fod => fod.code == codeProductionUnitProvider);
                    if (productionUnitProvider2 != null)
                    {
                        result = new
                        {
                            itsRepeated = 1,
                            Error = "El código está repetido."
                        };
                    }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }

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
        public JsonResult InitProviderItemItemCombo(int? id_item)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }

            person = person ?? new Person();


            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId) || w.id == id_item).Select(s => new
            {
                id = s.id,
                masterCode = s.masterCode,
                barCode = s.barCode,
                name = s.name
            }).ToList();

            var result = new
            {
                items = items

            };

            TempData.Keep("person");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItem(int? id_itemNew)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }

            person = person ?? new Person();
            person.Provider = person.Provider ?? new Provider();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };


            var providerItemAux = person.Provider.ProviderItem.FirstOrDefault(fod => fod.id_item == id_itemNew);
            if (providerItemAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Ítem: " + itemAux.name + ",  en los detalles."

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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }

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

            var accountingAssistantDetailTypes = db.AccountingAssistantDetailType.Where(w => (w.isActive && db.AccountDetailAssistantType.FirstOrDefault(fod => fod.id_account == id_account && fod.id_assistantType == w.id_assistantType) != null) || w.id == id_accountingAssistantDetailType).Select(s => new
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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }

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
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
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

        #region FrameworkContract

        private Person GetPersonCurrent(int id_person)
        {
            Person person = (TempData["person"] as Person);
            person = person ?? db.Person.FirstOrDefault(i => i.id == id_person);
            person = person ?? new Person();

            TempData["person"] = person;
            TempData.Keep("person");
            return person;

        }

        private FrameworkContract GetFrameworkContractCurrent(int id_person, int id_frameworkContract)
        {

            Person person = GetPersonCurrent(id_person);

            FrameworkContract frameworkContract = (TempData["frameworkContract"] as FrameworkContract);
            frameworkContract = frameworkContract ?? person.FrameworkContract.FirstOrDefault(p => p.id == id_frameworkContract);
            frameworkContract = frameworkContract ?? new FrameworkContract();

            TempData["frameworkContract"] = frameworkContract;
            TempData.Keep("frameworkContract");
            return frameworkContract;

        }

        private FrameworkContractItem GetFrameworkContractItemCurrent(int id_person, int id_frameworkContract, int id_frameworkContractItem)
        {

            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);

            FrameworkContractItem frameworkContractItem = (TempData["frameworkContractItem"] as FrameworkContractItem);
            frameworkContractItem = frameworkContractItem ?? frameworkContract.FrameworkContractItem.FirstOrDefault(p => p.id == id_frameworkContractItem);
            frameworkContractItem = frameworkContractItem ?? new FrameworkContractItem();

            TempData["frameworkContractItem"] = frameworkContractItem;
            TempData.Keep("frameworkContractItem");
            return frameworkContractItem;

        }

        [HttpPost]
        public JsonResult PersonFrameworkContractCompanyDetailData(int? id_company, int? id_person, int? id_frameworkContract)
        {

            //Person person = (TempData["person"] as Person);

            //person = person ?? new Person();
            //if (id_frameworkContract !=  null && id_frameworkContract != 0)
            //{
            ClearFrameworkContractDetail(id_person, id_frameworkContract);

            //}

            var typeContractFrameworks = db.TypeContractFramework.Where(w => (w.id_company == id_company && w.isActive)).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();


            var result = new
            {
                typeContractFrameworks = typeContractFrameworks
            };


            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");
            //TempData.Keep("frameworkContractDeliveryPlan");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PersonFrameworkContractTypeContractFrameworkDetailData(int? id_typeContractFramework, int? id_person, int? id_frameworkContract)
        {

            //Person person = (TempData["person"] as Person);

            //person = person ?? new Person();

            //if (id_frameworkContract != null && id_frameworkContract != 0)
            //{
            ClearFrameworkContractDetail(id_person, id_frameworkContract);

            //}

            var typeContractFramework = db.TypeContractFramework.FirstOrDefault(w => w.id == id_typeContractFramework);


            var result = new
            {
                code_typeContractFramework = typeContractFramework?.code ?? ""
            };


            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PersonFrameworkContractRolDetailData(int? id_person, int? id_frameworkContract)
        {


            ClearFrameworkContractDetail(id_person, id_frameworkContract);



            var result = new
            {
                Message = "OK"
            };


            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void ClearFrameworkContractDetail(int? id_person, int? id_contractFramework)
        {

            Person person = GetPersonCurrent(id_person ?? 0);

            var frameworkContract = person.FrameworkContract.FirstOrDefault(fod => fod.id == id_contractFramework);

            frameworkContract = frameworkContract ?? GetFrameworkContractCurrent(id_person ?? 0, id_contractFramework ?? 0);

            for (int i = frameworkContract.FrameworkContractItem.Count - 1; i >= 0; i--)
            {
                var detail = frameworkContract.FrameworkContractItem.ElementAt(i);

                for (int j = detail.FrameworkContractDeliveryPlan.Count - 1; j >= 0; j--)
                {
                    var detailFrameworkContractDeliveryPlan = detail.FrameworkContractDeliveryPlan.ElementAt(j);

                    detail.FrameworkContractDeliveryPlan.Remove(detailFrameworkContractDeliveryPlan);
                }

                for (int j = detail.FrameworkContractExtension.Count - 1; j >= 0; j--)
                {
                    var detailFrameworkContractExtension = detail.FrameworkContractExtension.ElementAt(j);

                    detail.FrameworkContractExtension.Remove(detailFrameworkContractExtension);
                }

                //detail.ProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail .Remove(detail);
                frameworkContract.FrameworkContractItem.Remove(detail);

            }

            for (int i = frameworkContract.FrameworkContractExtension.Count - 1; i >= 0; i--)
            {
                var detailFrameworkContractExtension = frameworkContract.FrameworkContractExtension.ElementAt(i);

                frameworkContract.FrameworkContractExtension.Remove(detailFrameworkContractExtension);
            }

            TempData.Keep("person");
            //TempData.Keep("frameworkContract");
            //TempData.Keep("frameworkContractItem");

        }


        [HttpPost]
        public JsonResult PersonFrameworkContractItemDetailData(int? id_company, int? id_rol, int? id_item, int? id_metricUnit)
        {

            //Person person = (TempData["person"] as Person);

            //person = person ?? new Person();

            var rolAux = db.Rol.FirstOrDefault(fod => fod.id == id_rol);
            var providerCustomer = rolAux?.name == "Proveedor" ? "Provider"
                                                               : (rolAux?.name == "Cliente Local" ||
                                                                  rolAux?.name == "Cliente Exterior" ? "Customer" : "");
            var items = db.Item.Where(w => (w.id_company == id_company && w.isActive &&
                                           ((providerCustomer == "Provider" && w.isPurchased) ||
                                            (providerCustomer == "Customer" && w.isSold))) || (w.id == id_item)).Select(s => new
                                            {
                                                id = s.id,
                                                masterCode = s.masterCode,
                                                name = s.name
                                            }).ToList();

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

            var metricUnits = (((providerCustomer == "Provider") ? item?.ItemPurchaseInformation?.MetricUnit.MetricType.MetricUnit.Where(w => w.isActive || w.id == id_metricUnit).ToList()
                                                                : ((providerCustomer == "Customer") ? (item?.Presentation?.MetricUnit.MetricType.MetricUnit.Where(w => w.isActive || w.id == id_metricUnit).ToList() ??
                                                                                                       item?.ItemSaleInformation?.MetricUnit.MetricType.MetricUnit.Where(w => w.isActive || w.id == id_metricUnit).ToList())
                                                                                                    : null)) ?? db.MetricUnit.Where(w => w.id == id_metricUnit).ToList());
            var id_metricUnitAux = (((providerCustomer == "Provider") ? item?.ItemPurchaseInformation?.id_metricUnitPurchase
                                                                : ((providerCustomer == "Customer") ? (item?.Presentation?.id_metricUnit ??
                                                                                                       item?.ItemSaleInformation?.id_metricUnitSale)
                                                                                                    : null)) ?? id_metricUnit);
            var result = new
            {
                items = items,
                metricUnits = metricUnits?.Select(s => new { id = s.id, code = s.code, name = s.name }),
                id_metricUnit = id_metricUnitAux
            };


            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PersonFrameworkContractItemChangedDetailData(int? id_item, int? id_rol)
        {


            var rolAux = db.Rol.FirstOrDefault(fod => fod.id == id_rol);
            var providerCustomer = rolAux?.name == "Proveedor" ? "Provider"
                                                               : (rolAux?.name == "Cliente Local" ||
                                                                  rolAux?.name == "Cliente Exterior" ? "Customer" : "");

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

            var metricUnits = (((providerCustomer == "Provider") ? item?.ItemPurchaseInformation?.MetricUnit.MetricType.MetricUnit.Where(w => w.isActive).ToList()
                                                                : ((providerCustomer == "Customer") ? (item?.Presentation?.MetricUnit.MetricType.MetricUnit.Where(w => w.isActive).ToList() ??
                                                                                                       item?.ItemSaleInformation?.MetricUnit.MetricType.MetricUnit.Where(w => w.isActive).ToList())
                                                                                                    : null)) ?? db.MetricUnit.Where(w => w.id == 0).ToList());
            var id_metricUnitAux = ((providerCustomer == "Provider") ? item?.ItemPurchaseInformation?.id_metricUnitPurchase
                                                                : ((providerCustomer == "Customer") ? (item?.Presentation?.id_metricUnit ??
                                                                                                       item?.ItemSaleInformation?.id_metricUnitSale)
                                                                                                    : null));
            var result = new
            {
                metricUnits = metricUnits?.Select(s => new { id = s.id, code = s.code, name = s.name }),
                id_metricUnit = id_metricUnitAux
            };


            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PersonFrameworkContractDeliveryPlanValidateAmount(int id_person, int id_frameworkContract, int id_frameworkContractItem,
                                                                            string amout, string amoutTotal, int? id_frameworkContractDeliveryPlan)
        {
            var result = new
            {
                itsValid = 1,
                Error = ""

            };

            decimal _amout = Convert.ToDecimal(amout);
            decimal _amoutTotal = Convert.ToDecimal(amoutTotal);

            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);
            FrameworkContractItem frameworkContractItem = GetFrameworkContractItemCurrent(id_person, id_frameworkContract, id_frameworkContractItem);

            var frameworkContractDeliveryPlan = frameworkContractItem.FrameworkContractDeliveryPlan.ToList();
            frameworkContractItem.FrameworkContractDeliveryPlan = frameworkContractDeliveryPlan ?? new List<FrameworkContractDeliveryPlan>();

            var frameworkContractDeliveryPlanWithOutId = frameworkContractItem.FrameworkContractDeliveryPlan.Where(w => w.id != id_frameworkContractDeliveryPlan).ToList();
            var amoutTotalWithOutId = frameworkContractDeliveryPlanWithOutId != null && frameworkContractDeliveryPlanWithOutId.Count > 0 ? frameworkContractDeliveryPlanWithOutId.Sum(s => s.amout) : 0;

            if (_amoutTotal <= 0)
            {
                result = new
                {
                    itsValid = 0,
                    Error = "No puede registrarse cantidad en Plan de Entrega debido a tener una cantidad total en el Item no válida"
                };
            }
            else
            {
                var restValue = _amoutTotal - amoutTotalWithOutId;
                if (restValue < 0)
                {
                    var restValueAux = amoutTotalWithOutId - _amoutTotal;
                    result = new
                    {
                        itsValid = 0,
                        Error = "Debe reajustar la cantidad del item o las cantidades planificadas pues existe un excedente de planificación en: " + restValueAux.ToString("N2")
                    };
                }
                else
                {
                    if (restValue == 0)
                    {
                        result = new
                        {
                            itsValid = 0,
                            Error = "Las cantidades planificadas ya cumplen con la cantidad del item: " + _amoutTotal.ToString("N2") + " no puede planificar más entrega"
                        };
                    }
                    else
                    {
                        if (restValue < _amout)
                        {
                            result = new
                            {
                                itsValid = 0,
                                Error = "La cantidad en Plan de Entrega no puede exceder a: " + restValue.ToString("N2")
                            };
                        }
                    }
                }

            }

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PersonFrameworkContractItemValidateAmount(int id_person, int id_frameworkContract, int id_frameworkContractItem,
                                                                    string amoutEdit, string amoutTotal, int? id_frameworkContractDeliveryPlan)
        {
            var result = new
            {
                itsValid = 1,
                Error = ""

            };

            decimal _amoutEdit = Convert.ToDecimal(amoutEdit);
            decimal _amoutTotal = Convert.ToDecimal(amoutTotal);

            Person person = GetPersonCurrent(id_person);
            FrameworkContract frameworkContract = GetFrameworkContractCurrent(id_person, id_frameworkContract);
            FrameworkContractItem frameworkContractItem = GetFrameworkContractItemCurrent(id_person, id_frameworkContract, id_frameworkContractItem);

            var frameworkContractDeliveryPlan = frameworkContractItem.FrameworkContractDeliveryPlan.ToList();
            frameworkContractItem.FrameworkContractDeliveryPlan = frameworkContractDeliveryPlan ?? new List<FrameworkContractDeliveryPlan>();

            var frameworkContractDeliveryPlanWithOutId = frameworkContractItem.FrameworkContractDeliveryPlan.Where(w => w.id != id_frameworkContractDeliveryPlan).ToList();
            var amoutTotalWithOutId = frameworkContractDeliveryPlanWithOutId != null && frameworkContractDeliveryPlanWithOutId.Count > 0 ? frameworkContractDeliveryPlanWithOutId.Sum(s => s.amout) : 0;

            var resultValueCurrent = _amoutEdit + amoutTotalWithOutId;
            if (_amoutTotal < resultValueCurrent)
            {
                var restValue = resultValueCurrent - _amoutTotal;
                result = new
                {
                    itsValid = 0,
                    Error = "La planificación de entrega excede la cantidad total en el Item en:" + restValue.ToString("N2")
                };
            }
            else
            {
                if (_amoutTotal > resultValueCurrent)
                {
                    var restValue = _amoutTotal - resultValueCurrent;
                    result = new
                    {
                        itsValid = 0,
                        Error = "Falta por planificación de entrega la cantidad de:" + restValue.ToString("N2")
                    };
                }
            }

            TempData.Keep("person");
            TempData.Keep("frameworkContract");
            TempData.Keep("frameworkContractItem");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion

        #region "MIGRACION PERSONA"
        protected async Task<JsonResult> AsyncAwait_GetSomeDataAsync(int id)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            var httpClient = new HttpClient();

            string resultado = string.Empty;
            try
            {
                var data = JsonConvert.SerializeObject(null);

                var actualizar = "/AddModifyPerson?id=" + id;

                var uri = ConfigurationManager.AppSettings["URIProduccion"] + actualizar;//"http://localhost:49067/Api/Provider + actualizar";
                var response = await httpClient.PostAsync(uri, new StringContent(data));

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    return null;
                }

                //var result = await httpClient.GetAsync("http://localhost:27631/Api/PersonProvider", HttpCompletionOption.ResponseContentRead);

                string content = await response.Content.ReadAsStringAsync();
                resultado = JsonConvert.DeserializeObject<string>(content);

            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());
            }
            int indiceRespuestaProveedor = 0;
            int indiceRepuestaCliente = 0;
            string respuestaProveedorTmp = string.Empty;
            string respuestaClienteTmp = string.Empty;
            if (resultado != string.Empty)
            {
                indiceRespuestaProveedor = resultado.IndexOf("respuestaProveedor:");
                indiceRepuestaCliente = resultado.IndexOf("respuestaCliente:");
                if (indiceRespuestaProveedor > -1)
                {
                    indiceRepuestaCliente = indiceRepuestaCliente > -1 ? indiceRepuestaCliente : 0;
                    if (indiceRepuestaCliente == 0)
                    {
                        respuestaProveedorTmp = resultado.Substring(19, (resultado.Length - 19));
                    }
                    else
                    {
                        respuestaProveedorTmp = resultado.Substring(19, (indiceRepuestaCliente - 19));
                    }
                }
                if (indiceRepuestaCliente > -1)
                {
                    respuestaClienteTmp = resultado.Substring((indiceRepuestaCliente + 17), (resultado.Length - indiceRepuestaCliente - 17));
                }
            }
            var result = new
            {
                respuestaProveedor = respuestaProveedorTmp,
                respuestaCliente = respuestaClienteTmp
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> MigrarIndividual(int id)
        {

            var resultado = await AsyncAwait_GetSomeDataAsync(id);
            var result = new
            {
                Data = JsonConvert.SerializeObject(resultado)
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"PERSON ROLS"
        public ActionResult PersonRolsPartial(string[] rols, int id_person)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            person = person ?? db.Person.FirstOrDefault(fod => fod.id == id_person);
            person = person ?? new Person();

            for (int i = person.Rol.Count - 1; i >= 0; i--)
            {
                Rol rol = person.Rol.ElementAt(i);
                //if (!id_roles.Contains(rol.id))
                person.Rol.Remove(rol);
            }

            foreach (var id in rols)
            {
                if (id == "" || id == null) continue;
                var idInt = int.Parse(id);
                var _rol = db.Rol.FirstOrDefault(r => r.id == idInt);
                var rol = person.Rol.FirstOrDefault(r => r.id == idInt);
                if (rol == null)
                {

                    person.Rol.Add(_rol);
                }
            }

            ViewBag.id_person = person.id;
            TempData["person"] = /*TempData["person"] ??*/ person;
            TempData.Keep("person");
            return PartialView("_RolsDetailPartial", person.Rol.Where(w => w.name == "Cliente Exterior").ToList());
        }

        #endregion

        #region {RA} - Invoice  Exterior

        [HttpPost]
        public JsonResult getBuyerData(int? id_person)
        {
            Person person = (TempData["person"] as Person);
            if (person != null)
            {
                TempData["person"] = person;
                TempData.Keep("person");
            }
            if (id_person == 0) return Json(new { }, JsonRequestBehavior.AllowGet);

            var personSeek = db.Person.Select(r => new
            {
                id = r.id,
                fullname_businessName = r.fullname_businessName,
                identification_number = r.identification_number,
                address = r.address,
                email = r.ForeignCustomerIdentification.FirstOrDefault().emailInterno,
                emailInterno = r.ForeignCustomerIdentification.FirstOrDefault().emailInternoCC,
                phone1FC = r.ForeignCustomerIdentification.FirstOrDefault().phone1FC,
                fax1FC = r.ForeignCustomerIdentification.FirstOrDefault().fax1FC,
            }).FirstOrDefault(r => r.id == id_person);

            //var jsonData = new JavaScriptSerializer().Serialize(personSeek);

            return Json(personSeek, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult getVendorAsigned(int? id_customer)
        //{
        //var result = new {name = ""};
        //var vendor = db.Customer.FirstOrDefault(p => p.id == id_customer);
        //return Json(new { name = (vendor.Vendor != null) 
        //    ? vendor.Vendor.name 
        //    : "NINGUNO"}, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Copacking
        [HttpPost]
        public JsonResult WarehouseChangeData(int id_warehouse)
        {
            if (TempData["machineForProd"] != null)
            {
                TempData.Keep("machineForProd");
            }
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            var result = new
            {
                warehouseLocations = warehouseLocations
                                    .Select(s => new
                                    {
                                        id = s.id,
                                        name = s.name
                                    })
            };

            TempData.Keep("machineForProd");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CostCenterChangeData(int id_costCenter)
        {
            if (TempData["machineForProd"] != null)
            {
                TempData.Keep("machineForProd");
            }
            var subcenterCost = db.CostCenter.Where(w => w.id_higherCostCenter == id_costCenter && w.isActive && w.id_higherCostCenter != null)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            return Json(subcenterCost, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Provider Plant ?? Copacking
        [HttpPost]
        private bool validarRolExistente(string nombreRol, int[] ids)
        {
            var rol = db.Rol.Where(p => p.name.Equals(nombreRol)).FirstOrDefault().id;
            if (ids.Contains(rol))
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}