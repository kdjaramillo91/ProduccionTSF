using Dapper;
using DXPANACEASOFT.Dapper;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace DXPANACEASOFT.Models
{
    public partial class ProductionProcess
    {
        public Nullable<bool> requestCarMachine { get; set; }


        private static string INSERTPRODUCTIONPROCESS =     " INSERT INTO ProductionProcess ( id_parentProcess,name,description,code, " +
                                                            " sequential, id_company, isActive, id_userCreate, dateCreate, id_userUpdate, " +
                                                            " dateUpdate, id_warehouse, id_WarehouseLocation, id_CostCenter, id_SubCostCenter, " +
                                                            " id_ProductionUnit, generateTransfer,requestliquidationmachine,generatesAbsorption, " +
                                                            " requestCarMachine) values ( @id_parentProcess, @name, @description, @code, " +
                                                            " @sequential, @id_company, @isActive, @id_userCreate, @dateCreate, @id_userUpdate, " +
                                                            " @dateUpdate, @id_warehouse, @id_WarehouseLocation, @id_CostCenter, @id_SubCostCenter, " +
                                                            " @id_ProductionUnit, @generateTransfer, @requestliquidationmachine, @generatesAbsorption, " +
                                                            " @requestCarMachine )";

        private static string UPDATEPRODUNCTINPROCESS = " UPDATE ProductionProcess " +
                                                        " SET id_parentProcess = @id_parentProcess, name= @name, description = @description, " +
                                                        " sequential=@sequential, isActive = @isActive, id_userUpdate=@id_userUpdate, " +
                                                        " dateUpdate = @dateUpdate, id_warehouse=@id_warehouse, id_WarehouseLocation=@id_WarehouseLocation, id_CostCenter=@id_CostCenter, id_SubCostCenter=@id_SubCostCenter, " +
                                                        " id_ProductionUnit=@id_ProductionUnit, generateTransfer=@generateTransfer,requestliquidationmachine=@requestliquidationmachine,generatesAbsorption=@generatesAbsorption, " +
                                                        " requestCarMachine=@requestCarMachine " +
                                                        " WHERE id= @id";


        private static string SELECT_ALL_BY_COMPANY =   " SELECT  id, id_parentProcess,name,description,code, " +
                                                        " sequential, id_company, isActive, id_userCreate, dateCreate, id_userUpdate, " +
                                                        " dateUpdate, id_warehouse, id_WarehouseLocation, id_CostCenter, id_SubCostCenter, " +
                                                        " id_ProductionUnit, generateTransfer,requestliquidationmachine,generatesAbsorption, " +
                                                        " requestCarMachine FROM ProductionProcess " +
                                                        " WHERE id_company = @id_company";

        private static string SELECT_ONE_BY_ID =        " SELECT  id, id_parentProcess,name,description,code, " +
                                                        " sequential, id_company, isActive, id_userCreate, dateCreate, id_userUpdate, " +
                                                        " dateUpdate, id_warehouse, id_WarehouseLocation, id_CostCenter, id_SubCostCenter, " +
                                                        " id_ProductionUnit, generateTransfer,requestliquidationmachine,generatesAbsorption, " +
                                                        " requestCarMachine FROM ProductionProcess " +
                                                        " WHERE id = @id";


        private static string SELECT_ONE_BY_IDS = " SELECT  id, id_parentProcess,name,description,code, " +
                                                        " sequential, id_company, isActive, id_userCreate, dateCreate, id_userUpdate, " +
                                                        " dateUpdate, id_warehouse, id_WarehouseLocation, id_CostCenter, id_SubCostCenter, " +
                                                        " id_ProductionUnit, generateTransfer,requestliquidationmachine,generatesAbsorption, " +
                                                        " requestCarMachine FROM ProductionProcess " +
                                                        " WHERE id IN (@ids) ";





        public static void InsertProductionProcess(SqlConnection connection, DbTransaction transaction, ProductionProcess item)
        {
            connection.Execute(INSERTPRODUCTIONPROCESS
                , new
                {
                     id_parentProcess = item.id_parentProcess
                    ,name = item.name
                    ,description = item.description
                    ,code = item.code
                    ,sequential = item.sequential
                    ,id_company = item.id_company
                    ,isActive = item.isActive
                    ,id_userCreate = item.id_userCreate
                    ,dateCreate = item.dateCreate
                    ,id_userUpdate = item.id_userUpdate
                    ,dateUpdate = item.dateUpdate
                    ,id_warehouse = item.id_warehouse
                    ,id_WarehouseLocation = item.id_WarehouseLocation
                    ,id_CostCenter = item.id_CostCenter
                    ,id_SubCostCenter = item.id_SubCostCenter
                    ,id_ProductionUnit = item.id_ProductionUnit
                    ,generateTransfer = item.generateTransfer
                    ,requestliquidationmachine = item.requestliquidationmachine     
                    ,generatesAbsorption = item.generatesAbsorption
                    ,requestCarMachine = item.requestCarMachine     

                }, transaction);
        }

        public static void UpdateProductionProcess(SqlConnection connection, DbTransaction transaction, ProductionProcess item)
        {
            connection.Execute(UPDATEPRODUNCTINPROCESS, new 
            {                             
                id_parentProcess = item.id_parentProcess,
                name = item.name,
                description = item.description,
                sequential = item.sequential,
                isActive = item.isActive,
                id_userUpdate = item.id_userUpdate,
                dateUpdate = item.dateUpdate,
                id_warehouse = item.id_warehouse,
                id_WarehouseLocation = item.id_WarehouseLocation,
                id_CostCenter = item.id_CostCenter,
                id_SubCostCenter = item.id_SubCostCenter,
                id_ProductionUnit = item.id_ProductionUnit,
                generateTransfer = item.generateTransfer,
                requestliquidationmachine = item.requestliquidationmachine,
                generatesAbsorption = item.generatesAbsorption,
                requestCarMachine = item.requestCarMachine,
                id = item.id
            }, transaction);
        }

        public static ProductionProcess[] GetAllByCompany(int companyId)
        {
            return DapperConnection.Execute<ProductionProcess>(SELECT_ALL_BY_COMPANY, new
            {
                id_company = companyId
            });
        }

        public static ProductionProcess GetOneById(int id)
        {
            return DapperConnection.Execute<ProductionProcess>(SELECT_ONE_BY_ID, new
            {
                id = id
            })?.FirstOrDefault();
        }

        public static ProductionProcess[] GetAllByIds(int[] ids)
        {
            return DapperConnection.Execute<ProductionProcess>(SELECT_ONE_BY_IDS, new
            {
                ids = ids
            });
        }
    }
}