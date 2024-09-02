using System.Collections.Generic;
using DXPANACEASOFT.Controllers;
using System.Linq;
using System;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using DXPANACEASOFT.Operations;

namespace DXPANACEASOFT.Models.ProductionLotCloseDTO
{
    public static partial class Extensions
    {
        public static ProductionLotFinalize ToProductionLotFinalize(this Document document)
        {
            if (document == null)
            {
                return null;
            }

            return PrivateToProductionLotFinalize(document, false);
        }
        public static ProductionLotFinalize.Detallado ToProductionLotFinalizeDetalled(this Document document)
        {
            if (document == null)
            {
                return null;
            }

            return PrivateToProductionLotFinalize(document, true);
        }
        public static ProductionLotFinalize.Detallado[] ToProductionLotFinalize(this IEnumerable<Document> documentos)
        {
            if (documentos == null)
            {
                return null;
            }

            return documentos
                .Select(d => PrivateToProductionLotFinalize(d, false))
                .ToArray();
        }
        public static void SetDetailLot(this ProductionLotFinalize.Detallado finalize)
        {
            var lotes = PrepararDetalleLoteDTO(finalize.IdsLote);
            finalize.DetallesLotes = lotes;
            finalize.NumeroLote = lotes.FirstOrDefault().NumeroLote;
        }
        public static void SetDetailKardex(this ProductionLotFinalize.Detallado finalize)
        {
            var kardex = PrepararDetalleKardexDTO(finalize.FechaEmision, finalize.NumeroLote);
            finalize.DetallesKardex = kardex;
        }
		private static ProductionLotFinalize.Detallado PrivateToProductionLotFinalize(Document document, bool usarDetallado)
        {
            DBContext db = new DBContext();
            var numeroLote = document
				.ProductionLotClose
				.FirstOrDefault(e => e.isActive)?
				.number;

            var solicitante = "";
            var user = db.User.FirstOrDefault(fod => fod.id == document.id_userCreate);
            if(user != null)
            {
                solicitante = db.Person.FirstOrDefault(fod => fod.id == user.id_employee).fullname_businessName;
            }
            

            var modelo = new ProductionLotFinalize.Detallado()
			{
				Id = document.id,
				NumeroDocumento = document.number,
				DocumentType = document.DocumentType.name,
				FechaEmision = document.emissionDate,
				Referencia = document.reference,
				NumeroLote = numeroLote,
				Estado = document.DocumentState.name,
				Descripcion = document.description,
                Solicitante = solicitante,
            };



            if (usarDetallado)
            {
                var idsLote = document.ProductionLotClose.Select(a => a.id_lot).ToArray();
                var lotes = PrepararDetalleLoteDTO(idsLote);
                var kardexResult = PrepararDetalleKardexDTO(document.emissionDate, numeroLote);

                modelo.DetallesLotes = lotes;
                modelo.DetallesKardex = kardexResult;
            }

            return modelo;
        }
        private static ProductionLotFinalize.DetalleLote[] PrepararDetalleLoteDTO(int[] idsLote)
        {
            var lotes = new List<ProductionLotFinalize.DetalleLote>();
            var orden = 1;
            foreach (var idLote in idsLote)
            {
                var lotData = GetLotDataFromTempData(idLote);
                lotes.Add(new ProductionLotFinalize.DetalleLote()
                {
                    Orden = orden++,
                    NumeroLote = lotData.NumeroLote,
                    SecuenciaTransaccional = lotData.NumeroInterno,
                    Cantidad = lotData.CantidadMaster,
                    CantidadKilos = lotData.CantidadKilos,
                    CantidadLibras = lotData.CantidadLibras,
                });
            }
            return lotes.ToArray();
        }

        private static ResultKardex[] PrepararDetalleKardexDTO(
           DateTime fechaEmision, string internalNumberLot)
        {
            DBContext db = new DBContext();
            Parametros.ParametrosBusquedaKardexSaldo parametrosBusquedaKardexSaldo = new Parametros.ParametrosBusquedaKardexSaldo();
            parametrosBusquedaKardexSaldo.internalNumberLot = internalNumberLot.Substring(0,5);
            parametrosBusquedaKardexSaldo.endEmissionDate = fechaEmision;

            var parametrosBusquedaKardexSaldoAux = new SqlParameter();
            parametrosBusquedaKardexSaldoAux.ParameterName = "@ParametrosBusquedaKardexSaldo";
            parametrosBusquedaKardexSaldoAux.Direction = ParameterDirection.Input;
            parametrosBusquedaKardexSaldoAux.SqlDbType = SqlDbType.NVarChar;

            var jsonDateTimeSetting = new JsonSerializerSettings { 
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
            };
            var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaKardexSaldo, jsonDateTimeSetting);
            parametrosBusquedaKardexSaldoAux.Value = jsonAux;

            db.Database.CommandTimeout = 3600;
            return db.Database.SqlQuery<ResultKardex>
                    ("exec inv_Consultar_Kardex_Saldo_StoredProcedure @ParametrosBusquedaKardexSaldo ",
                    parametrosBusquedaKardexSaldoAux)
                .ToArray();
        }

        public class LotData
        {
            public int IdLote { get; set; }
            public string TipoLote { get; set; }
            public string NumeroLote { get; set; }
            public string NumeroInterno { get; set; }
            public decimal CantidadMaster { get; set; }
            public decimal CantidadLibras { get; set; }
            public decimal CantidadKilos { get; set; }
        }

        public static LotData GetLotDataFromTempData(int idLote)
        {
            DBContext db = new DBContext();
            LotData data = null;
            var productionLot = db.ProductionLot.FirstOrDefault(e => e.id == idLote);
            if (productionLot != null)
            {
                data = new LotData()
                {
                    IdLote = idLote,
                    TipoLote = "PRD",
                    NumeroLote = productionLot.internalNumber,
                    NumeroInterno = productionLot.number,
                    CantidadMaster = productionLot.ProductionLotLiquidation.Sum(s => s.quantity),
                    CantidadKilos = productionLot.ProductionLotLiquidation.Sum(s => s.quantityTotal.Value),
                    CantidadLibras = productionLot.ProductionLotLiquidation.Sum(s => s.quantityPoundsLiquidation.Value)
                };
            }
            else
            {
                var lot = db.Lot.FirstOrDefault(e => e.id == idLote);
                data = new LotData()
                {
                    IdLote = idLote,
                    TipoLote = "MAN",
                    NumeroLote = lot.internalNumber,
                    NumeroInterno = lot.number,
                    //CantidadMaster = productionLot.ProductionLotLiquidation.Sum(s => s.quantity),
                };
            }

            return data;
        }
    }
}