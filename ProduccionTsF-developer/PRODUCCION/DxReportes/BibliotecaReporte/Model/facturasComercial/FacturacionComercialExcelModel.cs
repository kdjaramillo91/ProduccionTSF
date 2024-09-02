using System;


namespace BibliotecaReporte.Model.facturasComercial
{
    internal class FacturacionComercialExcelModel
    {
        public string BuquemasViaje { get; set; }
        public string Shipping_Line { get; set; }
        public string Documento { get; set; }
        public string InvComm_orden_pedido { get; set; }
        public string InvComm_fecha_embarque { get; set; }
        public string InvComm_numero_bl { get; set; }
        public decimal InvCommDet_cantidad { get; set; }
        public int InvCommDet_Cantidad_Cajas { get; set; }
        public decimal InvCommDet_Valor_Total { get; set; }
        public string TermsNegotiation_code { get; set; }
        public string Document { get; set; }
        public DateTime EmissionDateformat { get; set; }
        public string Fullname_businessName { get; set; }
        public string Good { get; set; }
        public string Portofdeparture { get; set; }
        public string Portofdestination { get; set; }
        public string Numcontendor { get; set; }
        public string Direccion { get; set; }
        public string Descripformapago { get; set; }
        public string Letras { get; set; }
        public string Plazo { get; set; }
        public string BankTransferInfo { get; set; }
        public decimal Descuento { get; set; }
        public string Item_Origen { get; set; }
        public string Talla_Origen { get; set; }
        public string Notifier { get; set; }
        public string Direccionnotif { get; set; }
        public string TelefonoNotif { get; set; }
        public string Telefonoconsig { get; set; }
        public decimal InvCommDet_precio_unitario { get; set; }
        public byte[] Logo { get; set; }
        public string Trademark_Company { get; set; }        
    }
}
