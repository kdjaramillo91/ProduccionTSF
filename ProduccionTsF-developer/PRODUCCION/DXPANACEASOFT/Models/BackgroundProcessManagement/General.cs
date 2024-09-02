using System;
using System.ComponentModel;

namespace DXPANACEASOFT.Models.BackgroundProcessManagement
{
    public class MonthlyBalanceExecution
    {
        public int Id_company { get; set; }
        public int Year { get; set; }
        public int UserId { get; set; }
        public MonthlyBalancePeriodo[] Periods { get; set; }
    }

    public class MonthlyBalancePeriodo
    {
        public int id_Warehouse { get; set; }
        public string codeTypePeriod { get; set; }
        public DateTime PeriodDetailInitId { get; set; }
        public DateTime PeriodDetailEndId { get; set; }

    }

    public class MonthlyBalanceProcessMessageDto
    { 
        public int id_company { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public DateTime dateToProcess { get; set; }
        public string idProcess { get; set; }
        public int idUser { get; set; }
        public bool isMassive { get; set; }
        public int? idWarehouse { get; set; }
        public int? idWarehouseLocation { get; set; }
        public int? idItem { get; set; }
        public string codigoTipoPeriodo { get; set; }
        public string descripcionTipoPeriodo { get; set; }
    }
    public class ComboboxGeneral
    {
        public string code {  get; set; }
        public string description { get; set; }
    }
    public class MonthlyBalanceProcess
    {
        public string code { get; set; }
        public string description { get; set; }
        public string state { get; set; }
        public string descState { get; set; }
    }
    public class MonthlyBalanceProcessFilterDto
    {
        public int idCompany { get; set; }
        public int idUser { get; set; }
        public string codePeriod { get; set; }
        public bool? isMassive { get; set; }
        public int? idWarehouse { get; set; }
        public int? idWarehouseLocation { get; set; }
        public int? idItem { get; set; }
        
    }
    public class MontlyBalanceExcelDto
    {
        [Description("Año")]
        public int Anio { get; set; }
        [Description("Periodo")]
        public int Periodo { get; set; }
        [Description("Código Produto")]
        public string CodigoProducto { get; set; }
        public string Producto { get; set; }
        public string Bodega { get; set; }
        [Description("Ubicación")]
        public string Ubicacion { get; set; }
        [Description("Secuencial de Lote")]
        public string SequencialLote { get; set; }
        [Description("Número de Lote")]
        public string NumeroLote { get; set; }
        public string Presentacion { get; set; }
        [Description("Unidad de Medida")]
        public string UnidadDeMedida { get; set; }
        [Description("Saldo Anterior")]
        public Nullable<decimal> SaldoAnterior { get; set; }
        [Description("Entrada")]
        public Nullable<decimal> Entrada { get; set; }
        [Description("Salida")]
        public Nullable<decimal> Salida { get; set; }
        [Description("Saldo Actual")]
        public Nullable<decimal> SaldoActual { get; set; }
        [Description("Libras Saldo Anterior")]
        public Nullable<decimal> LbSaldoAnterior { get; set; }
        [Description("Libras Entrada")]
        public Nullable<decimal> LbEntrada { get; set; }
        [Description("Libras Salida")]
        public Nullable<decimal> LbSalida { get; set; }
        [Description("Libras Saldo Actual")]
        public Nullable<decimal> LbSaldoActual { get; set; }
    }
}