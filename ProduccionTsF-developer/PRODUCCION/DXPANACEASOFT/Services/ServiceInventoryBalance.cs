

using Dapper;
using DevExpress.Charts.Native;
using DevExpress.CodeParser;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Office.Utils;
using DocumentFormat.OpenXml.Spreadsheet;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.BackgroundProcessManagement;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Operations;
using EntidadesAuxiliares.SQL;
using Newtonsoft.Json;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using static DXPANACEASOFT.Services.ServiceInventoryMove;

namespace DXPANACEASOFT.Services
{
    public class ServiceInventoryBalance
    {

        public static class ServiceInventoryGroupBy
        {
            public const string GROUPBY_ITEM =  "G000ITEM"; 
	        public const string GROUPBY_ITEM_LOTE = "GITEMLOT";
            public const string GROUPBY_BODEGA_ITEM = "GBODITEM";
            public const string GROUPBY_BODEGA_UBICA_ITEM = "GBODUBIT";
            public const string GROUPBY_BODEGA_UBICA_LOTE_ITEM = "GBUBLTIT";
        }


        public static Tuple<Exception, MonthlyBalanceControl[]> ValidateBalanceInventoryPeriod(DBContext db, MonthlyBalanceExecution parameters, DateTime dateInitInventory)
        {
            const string ESTADO_ABIERTO = "A";
            const string ESTADO_PENDIENTE = "P";
            int anio = dateInitInventory.Year;
            Exception exception = null;
            MonthlyBalanceControl[] monthlyBalanceControlAR = null;

            try
            {
                int[] bodegasIds = parameters
                               .Periods
                               .Select(r => r.id_Warehouse)
                               .ToArray();

                var lsPeriods_initial_pre = db
                                            .InventoryPeriodDetail
                                            .Include("InventoryPeriod")
                                            .Include("InventoryPeriod/Warehouse")
                                            .Include("InventoryPeriod/AdvanceParametersDetail")
                                            .Include("AdvanceParametersDetail")
                                            .Where(r => bodegasIds.Contains(r.InventoryPeriod.id_warehouse)
                                                        && r.InventoryPeriod.year >= anio
                                                        && r.InventoryPeriod.isActive
                                                        && r.InventoryPeriod.id_Company == parameters.Id_company)
                                            .Select(s => new
                                            {
                                                year = s.InventoryPeriod.year,
                                                period = s.periodNumber,
                                                dateInit = s.dateInit,
                                                dateEnd = s.dateEnd,
                                                warehouseId = s.InventoryPeriod.id_warehouse,
                                                warehouseName = s.InventoryPeriod.Warehouse.name,
                                                periodoStateCode = s.AdvanceParametersDetail.valueCode.Trim().ToUpper(),
                                                isClosed = s.isClosed,
                                                codigoTipoPeriodo = s.InventoryPeriod.AdvanceParametersDetail.valueCode.Trim().ToUpper(),
                                                nombreTipoPeriodo = s.InventoryPeriod.AdvanceParametersDetail.description
                                            }).ToList();


                #region VALIDACION -- EXISTA DEFINICION DE PERIODO DE INVENTARIO ANIO BODEGA --
                //Periodos no configurados Anio/Bodega
                int anioProcesar = parameters.Year;

                var anioBodegaIds = lsPeriods_initial_pre
                                        .Where(r => r.year == anioProcesar)
                                        .Select(r => r.warehouseId)
                                        .ToArray();

                int[] bodegasSinPeriodo = anioBodegaIds.Where(r => !anioBodegaIds.Contains(r)).ToArray();


                string bodegasSinPeriodos = lsPeriods_initial_pre
                                                    .Where(r => bodegasSinPeriodo.Contains(r.warehouseId))
                                                    .Select(r => r.warehouseName)
                                                    .Aggregate((i, j) => $"{i},{j}");

                if ((bodegasSinPeriodos?.Length ?? 0) > 0)
                {
                    throw new Exception($"No existe Periodo de inventario para el Año: {anioProcesar}, Bodega(s): {bodegasSinPeriodos}");
                }
                #endregion

                #region -- VALIDACION TODOS LOS PERIODOS ANTERIORES HASTA EL PERIODO ACTUAL ESTEN CERRADOS --
                // Periodos no cerrados 
                string[] estadosAbiertos = new string[] { ESTADO_ABIERTO, ESTADO_PENDIENTE };
                var lsPeriodsInitialNoCerrado = lsPeriods_initial_pre
                                                            .Where(r => !estadosAbiertos.Contains(r.periodoStateCode))
                                                            .ToArray();

                var lsPeriodsInitialBodegasPeriodosGroup = (from periodo in lsPeriodsInitialNoCerrado
                                                            join parametro in parameters.Periods
                                                            on new { bodega = periodo.warehouseId, codigoTipoPeriodo = periodo.codigoTipoPeriodo } equals
                                                            new { bodega = parametro.id_Warehouse, codigoTipoPeriodo = parametro.codeTypePeriod }
                                                            select new
                                                            {
                                                                periodo_data = periodo,
                                                                parametro_data = parametro
                                                            })
                                                           .Where(r => r.periodo_data.dateInit < r.parametro_data.PeriodDetailInitId)
                                                           .GroupBy(r => r.periodo_data.warehouseName);


                monthlyBalanceControlAR = lsPeriodsInitialBodegasPeriodosGroup
                                                         .Select(r => new
                                                         {
                                                             bodega = r.Key,
                                                             bodegaId = r.Max(t => t.periodo_data.warehouseId),
                                                             periodo = r.Select(t => new
                                                             {
                                                                 codigoTipoPeriodo = t.periodo_data.codigoTipoPeriodo,
                                                                 fechaInicio = t.periodo_data.dateInit,
                                                                 periodoMes = t.periodo_data.dateInit.Month,
                                                                 diaMes = t.periodo_data.dateInit.Day

                                                             })
                                                             .OrderByDescending(t => t.fechaInicio)
                                                             .FirstOrDefault()

                                                         })
                                                         .Select(r => new MonthlyBalanceControl
                                                         {
                                                             id_company = parameters.Id_company,
                                                             id_warehouse = r.bodegaId,
                                                             Anio = parameters.Year,
                                                             Mes = r.periodo.periodoMes,
                                                             IsValid = (r.periodo.diaMes == 1) ? false : true,
                                                             LastDateProcess = (r.periodo.diaMes == 1) ? null : (DateTime?)r.periodo.fechaInicio,
                                                             DateIsNotValid = (r.periodo.diaMes == 1) ? (DateTime?)DateTime.Now : null
                                                         })
                                                         .ToArray();

                var bodegasPeriodosErrorMessage = lsPeriodsInitialBodegasPeriodosGroup
                                                        .Select(r => new
                                                        {
                                                            bodega = r.Key,
                                                            periodos = r.Select(t => new
                                                            {
                                                                nombreTipoPeriodo = t.periodo_data.nombreTipoPeriodo,
                                                                fechaPeriodoTexto = (t.periodo_data.codigoTipoPeriodo.Trim() == "M") ? $"{t.periodo_data.year}-{GlobalUtils.ConvertToMonthName(t.periodo_data.period)}" : GlobalUtils.DateFromDayYear(t.periodo_data.year, t.periodo_data.period).ToString("yyyy-MM-dd")
                                                            })
                                                             .Select(t => $"{t.nombreTipoPeriodo}: {t.fechaPeriodoTexto}")
                                                             .Aggregate((i, j) => $"{i}, {j}")

                                                        })
                                                         .Select(r => $"Bodega: {r.bodega}, Periodos: {r.periodos} {Environment.NewLine}")
                                                         .ToArray();

                if ((bodegasPeriodosErrorMessage?.Length ?? 0) > 0)
                {
                    var mensaje = $"Periodos no cerrados: {bodegasPeriodosErrorMessage.Aggregate((i, j) => $"{i},{j}")}";
                    throw new Exception(mensaje);
                }

                #endregion
            }
            catch (Exception e)
            {
                exception = e;
            }

            return new Tuple<Exception, MonthlyBalanceControl[]>(exception, monthlyBalanceControlAR);

        }
        public static Tuple<Exception, MonthlyBalanceControl[]> ValidateBalancePeriod(DBContext db, MonthlyBalanceExecution parameters, DateTime dateInitInventory)
        {
            const string sentencia_MonthlyBalanceControl_Estado_PeriodoBodega = "select id,id_company,id_warehouse,Anio,Mes,IsValid,DateIsNotValid,LastDateProcess from MonthlyBalanceControl where id_company = @id_company AND  Anio <= @Anio;";

            var control = DapperConnection.Execute<MonthlyBalanceControl>(sentencia_MonthlyBalanceControl_Estado_PeriodoBodega, new
            {
                id_company = parameters.Id_company,
                Anio = parameters.Year
            });


            int[] bodegasIds = parameters
                                       .Periods
                                       .Select(r => r.id_Warehouse)
                                       .ToArray();

            var parametrosBodegasPeriodos = parameters
                                                .Periods
                                                .Select(r => new
                                                {
                                                    bodegaId = r.id_Warehouse,
                                                    periodoMesAnio = r.PeriodDetailInitId.ToYearMonthPeriod(),
                                                    fechaInicio = r.PeriodDetailInitId,
                                                    fechaFin = r.PeriodDetailEndId
                                                })
                                                .ToArray();

            var controlBodegas = control
                                    .Where(r => bodegasIds.Contains(r.id_warehouse))
                                    .Select(r => new
                                    {
                                        id = r.id,
                                        periodoMesAnio = $"{r.Anio}{r.Mes.ToString().PadLeft(2, '0')}",
                                        id_company = r.id_company,
                                        id_warehouse = r.id_warehouse,
                                        isValid = r.IsValid,
                                        LastDateProcess = r.LastDateProcess
                                    })
                                    .ToArray();

            var filtererdPeriod = (from parametro in parametrosBodegasPeriodos
                                   join controls in controlBodegas
                                   on new { bodega = parametro.bodegaId, periodo = parametro.periodoMesAnio }
                                   equals new { bodega = controls.id_warehouse, periodo = controls.periodoMesAnio }
                                   select new
                                   {
                                       parametros = parametro,
                                       control = controls,
                                   })
                                   .ToArray();

            return new Tuple<Exception, MonthlyBalanceControl[]>(null, null);
        }

