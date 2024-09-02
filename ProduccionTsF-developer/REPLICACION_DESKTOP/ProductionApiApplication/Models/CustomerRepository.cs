using ProductionApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MigracionProduccionCIWebApi.Models
{
    internal class CustomerRepository
    {
        private DBContext db = new DBContext();
        private DBContextCI dbCI = new DBContextCI();

        public List<AnswerMigration> AddCustomers()
        {
            var listCustomerUpdate = new List<MigrationPerson>();

            try
            {
                listCustomerUpdate = db.MigrationPerson
                    .Where(w => w.Rol.name == "Cliente Local" || w.Rol.name == "Cliente Exterior")
                    .ToList();
            }
            catch (Exception e)
            {
                return new List<AnswerMigration>
                {
                new AnswerMigration
                {
                    resultado = false,
                    message = e.Message
					// Puedes agregar más propiedades según sea necesario para proporcionar información detallada
				}
                };
            }


            var listAnswerMigration = new List<AnswerMigration>();

            foreach (var customer in listCustomerUpdate)
            {
                var answerMigrationCustomer = this.AddCustomer(customer);
                listAnswerMigration.Add(answerMigrationCustomer);
            }

            return listAnswerMigration;
        }

        public string AddModifyCustomer(int id)
        {
            MigrationPerson mpTmp = null;
            AnswerMigration amTmp = null;

            try
            {
                mpTmp = db.MigrationPerson
                    .FirstOrDefault(p => p.Rol.name == "Cliente Local" && p.id_person == id);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en respuesta ADD/UPDATE: {e}");
            }

            if (mpTmp != null)
            {
                amTmp = this.AddCustomer(mpTmp, "En línea");
            }

            return (amTmp == null) ? "NO EXISTE CLIENTE DISPONIBLE"
                : (amTmp.resultado == true) ? "OK"
                : amTmp.message;
        }

        public void DeleteCustomers()
        {
            var listCustomerUpdate = db.MigrationPerson
                .Where(w => w.Rol.name == "Cliente Local" || w.Rol.name == "Cliente Exterior");

            foreach (var customer in listCustomerUpdate)
            {
                DeleteCustomer(customer);
            }
        }

        private AnswerMigration AddCustomer(MigrationPerson migrationPerson, string modo = "manual")
        {
            var answerMigration = new AnswerMigration();
            var newPerson = false;

            var messageCommon = String.Concat(
                "Persona: ", migrationPerson.Person?.fullname_businessName ?? String.Empty,
                " con tipo de Identificación: ", migrationPerson.Person?.IdentificationType.code ?? String.Empty,
                "(", migrationPerson.Person?.identification_number ?? String.Empty, ")");


            var customer = db.Customer
                .FirstOrDefault(c => c.id == migrationPerson.id_person);
            if (customer == null)
            {
                answerMigration.message = messageCommon
                    + ". No existe actualmente como CLIENTE. Se descarta su migración con este Rol";

                DeleteMigrationInsertHistoryCustomer(migrationPerson, answerMigration.message, modo);
                return answerMigration;
            }

            using (var transaction = dbCI.Database.BeginTransaction())
            {
                try
                {
                    var clienteCI = dbCI.TblGeCliente
                        .FirstOrDefault(c => c.CCiCliente == migrationPerson.Person.codeCI);

                    if (clienteCI == null)
                    {
                        clienteCI = new TblGeCliente();
                        newPerson = true;
                        clienteCI.CCiCliente = migrationPerson.Person.codeCI;
                    }

                    clienteCI.CCeCliente = migrationPerson.Person.isActive ? "A" : "I";
                    clienteCI.CNoCliente = migrationPerson.Person.fullname_businessName;
                    clienteCI.CdsNombreComercial = migrationPerson.Person.tradeName;
                    var maxLengthIdentification = (migrationPerson.Person.identification_number.Length > 13) ? 13 : (migrationPerson.Person.identification_number.Length);
                    clienteCI.CCiIdentificacion = migrationPerson.Person.identification_number.Substring(0, maxLengthIdentification); 
                    clienteCI.CCiTipoIdentificacion = (migrationPerson.Person.IdentificationType.codeSRI == "04") ? "R"
                        : (migrationPerson.Person.IdentificationType.codeSRI == "05") ? "C"
                        : (migrationPerson.Person.IdentificationType.codeSRI == "06") ? "P"
                        : (migrationPerson.Person.IdentificationType.codeSRI == "07") ? "CF"
                        : (migrationPerson.Person.IdentificationType.codeSRI == "08") ? "IE"
                        : (migrationPerson.Person.IdentificationType.codeSRI == "09") ? "PL"
                        : "";
                    var maxLength = (migrationPerson.Person.address.Length > 255) ? 255 : (migrationPerson.Person.address.Length);
                    clienteCI.CDsDireccion = migrationPerson.Person.address.Substring(0, maxLength);

                    clienteCI.CNuCelular = String.Empty;
                    clienteCI.CDsEmail2 = String.Empty;
                    clienteCI.CNuFax = String.Empty;
                    clienteCI.CNoNacionalidad = String.Empty;
                    clienteCI.CCiCiudad = String.Empty;
                    clienteCI.CCiParroquia = String.Empty;
                    clienteCI.CCiZona = String.Empty;
                    clienteCI.CDsLugarNacimiento = String.Empty;
                    clienteCI.DFxNacimiento = null;
                    clienteCI.CCiEstadoCivil = String.Empty;
                    clienteCI.CCiSexo = String.Empty;
                    clienteCI.NQnCarga = 0;
                    clienteCI.DFxRegistro = DateTime.Now;

                    clienteCI.CSNContribEspecial = customer.specialTaxPayerCusm ? "S" : "N";

                    clienteCI.CSNGrabaIva = customer.applyIva ? "S" : "N";
                    clienteCI.CCiTipoIdentificacionCony = String.Empty;
                    clienteCI.CCiIdentificacionCony = String.Empty;
                    clienteCI.CNoConyuge = String.Empty;
                    clienteCI.CNoNacionalidadCony = String.Empty;
                    clienteCI.CDsLugarNacimientoCony = String.Empty;
                    clienteCI.DFxNacimientoCony = null;
                    clienteCI.CDsDireccionCony = String.Empty;
                    clienteCI.CSNTieneCredito = String.Empty;

                    clienteCI.CNoContacto = String.Empty;
                    clienteCI.CNuTelefContacto = String.Empty;
                    clienteCI.CDsEmailContacto = String.Empty;
                    clienteCI.CNuFaxContacto = String.Empty;
                    clienteCI.CNuExtensionContacto = String.Empty;
                    clienteCI.CCiAcercamiento = String.Empty;
                    clienteCI.CCiEvento = String.Empty;
                    clienteCI.CCiMedio = String.Empty;
                    clienteCI.CNoReferido = String.Empty;
                    clienteCI.GlgFoto = null;
                    clienteCI.GlgFirma = null;

                    if (migrationPerson.isNewPerson || newPerson)
                    {
                        clienteCI.DFiIngreso = migrationPerson.dateCreate;
                        clienteCI.CCiUsuarioIngreso = db.User
                            .FirstOrDefault(u => u.id == migrationPerson.Person.id_userUpdate)
                            .username ?? "";
                        clienteCI.CDsEstacionIngreso = "PC-Producción";
                        clienteCI.DFmModifica = null;
                        clienteCI.CCiUsuarioModifica = null;
                        clienteCI.CDsEstacionModifica = null;
                    }
                    else
                    {
                        clienteCI.DFmModifica = migrationPerson.dateCreate;
                        clienteCI.CCiUsuarioModifica = db.User
                            .FirstOrDefault(u => u.id == migrationPerson.Person.id_userUpdate)
                            .username ?? "";
                        clienteCI.CDsEstacionModifica = "PC-Producción";
                    }

                    // Tipo de cliente
                    clienteCI.CCiTipoCliente = customer?.CustomerType?.code ?? "";
                    if (clienteCI.CCiTipoCliente != "")
                    {
                        var tipoCliente = dbCI.TblGeTipoCliente
                            .FirstOrDefault(tc => tc.CCiTipoCliente == customer.CustomerType.code);

                        var newTipoCliente = false;

                        if (tipoCliente == null)
                        {
                            tipoCliente = new TblGeTipoCliente();
                            tipoCliente.CCiTipoCliente = customer.CustomerType.code;
                            tipoCliente.CNoTipoCliente = customer.CustomerType.name;
                            tipoCliente.CCeTipoCliente = customer.CustomerType.isActive ? "A" : "I";

                            newTipoCliente = true;
                        }

                        if (newTipoCliente)
                        {
                            dbCI.TblGeTipoCliente.Add(tipoCliente);
                            dbCI.Entry(tipoCliente).State = EntityState.Added;
                        }
                    }

                    // Grupo económico
                    if (customer.id_economicGroupCusm != null)
                    {
                        clienteCI.CCiGrupoEconomico = customer.EconomicGroup.code;

                        var grupoEconomico = dbCI.TblGeGrupoEconomico
                            .FirstOrDefault(ge => ge.CCiGrupoEconomico == clienteCI.CCiGrupoEconomico);

                        var newGrupoEconomico = false;

                        if (grupoEconomico == null)
                        {
                            grupoEconomico = new TblGeGrupoEconomico();
                            grupoEconomico.CCiGrupoEconomico = customer.EconomicGroup.code;
                            grupoEconomico.CNoGrupoEconomico = customer.EconomicGroup.name;
                            grupoEconomico.CCeGrupoEconomico = customer.EconomicGroup.isActive ? "S" : "N";
                            grupoEconomico.CCiUsuarioIngreso = "master";
                            grupoEconomico.CDsEstacionIngreso = "producción";
                            grupoEconomico.DFxIngreso = DateTime.Now;
                            newGrupoEconomico = true;
                        }
                        if (newGrupoEconomico)
                        {
                            dbCI.TblGeGrupoEconomico.Add(grupoEconomico);
                            dbCI.Entry(grupoEconomico).State = EntityState.Added;
                        }
                    }

                    // Datos de Contacto
                    var contactoClienteTmp = db.CustomerContact
                        .FirstOrDefault(dc => dc.id_customer == customer.id);

                    if (contactoClienteTmp != null)
                    {
                        clienteCI.CNuTelefono1 = contactoClienteTmp.phoneNumber;
                        clienteCI.CNuTelefono2 = contactoClienteTmp.phoneCellular;
                        clienteCI.CDsEmail1 = contactoClienteTmp.emailGeneralBilling;
                        clienteCI.CDsEmailNCVenta = contactoClienteTmp.emailNC;
                        clienteCI.CDsEmailNCVentaHide = contactoClienteTmp.emailNC;
                        clienteCI.CDsEmailND = contactoClienteTmp.emailND;
                        clienteCI.CDsEmailNDHide = contactoClienteTmp.emailNDBc;
                        clienteCI.CDsPagWeb = contactoClienteTmp.website;
                    }

                    clienteCI.CsnAutorizaPagoCheque = String.Empty;
                    clienteCI.NvtMontoMaximoCheque = 0;
                    clienteCI.CsnBloqueoConsumo = String.Empty;
                    clienteCI.NNuDiasVctoPagoCheque = 0;
                    clienteCI.DFxVctoPagoCheque = null;
                    clienteCI.NNuPorcFactoring = 0;
                    clienteCI.NNuPorSeguroCredito = 0;


                    // Dirección del cliente
                    var customerAddressList = db.CustomerAddress
                        .Where(dc => dc.id_customer == customer.id)
                        .ToList();

                    if (customerAddressList != null && customerAddressList.Count > 0)
                    {
                        var direccionClienteList = dbCI.TblFaDireccionCliente
                            .Where(dc => dc.CciCliente == customer.Person.codeCI)
                            .ToList();

                        foreach (var caLstTmp in customerAddressList)
                        {
                            var dcTmp = direccionClienteList
                                .FirstOrDefault(dcl => dcl.CDsDireccion == caLstTmp.addressdescription);

                            var newDireccionCliente = false;

                            if (dcTmp == null)
                            {
                                dcTmp = new TblFaDireccionCliente();
                                dcTmp.CCiTipoDireccion = caLstTmp.AddressType.code;
                                dcTmp.CDsDireccion = caLstTmp.addressdescription;
                                dcTmp.CciCliente = clienteCI.CCiCliente;
                                dcTmp.CCiPais = String.Empty;
                                dcTmp.CCiCiudad = String.Empty;
                                dcTmp.CCiCanton = String.Empty;
                                dcTmp.CciParroquia = String.Empty;
                                dcTmp.CCiProvincia = String.Empty;

                                dcTmp.CTxTelefono = "0999999999";

                                dcTmp.NNuIdDireccion = direccionClienteList.Count > 0 && direccionClienteList?.Max(m => m.NNuIdDireccion) != null ? (short)((direccionClienteList?.Max(m => m.NNuIdDireccion)) + 1) : (short)1;
                                dcTmp.CCiTipoIdentificacion = (migrationPerson.Person.IdentificationType.codeSRI == "04") ? "R"
                                    : (migrationPerson.Person.IdentificationType.codeSRI == "05") ? "C"
                                    : (migrationPerson.Person.IdentificationType.codeSRI == "06") ? "P"
                                    : (migrationPerson.Person.IdentificationType.codeSRI == "07") ? "CF"
                                    : (migrationPerson.Person.IdentificationType.codeSRI == "08") ? "IE"
                                    : (migrationPerson.Person.IdentificationType.codeSRI == "09") ? "PL"
                                    : "";

                                newDireccionCliente = true;
                            }
                            if (newDireccionCliente)
                            {
                                dbCI.TblFaDireccionCliente.Add(dcTmp);
                                dbCI.Entry(dcTmp).State = EntityState.Added;
                            }
                        }
                    }


                    // Información de crédito
                    var customerCreditInfo = db.CustomerCreditInfo
                        .FirstOrDefault(cr => cr.id_customer == customer.id);

                    if (customerCreditInfo != null)
                    {
                        clienteCI.NNuDiasCredito = (short)customerCreditInfo.creditDays;
                        clienteCI.NNuValorCredito = customerCreditInfo.creditQuota;
                        clienteCI.NNuPorcDesc = customerCreditInfo.discountRate;

                        if (customerCreditInfo.PaymentMethod != null)
                        {
                            clienteCI.CCiFormaPago = customerCreditInfo?.PaymentMethod.code;
                        }
                        if (customerCreditInfo.PaymentTerm != null)
                        {
                            clienteCI.CCiPlazoPago = customerCreditInfo.PaymentTerm.code;
                        }
                    }
                    //clienteCI.CCiVendedor = "004";

                    // Vendedor asignado
                    if (customer.id_vendorAssigned != null)
                    {
                        clienteCI.CCiVendedor = customer.id_vendorAssigned.ToString();

                        var vendedor = dbCI.TblFaVendedor
                            .FirstOrDefault(v => v.CCiVendedor == customer.id_vendorAssigned.ToString());

                        var newVendedor = false;

                        if (vendedor == null)
                        {
                            vendedor = new TblFaVendedor();
                            vendedor.CCiVendedor = customer.id_vendorAssigned.ToString();
                            vendedor.CNoVendedor = customer.name_vendorAssigned;
                            vendedor.CCeVendedor = "A";
                            vendedor.CCiUsuarioIngreso = "master";
                            vendedor.DFxIngreso = DateTime.Now;
                            vendedor.CDsEstacionIngreso = "PRODUCCION";
                            vendedor.CCiUsuario = "";
                            newVendedor = true;
                        }

                        if (newVendedor)
                        {
                            dbCI.TblFaVendedor.Add(vendedor);
                            dbCI.Entry(vendedor).State = EntityState.Added;
                        }
                    }


                    if(customer.id_businessLine != null)
                    {
                        clienteCI.CCiCategoriaCliente = customer.id_businessLine.ToString();
                    }
                    else
                    {
                        clienteCI.CCiCategoriaCliente = String.Empty;
                    }
                    
                    if(customer.id_clientCategory != null)
                    {
                        clienteCI.CCiDistribucionClte = customer.id_clientCategory.ToString();
                    }
                    else
                    {
                        clienteCI.CCiDistribucionClte = String.Empty;
                    }

                    


                    clienteCI.CCiRutaVisitaCliente = String.Empty;
                    clienteCI.CSnLunes = String.Empty;
                    clienteCI.CSnMartes = String.Empty;
                    clienteCI.CSnMiercoles = String.Empty;
                    clienteCI.CSnJueves = String.Empty;
                    clienteCI.CSnViernes = String.Empty;
                    clienteCI.CSnSabado = String.Empty;
                    clienteCI.CSnDomingo = String.Empty;
                    clienteCI.CCiSujetoExpoImpo = String.Empty;
                    clienteCI.CNoRepresentanteLegal = String.Empty;
                    clienteCI.CCiRefContableClte = String.Empty;
                    clienteCI.CCtNaturaleza = String.Empty;
                    clienteCI.CciClienteRepresentante = String.Empty;
                    clienteCI.CCiEmpleadoRrhh = String.Empty;
                    clienteCI.CNoEmpleadoRrhh = String.Empty;
                    clienteCI.CsnObligaContabilidad = customer.forceToKeepAccountsCusm ? "S" : "N";
                    clienteCI.CSnRequiereNumeroOC = String.Empty;
                    clienteCI.CSnImprimeCertificadoCalidad = String.Empty;
                    clienteCI.CSnExcedenteDespacho = String.Empty;
                    clienteCI.NNuPorcExcedenteDespacho = 0;
                    clienteCI.CDsRecepcionMercaderia = String.Empty;
                    clienteCI.CDsRecepcionFactura = String.Empty;
                    clienteCI.CDsInstruccionEntrega = String.Empty;
                    clienteCI.CSnAntiguedadCartera = String.Empty;
                    clienteCI.CCtAntiguedadCartera = String.Empty;
                    clienteCI.NVtMaximoAntiguedadCartera = 0;
                    clienteCI.NNuPorcMaximoAntiguedadCartera = 0;
                    clienteCI.NNuDiaMaximoAntiguedadCartera = 0;
                    clienteCI.NCiRetencionFte = 0;
                    clienteCI.NCiRetencionFteIva = 0;
                    clienteCI.NCiRetenSuperIntCIA = 0;
                    clienteCI.CCiProvincia = String.Empty;
                    clienteCI.CCiOrigenIngresos = String.Empty;
                    clienteCI.CCiCanton = String.Empty;
                    clienteCI.CCiPais = String.Empty;

                    clienteCI.CSnObligadoContabilidad = customer.forceToKeepAccountsCusm ? "S" : "N";

                    clienteCI.NNuResolucionSRI = 0;
                    clienteCI.CSnEnviarCorreo = String.Empty;
                    clienteCI.CSnAutenticadoPortalExterno = String.Empty;
                    clienteCI.CCiGrupoEconomico = String.Empty;
                    clienteCI.CCiTipoPrecioProducto = String.Empty;
                    clienteCI.CDsEmailNCCobranza = String.Empty;

                    clienteCI.CTxCampoAdicionalDE1 = String.Empty;
                    clienteCI.CTxCampoAdicionalDE2 = String.Empty;
                    clienteCI.CTxCampoAdicionalDE3 = String.Empty;
                    clienteCI.CTxCampoAdicionalDE4 = String.Empty;
                    clienteCI.CTxCampoAdicionalDE5 = String.Empty;
                    clienteCI.CSnExcluyeCADireccionXML = String.Empty;
                    clienteCI.CSnExcluyeCATelefonoXML = String.Empty;
                    clienteCI.CSnExcluyeCAEmailXML = String.Empty;
                    clienteCI.CSnIncluyeEnCorreoCA1 = String.Empty;
                    clienteCI.CSnIncluyeEnCorreoCA2 = String.Empty;
                    clienteCI.CSnIncluyeEnCorreoCA3 = String.Empty;
                    clienteCI.CSnIncluyeEnCorreoCA4 = String.Empty;
                    clienteCI.CSnIncluyeEnCorreoCA5 = String.Empty;
                    clienteCI.CDsEmailFacturaHide = String.Empty;
                    clienteCI.CDsEmailNCCobranzaHide = String.Empty;

                    clienteCI.CSnCnfgMsjEmailxClte = String.Empty;
                    clienteCI.CDsMensajeEmailFE = String.Empty;
                    clienteCI.CSnExcluyeCAObsvXML = String.Empty;
                    clienteCI.CSnExcluyeRefDcto = String.Empty;
                    clienteCI.CSnCnfgAsuntoEmailxClte = String.Empty;
                    clienteCI.CDsAsuntoEmailFESA = String.Empty;
                    clienteCI.CDsAsuntoEmailFECA = String.Empty;
                    clienteCI.CSnExcluyeXMLAdjunto = String.Empty;
                    clienteCI.CSnExcluyeRIDEAdjunto = String.Empty;
                    clienteCI.CDsEmailRetPresuntiva = String.Empty;
                    clienteCI.NVtPonderado = 0;
                    clienteCI.DFxPonderado = null;
                    clienteCI.CDsObservacion = String.Empty;
                    clienteCI.CCtOrigen = String.Empty;
                    clienteCI.CSnParteRelacionada = String.Empty;
                    clienteCI.NVtValorDonacion = 0;
                    clienteCI.NnuFichaProducto = 0;
                    clienteCI.CDsEmailAvisoCobro = String.Empty;
                    clienteCI.NNuPorcDsctoRecarga = 0;
                    clienteCI.CsnAplicaLeySolidaria = String.Empty;

                    if (newPerson)
                    {
                        dbCI.TblGeCliente.Add(clienteCI);
                        dbCI.Entry(clienteCI).State = EntityState.Added;
                    }
                    else
                    {
                        dbCI.Entry(clienteCI).State = EntityState.Modified;
                    }

                    dbCI.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR en respuesta ADD: {e}");

                    transaction.Rollback();

                    answerMigration.message = messageCommon + ". Error no esperado: " + e.Message;
                    answerMigration.resultado = false;
                    UpdateMigrationCustomer(migrationPerson, answerMigration.message, modo);

                    return answerMigration;
                }
            }

            answerMigration.message = messageCommon + ". Migrado satisfactoriamente.";
            DeleteMigrationInsertHistoryCustomer(migrationPerson, answerMigration.message, modo);
            answerMigration.resultado = true;

            return answerMigration;
        }

        private void DeleteCustomer(MigrationPerson migrationPerson)
        {
            try
            {
                db.Entry(migrationPerson).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en respuesta DELETE: {e}");
            }
        }

        private void UpdateMigrationCustomer(MigrationPerson migrationPerson, string message, string modo = "manual")
        {
            var historyMigrationCustomer = db.HistoryMigrationPerson
                .FirstOrDefault(h => h.id_migrationPerson == migrationPerson.id_person);

            var fechaCreacion = migrationPerson.dateCreate;

            if (historyMigrationCustomer != null)
            {
                historyMigrationCustomer.message = message;
                historyMigrationCustomer.mode = modo;
                db.Entry(historyMigrationCustomer).State = EntityState.Modified;
            }
            else
            {
                var userCreateAux = db.User
                    .FirstOrDefault(u => u.username == "admin" /*|| fod.id == 1*/ || u.username.Contains("admin"));

                var historyMigrationPerson = new HistoryMigrationPerson
                {
                    id_migrationPerson = migrationPerson.id,
                    id_person = migrationPerson.id_person,
                    id_rol = migrationPerson.id_rol,
                    id_userCreateMigrationPerson = migrationPerson.id_userCreate,
                    dateCreateMigrationPerson = migrationPerson.dateCreate,
                    message = message,
                    mode = modo,
                    id_userCreate = userCreateAux?.id,
                    dateCreate = DateTime.Now
                };

                db.Entry(historyMigrationPerson).State = EntityState.Added;
            }

            db.SaveChanges();
        }

        private void DeleteMigrationInsertHistoryCustomer(MigrationPerson migrationPerson, string message, string modo = "manual")
        {
            var id_ur = migrationPerson.id_user_replicate;
            var fechaCreacion = migrationPerson.dateCreate;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var userCreateAux = db.User
                        .FirstOrDefault(u => u.username == "admin" /*|| fod.id == 1*/ || u.username.Contains("admin"));
                    // ?? db.User.FirstOrDefault();

                    var historyMigrationPersonTmp = new HistoryMigrationPerson();

                    historyMigrationPersonTmp.id_migrationPerson = migrationPerson.id;
                    historyMigrationPersonTmp.id_person = migrationPerson.id_person;
                    historyMigrationPersonTmp.id_rol = migrationPerson.id_rol;
                    historyMigrationPersonTmp.id_userCreateMigrationPerson = migrationPerson.id_userCreate;
                    historyMigrationPersonTmp.dateCreateMigrationPerson = migrationPerson.dateCreate;
                    historyMigrationPersonTmp.message = message;
                    historyMigrationPersonTmp.mode = modo;

                    historyMigrationPersonTmp.id_user_replicate = id_ur;
                    var maxLength = (migrationPerson.Person.address.Length > 255) ? 255 : (migrationPerson.Person.address.Length);
                    historyMigrationPersonTmp.id_userCreate = migrationPerson.id_userCreate;
                    historyMigrationPersonTmp.dateCreate = DateTime.Now;

                    db.HistoryMigrationPerson.Add(historyMigrationPersonTmp);
                    db.Entry(historyMigrationPersonTmp).State = EntityState.Added;

                    //Delete
                    db.MigrationPerson.Remove(migrationPerson);
                    db.Entry(migrationPerson).State = EntityState.Deleted;

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR en respuesta DELETE: {e}");
                    transaction.Rollback();
                }
            }
        }
    }
}