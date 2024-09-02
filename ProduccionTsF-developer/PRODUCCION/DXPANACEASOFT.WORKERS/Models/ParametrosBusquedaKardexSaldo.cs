namespace DXPANACEASOFT.WORKERS.Models
{
    public class ParametrosBusquedaKardexSaldo
    {
        #region Atributos
        private int? _id_documentType;
        private string _number;
        private string _reference;
        private DateTime? _startEmissionDate;
        private DateTime? _endEmissionDate;
        private int? _idNatureMove;
        private int? _id_inventoryReason;
        private int? _id_warehouseExit;
        private int? _id_warehouseLocationExit;
        private int? _id_dispatcher;
        private int? _id_warehouseEntry;
        private int? _id_warehouseLocationEntry;
        private int? _id_receiver;
        private string _numberLot;
        private string _internalNumberLot;
        private string _lotMarked;
        private string _items;
        private int? _id_user;
        private string _codeReport;
        #endregion

        #region Propiedades
        public int? id_documentType { get => _id_documentType; set => _id_documentType = value; }
        public string number { get => _number; set => _number = value; }
        public string reference { get => _reference; set => _reference = value; }
        public DateTime? startEmissionDate { get => _startEmissionDate; set => _startEmissionDate = value; }
        public DateTime? endEmissionDate { get => _endEmissionDate; set => _endEmissionDate = value; }
        public int? idNatureMove { get => _idNatureMove; set => _idNatureMove = value; }
        public int? id_inventoryReason { get => _id_inventoryReason; set => _id_inventoryReason = value; }
        public int? id_warehouseExit { get => _id_warehouseExit; set => _id_warehouseExit = value; }
        public int? id_warehouseLocationExit { get => _id_warehouseLocationExit; set => _id_warehouseLocationExit = value; }
        public int? id_dispatcher { get => _id_dispatcher; set => _id_dispatcher = value; }
        public int? id_warehouseEntry { get => _id_warehouseEntry; set => _id_warehouseEntry = value; }
        public int? id_warehouseLocationEntry { get => _id_warehouseLocationEntry; set => _id_warehouseLocationEntry = value; }
        public int? id_receiver { get => _id_receiver; set => _id_receiver = value; }
        public string numberLot { get => _numberLot; set => _numberLot = value; }
        public string internalNumberLot { get => _internalNumberLot; set => _internalNumberLot = value; }
        public string lotMarked { get => _lotMarked; set => _lotMarked = value; }
        public string items { get => _items; set => _items = value; }
        public int? id_user { get => _id_user; set => _id_user = value; }
        public string codeReport { get => _codeReport; set => _codeReport = value; }
        #endregion

        #region Constructor
        public ParametrosBusquedaKardexSaldo()
        {
            _id_documentType = 0;
            _number = "Todos";
            _reference = "Todas";
            _startEmissionDate = null;
            _endEmissionDate = null;
            _idNatureMove = 0;
            _id_inventoryReason = 0;
            _id_warehouseExit = 0;
            _id_warehouseLocationExit = 0;
            _id_dispatcher = 0;
            _id_warehouseEntry = 0;
            _id_warehouseLocationEntry = 0;
            _id_receiver = 0;
            _numberLot = "Todos";
            _internalNumberLot = "Todos";
            _lotMarked = "Todos";
            _items = "Todos";
            _id_user = 0;
            _codeReport = "SinCodigo";
        }

        #endregion
    }
}
