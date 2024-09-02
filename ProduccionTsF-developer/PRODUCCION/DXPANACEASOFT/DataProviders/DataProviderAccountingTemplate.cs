using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderAccountingTemplate
	{
		private static DBContext _db = null;
		public static IEnumerable AccountingTemplateByCompany(int id_company)
		{
			using (var db = new DBContext())
			{
				return db.AccountingTemplate
							.Where(w => w.id_company == id_company).ToList();
			}
		}
		public static IEnumerable AccountingTemplateByProductionCostAndProductionExpense(int id_productionCost
			, int id_productionExpense)
		{
			using (var db = new DBContext())
			{
				return db.AccountingTemplate
							.Where(w => w.id_costProduction == id_productionCost
							&& w.id_expenseProduction == id_productionExpense).ToList();
			}
		}
		public static AccountingTemplate AccountingTemplateById(int? id)
		{
			_db = new DBContext();
			var query = _db.AccountingTemplate.FirstOrDefault(t => t.id == id);
			return query;
		}

		public static IEnumerable LoadAccountingTempate(int? id_productionCost, int? id_productionExpense)
        {
			_db = new DBContext();
			var result = _db.AccountingTemplate.Where(e => e.id_costProduction == id_productionCost && e.id_expenseProduction == id_productionExpense).ToList();
			return result;
        }

		public static IEnumerable LoadCodeComboAccountingTemplate(List<string> codigos)
		{

			var dbCI = new DBContextIntegration();
			var codeAccount = dbCI.TblciCuenta.Where(e => e.CCeCuenta == "A").ToList();

			string result, result2, result3;
			var code = dbCI.TblciCuenta.ToList();
			string level1, level2, level3, level4, level5;

            var planDeCuenta = dbCI.tblCiPlanDeCuenta.Where(e => e.CCePlanCtas == "A").ToList();
			int plan = 0, plan1 = 0, plan2 = 0, plan3 = 0, plan4 = 0, plan5 = 0, sumalevel, sumalevel5, sumalevel4, sumalevel3, sumalevel2;

			List<decimal?> bb = new List<decimal?>();
			for(var i = 0; i < planDeCuenta.Count(); i++)
            {
				plan = (int)planDeCuenta[0].NNuNivel1;
				plan1 = (int)planDeCuenta[0].NNuNivel2;
				plan2 = (int)planDeCuenta[0].NNuNivel3;
				plan3 = (int)planDeCuenta[0].NNuNivel4;
				plan4 = (int)planDeCuenta[0].NNuNivel5;
				plan5 = (int)planDeCuenta[0].NNuNivel6;
            }

			sumalevel = plan + plan1 + plan2 + plan3 + plan4 + plan5;
			sumalevel5 = plan + plan1 + plan2 + plan3 + plan4;
			sumalevel4 = plan + plan1 + plan2 + plan3;
			sumalevel3 = plan + plan1 + plan2;
			sumalevel2 = plan + plan1;


			int lenghtLevel = 0;

			if(codigos != null)
            {
				for (var i = 0; i < codigos.Count(); i++)
				{
					result = codigos[i].Trim();
					lenghtLevel = result.Length;

					//Protegemos el código actual de la cuenta
					result2 = result + "0";
					result3 = result + "1";

					var listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta == result);
					
					// Eliminación de cuentas hacia Arriba
					//Cuenta Nivel 6
					if(lenghtLevel == sumalevel)
                    {
						//Accedemos un nivel más bajo 5
						level5 = result.Remove(result.Length - plan5);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level5);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 4
						level4 = level5.Remove(level5.Length - plan4);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level4);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 3
						level3 = level4.Remove(level4.Length - plan3);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level3);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 2
						level2 = level3.Remove(level3.Length - plan2);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level2);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 1
						level1 = level2.Remove(level2.Length - plan1);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level1);
						codeAccount.Remove(listaAux2);
					}
					else if(lenghtLevel == sumalevel5) // Cuenta Nivel 5
					{
						//Accedemos un nivel más bajo 4
						level4 = result.Remove(result.Length - plan4);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level4);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 3
						level3 = level4.Remove(level4.Length - plan3);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level3);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 2
						level2 = level3.Remove(level3.Length - plan2);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level2);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 1
						level1 = level2.Remove(level2.Length - plan1);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level1);
						codeAccount.Remove(listaAux2);
					}
					else if (lenghtLevel == sumalevel4) //Cuenta Nivel 4
					{
						//Accedemos un nivel más bajo 3
						level3 = result.Remove(result.Length - plan3);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level3);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 2
						level2 = level3.Remove(level3.Length - plan2);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level2);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 1
						level1 = level2.Remove(level2.Length - plan1);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level1);
						codeAccount.Remove(listaAux2);
					}
					else if(lenghtLevel == sumalevel3) // Cuenta Nivel 3
                    {
						//Accedemos un nivel más bajo 2
						level2 = result.Remove(result.Length - plan2);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level2);
						codeAccount.Remove(listaAux2);

						//Accedemos un nivel más bajo 1
						level1 = level2.Remove(level2.Length - plan1);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level1);
						codeAccount.Remove(listaAux2);
					}
					else if(lenghtLevel == sumalevel2) // Cuenta Nivel 1
                    {
						//Accedemos un nivel más bajo 1
						level1 = result.Remove(result.Length - plan1);
						listaAux2 = codeAccount.FirstOrDefault(e => e.CCiCuenta.Trim() == level1);
						codeAccount.Remove(listaAux2);
					}

					//Eliminación de Cuentas hacia Abajo
					code = codeAccount.Where(e => !e.CCiCuenta.Contains(result2)).ToList();
					codeAccount = code;

					code = codeAccount.Where(e => !e.CCiCuenta.Contains(result3)).ToList();
					codeAccount = code;


				}
			}
            else
            {
				code = null;
            }
			

			return codeAccount;
		}

		public static TblCiProyecto AccountingTemplateCenterCostById(string id)
		{
			var dbCI = new DBContextIntegration();
			var codeAccount2 = dbCI.TblCiProyecto.FirstOrDefault(i => i.CCiProyecto == id);
			if(codeAccount2 != null)
            {
				return codeAccount2;
			}
            else
            {
				return null;
            }
		}

		public static TblCiSubProyecto AccountingTemplateSubCenterCostById(string id, string idSub)
		{
			var dbCI = new DBContextIntegration();
			var codeAccount = dbCI.TblCiSubProyecto.FirstOrDefault(i => i.CCiProyecto == id && i.CCiSubProyecto == idSub);
			if (codeAccount != null)
			{
				return codeAccount;
			}
			else
			{
				return null;
			}
		}

		public static TblCiTipoAuxiliar AccountingTemplateTypeAuxiliar(string id)
		{
			var dbCI = new DBContextIntegration();
			var codeTipoAuxiliar = dbCI.TblCiTipoAuxiliar .FirstOrDefault(i => i.CCiTipoAuxiliar == id);
			if (codeTipoAuxiliar != null)
			{
				return codeTipoAuxiliar;
			}
			else
			{
				return null;
			}
		}
		public static TblCiAuxiliar AccountingTemplateAuxiliar(string id)
		{
			var dbCI = new DBContextIntegration();
			var codeAccount = dbCI.TblCiAuxiliar.FirstOrDefault(i => i.CCiAuxiliar == id);
			if (codeAccount != null)
			{
				return codeAccount;
			}
			else
			{
				return null;
			}
		}
		public static IEnumerable AccountingTemplateTypeAuxiliarAll(string cciCuenta, bool? aceptaAuxiliar)
		{
			var dbCI = new DBContextIntegration();
			string tmpCciCuenta = cciCuenta;

			tmpCciCuenta = string.IsNullOrEmpty(tmpCciCuenta) ? "" : tmpCciCuenta.Trim();

			if (aceptaAuxiliar == null)
				return null;
			if (aceptaAuxiliar.Value == false)
				return null;

			var lsTipoAuxiliarAceptado = dbCI.TblCiRel_CtaMy_GpoTipAux
										.Where(w => w.CCiCuenta == tmpCciCuenta
										&& w.CCeRel_CtaMy_GpoTipAux == "A")
										.Select(s => s.CCiGpoTipoAuxiliar).ToList();

			var codeAccount = dbCI.TblCiTipoAuxiliar
				.Where(w => lsTipoAuxiliarAceptado.Contains(w.CCiTipoAuxiliar)
				&& w.CCeTipoAuxiliar =="A")
				.Select(s => new { name = s.CDsTipoAuxiliar, id = s.CCiTipoAuxiliar }).ToList();

			if (codeAccount != null)
			{
				return codeAccount;
			}
			else
			{
				return null;
			}
		}
		public static IEnumerable AccountingTemplateAuxiliarByTypeAuxiliar(string typeAuxiliar)
		{
			var dbCI = new DBContextIntegration();

			var auxiliarTmp = dbCI
								.TblCiRel_Auxiliar_TipoAuxiliar
								.Where(w => w.CCiTipoAuxiliar == typeAuxiliar)
								.Select(s => s.CCiAuxiliar)
								.ToList();

			var codeAccount = dbCI.TblCiAuxiliar.Where(i => auxiliarTmp.Contains(i.CCiAuxiliar)).Select(s => new { name = s.CDsAuxiliar, id = s.CCiAuxiliar }).ToList();

			if (codeAccount != null)
			{
				return codeAccount;
			}
			else
			{
				return null;
			}
		}
		public static IEnumerable AccountingTemplateAuxiliarNombre(string codigo)
		{
			var dbCI = new DBContextIntegration();

			var codeAccount = dbCI.TblCiAuxiliar.Where(i => i.CCiAuxiliar == codigo).Select(s => new { name = s.CDsAuxiliar, id = s.CCiAuxiliar}).ToList();
			
			if (codeAccount != null)
			{
				return codeAccount;
			}
			else
			{
				return null;
			}
		}

		public static IEnumerable AccountingTemplateCenterCost()
		{
			var dbCI = new DBContextIntegration();
			var codeAccount2 = dbCI.TblCiProyecto.Select(s => new { name = s.CDsProyecto, id = s.CCiProyecto }).ToList();
			if (codeAccount2 != null)
			{
				return codeAccount2;
			}
			else
			{
				return null;
			}
		}

		public static IEnumerable AccountingTemplateSubCenterCost(string id)
		{
			var dbCI = new DBContextIntegration();
			var codeAccount = dbCI.TblCiSubProyecto.Where(i => i.CCiProyecto == id).Select(s => new { CDsSubProyecto = s.CDsSubProyecto, id = s.CCiSubProyecto }).ToList(); ;
			if (codeAccount != null)
			{
				return codeAccount;
			}
			else
			{
				return null;
			}
		}

		public static TblciCuenta AccountingTemplateCode(string id)
		{
			var dbCI = new DBContextIntegration();
			var codeAccount = dbCI.TblciCuenta.FirstOrDefault(i => i.CCiCuenta == id);
			if (codeAccount != null)
			{
				return codeAccount;
			}
			else
			{
				return null;
			}
		}
	}
}