using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Filter
{
    public class PurchaseOrderFilterForm : BaseFilterForm
    {
        public PurchaseOrderFilterForm()
        {
            //Fechas
            ListFilterType.Add(new FilterType { id = 1, type = "Date", name = "Document.emissionDate", alias = "Fecha Emisión" });
            ListFilterType.Add(new FilterType { id = 2, type = "Date", name = "deliveryDate", alias = "Fecha Entrega" });
            ListFilterType.Add(new FilterType { id = 3, type = "Date", name = "PurchaseOrderImportationInformation.shipmentDate", alias = "Fecha Embarque" });
            ListFilterType.Add(new FilterType { id = 4, type = "Date", name = "PurchaseOrderImportationInformation.departureCustomsDate", alias = "Fecha Salida Aduana" });
            ListFilterType.Add(new FilterType { id = 5, type = "Date", name = "PurchaseOrderImportationInformation.arrivalDate", alias = "Fecha Arribo" });
            //Textos
            ListFilterType.Add(new FilterType { id = 6, type = "Text", name = "Document.number", alias = "Número" });
            ListFilterType.Add(new FilterType { id = 7, type = "Text", name = "sendTo", alias = "Enviar a" });
            ListFilterType.Add(new FilterType { id = 8, type = "Text", name = "deliveryTo", alias = "Entregar a" });
            //Número
            ListFilterType.Add(new FilterType { id = 9, type = "Number", name = "PurchaseOrderTotal.subtotalIVA12Percent", alias = "Subtotal IVA 12%"});
            ListFilterType.Add(new FilterType { id = 10, type = "Number", name = "PurchaseOrderTotal.subtotalIVA14Percent", alias = "Subtotal IVA 14%"});
            ListFilterType.Add(new FilterType { id = 11, type = "Number", name = "PurchaseOrderTotal.subtotal", alias = "Subtotal Orden"});
            ListFilterType.Add(new FilterType { id = 12, type = "Number", name = "PurchaseOrderTotal.totalIVA12", alias = "Total IVA 12%" });
            ListFilterType.Add(new FilterType { id = 13, type = "Number", name = "PurchaseOrderTotal.totalIVA14", alias = "Total IVA 14%" });
            ListFilterType.Add(new FilterType { id = 14, type = "Number", name = "PurchaseOrderTotal.total", alias = "Total Orden" });
            ListFilterType.Add(new FilterType { id = 15, type = "Number", name = "subtotal", alias = "Subtotal", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 16, type = "Number", name = "total", alias = "Total", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 17, type = "Number", name = "quantityRequested", alias = "Cantidad Requerida", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 18, type = "Number", name = "quantityOrder", alias = "Cantidad Ordenada", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 19, type = "Number", name = "quantityApproved", alias = "Cantidad Aprobada", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 20, type = "Number", name = "quantityReceived", alias = "Cantidad Recibida", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 21, type = "Number", name = "price", alias = "Precio", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 22, type = "Number", name = "iva", alias = "IVA Detalle", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 23, type = "Number", name = "subtotal", alias = "Subtotal Detalle", nameDetail = "PurchaseOrderDetail" });
            ListFilterType.Add(new FilterType { id = 24, type = "Number", name = "total", alias = "Total Detalle", nameDetail = "PurchaseOrderDetail" });
            //Selección
            ListFilterType.Add(new FilterType { id = 25, type = "Select", name = "Document.id_documentState", alias = "Estado", dataSource = "DataProviderDocumentState.AllDocumentStatesByCompany" });
            ListFilterType.Add(new FilterType { id = 26, type = "Select", name = "id_paymentTerm", alias = "Plazo de Pago", dataSource = "DataProviderPaymentTerm.AllPaymentTermsByCompany" });
            ListFilterType.Add(new FilterType { id = 27, type = "Select", name = "id_paymentMethod", alias = "Medio de Pago", dataSource = "DataProviderPaymentMethod.AllPaymentMethodsByCompany" });
            ListFilterType.Add(new FilterType { id = 28, type = "Select", name = "id_provider", alias = "Proveedor", dataSource = "DataProviderPerson.AllProvidersByCompany" });
            ListFilterType.Add(new FilterType { id = 29, type = "Select", name = "id_providerapparent", alias = "Proveedor Amparante", dataSource = "DataProviderPerson.AllProvidersByCompany" });
            ListFilterType.Add(new FilterType { id = 30, type = "Select", name = "id_buyer", alias = "Comprador", dataSource = "DataProviderPerson.AllBuyersByCompany" });
            ListFilterType.Add(new FilterType { id = 31, type = "Select", name = "id_item", alias = "Producto Detalle", nameDetail = "PurchaseOrderDetail", dataSource = "DataProviderItem.AllPurchaseItemsByCompany" });
            ListFilterType.Add(new FilterType { id = 32, type = "Select", name = "id_priceList", alias = "Lista de Precio/Cotización", dataSource = "DataProviderPriceList.AllPriceListsForPurchaseByCompany" });
            //Chequeo
            ListFilterType.Add(new FilterType { id = 33, type = "Check", name = "requiredLogistic", alias = "Requiere Logística" });
            ListFilterType.Add(new FilterType { id = 34, type = "Check", name = "isImportation", alias = "Requiere Importación" });

            //this.octcodigo = "";
            //this.ListFilterType = new List<FilterType>();
            //this.ListFilterText = new List<FilterText>();
            //this.ListFilterNumber = new List<FilterNumber>();
            //this.ListFilterSelect = new List<FilterSelect>();
        }


        //public string octcodigo { get; set; }

        //public List<FilterType> ListFilterType { get; set; }
        //public List<FilterDate> ListFilterDate { get; set; }
        //public List<FilterDate> ListFilterDate { get; set; }
        //public List<FilterDate> ListFilterDate { get; set; }

        //public class FilterType
        //{
        //    public string type { get; set; }//Date, Text, Number, Select, Check
        //    public string name { get; set; }
        //    public string alias { get; set; }
        //    public string nameDetail { get; set; }
        //    public string dataSource { get; set; }
        //}
            
        //public Phase phase { get; set; }
        //public string listId_BusinessOportunity { get; set; }
        //public int quantity { get; set; }
        //public decimal amountExpected { get; set; }
        //public decimal weightedAmount { get; set; }
        //public decimal percent { get; set; }

    }
}