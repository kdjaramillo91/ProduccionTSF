using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class SalesQuotationExterior
    {



        private DBContext db = new DBContext();
        public void updateSalesQuotationExterior(SalesQuotationExterior updaterInvoiceExterior, Boolean isFirstTime, Person newBuyerData, User ActiveUser)
        {

            if (isFirstTime)
            {
                this.dateCreate = DateTime.Now;
                this.id_userCreate = ActiveUser.id;
            }

            this.id_consignee = updaterInvoiceExterior.id_consignee;
            this.id_notifier = updaterInvoiceExterior.id_notifier;
            this.idVendor = updaterInvoiceExterior.idVendor;
            this.purchaseOrder = updaterInvoiceExterior.purchaseOrder;
            this.dateShipment = updaterInvoiceExterior.dateShipment;
            this.strDateShipment = updaterInvoiceExterior.strDateShipment;
            this.id_PaymentMethod = updaterInvoiceExterior.id_PaymentMethod;
            this.id_PaymentTerm = updaterInvoiceExterior.id_PaymentTerm;
            this.id_addressCustomer = updaterInvoiceExterior.id_addressCustomer;
            this.transport = updaterInvoiceExterior.transport;
            this.id_portDestination = updaterInvoiceExterior.id_portDestination;
            this.id_portDischarge = updaterInvoiceExterior.id_portDischarge;
            this.id_portShipment = updaterInvoiceExterior.id_portShipment;
            this.id_portTerminal = updaterInvoiceExterior.id_portTerminal;
            this.id_termsNegotiation = updaterInvoiceExterior.id_termsNegotiation;
            this.id_bank = updaterInvoiceExterior.id_bank;
            this.id_personContact = updaterInvoiceExterior.id_personContact;
            this.id_personContactConsignatario = updaterInvoiceExterior.id_personContactConsignatario;
            this.Product = updaterInvoiceExterior.Product;
            this.ColourGrade = updaterInvoiceExterior.ColourGrade;
            this.PackingDetails = updaterInvoiceExterior.PackingDetails;
            this.ContainerDetails = updaterInvoiceExterior.ContainerDetails;
            this.Shipment_date = updaterInvoiceExterior.Shipment_date;

            this.valueCustomsExpenditures = updaterInvoiceExterior.valueCustomsExpenditures;
            this.valueInternationalFreight = updaterInvoiceExterior.valueInternationalFreight;
            this.valueInternationalInsurance = updaterInvoiceExterior.valueInternationalInsurance;
            this.valueTransportationExpenses = updaterInvoiceExterior.valueTransportationExpenses;
            this.valuetotalCIF = updaterInvoiceExterior.valuetotalCIF;
            this.valueTotalFOB = updaterInvoiceExterior.valueTotalFOB;

            this.valueSubscribed = updaterInvoiceExterior.valueSubscribed;
            this.numeroContenedores = updaterInvoiceExterior.numeroContenedores;
            this.id_capacityContainer = updaterInvoiceExterior.id_capacityContainer;
            this.temperatureInstruction = updaterInvoiceExterior.temperatureInstruction;
            this.idTipoTemperatura = updaterInvoiceExterior.idTipoTemperatura;
            this.temperatureInstrucDate = updaterInvoiceExterior.temperatureInstrucDate;

            this.vessel = updaterInvoiceExterior.vessel;
            this.trip = updaterInvoiceExterior.trip;
            this.id_BankTransfer = updaterInvoiceExterior.id_BankTransfer;
            this.PO = updaterInvoiceExterior.PO;
            this.noContrato = updaterInvoiceExterior.noContrato;

            if (newBuyerData != null)
            {
                int lenDireccion = (newBuyerData.address.Length > 99) ? 99 : newBuyerData.address.Length;
                this.direccion = newBuyerData.address.Substring(0, lenDireccion);
                int lenEmail = (newBuyerData.email.Length > 49) ? 49 : newBuyerData.email.Length;
                if (newBuyerData != null) this.email = newBuyerData.email.Substring(0, lenEmail);
            }



            this.id_userUpdate = ActiveUser.id;
            this.dateUpdate = DateTime.Now;
        }
    }
}