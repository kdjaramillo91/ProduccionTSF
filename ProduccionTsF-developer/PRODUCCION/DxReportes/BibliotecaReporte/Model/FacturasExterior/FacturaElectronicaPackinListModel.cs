using System;

namespace BibliotecaReporte.Model.FacturasExterior
{
    internal class FacturaElectronicaPackinListModel
    {
        public int ID { get; set; }
        public string NombreCia { get; set; }
        public string TelefONoCia { get; set; }
        public string Email { get; set; }

        public string PlantCode { get; set; }
        public string FDA { get; set; }
        public string Web { get; set; }
        public string DirSucural { get; set; }
        public string RucCia { get; set; }
        public string Factura { get; set; }
        public DateTime FechaEmisiON { get; set; }
        public string TerminONegociacion { get; set; }
        public string OrdenPedido { get; set; }
        public string RazONSocialSoldTo { get; set; }
        public string USCISoldTo { get; set; }
        public string AddressSoldTo1 { get; set; }
        public string AddressSoldTo2 { get; set; }
        public string CountrySoldTo { get; set; }
        public string CitySoldTo { get; set; }
        public string TelefONo1SoldTo { get; set; }
        public string TelefONo2SoldTo { get; set; }
        public string RazONSocialShipTo { get; set; }
        public string USCIShipTo { get; set; }
        public string AddressShipTo1 { get; set; }
        public string AddressShipTo2 { get; set; }
        public string CountryShipTo { get; set; }
        public string CityShipTo { get; set; }
        public string TelefONo1ShipTo { get; set; }
        public string TelefONo2ShipTo { get; set; }
        public string CountryOfOrigin { get; set; }
        public string PuertoDestino { get; set; }
        public string PuertoDeEmbaruqe { get; set; }
        public string Shipper { get; set; }
        public string Buque { get; set; }
        public string VIAJE { get; set; }
        public string Naviera { get; set; }
        public string BL { get; set; }
        public string PurchASeORDER { get; set; }
        public string FechaEmbarque { get; set; }
        public int CartONes { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Size { get; set; }
        public string Certificacion { get; set; }
        public decimal PesoKG { get; set; }
        public decimal PesoLB { get; set; }
        public decimal PesoGlaseoKG { get; set; }
        public decimal PesoGlaseoLB { get; set; }
        public decimal GlaseoLIBRA { get; set; }
        public decimal GlaseoKG { get; set; }
        public string Marca { get; set; }
        public string DescripciON2 { get; set; }
        public string DescripciON { get; set; }
        public string Total { get; set; }
        public string NumeroContenedores { get; set; }
        public string Contenedores { get; set; }
        public string Casepack { get; set; }

        public DateTime FechaProforma { get; set; }
        public string Dae { get; set; }
        public DateTime FechaDae { get; set; }
        public DateTime FechaCarga { get; set; }
        public decimal NetoKilos { get; set; }
        public decimal NetoLibras { get; set; }
        public decimal BrutoKilos { get; set; }
        public decimal BrutoLibras { get; set; }
        public decimal GlaseoKilos { get; set; }
        public decimal GlaseoLibras { get; set; }
        public DateTime FechaETD { get; set; }
        public byte[] Logo { get; set; }
    }
}