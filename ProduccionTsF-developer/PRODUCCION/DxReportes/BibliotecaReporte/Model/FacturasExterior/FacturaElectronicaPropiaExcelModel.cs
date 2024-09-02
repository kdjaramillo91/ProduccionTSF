using System;

namespace BibliotecaReporte.Model.FacturasExterior
{
   internal  class FacturaElectronicaPropiaExcelModel
    {
        public string NombreCia { get; set; }
        public string TelefONoCia { get; set; }
        public string Email { get; set; }

        public string Web { get; set; }
        public string DirSucural { get; set; }
        public string RucCia { get; set; }
        public byte[] Logo { get; set; }
        public string Factura { get; set; }
        public DateTime FechaEmisiON { get; set; }
        public string Observacion { get; set; }
        public string TerminONegociacion { get; set; }
        public string OrdenPedido { get; set; }
        public string RazONSocialSoldTo { get; set; }
        public string USCISoldto { get; set; }
        public string AddressSoldTo1 { get; set; }
        public string RazONSocialShipTo { get; set; }
        public string USCIShipTo { get; set; }
        public string AddressShipTo1 { get; set; }
        public string Contacto { get; set; }
        public string PuertoDestino { get; set; }
        public string PuertoDeEmbaruqe { get; set; }

        public string FechaEmbarque { get; set; }
        public string Buque { get; set; }
        public string VIAJE { get; set; }
        public string Naviera { get; set; }
        public int CartONes { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioProforma { get; set; }
        public string Size { get; set; }
        public string Marca { get; set; }
        public string DescripciON { get; set; }
        public string Total { get; set; }
        public string CodigoBanco { get; set; }
        public string NombreBanco { get; set; }
        public string PaisBanco { get; set; }
        public string DireccionBanco { get; set; }
        public string MonedaBanco { get; set; }
        public string CuentaBanco { get; set; }
        public string NombreCompaniaBanco { get; set; }
        public string CodigoBancoIntermediario { get; set; }

        public string NombreBancoIntermediario { get; set; }
        public string CuentaBancoIntermediario { get; set; }
        public string MonedaBancoIntermediario { get; set; }
        public string PaisBancoIntermediario { get; set; }
        public string NumeroContenedores { get; set; }
        public string FechaProforma { get; set; }
        public string Proforma { get; set; }
        public string Dae { get; set; }
        public string PlazoPago { get; set; }
        public DateTime FechaDae { get; set; }
        public DateTime FechaCarga { get; set; }
        public decimal NetoKilos { get; set; }
        public decimal NetoLibras { get; set; }
        public decimal BrutoKilos { get; set; }
        public decimal BrutoLibras { get; set; }
        public byte[] Logo2 { get; set; }
        public string Contenedores { get; set; }
        public string Casepack { get; set; }
        public int EnrutamientoBanco { get; set; }
        public string CitySoldTo { get; set; }
        public string CountrySoldTo { get; set; }
        public string Telefono1SoldTo { get; set; }
        public string CityShipTo { get; set; }
        public string CountryShipTo { get; set; }
        public string Telefono1ShipTo { get; set; }
    }
}
