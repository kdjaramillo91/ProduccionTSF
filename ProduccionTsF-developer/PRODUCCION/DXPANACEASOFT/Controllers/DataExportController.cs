using DXPANACEASOFT.Models;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Linq;

namespace DXPANACEASOFT.Controllers
{
    public class DataExportController : DefaultController
    {
        private const string m_Controller = "DataExport";

        private class Reporte
        {
            public TipoReporte TipoRpt { get; set; }
            public string CodigoReporte { get; set; }
            public string NombreReporte { get; set; }

            public Reporte(TipoReporte TipoRpt, string CodigoReporte, string NombreReporte)
            {
                this.TipoRpt = TipoRpt;
                this.CodigoReporte = CodigoReporte;
                this.NombreReporte = NombreReporte;
            }
        }
        public PartialViewResult Index()
        {
            this.ViewBag.ListaReportesAutorizados = this.GetTiposReporte();
            return this.PartialView();
        }

        public void SetDataTable(DataTable dataTable)
        {
            Session["GridViewDataTable"] = dataTable;
        }

        public DataTable GetDataTable()
        {
            if (!(Session["GridViewDataTable"] is DataTable dataTable))
                dataTable = GetEmptyDataTable();
            return dataTable;
        }

        public void SetColumnFormat(Dictionary<string, string> dataTable)
        {
            Session["GridViewColumnFormat"] = dataTable;
        }

        public Dictionary<string, string> GetColumnFormat()
        {
            if (!(Session["GridViewColumnFormat"] is Dictionary<string, string> dataTable))
                dataTable = new Dictionary<string, string>();
            return dataTable;
        }

        public void SetReportType(TipoReporte? reportType)
        {
            Session["GridViewReportType"] = reportType;
        }

        public TipoReporte GetReportType()
        {
            if (!(Session["GridViewReportType"] is TipoReporte reportType))
                reportType = 0;
            return reportType;
        }

        public ActionResult GridViewPartial()
        {
            ViewBag.FormatosColumna = GetColumnFormat();
            ViewBag.TipoReporte = GetReportType();
            return this.PartialView("_DataExportQueryGridViewPartial", GetDataTable());
        }

