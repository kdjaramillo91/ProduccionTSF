using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultSalesQuotationExterior
    {
        public int id { get; set; }
        public string NombreCia { get; set; }
        public string TelefonoCia { get; set; }
        public string email { get; set; }
        public string PlantCode { get; set; }
        public string Web { get; set; }
        public string DirSucural { get; set; }
        public string RucCia { get; set; }
        public string DireccionCia { get; set; }
        public string Factura { get; set; }
        public DateTime FechaEmisión { get; set; }
        public string Descripción { get; set; }
        public string Referencia { get; set; }
        public string TerminoNegociación { get; set; }
        public string OrdenPedido { get; set; }
        public string RazonSocialSoldTo { get; set; }
        public string USCISoldTo { get; set; }
        public string AddressSoldTo1 { get; set; }
        public string AddressSoldTo2 { get; set; }
        public string CountrySoldTo { get; set; }
        public string CitySoldTo { get; set; }
        public string Telefono1SoldTo { get; set; }
        public string Telefono2SoldTo { get; set; }
        public string EmailSoldTo { get; set; }
        public string RazonSocialShipTo { get; set; }
        public string USCIShipTo { get; set; }
        public string packingdetail { get; set; }
        public string AddressShipTo1 { get; set; }
        public string AddressShipTo2 { get; set; }
        public string CountryShipTo { get; set; }
        public string CityShipTo { get; set; }
        public string Telefono1ShipTo { get; set; }
        public string Telefono2ShipTo { get; set; }
        public string EmailShipTo { get; set; }
        public string PuertoDestino { get; set; }
        public string portDischarge { get; set; }
        public string Shipper { get; set; }
        public string PuertoDeEmbaruqe { get; set; }
        public string FechaEmbarque { get; set; }
        public string PurchaseOrder { get; set; }
        public int? Cartones { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Cantidad_kg { get; set; }
        public decimal? Cantidad_lb { get; set; }
        public decimal? Precio { get; set; }
        public decimal? gastoDistribuido { get; set; }
        public string size { get; set; }
        public string size2 { get; set; }
        public string Total { get; set; }
        public string Estado { get; set; }
        public decimal? precioFob { get; set; }
        public decimal? valuetotalCIF { get; set; }
        public string Vendedor { get; set; }
        public string Contacto { get; set; }
        public string EmailContacto { get; set; }
        public string PlazoPago { get; set; }
        public string CodigoBanco { get; set; }
        public string NombreBanco { get; set; }
        public string PaisBanco { get; set; }
        public string DireccionBanco { get; set; }
        public string MonedaBanco { get; set; }
        public int EnrutamientoBanco { get; set; }
        public string CuentaBanco { get; set; }
        public string NombreCompaniaBanco { get; set; }
        public string CodigoBancoIntermediario { get; set; }
        public string NombreBancoIntermediario { get; set; }
        public string CuentaBancoIntermediario { get; set; }
        public string MonedaBancoIntermediario { get; set; }
        public string PaisBancoIntermediario { get; set; }
        public string Product { get; set; }
        public string ColourGrade { get; set; }
        public string PackingDetails { get; set; }
        public string ContainerDetails { get; set; }
        public string ShipmentDate { get; set; }
        public decimal NetoKilos { get; set; }
        public decimal NetoLibras { get; set; }
        public decimal BrutoKilos { get; set; }
        public decimal BrutoLibras { get; set; }
        public decimal GlaseoKilos { get; set; }
        public decimal GlaseoLibras { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaActual { get; set; }
        public string Usuario { get; set; }
        public string CodigoPlanta { get; set; }
        public string FDA { get; set; }
        public string NumeroContenedores { get; set; }
        public decimal ValorAbonado { get; set; }
        public string contacto_2 { get; set; }
        public string descripcion { get; set; }
        public string transport { get; set; }
        public int? MU { get; set; }
        public string METODOPAGO { get; set; }
        public int? ID_METODOPAGO { get; set; }
        public string BankTransferInfo { get; set; }
        public string Notificador { get; set; }
        public string direccionNotif { get; set; }
        public string cellPhoneNumberPerson { get; set; }
        public string EspecialCondition { get; set; }
        public string USCINotif { get; set; }
        public string leyenda { get; set; }
        public string vessel { get; set; }
        public int? id_portTerminal { get; set; }
        public int? id_portDestination { get; set; }
        public int? id_portDischarge { get; set; }
        public int? id_portShipment { get; set; }
        public string Countrynotif { get; set; }
        public string Citynotif { get; set; }
        public string Telefono1notif { get; set; }
        public int? IdClienteexterior { get; set; }
        public int? id_consignee { get; set; }
        public decimal fiorital_weightwithglazing { get; set; }
        public decimal fiorital_total { get; set; }
        public decimal fiorital_totalweight { get; set; }
        public string umSingular { get; set; }

    }
}