        public static void ValidateBalanceProcess(DBContext db, int id_company, int idUser, MonthlyBalanceExecution parameters)
        {
            // return Tuple Exceptioon MontlyBalanceParameter
            const string CODIGO_PARAMETRO_SALDO_INICIAL = "PSALINI";
            string mensaje = null;

            var settingSaldoInicial = db.Setting.FirstOrDefault(r => r.code == CODIGO_PARAMETRO_SALDO_INICIAL);
            if (settingSaldoInicial == null)
            {
                mensaje = $"No se ha definido el parámetro: {CODIGO_PARAMETRO_SALDO_INICIAL}";

            }
            int anio = int.Parse(settingSaldoInicial.value.Substring(0, 4));
            int mes = int.Parse(settingSaldoInicial.value.Substring(4, 2));

            DateTime dateInitInventory = new DateTime(anio, mes, 1);

            var resultValidaInventoryPeriod = ValidateBalanceInventoryPeriod(db, parameters, dateInitInventory);
            Exception exceptionValidaInventoryPeriod = resultValidaInventoryPeriod.Item1;
            MonthlyBalanceControl[] dataErrorValidaInventoryPeriod = resultValidaInventoryPeriod.Item2; //=>
            if (exceptionValidaInventoryPeriod != null)
            {
                throw exceptionValidaInventoryPeriod;
            }


            //return 
        }

