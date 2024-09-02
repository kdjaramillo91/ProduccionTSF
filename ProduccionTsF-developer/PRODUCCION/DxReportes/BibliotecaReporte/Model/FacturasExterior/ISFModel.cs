using System;

namespace BibliotecaReporte.Model.FacturasExterior
{
    internal class ISFModel
    {
        public int ID { get; set; }
        public string BL { get; set; }
        public string Buque { get; set; }
        public string VIAJE { get; set; }

        public string Direccion { get; set; }
        public string PurchaseOrder { get; set; }
        public string Naviera { get; set; }
        public string NOMBRECIA { get; set; }
        public string DIRECCIONCIA { get; set; }
        public string CIUDADPAIS { get; set; }
        public string CLIENTEEXTERIOR { get; set; }
        public string CONSIGNATARIO { get; set; }
        public string CONSIGNATARIODIRECCION { get; set; }
        public string CIUDADEMBARQUE { get; set; }
        public string PUERTODESCARGA2 { get; set; }
        public string Contenedores { get; set; }        
        public string FechaEmbarque { get; set; }
        public string Fechaeta { get; set; }
        public string FECHAISF { get; set; }
        public string FECHAETD2 { get; set; }

    }
}