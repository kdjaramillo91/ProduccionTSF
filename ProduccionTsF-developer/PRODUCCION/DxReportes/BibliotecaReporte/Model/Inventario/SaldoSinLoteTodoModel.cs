using System;
namespace BibliotecaReporte.Model.Inventario
{
    internal class SaldoSinLoteTodoModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int IdBodega { get; set; }
        public string NombreBodega { get; set; }
        public int IdUbicacion { get; set; }
        public string NombreUbicacion { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string NombreUnidadMedida { get; set; }
        public decimal MontoEntrada { get; set; }
        public decimal MontoSalida { get; set; }
        public string NameCompania { get; set; }
        public string NameDivision { get; set; }
        public string NameBranchOffice { get; set; }
        public string ItemType { get; set; }
        public string ItemMetricUnit { get; set; }
        public decimal ItemPresentationValue { get; set; }    

    }
}
