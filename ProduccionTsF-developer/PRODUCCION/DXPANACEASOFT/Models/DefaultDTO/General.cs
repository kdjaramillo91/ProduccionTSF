
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.General
{
    public class AnswerValidateForm
    {
        public string hasError { get; set; }
        public string message { get; set; }
    }
    public class AnswerForAction
    {
        public ButtonsOnEditForm btnOnEditForm { get; set; }

        public string messageAnswer;

        public int idEntity;

        public string codeStateAnswer;

        public string nameStateAnswer;

        public string strNumberDocAnswer;

    }
    public class AnswerForActionRemissionGuide
    {
        public ButtonsOnEditFormRemissionGuide btnOnEditFormRemissionGuide { get; set; }

        public string messageAnswer;

        public int idEntity;

        public string codeStateAnswer;

        public string nameStateAnswer;

        public string strNumberDocAnswer;
    }
    public class ButtonsOnEditForm
    {
        public bool btnApprove { get; set; }
        public bool btnAutorize { get; set; }
        public bool btnProtect { get; set; }
        public bool btnCancel { get; set; }
        public bool btnRevert { get; set; }

        public bool btnSave { get; set; }
    }

    public class ButtonsOnEditFormRemissionGuide
    {
        public bool btnApprove { get; set; }
        public bool btnAutorize { get; set; }
        public bool btnProtect { get; set; }
        public bool btnCancel { get; set; }
        public bool btnRevert { get; set; }
        public bool btnreassignment { get; set; }
        public bool btnCancelRG { get; set; }
        public bool btnPrint { get; set; }
        public bool btnPrintAlldoc { get; set; }

        public bool btnPrintManual { get; set; }
        public bool btnPrintAlldocManual { get; set; }
        public bool btnSave { get; set; }
    }

    public class MetricUnitModelP
    {
        public int idMetricUnitModelP { get; set; }
        public string codeMetricUnitModelP { get; set; }

        public string nameMetricUnitModelP { get; set; }
    }
    public class EmissionPointModelP
    {
        public int idEmissionPointModelP { get; set; }
        public int idBranchOfficeModelP { get; set; }
        public int idDivisionModelP { get; set; }
        public int idCompanyModelP { get; set; }
        public string nameModelP { get; set; }
        public string descriptionModelP { get; set; }
        public int codeModelP { get; set; }
        public string addressModelP { get; set; }
        public string emailModelP { get; set; }
        public string phoneNumberModelP { get; set; }
    }
    //public class RepoKardexSaldo
    //{
    //    public int idDetalleInventario { get; set; }
    //    public int idCabeceraInventario { get; set; }
    //    public DateTime fechaInicio { get; set; }
    //    public DateTime fechaFin { get; set; }
    //    public string numeroDocumentoInventario { get; set; }
    //    public int idProducto { get; set; }
    //    public string nombreProducto { get; set; }
    //    public DateTime fechaEmison { get; set; }
    //    public int idMotivoInventario { get; set; }
    //    public string nombreMotivoInventario { get; set; }
    //    public int idUnidadMedida { get; set; }
    //    public string nombreUnidadMedida { get; set; }
    //    public decimal montoEntrada { get; set; }
    //    public decimal montoSalida { get; set; }
    //    public decimal balance { get; set; }
    //    public int idCompania { get; set; }
    //}
    //public class RepoCompany
    //{
    //    public int idCompany { get; set; }
    //    public byte[] logo { get; set; }
    //}
    
}