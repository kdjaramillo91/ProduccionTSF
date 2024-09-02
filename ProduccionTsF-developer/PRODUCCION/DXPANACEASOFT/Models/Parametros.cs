using System;

namespace DXPANACEASOFT.Models
{
	public class Parametros
	{
		#region Parametros de búsqueda
		/// <summary>
		/// Clase que tiene como atributos los parámetros de búsqueda de Kardex-Saldo.
		/// </summary>
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
            private string _movAnulados;
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
            public string movAnulados { get => _movAnulados; set => _movAnulados = value; }
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
                _movAnulados = "N";
			}

			#endregion
		}

		/// <summary>
		/// Clase que tiene como atributos los parámetros de búsqueda de Kardex-Saldo.
		/// </summary>
		public class ParametrosBusquedaInventoryMove
		{
			#region Atributos
			private int? _id_documentType;
			private int? _id_documentState;
			private string _codeInventoryReasonIPXM;
			private string _number;
			private string _reference;
			private DateTime? _startEmissionDate;
			private DateTime? _endEmissionDate;
			private DateTime? _startAuthorizationDate;
			private DateTime? _endAuthorizationDate;
			private string _accessKey;
			private string _authorizationNumber;
			//private int? _idNatureMove;
			private int? _id_receiver;
			private int? _id_dispatcher;
			private int? _id_inventoryReason;
			private int? _id_warehouseEntry;
			private int? _id_warehouseLocationEntry;
			private int? _id_warehouseExit;
			private int? _id_warehouseLocationExit;
			private int? _id_user;
			#endregion

			#region Propiedades
			public int? id_documentType { get => _id_documentType; set => _id_documentType = value; }
			public int? id_documentState { get => _id_documentState; set => _id_documentState = value; }
			public string codeInventoryReasonIPXM { get => _codeInventoryReasonIPXM; set => _codeInventoryReasonIPXM = value; }
			public string number { get => _number; set => _number = value; }
			public string reference { get => _reference; set => _reference = value; }
			public DateTime? startEmissionDate { get => _startEmissionDate; set => _startEmissionDate = value; }
			public DateTime? endEmissionDate { get => _endEmissionDate; set => _endEmissionDate = value; }
			public DateTime? startAuthorizationDate { get => _startAuthorizationDate; set => _startAuthorizationDate = value; }
			public DateTime? endAuthorizationDate { get => _endAuthorizationDate; set => _endAuthorizationDate = value; }
			public string accessKey { get => _accessKey; set => _accessKey = value; }
			public string authorizationNumber { get => _authorizationNumber; set => _authorizationNumber = value; }
			//public int? idNatureMove { get => _idNatureMove; set => _idNatureMove = value; }
			public int? id_receiver { get => _id_receiver; set => _id_receiver = value; }
			public int? id_dispatcher { get => _id_dispatcher; set => _id_dispatcher = value; }
			public int? id_inventoryReason { get => _id_inventoryReason; set => _id_inventoryReason = value; }
			public int? id_warehouseEntry { get => _id_warehouseEntry; set => _id_warehouseEntry = value; }
			public int? id_warehouseLocationEntry { get => _id_warehouseLocationEntry; set => _id_warehouseLocationEntry = value; }
			public int? id_warehouseExit { get => _id_warehouseExit; set => _id_warehouseExit = value; }
			public int? id_warehouseLocationExit { get => _id_warehouseLocationExit; set => _id_warehouseLocationExit = value; }
			public int? id_user { get => _id_user; set => _id_user = value; }
			#endregion

			#region Constructor
			public ParametrosBusquedaInventoryMove()
			{
				_id_documentType = 0;
				_id_documentState = 0;
				_codeInventoryReasonIPXM = "Todos";
				_number = "Todos";
				_reference = "Todas";
				_startEmissionDate = null;
				_endEmissionDate = null;
				_startAuthorizationDate = null;
				_endAuthorizationDate = null;
				_accessKey = "Todos";
				_authorizationNumber = "Todos";
				_id_receiver = 0;
				_id_dispatcher = 0;
				//_idNatureMove = 0;
				_id_inventoryReason = 0;
				_id_warehouseEntry = 0;
				_id_warehouseLocationEntry = 0;
				_id_warehouseExit = 0;
				_id_warehouseLocationExit = 0;
				_id_user = 0;
			}

			#endregion
		}

        /// <summary>
		/// Clase que tiene como atributos los parámetros de búsqueda de QualityControl.
		/// </summary>
		public class ParametrosBusquedaQualityControl
        {
            #region Atributos
            private int? _id_documentState;
            private string _qualityControlNumber;
            private string _documentReference;
            private DateTime? _startEmissionDate;
            private DateTime? _endEmissionDate;
            private int? _id_qualityControlConfiguration;
            private int? _id_analyst;
            private string _conforms;
            #endregion

            #region Propiedades
            public int? id_documentState { get => _id_documentState; set => _id_documentState = value; }
            public string qualityControlNumber { get => _qualityControlNumber; set => _qualityControlNumber = value; }
            public string documentReference { get => _documentReference; set => _documentReference = value; }
            public DateTime? startEmissionDate { get => _startEmissionDate; set => _startEmissionDate = value; }
            public DateTime? endEmissionDate { get => _endEmissionDate; set => _endEmissionDate = value; }
            public int? id_qualityControlConfiguration { get => _id_qualityControlConfiguration; set => _id_qualityControlConfiguration = value; }
            public int? id_analyst { get => _id_analyst; set => _id_analyst = value; }
            public string conforms { get => _conforms; set => _conforms = value; }

            #endregion

            #region Constructor
            public ParametrosBusquedaQualityControl()
            {
                _id_documentState = 0;
                _qualityControlNumber = "Todos";
                _documentReference = "Todos";
                _startEmissionDate = null;
                _endEmissionDate = null;
                _id_qualityControlConfiguration = 0;
                _id_analyst = 0;
                _conforms = "Todos";
            }

            #endregion
        }

        /// <summary>
		/// Clase que tiene como atributos los parámetros de búsqueda en Mastered.
		/// </summary>
		public class ParametrosBusquedaMastered
        {
            #region Atributos
            private int? _id_warehouse;
            private int? _id_warehouseLocation;
            private int? _id_warehouseType;
            private string _emissionDate;
            private short _for_lot;
            #endregion

            #region Propiedades
            public int? id_warehouse { get => _id_warehouse; set => _id_warehouse = value; }
            public int? id_warehouseLocation { get => _id_warehouseLocation; set => _id_warehouseLocation = value; }
            public int? id_warehouseType { get => _id_warehouseType; set => _id_warehouseType = value; }
            public string emissionDate { get => _emissionDate; set => _emissionDate = value; }
            public short for_lot { get => _for_lot; set => _for_lot = value; }


            #endregion

            #region Constructor
            public ParametrosBusquedaMastered()
            {
                _id_warehouse = 0;
                _id_warehouseLocation = 0;
                _id_warehouseType = 0;
                _for_lot = 0;
            }

            #endregion
        }

        /// <summary>
		/// Clase que tiene como atributos los parámetros de búsqueda en OpeningClosingPlateLying.
		/// </summary>
		public class ParametrosBusquedaOpeningClosingPlateLying
        {
            #region Atributos
            private int? _id_warehouse;
            private int? _id_openingClosingPlateLyingDto;
            private int? _id_warehouseType;
            private short _for_lot;
            #endregion

            #region Propiedades
            public int? id_warehouse { get => _id_warehouse; set => _id_warehouse = value; }
            public int? id_openingClosingPlateLyingDto { get => _id_openingClosingPlateLyingDto; set => _id_openingClosingPlateLyingDto = value; }
            public int? id_warehouseType { get => _id_warehouseType; set => _id_warehouseType = value; }
            public short for_lot { get => _for_lot; set => _for_lot = value; }


            #endregion

            #region Constructor
            public ParametrosBusquedaOpeningClosingPlateLying()
            {
                _id_warehouse = 0;
                _id_openingClosingPlateLyingDto = 0;
                _id_warehouseType = 0;
                _for_lot = 0;
            }

            #endregion
        }

        /// <summary>
		/// Clase que tiene como atributos los parámetros de búsqueda para consultar las Transferencia en plantas realizadas.
		/// </summary>
		public class ParametrosBusquedaInventoryMovePlantTransfer
        {
            #region Atributos
            private int? _id_machineForProd;
            private string _number;
            private string _reference;
            private DateTime? _startEmissionDate;
            private DateTime? _endEmissionDate;
            private int? _id_state;
            private int? _id_productionCart;
            private int? _id_processType;
            private int? _id_provider;
            private int? _id_warehouseEntry;
            private int? _id_warehouseLocationEntry;
            private int? _id_receiver;
            private string _numberLot;
            private string _secTransaction;
            private int? _id_turn;
            private int? _id_inventoryReason;

            #endregion

            #region Propiedades
            public int? id_machineForProd { get => _id_machineForProd; set => _id_machineForProd = value; }
            public string number { get => _number; set => _number = value; }
            public string reference { get => _reference; set => _reference = value; }
            public DateTime? startEmissionDate { get => _startEmissionDate; set => _startEmissionDate = value; }
            public DateTime? endEmissionDate { get => _endEmissionDate; set => _endEmissionDate = value; }
            public int? id_state { get => _id_state; set => _id_state = value; }
            public int? id_productionCart { get => _id_productionCart; set => _id_productionCart = value; }
            public int? id_processType { get => _id_processType; set => _id_processType = value; }
            public int? id_provider { get => _id_provider; set => _id_provider = value; }
            public int? id_warehouseEntry { get => _id_warehouseEntry; set => _id_warehouseEntry = value; }
            public int? id_warehouseLocationEntry { get => _id_warehouseLocationEntry; set => _id_warehouseLocationEntry = value; }
            public int? id_receiver { get => _id_receiver; set => _id_receiver = value; }
            public string numberLot { get => _numberLot; set => _numberLot = value; }
            public string secTransaction { get => _secTransaction; set => _secTransaction = value; }
            public int? id_turn { get => _id_turn; set => _id_turn = value; }
            public int? id_inventoryReason { get => _id_inventoryReason; set => _id_inventoryReason = value; }
            
            #endregion

            #region Constructor
            public ParametrosBusquedaInventoryMovePlantTransfer()
            {
                _id_machineForProd = 0;
                _number = "";
                _reference = "";
                _startEmissionDate = null;
                _endEmissionDate = null;
                _id_state = 0;
                _id_productionCart = 0;
                _id_processType = 0;
                _id_provider = 0;
                _id_warehouseEntry = 0;
                _id_warehouseLocationEntry = 0;
                _id_receiver = 0;
                _numberLot = "";
                _secTransaction = "";
                _id_turn = 0;
                _id_inventoryReason = 0;


               
            }

            #endregion

            
        }

        /// <summary>
        /// Clase que tiene como atributos los parámetros de búsqueda de Internal Process Saldo.
        /// </summary>
        public class ParametrosBusquedaInternalProcessSaldo
        {
            #region Atributos
            private DateTime? _startEmissionDate;
            private DateTime? _endEmissionDate;
            private int? _id_warehouse;
            private int? _id_warehouseLocation;
            private int? _id_user;
            #endregion

            #region Propiedades
            public DateTime? startEmissionDate { get => _startEmissionDate; set => _startEmissionDate = value; }
            public DateTime? endEmissionDate { get => _endEmissionDate; set => _endEmissionDate = value; }
            public int? id_warehouse { get => _id_warehouse; set => _id_warehouse = value; }
            public int? id_warehouseLocation { get => _id_warehouseLocation; set => _id_warehouseLocation = value; }
            public int? id_user { get => _id_user; set => _id_user = value; }
            #endregion

            #region Constructor
            public ParametrosBusquedaInternalProcessSaldo()
            {
                _startEmissionDate = null;
                _endEmissionDate = null;
                _id_warehouse = 0;
                _id_warehouseLocation = 0;
                _id_user = 0;
            }

            #endregion
        }

        /// <summary>
        /// Clase que tiene como atributos los parámetros de búsqueda para consultar NonProductiveHourPending.
        /// </summary>
        public class ParametrosNonProductiveHourPending
        {
            #region Atributos
            private int? _idUsuario;
            #endregion

            #region Propiedades
            public int? idUsuario { get => _idUsuario; set => _idUsuario = value; }
            #endregion

            #region Constructor
            public ParametrosNonProductiveHourPending()
            {
                _idUsuario = 0;
            }

            #endregion
        }
        #endregion
    }
}