using System;
namespace BibliotecaReporte.Model.ControlDeCalidad
{
    internal class AnalisisDeCalidadModel
    {
        public string TipodeAnalisis { get; set; }
        public string NAnalisis { get; set; }
        public string SecTransaccional { get; set; }
        public string NLote { get; set; }
        public DateTime FechaHoradeLlegada { get; set; }
        public string Proveedor { get; set; }
        public string Camaronera { get; set; }
        public string NPiscina { get; set; }
        public string INP { get; set; }
        public string NAcuerdoMinisterial { get; set; }
        public string NTramite { get; set; }
        public string NGuiaRemision { get; set; }
        public string Producto { get; set; }
        public string TipoProceso { get; set; }
        public int NGavetas { get; set; }
        public decimal LibrasProgramadas { get; set; }
        public decimal LibrasRemitidas { get; set; }
        public decimal LibrasRecibidas { get; set; }
        public DateTime FechadeAnalisis { get; set; }
        public string HoradeAnalisis { get; set; }
        public string ResidualS02 { get; set; }
        public decimal Temperatura { get; set; }
        public decimal Gramaje { get; set; }
        public string OlorCrudo { get; set; }
        public string SaborCabeza { get; set; }
        public string OlorCocido { get; set; }
        public string SaborCola { get; set; }
        public string Color { get; set; }
        public string Flacidez { get; set; }
        public string Mudado { get; set; }
        public string DeshidratadoLeve { get; set; }
        public string DeshidratadoModer { get; set; }
        public string CabezaFloja { get; set; }
        public string CabezaRoja { get; set; }
        public string CabezaAnaranjada { get; set; }
        public string HepatoRojo { get; set; }
        public string BranquiasSucias { get; set; }
        public string PicadoLeve { get; set; }
        public string PicadoFuerte { get; set; }
        public string Quebrado { get; set; }
        public string Melanosis { get; set; }
        public string Rosado { get; set; }
        public string Semirosado { get; set; }
        public string Corbata { get; set; }
        public string Juvenil { get; set; }
        public string Otros { get; set; }
        public int TotalDefectos { get; set; }
        public decimal RendimientoEntero { get; set; }
        public string PCaliforniense { get; set; }
        public string PStylirrostris { get; set; }
        public string POccidental { get; set; }
        public string MaterialExtrano { get; set; }
        public int TotalOtrasEspecies { get; set; }
        public string AccionesCorrectivas { get; set; }
        public string Referencia { get; set; }
        public string Analista { get; set; }
        public string EsConforme { get; set; }
        public string Observacion { get; set; }
        public string Proceso { get; set; }
        public string Estado { get; set; }
        public int Idcompany { get; set; }       
        public decimal TotalMuestra { get; set; }       
        public string Especie { get; set; }       
        public decimal TotalPiezas { get; set; }       
        public string ContaminacionHielo { get; set; }       
        public string Talla { get; set; }       
        public string CondicionTransporte { get; set; }       
    }
}