        public static Exception ProcessBalanceOpenCloseInventoryPeriod(DBContext db, InventoryPeriod memInventoryPeriod, InventoryPeriod dbInventoryPeriod)
        {
            Exception exception = null;
            try
            {
                var tipos = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("TPGV1");
                int tipoDiarioId = (tipos.FirstOrDefault(r => r.valueCode == "D")?.id ?? 0);
                int tipoMensualId = (tipos.FirstOrDefault(r => r.valueCode == "M")?.id ?? 0);

                var estados = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
                int estadoAbiertoId = (tipos.FirstOrDefault(r => r.valueCode == "A")?.id ?? 0);
                int estadoCerradoId = (tipos.FirstOrDefault(r => r.valueCode == "C")?.id ?? 0);

                if (dbInventoryPeriod.id_PeriodType == tipoDiarioId)
                {
                    InventoryPeriodDetail[] memPeridosDetalle = memInventoryPeriod
                                                                    .InventoryPeriodDetail
                                                                    .Select(r => r)
                                                                    .ToArray();
                    InventoryPeriodDetail[] dbPeridosDetalle = dbInventoryPeriod
                                                                    .InventoryPeriodDetail
                                                                    .Select(r => r)
                                                                    .ToArray();
                    #region -- Buscar Periodos/Elementos con cambios --

                    var detallesSinCambio = (from dbP in dbPeridosDetalle
                                             join memP in memPeridosDetalle
                                             on new { id = dbP.id, estado = dbP.id_PeriodState }
                                             equals new { id = memP.id, estado = memP.id_PeriodState }
                                             select memP.id)
                                             .ToArray();

                    var detallesCambios = memPeridosDetalle
                                                .Where(r => !detallesSinCambio.Contains(r.id))
                                                .ToArray();

                    var aperturados = detallesCambios
                                            .Where(r => r.id_PeriodState == estadoAbiertoId)
                                            .GroupBy(r => r.dateInit.Month)
                                            .Select(r => new
                                            {
                                                Mes = r.Key,
                                                Dias = r.Select(t => new
                                                {
                                                    fecha = t.dateInit
                                                }).ToArray()

                                            })
                                            .ToArray();

                    var cerrados = detallesCambios
                                            .Where(r => r.id_PeriodState == estadoCerradoId)
                                            .GroupBy(r => r.dateInit.Month)
                                            .Select(r => new
                                            {
                                                Mes = r.Key,
                                                Dias = r.Select(t => new
                                                {
                                                    fecha = t.dateInit
                                                }).ToArray()

                                            })
                                            .ToArray();

                    #endregion

                    List<MonthlyBalanceControl> toUpdate = new List<MonthlyBalanceControl>();
                    List<MonthlyBalanceProcessMessageDto> toProccess = new List<MonthlyBalanceProcessMessageDto>();

                    #region -- Procesar  Aperturados --

                    var mesMinimoAperturado = aperturados.OrderBy(r => r.Mes).Select(r => r).FirstOrDefault();
                    DateTime fechaMinApertura = mesMinimoAperturado.Dias.OrderBy(r => r.fecha).Select(r => r.fecha).FirstOrDefault();

                    var periodos = memInventoryPeriod
                                        .InventoryPeriodDetail
                                        .Where(r => r.dateInit.Date >= fechaMinApertura.Date)
                                        .ToList();

                    var mesPeriodosDesactualizar = periodos
                                                       .Select(r => r.dateInit.Month)
                                                       .GroupBy(r => r)
                                                       .Select(r => r.Key)
                                                       .ToArray();

                    var balanceControlAnio = GetMonthlyBalanceControlAnio(memInventoryPeriod.id_Company, memInventoryPeriod.id_warehouse, memInventoryPeriod.year);
                    var balanceControlAnioFilter = balanceControlAnio
                                                            .Where(r => mesPeriodosDesactualizar.Contains(r.Mes))
                                                            .ToArray();

                    /*
                     -- mes ene
                         01
                         02
                     -- mes feb
                         01 
                         02
                     
                     */


                    for (int i = 0; i < aperturados.Length; i++)
                    {
                        var periodoMes = aperturados[i];

                        DateTime fechaMinima = periodoMes.Dias.OrderBy(r => r.fecha).Select(r => r.fecha).FirstOrDefault();
                        int mes = fechaMinima.Month;

                        MonthlyBalanceControl monthlyBalanceControl = GetMonthlyBalanceControl(dbInventoryPeriod.id_Company,
                                                                                                dbInventoryPeriod.id_warehouse,
                                                                                                dbInventoryPeriod.year,
                                                                                                mes);
                        if (monthlyBalanceControl == null) continue;

                        int dia = fechaMinima.Day;

                        if (dia == 1)
                        {
                            monthlyBalanceControl.IsValid = false;
                            monthlyBalanceControl.DateIsNotValid = DateTime.Now;
                            monthlyBalanceControl.LastDateProcess = fechaMinima;
                        }
                        else
                        {
                            monthlyBalanceControl.LastDateProcess = fechaMinima.AddDays(-1);
                        }
                        toUpdate.Add(monthlyBalanceControl);

                    }
                    #endregion

                    #region -- Procesar  Cierre --

                    //var cerradosAgrupadosMes = cerrados
                    //                                .Select()

                    for (int i = 0; i < cerrados.Length; i++)
                    {
                        var periodMes = cerrados[i];
                        DateTime fechaInicio = new DateTime(memInventoryPeriod.year, periodMes.Mes, 1);
                        DateTime fechaMaxima = periodMes.Dias.OrderByDescending(r => r.fecha).Select(r => r.fecha).FirstOrDefault();

                        var resultPeriodosDiasMes = memInventoryPeriod
                                                            .InventoryPeriodDetail
                                                            .Where(r => r.dateInit.Date <= fechaMaxima.Date &&
                                                                        r.dateEnd.Date >= fechaInicio.Date)
                                                            .ToArray();

                    }
                    #endregion


                }
                else if (dbInventoryPeriod.id_PeriodType == tipoMensualId)
                {

                }


            } catch (Exception e)
            {
                exception = e;


            }
            return exception;

        }


        public static bool Execute(DBContext db)
        {
            bool canContinue = false;
            BackgroundProcess bp = null;
            try
            {

                bp = db
                        .BackgroundProcess
                        .FirstOrDefault(fod => fod.code == "BALCALPRO");

                if (bp == null)
                {
                    db.BackgroundProcess.Add(new BackgroundProcess
                    {
                        code = "BALCALPRO",
                        state = "INPROCESS",
                        dateCreation = DateTime.Now,
                    });
                    db.SaveChanges();
                    canContinue = true;
                }
                else
                {
                    if (bp.state == "AVAILABLE")
                    {
                        bp.state = "INPROCESS";
                        bp.dateModification = DateTime.Now;
                        db.SaveChanges();
                        canContinue = true;
                    }
                    else if (bp.state == "INPROCESS")
                        canContinue = false;
                    else
                        canContinue = false;
                }
            }
            catch (Exception ex)
            {
                canContinue = false;
            }
            return canContinue;
        }

