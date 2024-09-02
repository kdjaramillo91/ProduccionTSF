using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderInventoryReason
	{
		private static DBContext db = null;

		public class CategoriaCosto
        {
			public string Codigo { get; set; }
			public string Nombre { get; set; }

			public CategoriaCosto(string codigo, string Nombre)
            {
				this.Codigo = codigo;
				this.Nombre = Nombre;
            }
        }

		public class MotivoCosto
        {
			public string Codigo { get; set; }
			public string Nombre { get; set; }

			public MotivoCosto(string codigo, string Nombre)
			{
				this.Codigo = codigo;
				this.Nombre = Nombre;
			}
		}

		public static InventoryReason InventoryReasonById(int? id_InventoryReason)
		{
			db = new DBContext();
			var InventoryReason = db.InventoryReason.Where(t => t.id == id_InventoryReason).FirstOrDefault();
			return InventoryReason;
		}
		public static IEnumerable GetAll()
		{
			db = new DBContext();

			var ls = db.InventoryReason.Where(w => w.isActive).Select(s => new { id = s.id.ToString(), name = s.name}).ToList();

			return ls;
		}
		public static IEnumerable InventoryReasonAll(int? id_current)
		{
			db = new DBContext();
			var InventoryReasonaux = db.InventoryReason.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

			if (id_current != null && id_current > 0)
			{
				var cant = (from de in InventoryReasonaux
							where de.id == id_current
							select de).ToList().Count;
				if (cant == 0)
				{
					var InventoryReasoncuuretaux = db.InventoryReason.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

					InventoryReasonaux.AddRange(InventoryReasoncuuretaux);
				}
			}

			return InventoryReasonaux;
		}
		public static string GetCodeNatureMoveById(int? idNatureMove)
		{
			db = new DBContext();
			var aNatureMove = db.AdvanceParametersDetail.FirstOrDefault(t => t.id == idNatureMove);
            return aNatureMove?.valueCode.Trim();
		}
		public static IEnumerable GetValorizations()
		{
			var aListSelectItem = new List<SelectListItem>();
            aListSelectItem.Add (new SelectListItem
								{
									Text ="Manual",
									Value="Manual"
								});
			aListSelectItem.Add (new SelectListItem
								{
									Text ="Automático",
									Value="Automático"
								});
			return aListSelectItem;
		}
		public static IEnumerable GetTypeOfCalculations(string valorization)
		{
			var aListSelectItem = new List<SelectListItem>();
            aListSelectItem.Add (new SelectListItem
								{
									Text ="Promedio",
									Value="Promedio"
								});
			aListSelectItem.Add (new SelectListItem
								{
									Text ="Proceso",
									Value="Proceso"
								});
            if (valorization == "Automático") {
                aListSelectItem.Add(new SelectListItem
                {
                    Text = "Heredado",
                    Value = "Heredado"
                });
            }
            return aListSelectItem;

        }
        public static IEnumerable GetInventoryReasonDifIdNatureMove(int? idNatureMove)
		{
			db = new DBContext();
			var InventoryReasonaux = db.InventoryReason.Where(g => (g.isActive && g.idNatureMove != idNatureMove)).Select(p => new { p.id, p.name }).ToList();

           return InventoryReasonaux;
		}

        public static IEnumerable GetInventoryReasonExit()
        {
            db = new DBContext();
            var InventoryReasonaux = db.InventoryReason.Where(g => (g.isActive && g.AdvanceParametersDetail.valueCode == "E")).Select(p => new { p.id, p.name }).ToList();

            return InventoryReasonaux;
        }

		#region Categorias de costos
		public const string m_SaldoInicial = "CTCSSAIN";
		public const string m_Compras = "CTCSCMPR";
		public const string m_InventarioFinal = "CTCSINVF";
		public const string m_CostoVentas = "CTCSCSVN";
		public const string m_AjusteInventario = "CTCSAJIN";
		public const string m_IngresosTransferencias = "CTCSINTR";
		public const string m_EgresosTransferencias = "CTCSEGTR";
		#endregion

		public static IEnumerable GetCategoriaCosto()
        {
			var categoriaCosto = new[]
			{
				new CategoriaCosto(m_SaldoInicial, "Saldo Inicial"),
				new CategoriaCosto(m_Compras, "Compras"),
				new CategoriaCosto(m_InventarioFinal, "Inventario Final"),
				new CategoriaCosto(m_CostoVentas, "Costo de Ventas"),
				new CategoriaCosto(m_AjusteInventario, "Ajuste de Inventario"),
				new CategoriaCosto(m_IngresosTransferencias, "Ingresos y Transferencias"),
				new CategoriaCosto(m_EgresosTransferencias, "Egresos y Transferencias"),
			};

			return categoriaCosto.Select(p => new { p.Codigo, p.Nombre }).ToList(); ;
        }
		public static string GetCategoriaCostoString(string codigo)
        {
            switch (codigo)
            {
				case m_SaldoInicial:
					return "Saldo Inicial";
				case m_Compras:
					return "Compras";
				case m_InventarioFinal:
					return "Inventario Final";
				case m_CostoVentas:
					return "Costo de Ventas";
				case m_AjusteInventario:
					return "Ajuste de Inventario";
				case m_IngresosTransferencias:
					return "Ingresos y Transferencias";
				case m_EgresosTransferencias:
					return "Egresos y Transferencias";

				default: return "Codigo desconocido";
			}
        }

		#region Motivos de Costeo y Motivos de Egreso

		public const string m_CalculoNinguno = "NINGUNO";
		public const string m_CalculoProductoTalla = "PRODTALL";
		public const string m_CalculoPromedioLibra = "PROMEDI";

		public static IEnumerable GetMotivosCosto()
        {
			var motivosCosto = new[]
			{
				new MotivoCosto(m_CalculoNinguno, "Ninguno"),
				new MotivoCosto(m_CalculoProductoTalla, "Producto - Talla"),
				new MotivoCosto(m_CalculoPromedioLibra, "Promedio Libras"),
			};
			return motivosCosto.Select(p => new { p.Codigo, p.Nombre }).ToList();
		}

		public static IEnumerable GetMotivosEgreso()
        {
			var idAdvanceParameter = db.AdvanceParameters.FirstOrDefault(e => e.code == "NMMGI")?.id;
			var idNatureMove = db.AdvanceParametersDetail
				.FirstOrDefault(e => e.id_AdvanceParameters == idAdvanceParameter
					&& e.valueCode.Trim() == "E")?
					.id;

			db = new DBContext();
			var motivosEgreso = db.InventoryReason
				.Where(e => e.idNatureMove == idNatureMove)
				.Select(p => new
				{
					p.id,
                    p.code,
                    p.name, 
				}).ToList();


			return motivosEgreso;
		}

		#endregion
	}
}

			