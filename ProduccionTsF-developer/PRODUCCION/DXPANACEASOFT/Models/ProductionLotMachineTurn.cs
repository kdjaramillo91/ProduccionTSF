using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DXPANACEASOFT.Dapper;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace DXPANACEASOFT.Models
{
    public partial class ProductionLotMachineTurn
    {


        private static string INSERTPRODUCTLOTMACHINETURN = "INSERT INTO ProductionLotMachineTurn ( " +
                                                            " id, idMachineProdOpeningDetail, idMachineProdOpening, idTurn, " +
                                                            " idMachineForProd, timeInit, timeEnd, id_userCreate, "+
                                                            " dateCreate ) VALUES ( " +
                                                            " @id, @idMachineProdOpeningDetail, @idMachineProdOpening, @idTurn, " +
                                                            " @idMachineForProd, @timeInit, @timeEnd, @id_userCreate, " +
                                                            " @dateCreate ) ";

        private static string UPDATEPRODUCTLOTMACHINETURN = " UPDATE ProductionLotMachineTurn SET " +
                                                            " idMachineProdOpeningDetail=@idMachineProdOpeningDetail, idMachineProdOpening=@idMachineProdOpening, idTurn=@idTurn, " +
                                                            " idMachineForProd=@idMachineForProd, timeInit=@timeInit, timeEnd=@timeEnd, " +
                                                            " id_userUpdate=@id_userUpdate, dateUpdate=@dateUpdate " +
                                                            " WHERE id = @id";

        private static string SELECT_ONE_BY_PRODUCTIONLOT = "SELECT  P.id, idMachineProdOpeningDetail, idMachineProdOpening, idTurn,  idMachineForProd,\r\n" +
                                                            " P.timeInit, P.timeEnd,  \r\n" +
                                                            "id_userCreate,  dateCreate, id_userUpdate, dateUpdate,\r\n " +
                                                            "T.name TurnNameLiqNoVal\r\n " +
                                                            "FROM ProductionLotMachineTurn P\r\n " +
                                                            "INNER JOIN Turn  T ON P.idTurn = T.id" +
                                                            " WHERE P.id = @id ";

        public int id {get; set;}  // id ProductionLot
        public int idMachineProdOpeningDetail { get; set; }
        public int idMachineProdOpening { get; set; }
        public int idTurn { get; set; }
        public int idMachineForProd {get; set;}
        public Nullable<System.TimeSpan> timeInit { get; set; }
        public Nullable<System.TimeSpan> timeEnd { get; set; }
        public int id_userCreate { get; set; }
        public DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public DateTime dateUpdate { get; set; }
        public string TurnNameLiqNoVal { get; set; }

        public static void InsertProductionLotMachineTurn(SqlConnection connection, DbTransaction transaction, ProductionLotMachineTurn item)
        {
            connection.Execute(INSERTPRODUCTLOTMACHINETURN, new 
            {
                id = item.id
                ,idMachineProdOpeningDetail = item.idMachineProdOpeningDetail
                ,idMachineProdOpening = item.idMachineProdOpening
                ,idTurn = item.idTurn
                ,idMachineForProd = item.idMachineForProd
                ,timeInit = item.timeInit
                ,timeEnd = item.timeEnd
                ,id_userCreate = item.id_userCreate
                ,dateCreate = item.dateCreate
            }, transaction);
        }

        public static void UpdateProductionLotMachineTurn(SqlConnection connection, DbTransaction transaction, ProductionLotMachineTurn item)
        {
            connection.Execute(UPDATEPRODUCTLOTMACHINETURN, new
            {
                idMachineProdOpeningDetail= item.idMachineProdOpeningDetail,
                idMachineProdOpening =item.idMachineProdOpening,
                idTurn = item.idTurn,
                idMachineForProd = item.idMachineForProd,
                timeInit = item.timeInit, 
                timeEnd = item.timeEnd, 
                id_userUpdate = item.id_userUpdate, 
                dateUpdate = item.dateUpdate,
                id = item.id
            }, transaction);
        }

        public static ProductionLotMachineTurn GetOneByProductionLot(int productionLotId)
        {
            return DapperConnection.Execute<ProductionLotMachineTurn>(SELECT_ONE_BY_PRODUCTIONLOT, new
            {
                id = productionLotId
            })?.FirstOrDefault();
        }

    }
}