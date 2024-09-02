namespace BibliotecaReporte.Model.Logistica
{
    internal class GuiaRemisionPersModel
    {
        public string NumeroDocumento { get; set; }
        public int DiaEmision { get; set; }
        public int MesEmision { get; set; }
        public int AnioEmision { get; set; }
        public string ProveedorCompleto { get; set; }
        public string SitioCompleto { get; set; }
        public string NombreZona { get; set; }
        public string NombreConductor { get; set; }
        public string IdConductor { get; set; }
        public string NombreColor { get; set; }
        public string PlacaVehiculo { get; set; }
        public string SelloSalida { get; set; }
        public string SelloEntrada { get; set; }
        public string NombreSeguridad { get; set; }
        public string NombreBiologo { get; set; }
        public string DescripcionDocumento { get; set; }
        public int LibrasProgramadas { get; set; }
        public string TipoProceso { get; set; }
        public string ClaveAcceso { get; set; }
        public string ProveedorAmparante { get; set; }
        public string DescripcionTransporte { get; set; }
        public string INP { get; set; }
        public string FechaDespacho { get; set; }
        public string HoraDespacho { get; set; }
        public string Comprador { get; set; }
        public string ProcesoPlanta { get; set; }
        public string ListaPrecio { get; set; }
        public string Camaronera { get; set; }
    }
}