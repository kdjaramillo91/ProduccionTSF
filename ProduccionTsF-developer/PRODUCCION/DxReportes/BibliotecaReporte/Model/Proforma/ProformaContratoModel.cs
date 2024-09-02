using System;
namespace BibliotecaReporte.Model.Proforma
{
   internal  class ProformaContratoModel
    {
        public string NombreCiaLargo { get; set; }
        public string DirSucural { get; set; }
        public DateTime FechaEmision { get; set; }
        public string RazonSocialSoldTo { get; set; }
        public string AddressSoldTo1 { get; set; }
        public string PuertoDestino { get; set; }
        public int Cartones { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string CasePackDimension { get; set; }
        public string Marca { get; set; }
        public string DescripcionGeneral { get; set; }
        public string NombreBanco { get; set; }
        public string DireccionBanco { get; set; }
        public int EnrutamientoBanco { get; set; }
        public string CuentaBanco { get; set; }
        public string NombreCompaniaBanco { get; set; }
        public decimal NetoKilos { get; set; }
        public decimal BrutoKilos { get; set; }
        public string CantidadContenedores { get; set; }
        public string DescripcionCliente { get; set; }
        public string Nproforma { get; set; }
        public string Categoria2 { get; set; }
        public string TallaMarcada { get; set; }
        public string ValorTotalTermPago { get; set; }
        public string ValorTotalTermPagoString { get; set; }
        public decimal Balance { get; set; }
        public string BalanceString { get; set; }
        public byte[] Logo { get; set; }

    }
}
