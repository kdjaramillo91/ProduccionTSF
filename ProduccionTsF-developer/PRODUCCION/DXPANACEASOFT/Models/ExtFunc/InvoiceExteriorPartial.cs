using System;

namespace DXPANACEASOFT.Models
{
	public partial class InvoiceExterior
	{
		public void updateInvoiceExterior(InvoiceExterior updaterInvoiceExterior, Boolean isFirstTime, Person newBuyerData, User ActiveUser)
		{

			if (isFirstTime)
			{
				this.dateCreate = DateTime.Now;
				this.id_userCreate = ActiveUser.id;
			}

			this.id_consignee = updaterInvoiceExterior.id_consignee;
			this.id_notifier = updaterInvoiceExterior.id_notifier;
			this.purchaseOrder = updaterInvoiceExterior.purchaseOrder;
			this.daeNumber = updaterInvoiceExterior.daeNumber;
			this.daeNumber2 = updaterInvoiceExterior.daeNumber2;
			this.daeNumber3 = updaterInvoiceExterior.daeNumber3;
			this.daeNumber4 = updaterInvoiceExterior.daeNumber4;
			this.BLNumber = updaterInvoiceExterior.BLNumber;
			this.numberRemissionGuide = updaterInvoiceExterior.numberRemissionGuide;
			this.id_capacityContainer = updaterInvoiceExterior.id_capacityContainer;
			this.numeroContenedores = updaterInvoiceExterior.numeroContenedores;

			this.dateShipment = updaterInvoiceExterior.dateShipment;
            this.strDateShipment = updaterInvoiceExterior.strDateShipment;

            this.id_metricUnitInvoice = (updaterInvoiceExterior.id_metricUnitInvoice == 999) ? null : updaterInvoiceExterior.id_metricUnitInvoice;
			this.id_PaymentMethod = updaterInvoiceExterior.id_PaymentMethod;
			this.id_PaymentTerm = updaterInvoiceExterior.id_PaymentTerm;
			this.id_portDestination = updaterInvoiceExterior.id_portDestination;
			this.id_portDischarge = updaterInvoiceExterior.id_portDischarge;
			this.id_portShipment = updaterInvoiceExterior.id_portShipment;
			this.id_shippingAgency = updaterInvoiceExterior.id_shippingAgency;
			this.id_ShippingLine = updaterInvoiceExterior.id_ShippingLine;
			this.id_addressCustomer = updaterInvoiceExterior.id_addressCustomer;
			//this.id_tariffHeading = updaterInvoiceExterior.id_tariffHeading;
			this.id_termsNegotiation = updaterInvoiceExterior.id_termsNegotiation;
            this.transport = updaterInvoiceExterior.transport;

            this.shipName = updaterInvoiceExterior.shipName;
			this.shipNumberTrip = updaterInvoiceExterior.shipNumberTrip;
			this.valueCustomsExpenditures = updaterInvoiceExterior.valueCustomsExpenditures;
			this.valueInternationalFreight = updaterInvoiceExterior.valueInternationalFreight;
			this.valueInternationalInsurance = updaterInvoiceExterior.valueInternationalInsurance;
            this.valueTransportationExpenses = updaterInvoiceExterior.valueTransportationExpenses;
			this.valuetotalCIF = updaterInvoiceExterior.valuetotalCIF;
            this.valueTotalFOB = updaterInvoiceExterior.valueTotalFOB;

			this.id_userUpdate = ActiveUser.id;
			this.dateUpdate = DateTime.Now;

			this.idVendor = updaterInvoiceExterior.idVendor;
			this.id_fincncyCategory = updaterInvoiceExterior.id_fincncyCategory;
			this.id_bank = updaterInvoiceExterior.id_bank;
			this.valueSubscribed = updaterInvoiceExterior.valueSubscribed;
            this.finalPayment = updaterInvoiceExterior.finalPayment;
            this.balance = updaterInvoiceExterior.valuetotalCIF - updaterInvoiceExterior.valueSubscribed - updaterInvoiceExterior.finalPayment;     
            this.operation = updaterInvoiceExterior.operation;
			this.updateDate = updaterInvoiceExterior.updateDate;
			this.valueFinanced = updaterInvoiceExterior.valueFinanced;
			this.expirationDate = updaterInvoiceExterior.expirationDate;
			this.concessionDate = updaterInvoiceExterior.concessionDate;
			this.seals = updaterInvoiceExterior.seals;
			this.containers = updaterInvoiceExterior.containers;
			this.isInvoice = updaterInvoiceExterior.isInvoice;
            this.paid = updaterInvoiceExterior.paid;
            this.BL = updaterInvoiceExterior.BL;
			this.pList = updaterInvoiceExterior.pList;
			this.certificate = updaterInvoiceExterior.certificate;
			this.documentationState = updaterInvoiceExterior.documentationState;
			this.etaDate = updaterInvoiceExterior.etaDate;
			this.id_commissionAgent = updaterInvoiceExterior.id_commissionAgent;
			this.commissionAgent = updaterInvoiceExterior.commissionAgent;

			this.email = updaterInvoiceExterior.email;
			this.direccion = updaterInvoiceExterior.direccion;
			this.daeDate = updaterInvoiceExterior.daeDate;
			this.loadingDate = updaterInvoiceExterior.loadingDate;

			// Campos nuevos 20210706
			this.etdDate = updaterInvoiceExterior.etdDate;
			this.temperatureInstrucDate = updaterInvoiceExterior.temperatureInstrucDate;
			this.BookingNumber = updaterInvoiceExterior.BookingNumber;
			this.temperatureInstruction = updaterInvoiceExterior.temperatureInstruction;
			this.idTipoTemperatura = updaterInvoiceExterior.idTipoTemperatura;
			this.isfDate = updaterInvoiceExterior.isfDate;

            // Campos nuevos 20210926
            this.noContrato = updaterInvoiceExterior.noContrato;
            this.PO = updaterInvoiceExterior.PO;

			// Campos nuevos 20221024
			this.id_BankTransfer = updaterInvoiceExterior.id_BankTransfer;
			this.idPortfolioFinancing = updaterInvoiceExterior.idPortfolioFinancing;
			this.id_CityDelivery = updaterInvoiceExterior.id_CityDelivery;
			this.blDate = updaterInvoiceExterior.blDate;
		}

    }
}