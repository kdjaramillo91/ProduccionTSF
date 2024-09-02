using System;
namespace BibliotecaReporte.Model.Inventario
{
    internal class MovimientoLoteModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string NumeroDocumentoInventario { get; set; }
        public int IdBodega { get; set; }
        public string NombreBodega { get; set; }
        public int IdUbicacion { get; set; }
        public string NombreUbicacion { get; set; }
        public int IdProducto { get; set; }
        public DateTime FechaEmison { get; set; }
        public string NombreMotivoInventario { get; set; }
        public string NombreUnidadMedida { get; set; }
        public decimal MontoEntrada { get; set; }
        public decimal MontoSalida { get; set; }
        public decimal Balance { get; set; }
        public decimal PreviousBalance { get; set; }
        public string NameCompania { get; set; }
        public string NameDivision { get; set; }
        public string NameBranchOffice { get; set; }
        public string NumberRemissionGuide { get; set; }
        public string NombreProducto { get; set; }
        public string NumberLot { get; set; }
        public int IdMotivoInventario { get; set; }

    }
}
