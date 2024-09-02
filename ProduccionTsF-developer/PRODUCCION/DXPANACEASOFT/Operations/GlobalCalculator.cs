using System;

namespace DXPANACEASOFT.Operations
{
	public static class GlobalCalculator
	{
		public static int RedondearEntero(decimal valor)
		{
			var resultado = Decimal.Round(valor, 0, MidpointRounding.AwayFromZero);
			return Decimal.ToInt32(resultado);
		}

		public static decimal RedondearCantidad(decimal cantidad)
		{
			return Decimal.Round(cantidad, 6, MidpointRounding.AwayFromZero);
		}

		public static decimal RedondearFactor(decimal factor)
		{
			return Decimal.Round(factor, 6, MidpointRounding.AwayFromZero);
		}

		public static decimal RedondearMonto(decimal cantidad)
		{
			return Decimal.Round(cantidad, 2, MidpointRounding.AwayFromZero);
		}

		public static decimal RedondearMontoDetalle(decimal cantidad)
		{
			return Decimal.Round(cantidad, 4, MidpointRounding.AwayFromZero);
		}

		public static decimal RedondearMontoMayorPrecision(decimal cantidad)
		{
			return Decimal.Round(cantidad, 6, MidpointRounding.AwayFromZero);
		}
		public static decimal RedondearMontoTotalPrecision(decimal cantidad)
		{
			return Decimal.Round(cantidad, 18, MidpointRounding.AwayFromZero);
		}

		public static int CalcularDiasRango(DateTime fechaInicio, DateTime? fechaFinal)
		{
			var diasRango = ((fechaFinal ?? DateTime.Today) - fechaInicio).Days + 1;

			if (diasRango < 0)
			{
				diasRango = 0;
			}

			return diasRango;
		}

		public static DateTime? CalcularFechaMenor(DateTime? fecha1, DateTime? fecha2)
		{
			if (!fecha1.HasValue || !fecha2.HasValue)
			{
				return fecha1 ?? fecha2;
			}
			else if (fecha1.Value <= fecha2.Value)
			{
				return fecha1;
			}
			else
			{
				return fecha2;
			}
		}

		public static DateTime? CalcularFechaMayor(DateTime? fecha1, DateTime? fecha2)
		{
			if (!fecha1.HasValue || !fecha2.HasValue)
			{
				return fecha1 ?? fecha2;
			}
			else if (fecha1.Value >= fecha2.Value)
			{
				return fecha1;
			}
			else
			{
				return fecha2;
			}
		}
	}
}