        public static void Execute(DBContext db, MonthlyBalanceProcessMessageDto monthlyBalanceProcess)
        {
            if (monthlyBalanceProcess.isMassive)
            {
                var exceptionTruncate = Truncate(db);
                if (exceptionTruncate != null)
                {
                    throw exceptionTruncate;
                }
                List<MonthlyBalance> lsMonthlyBalances = new List<MonthlyBalance>();
                DateTime? lastDateProcess = null;
                var exceptionData = GetData(monthlyBalanceProcess.id_company
                                            , null
                                            , null
                                            , monthlyBalanceProcess.idWarehouse
                                            , monthlyBalanceProcess.idWarehouseLocation
                                            , monthlyBalanceProcess.idItem
                                            , monthlyBalanceProcess.codigoTipoPeriodo
                                            , out lsMonthlyBalances
                                            , out lastDateProcess);

                if (exceptionData != null)
                {
                    throw exceptionData;
                }
                var exceptionInsert = InsertData(lsMonthlyBalances);
                if (exceptionInsert != null)
                {
                    throw exceptionInsert;
                }
            }
            else
            {

                var exceptionDelete = DeleteInformation(db,
                                            monthlyBalanceProcess.anio
                                            , monthlyBalanceProcess.mes
                                            , monthlyBalanceProcess.idWarehouse
                                            , monthlyBalanceProcess.idWarehouseLocation
                                            , monthlyBalanceProcess.idItem);

                if (exceptionDelete != null)
                {
                    throw exceptionDelete;
                }


                List<MonthlyBalance> lsMonthlyBalances = new List<MonthlyBalance>();
                DateTime? lastDateProcess = null;
                var exceptionData = GetData(monthlyBalanceProcess.id_company,
                                            monthlyBalanceProcess.anio
                                            , monthlyBalanceProcess.mes
                                            , monthlyBalanceProcess.idWarehouse
                                            , monthlyBalanceProcess.idWarehouseLocation
                                            , monthlyBalanceProcess.idItem
                                            , monthlyBalanceProcess.codigoTipoPeriodo
                                            , out lsMonthlyBalances
                                            , out lastDateProcess);

                if (exceptionData != null)
                {
                    throw exceptionData;
                }
                lsMonthlyBalances = lsMonthlyBalances
                                            .Where(r => r.Anio == monthlyBalanceProcess.anio && r.Periodo == monthlyBalanceProcess.mes)
                                            .ToList();

                var exceptionInsert = InsertData(lsMonthlyBalances);
                if (exceptionInsert != null)
                {
                    throw exceptionInsert;
                }

                if (!monthlyBalanceProcess.idWarehouseLocation.HasValue
                     && !monthlyBalanceProcess.idItem.HasValue)
                {
                    string select_balance_control = "select  id,id_company,id_warehouse,Anio,Mes,IsValid,DateIsNotValid,LastDateProcess from MonthlyBalanceControl where id_company = @id_company AND Anio = @Anio AND Mes = @Mes";

                    var lsControl = DapperConnection.Execute<MonthlyBalanceControl>(select_balance_control, new
                    {
                        id_company = monthlyBalanceProcess.id_company,
                        Anio = monthlyBalanceProcess.anio,
                        Mes = monthlyBalanceProcess.mes
                    });

                    List<int> bodegasIds = new List<int>();
                    if (monthlyBalanceProcess.idWarehouse.HasValue)
                    {
                        bodegasIds.Add(monthlyBalanceProcess.idWarehouse.Value);
                    }
                    else
                    {
                        bodegasIds = db.Warehouse
                                            .Where(w => w.id_company == monthlyBalanceProcess.id_company && w.isActive == true)
                                            .Select(r => r.id)
                                            .ToList();
                    }
                    List<int> bodegaToInsertsIds = new List<int>();
                    List<MonthlyBalanceControl> lsControlToUpdate = null;


                    if ((lsControl?.Length ?? 0) > 0)
                    {
                        var lsControlFilterWarehouse = lsControl
                                                        .Where(r => bodegasIds.Contains(r.id_warehouse))
                                                        .ToList();

                        // En tabla de control
                        lsControlToUpdate = lsControlFilterWarehouse
                                                    .Select(r => new MonthlyBalanceControl
                                                    {
                                                        id = r.id,
                                                        Anio = r.Anio,
                                                        DateIsNotValid = null,
                                                        id_warehouse = r.id_warehouse,
                                                        id_company = r.id_company,
                                                        IsValid = true,
                                                        Mes = r.Mes,
                                                        LastDateProcess = lastDateProcess.Value

                                                    }).ToList();

                        var warehouseUpdateIds = lsControlFilterWarehouse
                                                            .Select(r => r.id_warehouse)
                                                            .ToArray();


                        bodegaToInsertsIds = bodegasIds
                                                    .Where(r => !warehouseUpdateIds.Contains(r))
                                                    .ToList();
                    }
                    else
                    {
                        bodegaToInsertsIds = bodegasIds;
                    }

                    List<MonthlyBalanceControl> lsControlToInsert = bodegaToInsertsIds
                                                                            .Select(r => new MonthlyBalanceControl
                                                                            {
                                                                                Anio = monthlyBalanceProcess.anio,
                                                                                DateIsNotValid = null,
                                                                                id_company = monthlyBalanceProcess.id_company,
                                                                                id_warehouse = r,
                                                                                IsValid = true,
                                                                                Mes = monthlyBalanceProcess.mes,
                                                                                LastDateProcess = lastDateProcess.Value
                                                                            }).ToList();

                    if ((lsControlToUpdate?.Count ?? 0) > 0)
                    {
                        var exceptionControlUpdate = UpdateControl(lsControlToUpdate);
                        if (exceptionControlUpdate != null)
                        {
                            throw exceptionControlUpdate;
                        }
                    }

                    if ((lsControlToInsert?.Count ?? 0) > 0)
                    {
                        var exceptionControlInsert = InsertControl(lsControlToInsert);
                        if (exceptionControlInsert != null)
                        {
                            throw exceptionControlInsert;
                        }
                    }
                }


            }
            SetAvailableProcess(db);
        }

        public static Tuple<int, int> GetPreviewPeriod(int anio, int mes)
        {
            int nanio = anio;
            int nmes = mes;

            if (mes == 1)
            {
                nanio -= 1;
                nmes = 12;
            }
            else
            {
                nmes -= 1;
            }
            return new Tuple<int, int>(nanio, nmes);

        }


