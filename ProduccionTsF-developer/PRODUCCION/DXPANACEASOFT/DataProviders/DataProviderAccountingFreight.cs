using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderAccountingFreight
    {
        public static IEnumerable GetLiquidationType()
        {

            var liquidationTypes = new List<object>
            {
                new { id = "T", name = "TERRESTRE" },
                new { id = "F", name = "FLUVIAL" }
            };
            return liquidationTypes;
        }

        public static string AccountingCountCode(string code)
        {
            DBContextIntegration dbCI = new DBContextIntegration();
            DBContext db = new DBContext();
            var parameterCC = db.Setting.Where(x => x.code == "PLANCTACONT").Select(x => x.value).FirstOrDefault();

            var accountingAccountPlan = dbCI.tblCiPlanDeCuenta.Where(x => x.CDsPlanCuenta == parameterCC ).FirstOrDefault();

            var accountingAccount = dbCI.TblciCuenta.Where(x => x.CCiPlanCta == accountingAccountPlan.CCiPlanCta && x.CCiCuenta == code ).FirstOrDefault();

            return accountingAccount.CCiCuenta.ToString() + " - " + accountingAccount.CDsCuenta;

        }

        public static IEnumerable AccountingCount()
        {
            DBContextIntegration dbCI = new DBContextIntegration();
            DBContext db = new DBContext();
            var parameterCC = db.Setting.Where(x => x.code == "PLANCTACONT").Select(x => x.value).FirstOrDefault();

            var accountingAccountPlan = dbCI.tblCiPlanDeCuenta.Where(x => x.CDsPlanCuenta == parameterCC).FirstOrDefault();

            var accountingAccount = dbCI.TblciCuenta.Where(x => x.CCiPlanCta == accountingAccountPlan.CCiPlanCta).ToList();

            var list = accountingAccount.Select( c=> new{
                idCuentaContable = c.CCiCuenta,
                cuentaContable = c.CDsCuenta,
                descripcion = String.Concat(c.CCiCuenta.TrimEnd(), " - ", c.CDsCuenta.TrimEnd()),
                aceptaAuxiliar = c.BSnAceptaAux.HasValue ? c.BSnAceptaAux.Value : false,
                aceptaCentroCosto = c.BsnAceptaProyecto.HasValue ? c.BsnAceptaProyecto.Value : false
            }).ToList();

            return list;
        }

        public static IEnumerable<TblCiTipoAuxiliar> AccountingCountTypeAuxiliar(string id_cuentaContable)
        {
            if (String.IsNullOrEmpty(id_cuentaContable))
            {
                return new TblCiTipoAuxiliar[] { };
            }

            DBContextIntegration dbCI = new DBContextIntegration();

            var idsTiposAuxiliares = dbCI
                .TblCiRel_CtaMy_GpoTipAux
                .Where(rta => rta.CCiCuenta == id_cuentaContable)
                .Select(rta => rta.CCiGpoTipoAuxiliar)
                .ToArray();

            return dbCI
                .TblCiTipoAuxiliar
                .Where(ta => (idsTiposAuxiliares.Contains(ta.CCiTipoAuxiliar)))
                .OrderBy(ta => ta.CCiTipoAuxiliar)
                .ToArray();
        }

        public static IEnumerable GetAuxiliaresContablesByCurrent(
             string idTipoAuxiliar, string idAuxiliarActual)
        {
            if (String.IsNullOrEmpty(idTipoAuxiliar))
            {
                return new TblCiAuxiliar[] { };
            }

            DBContextIntegration dbCI = new DBContextIntegration();

            // Recuperamos la lista de tipos de auxiliares
            var idsAuxiliares = dbCI
                .TblCiRel_Auxiliar_TipoAuxiliar
                .Where(rta => rta.CCiTipoAuxiliar == idTipoAuxiliar)
                .Select(rta => rta.CCiAuxiliar)
                .ToArray();

            return dbCI
                .TblCiAuxiliar
                .Where(ax => (idsAuxiliares.Contains(ax.CCiAuxiliar)
                            || (ax.CCiAuxiliar == idAuxiliarActual)))
                .Select( ax => new
                {
                    idAuxContable = ax.CCiAuxiliar,
                    auxContable = ax.CDsAuxiliar
                })
                .OrderBy(ta => ta.idAuxContable)
                .ToArray();
        }

        public static string GetAuxiliaresContablesForCode(string idAuxiliarActual)
        {
            DBContextIntegration dbCI = new DBContextIntegration();

            return idAuxiliarActual + " - " + dbCI.TblCiAuxiliar.Where(x => x.CCiAuxiliar == idAuxiliarActual).Select(x => x.CDsAuxiliar).FirstOrDefault();

        }

        public static string GetTypeAuxiliaresContablesForCode(string code_Auxiliar)
        {
            DBContextIntegration dbCI = new DBContextIntegration();

            return code_Auxiliar + " - " + dbCI.TblCiTipoAuxiliar.Where(x => x.CCiTipoAuxiliar == code_Auxiliar).Select(x => x.CDsTipoAuxiliar).FirstOrDefault();

        }


        public static IEnumerable GetAccountingType()
        {

            var liquidationTypes = new List<object>
            {
                new { id = "D", name = "DÉBITO" },
                new { id = "C", name = "CRÉDITO" }
            };
            return liquidationTypes;
        }

        public static IEnumerable GetAccountingTypeCode(string accountType)
        {

            var liquidationTypes = new List<object>
            {
                new { id = "D", name = "DÉBITO" },
                new { id = "C", name = "CRÉDITO" }
            };

            var account = "";
            foreach (var item in liquidationTypes)
            {
                // Verificar si el ID del elemento actual coincide con el ID proporcionado
                if (item.GetType().GetProperty("id").GetValue(item, null).ToString() == accountType)
                {
                    // Si hay una coincidencia, devolver el valor de la propiedad "name"
                     account =  item.GetType().GetProperty("name").GetValue(item, null).ToString();
                }
            }

            return account;
        }


        public static string GetCostCenter(int? id)
        {
            DBContext db = new DBContext();
            var costCenter = db.CostCenter.Where(x => x.id == id).Select(x => x.name).FirstOrDefault();


            return costCenter;

        }

        public static IEnumerable GetCostCenterList()
        {
            DBContext db = new DBContext();
            var costCenter = db.CostCenter.Where(x => x.isActive && x.id_higherCostCenter == null ).ToList();

            var listCC = costCenter.Select(c => new
            {
                id = c.id,
                name = c.name
            }).ToList();

            return listCC;

        }

    }
}