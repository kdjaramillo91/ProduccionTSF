using System;
using System.Data;
using System.Data.SqlClient;

namespace DXPANACEASOFT.Dapper
{
    public static class InventoryMoveDetailDapper
    {
        private const string m_TablaName = "InventoryMoveDetail";

        private const string m_IdParamName = "@id";
        private const string m_EntryAmountCostParamName = "@entryAmountCost";
        private const string m_ExitAmountCostParamName = "@exitAmountCost";
        private const string m_UnitPriceParamName = "@unitPrice";
        private const string m_UnitPriceMoveParamName = "@unitPriceMove";
        private const string m_Id_userUpdateParamName = "@id_userUpdate";
        private const string m_DateUpdateParamName = "@dateUpdate";

        public static void ActualizarCostos(SqlConnection db, SqlTransaction tr,
             int id, decimal entryAmountCost, decimal exitAmountCost, decimal unitPrice, int id_userUpdate, DateTime dateUpdate)
        {
            using (var command = new SqlCommand())
            {
                command.CommandText = $"UPDATE {m_TablaName} " +
                    $"SET entryAmountCost = {m_EntryAmountCostParamName}, exitAmountCost = {m_ExitAmountCostParamName}," +
                    $"unitPrice = {m_UnitPriceParamName}, id_userUpdate = {m_Id_userUpdateParamName}, dateUpdate = {m_DateUpdateParamName}, " +
                    $"unitPriceMove = {m_UnitPriceMoveParamName} " +
                    $"WHERE id = {m_IdParamName};";

                command.Parameters.Add(m_IdParamName, SqlDbType.Int).Value = id;
                command.Parameters.Add(m_EntryAmountCostParamName, SqlDbType.Decimal).Value = entryAmountCost;
                command.Parameters.Add(m_ExitAmountCostParamName, SqlDbType.Decimal).Value = exitAmountCost;
                command.Parameters.Add(m_UnitPriceParamName, SqlDbType.Decimal).Value = unitPrice;
                command.Parameters.Add(m_UnitPriceMoveParamName, SqlDbType.Decimal).Value = unitPrice;
                command.Parameters.Add(m_Id_userUpdateParamName, SqlDbType.Int).Value = id_userUpdate;
                command.Parameters.Add(m_DateUpdateParamName, SqlDbType.DateTime).Value = dateUpdate;

                command.CommandType = CommandType.Text;
                command.Connection = db;
                command.Transaction = tr;
                command.ExecuteNonQuery();
            }
        }

    }
}