        private static Exception Truncate(DBContext db)
        {
            Exception exception = null;
            try
            {

                db.Database.ExecuteSqlCommand("TRUNCATE TABLE \"MonthlyBalance\"");
                db.SaveChanges();

            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }



        internal static string SetAvailableProcessInt(DBContext db)
        {
            return SetAvailableProcess(db);
        }
        private static string SetAvailableProcess(DBContext db)
        {
            string result = "ERROR";
            BackgroundProcess bp = null;
            try
            {

                bp = db
                        .BackgroundProcess
                        .FirstOrDefault(fod => fod.code == "BALCALPRO");

                if (bp != null)
                {
                    bp.state = InventoryBalanceConstants.m_AvailableState;

                    db.Entry(bp).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (Exception)
            {
                result = "ERROR";
            }
            return result;
        }



        private static Exception GetData(int id_company, int? anio, int? mes, int? idWarehouse, int? idWarehouseLocation
            , int? idItem, string codeTypePeriod, out List<MonthlyBalance> lsMonthlyBalance, out DateTime? lastDateProcess)
        {
            Exception exception = null;
            lsMonthlyBalance = null;
            lastDateProcess = null;
            ParametrosBusquedaKardexSaldo _params = new ParametrosBusquedaKardexSaldo();

            _params.id_company = id_company;
            _params.codeTypePeriod = codeTypePeriod;
            if (!(anio == null && mes == null))
            {
                int diaUpd = 1;
                int mesUpd = mes.Value;
                if (codeTypePeriod == "D")
                {
                    DateTime fechaInicio = GlobalUtils.DateFromDayYear(anio.Value, mes.Value);
                    diaUpd = fechaInicio.Day;
                    mesUpd = fechaInicio.Month;

                    _params.endEmissionDate = new DateTime(anio.Value, mesUpd, diaUpd);

                }
                else
                {
                    _params.endEmissionDate = GlobalUtils.DateLastDayFromMonth(anio.Value, mesUpd);

                }
                lastDateProcess = _params.endEmissionDate;
                _params.startEmissionDate = new DateTime(anio.Value, mesUpd, diaUpd);

            }
            if ((idWarehouse ?? 0) > 0)
            {
                _params.id_warehouseEntry = idWarehouse.Value;
            }
            if ((idWarehouseLocation ?? 0) > 0)
            {
                _params.id_warehouseLocationEntry = idWarehouseLocation.Value;
            }
            if ((idItem ?? 0) > 0)
            {
                _params.items = idItem.Value.ToString() + ',';
            }
            try
            {
                string serializedParams = JsonConvert.SerializeObject(_params);

                //lsMonthlyBalance = DapperConnection.Execute<MonthlyBalance>("inv_Consultar_Kardex_Saldo_Mes_StoredProcedure2", new
                lsMonthlyBalance = DapperConnection.Execute<MonthlyBalance>("inv_Genera_Saldo_Mes_Control", new
                {
                    ParametrosBusquedaKardexSaldo = serializedParams,
                    printDebug = 0
                }, System.Data.CommandType.StoredProcedure, timeout: 300).ToList();


                if (lsMonthlyBalance != null && lsMonthlyBalance.Count > 0)
                {
                    int lenArray = lsMonthlyBalance.Count;
                    decimal saldoAnterior = 0;
                    string productoBodegaAnterior = "";
                    string productoBodegaActual = "";

                    for (int i = 0; i < lenArray; i++)
                    {
                        lsMonthlyBalance[i].id_company = id_company;
                        //productoBodegaActual = string.Concat(lsMonthlyBalance[i].id_warehouse.ToString(), "|", lsMonthlyBalance[i].id_item.ToString());
                        //if (productoBodegaAnterior != productoBodegaActual)
                        //{
                        //    saldoAnterior = 0;
                        //}
                        //lsMonthlyBalance[i].SaldoAnterior = saldoAnterior;
                        //lsMonthlyBalance[i].SaldoActual = saldoAnterior + (lsMonthlyBalance[i].Entrada - lsMonthlyBalance[i].Salida);
                        //
                        //saldoAnterior = lsMonthlyBalance[i].SaldoActual ?? 0;
                        //productoBodegaAnterior = productoBodegaActual;

                        // Convertir De KGS A LBS
                        if (lsMonthlyBalance[i].code_metric_unit == InventoryBalanceConstants.m_KG_CodeMetricUnit)
                        {
                            lsMonthlyBalance[i].LB_SaldoAnterior =
                                lsMonthlyBalance[i].SaldoAnterior * InventoryBalanceConstants.m_Factor_KG_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;

                            lsMonthlyBalance[i].LB_Entrada =
                                lsMonthlyBalance[i].Entrada * InventoryBalanceConstants.m_Factor_KG_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;

                            lsMonthlyBalance[i].LB_Salida =
                                lsMonthlyBalance[i].Salida * InventoryBalanceConstants.m_Factor_KG_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;

                            lsMonthlyBalance[i].LB_SaldoActual =
                                lsMonthlyBalance[i].SaldoActual * InventoryBalanceConstants.m_Factor_KG_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;
                        }

                        if (lsMonthlyBalance[i].code_metric_unit == InventoryBalanceConstants.m_LBS_CodeMetricUnit)
                        {
                            lsMonthlyBalance[i].LB_SaldoAnterior =
                                lsMonthlyBalance[i].SaldoAnterior * InventoryBalanceConstants.m_Factor_LBS_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;

                            lsMonthlyBalance[i].LB_Entrada =
                                lsMonthlyBalance[i].Entrada * InventoryBalanceConstants.m_Factor_LBS_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;

                            lsMonthlyBalance[i].LB_Salida =
                                lsMonthlyBalance[i].Salida * InventoryBalanceConstants.m_Factor_LBS_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;

                            lsMonthlyBalance[i].LB_SaldoActual =
                                lsMonthlyBalance[i].SaldoActual * InventoryBalanceConstants.m_Factor_LBS_LBS * lsMonthlyBalance[i].minimum * lsMonthlyBalance[i].maximum;
                        }
                    }
                }


            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }

        private static Exception InsertControl(List<MonthlyBalanceControl> lsMonthlyBalancesControl)
        {
            Exception exception = null;

            try
            {
                DapperConnection.BulkInsert<List<MonthlyBalanceControl>>(lsMonthlyBalancesControl);
            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }

        private static Exception UpdateControl(List<MonthlyBalanceControl> lsMonthlyBalancesControl)
        {
            Exception exception = null;

            try
            {
                DapperConnection.BulkUpdate<List<MonthlyBalanceControl>>(lsMonthlyBalancesControl);
            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }

        private static Exception InsertData(List<MonthlyBalance> lsMonthlyBalances)
        {
            Exception exception = null;

            try
            {
                DapperConnection.BulkInsert<List<MonthlyBalance>>(lsMonthlyBalances);
            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }



        private static Exception DeleteInformation(DBContext db, int anio, int periodo, int? idWarehouse, int? idWarehouseLocation, int? idItem)
        {
            Exception exception = null;
            int anioActual = DateTime.Now.Year;

            try
            {
                string query = "";
                //if (anio < anioActual)
                //{
                //    query = $"DELETE FROM \"MonthlyBalance\" WHERE (\"Anio\" = {anio} AND \"Periodo\" >= {periodo}) or (\"Anio\" >={anio + 1} and \"Periodo\" >=1) ";
                //}
                //else
                //{
                //    query = $"DELETE FROM \"MonthlyBalance\" WHERE \"Anio\" = {anio} AND \"Periodo\" >= {periodo} ";
                //}

                query = $"DELETE FROM \"MonthlyBalance\" WHERE \"Anio\" = {anio} AND \"Periodo\" = {periodo} ";
                query += idWarehouse != null ? $" AND id_warehouse = {idWarehouse} " : "";
                query += idWarehouseLocation != null ? $" AND id_warehouseLocation = {idWarehouseLocation} " : "";

                query += idItem != null ? $" AND id_item = {idItem} " : "";

                db.Database.ExecuteSqlCommand(query);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }



        public static Tuple<MonthlyBalance[], SaldoProductoLote[]> ValidateBalanceGeneral(InvParameterBalanceGeneral invParameterBalance,
                                                                                            bool modelSaldoProductlote,
                                                                                            SqlConnection connection = null,
                                                                                            IDbTransaction transaction = null,
                                                                                            bool withNegatives = false)
        {
            MonthlyBalance[] monthlyBalance = null;
            SaldoProductoLote[] saldoProductoLote = null;

            string _cadenaConexion = null;
            if (connection == null)
            {
                _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            }

            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];


            if (invParameterBalance.id_productionCart.HasValue
                    || !string.IsNullOrEmpty(invParameterBalance.lotMarket))
            {

                var sqlParam = new[]
                {
                    new ParamSQL()
                    {
                        Nombre = "@requiresLot",
                        TipoDato = DbType.Boolean,
                        Valor = invParameterBalance.requiresLot,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_warehouse",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_Warehouse,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_warehouseLocation",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_WarehouseLocation,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_item",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_Item,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_lot",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_ProductionLot,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@lotMarket",
                        TipoDato = DbType.String,
                        Valor = invParameterBalance.lotMarket,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@fechaEmision",
                        TipoDato = DbType.Int64,
                        Valor =  invParameterBalance.cut_Date.HasValue
                            ? (int?)(invParameterBalance.cut_Date.Value.Year * 10000 + invParameterBalance.cut_Date.Value.Month * 100 + invParameterBalance.cut_Date.Value.Day)
                            : null,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_productionCart",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_productionCart,
                    },
                };



                DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
                    .ObtieneDatos(_cadenaConexion, "Get_Saldo_Lote_Producto", _rutaLog,
                        "ServiceInventoryMove", "PROD", sqlParam.ToList(), timeout: 1200, connection: connection);

                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    var resultados = dataSet.Tables[0].AsEnumerable();
                    saldoProductoLote = resultados
                        .Select(e => new ServiceInventoryMove.SaldoProductoLote
                        {
                            id_item = e.Field<Int32>("id_item"),
                            id_lote = e.Field<Int32?>("id_lote"),
                            saldo = e.Field<Decimal>("saldo"),
                            lot_market = e.Field<String>("lot_market"),
                            number = e.Field<String>("number"),
                            id_metricUnit = e.Field<Int32>("unidadMedida"),
                            internalNumber = e.Field<String>("internalNumber"),
                            receptionDate = e.Field<DateTime?>("receptionDate"),

                        }).ToArray();
                }
                else
                {
                    saldoProductoLote = Array.Empty<SaldoProductoLote>();
                }

            }
            else
            {

                var sqlParam = new[]
                {
                    new ParamSQL()
                    {
                        Nombre = "@id_company",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_company,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_Item",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_Item,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@fecha_corte",
                        TipoDato = DbType.String,
                        Valor = invParameterBalance.cut_Date.HasValue
                        ? (int?)(invParameterBalance.cut_Date.Value.Year * 10000 + invParameterBalance.cut_Date.Value.Month * 100 + invParameterBalance.cut_Date.Value.Day)
                        : null,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@consolidado",
                        TipoDato = DbType.Byte,
                        Valor = invParameterBalance.consolidado,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@groupby",
                        TipoDato = DbType.String,
                        Valor = invParameterBalance.groupby,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_Warehouse",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_Warehouse,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_WarehouseLocation",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_WarehouseLocation,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_ProductionLot",
                        TipoDato = DbType.Int64,
                        Valor = invParameterBalance.id_ProductionLot,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@requiresLot",
                        TipoDato = DbType.Byte,
                        Valor = invParameterBalance.requiresLot,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_itemlist",
                        TipoDato = DbType.String,
                        Valor = invParameterBalance.idItemList,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@id_inventorydetails",
                        TipoDato = DbType.String,
                        Valor = invParameterBalance.idInventoryDetails,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@with_negatives",
                        TipoDato = DbType.Boolean,
                        Valor = withNegatives,
                    }

                };

                DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
                        .ObtieneDatos(_cadenaConexion, "inv_Consulta_Saldo_Mes_Control_General", _rutaLog,
                            "ServiceInventoryBalanceMove", "PROD", sqlParam.ToList(), timeout: 1200,
                            connection: connection,
                            transaction: transaction);


                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    var resultados = dataSet.Tables[0].AsEnumerable();

                    if (modelSaldoProductlote)
                    {
                        saldoProductoLote = resultados
                               .Select(e => new ServiceInventoryMove.SaldoProductoLote
                               {
                                   id_item = e.Field<Int32>("id_item"),
                                   id_lote = e.Field<Int32?>("id_productionLot"),
                                   saldo = e.Field<Decimal>("SaldoActual"),
                                   number = e.Field<String>("numberLot"),
                                   id_metricUnit = e.Field<Int32>("id_metricUnit"),
                                   internalNumber = e.Field<String>("internalNumberLot"),
                                   receptionDate = e.Field<DateTime?>("fechaRecepcion"),
                                   id_warehouseLocation = e.Field<Int32?>("id_warehouseLocation"),
                               }).ToArray();

                    }
                    else
                    {
                        monthlyBalance = resultados
                            .Select(e => new MonthlyBalance
                            {
                                id_item = e.Field<Int32>("id_item"),
                                id_productionLot = e.Field<Int32>("id_productionLot"),
                                SaldoActual = e.Field<Decimal>("SaldoActual"),
                                sequencial_productionLot = e.Field<String>("numberLot"),
                                number_productionLot = e.Field<String>("internalNumberLot"),
                                id_metric_unit = e.Field<Int32>("id_metricUnit"),
                                id_warehouse = e.Field<Int32>("id_warehouse"),
                                id_warehouseLocation = e.Field<Int32>("id_warehouseLocation"),
                            }).ToArray();


                    }


                }
                else
                {
                    if (modelSaldoProductlote)
                    {
                        saldoProductoLote = Array.Empty<SaldoProductoLote>();
                    }
                    else
                    {
                        monthlyBalance = Array.Empty<MonthlyBalance>();
                    }

                }


            }

            return new Tuple<MonthlyBalance[], SaldoProductoLote[]>(monthlyBalance, saldoProductoLote);
        }

        public static Tuple<MonthlyBalance[], SaldoProductoLote[]> ValidateBalanceGeneralDap(DBContext db, 
                                                                            InvParameterBalanceGeneral invParameterBalance,
                                                                            bool modelSaldoProductlote)
        {
            db.Database.CommandTimeout = 2200;
            MonthlyBalance[] monthlyBalance = null; 
            SaldoProductoLote[] saldoProductoLote = null;

            if (invParameterBalance.id_productionCart.HasValue
                    || !string.IsNullOrEmpty(invParameterBalance.lotMarket))
            {
                saldoProductoLote = DapperConnection.Execute<SaldoProductoLote>("Get_Saldo_Lote_Producto", new
                {
                    requiresLot = invParameterBalance.requiresLot,
                    id_warehouse = (object)invParameterBalance.id_Warehouse ?? DBNull.Value,
                    id_warehouseLocation = (object)invParameterBalance.id_WarehouseLocation ?? DBNull.Value,
                    id_item = (object)invParameterBalance.id_Item ?? DBNull.Value,
                    id_lot = (object)invParameterBalance.id_ProductionLot ?? DBNull.Value,
                    lotMarket = (object)(string.IsNullOrEmpty(invParameterBalance.lotMarket)) ?? DBNull.Value,
                    fechaEmision = invParameterBalance.cut_Date.HasValue
                        ? (int?)(invParameterBalance.cut_Date.Value.Year * 10000 + invParameterBalance.cut_Date.Value.Month * 100 + invParameterBalance.cut_Date.Value.Day)
                        : null,
                    id_productionCart = invParameterBalance.id_productionCart,


                }, System.Data.CommandType.StoredProcedure);
            }
            else
            {
                var monthlyBalanceResult = DapperConnection.Execute<InventoryBalanceModel>("[dbo].[inv_Consulta_Saldo_Mes_Control_General]", new
                {

                    id_company = invParameterBalance.id_company,
                    id_Item = (object)invParameterBalance.id_Item ?? DBNull.Value,
                    fecha_corte = invParameterBalance.cut_Date,
                    consolidado = invParameterBalance.consolidado,
                    groupby = invParameterBalance.groupby,
                    id_Warehouse = (object)invParameterBalance.id_Warehouse ?? DBNull.Value,
                    id_WarehouseLocation = (object)invParameterBalance.id_WarehouseLocation ?? DBNull.Value,
                    id_ProductionLot = (object)invParameterBalance.id_ProductionLot ?? DBNull.Value,
                    requiresLot = invParameterBalance.requiresLot,
                }, System.Data.CommandType.StoredProcedure);

                if (modelSaldoProductlote)
                {
                    saldoProductoLote = monthlyBalanceResult?.Select(r => new SaldoProductoLote
                    {
                        id_item = r.id_item,
                        id_lote = r.id_productionLot,
                        saldo = (r.SaldoActual ?? 0),
                        lot_market = null,
                        number = r.numberLot,
                        internalNumber = r.internalNumberLot,
                        id_metricUnit = (r.id_metricUnit ?? 0),
                        receptionDate = r.fechaRecepcion,

                    })?.ToArray();
                }
                else
                {
                    monthlyBalance = monthlyBalanceResult?.Select(r => new MonthlyBalance
                    {
                        id_item = r.id_item,
                        id_productionLot= r.id_productionLot,
                        SaldoActual= (r.SaldoActual ?? 0),
                        sequencial_productionLot = r.numberLot,
                        number_productionLot = r.internalNumberLot,
                        id_metric_unit= (r.id_metricUnit ?? 0) ,
                        id_warehouse = r.id_warehouse,
                        id_warehouseLocation = r.id_warehouseLocation,
                    })?.ToArray();
                }
    
            }

            return new Tuple<MonthlyBalance[], SaldoProductoLote[]>(monthlyBalance, saldoProductoLote);

        }

        public static MonthlyBalance ValidateBalance(DBContext db, InvParameterBalance invParameterBalance)
        {

            db.Database.CommandTimeout = 2200;
            MonthlyBalance monthlyBalance = new MonthlyBalance();

            var monthlyBalanceResult = DapperConnection.Execute<MonthlyBalance>("inv_Consulta_Saldo_Mes_Control", new
            {
                id_company = invParameterBalance.id_company,
                id_Item = invParameterBalance.id_Item,
                fecha_corte = invParameterBalance.cut_Date,

                consolidado = invParameterBalance.consolidado,
                id_Warehouse = (object)invParameterBalance.id_Warehouse ?? DBNull.Value,
                id_WarehouseLocation = (object)invParameterBalance.id_WarehouseLocation ?? DBNull.Value,
                id_ProductionLot = (object)invParameterBalance.id_ProductionLot ?? DBNull.Value
            }, System.Data.CommandType.StoredProcedure);

            if ((monthlyBalanceResult?.Length ?? 0) == 0)
            {
                throw new Exception("No existen datos o el Saldo Actual del Producto es Cero");
            }
            monthlyBalance = monthlyBalanceResult.FirstOrDefault();

            if (monthlyBalance.SaldoActual == 0)
            {
                throw new Exception("No existen datos o el Saldo Actual del Producto es Cero");
            }


            return monthlyBalance;
        }

        public static MonthlyBalanceControl[] BalanceControlActualizar(int id_company, MonthlyBalanceControl[] bodegaPeriodos)
        {
            var warehouses = bodegaPeriodos
                                    .GroupBy(r => r.id_warehouse)
                                    .Select(r => r.Key)
                                    .ToArray();

            string SELECT_MONTHLY_BALANCE_CONTROL = "SELECT * FROM MonthlyBalanceControl " +
                                                    " WHERE id_company = @id_company AND id_warehouse in( @id_warehouses);";

            var result = Dapper.DapperConnection.Execute<MonthlyBalanceControl>(SELECT_MONTHLY_BALANCE_CONTROL, new
            {
                id_company = id_company,
                id_warehouses = warehouses
            });
            
            return (from bodegaApertura in bodegaPeriodos
                    join bodega in result
                    on new { bodega = bodegaApertura.id_warehouse, anio = bodegaApertura.Anio, mes = bodegaApertura.Mes }
                    equals new { bodega = bodega.id_warehouse, anio = bodega.Anio, mes = bodega.Mes }
                    select new MonthlyBalanceControl
                    {
                        id = bodega.id,
                        id_company = bodega.id_company,
                        Anio = bodega.Anio,
                        Mes = bodega.Mes,
                        id_warehouse = bodega.id_warehouse,
                        //IsValid = (bodegaApertura.LastDateProcess.Value.Day == 1 ) ? false: true,
                        DateIsNotValid = DateTime.Now,
                        //LastDateProcess = (bodegaApertura.LastDateProcess.Day == 1) ? bodegaApertura.LastDateProcess : bodegaApertura.LastDateProcess.AddDays(-1)

                    }).ToArray();

        }
        //public static  void UpdateMonthlyBalanceControlNoValid(MonthlyBalanceControl monthlyBalanceControl, SqlConnection connection, SqlTransaction transaction)
        //{
        //    string UPDATE_MONTHLY_BALANCE_CONTROL = "update MonthlyBalanceControl set IsValid = 0, DateIsNotValid = @DateIsNotValid " +
        //                                            " WHERE id_company = @id_company AND id_warehouse = @id_warehouse AND Anio = @Anio "+
        //                                            " AND Mes = @Mes; ";
        //
        //    connection.Execute(UPDATE_MONTHLY_BALANCE_CONTROL, new
        //    {
        //        DateIsNotValid = monthlyBalanceControl.DateIsNotValid,
        //        id_company = monthlyBalanceControl.id_company,
        //        id_warehouse = monthlyBalanceControl.id_warehouse,
        //        Anio = monthlyBalanceControl.Anio,
        //        Mes = monthlyBalanceControl.Mes
        //
        //    }, transaction);
        //}

        #region  -- Approach 2 --
        internal static bool ValidateData(MonthlyBalanceProcessFilterDto dataToProcess, ref string mensaje)
        {
            bool respuesta = false;

            if (dataToProcess == null)
            {
                mensaje = "Data para procesar incompleta.";
                return respuesta;
            }

            if (string.IsNullOrEmpty(dataToProcess.codePeriod)
                || string.IsNullOrWhiteSpace(dataToProcess.codePeriod))
            {
                mensaje = "Debe seleccionar un periodo.";
                return respuesta;
            }
             
            respuesta = true;
            return respuesta;
        }


        internal static MonthlyBalanceProcessMessageDto ConvertToParameter(MonthlyBalanceProcessFilterDto dataToProcess, ref string message)
        {
            MonthlyBalanceProcessMessageDto objToSend = null;
            try
            {

                objToSend = new MonthlyBalanceProcessMessageDto();

                if (!(string.IsNullOrEmpty(dataToProcess.codePeriod) ||
                    string.IsNullOrWhiteSpace(dataToProcess.codePeriod)))
                {
                    var arCode = dataToProcess.codePeriod.Split('-');
                    objToSend.anio = Convert.ToInt32(arCode[0]);
                    objToSend.mes = Convert.ToInt32(arCode[1]);
                    objToSend.codigoTipoPeriodo = arCode[2];
                    objToSend.descripcionTipoPeriodo = arCode[3];
                }
                objToSend.idProcess = "";
                objToSend.idUser = dataToProcess.idUser;
                objToSend.isMassive = dataToProcess.isMassive ?? false;
                objToSend.idWarehouse = dataToProcess.idWarehouse;
                objToSend.idWarehouseLocation = dataToProcess.idWarehouseLocation;
                objToSend.idItem = dataToProcess.idItem;
                objToSend.id_company = dataToProcess.idCompany;

            }
            catch (Exception)
            {
                message = "Se produjo un error al parsear mensaje.";
                return null;
            }

            return objToSend;
        }

        internal static bool ValidatePeriodoControl( DBContext db, MonthlyBalanceProcessMessageDto dataToProcess, ref string mensaje)
        {
            const string CODIGO_PARAMETRO_SALDO_INICIAL = "PSALINI";
            const string sentencia_MonthlyBalanceControl_Estado_PeriodoBodega = "select id,id_company,id_warehouse,Anio,Mes,IsValid,DateIsNotValid from MonthlyBalanceControl where id_company = @id_company AND  Anio = @Anio AND Mes = @Mes;";

            List<int> bodegasIds = new List<int>();
            string nombreBodogaParametro = null;
            if (dataToProcess.idWarehouse.HasValue)
            {
                bodegasIds.Add(dataToProcess.idWarehouse.Value);
                nombreBodogaParametro = db.Warehouse
                                                .FirstOrDefault(w => w.id == dataToProcess.idWarehouse.Value)?.name;
            }
            else
            {
                bodegasIds = db.Warehouse
                                    .Where(w => w.id_company == dataToProcess.id_company && w.isActive == true)
                                    .Select(r => r.id)
                                    .ToList();
            }

            var previewPeriod = ServiceInventoryBalance.GetPreviewPeriod(dataToProcess.anio, dataToProcess.mes);
            int anioPrev = previewPeriod.Item1;
            int mesPrev = previewPeriod.Item2;

            var settingSaldoInicial = db.Setting.FirstOrDefault(r => r.code == CODIGO_PARAMETRO_SALDO_INICIAL);
            if (settingSaldoInicial == null)
            {
                mensaje = $"No se ha definido el parámetro: {CODIGO_PARAMETRO_SALDO_INICIAL}";
                return false;
            }
            int anio = 0;
            int mes = 0;
            int.TryParse(settingSaldoInicial.value.Substring(0, 4), out anio);
            int.TryParse(settingSaldoInicial.value.Substring(4, 2), out mes);

            if (dataToProcess.anio == anio && dataToProcess.mes == mes) return true;

            var control = DapperConnection.Execute<MonthlyBalanceControl>(sentencia_MonthlyBalanceControl_Estado_PeriodoBodega, new
            {
                id_company = dataToProcess.id_company,
                Anio = anioPrev,
                Mes = mesPrev
            });

            if ((control?.Length ?? 0) == 0)
            {
                string mensajePredicado = (string.IsNullOrEmpty(nombreBodogaParametro) ? "para todas las bodega" : $"para la bodega {nombreBodogaParametro}");
                mensaje = $"No se ha procesado el periodo Anio Mes {anioPrev}/{mesPrev} {mensajePredicado}";
                return false;
            }

            int[] bodegasProcesadasIds = control.Where(r => r.IsValid).Select(r => r.id_warehouse).ToArray();
            var bodegaNoProcesadasIds = bodegasIds
                                            .Where(r => !bodegasProcesadasIds.Contains(r))
                                            .ToArray();

            if ((bodegaNoProcesadasIds?.Length ?? 0) > 0)
            {
                var warehouses = db.Warehouse
                                                .Where(r => bodegaNoProcesadasIds.Contains(r.id))
                                                .Select(r => r.name)
                                                .ToArray();

                //string bodegasNombres = warehouses
                //                                .Aggregate((i, j) => $"{i},{j}");

                string bodegasNombres = null;

                mensaje = $"No se ha procesado el periodo Anio/Mes: {anioPrev}/{mesPrev}, para la(s) bodega(s): {bodegasNombres}";
                return false;
            }

            return true;
        }
        #endregion


        #region  -- Funciones Control Saldo --
        internal static MonthlyBalanceControl GetMonthlyBalanceControl(int id_company, int id_warehouse, int anio, int mes )
        {
            string SELECT_MONTHLY_BALANCE_CONTROL = "SELECT * FROM MonthlyBalanceControl " +
                                                    " WHERE id_company = @id_company AND id_warehouse = @id_warehouse and Anio = @Anio AND  Mes = @Mes;";

            var result = Dapper.DapperConnection.Execute<MonthlyBalanceControl>(SELECT_MONTHLY_BALANCE_CONTROL, new
            {
                id_company = id_company,
                id_warehouse = id_warehouse,
                Anio = anio,
                Mes = mes
            });

            return result?.FirstOrDefault();

        }
        internal static MonthlyBalanceControl[] GetMonthlyBalanceControlAnio(int id_company, int id_warehouse, int anio)
        {
            string SELECT_MONTHLY_BALANCE_CONTROL = "SELECT * FROM MonthlyBalanceControl " +
                                                    " WHERE id_company = @id_company AND id_warehouse = @id_warehouse and Anio = @Anio;";

            var result = Dapper.DapperConnection.Execute<MonthlyBalanceControl>(SELECT_MONTHLY_BALANCE_CONTROL, new
            {
                id_company = id_company,
                id_warehouse = id_warehouse,
                Anio = anio,
            });

            return result;

        }
        #endregion


    }
}