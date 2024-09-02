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

namespace DXPANACEASOFT.Controllers
{
    public class DataExportUserPermissionController : DefaultController
    {
        public PartialViewResult Index()
        {
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
            return this.PartialView("_DataExportUserPermissionQueryGridViewPartial", GetDataTable());
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
                    
                    case TipoReporte.PermisosBodegas:
                        dataExport = this.GetPermisosBodegasDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                   
                    case TipoReporte.PermisosPorUsuarios:
                        dataExport = this.GetPermisosPorUsuariosDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        break;

                    default:
                        dataExport = this.GetEmptyDataTable();
                        formatosColumna = new Dictionary<string, string>();
                        tipoReporte = null;
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
                return this.PartialView("_DataExportUserPermissionQueryGridViewPartial", dataExport);
            }
            else
            {
                return this.PartialView("_DataExportUserPermissionQueryResultPartial", dataExport);
            }
        }

        public enum TipoReporte
        {
            Desconocido,
            PermisosBodegas,
            PermisosPorUsuarios,
        }

        public static Dictionary<TipoReporte, string> GetTiposReporte()
        {
            return new Dictionary<TipoReporte, string>()
            {
                { TipoReporte.PermisosBodegas, "Matriz Permisos a Bodegas"  },
                { TipoReporte.PermisosPorUsuarios, "Matriz Permisos por Usuario"  },
            };
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

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(
                ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString);
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