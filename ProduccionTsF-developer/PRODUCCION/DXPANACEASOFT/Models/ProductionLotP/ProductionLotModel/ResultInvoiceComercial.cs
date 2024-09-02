using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultInvoiceComercial
    {
        public DateTime emissionDateformat { get; set; }
        public string documento { get; set; }
        public string InvComm_orden_pedido { get; set; }
        public string fullname_businessName { get; set; }
        public string USCI { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string notificador { get; set; }
        public string direccionnotif { get; set; }
        public string TelefonoNotif { get; set; }
        public string consignador { get; set; }
        public string Telefonoconsig { get; set; }
        public string USCIconsig { get; set; }
        public string direccconsigna { get; set; }
        public string BuquemasViaje { get; set; }
        public string InvComm_fecha_embarque { get; set; }
        public string Portofdeparture { get; set; }
        public string Portofdestination { get; set; }
        public string Shipping_Line { get; set; }
        public string InvComm_numero_bl { get; set; }
        public string numcontenedor { get; set; }
        public string descripformapago { get; set; }
        public string plazo { get; set; }
        public string Good { get; set; }
        public string Item_Origen { get; set; }
        public string Talla_Origen { get; set; }
        public int? InvCommDet_cantidad_cajas { get; set; }
        public decimal InvCommDet_cantidad { get; set; }
        public decimal InvCommDet_precio_unitario { get; set; }
        public decimal InvCommDet_valor_total { get; set; }
        public string BankTransferInfo { get; set; }
        public string letras { get; set; }


    }
}