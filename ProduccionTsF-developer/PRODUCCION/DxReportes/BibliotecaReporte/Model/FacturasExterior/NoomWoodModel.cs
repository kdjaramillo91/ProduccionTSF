using System;

namespace BibliotecaReporte.Model.FacturasExterior
{
    internal class NoomWoodModel
    {
        public int ID { get; set; }
        public string Contenedores { get; set; }
        public DateTime FechaEmisiON { get; set; }
        public int TotalCartones { get; set; }
        public string Buyer { get; set; }
        public string PuertoDeEmbaruqe { get; set; }

        public DateTime FechaEmbarque { get; set; }

        public string PaisDestino { get; set; }

        public decimal GlaseoKilos { get; set; }

        public string Marcas { get; set; }

        public string ProductProforma { get; set; }

        public DateTime FechaETD { get; set; }

        public decimal PesoNeto { get; set; }

        public byte[] Logo { get; set; }

        public byte[] Logo2 { get; set; }
    }
}