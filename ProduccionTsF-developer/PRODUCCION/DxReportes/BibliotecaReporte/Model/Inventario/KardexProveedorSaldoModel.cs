using System;
namespace BibliotecaReporte.Model.Inventario
{
    internal class KardexProveedorSaldoModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string NombreBodega { get; set; }
        public string NombreUbicacion { get; set; }
        public string NombreProducto { get; set; }
        public string NombreMotivoInventario { get; set; }
        public decimal MontoEntrada { get; set; }
        public decimal MontoSalida { get; set; }
        public string NameCompania { get; set; }
        public string NameDivision { get; set; }
        public string NameBranchOffice { get; set; }        

    }
}
