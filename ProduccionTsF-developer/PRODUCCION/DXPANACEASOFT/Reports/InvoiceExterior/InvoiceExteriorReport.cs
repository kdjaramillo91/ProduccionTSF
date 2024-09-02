using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DevExpress.XtraReports.UI;
using DXPANACEASOFT.Models;

/// <summary>
/// Summary description for InventoryMovesListReport
/// </summary>
public class InvoiceExteriorReport : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DevExpress.DataAccess.EntityFramework.EFDataSource efDataSource1;
    private ReportHeaderBand ReportHeader;
    private XRTable xrTable1;
    private XRTableRow xrTableRow1;
    private XRTableCell xrTableCell1;
    private XRTableCell xrTableCell2;
    private XRTableCell xrTableCell3;
    private XRTableCell xrTableCell4;
    private XRTableCell xrTableCell5;
    private XRTableCell xrTableCell6;
    private XRTableCell xrTableCell7;
    private XRTableCell xrTableCell8;
    private ReportFooterBand ReportFooter;
    private XRPanel xrPanel1;
    private XRLabel xrLabel6;
    private XRLabel xrLabel5;
    private XRLabel xrLabel3;
    private XRLabel xrLabel2;
    private XRTableCell xrTableCell9;
    private XRPanel xrPanel2;
    private XRLabel xrLabel7;
    private XRLabel xrLabel14;
    private XRTable xrTable2;
    private XRTableRow xrTableRow2;
    private XRTableCell xrTableCell11;
    private XRTableCell xrTableCell12;
    private XRTableRow xrTableRow3;
    private XRTableCell xrTableCell15;
    private XRLabel xrLabel8;
    private XRTableRow xrTableRow4;
    private XRTableCell xrTableCell10;
    private XRLabel xrLabel9;
    private XRTableRow xrTableRow5;
    private XRTableCell xrTableCell14;
    private XRLabel xrLabel11;
    private XRTableRow xrTableRow6;
    private XRTableCell xrTableCell18;
    private XRPanel xrPanel3;
    private XRTable xrTable3;
    private XRTableRow xrTableRow8;
    private XRTableCell xrTableCell22;
    private XRLabel xrLabel15;
    private XRTableCell xrTableCell24;
    private XRLabel xrLabel17;
    private XRTableRow xrTableRow9;
    private XRTableCell xrTableCell25;
    private XRLabel xrLabel16;
    private XRTableCell xrTableCell26;
    private XRTableCell xrTableCell27;
    private XRLabel xrLabel18;
    private XRTableCell xrTableCell29;
    private XRBarCode xrBarCode1;
    private XRLabel xrLabel56;
    private CalculatedField formapago;
    private CalculatedField plazo;
    private CalculatedField calculatedField1;
    private XRLabel xrLabel74;
    private XRLabel xrLabel73;
    private XRLabel xrLabel80;
    private CalculatedField calculatedField2;
    private CalculatedField calculatedField3;
    private XRTable xrTable5;
    private XRTableRow xrTableRow24;
    private XRTableCell xrTableCell58;
    private XRLabel xrLabel32;
    private XRTableRow xrTableRow26;
    private XRTableCell xrTableCell62;
    private XRLabel xrLabel33;
    private XRTableRow xrTableRow25;
    private XRTableCell xrTableCell59;
    private XRLabel xrLabel34;
    private XRTableRow xrTableRow27;
    private XRTableCell xrTableCell64;
    private XRLabel xrLabel35;
    private XRTableRow xrTableRow28;
    private XRTableCell xrTableCell66;
    private XRLabel xrLabel36;
    private XRTableCell xrTableCell67;
    private XRLabel xrLabel66;
    private XRTableRow xrTableRow29;
    private XRTableCell xrTableCell68;
    private XRLabel xrLabel37;
    private XRTableCell xrTableCell69;
    private XRLabel xrLabel67;
    private XRTableRow xrTableRow30;
    private XRTableCell xrTableCell70;
    private XRLabel xrLabel38;
    private XRTableCell xrTableCell71;
    private XRLabel xrLabel68;
    private XRTableRow xrTableRow31;
    private XRTableCell xrTableCell72;
    private XRLabel xrLabel39;
    private XRTableCell xrTableCell73;
    private XRLabel xrLabel69;
    private XRTableRow xrTableRow32;
    private XRTableCell xrTableCell74;
    private XRLabel xrLabel40;
    private XRTableCell xrTableCell75;
    private XRLabel xrLabel70;
    private XRTableRow xrTableRow34;
    private XRTableCell xrTableCell78;
    private XRLabel xrLabel41;
    private XRTableCell xrTableCell79;
    private XRLabel xrLabel71;
    private XRTableRow xrTableRow33;
    private XRTableCell xrTableCell76;
    private XRLabel xrLabel42;
    private XRTableRow xrTableRow36;
    private XRTableCell xrTableCell82;
    private XRLabel xrLabel43;
    private XRTableCell xrTableCell83;
    private XRTableRow xrTableRow35;
    private XRTableCell xrTableCell80;
    private XRLabel xrLabel44;
    private XRTableCell xrTableCell81;
    private XRTableRow xrTableRow37;
    private XRTableCell xrTableCell84;
    private XRLabel xrLabel45;
    private XRTableCell xrTableCell85;
    private XRTableRow xrTableRow39;
    private XRTableCell xrTableCell88;
    private XRLabel xrLabel46;
    private XRTableCell xrTableCell89;
    private XRTableRow xrTableRow38;
    private XRTableCell xrTableCell86;
    private XRTableCell xrTableCell87;
    private XRPanel xrPanel6;
    private XRTable xrTable6;
    private XRTableRow xrTableRow40;
    private XRTableCell xrTableCell90;
    private XRLabel xrLabel49;
    private XRTableCell xrTableCell91;
    private XRLabel xrLabel50;
    private XRTableCell xrTableCell93;
    private XRLabel xrLabel51;
    private XRTableCell xrTableCell92;
    private XRLabel xrLabel52;
    private XRTableRow xrTableRow41;
    private XRTableCell xrTableCell95;
    private XRTableCell xrTableCell97;
    private XRPanel xrPanel4;
    private XRTable xrTable4;
    private XRTableRow xrTableRow11;
    private XRTableCell xrTableCell32;
    private XRLabel xrLabel20;
    private XRTableRow xrTableRow12;
    private XRTableCell xrTableCell34;
    private XRLabel xrLabel21;
    private XRTableRow xrTableRow13;
    private XRTableCell xrTableCell36;
    private XRLabel xrLabel22;
    private XRTableRow xrTableRow14;
    private XRTableCell xrTableCell38;
    private XRLabel xrLabel23;
    private XRTableRow xrTableRow15;
    private XRTableCell xrTableCell40;
    private XRLabel xrLabel24;
    private XRTableRow xrTableRow16;
    private XRTableCell xrTableCell42;
    private XRLabel xrLabel25;
    private XRTableRow xrTableRow17;
    private XRTableCell xrTableCell44;
    private XRLabel xrLabel26;
    private XRTableRow xrTableRow18;
    private XRTableCell xrTableCell47;
    private XRLabel xrLabel27;
    private XRTableRow xrTableRow42;
    private XRTableCell xrTableCell99;
    private XRLabel xrLabel53;
    private XRTableRow xrTableRow19;
    private XRTableCell xrTableCell48;
    private XRLabel xrLabel28;
    private XRTableRow xrTableRow20;
    private XRLabel xrLabel29;
    private XRTableRow xrTableRow21;
    private XRLabel xrLabel30;
    private XRTableRow xrTableRow44;
    private XRTableCell xrTableCell112;
    private XRLabel xrLabel81;
    private XRTableCell xrTableCell116;
    private XRLabel xrLabel82;
    private XRTableRow xrTableRow22;
    private XRTableCell xrTableCell54;
    private XRLabel xrLabel31;
    private XRTableCell xrTableCell101;
    private XRTableCell xrTableCell100;
    private XRTableCell xrTableCell103;
    private XRTableRow xrTableRow23;
    private XRTableCell xrTableCell56;
    private XRLabel xrLabel10;
    private XRTableCell xrTableCell57;
    private XRLabel xrLabel59;
    private XRLabel xrLabel19;
    private CalculatedField calculatedField4;
    private CalculatedField calculatedField5;
    private XRTableCell xrTableCell51;
    private XRTableCell xrTableCell53;
    private CalculatedField Netokilo;
    private CalculatedField netolibra;
    private CalculatedField brutokilo;
    private CalculatedField calculatedField6;
    private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
    private XRLabel xrLabel55;
    private XRLabel xrLabel4;
    private XRLabel xrLabel1;
    private XRLabel xrLabel63;
    private XRLabel xrLabel62;
    private XRLabel xrLabel61;
    private XRLabel xrLabel60;
    private XRLabel xrLabel58;
    private XRLabel xrLabel57;
    private XRTableRow xrTableRow10;
    private XRTableCell xrTableCell28;
    private XRLabel xrLabel48;
    private XRTableCell xrTableCell30;
    private XRTableCell xrTableCell31;
    private XRTableCell xrTableCell33;
    private XRTableCell xrTableCell35;
    private XRTableCell xrTableCell37;
    private XRTableCell xrTableCell39;
    private XRTableCell xrTableCell41;
    private XRTableCell xrTableCell43;
    private XRTableCell xrTableCell45;
    private XRTableCell xrTableCell46;
    private XRTableCell xrTableCell49;
    private XRTableCell xrTableCell52;
    private XRTableCell xrTableCell50;
    private XRTableCell xrTableCell55;
    private XRTableCell xrTableCell60;
    private XRTableCell xrTableCell63;
    private XRTableCell xrTableCell61;
    private XRTableCell xrTableCell65;
    private XRTableCell xrTableCell96;
    private XRTableCell xrTableCell94;
    private XRTableCell xrTableCell77;
    private XRTableCell xrTableCell105;
    private XRTableCell xrTableCell106;
    private XRTableCell xrTableCell107;
    private XRTableCell xrTableCell108;
    private XRTableCell xrTableCell109;
    private XRTableCell xrTableCell98;
    private XRTableCell xrTableCell102;
    private XRTableCell xrTableCell23;
    private XRLabel xrLabel75;
    private XRLabel xrLabel72;
    private XRLabel xrLabel65;
    private XRTableCell xrTableCell111;
    private XRTable xrTable7;
    private XRTableRow xrTableRow43;
    private XRTableCell xrTableCell113;
    private XRTableCell xrTableCell114;
    private XRTableCell xrTableCell115;
    private XRTableCell xrTableCell117;
    private XRTableCell xrTableCell118;
    private XRTableCell xrTableCell119;
    private XRTableCell xrTableCell120;
    private XRTableCell xrTableCell121;
    private XRTableCell xrTableCell122;
    private XRLabel xrLabel83;
    private CalculatedField buque;
    private XRTableCell xrTableCell13;
    private XRLabel xrLabel84;
    private XRTableRow xrTableRow7;
    private XRTableCell xrTableCell17;
    private XRTableRow xrTableRow47;
    private XRTableCell xrTableCell21;
    private XRTableRow xrTableRow46;
    private XRTableCell xrTableCell16;
    private XRTableCell xrTableCell19;
    private XRLabel xrLabel12;
    private XRTableCell xrTableCell123;
    private XRLabel xrLabel13;
    private XRLabel xrLabel77;
    private XRLabel xrLabel78;
    private XRPictureBox xrPictureBox1;
    private CalculatedField ciudadembarque;
    private CalculatedField ciudaddestino;
    private CalculatedField ciudaddescarga;
    private XRTableRow xrTableRow45;
    private XRTableCell xrTableCell20;
    private CalculatedField TotalTerm;
    private XRLabel xrLabel47;
    private XRLabel xrLabel76;
    private XRTableRow xrTableRow48;
    private XRTableCell xrTableCell124;
    private XRTableCell xrTableCell125;
    private CalculatedField calculatedField7;
    private CalculatedField archivoXML;
    private CalculatedField CON;
    private DevExpress.XtraReports.Parameters.Parameter id_InvoiceExterior;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public InvoiceExteriorReport()
    {
        InitializeComponent();
        
        using(DBContext db = new DBContext())
        {
            //int id = (int)this.id_company.Value;
            //Company company = db.Company.FirstOrDefault(c => c.id == id);

            //if(company != null)
            //{
            //    byte[] data = company.logo;
            //    MemoryStream stream = new MemoryStream(data);
            //    xrPictureBoxLogo.Image = Image.FromStream(stream);

            //    //xrLabelBuisinessName.Text = company.businessName;
            //    //xrLabelRuc.Text = company.ruc;

            //    //xrLabelAddress.Text = company.address;
            //    //xrLabelPhone.Text = company.phoneNumber;
            //}
            
        }
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            DevExpress.DataAccess.EntityFramework.EFConnectionParameters efConnectionParameters1 = new DevExpress.DataAccess.EntityFramework.EFConnectionParameters();
            DevExpress.XtraPrinting.BarCode.CodabarGenerator codabarGenerator1 = new DevExpress.XtraPrinting.BarCode.CodabarGenerator();
            DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery1 = new DevExpress.DataAccess.Sql.StoredProcQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvoiceExteriorReport));
            this.efDataSource1 = new DevExpress.DataAccess.EntityFramework.EFDataSource(this.components);
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable7 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow43 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell113 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell114 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell115 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel57 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell117 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel55 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell118 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel58 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell119 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel60 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell120 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel61 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell121 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel62 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell122 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel63 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrPictureBox1 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrPanel3 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrTable3 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow8 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell22 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell23 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell24 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel56 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow9 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell25 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell26 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel74 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell27 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell29 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel73 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel78 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel77 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel75 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel72 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel65 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanel2 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel80 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow4 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel84 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow5 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow6 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell18 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow7 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell17 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow45 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell20 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow47 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell19 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell21 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow46 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell123 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrBarCode1 = new DevExpress.XtraReports.UI.XRBarCode();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrTable5 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow24 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell105 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel32 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell58 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow26 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell106 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel33 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell62 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow25 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell107 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel34 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell59 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow27 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell108 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel35 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell64 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow28 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell66 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel36 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell67 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel66 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow29 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell68 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel37 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell69 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel67 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow30 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell70 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel38 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell71 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel68 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow31 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell72 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel39 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell73 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel69 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow32 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell74 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel40 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell75 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel70 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow34 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell78 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel41 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell79 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel71 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow33 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell109 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel42 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell76 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow36 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell82 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel43 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell83 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow35 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell80 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel44 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell81 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow37 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell84 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel45 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell85 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow39 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell88 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel46 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell89 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow38 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell86 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel83 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell87 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrPanel6 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrTable6 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow40 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell90 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel49 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell91 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel50 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell93 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel51 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell92 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel52 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow41 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell98 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell95 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell102 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell97 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrPanel4 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrTable4 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow10 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell28 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel48 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell30 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow11 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell31 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell32 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow12 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell33 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell34 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow13 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell35 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell36 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow14 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell37 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell38 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow15 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell39 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell40 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow16 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell41 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell42 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow17 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell111 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel26 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell44 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow18 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell43 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel27 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell47 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow42 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell45 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel53 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell99 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow19 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell46 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel28 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell48 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow20 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell49 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel29 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell52 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell50 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell55 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell51 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow21 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell60 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel30 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell63 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell61 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell65 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell53 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow44 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell112 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel81 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell96 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell94 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell77 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell116 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel82 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow22 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell54 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel31 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell101 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell100 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell103 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow23 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell56 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel47 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell57 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel76 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRow48 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell124 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell125 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel59 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.formapago = new DevExpress.XtraReports.UI.CalculatedField();
            this.plazo = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField1 = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField2 = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField3 = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField4 = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField5 = new DevExpress.XtraReports.UI.CalculatedField();
            this.Netokilo = new DevExpress.XtraReports.UI.CalculatedField();
            this.netolibra = new DevExpress.XtraReports.UI.CalculatedField();
            this.brutokilo = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField6 = new DevExpress.XtraReports.UI.CalculatedField();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.buque = new DevExpress.XtraReports.UI.CalculatedField();
            this.ciudadembarque = new DevExpress.XtraReports.UI.CalculatedField();
            this.ciudaddestino = new DevExpress.XtraReports.UI.CalculatedField();
            this.ciudaddescarga = new DevExpress.XtraReports.UI.CalculatedField();
            this.TotalTerm = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField7 = new DevExpress.XtraReports.UI.CalculatedField();
            this.archivoXML = new DevExpress.XtraReports.UI.CalculatedField();
            this.CON = new DevExpress.XtraReports.UI.CalculatedField();
            this.id_InvoiceExterior = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.efDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // efDataSource1
            // 
            efConnectionParameters1.ConnectionString = "";
            efConnectionParameters1.ConnectionStringName = "DBContext";
            efConnectionParameters1.Source = typeof(DXPANACEASOFT.Models.DBContext);
            this.efDataSource1.ConnectionParameters = efConnectionParameters1;
            this.efDataSource1.Name = "efDataSource1";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable7});
            this.Detail.HeightF = 24.97915F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable7
            // 
            this.xrTable7.LocationFloat = new DevExpress.Utils.PointFloat(8.267177F, 0F);
            this.xrTable7.Name = "xrTable7";
            this.xrTable7.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow43});
            this.xrTable7.SizeF = new System.Drawing.SizeF(790.7329F, 24.97915F);
            // 
            // xrTableRow43
            // 
            this.xrTableRow43.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableRow43.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell113,
            this.xrTableCell114,
            this.xrTableCell115,
            this.xrTableCell117,
            this.xrTableCell118,
            this.xrTableCell119,
            this.xrTableCell120,
            this.xrTableCell121,
            this.xrTableCell122});
            this.xrTableRow43.Name = "xrTableRow43";
            this.xrTableRow43.StylePriority.UseBorders = false;
            this.xrTableRow43.Weight = 1D;
            // 
            // xrTableCell113
            // 
            this.xrTableCell113.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1});
            this.xrTableCell113.Name = "xrTableCell113";
            this.xrTableCell113.Text = "xrTableCell113";
            this.xrTableCell113.Weight = 1.0416663390685226D;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[auxCode]")});
            this.xrLabel1.Font = new System.Drawing.Font("Calibri", 6.5F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(1.852459F, 6.152815F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(100.9342F, 10.84722F);
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "xrLabel1";
            // 
            // xrTableCell114
            // 
            this.xrTableCell114.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4});
            this.xrTableCell114.Name = "xrTableCell114";
            this.xrTableCell114.Text = "xrTableCell114";
            this.xrTableCell114.Weight = 0.48539023278311949D;
            // 
            // xrLabel4
            // 
            this.xrLabel4.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[masterCode]")});
            this.xrLabel4.Font = new System.Drawing.Font("Calibri", 6.5F);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(6.67572E-06F, 5.999962F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(47.24458F, 10.84722F);
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = "xrLabel4";
            // 
            // xrTableCell115
            // 
            this.xrTableCell115.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel57});
            this.xrTableCell115.Name = "xrTableCell115";
            this.xrTableCell115.Text = "xrTableCell115";
            this.xrTableCell115.Weight = 0.5276488571032385D;
            // 
            // xrLabel57
            // 
            this.xrLabel57.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel57.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[id_amountInvoice]")});
            this.xrLabel57.Font = new System.Drawing.Font("Calibri", 6.5F);
            this.xrLabel57.LocationFloat = new DevExpress.Utils.PointFloat(1F, 5.999961F);
            this.xrLabel57.Name = "xrLabel57";
            this.xrLabel57.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel57.SizeF = new System.Drawing.SizeF(50.08866F, 10.84722F);
            this.xrLabel57.StylePriority.UseBorders = false;
            this.xrLabel57.StylePriority.UseFont = false;
            this.xrLabel57.StylePriority.UseTextAlignment = false;
            this.xrLabel57.Text = "xrLabel57";
            this.xrLabel57.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel57.TextFormatString = "{0:#.00}";
            // 
            // xrTableCell117
            // 
            this.xrTableCell117.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel55});
            this.xrTableCell117.Name = "xrTableCell117";
            this.xrTableCell117.Text = "xrTableCell117";
            this.xrTableCell117.Weight = 2.449094973721798D;
            // 
            // xrLabel55
            // 
            this.xrLabel55.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel55.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[name]")});
            this.xrLabel55.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            this.xrLabel55.LocationFloat = new DevExpress.Utils.PointFloat(0F, 6.999969F);
            this.xrLabel55.Name = "xrLabel55";
            this.xrLabel55.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel55.SizeF = new System.Drawing.SizeF(236.3672F, 10.84722F);
            this.xrLabel55.StylePriority.UseBorders = false;
            this.xrLabel55.StylePriority.UseFont = false;
            this.xrLabel55.StylePriority.UseTextAlignment = false;
            this.xrLabel55.Text = "xrLabel55";
            this.xrLabel55.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrTableCell118
            // 
            this.xrTableCell118.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel58});
            this.xrTableCell118.Name = "xrTableCell118";
            this.xrTableCell118.Text = "xrTableCell118";
            this.xrTableCell118.Weight = 0.64702555722178867D;
            // 
            // xrLabel58
            // 
            this.xrLabel58.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel58.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[numBoxes]")});
            this.xrLabel58.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            this.xrLabel58.LocationFloat = new DevExpress.Utils.PointFloat(5.000016F, 6.999969F);
            this.xrLabel58.Name = "xrLabel58";
            this.xrLabel58.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel58.SizeF = new System.Drawing.SizeF(56.08762F, 10.84722F);
            this.xrLabel58.StylePriority.UseBorders = false;
            this.xrLabel58.StylePriority.UseFont = false;
            this.xrLabel58.StylePriority.UseTextAlignment = false;
            this.xrLabel58.Text = "xrLabel58";
            this.xrLabel58.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel58.TextFormatString = "{0:#.00}";
            // 
            // xrTableCell119
            // 
            this.xrTableCell119.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel60});
            this.xrTableCell119.Name = "xrTableCell119";
            this.xrTableCell119.Text = "xrTableCell119";
            this.xrTableCell119.Weight = 0.54426168901076644D;
            // 
            // xrLabel60
            // 
            this.xrLabel60.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel60.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[code]")});
            this.xrLabel60.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            this.xrLabel60.LocationFloat = new DevExpress.Utils.PointFloat(1F, 6.999982F);
            this.xrLabel60.Name = "xrLabel60";
            this.xrLabel60.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel60.SizeF = new System.Drawing.SizeF(51.75012F, 10.84722F);
            this.xrLabel60.StylePriority.UseBorders = false;
            this.xrLabel60.StylePriority.UseFont = false;
            this.xrLabel60.StylePriority.UseTextAlignment = false;
            this.xrLabel60.Text = "xrLabel60";
            this.xrLabel60.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrTableCell120
            // 
            this.xrTableCell120.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel61});
            this.xrTableCell120.Name = "xrTableCell120";
            this.xrTableCell120.Text = "xrTableCell120";
            this.xrTableCell120.Weight = 0.68644884662249572D;
            // 
            // xrLabel61
            // 
            this.xrLabel61.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel61.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[unitPrice]")});
            this.xrLabel61.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            this.xrLabel61.LocationFloat = new DevExpress.Utils.PointFloat(6.589636F, 6.999969F);
            this.xrLabel61.Name = "xrLabel61";
            this.xrLabel61.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel61.SizeF = new System.Drawing.SizeF(58.46378F, 10.84723F);
            this.xrLabel61.StylePriority.UseBorders = false;
            this.xrLabel61.StylePriority.UseFont = false;
            this.xrLabel61.StylePriority.UseTextAlignment = false;
            this.xrLabel61.Text = "xrLabel61";
            this.xrLabel61.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel61.TextFormatString = "{0:$###,###,##0.0000}";
            // 
            // xrTableCell121
            // 
            this.xrTableCell121.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel62});
            this.xrTableCell121.Name = "xrTableCell121";
            this.xrTableCell121.Text = "xrTableCell121";
            this.xrTableCell121.Weight = 0.65999441385125568D;
            // 
            // xrLabel62
            // 
            this.xrLabel62.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel62.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[discount]")});
            this.xrLabel62.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            this.xrLabel62.LocationFloat = new DevExpress.Utils.PointFloat(2.000014F, 5.999989F);
            this.xrLabel62.Name = "xrLabel62";
            this.xrLabel62.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel62.SizeF = new System.Drawing.SizeF(62.42523F, 13.00001F);
            this.xrLabel62.StylePriority.UseBorders = false;
            this.xrLabel62.StylePriority.UseFont = false;
            this.xrLabel62.StylePriority.UseTextAlignment = false;
            this.xrLabel62.Text = "xrLabel62";
            this.xrLabel62.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel62.TextFormatString = "{0:$###,###,##0.00}";
            // 
            // xrTableCell122
            // 
            this.xrTableCell122.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel63});
            this.xrTableCell122.Name = "xrTableCell122";
            this.xrTableCell122.Text = "xrTableCell122";
            this.xrTableCell122.Weight = 0.81901123539240528D;
            // 
            // xrLabel63
            // 
            this.xrLabel63.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel63.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Total]")});
            this.xrLabel63.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            this.xrLabel63.LocationFloat = new DevExpress.Utils.PointFloat(2F, 5.999992F);
            this.xrLabel63.Name = "xrLabel63";
            this.xrLabel63.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel63.SizeF = new System.Drawing.SizeF(77.84691F, 13.00001F);
            this.xrLabel63.StylePriority.UseBorders = false;
            this.xrLabel63.StylePriority.UseFont = false;
            this.xrLabel63.StylePriority.UseTextAlignment = false;
            this.xrLabel63.Text = "xrLabel63";
            this.xrLabel63.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel63.TextFormatString = "{0:$###,###,##0.00}";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 0F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 100F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox1,
            this.xrTable1,
            this.xrPanel3,
            this.xrPanel1,
            this.xrPanel2});
            this.ReportHeader.HeightF = 373.2361F;
            this.ReportHeader.Name = "ReportHeader";
            this.ReportHeader.StylePriority.UseTextAlignment = false;
            this.ReportHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrPictureBox1
            // 
            this.xrPictureBox1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Image", "[logo]")});
            this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(12.43382F, 12.87499F);
            this.xrPictureBox1.Name = "xrPictureBox1";
            this.xrPictureBox1.SizeF = new System.Drawing.SizeF(374.0981F, 72.52258F);
            this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // xrTable1
            // 
            this.xrTable1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable1.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(8.267217F, 341.1527F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 0, 0, 0, 100F);
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(790.7328F, 32.08334F);
            this.xrTable1.StylePriority.UseBackColor = false;
            this.xrTable1.StylePriority.UseBorders = false;
            this.xrTable1.StylePriority.UseFont = false;
            this.xrTable1.StylePriority.UsePadding = false;
            this.xrTable1.StylePriority.UseTextAlignment = false;
            this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell2,
            this.xrTableCell3,
            this.xrTableCell4,
            this.xrTableCell5,
            this.xrTableCell6,
            this.xrTableCell7,
            this.xrTableCell8,
            this.xrTableCell9});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.CanShrink = true;
            this.xrTableCell1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.StylePriority.UseFont = false;
            this.xrTableCell1.StylePriority.UseTextAlignment = false;
            this.xrTableCell1.Text = "Código Principal";
            this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell1.Weight = 1.1772810674216281D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.StylePriority.UseFont = false;
            this.xrTableCell2.StylePriority.UseTextAlignment = false;
            this.xrTableCell2.Text = "Código Auxiliar";
            this.xrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleJustify;
            this.xrTableCell2.Weight = 0.55326472784975989D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.StylePriority.UseFont = false;
            this.xrTableCell3.StylePriority.UseTextAlignment = false;
            this.xrTableCell3.Text = "Cant.";
            this.xrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell3.Weight = 0.59166364637744528D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.StylePriority.UseFont = false;
            this.xrTableCell4.StylePriority.UseTextAlignment = false;
            this.xrTableCell4.Text = "Descripción";
            this.xrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell4.Weight = 2.7679438884357648D;
            // 
            // xrTableCell5
            // 
            this.xrTableCell5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.StylePriority.UseFont = false;
            this.xrTableCell5.Text = "Cartones";
            this.xrTableCell5.Weight = 0.740529016416519D;
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.StylePriority.UseFont = false;
            this.xrTableCell6.StylePriority.UseTextAlignment = false;
            this.xrTableCell6.Text = "Unidad";
            this.xrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell6.Weight = 0.60585281666032242D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.StylePriority.UseFont = false;
            this.xrTableCell7.StylePriority.UseTextAlignment = false;
            this.xrTableCell7.Text = "Precio";
            this.xrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell7.Weight = 0.77581883509842187D;
            // 
            // xrTableCell8
            // 
            this.xrTableCell8.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell8.Name = "xrTableCell8";
            this.xrTableCell8.StylePriority.UseBorders = false;
            this.xrTableCell8.StylePriority.UseFont = false;
            this.xrTableCell8.StylePriority.UseTextAlignment = false;
            this.xrTableCell8.Text = "Dscto.";
            this.xrTableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell8.Weight = 0.74591884844385214D;
            // 
            // xrTableCell9
            // 
            this.xrTableCell9.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell9.Name = "xrTableCell9";
            this.xrTableCell9.StylePriority.UseBorders = false;
            this.xrTableCell9.StylePriority.UseFont = false;
            this.xrTableCell9.StylePriority.UseTextAlignment = false;
            this.xrTableCell9.Text = "Precio Total";
            this.xrTableCell9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell9.Weight = 0.9256377990159057D;
            // 
            // xrPanel3
            // 
            this.xrPanel3.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPanel3.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable3});
            this.xrPanel3.LocationFloat = new DevExpress.Utils.PointFloat(8.26719F, 287.6111F);
            this.xrPanel3.Name = "xrPanel3";
            this.xrPanel3.SizeF = new System.Drawing.SizeF(790.7329F, 51.66666F);
            this.xrPanel3.StylePriority.UseBorders = false;
            // 
            // xrTable3
            // 
            this.xrTable3.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTable3.LocationFloat = new DevExpress.Utils.PointFloat(4.166635F, 5.041749F);
            this.xrTable3.Name = "xrTable3";
            this.xrTable3.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow8,
            this.xrTableRow9});
            this.xrTable3.SizeF = new System.Drawing.SizeF(777.8334F, 44.99989F);
            this.xrTable3.StylePriority.UseBorders = false;
            // 
            // xrTableRow8
            // 
            this.xrTableRow8.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell22,
            this.xrTableCell23,
            this.xrTableCell24});
            this.xrTableRow8.Name = "xrTableRow8";
            this.xrTableRow8.Weight = 1D;
            // 
            // xrTableCell22
            // 
            this.xrTableCell22.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell22.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel15});
            this.xrTableCell22.Name = "xrTableCell22";
            this.xrTableCell22.StylePriority.UseBorders = false;
            this.xrTableCell22.Text = "xrTableCell22";
            this.xrTableCell22.Weight = 2.486875322468411D;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(202.5269F, 24.99997F);
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "Razón Social/Nombres y Apellidos:";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableCell23
            // 
            this.xrTableCell23.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[fullname_businessName]")});
            this.xrTableCell23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell23.Name = "xrTableCell23";
            this.xrTableCell23.StylePriority.UseFont = false;
            this.xrTableCell23.Text = "xrTableCell23";
            this.xrTableCell23.Weight = 3.1208569163011197D;
            // 
            // xrTableCell24
            // 
            this.xrTableCell24.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel56,
            this.xrLabel17});
            this.xrTableCell24.Name = "xrTableCell24";
            this.xrTableCell24.Text = "xrTableCell24";
            this.xrTableCell24.Weight = 3.9434646606445312D;
            // 
            // xrLabel56
            // 
            this.xrLabel56.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[identification_number]")});
            this.xrLabel56.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel56.LocationFloat = new DevExpress.Utils.PointFloat(168.0084F, 0F);
            this.xrLabel56.Name = "xrLabel56";
            this.xrLabel56.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel56.SizeF = new System.Drawing.SizeF(149.6494F, 22.99999F);
            this.xrLabel56.StylePriority.UseFont = false;
            this.xrLabel56.Text = "xrLabel56";
            // 
            // xrLabel17
            // 
            this.xrLabel17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(10.0001F, 1.999982F);
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(158.0083F, 22.99998F);
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.Text = "Ruc:/Cédula:";
            // 
            // xrTableRow9
            // 
            this.xrTableRow9.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell25,
            this.xrTableCell26,
            this.xrTableCell27,
            this.xrTableCell29});
            this.xrTableRow9.Name = "xrTableRow9";
            this.xrTableRow9.Weight = 1D;
            // 
            // xrTableCell25
            // 
            this.xrTableCell25.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel16});
            this.xrTableCell25.Name = "xrTableCell25";
            this.xrTableCell25.Text = "xrTableCell25";
            this.xrTableCell25.Weight = 2.4868751564059455D;
            // 
            // xrLabel16
            // 
            this.xrLabel16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(0F, 2.000046F);
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.StylePriority.UseTextAlignment = false;
            this.xrLabel16.Text = "Fecha Emisión:";
            this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableCell26
            // 
            this.xrTableCell26.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel74});
            this.xrTableCell26.Name = "xrTableCell26";
            this.xrTableCell26.Text = "xrTableCell26";
            this.xrTableCell26.Weight = 3.120857234951476D;
            // 
            // xrLabel74
            // 
            this.xrLabel74.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[emissionDate]")});
            this.xrLabel74.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel74.LocationFloat = new DevExpress.Utils.PointFloat(32.19797F, 1.99995F);
            this.xrLabel74.Name = "xrLabel74";
            this.xrLabel74.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel74.SizeF = new System.Drawing.SizeF(221.9594F, 23.00005F);
            this.xrLabel74.StylePriority.UseFont = false;
            this.xrLabel74.TextFormatString = "{0:dd/MM/yyyy HH:mm}";
            // 
            // xrTableCell27
            // 
            this.xrTableCell27.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel18});
            this.xrTableCell27.Name = "xrTableCell27";
            this.xrTableCell27.Text = "xrTableCell27";
            this.xrTableCell27.Weight = 2.0630145213876867D;
            // 
            // xrLabel18
            // 
            this.xrLabel18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(52.56499F, 0F);
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(114.0331F, 22.49994F);
            this.xrLabel18.StylePriority.UseFont = false;
            this.xrLabel18.Text = "Guia Remisión:";
            // 
            // xrTableCell29
            // 
            this.xrTableCell29.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel73});
            this.xrTableCell29.Name = "xrTableCell29";
            this.xrTableCell29.Text = "xrTableCell29";
            this.xrTableCell29.Weight = 1.8804499866689535D;
            // 
            // xrLabel73
            // 
            this.xrLabel73.LocationFloat = new DevExpress.Utils.PointFloat(20.56248F, 0F);
            this.xrLabel73.Name = "xrLabel73";
            this.xrLabel73.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel73.SizeF = new System.Drawing.SizeF(100F, 23F);
            // 
            // xrPanel1
            // 
            this.xrPanel1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel78,
            this.xrLabel77,
            this.xrLabel75,
            this.xrLabel72,
            this.xrLabel65,
            this.xrLabel6,
            this.xrLabel5,
            this.xrLabel3,
            this.xrLabel2});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(9.100406F, 97.14438F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(377.4315F, 164.4875F);
            this.xrPanel1.StylePriority.UseBorders = false;
            // 
            // xrLabel78
            // 
            this.xrLabel78.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel78.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ForzadoContabilidad]")});
            this.xrLabel78.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrLabel78.LocationFloat = new DevExpress.Utils.PointFloat(182.9167F, 129.314F);
            this.xrLabel78.Name = "xrLabel78";
            this.xrLabel78.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel78.SizeF = new System.Drawing.SizeF(181.1813F, 14.04171F);
            this.xrLabel78.StylePriority.UseBorders = false;
            this.xrLabel78.StylePriority.UseFont = false;
            this.xrLabel78.Text = "xrLabel78";
            // 
            // xrLabel77
            // 
            this.xrLabel77.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel77.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NumeroContribuyente]")});
            this.xrLabel77.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrLabel77.LocationFloat = new DevExpress.Utils.PointFloat(182.9167F, 106.7868F);
            this.xrLabel77.Name = "xrLabel77";
            this.xrLabel77.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel77.SizeF = new System.Drawing.SizeF(181.1813F, 15.2341F);
            this.xrLabel77.StylePriority.UseBorders = false;
            this.xrLabel77.StylePriority.UseFont = false;
            this.xrLabel77.Text = "xrLabel77";
            // 
            // xrLabel75
            // 
            this.xrLabel75.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel75.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BranchOffice_Address]")});
            this.xrLabel75.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrLabel75.LocationFloat = new DevExpress.Utils.PointFloat(109.9999F, 75.47916F);
            this.xrLabel75.Name = "xrLabel75";
            this.xrLabel75.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel75.SizeF = new System.Drawing.SizeF(254.0981F, 23.00002F);
            this.xrLabel75.StylePriority.UseBorders = false;
            this.xrLabel75.StylePriority.UseFont = false;
            this.xrLabel75.StylePriority.UseTextAlignment = false;
            this.xrLabel75.Text = "xrLabel75";
            this.xrLabel75.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrLabel72
            // 
            this.xrLabel72.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel72.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[address]")});
            this.xrLabel72.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrLabel72.LocationFloat = new DevExpress.Utils.PointFloat(110F, 52.47913F);
            this.xrLabel72.Name = "xrLabel72";
            this.xrLabel72.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel72.SizeF = new System.Drawing.SizeF(254.0981F, 23.00002F);
            this.xrLabel72.StylePriority.UseBorders = false;
            this.xrLabel72.StylePriority.UseFont = false;
            this.xrLabel72.StylePriority.UseTextAlignment = false;
            this.xrLabel72.Text = "xrLabel72";
            this.xrLabel72.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrLabel65
            // 
            this.xrLabel65.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel65.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Trademark_Company]")});
            this.xrLabel65.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel65.LocationFloat = new DevExpress.Utils.PointFloat(5.899429F, 7.0165F);
            this.xrLabel65.Name = "xrLabel65";
            this.xrLabel65.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel65.SizeF = new System.Drawing.SizeF(281.6014F, 28.19581F);
            this.xrLabel65.StylePriority.UseBorders = false;
            this.xrLabel65.StylePriority.UseFont = false;
            this.xrLabel65.StylePriority.UseTextAlignment = false;
            this.xrLabel65.Text = "xrLabel65";
            this.xrLabel65.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel6.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(10.06619F, 130.3556F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(172.8505F, 13.00003F);
            this.xrLabel6.StylePriority.UseBorders = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "OBLIGADO LLEVAR CONTABILIDAD:";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel5
            // 
            this.xrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel5.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(9.999943F, 105.4792F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(172.9167F, 16.54167F);
            this.xrLabel5.StylePriority.UseBorders = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "Contribuyente E·special Nro.:";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel3.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(9.999942F, 75.47917F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel3.StylePriority.UseBorders = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "Dir. Sucursal:";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel2.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(9.999963F, 52.47918F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Dir. Matriz:";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPanel2
            // 
            this.xrPanel2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPanel2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel14,
            this.xrTable2,
            this.xrBarCode1});
            this.xrPanel2.LocationFloat = new DevExpress.Utils.PointFloat(392F, 10F);
            this.xrPanel2.Name = "xrPanel2";
            this.xrPanel2.SizeF = new System.Drawing.SizeF(407.0001F, 251.6319F);
            this.xrPanel2.StylePriority.UseBorders = false;
            // 
            // xrLabel14
            // 
            this.xrLabel14.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel14.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(12.33951F, 169.2141F);
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(122.9009F, 16.87505F);
            this.xrLabel14.StylePriority.UseBorders = false;
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.StylePriority.UseTextAlignment = false;
            this.xrLabel14.Text = "CLAVE DE ACCESO:";
            this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable2
            // 
            this.xrTable2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(10.23976F, 2.624988F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2,
            this.xrTableRow3,
            this.xrTableRow4,
            this.xrTableRow5,
            this.xrTableRow6,
            this.xrTableRow7,
            this.xrTableRow45,
            this.xrTableRow47,
            this.xrTableRow46});
            this.xrTable2.SizeF = new System.Drawing.SizeF(386.7603F, 166.5891F);
            this.xrTable2.StylePriority.UseBorders = false;
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell11,
            this.xrTableCell12});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 1D;
            // 
            // xrTableCell11
            // 
            this.xrTableCell11.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel7});
            this.xrTableCell11.Name = "xrTableCell11";
            this.xrTableCell11.Text = "xrTableCell11";
            this.xrTableCell11.Weight = 0.49591611383856571D;
            // 
            // xrLabel7
            // 
            this.xrLabel7.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel7.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(0.2398226F, 0.2499951F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(45.80161F, 17.94316F);
            this.xrLabel7.StylePriority.UseBorders = false;
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "R.U.C.:";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableCell12
            // 
            this.xrTableCell12.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel80});
            this.xrTableCell12.Name = "xrTableCell12";
            this.xrTableCell12.Text = "xrTableCell12";
            this.xrTableCell12.Weight = 3.4023252833547506D;
            // 
            // xrLabel80
            // 
            this.xrLabel80.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ruc]")});
            this.xrLabel80.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel80.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0.2499951F);
            this.xrLabel80.Name = "xrLabel80";
            this.xrLabel80.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel80.SizeF = new System.Drawing.SizeF(210.175F, 17.94316F);
            this.xrLabel80.StylePriority.UseFont = false;
            this.xrLabel80.StylePriority.UseTextAlignment = false;
            this.xrLabel80.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableRow3
            // 
            this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell15});
            this.xrTableRow3.Name = "xrTableRow3";
            this.xrTableRow3.Weight = 1D;
            // 
            // xrTableCell15
            // 
            this.xrTableCell15.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel8});
            this.xrTableCell15.Name = "xrTableCell15";
            this.xrTableCell15.Text = "xrTableCell15";
            this.xrTableCell15.Weight = 3.8982413971933161D;
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0.2398376F, 0F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(125F, 18.19315F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "FACTURA";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableRow4
            // 
            this.xrTableRow4.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell13,
            this.xrTableCell10});
            this.xrTableRow4.Name = "xrTableRow4";
            this.xrTableRow4.Weight = 1D;
            // 
            // xrTableCell13
            // 
            this.xrTableCell13.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel84});
            this.xrTableCell13.Name = "xrTableCell13";
            this.xrTableCell13.StylePriority.UseTextAlignment = false;
            this.xrTableCell13.Text = "xrTableCell13";
            this.xrTableCell13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell13.Weight = 0.305475223030259D;
            // 
            // xrLabel84
            // 
            this.xrLabel84.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel84.LocationFloat = new DevExpress.Utils.PointFloat(3.808975F, 0F);
            this.xrLabel84.Name = "xrLabel84";
            this.xrLabel84.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel84.SizeF = new System.Drawing.SizeF(23.99444F, 17.19315F);
            this.xrLabel84.StylePriority.UseFont = false;
            this.xrLabel84.StylePriority.UseTextAlignment = false;
            this.xrLabel84.Text = "No. ";
            this.xrLabel84.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableCell10
            // 
            this.xrTableCell10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[number]")});
            this.xrTableCell10.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.xrTableCell10.Name = "xrTableCell10";
            this.xrTableCell10.StylePriority.UseFont = false;
            this.xrTableCell10.StylePriority.UseTextAlignment = false;
            this.xrTableCell10.Text = "xrTableCell10";
            this.xrTableCell10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell10.Weight = 3.5927661741630574D;
            // 
            // xrTableRow5
            // 
            this.xrTableRow5.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell14});
            this.xrTableRow5.Name = "xrTableRow5";
            this.xrTableRow5.Weight = 0.999998979236064D;
            // 
            // xrTableCell14
            // 
            this.xrTableCell14.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel9});
            this.xrTableCell14.Name = "xrTableCell14";
            this.xrTableCell14.Text = "xrTableCell14";
            this.xrTableCell14.Weight = 3.8982413971933165D;
            // 
            // xrLabel9
            // 
            this.xrLabel9.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(0F, 9.536743E-07F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(188.9979F, 13.38629F);
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.Text = "Número de Autorización :";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableRow6
            // 
            this.xrTableRow6.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell18});
            this.xrTableRow6.Name = "xrTableRow6";
            this.xrTableRow6.Weight = 1.0313388704569477D;
            // 
            // xrTableCell18
            // 
            this.xrTableCell18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[authorizationNumber]")});
            this.xrTableCell18.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrTableCell18.Name = "xrTableCell18";
            this.xrTableCell18.StylePriority.UseFont = false;
            this.xrTableCell18.StylePriority.UseTextAlignment = false;
            this.xrTableCell18.Text = "xrTableCell18";
            this.xrTableCell18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell18.Weight = 3.8982413971933161D;
            // 
            // xrTableRow7
            // 
            this.xrTableRow7.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell17});
            this.xrTableRow7.Name = "xrTableRow7";
            this.xrTableRow7.Weight = 1.0313388704569477D;
            // 
            // xrTableCell17
            // 
            this.xrTableCell17.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel11});
            this.xrTableCell17.Name = "xrTableCell17";
            this.xrTableCell17.Text = "xrTableCell17";
            this.xrTableCell17.Weight = 3.8982413971933161D;
            // 
            // xrLabel11
            // 
            this.xrLabel11.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(0.2399021F, 0F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(193.9158F, 16.66988F);
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.Text = "Fecha y Hora de Autorización :";
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableRow45
            // 
            this.xrTableRow45.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell20});
            this.xrTableRow45.Name = "xrTableRow45";
            this.xrTableRow45.Weight = 1.0313388704569477D;
            // 
            // xrTableCell20
            // 
            this.xrTableCell20.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AuthDate]")});
            this.xrTableCell20.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrTableCell20.Name = "xrTableCell20";
            this.xrTableCell20.StylePriority.UseBackColor = false;
            this.xrTableCell20.StylePriority.UseFont = false;
            this.xrTableCell20.StylePriority.UseTextAlignment = false;
            this.xrTableCell20.Text = "xrTableCell20";
            this.xrTableCell20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell20.TextFormatString = "{0:dd/MM/yyyy H:mm:ss}";
            this.xrTableCell20.Weight = 3.8982413971933161D;
            // 
            // xrTableRow47
            // 
            this.xrTableRow47.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell19,
            this.xrTableCell21});
            this.xrTableRow47.Name = "xrTableRow47";
            this.xrTableRow47.Weight = 1.0313388704569477D;
            // 
            // xrTableCell19
            // 
            this.xrTableCell19.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel12});
            this.xrTableCell19.Name = "xrTableCell19";
            this.xrTableCell19.Text = "xrTableCell19";
            this.xrTableCell19.Weight = 1.949120698596658D;
            // 
            // xrLabel12
            // 
            this.xrLabel12.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(137.3264F, 18.7633F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.StylePriority.UseTextAlignment = false;
            this.xrLabel12.Text = "Ambiente :";
            this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableCell21
            // 
            this.xrTableCell21.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EnvironmentDescription]")});
            this.xrTableCell21.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrTableCell21.Name = "xrTableCell21";
            this.xrTableCell21.StylePriority.UseFont = false;
            this.xrTableCell21.StylePriority.UseTextAlignment = false;
            this.xrTableCell21.Text = "xrTableCell21";
            this.xrTableCell21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell21.Weight = 1.949120698596658D;
            // 
            // xrTableRow46
            // 
            this.xrTableRow46.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell123,
            this.xrTableCell16});
            this.xrTableRow46.Name = "xrTableRow46";
            this.xrTableRow46.Weight = 1.0313388704569477D;
            // 
            // xrTableCell123
            // 
            this.xrTableCell123.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel13});
            this.xrTableCell123.Name = "xrTableCell123";
            this.xrTableCell123.Text = "xrTableCell123";
            this.xrTableCell123.Weight = 1.949120698596658D;
            // 
            // xrLabel13
            // 
            this.xrLabel13.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(137.3263F, 18.76332F);
            this.xrLabel13.StylePriority.UseFont = false;
            this.xrLabel13.StylePriority.UseTextAlignment = false;
            this.xrLabel13.Text = "Emisión";
            this.xrLabel13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableCell16
            // 
            this.xrTableCell16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EmissionTypeDescription]")});
            this.xrTableCell16.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrTableCell16.Name = "xrTableCell16";
            this.xrTableCell16.StylePriority.UseFont = false;
            this.xrTableCell16.StylePriority.UseTextAlignment = false;
            this.xrTableCell16.Text = "xrTableCell16";
            this.xrTableCell16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell16.Weight = 1.949120698596658D;
            // 
            // xrBarCode1
            // 
            this.xrBarCode1.AutoModule = true;
            this.xrBarCode1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrBarCode1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[accessKey]")});
            this.xrBarCode1.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrBarCode1.LocationFloat = new DevExpress.Utils.PointFloat(11F, 187.8735F);
            this.xrBarCode1.Name = "xrBarCode1";
            this.xrBarCode1.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.xrBarCode1.SizeF = new System.Drawing.SizeF(386F, 63.75842F);
            this.xrBarCode1.StylePriority.UseBorders = false;
            this.xrBarCode1.StylePriority.UseFont = false;
            codabarGenerator1.WideNarrowRatio = 2F;
            this.xrBarCode1.Symbology = codabarGenerator1;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable5,
            this.xrPanel6,
            this.xrPanel4});
            this.ReportFooter.HeightF = 440F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrTable5
            // 
            this.xrTable5.Font = new System.Drawing.Font("Calibri", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTable5.LocationFloat = new DevExpress.Utils.PointFloat(442.1057F, 16.28564F);
            this.xrTable5.Name = "xrTable5";
            this.xrTable5.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow24,
            this.xrTableRow26,
            this.xrTableRow25,
            this.xrTableRow27,
            this.xrTableRow28,
            this.xrTableRow29,
            this.xrTableRow30,
            this.xrTableRow31,
            this.xrTableRow32,
            this.xrTableRow34,
            this.xrTableRow33,
            this.xrTableRow36,
            this.xrTableRow35,
            this.xrTableRow37,
            this.xrTableRow39,
            this.xrTableRow38});
            this.xrTable5.SizeF = new System.Drawing.SizeF(349.8943F, 352.9845F);
            this.xrTable5.StylePriority.UseFont = false;
            // 
            // xrTableRow24
            // 
            this.xrTableRow24.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell105,
            this.xrTableCell58});
            this.xrTableRow24.Name = "xrTableRow24";
            this.xrTableRow24.Weight = 1D;
            // 
            // xrTableCell105
            // 
            this.xrTableCell105.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel32});
            this.xrTableCell105.Name = "xrTableCell105";
            this.xrTableCell105.Text = "xrTableCell105";
            this.xrTableCell105.Weight = 1D;
            // 
            // xrLabel32
            // 
            this.xrLabel32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel32.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel32.Name = "xrLabel32";
            this.xrLabel32.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel32.SizeF = new System.Drawing.SizeF(160.4167F, 19.28561F);
            this.xrLabel32.StylePriority.UseFont = false;
            this.xrLabel32.StylePriority.UseTextAlignment = false;
            this.xrLabel32.Text = "Subtotal 12%";
            this.xrLabel32.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell58
            // 
            this.xrTableCell58.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[subtotalIVA]")});
            this.xrTableCell58.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell58.Name = "xrTableCell58";
            this.xrTableCell58.StylePriority.UseFont = false;
            this.xrTableCell58.StylePriority.UseTextAlignment = false;
            this.xrTableCell58.Text = "xrTableCell58";
            this.xrTableCell58.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell58.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell58.Weight = 1D;
            // 
            // xrTableRow26
            // 
            this.xrTableRow26.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell106,
            this.xrTableCell62});
            this.xrTableRow26.Name = "xrTableRow26";
            this.xrTableRow26.Weight = 1D;
            // 
            // xrTableCell106
            // 
            this.xrTableCell106.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel33});
            this.xrTableCell106.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell106.Name = "xrTableCell106";
            this.xrTableCell106.StylePriority.UseFont = false;
            this.xrTableCell106.Text = "xrTableCell106";
            this.xrTableCell106.Weight = 1D;
            // 
            // xrLabel33
            // 
            this.xrLabel33.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel33.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel33.Name = "xrLabel33";
            this.xrLabel33.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel33.SizeF = new System.Drawing.SizeF(160.4167F, 22.61895F);
            this.xrLabel33.StylePriority.UseFont = false;
            this.xrLabel33.StylePriority.UseTextAlignment = false;
            this.xrLabel33.Text = "Subtotal 0%";
            this.xrLabel33.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell62
            // 
            this.xrTableCell62.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[subTotalIVA0]")});
            this.xrTableCell62.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell62.Name = "xrTableCell62";
            this.xrTableCell62.StylePriority.UseFont = false;
            this.xrTableCell62.StylePriority.UseTextAlignment = false;
            this.xrTableCell62.Text = "xrTableCell62";
            this.xrTableCell62.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell62.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell62.Weight = 1D;
            // 
            // xrTableRow25
            // 
            this.xrTableRow25.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell107,
            this.xrTableCell59});
            this.xrTableRow25.Name = "xrTableRow25";
            this.xrTableRow25.Weight = 1D;
            // 
            // xrTableCell107
            // 
            this.xrTableCell107.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel34});
            this.xrTableCell107.Name = "xrTableCell107";
            this.xrTableCell107.Text = "xrTableCell107";
            this.xrTableCell107.Weight = 1D;
            // 
            // xrLabel34
            // 
            this.xrLabel34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel34.LocationFloat = new DevExpress.Utils.PointFloat(0F, 1.88748F);
            this.xrLabel34.Name = "xrLabel34";
            this.xrLabel34.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel34.SizeF = new System.Drawing.SizeF(160.4167F, 20.17405F);
            this.xrLabel34.StylePriority.UseFont = false;
            this.xrLabel34.StylePriority.UseTextAlignment = false;
            this.xrLabel34.Text = "Subtotal No Sujeto de Iva %";
            this.xrLabel34.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell59
            // 
            this.xrTableCell59.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell59.Multiline = true;
            this.xrTableCell59.Name = "xrTableCell59";
            this.xrTableCell59.StylePriority.UseFont = false;
            this.xrTableCell59.StylePriority.UseTextAlignment = false;
            this.xrTableCell59.Text = "$0.00\r\n";
            this.xrTableCell59.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell59.Weight = 1D;
            // 
            // xrTableRow27
            // 
            this.xrTableRow27.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell108,
            this.xrTableCell64});
            this.xrTableRow27.Name = "xrTableRow27";
            this.xrTableRow27.Weight = 1D;
            // 
            // xrTableCell108
            // 
            this.xrTableCell108.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel35});
            this.xrTableCell108.Name = "xrTableCell108";
            this.xrTableCell108.Text = "xrTableCell108";
            this.xrTableCell108.Weight = 1D;
            // 
            // xrLabel35
            // 
            this.xrLabel35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel35.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel35.Name = "xrLabel35";
            this.xrLabel35.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel35.SizeF = new System.Drawing.SizeF(160.4167F, 22.06153F);
            this.xrLabel35.StylePriority.UseFont = false;
            this.xrLabel35.StylePriority.UseTextAlignment = false;
            this.xrLabel35.Text = "SUBTOTAL SIN IMPUESTOS";
            this.xrLabel35.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell64
            // 
            this.xrTableCell64.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[subtotalNoTaxes]")});
            this.xrTableCell64.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell64.Name = "xrTableCell64";
            this.xrTableCell64.StylePriority.UseFont = false;
            this.xrTableCell64.StylePriority.UseTextAlignment = false;
            this.xrTableCell64.Text = "xrTableCell64";
            this.xrTableCell64.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell64.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell64.Weight = 1D;
            // 
            // xrTableRow28
            // 
            this.xrTableRow28.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell66,
            this.xrTableCell67});
            this.xrTableRow28.Name = "xrTableRow28";
            this.xrTableRow28.Weight = 1D;
            // 
            // xrTableCell66
            // 
            this.xrTableCell66.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel36});
            this.xrTableCell66.Name = "xrTableCell66";
            this.xrTableCell66.Text = "xrTableCell66";
            this.xrTableCell66.Weight = 1D;
            // 
            // xrLabel36
            // 
            this.xrLabel36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel36.LocationFloat = new DevExpress.Utils.PointFloat(0F, 3.178914E-05F);
            this.xrLabel36.Name = "xrLabel36";
            this.xrLabel36.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel36.SizeF = new System.Drawing.SizeF(160.4167F, 20.49908F);
            this.xrLabel36.StylePriority.UseFont = false;
            this.xrLabel36.StylePriority.UseTextAlignment = false;
            this.xrLabel36.Text = "Subtotal Exento de IVA:";
            this.xrLabel36.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell67
            // 
            this.xrTableCell67.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel66});
            this.xrTableCell67.Name = "xrTableCell67";
            this.xrTableCell67.Text = "xrTableCell67";
            this.xrTableCell67.Weight = 1D;
            // 
            // xrLabel66
            // 
            this.xrLabel66.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel66.LocationFloat = new DevExpress.Utils.PointFloat(9.37265F, 1.942635F);
            this.xrLabel66.Name = "xrLabel66";
            this.xrLabel66.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel66.SizeF = new System.Drawing.SizeF(165.5745F, 20.11898F);
            this.xrLabel66.StylePriority.UseFont = false;
            this.xrLabel66.StylePriority.UseTextAlignment = false;
            this.xrLabel66.Text = "$0,00";
            this.xrLabel66.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel66.TextFormatString = "{0:$0.00}";
            // 
            // xrTableRow29
            // 
            this.xrTableRow29.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell68,
            this.xrTableCell69});
            this.xrTableRow29.Name = "xrTableRow29";
            this.xrTableRow29.Weight = 1D;
            // 
            // xrTableCell68
            // 
            this.xrTableCell68.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel37});
            this.xrTableCell68.Name = "xrTableCell68";
            this.xrTableCell68.Text = "xrTableCell68";
            this.xrTableCell68.Weight = 1D;
            // 
            // xrLabel37
            // 
            this.xrLabel37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel37.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel37.Name = "xrLabel37";
            this.xrLabel37.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel37.SizeF = new System.Drawing.SizeF(160.4167F, 23.45229F);
            this.xrLabel37.StylePriority.UseFont = false;
            this.xrLabel37.StylePriority.UseTextAlignment = false;
            this.xrLabel37.Text = "Descuento:";
            this.xrLabel37.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell69
            // 
            this.xrTableCell69.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel67});
            this.xrTableCell69.Name = "xrTableCell69";
            this.xrTableCell69.Text = "xrTableCell69";
            this.xrTableCell69.Weight = 1D;
            // 
            // xrLabel67
            // 
            this.xrLabel67.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[totalDiscount]")});
            this.xrLabel67.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel67.LocationFloat = new DevExpress.Utils.PointFloat(15.29774F, 0F);
            this.xrLabel67.Name = "xrLabel67";
            this.xrLabel67.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel67.SizeF = new System.Drawing.SizeF(159.6494F, 23.4523F);
            this.xrLabel67.StylePriority.UseFont = false;
            this.xrLabel67.StylePriority.UseTextAlignment = false;
            this.xrLabel67.Text = "$0,00";
            this.xrLabel67.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel67.TextFormatString = "{0:$###,###,##0.00}";
            // 
            // xrTableRow30
            // 
            this.xrTableRow30.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell70,
            this.xrTableCell71});
            this.xrTableRow30.Name = "xrTableRow30";
            this.xrTableRow30.Weight = 1D;
            // 
            // xrTableCell70
            // 
            this.xrTableCell70.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel38});
            this.xrTableCell70.Name = "xrTableCell70";
            this.xrTableCell70.Text = "xrTableCell70";
            this.xrTableCell70.Weight = 1D;
            // 
            // xrLabel38
            // 
            this.xrLabel38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel38.LocationFloat = new DevExpress.Utils.PointFloat(0F, 1.785649F);
            this.xrLabel38.Name = "xrLabel38";
            this.xrLabel38.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel38.SizeF = new System.Drawing.SizeF(160.4167F, 20.27591F);
            this.xrLabel38.StylePriority.UseFont = false;
            this.xrLabel38.StylePriority.UseTextAlignment = false;
            this.xrLabel38.Text = "ICE:";
            this.xrLabel38.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell71
            // 
            this.xrTableCell71.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel68});
            this.xrTableCell71.Name = "xrTableCell71";
            this.xrTableCell71.Text = "xrTableCell71";
            this.xrTableCell71.Weight = 1D;
            // 
            // xrLabel68
            // 
            this.xrLabel68.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel68.LocationFloat = new DevExpress.Utils.PointFloat(15.29774F, 1.785633F);
            this.xrLabel68.Name = "xrLabel68";
            this.xrLabel68.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel68.SizeF = new System.Drawing.SizeF(159.6494F, 20.27591F);
            this.xrLabel68.StylePriority.UseFont = false;
            this.xrLabel68.StylePriority.UseTextAlignment = false;
            this.xrLabel68.Text = "$0,00";
            this.xrLabel68.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel68.TextFormatString = "{0:$0.00}";
            // 
            // xrTableRow31
            // 
            this.xrTableRow31.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell72,
            this.xrTableCell73});
            this.xrTableRow31.Name = "xrTableRow31";
            this.xrTableRow31.Weight = 1D;
            // 
            // xrTableCell72
            // 
            this.xrTableCell72.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel39});
            this.xrTableCell72.Name = "xrTableCell72";
            this.xrTableCell72.Text = "xrTableCell72";
            this.xrTableCell72.Weight = 1D;
            // 
            // xrLabel39
            // 
            this.xrLabel39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel39.LocationFloat = new DevExpress.Utils.PointFloat(0F, 1.960013F);
            this.xrLabel39.Name = "xrLabel39";
            this.xrLabel39.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel39.SizeF = new System.Drawing.SizeF(160.4167F, 16.94253F);
            this.xrLabel39.StylePriority.UseFont = false;
            this.xrLabel39.StylePriority.UseTextAlignment = false;
            this.xrLabel39.Text = "IVA 12%:";
            this.xrLabel39.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell73
            // 
            this.xrTableCell73.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel69});
            this.xrTableCell73.Font = new System.Drawing.Font("Calibri", 13.8F);
            this.xrTableCell73.Name = "xrTableCell73";
            this.xrTableCell73.StylePriority.UseFont = false;
            this.xrTableCell73.Text = "xrTableCell73";
            this.xrTableCell73.Weight = 1D;
            // 
            // xrLabel69
            // 
            this.xrLabel69.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel69.LocationFloat = new DevExpress.Utils.PointFloat(15.29774F, 1.960041F);
            this.xrLabel69.Name = "xrLabel69";
            this.xrLabel69.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel69.SizeF = new System.Drawing.SizeF(159.6494F, 16.94253F);
            this.xrLabel69.StylePriority.UseFont = false;
            this.xrLabel69.StylePriority.UseTextAlignment = false;
            this.xrLabel69.Text = "$0,00";
            this.xrLabel69.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel69.TextFormatString = "{0:$0.00}";
            // 
            // xrTableRow32
            // 
            this.xrTableRow32.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell74,
            this.xrTableCell75});
            this.xrTableRow32.Name = "xrTableRow32";
            this.xrTableRow32.Weight = 1D;
            // 
            // xrTableCell74
            // 
            this.xrTableCell74.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel40});
            this.xrTableCell74.Name = "xrTableCell74";
            this.xrTableCell74.Text = "xrTableCell74";
            this.xrTableCell74.Weight = 1D;
            // 
            // xrLabel40
            // 
            this.xrLabel40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel40.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel40.Name = "xrLabel40";
            this.xrLabel40.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel40.SizeF = new System.Drawing.SizeF(160.4167F, 22.06154F);
            this.xrLabel40.StylePriority.UseFont = false;
            this.xrLabel40.StylePriority.UseTextAlignment = false;
            this.xrLabel40.Text = "IRBPNR:";
            this.xrLabel40.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell75
            // 
            this.xrTableCell75.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel70});
            this.xrTableCell75.Name = "xrTableCell75";
            this.xrTableCell75.Text = "xrTableCell75";
            this.xrTableCell75.Weight = 1D;
            // 
            // xrLabel70
            // 
            this.xrLabel70.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel70.LocationFloat = new DevExpress.Utils.PointFloat(15.29774F, 3.814697E-06F);
            this.xrLabel70.Name = "xrLabel70";
            this.xrLabel70.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel70.SizeF = new System.Drawing.SizeF(159.6494F, 22.06154F);
            this.xrLabel70.StylePriority.UseFont = false;
            this.xrLabel70.StylePriority.UseTextAlignment = false;
            this.xrLabel70.Text = "$0,00";
            this.xrLabel70.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel70.TextFormatString = "{0:$0.00}";
            // 
            // xrTableRow34
            // 
            this.xrTableRow34.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell78,
            this.xrTableCell79});
            this.xrTableRow34.Name = "xrTableRow34";
            this.xrTableRow34.Weight = 1D;
            // 
            // xrTableCell78
            // 
            this.xrTableCell78.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel41});
            this.xrTableCell78.Name = "xrTableCell78";
            this.xrTableCell78.Text = "xrTableCell78";
            this.xrTableCell78.Weight = 1D;
            // 
            // xrLabel41
            // 
            this.xrLabel41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel41.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel41.Name = "xrLabel41";
            this.xrLabel41.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel41.SizeF = new System.Drawing.SizeF(160.4167F, 22.06155F);
            this.xrLabel41.StylePriority.UseFont = false;
            this.xrLabel41.StylePriority.UseTextAlignment = false;
            this.xrLabel41.Text = "Propina:";
            this.xrLabel41.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell79
            // 
            this.xrTableCell79.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel71});
            this.xrTableCell79.Name = "xrTableCell79";
            this.xrTableCell79.Text = "xrTableCell79";
            this.xrTableCell79.Weight = 1D;
            // 
            // xrLabel71
            // 
            this.xrLabel71.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel71.LocationFloat = new DevExpress.Utils.PointFloat(15.29774F, 3.814697E-06F);
            this.xrLabel71.Name = "xrLabel71";
            this.xrLabel71.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel71.SizeF = new System.Drawing.SizeF(159.6494F, 22.06154F);
            this.xrLabel71.StylePriority.UseFont = false;
            this.xrLabel71.StylePriority.UseTextAlignment = false;
            this.xrLabel71.Text = "$0,00";
            this.xrLabel71.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel71.TextFormatString = "{0:$0.00}";
            // 
            // xrTableRow33
            // 
            this.xrTableRow33.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell109,
            this.xrTableCell76});
            this.xrTableRow33.Name = "xrTableRow33";
            this.xrTableRow33.Weight = 1D;
            // 
            // xrTableCell109
            // 
            this.xrTableCell109.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel42});
            this.xrTableCell109.Name = "xrTableCell109";
            this.xrTableCell109.Text = "xrTableCell109";
            this.xrTableCell109.Weight = 1D;
            // 
            // xrLabel42
            // 
            this.xrLabel42.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel42.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel42.Name = "xrLabel42";
            this.xrLabel42.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel42.SizeF = new System.Drawing.SizeF(160.4167F, 18.04767F);
            this.xrLabel42.StylePriority.UseFont = false;
            this.xrLabel42.StylePriority.UseTextAlignment = false;
            this.xrLabel42.Text = "VALOR TOTAL FOB:";
            this.xrLabel42.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell76
            // 
            this.xrTableCell76.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[valueTotalFOB]")});
            this.xrTableCell76.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.xrTableCell76.Name = "xrTableCell76";
            this.xrTableCell76.StylePriority.UseFont = false;
            this.xrTableCell76.StylePriority.UseTextAlignment = false;
            this.xrTableCell76.Text = "xrTableCell76";
            this.xrTableCell76.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell76.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell76.Weight = 1D;
            // 
            // xrTableRow36
            // 
            this.xrTableRow36.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell82,
            this.xrTableCell83});
            this.xrTableRow36.Name = "xrTableRow36";
            this.xrTableRow36.Weight = 1D;
            // 
            // xrTableCell82
            // 
            this.xrTableCell82.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel43});
            this.xrTableCell82.Name = "xrTableCell82";
            this.xrTableCell82.Text = "xrTableCell82";
            this.xrTableCell82.Weight = 1D;
            // 
            // xrLabel43
            // 
            this.xrLabel43.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel43.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel43.Name = "xrLabel43";
            this.xrLabel43.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel43.SizeF = new System.Drawing.SizeF(160.4167F, 22.06154F);
            this.xrLabel43.StylePriority.UseFont = false;
            this.xrLabel43.StylePriority.UseTextAlignment = false;
            this.xrLabel43.Text = "Flete Internacional:";
            this.xrLabel43.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell83
            // 
            this.xrTableCell83.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[valueInternationalFreight]")});
            this.xrTableCell83.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell83.Name = "xrTableCell83";
            this.xrTableCell83.StylePriority.UseFont = false;
            this.xrTableCell83.StylePriority.UseTextAlignment = false;
            this.xrTableCell83.Text = "xrTableCell83";
            this.xrTableCell83.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell83.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell83.Weight = 1D;
            // 
            // xrTableRow35
            // 
            this.xrTableRow35.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell80,
            this.xrTableCell81});
            this.xrTableRow35.Name = "xrTableRow35";
            this.xrTableRow35.Weight = 1D;
            // 
            // xrTableCell80
            // 
            this.xrTableCell80.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel44});
            this.xrTableCell80.Name = "xrTableCell80";
            this.xrTableCell80.Text = "xrTableCell80";
            this.xrTableCell80.Weight = 1D;
            // 
            // xrLabel44
            // 
            this.xrLabel44.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel44.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel44.Name = "xrLabel44";
            this.xrLabel44.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel44.SizeF = new System.Drawing.SizeF(160.4167F, 21.7857F);
            this.xrLabel44.StylePriority.UseFont = false;
            this.xrLabel44.StylePriority.UseTextAlignment = false;
            this.xrLabel44.Text = "Seguro Internacional:";
            this.xrLabel44.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell81
            // 
            this.xrTableCell81.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[valueInternationalInsurance]")});
            this.xrTableCell81.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell81.Name = "xrTableCell81";
            this.xrTableCell81.StylePriority.UseFont = false;
            this.xrTableCell81.StylePriority.UseTextAlignment = false;
            this.xrTableCell81.Text = "xrTableCell81";
            this.xrTableCell81.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell81.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell81.Weight = 1D;
            // 
            // xrTableRow37
            // 
            this.xrTableRow37.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell84,
            this.xrTableCell85});
            this.xrTableRow37.Name = "xrTableRow37";
            this.xrTableRow37.Weight = 1D;
            // 
            // xrTableCell84
            // 
            this.xrTableCell84.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel45});
            this.xrTableCell84.Name = "xrTableCell84";
            this.xrTableCell84.Text = "xrTableCell84";
            this.xrTableCell84.Weight = 1D;
            // 
            // xrLabel45
            // 
            this.xrLabel45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel45.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0.1190186F);
            this.xrLabel45.Name = "xrLabel45";
            this.xrLabel45.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel45.SizeF = new System.Drawing.SizeF(160.4167F, 21.94246F);
            this.xrLabel45.StylePriority.UseFont = false;
            this.xrLabel45.StylePriority.UseTextAlignment = false;
            this.xrLabel45.Text = "Gastos Aduaneros:";
            this.xrLabel45.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell85
            // 
            this.xrTableCell85.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[valueCustomsExpenditures]")});
            this.xrTableCell85.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell85.Name = "xrTableCell85";
            this.xrTableCell85.StylePriority.UseFont = false;
            this.xrTableCell85.StylePriority.UseTextAlignment = false;
            this.xrTableCell85.Text = "xrTableCell85";
            this.xrTableCell85.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell85.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell85.Weight = 1D;
            // 
            // xrTableRow39
            // 
            this.xrTableRow39.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell88,
            this.xrTableCell89});
            this.xrTableRow39.Name = "xrTableRow39";
            this.xrTableRow39.Weight = 1D;
            // 
            // xrTableCell88
            // 
            this.xrTableCell88.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel46});
            this.xrTableCell88.Name = "xrTableCell88";
            this.xrTableCell88.Text = "xrTableCell88";
            this.xrTableCell88.Weight = 1D;
            // 
            // xrLabel46
            // 
            this.xrLabel46.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrLabel46.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel46.Name = "xrLabel46";
            this.xrLabel46.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel46.SizeF = new System.Drawing.SizeF(160.4167F, 22.06154F);
            this.xrLabel46.StylePriority.UseFont = false;
            this.xrLabel46.StylePriority.UseTextAlignment = false;
            this.xrLabel46.Text = "Gastos Transporte/Otros:";
            this.xrLabel46.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell89
            // 
            this.xrTableCell89.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[valueTransportationExpenses]")});
            this.xrTableCell89.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.xrTableCell89.Name = "xrTableCell89";
            this.xrTableCell89.StylePriority.UseFont = false;
            this.xrTableCell89.StylePriority.UseTextAlignment = false;
            this.xrTableCell89.Text = "xrTableCell89";
            this.xrTableCell89.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell89.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell89.Weight = 1D;
            // 
            // xrTableRow38
            // 
            this.xrTableRow38.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell86,
            this.xrTableCell87});
            this.xrTableRow38.Name = "xrTableRow38";
            this.xrTableRow38.Weight = 1D;
            // 
            // xrTableCell86
            // 
            this.xrTableCell86.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel83});
            this.xrTableCell86.Name = "xrTableCell86";
            this.xrTableCell86.Text = "xrTableCell86";
            this.xrTableCell86.Weight = 1D;
            // 
            // xrLabel83
            // 
            this.xrLabel83.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalTerm]")});
            this.xrLabel83.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel83.LocationFloat = new DevExpress.Utils.PointFloat(85.30988F, 0.7914224F);
            this.xrLabel83.Name = "xrLabel83";
            this.xrLabel83.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel83.SizeF = new System.Drawing.SizeF(75.1068F, 19.27016F);
            this.xrLabel83.StylePriority.UseFont = false;
            this.xrLabel83.StylePriority.UseTextAlignment = false;
            this.xrLabel83.Text = "xrLabel83";
            this.xrLabel83.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrTableCell87
            // 
            this.xrTableCell87.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalFinal]")});
            this.xrTableCell87.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.xrTableCell87.Name = "xrTableCell87";
            this.xrTableCell87.StylePriority.UseFont = false;
            this.xrTableCell87.StylePriority.UseTextAlignment = false;
            this.xrTableCell87.Text = "xrTableCell87";
            this.xrTableCell87.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell87.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell87.Weight = 1D;
            // 
            // xrPanel6
            // 
            this.xrPanel6.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrPanel6.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable6});
            this.xrPanel6.LocationFloat = new DevExpress.Utils.PointFloat(9.100422F, 387.4675F);
            this.xrPanel6.Name = "xrPanel6";
            this.xrPanel6.SizeF = new System.Drawing.SizeF(424.3914F, 42.53247F);
            this.xrPanel6.StylePriority.UseBorders = false;
            // 
            // xrTable6
            // 
            this.xrTable6.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTable6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 2.000031F);
            this.xrTable6.Name = "xrTable6";
            this.xrTable6.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow40,
            this.xrTableRow41});
            this.xrTable6.SizeF = new System.Drawing.SizeF(421.8253F, 40.53242F);
            this.xrTable6.StylePriority.UseFont = false;
            // 
            // xrTableRow40
            // 
            this.xrTableRow40.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell90,
            this.xrTableCell91,
            this.xrTableCell93,
            this.xrTableCell92});
            this.xrTableRow40.Name = "xrTableRow40";
            this.xrTableRow40.Weight = 1D;
            // 
            // xrTableCell90
            // 
            this.xrTableCell90.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell90.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel49});
            this.xrTableCell90.Name = "xrTableCell90";
            this.xrTableCell90.StylePriority.UseBorders = false;
            this.xrTableCell90.Text = "xrTableCell90";
            this.xrTableCell90.Weight = 1.8703050571268352D;
            // 
            // xrLabel49
            // 
            this.xrLabel49.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel49.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel49.LocationFloat = new DevExpress.Utils.PointFloat(1.852481F, 2.000061F);
            this.xrLabel49.Name = "xrLabel49";
            this.xrLabel49.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel49.SizeF = new System.Drawing.SizeF(127.1304F, 15.84866F);
            this.xrLabel49.StylePriority.UseBorders = false;
            this.xrLabel49.StylePriority.UseFont = false;
            this.xrLabel49.StylePriority.UseTextAlignment = false;
            this.xrLabel49.Text = "Forma de Pago";
            this.xrLabel49.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrTableCell91
            // 
            this.xrTableCell91.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell91.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel50});
            this.xrTableCell91.Name = "xrTableCell91";
            this.xrTableCell91.StylePriority.UseBorders = false;
            this.xrTableCell91.Text = "xrTableCell91";
            this.xrTableCell91.Weight = 1.4290353394389115D;
            // 
            // xrLabel50
            // 
            this.xrLabel50.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel50.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel50.LocationFloat = new DevExpress.Utils.PointFloat(1.00001F, 2.000046F);
            this.xrLabel50.Name = "xrLabel50";
            this.xrLabel50.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel50.SizeF = new System.Drawing.SizeF(96.77431F, 15.84869F);
            this.xrLabel50.StylePriority.UseBorders = false;
            this.xrLabel50.StylePriority.UseFont = false;
            this.xrLabel50.StylePriority.UseTextAlignment = false;
            this.xrLabel50.Text = "Valor";
            this.xrLabel50.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrTableCell93
            // 
            this.xrTableCell93.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell93.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel51});
            this.xrTableCell93.Name = "xrTableCell93";
            this.xrTableCell93.StylePriority.UseBorders = false;
            this.xrTableCell93.Text = "xrTableCell93";
            this.xrTableCell93.Weight = 1.6925261775072797D;
            // 
            // xrLabel51
            // 
            this.xrLabel51.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel51.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel51.LocationFloat = new DevExpress.Utils.PointFloat(2.0001F, 2.000061F);
            this.xrLabel51.Name = "xrLabel51";
            this.xrLabel51.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel51.SizeF = new System.Drawing.SizeF(114.859F, 15.84866F);
            this.xrLabel51.StylePriority.UseBorders = false;
            this.xrLabel51.StylePriority.UseFont = false;
            this.xrLabel51.StylePriority.UseTextAlignment = false;
            this.xrLabel51.Text = "Plazo";
            this.xrLabel51.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrTableCell92
            // 
            this.xrTableCell92.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell92.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel52});
            this.xrTableCell92.Name = "xrTableCell92";
            this.xrTableCell92.StylePriority.UseBorders = false;
            this.xrTableCell92.Text = "xrTableCell92";
            this.xrTableCell92.Weight = 1.0378895500131335D;
            // 
            // xrLabel52
            // 
            this.xrLabel52.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel52.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel52.LocationFloat = new DevExpress.Utils.PointFloat(0.8161602F, 2.000061F);
            this.xrLabel52.Name = "xrLabel52";
            this.xrLabel52.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel52.SizeF = new System.Drawing.SizeF(69.02462F, 15.84866F);
            this.xrLabel52.StylePriority.UseBorders = false;
            this.xrLabel52.StylePriority.UseFont = false;
            this.xrLabel52.StylePriority.UseTextAlignment = false;
            this.xrLabel52.Text = "Tiempo";
            this.xrLabel52.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrTableRow41
            // 
            this.xrTableRow41.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell98,
            this.xrTableCell95,
            this.xrTableCell102,
            this.xrTableCell97});
            this.xrTableRow41.Name = "xrTableRow41";
            this.xrTableRow41.Weight = 1D;
            // 
            // xrTableCell98
            // 
            this.xrTableCell98.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell98.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[formapago]")});
            this.xrTableCell98.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.xrTableCell98.Name = "xrTableCell98";
            this.xrTableCell98.StylePriority.UseBorders = false;
            this.xrTableCell98.StylePriority.UseFont = false;
            this.xrTableCell98.StylePriority.UseTextAlignment = false;
            this.xrTableCell98.Text = "xrTableCell98";
            this.xrTableCell98.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell98.Weight = 1.4791669459907633D;
            // 
            // xrTableCell95
            // 
            this.xrTableCell95.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell95.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalFinal]")});
            this.xrTableCell95.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.xrTableCell95.Name = "xrTableCell95";
            this.xrTableCell95.StylePriority.UseBorders = false;
            this.xrTableCell95.StylePriority.UseFont = false;
            this.xrTableCell95.StylePriority.UseTextAlignment = false;
            this.xrTableCell95.Text = "xrTableCell95";
            this.xrTableCell95.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell95.TextFormatString = "{0:$###,###,##0.00}";
            this.xrTableCell95.Weight = 1.130180740373794D;
            // 
            // xrTableCell102
            // 
            this.xrTableCell102.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell102.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[plazo]")});
            this.xrTableCell102.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.xrTableCell102.Name = "xrTableCell102";
            this.xrTableCell102.StylePriority.UseBorders = false;
            this.xrTableCell102.StylePriority.UseFont = false;
            this.xrTableCell102.StylePriority.UseTextAlignment = false;
            this.xrTableCell102.Text = "xrTableCell102";
            this.xrTableCell102.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell102.Weight = 1.338567441628387D;
            // 
            // xrTableCell97
            // 
            this.xrTableCell97.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell97.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[tiempo]")});
            this.xrTableCell97.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.xrTableCell97.Name = "xrTableCell97";
            this.xrTableCell97.StylePriority.UseBorders = false;
            this.xrTableCell97.StylePriority.UseFont = false;
            this.xrTableCell97.StylePriority.UseTextAlignment = false;
            this.xrTableCell97.Text = "xrTableCell97";
            this.xrTableCell97.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell97.Weight = 0.820834405718171D;
            // 
            // xrPanel4
            // 
            this.xrPanel4.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPanel4.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable4,
            this.xrLabel19});
            this.xrPanel4.LocationFloat = new DevExpress.Utils.PointFloat(9.100422F, 0F);
            this.xrPanel4.Name = "xrPanel4";
            this.xrPanel4.SizeF = new System.Drawing.SizeF(421.8253F, 387.4675F);
            this.xrPanel4.StylePriority.UseBorders = false;
            // 
            // xrTable4
            // 
            this.xrTable4.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTable4.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 29.28563F);
            this.xrTable4.Name = "xrTable4";
            this.xrTable4.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow10,
            this.xrTableRow11,
            this.xrTableRow12,
            this.xrTableRow13,
            this.xrTableRow14,
            this.xrTableRow15,
            this.xrTableRow16,
            this.xrTableRow17,
            this.xrTableRow18,
            this.xrTableRow42,
            this.xrTableRow19,
            this.xrTableRow20,
            this.xrTableRow21,
            this.xrTableRow44,
            this.xrTableRow22,
            this.xrTableRow23,
            this.xrTableRow48});
            this.xrTable4.SizeF = new System.Drawing.SizeF(395.1863F, 314.3615F);
            this.xrTable4.StylePriority.UseBorders = false;
            // 
            // xrTableRow10
            // 
            this.xrTableRow10.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell28,
            this.xrTableCell30});
            this.xrTableRow10.Name = "xrTableRow10";
            this.xrTableRow10.Weight = 0.68091562234169156D;
            // 
            // xrTableCell28
            // 
            this.xrTableCell28.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell28.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel48});
            this.xrTableCell28.Name = "xrTableCell28";
            this.xrTableCell28.StylePriority.UseBorders = false;
            this.xrTableCell28.Text = "xrTableCell28";
            this.xrTableCell28.Weight = 1.4991599516786376D;
            // 
            // xrLabel48
            // 
            this.xrLabel48.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel48.LocationFloat = new DevExpress.Utils.PointFloat(9.999974F, 0F);
            this.xrLabel48.Name = "xrLabel48";
            this.xrLabel48.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel48.SizeF = new System.Drawing.SizeF(139.198F, 17.46241F);
            this.xrLabel48.StylePriority.UseFont = false;
            this.xrLabel48.Text = "Puerto de Embarque";
            // 
            // xrTableCell30
            // 
            this.xrTableCell30.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell30.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ciudadembarque]")});
            this.xrTableCell30.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell30.Name = "xrTableCell30";
            this.xrTableCell30.StylePriority.UseBorders = false;
            this.xrTableCell30.StylePriority.UseFont = false;
            this.xrTableCell30.Text = "xrTableCell30";
            this.xrTableCell30.Weight = 2.2925050120887693D;
            // 
            // xrTableRow11
            // 
            this.xrTableRow11.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell31,
            this.xrTableCell32});
            this.xrTableRow11.Name = "xrTableRow11";
            this.xrTableRow11.Weight = 0.70747251040896075D;
            // 
            // xrTableCell31
            // 
            this.xrTableCell31.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel20});
            this.xrTableCell31.Name = "xrTableCell31";
            this.xrTableCell31.Text = "xrTableCell31";
            this.xrTableCell31.Weight = 1.4991599516786376D;
            // 
            // xrLabel20
            // 
            this.xrLabel20.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 1.19945F);
            this.xrLabel20.Name = "xrLabel20";
            this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel20.SizeF = new System.Drawing.SizeF(100F, 15.46241F);
            this.xrLabel20.StylePriority.UseFont = false;
            this.xrLabel20.Text = "Puerto Destino";
            // 
            // xrTableCell32
            // 
            this.xrTableCell32.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ciudaddestino]")});
            this.xrTableCell32.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell32.Name = "xrTableCell32";
            this.xrTableCell32.StylePriority.UseFont = false;
            this.xrTableCell32.Text = "xrTableCell32";
            this.xrTableCell32.Weight = 2.2925050120887693D;
            // 
            // xrTableRow12
            // 
            this.xrTableRow12.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell33,
            this.xrTableCell34});
            this.xrTableRow12.Name = "xrTableRow12";
            this.xrTableRow12.Weight = 0.83024096107501144D;
            // 
            // xrTableCell33
            // 
            this.xrTableCell33.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel21});
            this.xrTableCell33.Name = "xrTableCell33";
            this.xrTableCell33.Text = "xrTableCell33";
            this.xrTableCell33.Weight = 1.4991599516786376D;
            // 
            // xrLabel21
            // 
            this.xrLabel21.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.Text = "Dirección";
            // 
            // xrTableCell34
            // 
            this.xrTableCell34.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[direccion]")});
            this.xrTableCell34.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell34.Name = "xrTableCell34";
            this.xrTableCell34.StylePriority.UseFont = false;
            this.xrTableCell34.Text = "xrTableCell34";
            this.xrTableCell34.Weight = 2.2925050120887693D;
            // 
            // xrTableRow13
            // 
            this.xrTableRow13.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell35,
            this.xrTableCell36});
            this.xrTableRow13.Name = "xrTableRow13";
            this.xrTableRow13.Weight = 0.6809155837321017D;
            // 
            // xrTableCell35
            // 
            this.xrTableCell35.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel22});
            this.xrTableCell35.Name = "xrTableCell35";
            this.xrTableCell35.Text = "xrTableCell35";
            this.xrTableCell35.Weight = 1.4991599516786376D;
            // 
            // xrLabel22
            // 
            this.xrLabel22.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 1.423853F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(100F, 14.692F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.Text = "Email";
            // 
            // xrTableCell36
            // 
            this.xrTableCell36.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[emailInterno]")});
            this.xrTableCell36.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell36.Name = "xrTableCell36";
            this.xrTableCell36.StylePriority.UseFont = false;
            this.xrTableCell36.Text = "xrTableCell36";
            this.xrTableCell36.Weight = 2.2925050120887693D;
            // 
            // xrTableRow14
            // 
            this.xrTableRow14.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell37,
            this.xrTableCell38});
            this.xrTableRow14.Name = "xrTableRow14";
            this.xrTableRow14.Weight = 0.90946293680698764D;
            // 
            // xrTableCell37
            // 
            this.xrTableCell37.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel23});
            this.xrTableCell37.Name = "xrTableCell37";
            this.xrTableCell37.Text = "xrTableCell37";
            this.xrTableCell37.Weight = 1.4991599516786376D;
            // 
            // xrLabel23
            // 
            this.xrLabel23.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 3.671828F);
            this.xrLabel23.Name = "xrLabel23";
            this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel23.SizeF = new System.Drawing.SizeF(100F, 11.18892F);
            this.xrLabel23.StylePriority.UseFont = false;
            this.xrLabel23.Text = "Puerto Descarga";
            // 
            // xrTableCell38
            // 
            this.xrTableCell38.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ciudaddescarga]")});
            this.xrTableCell38.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell38.Name = "xrTableCell38";
            this.xrTableCell38.StylePriority.UseFont = false;
            this.xrTableCell38.Text = "xrTableCell38";
            this.xrTableCell38.Weight = 2.2925050120887693D;
            // 
            // xrTableRow15
            // 
            this.xrTableRow15.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell39,
            this.xrTableCell40});
            this.xrTableRow15.Name = "xrTableRow15";
            this.xrTableRow15.Weight = 0.85995755046731581D;
            // 
            // xrTableCell39
            // 
            this.xrTableCell39.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel24});
            this.xrTableCell39.Name = "xrTableCell39";
            this.xrTableCell39.Text = "xrTableCell39";
            this.xrTableCell39.Weight = 1.4991599516786376D;
            // 
            // xrLabel24
            // 
            this.xrLabel24.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel24.Name = "xrLabel24";
            this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel24.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel24.StylePriority.UseFont = false;
            this.xrLabel24.Text = "Fecha Embarque";
            // 
            // xrTableCell40
            // 
            this.xrTableCell40.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[dateShipment]")});
            this.xrTableCell40.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell40.Name = "xrTableCell40";
            this.xrTableCell40.StylePriority.UseFont = false;
            this.xrTableCell40.Text = "xrTableCell40";
            this.xrTableCell40.TextFormatString = "{0:dd/MM/yyyy}";
            this.xrTableCell40.Weight = 2.2925050120887693D;
            // 
            // xrTableRow16
            // 
            this.xrTableRow16.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell41,
            this.xrTableCell42});
            this.xrTableRow16.Name = "xrTableRow16";
            this.xrTableRow16.Weight = 0.74012655310198938D;
            // 
            // xrTableCell41
            // 
            this.xrTableCell41.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel25});
            this.xrTableCell41.Name = "xrTableCell41";
            this.xrTableCell41.Text = "xrTableCell41";
            this.xrTableCell41.Weight = 1.4991599516786376D;
            // 
            // xrLabel25
            // 
            this.xrLabel25.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel25.StylePriority.UseFont = false;
            this.xrLabel25.Text = "Naviera";
            // 
            // xrTableCell42
            // 
            this.xrTableCell42.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ShippingAgency_name]")});
            this.xrTableCell42.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell42.Name = "xrTableCell42";
            this.xrTableCell42.StylePriority.UseFont = false;
            this.xrTableCell42.Text = "xrTableCell42";
            this.xrTableCell42.Weight = 2.2925050120887693D;
            // 
            // xrTableRow17
            // 
            this.xrTableRow17.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell111,
            this.xrTableCell44});
            this.xrTableRow17.Name = "xrTableRow17";
            this.xrTableRow17.Weight = 0.86318351167434593D;
            // 
            // xrTableCell111
            // 
            this.xrTableCell111.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel26});
            this.xrTableCell111.Name = "xrTableCell111";
            this.xrTableCell111.Text = "xrTableCell111";
            this.xrTableCell111.Weight = 1.4991599516786376D;
            // 
            // xrLabel26
            // 
            this.xrLabel26.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel26.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 2.518786F);
            this.xrLabel26.Name = "xrLabel26";
            this.xrLabel26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel26.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel26.StylePriority.UseFont = false;
            this.xrLabel26.Text = "País Destino";
            // 
            // xrTableCell44
            // 
            this.xrTableCell44.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[name_1]")});
            this.xrTableCell44.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell44.Name = "xrTableCell44";
            this.xrTableCell44.StylePriority.UseFont = false;
            this.xrTableCell44.Text = "xrTableCell44";
            this.xrTableCell44.Weight = 2.2925050120887693D;
            // 
            // xrTableRow18
            // 
            this.xrTableRow18.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell43,
            this.xrTableCell47});
            this.xrTableRow18.Name = "xrTableRow18";
            this.xrTableRow18.Weight = 0.62759379434452867D;
            // 
            // xrTableCell43
            // 
            this.xrTableCell43.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel27});
            this.xrTableCell43.Name = "xrTableCell43";
            this.xrTableCell43.Text = "xrTableCell43";
            this.xrTableCell43.Weight = 1.4991599516786376D;
            // 
            // xrLabel27
            // 
            this.xrLabel27.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel27.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel27.Name = "xrLabel27";
            this.xrLabel27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel27.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel27.StylePriority.UseFont = false;
            this.xrLabel27.Text = "Buque";
            // 
            // xrTableCell47
            // 
            this.xrTableCell47.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[buque]")});
            this.xrTableCell47.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell47.Name = "xrTableCell47";
            this.xrTableCell47.StylePriority.UseFont = false;
            this.xrTableCell47.StylePriority.UseTextAlignment = false;
            this.xrTableCell47.Text = "xrTableCell47";
            this.xrTableCell47.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell47.Weight = 2.2925050120887693D;
            // 
            // xrTableRow42
            // 
            this.xrTableRow42.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell45,
            this.xrTableCell99});
            this.xrTableRow42.Name = "xrTableRow42";
            this.xrTableRow42.Weight = 0.62759379434452867D;
            // 
            // xrTableCell45
            // 
            this.xrTableCell45.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel53});
            this.xrTableCell45.Name = "xrTableCell45";
            this.xrTableCell45.Text = "xrTableCell45";
            this.xrTableCell45.Weight = 1.4991599516786376D;
            // 
            // xrLabel53
            // 
            this.xrLabel53.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel53.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel53.Name = "xrLabel53";
            this.xrLabel53.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel53.SizeF = new System.Drawing.SizeF(100F, 15.77553F);
            this.xrLabel53.StylePriority.UseFont = false;
            this.xrLabel53.Text = "BL";
            // 
            // xrTableCell99
            // 
            this.xrTableCell99.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[daeNumber]")});
            this.xrTableCell99.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell99.Name = "xrTableCell99";
            this.xrTableCell99.StylePriority.UseFont = false;
            this.xrTableCell99.Text = "xrTableCell99";
            this.xrTableCell99.Weight = 2.2925050120887693D;
            // 
            // xrTableRow19
            // 
            this.xrTableRow19.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell46,
            this.xrTableCell48});
            this.xrTableRow19.Name = "xrTableRow19";
            this.xrTableRow19.Weight = 0.70990100735227268D;
            // 
            // xrTableCell46
            // 
            this.xrTableCell46.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel28});
            this.xrTableCell46.Name = "xrTableCell46";
            this.xrTableCell46.Text = "xrTableCell46";
            this.xrTableCell46.Weight = 1.4991599516786376D;
            // 
            // xrLabel28
            // 
            this.xrLabel28.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel28.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel28.Name = "xrLabel28";
            this.xrLabel28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel28.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel28.StylePriority.UseFont = false;
            this.xrLabel28.Text = "Total CM";
            // 
            // xrTableCell48
            // 
            this.xrTableCell48.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[totalBoxes]")});
            this.xrTableCell48.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell48.Name = "xrTableCell48";
            this.xrTableCell48.StylePriority.UseFont = false;
            this.xrTableCell48.Text = "xrTableCell48";
            this.xrTableCell48.Weight = 2.2925050120887693D;
            // 
            // xrTableRow20
            // 
            this.xrTableRow20.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell49,
            this.xrTableCell52,
            this.xrTableCell50,
            this.xrTableCell55,
            this.xrTableCell51});
            this.xrTableRow20.Name = "xrTableRow20";
            this.xrTableRow20.Weight = 0.740125994428153D;
            // 
            // xrTableCell49
            // 
            this.xrTableCell49.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel29});
            this.xrTableCell49.Name = "xrTableCell49";
            this.xrTableCell49.Text = "xrTableCell49";
            this.xrTableCell49.Weight = 1.4991599516786376D;
            // 
            // xrLabel29
            // 
            this.xrLabel29.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel29.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 1.153124F);
            this.xrLabel29.Name = "xrLabel29";
            this.xrLabel29.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel29.SizeF = new System.Drawing.SizeF(100F, 16.45111F);
            this.xrLabel29.StylePriority.UseFont = false;
            this.xrLabel29.Text = "Peso Neto";
            // 
            // xrTableCell52
            // 
            this.xrTableCell52.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[pesonetolibras]")});
            this.xrTableCell52.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell52.Name = "xrTableCell52";
            this.xrTableCell52.StylePriority.UseFont = false;
            this.xrTableCell52.Text = "xrTableCell52";
            this.xrTableCell52.Weight = 0.83613859735659979D;
            // 
            // xrTableCell50
            // 
            this.xrTableCell50.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell50.Name = "xrTableCell50";
            this.xrTableCell50.StylePriority.UseFont = false;
            this.xrTableCell50.Text = "lbs";
            this.xrTableCell50.Weight = 0.508450173790318D;
            // 
            // xrTableCell55
            // 
            this.xrTableCell55.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[pesonetokilos]")});
            this.xrTableCell55.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell55.Name = "xrTableCell55";
            this.xrTableCell55.StylePriority.UseFont = false;
            this.xrTableCell55.Text = "xrTableCell55";
            this.xrTableCell55.Weight = 0.68627283435894482D;
            // 
            // xrTableCell51
            // 
            this.xrTableCell51.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell51.Name = "xrTableCell51";
            this.xrTableCell51.StylePriority.UseFont = false;
            this.xrTableCell51.Text = "kls";
            this.xrTableCell51.Weight = 0.26164340658290697D;
            // 
            // xrTableRow21
            // 
            this.xrTableRow21.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell60,
            this.xrTableCell63,
            this.xrTableCell61,
            this.xrTableCell65,
            this.xrTableCell53});
            this.xrTableRow21.Name = "xrTableRow21";
            this.xrTableRow21.Weight = 0.59792176726288448D;
            // 
            // xrTableCell60
            // 
            this.xrTableCell60.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel30});
            this.xrTableCell60.Name = "xrTableCell60";
            this.xrTableCell60.Text = "xrTableCell60";
            this.xrTableCell60.Weight = 1.4991605372881081D;
            // 
            // xrLabel30
            // 
            this.xrLabel30.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel30.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 3.178914E-05F);
            this.xrLabel30.Name = "xrLabel30";
            this.xrLabel30.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel30.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel30.StylePriority.UseFont = false;
            this.xrLabel30.Text = "Peso Bruto";
            // 
            // xrTableCell63
            // 
            this.xrTableCell63.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[pesobrutolibras]")});
            this.xrTableCell63.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell63.Name = "xrTableCell63";
            this.xrTableCell63.StylePriority.UseFont = false;
            this.xrTableCell63.Text = "xrTableCell63";
            this.xrTableCell63.Weight = 0.83613771894238842D;
            // 
            // xrTableCell61
            // 
            this.xrTableCell61.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell61.Name = "xrTableCell61";
            this.xrTableCell61.StylePriority.UseFont = false;
            this.xrTableCell61.Text = "lbs";
            this.xrTableCell61.Weight = 0.50845046659505877D;
            // 
            // xrTableCell65
            // 
            this.xrTableCell65.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[pesobrutokilos]")});
            this.xrTableCell65.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell65.Name = "xrTableCell65";
            this.xrTableCell65.StylePriority.UseFont = false;
            this.xrTableCell65.Text = "xrTableCell65";
            this.xrTableCell65.Weight = 0.68783816835271472D;
            // 
            // xrTableCell53
            // 
            this.xrTableCell53.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell53.Name = "xrTableCell53";
            this.xrTableCell53.StylePriority.UseFont = false;
            this.xrTableCell53.Text = "kls";
            this.xrTableCell53.Weight = 0.26007807258913718D;
            // 
            // xrTableRow44
            // 
            this.xrTableRow44.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell112,
            this.xrTableCell96,
            this.xrTableCell94,
            this.xrTableCell77,
            this.xrTableCell116});
            this.xrTableRow44.Name = "xrTableRow44";
            this.xrTableRow44.Weight = 0.74046043576852516D;
            // 
            // xrTableCell112
            // 
            this.xrTableCell112.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel81});
            this.xrTableCell112.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell112.Name = "xrTableCell112";
            this.xrTableCell112.StylePriority.UseFont = false;
            this.xrTableCell112.Text = "xrTableCell112";
            this.xrTableCell112.Weight = 1.4991604917961769D;
            // 
            // xrLabel81
            // 
            this.xrLabel81.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel81.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel81.Name = "xrLabel81";
            this.xrLabel81.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel81.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel81.StylePriority.UseFont = false;
            this.xrLabel81.Text = "Peso con Glaseo";
            // 
            // xrTableCell96
            // 
            this.xrTableCell96.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[pesoglaskilos]")});
            this.xrTableCell96.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell96.Name = "xrTableCell96";
            this.xrTableCell96.StylePriority.UseFont = false;
            this.xrTableCell96.Text = "xrTableCell96";
            this.xrTableCell96.Weight = 0.83613772365640726D;
            // 
            // xrTableCell94
            // 
            this.xrTableCell94.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell94.Name = "xrTableCell94";
            this.xrTableCell94.StylePriority.UseFont = false;
            this.xrTableCell94.Text = "KLS";
            this.xrTableCell94.Weight = 0.50845113123117514D;
            // 
            // xrTableCell77
            // 
            this.xrTableCell77.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[pesoglaslibras]")});
            this.xrTableCell77.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell77.Name = "xrTableCell77";
            this.xrTableCell77.StylePriority.UseFont = false;
            this.xrTableCell77.Text = "xrTableCell77";
            this.xrTableCell77.Weight = 0.687837557914318D;
            // 
            // xrTableCell116
            // 
            this.xrTableCell116.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel82});
            this.xrTableCell116.Font = new System.Drawing.Font("Calibri", 8F);
            this.xrTableCell116.Name = "xrTableCell116";
            this.xrTableCell116.StylePriority.UseFont = false;
            this.xrTableCell116.Text = "xrTableCell116";
            this.xrTableCell116.Weight = 0.26007805916932958D;
            // 
            // xrLabel82
            // 
            this.xrLabel82.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel82.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel82.Name = "xrLabel82";
            this.xrLabel82.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel82.SizeF = new System.Drawing.SizeF(25.97846F, 17.61261F);
            this.xrLabel82.StylePriority.UseFont = false;
            this.xrLabel82.StylePriority.UseTextAlignment = false;
            this.xrLabel82.Text = "LBS";
            this.xrLabel82.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableRow22
            // 
            this.xrTableRow22.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell54,
            this.xrTableCell101,
            this.xrTableCell100,
            this.xrTableCell103});
            this.xrTableRow22.Name = "xrTableRow22";
            this.xrTableRow22.Weight = 0.71831525771034732D;
            // 
            // xrTableCell54
            // 
            this.xrTableCell54.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel31});
            this.xrTableCell54.Name = "xrTableCell54";
            this.xrTableCell54.Text = "xrTableCell54";
            this.xrTableCell54.Weight = 1.4991604925370161D;
            // 
            // xrLabel31
            // 
            this.xrLabel31.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel31.LocationFloat = new DevExpress.Utils.PointFloat(9.999976F, 0F);
            this.xrLabel31.Name = "xrLabel31";
            this.xrLabel31.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel31.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel31.StylePriority.UseFont = false;
            this.xrLabel31.Text = "Partida";
            // 
            // xrTableCell101
            // 
            this.xrTableCell101.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TariffHeadingCode]")});
            this.xrTableCell101.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell101.Name = "xrTableCell101";
            this.xrTableCell101.StylePriority.UseFont = false;
            this.xrTableCell101.Text = "xrTableCell101";
            this.xrTableCell101.Weight = 0.8361380025892915D;
            // 
            // xrTableCell100
            // 
            this.xrTableCell100.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CON]")});
            this.xrTableCell100.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell100.Name = "xrTableCell100";
            this.xrTableCell100.StylePriority.UseFont = false;
            this.xrTableCell100.StylePriority.UseTextAlignment = false;
            this.xrTableCell100.Text = "xrTableCell100";
            this.xrTableCell100.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell100.Weight = 0.919365274613284D;
            // 
            // xrTableCell103
            // 
            this.xrTableCell103.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[numeroContenedores]")});
            this.xrTableCell103.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrTableCell103.Name = "xrTableCell103";
            this.xrTableCell103.StylePriority.UseFont = false;
            this.xrTableCell103.StylePriority.UseTextAlignment = false;
            this.xrTableCell103.Text = "xrTableCell103";
            this.xrTableCell103.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell103.Weight = 0.53700119402781588D;
            // 
            // xrTableRow23
            // 
            this.xrTableRow23.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell56,
            this.xrTableCell57});
            this.xrTableRow23.Name = "xrTableRow23";
            this.xrTableRow23.Weight = 0.7359865279124661D;
            // 
            // xrTableCell56
            // 
            this.xrTableCell56.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel47});
            this.xrTableCell56.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold);
            this.xrTableCell56.Name = "xrTableCell56";
            this.xrTableCell56.StylePriority.UseFont = false;
            this.xrTableCell56.Text = "xrTableCell56";
            this.xrTableCell56.Weight = 1.4991605810698374D;
            // 
            // xrLabel47
            // 
            this.xrLabel47.LocationFloat = new DevExpress.Utils.PointFloat(9.999973F, 0F);
            this.xrLabel47.Name = "xrLabel47";
            this.xrLabel47.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel47.SizeF = new System.Drawing.SizeF(99.99998F, 18.50013F);
            // 
            // xrTableCell57
            // 
            this.xrTableCell57.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel76});
            this.xrTableCell57.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Bold);
            this.xrTableCell57.Name = "xrTableCell57";
            this.xrTableCell57.StylePriority.UseFont = false;
            this.xrTableCell57.Text = "xrTableCell57";
            this.xrTableCell57.Weight = 2.2925043826975697D;
            // 
            // xrLabel76
            // 
            this.xrLabel76.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel76.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0.0001271566F);
            this.xrLabel76.Name = "xrLabel76";
            this.xrLabel76.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrLabel76.SizeF = new System.Drawing.SizeF(234.8081F, 18.50004F);
            this.xrLabel76.StylePriority.UseFont = false;
            this.xrLabel76.Text = "EXPORTADORES HABITUALES DE BIENES";
            // 
            // xrTableRow48
            // 
            this.xrTableRow48.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell124,
            this.xrTableCell125});
            this.xrTableRow48.Name = "xrTableRow48";
            this.xrTableRow48.Weight = 0.7359865279124661D;
            // 
            // xrTableCell124
            // 
            this.xrTableCell124.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel10});
            this.xrTableCell124.Name = "xrTableCell124";
            this.xrTableCell124.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell124.Text = "xrTableCell124";
            this.xrTableCell124.Weight = 1.4991605810698374D;
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(9.999968F, 0F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(100F, 17.46241F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "Archivo";
            // 
            // xrTableCell125
            // 
            this.xrTableCell125.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel59});
            this.xrTableCell125.Name = "xrTableCell125";
            this.xrTableCell125.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell125.Text = "xrTableCell125";
            this.xrTableCell125.Weight = 2.2925043826975697D;
            // 
            // xrLabel59
            // 
            this.xrLabel59.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[archivoXML]")});
            this.xrLabel59.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.xrLabel59.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel59.Name = "xrLabel59";
            this.xrLabel59.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel59.SizeF = new System.Drawing.SizeF(187.5932F, 17.4624F);
            this.xrLabel59.StylePriority.UseFont = false;
            // 
            // xrLabel19
            // 
            this.xrLabel19.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel19.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(2.566112F, 6.28564F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(214.451F, 22.99999F);
            this.xrLabel19.StylePriority.UseBorders = false;
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.Text = "Información Adicional";
            // 
            // formapago
            // 
            this.formapago.DataMember = "Setting";
            this.formapago.Expression = "Iif([code]=\'FPFF\',[name] , [name])";
            this.formapago.Name = "formapago";
            // 
            // plazo
            // 
            this.plazo.DataMember = "Setting";
            this.plazo.Expression = "Iif([code]=\'PLFF\',[name] ,[name] )";
            this.plazo.Name = "plazo";
            // 
            // calculatedField1
            // 
            this.calculatedField1.DataMember = "Setting";
            this.calculatedField1.Expression = "Iif([code]=\'FPFF\',[name] ,[name] )";
            this.calculatedField1.Name = "calculatedField1";
            // 
            // calculatedField2
            // 
            this.calculatedField2.DataMember = "Setting";
            this.calculatedField2.Expression = "Iif([code]=\'PLFF\',[value] ,[value] )";
            this.calculatedField2.Name = "calculatedField2";
            // 
            // calculatedField3
            // 
            this.calculatedField3.DataMember = "Setting";
            this.calculatedField3.Expression = "Iif([code]=\'PTFF\',[name] ,[name] )";
            this.calculatedField3.Name = "calculatedField3";
            // 
            // calculatedField4
            // 
            this.calculatedField4.DataMember = "InvoiceDetail.Invoice.InvoiceExteriorWeight";
            this.calculatedField4.Name = "calculatedField4";
            // 
            // calculatedField5
            // 
            this.calculatedField5.DataMember = "Invoice";
            this.calculatedField5.Expression = "Iif([InvoiceExteriorWeight].[WeightType].[code]=\'NET\' & [InvoiceExteriorWeight].[" +
    "id_metricUnit] = 4,[InvoiceExteriorWeight].[peso] ,\'\' )";
            this.calculatedField5.Name = "calculatedField5";
            // 
            // Netokilo
            // 
            this.Netokilo.DataMember = "InvoiceDetail";
            this.Netokilo.Expression = "Iif([Invoice].[InvoiceExteriorWeight].[id_WeightType]= 1 &[Invoice].[InvoiceExter" +
    "iorWeight].[id_metricUnit]= 1,[Invoice].[InvoiceExteriorWeight].[peso] , \'n\')";
            this.Netokilo.Name = "Netokilo";
            // 
            // netolibra
            // 
            this.netolibra.DataMember = "InvoiceDetail";
            this.netolibra.Expression = "Iif([Invoice].[InvoiceExteriorWeight].[id_WeightType]= 1 &[Invoice].[InvoiceExter" +
    "iorWeight].[id_metricUnit]= 4,[Invoice].[InvoiceExteriorWeight].[peso] , \'nl\')";
            this.netolibra.Name = "netolibra";
            // 
            // brutokilo
            // 
            this.brutokilo.DataMember = "InvoiceDetail";
            this.brutokilo.Expression = "Iif([Invoice].[InvoiceExteriorWeight].[id_WeightType]= 2 &[Invoice].[InvoiceExter" +
    "iorWeight].[id_metricUnit]= 1,[Invoice].[InvoiceExteriorWeight].[peso] , \'b\')";
            this.brutokilo.Name = "brutokilo";
            // 
            // calculatedField6
            // 
            this.calculatedField6.DataMember = "InvoiceDetail";
            this.calculatedField6.DisplayName = "brutolibra";
            this.calculatedField6.Expression = "Iif([Invoice].[InvoiceExteriorWeight].[id_WeightType]= 2 &[Invoice].[InvoiceExter" +
    "iorWeight].[id_metricUnit]= 4,[Invoice].[InvoiceExteriorWeight].[peso] , \'n\')";
            this.calculatedField6.Name = "calculatedField6";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "DBContextReports";
            this.sqlDataSource1.Name = "sqlDataSource1";
            storedProcQuery1.Name = "par_InvoiceExterior";
            queryParameter1.Name = "@id";
            queryParameter1.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("[Parameters.id_InvoiceExterior]", typeof(int));
            storedProcQuery1.Parameters.Add(queryParameter1);
            storedProcQuery1.StoredProcName = "par_InvoiceExterior";
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            storedProcQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // buque
            // 
            this.buque.DataMember = "par_InvoiceExterior";
            this.buque.Expression = "[shipName]+ \' \' +[shipNumberTrip]";
            this.buque.Name = "buque";
            // 
            // ciudadembarque
            // 
            this.ciudadembarque.DataMember = "par_InvoiceExterior";
            this.ciudadembarque.Expression = "[Port_1_nombre]+\', \'+[City_1_name]";
            this.ciudadembarque.Name = "ciudadembarque";
            // 
            // ciudaddestino
            // 
            this.ciudaddestino.DataMember = "par_InvoiceExterior";
            this.ciudaddestino.Expression = "[Port_2_nombre]+\', \'+[City_2_nombre]";
            this.ciudaddestino.Name = "ciudaddestino";
            // 
            // ciudaddescarga
            // 
            this.ciudaddescarga.DataMember = "par_InvoiceExterior";
            this.ciudaddescarga.Expression = "[nombre]+\', \'+[name_1]";
            this.ciudaddescarga.Name = "ciudaddescarga";
            // 
            // TotalTerm
            // 
            this.TotalTerm.DataMember = "par_InvoiceExterior";
            this.TotalTerm.Expression = "\'Total \' +[code_2]";
            this.TotalTerm.Name = "TotalTerm";
            // 
            // calculatedField7
            // 
            this.calculatedField7.DataMember = "par_InvoiceExterior";
            this.calculatedField7.Name = "calculatedField7";
            // 
            // archivoXML
            // 
            this.archivoXML.DataMember = "par_InvoiceExterior";
            this.archivoXML.Expression = "[accessKey] + \'.xml\'";
            this.archivoXML.Name = "archivoXML";
            // 
            // CON
            // 
            this.CON.DataMember = "par_InvoiceExterior";
            this.CON.Expression = "\'NroConten\' + [code_1]+ \':\'";
            this.CON.Name = "CON";
            // 
            // id_InvoiceExterior
            // 
            this.id_InvoiceExterior.Name = "id_InvoiceExterior";
            this.id_InvoiceExterior.Type = typeof(int);
            this.id_InvoiceExterior.ValueInfo = "0";
            this.id_InvoiceExterior.Visible = false;
            // 
            // InvoiceExteriorReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.ReportFooter});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.formapago,
            this.plazo,
            this.calculatedField1,
            this.calculatedField2,
            this.calculatedField3,
            this.calculatedField4,
            this.calculatedField5,
            this.Netokilo,
            this.netolibra,
            this.brutokilo,
            this.calculatedField6,
            this.buque,
            this.ciudadembarque,
            this.ciudaddestino,
            this.ciudaddescarga,
            this.TotalTerm,
            this.calculatedField7,
            this.archivoXML,
            this.CON});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.efDataSource1,
            this.sqlDataSource1});
            this.DataMember = "par_InvoiceExterior";
            this.DataSource = this.sqlDataSource1;
            this.Font = new System.Drawing.Font("Calibri", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(0, 28, 0, 100);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.id_InvoiceExterior});
            this.Version = "17.2";
            ((System.ComponentModel.ISupportInitialize)(this.efDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion
}