        public ActionResult Query(TipoReporte? tipoReporte,
            DateTime? fechaInicio, DateTime? fechaFinal,
            bool isCallback = false)
        {
            DataTable dataExport;
            Dictionary<string, string> formatosColumna;

            if (tipoReporte.HasValue)
            {
                switch (tipoReporte.Value)
                {
                    case TipoReporte.Proveedores:
                        dataExport = this.GetProveedoresDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.Clientes:
                        dataExport = this.GetClientesDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.Productos:
                        dataExport = this.GetProductosDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.OrdenProduccion:
                        dataExport = this.GetOrdenProduccionDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.OrdenProduccionDetalle:
                        dataExport = this.GetOrdenProduccionDetalleDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.OrdenCompra:
                        dataExport = this.GetOrdenCompraDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.AnalisisCalidad:
                        dataExport = this.GetAnalisisCalidadDataTable(fechaInicio,fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            { "Libras Programadas", "N2" },
                            { "Libras Remitidas", "N2" },
                            { "Libras Recibidas", "N2" },
                            { "Residual S02", "N2" },
                            { "Temperatura", "N2" },
                            { "Gramaje", "N2" },

                        };
                        break;


                    case TipoReporte.FacturasFiscales:
                        dataExport = this.GetFacturasFiscalesDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;





                    case TipoReporte.FacturasFiscalesMatriz:
                        dataExport = this.GetFacturasFiscalesMatrizDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            { "PES_BRU_LB", "N2" },
                            { "PES_BRU_KL", "N2" },
                            { "PES_BRU_CRT", "N2" },
                            { "LIBRAS", "N2" },
                            { "KILOS", "N2" },
                            { "PES_NET_LB", "N2" },
                            { "PRECIO_LB", "N4" },
                            { "PRECIO_KL", "N4" },
                            { "PRE_CONV_LB", "N4" },
                            { "PRE_UNI", "N4" },
                            { "LBRS_NETA", "N2" },
                            { "VALOR_NETO", "N2" },

                        };

                        break;

                    /*****************************************/


                    case TipoReporte.FacturasComercialMatriz:
                        dataExport = this.GetFacturasComercialDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            { "PES_BRU_LB", "N2" },
                            { "PES_BRU_KL", "N2" },
                            { "PES_BRU_CRT", "N2" },
                            { "LIBRAS", "N2" },
                            { "KILOS", "N2" },
                            { "PES_NET_LB", "N2" },
                            { "PRECIO_LB", "N4" },
                            { "PRECIO_KL", "N4" },
                            { "PRE_CONV_LB", "N4" },
                            { "PRE_UNI", "N4" },
                            { "LBRS_NETA", "N2" },
                            { "VALOR_NETO", "N2" },

                        };

                        break;


                    /**************************************/





                    case TipoReporte.ReporteCuentasPorCobrarExterior:
                        dataExport = this.GetReporteCuentasPorCobrarExteriorDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;


                    /*******************************************************************************************/

                    case TipoReporte.ReporteEntradaVehiculoTercero:
                        dataExport = this.GetReporteEntradaVehiculoTercero(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.ReporteSalidaVehiculoTercero:
                        dataExport = this.GetReporteSalidaVehiculoTercero(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;


                    /*******************************************************************************************/


                    case TipoReporte.ListaPrecio:
                        dataExport = this.GetListaPreciosDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.ResumenLiquidacionMatriz:
                        dataExport = this.GetResumenLiquidacionMatrizDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            {"LbsProgramadas","N2"},
                            {"LbsRemitidas","N2"},
                            {"LbsRecibidas","N2"},
                            {"LbsProcesadas","N2"},
                            {"LbsCabeza","N2"},
                            {"LbsCola","N2"},
                            {"LbsRechazo","N2"},
                            {"RendimientoCab","N2"},
                            {"RendimeintoCol","N2"},
                            {"ValorCabeza","N2"},
                            {"ValorCola","N2"},
                            {"ValorTotal","N2"},
                            {"PrecioCabeza","N2"},
                            {"PrecioCola","N2"},
                            {"Gramaje","N2"},
                        };
                        break;

                    case TipoReporte.SaldoPorTallaMatriz:
                        dataExport = this.GetResumenSaldoPorTallaMatriz(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.GuiaRemisionTerrestre:
                        dataExport = this.GetGuiaRemisionMatrizDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.GuiaRemisionFluvial:
                        dataExport = this.GetGuiaRemisionMatrizFluvialDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.SaldoPorDiaMatriz:
                        dataExport = this.GetResumenSaldoPorDiaMatriz(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.ReporteCuentasPorCobrar:
                        dataExport = this.GetCuentasPorCobrarMatrizDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            {"ValorFactura","N2"},
                            {"ValorAbonado","N2"},
                            {"TotalSaldoCobrar","N2"},
                            {"ValorFinanciado","N2"},
                        };
                        break;

                    case TipoReporte.ProformasMatriz:
                        dataExport = this.GetProformasMatrizDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            { "SubTotalSinIva","N2"},
                            { "ValorTotal","N2"},
                            { "TotalDescuento","N2"},
                            { "Subtotal","N2"},
                        };
                        break;

                    case TipoReporte.ProformasMatrizDetalle:
                        dataExport = this.GetProformasMatrizDetalleDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            { "Valor_Neto","N2"},
                            { "TotalDescuento","N2"},
                            { "Subtotal","N2"},
                        };
                        break;

                    case TipoReporte.LiquidacionesValorizadasPorTalla:
                        dataExport = this.GetLiquidacionesValorizadasDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;


                    case TipoReporte.RecepcionesyLiquidacionMP:
                        dataExport = this.GetRecepcionyLiquidacionMPDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.InventarioMatriz:
                        dataExport = this.GetInventarioMatrizDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.PermisosBodegas:
                        dataExport = this.GetPermisosBodegasDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.MatrizCostoProduccion:
                        dataExport = this.GetCostosProduccionDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.FormulacionProductos:
                        dataExport = this.GetFormulacionProductosDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.ProcesoInternoMovimiento:
                        dataExport = this.GetProcesoInternoInventarioDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.EstadoBodegasPeriodo:
                        dataExport = this.GetEstadoBodegasPeriodoDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.OrdenProduccionMasterizado:
                        dataExport = this.GetOrdenProduccionMasterizadoDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.MatrizLibrasBajadas:
                        dataExport = this.GetMatrizLibrasBajadasDataTable(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.RecepcionMaterialDespacho:
                        dataExport = this.GetRecepcionMaterialesDespacho(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.PermisosPorUsuarios:
                        dataExport = this.GetPermisosPorUsuariosDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.MatrizMasterizado:
                        dataExport = this.GetMatrizMasterizado(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.MatrizMovimientoInventario:
                        dataExport = this.GetMatrizMovimientosInventario(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.MatrizResumenCuadratura:
                        dataExport = this.GetMatrizResumenCuadratura(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            { "LbIngresos","N6"},
                            { "LbEgresos","N6"},
                        };
                        break;

                    case TipoReporte.MatrizResumenCuadraturaDetallado:
                        dataExport = this.GetMatrizResumenCuadraturaDetallado(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>()
                        {
                            { "LbIngresos","N2"},
                            { "LbEgresos","N2"},
                        };
                        break;

                    case TipoReporte.MatrizSaldosIniciales:
                        dataExport = this.GetMatrizKardexValorizado(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.MatrizSaldosProcesosInternos:
                        dataExport = this.GetMatrizSaldosProcesosInternos(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    case TipoReporte.MatrizFacturasNoIntegradas:
                        dataExport = this.GetMatrizFacturasNoIntegradas(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    default:
                        dataExport = this.GetEmptyDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        tipoReporte = null;
                        break;


                    case TipoReporte.FacturaComercial:
                        dataExport = this.GetFactComercial(fechaInicio, fechaFinal);
                        formatosColumna = new Dictionary<string, string>();
                        break;

                }
            }
            else
            {
                dataExport = this.GetEmptyDataTable();
                formatosColumna = new Dictionary<string, string>();
            }

            this.ViewBag.TipoReporte = tipoReporte;
            this.ViewBag.FormatosColumna = formatosColumna;

            SetReportType(tipoReporte);
            SetColumnFormat(formatosColumna);
            SetDataTable(dataExport);

            // Retornar la vista...
            if (isCallback)
            {
                return this.PartialView("_DataExportQueryGridViewPartial", dataExport);
            }
            else
            {
                return this.PartialView("_DataExportQueryResultPartial", dataExport);
            }
        }

        public enum TipoReporte
        {
            Desconocido,
            Proveedores,
            Clientes,
            Productos,
            AnalisisCalidad,
            FacturasFiscales,
            FacturasFiscalesMatriz,
            ReporteCuentasPorCobrarExterior,
            ListaPrecio,
            ResumenLiquidacionMatriz,
            SaldoPorTallaMatriz,
            SaldoPorDiaMatriz,
            GuiaRemisionTerrestre,
            GuiaRemisionFluvial,
            ReporteCuentasPorCobrar,
            ProformasMatriz,
            ProformasMatrizDetalle,
            LiquidacionesValorizadasPorTalla,
            RecepcionesyLiquidacionMP,
            OrdenProduccion,
            OrdenProduccionDetalle,
            OrdenCompra,
            InventarioMatriz,
            PermisosBodegas,
            ProcesoInternoMovimiento,
            EstadoBodegasPeriodo,
            OrdenProduccionMasterizado,
            MatrizLibrasBajadas,
            RecepcionMaterialDespacho,
            PermisosPorUsuarios,
            MatrizMasterizado,
            FormulacionProductos,
            MatrizCostoProduccion,
            MatrizMovimientoInventario,
            MatrizResumenCuadratura,
            MatrizResumenCuadraturaDetallado,
            MatrizSaldosIniciales,
            MatrizSaldosProcesosInternos,
            MatrizFacturasNoIntegradas,
            FacturaComercial,
            FacturasComercialMatriz,
            ReporteEntradaVehiculoTercero,
            ReporteSalidaVehiculoTercero
        }

        private Dictionary<TipoReporte, string> GetTiposReporte()
        {
            var diccionarioReportes = new Dictionary<TipoReporte, string>();

            var reportes = new[]
            {
                new Reporte(TipoReporte.Proveedores, "MATRIZEXPPROVEEDORES", "Proveedores"),
                new Reporte(TipoReporte.Productos, "MATRIZEXRPTPRODUCTOS", "Productos"),
                new Reporte(TipoReporte.AnalisisCalidad, "MATRIZEXPANALISICALI", "Análisis de Calidad"),
                new Reporte(TipoReporte.FacturasFiscales, "MATRIZEXPFACTUFISCAL", "Facturas Fiscales"),
                new Reporte(TipoReporte.FacturasFiscalesMatriz, "MATRIZEXPFACTFISMATR", "Facturas Fiscales Matriz"),
                new Reporte(TipoReporte.ReporteCuentasPorCobrarExterior, "MATRIZEXPCTAPORCOBEX", "Reporte Cuentas por Cobrar Exterior"),
                new Reporte(TipoReporte.ListaPrecio, "MATRIZEXPLISTAPRECIO", "Lista de Precios"),
                new Reporte(TipoReporte.ResumenLiquidacionMatriz, "MATRIZEXPRESLIQUMATR", "Resumen Liquidación Matriz"),
                new Reporte(TipoReporte.SaldoPorTallaMatriz, "MATRIZEXPSALDPORTALL", "Saldo por Talla Matriz"),
                new Reporte(TipoReporte.SaldoPorDiaMatriz, "MATRIZEXPSALDOPORDIA", "Saldo por Día Matriz"),
                new Reporte(TipoReporte.GuiaRemisionTerrestre, "MATRIZEXPGUIAREMTERR", "Guia Remisión Terrestre Matriz"),
                new Reporte(TipoReporte.GuiaRemisionFluvial, "MATRIZEXPGUIAREMFLUV", "Guia Remisión Fluvial Matriz"),
                new Reporte(TipoReporte.ReporteCuentasPorCobrar, "MATRIZEXPCUTAPORCOBR", "Reporte de Cuentas Por Cobrar"),
                new Reporte(TipoReporte.ProformasMatriz, "MATRIZEXPPROFORMAMAT", "Proformas Matriz"),
                new Reporte(TipoReporte.ProformasMatrizDetalle, "MATRIZEXPPROFMATRDET", "Proformas Matriz Detalle"),
                new Reporte(TipoReporte.Clientes, "MATRIZEXPRPTCLIENTES", "Clientes"),
                new Reporte(TipoReporte.LiquidacionesValorizadasPorTalla, "MATRIZEXPLIQVALPTALL", "Liquidaciones Valorizadas Por Talla"),
                new Reporte(TipoReporte.RecepcionesyLiquidacionMP, "MATRIZEXPRECLIQUDEMP", "Recepciones y Liquidación de MP"),
                new Reporte(TipoReporte.OrdenProduccion, "MATRIZEXPMATROPRODUC", "Matriz Orden de Producción"),
                new Reporte(TipoReporte.OrdenProduccionDetalle, "MATRIZEXPMATRORDPRDT", "Matriz Orden de Producción Detalle"),
                new Reporte(TipoReporte.OrdenCompra, "MATRIZEXPMATRORDCOMP", "Matriz Orden de Compra"),
                new Reporte(TipoReporte.InventarioMatriz, "MATRIZEXPMATRINVENTA", "Matriz Inventario"),
                new Reporte(TipoReporte.PermisosBodegas, "MATRIZEXPMATRPERMBOD", "Matriz Permisos a Bodegas"),
                new Reporte(TipoReporte.ProcesoInternoMovimiento, "MATRIZEXPPROCINTMOVI", "Proceso Interno - Movimiento"),
                new Reporte(TipoReporte.EstadoBodegasPeriodo, "MATRIZEXPMATRESTBOPE", "Matriz de Estados por Bodegas de Periodos"),
                new Reporte(TipoReporte.OrdenProduccionMasterizado, "MATRIZEXPMTRORPRODMA", "Matriz Orden Producción - Materizado"),
                new Reporte(TipoReporte.MatrizLibrasBajadas, "MATRIZEXPMATRLIBBAJA", "Matriz Libras Bajadas"),
                new Reporte(TipoReporte.RecepcionMaterialDespacho, "MATRIZEXPMATRRECMADS", "Matriz Recep. Mat. Despacho"),
                new Reporte(TipoReporte.PermisosPorUsuarios, "MATRIZEXPMATRPERMUSU", "Matriz Permisos por Usuario"),
                new Reporte(TipoReporte.MatrizMasterizado, "MATRIZEXPMATRZMASTER", "Matriz de Masterizado"),
                new Reporte(TipoReporte.FormulacionProductos, "MATRIZEXPFORMUPRODUC", "Formulación de Productos"),
                new Reporte(TipoReporte.MatrizCostoProduccion, "MATRIZEXPMATRCOSPROD", "Matriz Costos de Producción"),
                new Reporte(TipoReporte.MatrizMovimientoInventario, "MATRIZEXPMATRMOVIINV", "Matriz Movimientos de Inventario"),
                new Reporte(TipoReporte.MatrizResumenCuadratura, "MATRIZEXPMATRRESUCUA", "Matriz Resumen Cuadratura"),
                new Reporte(TipoReporte.MatrizResumenCuadraturaDetallado, "MATRIZEXPMATRRESCUAD", "Matriz Resumen Cuadratura Detallado"),
                new Reporte(TipoReporte.MatrizSaldosIniciales, "MATRIZEXPMATRZKARDVA", "Matriz Kardex Valorizado"),
                new Reporte(TipoReporte.MatrizSaldosProcesosInternos, "MATRIZEXPMATRZSALVAL", "Matriz Saldos Valorizados"),
                new Reporte(TipoReporte.MatrizFacturasNoIntegradas, "MATRIZEXPFACTNOINTRG", "Matriz Facturas no Integradas"),
                new Reporte(TipoReporte.FacturaComercial, "EXPFACTCOM", "Facturas Comercial"),
                new Reporte(TipoReporte.FacturasComercialMatriz, "EXPFACTCOMMATRIZ", "Facturas Comercial Matriz"),
                new Reporte(TipoReporte.ReporteEntradaVehiculoTercero, "ENTVEHTER", "Entrada de Vehiculos Terceros"),
                new Reporte(TipoReporte.ReporteSalidaVehiculoTercero, "SALVEHTER", "Salida de Vehiculos Terceros"),

            };

            foreach (var reporte in reportes)
            {
                if (this.EstaAutorizadoObjeto(m_Controller, reporte.CodigoReporte))
                {
                    diccionarioReportes.Add(reporte.TipoRpt, reporte.NombreReporte);
                }
            }

            return diccionarioReportes;
        }

        public static Dictionary<TipoReporte, string> GetReporteAuxiliar()
        {
            return new Dictionary<TipoReporte, string>()
            {
                { TipoReporte.Proveedores, "Proveedores"  },
                { TipoReporte.Productos, "Productos"  },
                { TipoReporte.AnalisisCalidad, "Análisis de Calidad"  },
                { TipoReporte.FacturasFiscales, "Facturas Fiscales"  },
                { TipoReporte.FacturasFiscalesMatriz, "Facturas Fiscales Matriz"  },
                { TipoReporte.ReporteCuentasPorCobrarExterior, "Reporte Cuentas por Cobrar Exterior"  },
                { TipoReporte.ListaPrecio, "Lista de Precios"  },
                { TipoReporte.ResumenLiquidacionMatriz, "Resumen Liquidación Matriz"  },
                { TipoReporte.SaldoPorTallaMatriz, "Saldo por Talla Matriz"  },
                { TipoReporte.SaldoPorDiaMatriz, "Saldo por Día Matriz"  },
                { TipoReporte.GuiaRemisionTerrestre, "Guia Remisión Terrestre Matriz"  },
                { TipoReporte.GuiaRemisionFluvial, "Guia Remisión Fluvial Matriz"  },
                { TipoReporte.ReporteCuentasPorCobrar, "Reporte de Cuentas Por Cobrar"  },
                { TipoReporte.ProformasMatriz, "Proformas Matriz"},
                { TipoReporte.ProformasMatrizDetalle, "Proformas Matriz Detalle"},
                { TipoReporte.Clientes, "Clientes"  },
                { TipoReporte.LiquidacionesValorizadasPorTalla, "Liquidaciones Valorizadas Por Talla"  },
                { TipoReporte.RecepcionesyLiquidacionMP, "Recepciones y Liquidación de MP"  },
                { TipoReporte.OrdenProduccion, "Matriz Orden de Producción"  },
                { TipoReporte.OrdenProduccionDetalle, "Matriz Orden de Producción Detalle"  },
                { TipoReporte.OrdenCompra, "Matriz Orden de Compra"  },
                { TipoReporte.InventarioMatriz, "Matriz Inventario"  },
                { TipoReporte.PermisosBodegas, "Matriz Permisos a Bodegas"  },
                { TipoReporte.ProcesoInternoMovimiento, "Proceso Interno - Movimiento"  },
                { TipoReporte.EstadoBodegasPeriodo, "Matriz de Estados por Bodegas de Periodos"  },
                { TipoReporte.OrdenProduccionMasterizado, "Matriz Orden Producción - Materizado"  },
                { TipoReporte.MatrizLibrasBajadas, "Matriz Libras Bajadas"  },
                { TipoReporte.RecepcionMaterialDespacho, "Matriz Recep. Mat. Despacho"  },
                { TipoReporte.PermisosPorUsuarios, "Matriz Permisos por Usuario"  },
                { TipoReporte.MatrizMasterizado, "Matriz de Masterizado"  },
                { TipoReporte.FormulacionProductos, "Formulación de Productos"  },
                { TipoReporte.MatrizCostoProduccion, "Matriz Costos de Producción"  },
                { TipoReporte.MatrizMovimientoInventario, "Matriz Movimientos de Inventario"  },
                { TipoReporte.MatrizResumenCuadratura, "Matriz Resumen Cuadratura"  },
                { TipoReporte.MatrizResumenCuadraturaDetallado, "Matriz Resumen Cuadratura Detallado"  },
                { TipoReporte.MatrizSaldosIniciales, "Matriz Kardex Valorizado"  },
                { TipoReporte.MatrizSaldosProcesosInternos, "Matriz Saldos Valorizados"  },
                { TipoReporte.MatrizFacturasNoIntegradas, "Matriz Facturas no Integradas"  },
                { TipoReporte.FacturaComercial, "Facturas Comercial"  },
                { TipoReporte.FacturasComercialMatriz, "Facturas Comercial Matriz"  },
                { TipoReporte.ReporteEntradaVehiculoTercero, "Entrada de Vehiculos Terceros"  },
                { TipoReporte.ReporteSalidaVehiculoTercero, "Salida de Vehiculos Terceros"  },

            };
        }

        private DataTable GetProveedoresDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieProveedoresReport Order By id";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ProveedoresDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ProveedoresDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetClientesDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieClientesReport Order By [Nombre Completo]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ClientesDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ClientesDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetProductosDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieProductosReport Order By id";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ProductosDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ProductosDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetPermisosBodegasDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.viePermisosBodegas Order By [NombreUsuario],[Bodega],[NombreTipoBodega]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("PermisosBodegasDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "PermisosBodegasDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetFormulacionProductosDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieFormulaProductos Order By [CodigoItemPrincipal],[CodigoIngrediente]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("FormulacionProductosDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "FormulacionProductosDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetCostosProduccionDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieMatrizCostosProduccion Order By [Numero_Operacion],[Tipo_Ejecucion],[Tipo_Bodega]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("CostosProduccionDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "CostosProduccionDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetProcesoInternoInventarioDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieProcesoInternoInventario Order By [Lote]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ProcesoInternoInventarioDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ProcesoInternoInventarioDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetEstadoBodegasPeriodoDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.viePeriodos Order By [Año], [NPeriodo]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("EstadoBodegaPeriodoDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "EstadoBodegaPeriodoDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetOrdenProduccionMasterizadoDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.VieOrdenProduccionMasterizado Order By [Fecha de Emisión OP]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("OrdenProduccionMasterizadoDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "OrdenProduccionMasterizadoDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetPermisosPorUsuariosDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.ViePermisosUsuarios Order By [Usuario],[Opcion],[Permiso]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("PermisosUsuariosDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "PermisosUsuariosDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetOrdenProduccionDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieOrdenProduccionCabecera Order By [Fecha de Emisión]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("OrdenProduccionDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "OrdenProduccionDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetOrdenProduccionDetalleDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieOrdenProduccionDetalle Order By [Fecha de Emisión], [No. Documento]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("OrdenProduccionDetalleDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "OrdenProduccionDetalleDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetOrdenCompraDataTable()
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "Select * From dbo.vieOrdenCompra Order By [Fecha de Emisión]";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 90;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("OrdenCompraDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "OrdenCompraDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetRecepcionMaterialesDespacho(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec sp_MaterialesDespacho";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("FacturasFiscalesDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "FacturasFiscalesDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////
        private DataTable GetFactComercial(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec sp_FactComercial";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("FacturasComercialDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "FacturasComercialDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }






        /////////////////////////////////////////////////////////////////////////

        /**************************************************************************/

        private DataTable GetReporteEntradaVehiculoTercero(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec SP_EntradaVehiculos";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ReporteEntradaVehiculoTerceroDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ReporteEntradaVehiculoTerceroDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }



        private DataTable GetReporteSalidaVehiculoTercero(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec SP_SalidaVehiculo";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ReporteSalidaVehiculoTerceroDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ReporteSalidaVehiculoTerceroDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }




        /*********************************************************************/






        private DataTable GetMatrizMasterizado(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spcMatrizMasterizado";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("MatrizMasterizadoDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "MatrizMasterizadoDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetMatrizMovimientosInventario(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spcMatrizMovimientosInventario";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("MovimientoInventarioDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "MovimientoInventarioDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetMatrizResumenCuadratura(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spcResumenCuadraturaMatriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ResumenCuadraturaDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ResumenCuadraturaDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetMatrizResumenCuadraturaDetallado(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spcResumenCuadraturaMatrizDetallado";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ResumenCuadraturaDetalladoDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ResumenCuadraturaDetalladoDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetMatrizKardexValorizado(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spcMatrizSaldosIniciales";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("MatrizKardexValorazadoDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "MatrizKardexValorizadoDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetMatrizSaldosProcesosInternos(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec Spc_Consultar_Matriz_Procesos_Internos_Costos";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("MatrizSaldosProcesosInternosDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "MatrizSaldosProcesosInternosDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetAnalisisCalidadDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "exec spc_AnalisisCalidad";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmd.CommandText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmd.CommandText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmd.CommandText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmd.CommandText += " '','' ";
                    }

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 600;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("AnalisisCalidadDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "AnalisiCalidadDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetMatrizLibrasBajadasDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec SPLibrasBajadas";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("LibrasBajadasDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "LibrasBajadasDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        
        private DataTable GetFacturasFiscalesDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec Factura_Fiscal_Matriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("FacturasFiscalesDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "FacturasFiscalesDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }


        /*****************************************************************************************************************************************/
        private DataTable GetFacturasComercialDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spc_FacturasComercialMatrizReport";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("FacturasComercialMatrrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "FacturasComercialMatrizDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }


        /*****************************************************************************************************************************************/
        private DataTable GetFacturasFiscalesMatrizDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spc_FacturasFiscalesMatrizReport";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("FacturasFiscalesMatrrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "FacturasFiscalesMatrizDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetReporteCuentasPorCobrarExteriorDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec ReporteCuentasPorCobrarExterior";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ReporteCuentasPorCobrarExteriorDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ReporteCuentasPorCobrarExteriorDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetResumenLiquidacionMatrizDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec par_ResumenLiquidacionesMatriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ResumenLiquidacionMatrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ResumenLiquidacionMatrizDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetListaPreciosDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnnpl = GetSqlConnectionPriceList())
            {
                using (var cmd = cnnpl.CreateCommand())
                {
                    var cmdText = "exec spc_ListaPrecioReport";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ListaPreciosDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ListaPreciosDataTable";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetResumenSaldoPorTallaMatriz(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec par_SaldoPorTallaMatriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("SaldoPorTallaMatrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "SaldoPorTallaMatrizDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetResumenSaldoPorDiaMatriz(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec par_MatrizSaldoDia";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("SaldoPorDiaMatrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "SaldoPorDiaMatrizDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetGuiaRemisionMatrizDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            string sqlSentence = "par_GuiasRemisionTerrestreMatriz";
            string parametroCustodio = (db.Setting.FirstOrDefault(r => r.code == "TRC")?.value ?? "NO");
            if (parametroCustodio == "SI")
            {
                sqlSentence = "par_GuiasRemisionTerrestreMatriz_Custodian";
            }
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = $"exec {sqlSentence}";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 120;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("GuiaRemisionMatrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "GuiaRemisionMatrizDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetGuiaRemisionMatrizFluvialDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec par_GuiasRemisionFluvialMatriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 120;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("GuiaRemisionFluvialMatrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "GuiaRemisionFluvialMatrizDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetCuentasPorCobrarMatrizDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec Cuentas_Cobrar_Matriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 120;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ReporteCuentasPorCobrar");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ReporteCuentasPorCobrar";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetProformasMatrizDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "Exec Proforma_Matriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }

                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ProformaMatrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ProformaMatrizDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetProformasMatrizDetalleDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "Exec Proforma_MatrizDetalle";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }

                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ProformaMatrizDetalleDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ProformaMatrizDetalleDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetLiquidacionesValorizadasDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "Exec Liquidaciones_Valorizadas_Talla";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }

                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 120;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ProformaMatrizDetalleDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ProformaMatrizDetalleDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }
        private DataTable GetRecepcionyLiquidacionMPDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "Exec Recepciones_Liquidación_MP";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }

                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("ProformaMatrizDetalleDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "ProformaMatrizDetalleDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetInventarioMatrizDataTable(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "Exec Spc_Inventario_Matriz";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @fechaEmisionInicio, @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @fechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@fechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @fechaEmisionFinal";

                        cmd.Parameters.Add("@fechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }

                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 1200;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("InventarioMatrizDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "InventarioMatrizDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        private DataTable GetMatrizFacturasNoIntegradas(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            using (var cnn = this.GetSqlConnection())
            {
                using (var cmd = cnn.CreateCommand())
                {
                    var cmdText = "exec spc_FacturasNoIntegradas";

                    if (fechaInicio.HasValue && fechaFinal.HasValue)
                    {
                        cmdText += "  @FechaEmisionInicio, @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdText += " @FechaEmisionInicio, '' ";

                        cmd.Parameters.Add("@FechaEmisionInicio", SqlDbType.DateTime).Value = fechaInicio.Value.Date;
                    }
                    else if (fechaFinal.HasValue)
                    {
                        cmdText += " '', @FechaEmisionFinal";

                        cmd.Parameters.Add("@FechaEmisionFinal", SqlDbType.DateTime).Value = fechaFinal.Value.Date;
                    }
                    if (!fechaInicio.HasValue && !fechaFinal.HasValue)
                    {
                        cmdText += " '','' ";
                    }


                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 180;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet("FacturasNoIntegradasDataSet");
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "FacturasNoIntegradasDataSet";
                            return ds.Tables[0];
                        }
                        else
                        {
                            return this.GetEmptyDataTable();
                        }
                    }
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult OnDateDataExportValidation(string startDate, string endDate)
        {
            DateTime _startDate = JsonConvert.DeserializeObject<DateTime>(startDate);
            DateTime _endDate = JsonConvert.DeserializeObject<DateTime>(endDate);


            var result = new
            {
                itsValided = 1,
                Error = ""
            };

            int daysDiff = ((TimeSpan)(_endDate - _startDate)).Days;

            if (daysDiff > 7)
            {
                result = new
                {
                    itsValided = 0,
                    Error = "La Fecha ingresada debe ser menor o igual a 7 días"
                };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult OnDateDataExportDateValidation(string startDate, string endDate)
        {
            DateTime _startDate = JsonConvert.DeserializeObject<DateTime>(startDate);
            DateTime _endDate = JsonConvert.DeserializeObject<DateTime>(endDate);


            var result = new
            {
                itsValided = 1,
                Error = ""
            };

            int daysDiff = ((TimeSpan)(_endDate - _startDate)).Days;

            if (daysDiff > 31)
            {
                result = new
                {
                    itsValided = 0,
                    Error = "La Fecha ingresada debe ser menor o igual a 31 días"
                };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(
                ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PRCuentasCobrar(string startDate, string endDate)
        {
            DateTime fechaInicio, fechaFin;
            if (startDate != "")
            {
                fechaInicio = JsonConvert.DeserializeObject<DateTime>(startDate);
            }
            else
            {
                fechaInicio = new DateTime(1901, 01, 01);
            }
            if (endDate != "")
            {
                fechaFin = JsonConvert.DeserializeObject<DateTime>(endDate);
            }
            else
            {
                fechaFin = DateTime.Now;
            }
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            string str_starReceptionDate = "";
            if (fechaInicio != null) { str_starReceptionDate = fechaInicio.Date.ToString("yyyy/MM/dd"); }
            _param.Nombre = "@inicio";
            _param.Valor = str_starReceptionDate;
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (fechaFin != null) { str_endReceptionDate = fechaFin.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@fin";
            _param.Valor = str_endReceptionDate;
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "RCXC";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        private SqlConnection GetSqlConnectionPriceList()
        {
            return new SqlConnection(
                ConfigurationManager.ConnectionStrings["DBContextNEPL"].ConnectionString);
        }

        private DataTable GetEmptyDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Columna", typeof(string));
            return dataTable;
        }
    }
}