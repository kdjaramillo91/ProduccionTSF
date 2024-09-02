using Dapper;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Operations;
using EntidadesAuxiliares.SQL;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class CostPerWarehouseProductController : DefaultController
    {        
        private const string m_CostPerWarehouseProductModelKey = "costPerWarehouseProduct";
        private const string ExcelXlsxMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const int m_MaxRecorridos = 5;

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Procesamos los detales
        [HttpPost]
        public JsonResult Process(DateTime fecha, string tipo)
        {
            string message;
            bool isValid;

            try
            {
                if(tipo == "SALDO")
                {
                    this.TempData[m_CostPerWarehouseProductModelKey] = this.CalcularSaldos(fecha);
                    this.TempData.Keep(m_CostPerWarehouseProductModelKey);
                }
                else if(tipo == "COSTO")
                {
                    this.TempData[m_CostPerWarehouseProductModelKey] = this.ProcesarValorizacionDatos(fecha);
                    this.TempData.Keep(m_CostPerWarehouseProductModelKey);
                }
                else
                {
                    this.TempData[m_CostPerWarehouseProductModelKey] = new object[] { };
                    this.TempData.Keep(m_CostPerWarehouseProductModelKey);
                }

                message = "Movimientos valorizados exitosamente.";
                isValid = true;
            }
            catch (Exception exception)
            {
                message = "Error al calcular la valorización: " + exception.GetBaseException()?.Message ?? string.Empty;
                isValid = false;
            }

            var result = new
            {
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Exportación de excel

        [HttpGet]
        public FileContentResult GenerateExcel(string tipo)
        {
            dynamic detalles;

            if (tipo == "SALDO")
            {
                detalles = (this.TempData[m_CostPerWarehouseProductModelKey] as SaldoProductoBodega[]);
            }
            else if (tipo == "COSTO")
            {
                detalles = (this.TempData[m_CostPerWarehouseProductModelKey] as DataProcesada[]);
            }
            else
            {
                detalles = new object[] { };
            }

            byte[] pdfBuffer;
            if (tipo == "SALDO")
            {
                using (var outputStream = new System.IO.MemoryStream())
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Resultado");
                        ws.Cells["A1"].LoadFromCollection(Collection: detalles, PrintHeaders: true);
                        ws.Cells.AutoFitColumns();
                        var range = ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column];
                        var tab = ws.Tables.Add(range, "Resultado");
                        tab.TableStyle = TableStyles.Light1;

                        pck.SaveAs(outputStream);
                    }

                    pdfBuffer = outputStream.ToArray();
                }
            }
            else
            {
                pdfBuffer = this.GenerateExcelDynamic(detalles);
            }

            return new FileContentResult(pdfBuffer, ExcelXlsxMime)
            {
                FileDownloadName = $"CostPerWarehouseProduct.xlsx",
            };
        }

        private class Criterios
		{
            public string Titulo { get; set; }
            public decimal Libras { get; set; }
            public decimal Dolares { get; set; }

            public Criterios(string titulo, decimal libras, decimal dolares)
			{
                this.Titulo = titulo;
                this.Libras = libras;
                this.Dolares = dolares;
            }
        }

        private byte[] GenerateExcelDynamic(DataProcesada[] detalles)
        {
            byte[] pdfBuffer;

            // Obtención de datos de archivo
            var fechaCorte = detalles.FirstOrDefault().FechaEmision;
            var fechaProceso = DateTime.Now.ToDateTimeFormat();

            // Obtención de totales de costo y libras por filtro
            var totalSaldoInicial = detalles.Where(x => x.Motivo == "SALDO INICIAL").Sum(e => e.CantidadLibras);
            var totalSaldoInicial_costo = detalles.Where(x => x.Motivo == "SALDO INICIAL").Sum(e => e.CostoTotalLibras);
            var totalIngCompraMPAut = detalles.Where(x => x.Motivo == "Ingreso Compra Materia Prima Automático").Sum(e => e.CantidadLibras);
            var totalIngCompraMPAut_costo = detalles.Where(x => x.Motivo == "Ingreso Compra Materia Prima Automático").Sum(e => e.CostoTotalLibras);
            var totalIngAjusteInvPT = detalles.Where(x => x.Motivo == "Ingreso por Ajuste Inventario Producto Terminado").Sum(e => e.CantidadLibras);
            var totalIngAjusteInvPT_costo = detalles.Where(x => x.Motivo == "Ingreso por Ajuste Inventario Producto Terminado").Sum(e => e.CostoTotalLibras);
            var totalCostoVentas = detalles
                .Where(x =>
                    x.Motivo == "Egreso por Ventas locales" ||
                    x.Motivo == "Egreso por Venta Exportaciones " ||
                    x.Motivo == "Egreso por Muestra / Obsequio")
                .Sum(e => e.CantidadLibras);
            var totalCostoVentas_costo = detalles.Where(x => x.Motivo == "Ingreso por Ajuste Inventario Producto Terminado").Sum(e => e.CostoTotalLibras);

            var totalSumaPositivos = detalles.Where(x => x.CantidadLibras > 0).Sum(e => e.CantidadLibras);
            var totalSumaPositivos_costo = detalles.Where(x => x.CostoTotalLibras > 0).Sum(e => e.CostoTotalLibras);

            var totalSumaNegativos = detalles.Where(x => x.CantidadLibras < 0).Sum(e => e.CantidadLibras);
            var totalSumaNegativos_costo = detalles.Where(x => x.CostoTotalLibras < 0).Sum(e => e.CostoTotalLibras);

            var totalIngresosTransferencias = totalSumaPositivos - totalSaldoInicial - totalIngCompraMPAut - totalIngAjusteInvPT;
            var totalIngresosTransferencias_costo = totalSumaPositivos_costo - totalSaldoInicial_costo - totalIngCompraMPAut_costo - totalIngAjusteInvPT_costo;

            var totalEgresosTransferencias = totalSumaNegativos - totalCostoVentas;
            var totalEgresosTransferencias_costo = totalSumaNegativos_costo - totalCostoVentas_costo;

            var inventarioFinal = (totalSaldoInicial + totalIngCompraMPAut + totalIngAjusteInvPT + totalCostoVentas) * (-1);
            var inventarioFinal_costo = (totalSaldoInicial_costo + totalIngCompraMPAut_costo + totalIngAjusteInvPT_costo + totalCostoVentas_costo) * (-1);

            var criterios = new[]
            {
                new Criterios("SALDO INICIAL", totalSaldoInicial, totalSaldoInicial_costo ),
                new Criterios("COMPRAS", totalIngCompraMPAut, totalIngCompraMPAut_costo ),
                new Criterios("Subtotal 1", totalSaldoInicial + totalIngCompraMPAut, totalSaldoInicial_costo + totalIngCompraMPAut_costo ),
                new Criterios(" ", 0, 0 ),
                new Criterios("INVENTARIO FINAL", inventarioFinal, inventarioFinal_costo ),
                new Criterios("COSTO DE VENTAS", totalCostoVentas, totalCostoVentas_costo),
                new Criterios("AJUSTE INVENTARIO", totalIngAjusteInvPT, totalIngAjusteInvPT_costo),
                new Criterios("Subtotal 2", inventarioFinal + totalCostoVentas + totalIngAjusteInvPT, inventarioFinal_costo + totalCostoVentas_costo + totalIngAjusteInvPT_costo),

            };


            using (var outputStream = new System.IO.MemoryStream())
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage())
                {

                    // Agregar hojas al documento de Excel
                    ExcelWorksheet ws_resumen = pck.Workbook.Worksheets.Add("Resumen");
                    ExcelWorksheet ws_detalle = pck.Workbook.Worksheets.Add("Detalle");
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Resultado");

                    // Agregar datos a las hojas
                    #region Tabla Resumen
                    // Hoja Resumen de resultados totales

                    ws_resumen.Cells["E1"].Value = "PROEXPO";
                    ws_resumen.Cells["E2"].Value = "CUADRATURA DE LIBRAS PANACEA";
                    ws_resumen.Cells["E3"].Value = "Fecha de Corte: ";
                    ws_resumen.Cells["F3"].Value = fechaCorte;

                    ws_resumen.Cells["E4"].Value = "Fecha Proceso: ";
                    ws_resumen.Cells["F4"].Value = fechaProceso;

                    ws_resumen.Cells["A1"].Value = "SEGUN AUDITORIA";
                    ws_resumen.Cells["A1:C1"].Merge = true;
                    ws_resumen.Cells["A1:C1"].Style.Font.Bold = true;
                    ws_resumen.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws_resumen.Cells["A1:C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A1:C1"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                    ws_resumen.Cells["A2"].Value = "BODEGAS";
                    ws_resumen.Cells["B2"].Value = "LIBRAS";
                    ws_resumen.Cells["C2"].Value = "DOLARES";
                    ws_resumen.Cells["A2:C2"].Style.Font.Bold = true;
                    ws_resumen.Cells["A2:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(Color.Black);
                    ws_resumen.Cells["A2:C2"].Style.Font.Color.SetColor(Color.White);


                    int numInicio = 3;
					for (int i = 0; i < criterios.Length; i++)
					{
                        numInicio++;
                        var criterio = criterios[i];
                        ws_resumen.Cells[$"A{numInicio}"].Value = criterio.Titulo;
                        ws_resumen.Cells[$"B{numInicio}"].Value = criterio.Libras;
                        ws_resumen.Cells[$"C{numInicio}"].Value = criterio.Dolares;

                    }

                    ws_resumen.Cells["A12"].Value = "Diferencia";
                    ws_resumen.Cells["B12"].Value = (totalSaldoInicial + totalIngCompraMPAut) + (inventarioFinal + totalCostoVentas + totalIngAjusteInvPT);
                    ws_resumen.Cells["C12"].Value = (totalSaldoInicial_costo + totalIngCompraMPAut_costo) + (inventarioFinal_costo + totalCostoVentas_costo + totalIngAjusteInvPT_costo);
                    ws_resumen.Cells["A12:C12"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A12:C12"].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);


                    ws_resumen.Cells.AutoFitColumns();

                    #endregion

                    #region Tabla resultado

                    ws.Cells["A1"].LoadFromCollection(Collection: detalles, PrintHeaders: true);
                    ws.Cells.AutoFitColumns();

                    #endregion

                    #region Tabla Dinamica (Detalles)
                    var dataRange = ws.Cells[ws.Dimension.Address];

                    // Crear la tabla dinámica
                    var pivotTable = ws_detalle.PivotTables.Add(ws_detalle.Cells["A3"], dataRange, "PivotTable");


                    // Campos descriptivos
                    pivotTable.RowFields.Add(pivotTable.Fields["NombreBodega"]);
                    pivotTable.RowFields.Add(pivotTable.Fields["Motivo"]);
                    pivotTable.RowFields.Add(pivotTable.Fields["NombreItem"]);
                    pivotTable.DataOnRows = false;

                    // Campos de datos
                    ExcelPivotTableDataField cantidadField = pivotTable.DataFields.Add(
                        pivotTable.Fields["CantidadLibras"]
                    );
                    cantidadField.Function = DataFieldFunctions.Sum;
                    cantidadField.Name = "Cantidad";

                    ExcelPivotTableDataField costoLibrasField = pivotTable.DataFields.Add(
                        pivotTable.Fields["CostoTotalLibras"]
                    );
                    costoLibrasField.Function = DataFieldFunctions.Sum;
                    costoLibrasField.Name = "Costo Total Libras";

                    #endregion                    

                    pck.SaveAs(outputStream);
                }

                pdfBuffer = outputStream.ToArray();
            }
            return pdfBuffer;
        }
        #endregion

        #region Procesamiento de datos
        private SaldoProductoBodega[] CalcularSaldos(DateTime fecha)
        {
            var saldosProductoBodega = new List<SaldoProductoBodega>();

            var fechaInicio = new DateTime(fecha.Year, fecha.Month, 1);
            var fechaMesAnt = fechaInicio.AddDays(-1);
            var saldoIniciales = this.GetSaldosIniciales(fechaMesAnt.Year, fechaMesAnt.Month);

            // Actualizamos la data de productos a procesar en caso de ser necesario
            this.PoblarMovimientosProcesar(fecha);

            var movimientosCorte = new List<Movimiento>();
            var fechaPivote = fechaInicio;
            while (fechaPivote <= fecha)
            {
                movimientosCorte.AddRange(this.RecuperarMovimientosFecha(fechaPivote.ToIntegerDate()));

                fechaPivote = fechaPivote.AddDays(1);
            }

            var agrupacionesProcesar = movimientosCorte
                .GroupBy(e => new {
                    e.IdBodega,
                    e.IdItem,
                })
                .Select(e => new {
                    e.Key.IdBodega,
                    e.Key.IdItem,
                    FechasMovimientos = e.Select(x => x.FechaMovimiento).Distinct()
                        .OrderBy(x => x)
                        .ToArray()
                })
                .ToArray();

            foreach (var agrupacionProcesar in agrupacionesProcesar)
            {
                var bodega = this.GetWarehouseFromTempData(agrupacionProcesar.IdBodega);
                var item = this.GetItemFromTempData(agrupacionProcesar.IdItem);

                var saldoInicial = saldoIniciales
                    .FirstOrDefault(e => e.idBodega == agrupacionProcesar.IdBodega
                        && e.idItem == agrupacionProcesar.IdItem);
                var cantidadInicial  = saldoInicial?.cantidad ?? 0m;

                if(saldoInicial != null)
                {
                    saldosProductoBodega.Add(new SaldoProductoBodega()
                    {
                        FechaSaldoDt = fechaMesAnt,
                        FechaSaldo = fechaMesAnt.ToDateFormat(),
                        Bodega = bodega.name,
                        Item = $"{item.masterCode} - {item.name}",
                        EntradaDia = cantidadInicial,
                        SaldoAnterior = 0,
                        SalidaDia = 0,
                        SaldoDia = cantidadInicial,
                        SaldoFinal = cantidadInicial,
                        IdsInventoryMoveDetails = string.Empty,
                    });
                }

                foreach (var fechaMovimiento in agrupacionProcesar.FechasMovimientos)
                {
                    var movimientos = movimientosCorte
                        .Where(e => e.IdBodega == agrupacionProcesar.IdBodega
                            && e.IdItem == agrupacionProcesar.IdItem
                            && e.FechaMovimiento == fechaMovimiento)
                        .ToArray();
                    var cantidad = movimientos.Sum(e => e.Cantidad);
                    var entrada = movimientos
                        .Where(e => e.Cantidad > 0)
                        .Sum(e => e.Cantidad);

                    var salida = movimientos
                        .Where(e => e.Cantidad < 0)
                        .Sum(e => e.Cantidad);

                    var saldoAnterior = cantidadInicial;
                    cantidadInicial += movimientos.Sum(e => e.Cantidad);
                    var idsMovimientos = movimientos.Select(e => e.IdInventoryMoveDetail).ToArray();

                    saldosProductoBodega.Add(new SaldoProductoBodega()
                    {
                        FechaSaldoDt = fechaMovimiento.ToDateInteger(),
                        FechaSaldo = fechaMovimiento.ToDateInteger().ToDateFormat(),
                        Bodega = bodega.name,
                        Item = $"{item.masterCode} - {item.name}",
                        SaldoAnterior = saldoAnterior,
                        EntradaDia = entrada,
                        SalidaDia = salida,
                        SaldoDia = entrada - Math.Abs(salida),
                        SaldoFinal = cantidadInicial,
                        IdsInventoryMoveDetails = string.Join(",", idsMovimientos),
                    });
                }
            }

            return saldosProductoBodega
                .OrderBy(e => e.Bodega)
                .ThenBy(e => e.Item)
                .ThenBy(e => e.FechaSaldoDt)
                .ToArray();
        }


        private DataProcesada[] ProcesarValorizacionDatos(DateTime fecha)
        {
            var retorno = new List<DataProcesada>();
            var fechaInicio = new DateTime(fecha.Year, fecha.Month, 1);
            var fechaMesAnt = fechaInicio.AddDays(-1);
            var saldoIniciales = this.GetSaldosIniciales(fechaMesAnt.Year, fechaMesAnt.Month);

            // Actualizamos la data de productos a procesar en caso de ser necesario
            this.PoblarMovimientosProcesar(fecha);

            // Procesaremos los movimientos
            var fechaPivote = fechaInicio;
            while (fechaPivote <= fecha)
            {
                var movimientos = this.RecuperarMovimientosFecha(fechaPivote.ToIntegerDate())
                    .Where(e => !e.Procesado)
                    .ToList();
                int numRecorridos = 0;

                try
                {
                    this.CalcularCostos(movimientos, ref numRecorridos);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fecha: {fechaPivote.ToDateFormat()}. {ex.GetBaseException().Message}");
                }

                fechaPivote = fechaPivote.AddDays(1);
            }


            // Conversión de saldos
            retorno.AddRange(this.ConvertToDTO(fechaMesAnt, saldoIniciales));

            // Recuperamos las transacciones ya procesadas
            fechaPivote = fechaInicio;
            while (fechaPivote <= fecha)
            {
                var movimientos = this.RecuperarMovimientosFecha(fechaPivote.ToIntegerDate());
                retorno.AddRange(this.ConvertToDTO(movimientos));

                fechaPivote = fechaPivote.AddDays(1);
            }

            return retorno
                .ToArray();
        }

        private void CalcularCostos(List<Movimiento> movimientos, ref int numRecorridos)
        {
            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            numRecorridos += 1;

            // Procesamos cada movimiento
            //foreach (var movimiento in movimientos)
            for (int i = 0; i < movimientos.Count; i++)
            {
                var movimiento = movimientos[i];

                // Si el movimiento ya fue procesado, no hacemos nada
                if (movimiento.Procesado) continue;

                decimal cantidadActual = 0m, costoActual = 0m, costoPromedioActual = 0m, costoMovimiento = 0m;

                // Obtenemos el saldo Actual
                var saldoActual = this.ObtenerSaldoActual(movimiento.IdBodega, movimiento.IdItem);

                costoMovimiento = movimiento.Cantidad * saldoActual.CostoPromedio;
                // comprobamos que el movimiento no deje en negativo la bodega
                cantidadActual = saldoActual.Cantidad + movimiento.Cantidad;
                costoActual = saldoActual.CostoTotal + costoMovimiento;
                costoPromedioActual = cantidadActual > 0
                    ? Math.Round(costoActual / cantidadActual, 6, MidpointRounding.AwayFromZero)
                    : 0m;

                // Si la el producto bodega no queda en negativo en los saldos y el costo entrante es diferente de 0 se procesa
                if (cantidadActual >= 0 && Math.Abs(costoMovimiento) > 0)
                {
                    using (var db = new SqlConnection(dapperDBContext))
                    {
                        db.Open();

                        using (var tr = db.BeginTransaction())
                        {
                            try
                            {
                                // Actualizamos el movimiento
                                var costoUnitario = Math
                                    .Abs(GlobalCalculator.RedondearMontoMayorPrecision(costoMovimiento / movimiento.Cantidad));

                                MovimientoDapper.Actualizar(db, tr,
                                    movimiento.IdInventoryMoveDetail, costoUnitario, costoMovimiento, true);

                                // Procesaremos el saldo del movimiento
                                if (saldoActual.IdBodega != 0)
                                {
                                    SaldoActualDapper.Actualizar(db, tr,
                                        movimiento.IdBodega, movimiento.IdItem, cantidadActual, costoPromedioActual, costoActual);
                                }
                                else
                                {
                                    SaldoActualDapper.Actualizar(db, tr,
                                        movimiento.IdBodega, movimiento.IdItem, cantidadActual, costoPromedioActual, costoActual);
                                }

                                tr.Commit();
                            }
                            catch (Exception ex)
                            {
                                tr.Rollback();
                                throw ex;
                            }
                        }

                        db.Close();
                    }

                    // Marcamos como procesamiento
                    movimientos[i].Procesado = true; 
                }
            }

            // Procesamos los movimeientos omitidos
            if(movimientos.Any(e => !e.Procesado))
            {
                // Verificamos que el núm de elementos a procesar aún es válido
                if (numRecorridos >= m_MaxRecorridos)
                {
                    return;
                    //throw new Exception("Límite de intentos de reprocesamiento alcanzado.");
                }

                // Si aún se puede procesar, continuamos
                this.CalcularCostos(movimientos, ref numRecorridos);
            }
        }
        #endregion

        #region Clases adicionales
        private class DataProcesada
        {
            public int? IdInventoryMoveDetail { get; set; }
            public string FechaEmision { get; set; }
            public string Motivo { get; set; }
            public int IdBodega { get; set; }
            public string CodigoBodega { get; set; }
            public string NombreBodega { get; set; }
            public int IdItem { get; set; }
            public string NombreItem { get; set; }
            public decimal CantidadMaster { get; set; }
            public decimal PrecioUnitarioMaster { get; set; }
            public decimal CostoTotalMaster { get; set; }
            public decimal CantidadLibras { get; set; }
            public decimal PrecioUnitarioLibras { get; set; }
            public decimal CostoTotalLibras { get; set; }
            public decimal CantidadKilos { get; set; }
            public decimal PrecioUnitarioKilos { get; set; }
            public decimal CostoTotalKilos { get; set; }
            public string Procesado { get; set; }
        }

        private class Movimiento
        {
            public int IdInventoryMoveDetail { get; set; }
            public int FechaMovimiento { get; set; }
            public int IdMotivo { get; set; }
            public int IdBodega { get; set; }
            public int IdItem { get; set; }
            public decimal Cantidad { get; set; }
            public decimal CostoUnitario { get; set; }
            public decimal CostoTotal { get; set; }
            public bool Procesado { get; set; }
        }

        private class SaldoActual
        {
            public int IdBodega { get; set; }
            public int IdItem { get; set; }
            public decimal Cantidad { get; set; }
            public decimal CostoPromedio { get; set; }
            public decimal CostoTotal { get; set; }
        }
        private class SaldoProductoBodega
        {
            public DateTime FechaSaldoDt { get; set; }

            [Description("Bodega")]
            public string Bodega { get; set; }

            [Description("Ítem")] 
            public string Item { get; set; }

            [Description("Fecha de Saldo")]
            public string FechaSaldo { get; set; }

            [Description("Saldo Anterior")]
            public decimal SaldoAnterior { get; set; }

            [Description("Ingresos Master/Cajas Día")]
            public decimal EntradaDia { get; set; }

            [Description("Salida Master/Cajas Día")]
            public decimal SalidaDia { get; set; }

            [Description("Saldo Master/Cajas Día")]
            public decimal SaldoDia { get; set; }

            [Description("Saldo Stock Final")]
            public decimal SaldoFinal { get; set; }
            public string IdsInventoryMoveDetails { get; set; }
        }
        #endregion

        #region Funciones Adicionales
        private SaldoInicial[] GetSaldosIniciales(int año, int mes)
        {
            return db.SaldoInicial
                //.Where(e => e.año == año && e.mes == mes && e.activo)
                .ToArray();
        }
        private void PoblarMovimientosProcesar(DateTime fechaProcesar)
        {
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();

                var parameters = new { año = fechaProcesar.Year, mes = fechaProcesar.Month, dia = fechaProcesar.Day };
                cnn.Execute("inv_PoblarDataInventario", parameters, commandType: CommandType.StoredProcedure);

                cnn.Close();
            }
        }
        private Movimiento[] RecuperarMovimientosFecha(int fechaProcesar)
        {
            const string m_sql = "Select * from inv_MovimientosProcesar where fechaMovimiento = @fechaProcesar;";
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

            IEnumerable<Movimiento> retorno;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();
                retorno = cnn.Query<Movimiento>(m_sql, param:new { fechaProcesar });

                cnn.Close();
            }

            return retorno
                .ToArray();
        }
        private SaldoActual ObtenerSaldoActual(int idBodega, int idItem)
        {
            const string m_sql = "Select * from inv_SaldoActual where idBodega = @idBodega and idItem = @idItem;";
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

            SaldoActual saldo;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open(); ;
                saldo = cnn.QuerySingleOrDefault<SaldoActual>(m_sql, param: new { idBodega, idItem });
                cnn.Close();
            }

            return saldo ?? new SaldoActual();
        }
        #endregion

        #region Funciones Dapper
        private class MovimientoDapper
        {
            private const string m_TablaName = "inv_MovimientosProcesar";

            private const string m_IdInventoryMoveDetailName = "@idInventoryMoveDetail";
            private const string m_CostoUnitarioName = "@costoUnitario";
            private const string m_CostoTotalParamName = "@costoTotal";
            private const string m_ProcesadoParamName = "@procesado";

            public static void Actualizar(SqlConnection db, SqlTransaction tr,
                int idInventoryMoveDetail, decimal costoUnitario, decimal costoTotal,
                bool procesado)
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = $"UPDATE {m_TablaName} " +
                        $"SET   costoUnitario = {m_CostoUnitarioName}," +
                        $"      costoTotal = {m_CostoTotalParamName}," +
                        $"      procesado = {m_ProcesadoParamName} " +
                        $"WHERE idInventoryMoveDetail = {m_IdInventoryMoveDetailName};";

                    command.Parameters.Add(m_IdInventoryMoveDetailName, SqlDbType.Int).Value = idInventoryMoveDetail;
                    command.Parameters.Add(m_CostoUnitarioName, SqlDbType.Decimal).Value = costoUnitario;
                    command.Parameters.Add(m_CostoTotalParamName, SqlDbType.Decimal).Value = costoTotal;
                    command.Parameters.Add(m_ProcesadoParamName, SqlDbType.Bit).Value = procesado;

                    command.CommandType = CommandType.Text;
                    command.Connection = db;
                    command.Transaction = tr;
                    command.ExecuteNonQuery();
                }
            }
        }
        private class SaldoActualDapper
        {
            private const string m_TablaName = "inv_SaldoActual";

            private const string m_IdBodegaName = "@idBodega";
            private const string m_IdItemName = "@idItem";
            private const string m_CantidadName = "@cantidad";
            private const string m_CostoPromedioName = "@costoPromedio";
            private const string m_CostoTotalParamName = "@costoTotal";

            public static void Agregar(SqlConnection db, SqlTransaction tr,
               int idBodega, int idItem, decimal cantidad, decimal costoPromedio, decimal costoTotal)
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = $"Insert into {m_TablaName} " +
                        $"(idBodega, idItem, cantidad, costoPromedio, costoTotal)" +
                        $"Values {m_IdBodegaName}, {m_IdItemName}, {m_CantidadName}," +
                        $"      {m_CostoPromedioName}, {m_CostoTotalParamName};";

                    command.Parameters.Add(m_IdBodegaName, SqlDbType.Int).Value = idBodega;
                    command.Parameters.Add(m_IdItemName, SqlDbType.Int).Value = idItem;
                    command.Parameters.Add(m_CantidadName, SqlDbType.Decimal).Value = cantidad;
                    command.Parameters.Add(m_CostoPromedioName, SqlDbType.Decimal).Value = costoPromedio;
                    command.Parameters.Add(m_CostoTotalParamName, SqlDbType.Decimal).Value = costoTotal;

                    command.CommandType = CommandType.Text;
                    command.Connection = db;
                    command.Transaction = tr;
                    command.ExecuteNonQuery();
                }
            }
            public static void Actualizar(SqlConnection db, SqlTransaction tr,
               int idBodega, int idItem, decimal cantidad, decimal costoPromedio, decimal costoTotal)
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = $"UPDATE {m_TablaName} " +
                        $"SET   cantidad = {m_CantidadName}," +
                        $"      costoPromedio = {m_CostoPromedioName}," +
                        $"      costoTotal = {m_CostoTotalParamName} " +
                        $"WHERE idBodega = {m_IdBodegaName} and idItem = {m_IdItemName};";

                    command.Parameters.Add(m_IdBodegaName, SqlDbType.Int).Value = idBodega;
                    command.Parameters.Add(m_IdItemName, SqlDbType.Int).Value = idItem;
                    command.Parameters.Add(m_CantidadName, SqlDbType.Decimal).Value = cantidad;
                    command.Parameters.Add(m_CostoPromedioName, SqlDbType.Decimal).Value = costoPromedio;
                    command.Parameters.Add(m_CostoTotalParamName, SqlDbType.Decimal).Value = costoTotal;

                    command.CommandType = CommandType.Text;
                    command.Connection = db;
                    command.Transaction = tr;
                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Métodos de Conversion
        private DataProcesada[] ConvertToDTO(DateTime? fechaCorte, SaldoInicial[] saldos)
        {
            var unidadMedidaLibras = DataProviderMetricUnit.MetricUnitByCode("Lbs");
            var unidadMedidaKilos = DataProviderMetricUnit.MetricUnitByCode("Kg");

            var retorno = new List<DataProcesada>();
            for (int i = 0; i < saldos.Length; i++)
            {
                var saldo = saldos[i];
                var item = this.GetItemFromTempData(saldo.idItem);
                var presentacion = item.Presentation;
                var bodega = this.GetWarehouseFromTempData(saldo.idBodega);

                var factorPresentacion = presentacion != null
                   ? (decimal?)(presentacion.minimum * presentacion.maximum)
                   : null;

                var factorLibras = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaLibras.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaLibras.id)?.factor
                    : 1m;

                decimal cantidadLibras, precioUnitarioLibras, costoTotalLibras;
                if (factorLibras.HasValue && factorPresentacion.HasValue)
                {
                    cantidadLibras = saldo.cantidad * factorPresentacion.Value * factorLibras.Value;
                    costoTotalLibras = saldo.costo_total;
                    precioUnitarioLibras = cantidadLibras != 0m
                        ? Math.Abs(saldo.costo_total / cantidadLibras)
                        : 0m;
                }
                else
                {
                    cantidadLibras = precioUnitarioLibras = costoTotalLibras = 0m;
                }

                var factorKilos = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaKilos.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaKilos.id)?.factor
                    : 1m;
                decimal cantidadKilos, precioUnitarioKilos, costoTotalKilos;
                if (factorKilos.HasValue && factorPresentacion.HasValue)
                {
                    cantidadKilos = saldo.cantidad * factorPresentacion.Value * factorKilos.Value;
                    costoTotalKilos = saldo.costo_total;
                    precioUnitarioKilos = cantidadKilos != 0m
                        ? Math.Abs(saldo.costo_total / cantidadKilos)
                        : 0m;
                }
                else
                {
                    cantidadKilos = precioUnitarioKilos = costoTotalKilos = 0m;
                }

                // Retorno del proceso
                retorno.Add(new DataProcesada() 
                { 
                    FechaEmision = fechaCorte.ToDateFormat(),
                    Motivo = "Saldo Mes Anterior",
                    IdBodega = bodega.id,
                    CodigoBodega = bodega.code,
                    NombreBodega = bodega.name,
                    IdItem = item.id,
                    NombreItem = $"{item.masterCode} - {item.name}",

                    CantidadMaster = saldo.cantidad,
                    PrecioUnitarioMaster = saldo.costo_unitario,
                    CostoTotalMaster = saldo.costo_total,

                    CantidadLibras = cantidadLibras,
                    PrecioUnitarioLibras = precioUnitarioLibras,
                    CostoTotalLibras = costoTotalLibras,

                    CantidadKilos = cantidadKilos,
                    PrecioUnitarioKilos = precioUnitarioKilos,
                    CostoTotalKilos = costoTotalKilos,
                    Procesado = "SÍ",
                });
            }

            return retorno
                .ToArray();
        }
        private DataProcesada[] ConvertToDTO(Movimiento[] movimientos)
        {
            var unidadMedidaLibras = DataProviderMetricUnit.MetricUnitByCode("Lbs");
            var unidadMedidaKilos = DataProviderMetricUnit.MetricUnitByCode("Kg");

            var retorno = new List<DataProcesada>();
            for (int i = 0; i < movimientos.Length; i++)
            {
                var movimiento = movimientos[i];
                var item = this.GetItemFromTempData(movimiento.IdItem);
                var presentacion = item.Presentation;
                var bodega = this.GetWarehouseFromTempData(movimiento.IdBodega);

                var factorPresentacion = presentacion != null
                   ? (decimal?)(presentacion.minimum * presentacion.maximum)
                   : null;

                var factorLibras = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaLibras.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaLibras.id)?.factor
                    : 1m;

                decimal cantidadLibras, precioUnitarioLibras, costoTotalLibras;
                if (factorLibras.HasValue && factorPresentacion.HasValue)
                {
                    cantidadLibras = movimiento.Cantidad * factorPresentacion.Value * factorLibras.Value;
                    costoTotalLibras = movimiento.CostoTotal;
                    precioUnitarioLibras = cantidadLibras != 0m
                        ? Math.Abs(movimiento.CostoTotal / cantidadLibras)
                        : 0m;
                }
                else
                {
                    cantidadLibras = precioUnitarioLibras = costoTotalLibras = 0m;
                }

                var factorKilos = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaKilos.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaKilos.id)?.factor
                    : 1m;
                decimal cantidadKilos, precioUnitarioKilos, costoTotalKilos;
                if (factorKilos.HasValue && factorPresentacion.HasValue)
                {
                    cantidadKilos = movimiento.Cantidad * factorPresentacion.Value * factorKilos.Value;
                    costoTotalKilos = movimiento.CostoTotal;
                    precioUnitarioKilos = cantidadKilos != 0m
                        ? Math.Abs(movimiento.CostoTotal / cantidadKilos)
                        : 0m;
                }
                else
                {
                    cantidadKilos = precioUnitarioKilos = costoTotalKilos = 0m;
                }

                var motivo = this.GetInventoryReasonFromTempData(movimiento.IdMotivo);

                // Retorno del proceso
                retorno.Add(new DataProcesada() 
                { 
                    IdInventoryMoveDetail = movimiento.IdInventoryMoveDetail,
                    FechaEmision = movimiento.FechaMovimiento.ToDateInteger().ToDateFormat(),
                    Motivo = motivo.name,
                    IdBodega = bodega.id,
                    CodigoBodega = bodega.code,
                    NombreBodega = bodega.name,
                    IdItem = item.id,
                    NombreItem = $"{item.masterCode} - {item.name}",

                    CantidadMaster = movimiento.Cantidad,
                    PrecioUnitarioMaster = movimiento.CostoUnitario,
                    CostoTotalMaster = movimiento.CostoTotal,

                    CantidadLibras = cantidadLibras,
                    PrecioUnitarioLibras = precioUnitarioLibras,
                    CostoTotalLibras = costoTotalLibras,

                    CantidadKilos = cantidadKilos,
                    PrecioUnitarioKilos = precioUnitarioKilos,
                    CostoTotalKilos = costoTotalKilos,
                    Procesado = movimiento.Procesado ? "SÍ" : "NO",
                });
            }

            return retorno
                .ToArray();
        }
        #endregion

        #region Métodos de Memoria Temporal
        private Item GetItemFromTempData(int idItem)
        {
            var key = $"Item_{idItem}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as Item;
            }
            else
            {
                var warehouse = db.Item.FirstOrDefault(e => e.id == idItem);
                TempData[key] = warehouse;
                TempData.Keep(key);

                return warehouse;
            }
        }
        private Warehouse GetWarehouseFromTempData(int idWarehouse)
        {
            var key = $"WareHouse_{idWarehouse}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as Warehouse;
            }
            else
            {
                var warehouse = db.Warehouse.FirstOrDefault(e => e.id == idWarehouse);
                TempData[key] = warehouse;
                TempData.Keep(key);

                return warehouse;
            }
        }
        private MetricUnitConversion GetMetricUnitConversionFromTempData(int id_metricOrigin, int id_metricDestiny)
        {
            var key = $"MetricUnitConversion_{id_metricOrigin}_{id_metricDestiny}";
            MetricUnitConversion conversion;
            if (TempData.ContainsKey(key))
            {
                conversion = TempData[key] as MetricUnitConversion;
            }
            else
            {
                conversion = db.MetricUnitConversion
                    .FirstOrDefault(t => t.id_metricOrigin == id_metricOrigin &&
                        t.id_metricDestiny == id_metricDestiny && t.isActive);
            }

            TempData[key] = conversion;
            TempData.Keep(key);

            return conversion;
        }
        private InventoryReason GetInventoryReasonFromTempData(int idInventoryReason)
        {
            var key = $"InventoryReason_{idInventoryReason}";

            if (TempData.ContainsKey(key))
            {
                return TempData[key] as InventoryReason;
            }
            else
            {
                var inventoryReason = db.InventoryReason.FirstOrDefault(e => e.id == idInventoryReason);
                if (inventoryReason != null)
                {
                    TempData[key] = inventoryReason;
                    TempData.Keep(key);
                }
                return inventoryReason;
            }
        }
        #endregion
    }
}