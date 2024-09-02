using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models
{
	public class ListaMonth
	{
		private List<Month> lsMonths;
		public List<Month> LsMonth { get { return this.lsMonths; } }
		public ListaMonth()
		{
			lsMonths = new List<Month>();
			lsMonths.Add(new Month("1", "ENERO"));
			lsMonths.Add(new Month("2", "FEBRERO"));
			lsMonths.Add(new Month("3", "MARZO"));
			lsMonths.Add(new Month("4", "ABRIL"));
			lsMonths.Add(new Month("5", "MAYO"));
			lsMonths.Add(new Month("6", "JUNIO"));

			lsMonths.Add(new Month("7", "JULIO"));
			lsMonths.Add(new Month("8", "AGOSTO"));
			lsMonths.Add(new Month("9", "SEPTIEMBRE"));
			lsMonths.Add(new Month("10", "OCTUBRE"));
			lsMonths.Add(new Month("11", "NOVIEMBRE"));
			lsMonths.Add(new Month("12", "DICIEMBRE"));
		}
		public class Month
		{
			private string codigo { get; set; }
			private string descripcion { get; set; }
			public string Codigo { get { return this.codigo; } }
			public string Descripcion { get { return this.descripcion; } }
			public Month(string _codigo, string _descripcion)
			{
				this.codigo = _codigo;
				this.descripcion = _descripcion;
			}
		}
	}
	public class ListaYear
	{
		public List<Year> LsYear { get { return this.lsYear; } }
		private List<Year> lsYear;
		public ListaYear(int quantity)
		{
			int _quantityAbsolute = 0;
			_quantityAbsolute = Math.Abs(quantity);
			int yearCurrent = DateTime.Now.Year;


			lsYear = new List<Year>();
			if (quantity < 0)
			{
				int yearStart = 0;
				yearStart = yearCurrent - _quantityAbsolute;
				while (yearStart <= yearCurrent)
				{
					lsYear.Add(new Year(yearStart.ToString(), yearStart.ToString()));
					yearStart++;
				}
			}
			if (quantity >= 0)
			{
				int yearEnd = 0;
				yearEnd = yearCurrent + _quantityAbsolute;
				while (yearCurrent <= yearEnd)
				{
					lsYear.Add(new Year(yearCurrent.ToString(), yearCurrent.ToString()));
					yearCurrent++;
				}
			}
		}
		public class Year
		{
			private string codigo { get; set; }
			private string descripcion { get; set; }
			public string Codigo { get { return this.codigo; } }
			public string Descripcion { get { return this.descripcion; } }
			public Year(string _codigo, string _descripcion)
			{
				this.codigo = _codigo;
				this.descripcion = _descripcion;
			}
		}
	}
	public class ListaAssignmentTypeProductionCoefficient
	{
		public List<AssignmentTypeProductionCoefficient> LsAssignmentTypeProductionCoefficient { get { return this.lsAssignmentTypeProductionCoefficient; } }

		private List<AssignmentTypeProductionCoefficient> lsAssignmentTypeProductionCoefficient;

		public ListaAssignmentTypeProductionCoefficient()
		{
			lsAssignmentTypeProductionCoefficient = new List<AssignmentTypeProductionCoefficient>();
			this.lsAssignmentTypeProductionCoefficient.Add(new AssignmentTypeProductionCoefficient("COSTOREAL", "Costo Real"));
			this.lsAssignmentTypeProductionCoefficient.Add(new AssignmentTypeProductionCoefficient("COSTOPROY", "Costo Proyectado"));
		}

		public class AssignmentTypeProductionCoefficient
		{
			private string codigo { get; set; }
			private string descripcion { get; set; }
			public string Codigo { get { return this.codigo; } }
			public string Descripcion { get { return this.descripcion; } }
			public AssignmentTypeProductionCoefficient(string _codigo, string _descripcion)
			{
				this.codigo = _codigo;
				this.descripcion = _descripcion;
			}
		}
	}
	public class ListaTipoFormulaSimple
	{
		public List<TipoFormulaSimple> LsTipoFormulaSimple { get { return this.lsTipoFormulaSimple; } }

		private List<TipoFormulaSimple> lsTipoFormulaSimple;
		public ListaTipoFormulaSimple()
		{
			lsTipoFormulaSimple = new List<TipoFormulaSimple>();
			this.lsTipoFormulaSimple.Add(new TipoFormulaSimple("TIPOBODEGA", "Tipo Bodega"));
		}
		public class TipoFormulaSimple
		{
			private string codigo;
			private string descripcion;
			public string Codigo { get { return this.codigo; } }
			public string Descripcion { get { return this.descripcion; } }
			public TipoFormulaSimple(string _codigo, string _descripcion)
			{
				this.codigo = _codigo;
				this.descripcion = _descripcion;
			}
		}
	}
    public class ListaTipoLibrasCosteoProducion
    {
        public List<TipoLibraCosteoProduccion> LsTipoLibraCosteoProduccion { get { return this.lsTipoLibraCosteoProduccion; } }
        public ListaTipoLibrasCosteoProducion()
        {
            lsTipoLibraCosteoProduccion = new List<TipoLibraCosteoProduccion>();
            this.lsTipoLibraCosteoProduccion.Add(new TipoLibraCosteoProduccion("AMBAS", "Ambas", false));
            this.lsTipoLibraCosteoProduccion.Add(new TipoLibraCosteoProduccion("LIBPRO", "Libras Procesadas", true));
            this.lsTipoLibraCosteoProduccion.Add(new TipoLibraCosteoProduccion("LIBTERM", "Libras Terminadas", false));
        }
        private List<TipoLibraCosteoProduccion> lsTipoLibraCosteoProduccion;
        public class TipoLibraCosteoProduccion
        {
            private string codigo;
            private string descripcion;
            private bool isDefault;
            public string Codigo { get { return this.codigo; } }
            public bool IsDefault { get { return this.isDefault; } }
            public string Descripcion { get { return this.descripcion; } }
            public TipoLibraCosteoProduccion(string _codigo, string _descripcion, bool _isDefault)
            {
                this.codigo = _codigo;
                this.descripcion = _descripcion;
                this.isDefault = _isDefault;
            }
        }
    }
}