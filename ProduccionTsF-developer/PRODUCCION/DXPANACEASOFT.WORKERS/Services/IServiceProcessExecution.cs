using DXPANACEASOFT.WORKERS.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DXPANACEASOFT.WORKERS.Services
{
    public interface IServiceProcessExecution
    {
        void Execute(MonthlyBalanceProcessMessageDto monthlyBalanceProcess);
    }
    public class ServiceProcessExecution : IServiceProcessExecution
    {
        private readonly IConfiguration _configuration;
        public ServiceProcessExecution(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void Execute(MonthlyBalanceProcessMessageDto monthlyBalanceProcess)
        {
            if (monthlyBalanceProcess.isMassive)
            {
                string resultTruncate = Truncate();
                if (resultTruncate != "OK")
                {
                    SetAvailableProcess();
                    return;
                }
                List<MonthlyBalance> lsMonthlyBalances = new List<MonthlyBalance>();
                string resultData = GetData(null
                    , null
                    , monthlyBalanceProcess.idWarehouse
                    , monthlyBalanceProcess.idItem
                    , out lsMonthlyBalances);

                if (resultData != "OK")
                {
                    SetAvailableProcess();
                    return;
                }
                string resultInsert = InsertData(lsMonthlyBalances);
                if (resultInsert != "OK")
                {
                    SetAvailableProcess();
                    return;
                }
            }
            else 
            {
                string resultDelete = DeleteInformation(monthlyBalanceProcess.anio.Value
                                            , monthlyBalanceProcess.mes.Value
                                            , monthlyBalanceProcess.idWarehouse
                                            , monthlyBalanceProcess.idItem);

                if (resultDelete != "OK") 
                {
                    SetAvailableProcess() ;
                    return;
                }
                List<MonthlyBalance> lsMonthlyBalances = new List<MonthlyBalance>();
                string resultData = GetData(monthlyBalanceProcess.anio
                    , monthlyBalanceProcess.mes
                    , monthlyBalanceProcess.idWarehouse
                    , monthlyBalanceProcess.idItem
                    , out lsMonthlyBalances);

                if (resultData != "OK")
                {
                    SetAvailableProcess();
                    return;
                }
                string resultInsert = InsertData(lsMonthlyBalances);
                if (resultInsert != "OK")
                {
                    SetAvailableProcess();
                    return;
                }
            }
            SetAvailableProcess();
        }
        private string SetAvailableProcess()
        {
            string result = "ERROR";
            BackgroundProcess? bp = null;
            try 
            {
                using (ProductionContext context = new ProductionContext(_configuration))
                {
                    bp = context
                            .BackgroundProcesses
                            .FirstOrDefault(fod => fod.Code == "BALCALPRO");

                    if (bp != null) 
                    {
                        bp.State = Constants.m_AvailableState;

                        context.Entry(bp).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                result = "ERROR";
            }
            return result;
        }
        private string Truncate()
        {
            string result = "OK";
            try
            {
                using (ProductionContext _context = new ProductionContext(_configuration))
                {
                    _context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"MonthlyBalance\"");
                    _context.SaveChanges();
                }
            }
            catch (Exception) 
            {
                result = "ERROR";
            }
            return result;
        }
        private string DeleteInformation(int anio, int periodo, int? idWarehouse, int? idItem)
        {
            string result = "OK";
            int anioActual = DateTime.Now.Year;

            try
            {
                string query = "";
                if (anio < anioActual)
                {
                    query = $"DELETE FROM \"MonthlyBalance\" WHERE (\"Anio\" = {anio} AND \"Periodo\" >= {periodo}) or (\"Anio\" >={anio + 1} and \"Periodo\" >=1) ";
                }
                else 
                {
                    query = $"DELETE FROM \"MonthlyBalance\" WHERE \"Anio\" = {anio} AND \"Periodo\" >= {periodo} ";
                }
                query += idWarehouse != null? $" AND id_warehouse = {idWarehouse} ": "";
                query += idItem != null ? $" AND id_item = {idItem} " : "";
                using (ProductionContext _context = new ProductionContext(_configuration))
                {
                    _context.Database.ExecuteSqlRaw(query);
                    _context.SaveChanges();
                }
            
            }
            catch (Exception)
            {
                result = "ERROR";
            }
            return result;
        }
        private string GetData(int? anio, int? mes, int? idWarehouse
            , int? idItem, out List<MonthlyBalance> lsMonthlyBalance)
        {
            string result = "OK";
            lsMonthlyBalance = null;
            ParametrosBusquedaKardexSaldo _params = new ParametrosBusquedaKardexSaldo();
            if (!(anio == null && mes == null))
            {
                _params.startEmissionDate = new DateTime(anio.Value, mes.Value, 1);
            }
            if (idWarehouse > 0)
            {
                _params.id_warehouseEntry = idWarehouse.Value;
            }
            if (idItem > 0)
            {
                _params.items = idItem.Value.ToString() + ',';
            }
            try 
            {
                string serializedParams = JsonConvert.SerializeObject(_params);
                using (ProductionContext _context = new ProductionContext(_configuration))
                {
                    lsMonthlyBalance = _context.MonthlyBalances.FromSqlRaw($"inv_Consultar_Kardex_Saldo_Mes_StoredProcedure2 " +
                        $"@p0, @p1", serializedParams, 0).ToList();

                    if (lsMonthlyBalance != null && lsMonthlyBalance.Count > 0)
                    {
                        int lenArray = lsMonthlyBalance.Count;
                        decimal saldoAnterior = 0;
                        string productoBodegaAnterior = "";
                        string productoBodegaActual = "";

                        for (int i = 0; i < lenArray; i++)
                        {
                            productoBodegaActual = string.Concat(lsMonthlyBalance[i].IdWarehouse.ToString(), "|", lsMonthlyBalance[i].IdItem.ToString());
                            if (productoBodegaAnterior != productoBodegaActual)
                            {
                                saldoAnterior = 0;
                            }
                            lsMonthlyBalance[i].SaldoAnterior = saldoAnterior;
                            lsMonthlyBalance[i].SaldoActual = saldoAnterior + (lsMonthlyBalance[i].Entrada - lsMonthlyBalance[i].Salida);

                            saldoAnterior = lsMonthlyBalance[i].SaldoActual ?? 0;
                            productoBodegaAnterior = productoBodegaActual;

                            // Convertir De KGS A LBS
                            if(lsMonthlyBalance[i].CodeMetricUnit == Constants.m_KG_CodeMetricUnit) 
                            {
                                lsMonthlyBalance[i].LbSaldoAnterior =
                                    lsMonthlyBalance[i].SaldoAnterior * Constants.m_Factor_KG_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;

                                lsMonthlyBalance[i].LbEntrada =
                                    lsMonthlyBalance[i].Entrada * Constants.m_Factor_KG_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;

                                lsMonthlyBalance[i].LbSalida =
                                    lsMonthlyBalance[i].Salida * Constants.m_Factor_KG_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;

                                lsMonthlyBalance[i].LbSaldoActual =
                                    lsMonthlyBalance[i].SaldoActual * Constants.m_Factor_KG_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;
                            }

                            if (lsMonthlyBalance[i].CodeMetricUnit == Constants.m_LBS_CodeMetricUnit)
                            {
                                lsMonthlyBalance[i].LbSaldoAnterior =
                                    lsMonthlyBalance[i].SaldoAnterior * Constants.m_Factor_LBS_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;

                                lsMonthlyBalance[i].LbEntrada =
                                    lsMonthlyBalance[i].Entrada * Constants.m_Factor_LBS_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;

                                lsMonthlyBalance[i].LbSalida =
                                    lsMonthlyBalance[i].Salida * Constants.m_Factor_LBS_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;

                                lsMonthlyBalance[i].LbSaldoActual =
                                    lsMonthlyBalance[i].SaldoActual * Constants.m_Factor_LBS_LBS * lsMonthlyBalance[i].Minimum * lsMonthlyBalance[i].Maximum;
                            }
                        }
                    }

                }
            }
            catch (Exception) 
            {
                result = "ERROR";
            }
            return result;
        }
        private string InsertData(List<MonthlyBalance> lsMonthlyBalances) 
        {
            string result = "OK";

            try
            {
                using (ProductionContext _context = new ProductionContext(_configuration))
                { 
                    _context.BulkInsert(lsMonthlyBalances);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                result = "ERROR";
            }
            return result;
        }
    }
}
