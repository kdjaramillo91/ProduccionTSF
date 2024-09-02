using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderPlanDeCuentas
	{
		private const string m_IntegracionSettingCode = "INTX-CONTAB";

		public static IEnumerable PlanesDeCuentas(int? id_company)
		{
			var planCuentasLista = new List<tblCiPlanDeCuenta>();

			var planCuentas = GetPlanDeCuentaByCompany(id_company);

			if ((planCuentas != null)
				&& (planCuentas.CCePlanCtas == "A"))
			{
				planCuentasLista.Add(planCuentas);
			}

			return planCuentasLista;
		}
		public static tblCiPlanDeCuenta PlanDeCuentasById(string code)
		{
			return new DBContextIntegration()
				.tblCiPlanDeCuenta
				.FirstOrDefault(t => t.CCiPlanCta == code);
		}
		public static tblCiPlanDeCuenta GetPlanDeCuentaByCompany(int? id_company)
		{
			// Recuperamos el código de compañía
			var companyCode = new DBContext()
				.Company
				.FirstOrDefault(c => c.id == id_company)?
				.code;

			if (String.IsNullOrWhiteSpace(companyCode))
			{
				return null;
			}

			// Recuperamos el código de plan de cuentas
			var planCuentasCode = new DBContextIntegration()
				.tblCiCias
				.FirstOrDefault(c => c.CCiCia == companyCode && c.CCeCia == "A")?
				.CCiPlanCta;

			if (String.IsNullOrWhiteSpace(planCuentasCode))
			{
				return null;
			}

			// Verificamos que el plan de cuentas existe y retornamos el elemento
			return new DBContextIntegration()
				.tblCiPlanDeCuenta
				.FirstOrDefault(p => p.CCiPlanCta == planCuentasCode);
		}

		public static IEnumerable<TblciCuenta> GetCuentasContablesByCurrent(string idPlanDeCuentas, string idCuentaContableActual)
		{
			if (String.IsNullOrEmpty(idPlanDeCuentas))
			{
				return new TblciCuenta[] { };
			}

			return new DBContextIntegration()
				.TblciCuenta
				.Where(c => (c.CCiPlanCta == idPlanDeCuentas
							&& c.CCtTituloDetalle == "D"
							&& c.CCeCuenta == "A")
							|| (c.CCiCuenta == idCuentaContableActual))
				.OrderBy(c => c.CCiCuenta)
				.ToArray();
		}

		public static IEnumerable<TblCiTipoAuxiliar> GetTiposAuxiliaresContablesByCurrent(
			string idPlanDeCuentas, string idCuentaContable, string idTipoAuxiliarActual)
		{
			if (String.IsNullOrEmpty(idPlanDeCuentas)
				|| String.IsNullOrEmpty(idCuentaContable))
			{
				return new TblCiTipoAuxiliar[] { };
			}

			var dbIntegration = new DBContextIntegration();

			// Recuperamos la lista de tipos de auxiliares
			var idsTiposAuxiliares = dbIntegration
				.TblCiRel_CtaMy_GpoTipAux
				.Where(rta => rta.CCiPlanCta == idPlanDeCuentas
							&& rta.CCiCuenta == idCuentaContable
							&& rta.CCeRel_CtaMy_GpoTipAux == "A")
				.Select(rta => rta.CCiGpoTipoAuxiliar)
				.ToArray();

			return dbIntegration
				.TblCiTipoAuxiliar
				.Where(ta => (idsTiposAuxiliares.Contains(ta.CCiTipoAuxiliar)
							&& ta.CCeTipoAuxiliar == "A")
							|| (ta.CCiTipoAuxiliar == idTipoAuxiliarActual))
				.OrderBy(ta => ta.CCiTipoAuxiliar)
				.ToArray();
		}
		public static IEnumerable<TblCiAuxiliar> GetAuxiliaresContablesByCurrent(
			string idTipoAuxiliar, string idAuxiliarActual)
		{
			if (String.IsNullOrEmpty(idTipoAuxiliar))
			{
				return new TblCiAuxiliar[] { };
			}

			var dbIntegration = new DBContextIntegration();

			// Recuperamos la lista de tipos de auxiliares
			var idsAuxiliares = dbIntegration
				.TblCiRel_Auxiliar_TipoAuxiliar
				.Where(rta => rta.CCiTipoAuxiliar == idTipoAuxiliar
							&& rta.CCeRel_Aux_TipoAux == "A")
				.Select(rta => rta.CCiAuxiliar)
				.ToArray();

			return dbIntegration
				.TblCiAuxiliar
				.Where(ax => (idsAuxiliares.Contains(ax.CCiAuxiliar)
							&& ax.CCeAuxiliar == "A")
							|| (ax.CCiAuxiliar == idAuxiliarActual))
				.OrderBy(ta => ta.CCiAuxiliar)
				.ToArray();
		}

		public static IEnumerable<TblCiTipoPresObra> GetTiposPresupuestoContablesByCurrent(
			string idTipoPresupuestoActual)
		{
			// Recuperamos la lista de tipos de presupuesto
			return new DBContextIntegration()
				.TblCiTipoPresObra
				.Where(p => (p.CCeTipoPres == "A")
							|| (p.CCiTipoPres == idTipoPresupuestoActual))
				.OrderBy(p => p.CCiTipoPres)
				.ToArray();
		}
		public static IEnumerable<TblCiProyecto> GetCentrosCostoContablesByCurrent(
			string idTipoPresupuesto, string idCentroCostoActual)
		{
			if (String.IsNullOrEmpty(idTipoPresupuesto))
			{
				return new TblCiProyecto[] { };
			}

			// Recuperamos la lista de centros de costo
			return new DBContextIntegration()
				.TblCiProyecto
				.Where(c => (c.CCiTipoPres == idTipoPresupuesto
							&& c.CCeProyecto == "A")
							|| (c.CCiProyecto == idCentroCostoActual))
				.OrderBy(c => c.CCiProyecto)
				.ToArray();
		}
		public static IEnumerable<TblCiSubProyecto> GetSubcentrosCostoContablesByCurrent(
			string idTipoPresupuesto, string idCentroCosto, string idSubcentroCostoActual)
		{
			if (String.IsNullOrEmpty(idTipoPresupuesto)
				|| String.IsNullOrEmpty(idCentroCosto))
			{
				return new TblCiSubProyecto[] { };
			}

			// Recuperamos la lista de subcentros de costo
			return new DBContextIntegration()
				.TblCiSubProyecto
				.Where(c => (c.CCiTipoPres == idTipoPresupuesto
							&& c.CCiProyecto == idCentroCosto
							&& c.CCeSubProyecto == "A")
							|| (c.CCiSubProyecto == idSubcentroCostoActual))
				.OrderBy(c => c.CCiSubProyecto)
				.ToArray();
		}

		public static bool IsEnabledIntegracionContable()
		{
			var value = new DBContext()
				.Setting
				.FirstOrDefault(t => t.code == m_IntegracionSettingCode && t.isActive)?
				.value;

			return !String.IsNullOrWhiteSpace(value) && (value != "0");
		}
	}
}