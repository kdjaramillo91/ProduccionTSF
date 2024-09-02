using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrigenListaLP;
using DestinoListaProduccion;
using CAREPLICATEPRODUCTION.ListaPrecioToProd.Modelos;
using System.Data.Entity;
using Utilitarios.Logs;

namespace CAREPLICATEPRODUCTION.ListaPrecioToProd.Implementacion
{
    public class ListaPrecioReplicacion
    {
        #region ATTRIBUTES
        private int idListReplicate { get; set; }

        private string idsListReplicate { get; set; }
        private DBContextoListaLP dbLp { get; }
        private DBContextoListaProduccion dbProduccion { get; }
        private string ruta { get; set; }
        #endregion
        public ListaPrecioReplicacion(int _idLpReplicate, string _ruta, string _idsListReplicate)
        {
            this.idListReplicate = _idLpReplicate;
            this.idsListReplicate = _idsListReplicate;
            dbLp = new DBContextoListaLP();
            dbProduccion = new DBContextoListaProduccion();
            this.ruta = _ruta;
        }
        public void ReplicateInformation()
        {
            #region Obtiene Información Origen
            ListaSourceDTO _psDto = ObtieneListaOrigen(this.idListReplicate);
            #endregion

            #region Valida y Carga
            var resultado = ValidaCargaDatos(_psDto);
            if(resultado == 2) ValidaCargaDatos(_psDto);

            #endregion
        }
        public void ReplicateInformationInternal(int id)
        {
            #region Obtiene Información Origen
            ListaSourceDTO _psDto = ObtieneListaOrigen(id);
            #endregion

            #region Valida y Carga
            ValidaCargaDatos(_psDto);
            #endregion
        }
        public void ReplicateInformationMassive()
        {
            string idsReplicate = this.idsListReplicate;
            if (idsReplicate != "")
            {
                idsReplicate = idsReplicate.TrimEnd(',');

                var ids = idsReplicate.Split(',');
                if (ids != null && ids.Length > 0)
                {
                    foreach (string i in ids)
                    {
                        ReplicateInformationInternal(Convert.ToInt32(i));
                    }
                }
            }
        }
        private ListaSourceDTO ObtieneListaOrigen(int _idPriceList)
        {
            ListaSourceDTO psDTO = null;
            var _priceList = dbLp
                            .PriceList
                            .FirstOrDefault(fod => fod.id == _idPriceList);

            if (_priceList != null)
            {
                psDTO = new ListaSourceDTO();
                #region Recupero Lista

                psDTO.Lista = new ListaPrecio
                {
                    idLista = _priceList.id,
                    nombreLista = _priceList.name,
                    fechaInicio = _priceList.startDate,
                    fechaFin = _priceList.endDate,
                    esCompra = _priceList.isForPurchase,
                    esVenta = _priceList.isForSold,
                    esCotizacion = _priceList.isQuotation,
                    idListaPrecioCalendario = _priceList.id_calendarPriceList,
                    esGrupo = _priceList.byGroup,
                    idGrupoPersonaRol = _priceList.id_groupPersonByRol,
                    idListaBase = _priceList.id_priceListBase,
                    nombreListaBase = dbLp.PriceList.FirstOrDefault(fod => fod.id == _priceList.id_priceListBase)?.name,
                    codCertificacion = _priceList.Certification?.code,
                    idCertificacion = _priceList.id_certification,
                    id_company = _priceList.id_company,
                    idTipoProceso = _priceList.id_processtype,
                    codigoTipoProceso = _priceList.ProcessType.code,
                    commercialDate = _priceList.commercialDate,
                    idUsuarioResponsable = _priceList.id_userResponsable,
                    nombreUsuarioResponsable = dbLp.User.FirstOrDefault(fod => fod.id == _priceList.id_userResponsable)?.username ?? "",
                    idUsuarioCreacion = _priceList.Document.id_userCreate,
                    nombreUsuarioCreacion = dbLp.User.FirstOrDefault(fod => fod.id == _priceList.Document.id_userCreate)?.username ?? "",
                    dtUsuarioCreacion = _priceList.Document.dateCreate,
                    idUsuarioModificacion = _priceList.Document.id_userUpdate,
                    nombreUsuarioModificacion = dbLp.User.FirstOrDefault(fod => fod.id == _priceList.Document.id_userUpdate)?.username ?? "",
                    dtUsuarioModificacion = _priceList.Document.dateUpdate
                };

                #endregion

                #region Recupero Documento

                psDTO.ListaDocumento = new Documento
                {
                    idDocument = _priceList.Document.id,
                    numero = _priceList.Document.number,
                    sequencial = _priceList.Document.sequential,
                    fechaEmision = _priceList.Document.emissionDate,
                    idPuntoEmision = _priceList.Document.id_emissionPoint,
                    idTipoDocumento = _priceList.Document.id_documentType,
                    codigoTipoDocumento = _priceList.Document.DocumentType.code,
                    idEstadoDocumento = _priceList.Document.id_documentState,
                    nombreEstadoDocumento = _priceList.Document.DocumentState.name
                };

                #endregion

                #region Recupero Detalle

                psDTO.Detalle = _priceList
                                    .PriceListItemSizeDetail
                                    .Select(s => new ListaPrecioDetalle
                                    {
                                        id = s.Id,
                                        idTalla = s.Id_Itemsize,
                                        codigoTalla = s.ItemSize.code,
                                        valor = s.price,
                                        comision = s.commission,
                                        codigoClaseCamaron = s.ClassShrimp.code,
                                        idClaseCamaron = s.id_classShrimp,
                                        codigoCalidad = s.Class.code,
                                        idCalidad = s.id_class
                                    }).ToList();

                #endregion

                #region Recupero Penalización

                psDTO.DetallePenalizacion = _priceList
                                                .PriceListClassShrimp
                                                .Select(s => new ClasePenalizacion
                                                {
                                                    idClaseCamaron = s.id,
                                                    codigoClaseCamaron = s.ClassShrimp.code,
                                                    valorClaseCamaron = s.value
                                                }).ToList();

                #endregion

                #region Recupero Calendario

                psDTO.CalendarioLista = new Calendario
                {
                    id = _priceList.CalendarPriceList.id,
                    nombre = _priceList.CalendarPriceList.name,
                    idTipoCalendarioLista = _priceList.CalendarPriceList.id_calendarPriceListType,
                    fechaInicio = _priceList.CalendarPriceList.startDate,
                    fechaFin = _priceList.CalendarPriceList.endDate,
                    idCompania = _priceList.CalendarPriceList.id_company,
                    activo = _priceList.CalendarPriceList.isActive,
                };

                #endregion

                #region Recupero Personas en Lista

                psDTO.lstPersonaDetalle = _priceList
                                            .PriceListPersonPersonRol
                                            .Select(s => new PersonaListaDetalle
                                            {
                                                idPersona = s.id_Person,
                                                identificacionPersona = s.Person.identification_number,
                                                idRol = s.id_Rol,
                                                nombreRol = s.Rol.name
                                            }).ToList();
                #endregion

                #region Recupero Tipo Calendario

                psDTO.TipoCalendarioLista = new TipoCalendario
                {
                    id = _priceList.CalendarPriceList.id_calendarPriceListType,
                    nombre = _priceList.CalendarPriceList.CalendarPriceListType.name,
                    descripcion = _priceList.CalendarPriceList.CalendarPriceListType.description,
                    idCompania = _priceList.CalendarPriceList.CalendarPriceListType.id_company,
                    activo = _priceList.CalendarPriceList.CalendarPriceListType.isActive,
                };


                #endregion

                #region Recupero Grupo y su respectivo Detalle

                if (_priceList.byGroup != null && (bool)_priceList.byGroup)
                {
                    psDTO.GrupoPersonaRolLista = new GrupoPersonaRol
                    {
                        id = _priceList.GroupPersonByRol.id,
                        nombre = _priceList.GroupPersonByRol.name,
                        descripcion = _priceList.GroupPersonByRol.description,
                        idCompania = _priceList.GroupPersonByRol.id_company,
                        idRol = _priceList.GroupPersonByRol.id_rol,
                        nombreRol = _priceList.GroupPersonByRol.Rol.name,
                        activo = _priceList.GroupPersonByRol.isActive
                    };

                    psDTO.lstGrupoPersonaRolDetalle = new List<GrupoPersonaRolDetalle>();
                    psDTO.lstGrupoPersonaRolDetalle = _priceList
                                                        .GroupPersonByRol
                                                        .GroupPersonByRolDetail
                                                        .Select(s => new GrupoPersonaRolDetalle
                                                        {
                                                            idGrupo = s.id_groupPersonByRol,
                                                            nombreGrupo = s.GroupPersonByRol.name,
                                                            idPersona = s.id_person,
                                                            identificacionPersona = s.Person.identification_number
                                                        }).ToList();
                }

                #endregion

                #region Recupero Documento Cambio
                psDTO.CambioEstadoDocumento = _priceList
                                                .Document
                                                .DocumentStateChange
                                                .Select(s => new DocumentoCambioEstado
                                                {
                                                    idEstadoDocumentOldSource = s.id_documentStateOld,
                                                    codigoEstadoDocumentoOldSource = s.DocumentState.code,
                                                    nombreEstadoDocumentoOldSource = s.DocumentState.name,
                                                    idEstadoDocumentNewSource = s.id_documentStateNew,
                                                    codigoEstadoDocumentoNewSource = s.DocumentState1.code,
                                                    nombreEstadoDocumentoNewSource = s.DocumentState1.name,
                                                    idUsuarioSource = s.id_user,
                                                    nombreUsuarioSource = s.User.username,
                                                    idUsuarioGrupoSource = s.id_userGroup,
                                                    nombreUsuarioGrupoSource = s.UserGroup.name,
                                                    changeTimeSource = s.changeTime
                                                }).ToList();
                #endregion
            }


            return psDTO;
        }
        private int ValidaCargaDatos(ListaSourceDTO _psDto)
        {
            int _resultado = 0;
            bool isNew = false;
            bool repeat = false;

            #region Variables
            DestinoListaProduccion.CalendarPriceList _cpl = null;
            DestinoListaProduccion.GroupPersonByRol _gpr = null;
            #endregion

            if (_psDto != null)
            {
                //using (DbContextTransaction trans = dbProduccion.Database.BeginTransaction())
                {
                    try
                    {
                        //Busco Lista en RegisterInfReplicationSource
                        int idRegisterSource =  dbProduccion
                                                .RegisterInfReplicationSource
                                                .FirstOrDefault(fod => fod.idRegisterSource == _psDto.Lista.idLista 
                                                && fod.schemaRegister.Equals("PriceList"))?.idRegisterDestination ?? 0;

                        DestinoListaProduccion.PriceList _plProd = dbProduccion
                                                                    .PriceList
                                                                    .FirstOrDefault(fod => fod.id == idRegisterSource);
                        if (_plProd != null)
                        {
                            _plProd.name = _psDto.Lista.nombreLista.Trim();
                            _plProd.startDate = _psDto.Lista.fechaInicio;
                            _plProd.endDate = (DateTime)_psDto.Lista.fechaFin;
                            _plProd.isForPurchase = _psDto.Lista.esCompra;
                            _plProd.isForSold = _psDto.Lista.esVenta;
                            _plProd.isQuotation = _psDto.Lista.esCotizacion;

                            int idRegisterSourceBase = dbProduccion
                                .RegisterInfReplicationSource
                                .FirstOrDefault(fod => fod.idRegisterSource == _psDto.Lista.idListaBase
                                && fod.schemaRegister.Equals("PriceList"))?.idRegisterDestination ?? 0;

                            _plProd.id_priceListBase = dbProduccion.PriceList.FirstOrDefault(fod => fod.id == idRegisterSourceBase)?.id ?? null;
                           
                            #region Información Documento

                            _plProd.Document.number = _psDto.ListaDocumento.numero;
                            _plProd.Document.sequential = _psDto.ListaDocumento.sequencial;
                            _plProd.Document.emissionDate = _psDto.ListaDocumento.fechaEmision;
                            _plProd.Document.dateUpdate = DateTime.Now;
                            _plProd.Document.id_documentState = dbProduccion.DocumentState.FirstOrDefault(fod => fod.name.Trim().Equals(_psDto.ListaDocumento.nombreEstadoDocumento.Trim())).id;

                            #endregion
                            
                            #region Dato Maestro Calendario
                            //DatoMaestro Calendario
                            int idRegisterCalendarSource = dbProduccion
                                                .ReplicationMasterProduction
                                                .FirstOrDefault(fod => fod.idPrincipalSchemaSource == _psDto.CalendarioLista.id
                                                && fod.nameSchema.Equals("CalendarPriceList"))?
                                                .idPrincipalSchemaDestination ?? 0;

                            var _CalendarioTmp = dbProduccion
                                                    .CalendarPriceList
                                                    .FirstOrDefault(fod => fod.id == idRegisterCalendarSource);

                            if (_CalendarioTmp != null)
                            {
                                _plProd.id_calendarPriceList = _CalendarioTmp.id;
                            }
                            else
                            {
                                var _TipoCalendarioTmp = dbProduccion
                                                        .CalendarPriceListType
                                                        .FirstOrDefault(fod => fod.name.Trim().Equals(_psDto.TipoCalendarioLista.nombre.Trim()));

                                _cpl = new DestinoListaProduccion.CalendarPriceList
                                {
                                    name = _psDto.CalendarioLista.nombre,
                                    id_calendarPriceListType = _TipoCalendarioTmp.id,
                                    startDate = _psDto.CalendarioLista.fechaInicio,
                                    endDate = _psDto.CalendarioLista.fechaFin,
                                    id_company = 2,
                                    isActive = _psDto.CalendarioLista.activo,
                                    id_userCreate = 1,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = 1,
                                    dateUpdate = DateTime.Now
                                };

                                dbProduccion.CalendarPriceList.Add(_cpl);
                                _plProd.id_calendarPriceList = _cpl.id;


                            }
                            #endregion
                            
                           #region Dato Maestro Grupo Persona Calendario
                           _plProd.byGroup = _psDto.Lista.esGrupo;

                           if (_plProd.byGroup != null && (bool)_plProd.byGroup)
                           {

                               int idRegisterGroupSource = dbProduccion
                                               .ReplicationMasterProduction
                                               .FirstOrDefault(fod => fod.idPrincipalSchemaSource == _psDto.GrupoPersonaRolLista.id
                                               && fod.nameSchema.Equals("GroupPersonByRol")
                                               )?.idPrincipalSchemaDestination ?? 0;

                               var _GrupoPersonaTmp = dbProduccion
                                                       .GroupPersonByRol
                                                       .FirstOrDefault(fod => fod.id == idRegisterGroupSource);

                               if (_GrupoPersonaTmp != null)
                               {
                                   _plProd.id_groupPersonByRol = _GrupoPersonaTmp.id;
                               }
                               else
                               {
                                   var _RolTmp = dbProduccion
                                                   .Rol
                                                   .FirstOrDefault(fod => fod.name.Trim().Equals(_psDto.GrupoPersonaRolLista.nombreRol.Trim()));

                                   _gpr = new DestinoListaProduccion.GroupPersonByRol();

                                   _gpr.name = _psDto.GrupoPersonaRolLista.nombre;
                                   _gpr.description = _psDto.GrupoPersonaRolLista.descripcion;
                                   _gpr.id_company = 2;
                                   _gpr.id_rol = _RolTmp.id;
                                   _gpr.isActive = _psDto.GrupoPersonaRolLista.activo;
                                   _gpr.id_userCreate = 1;
                                   _gpr.dateCreate = DateTime.Now;
                                   _gpr.id_userUpdate = 1;
                                   _gpr.dateUpdate = DateTime.Now;


                                   if (_psDto.lstGrupoPersonaRolDetalle != null && _psDto.lstGrupoPersonaRolDetalle.Count() > 0)
                                   {
                                       _gpr.GroupPersonByRolDetail = new List<DestinoListaProduccion.GroupPersonByRolDetail>();
                                       foreach (var det in _psDto.lstGrupoPersonaRolDetalle)
                                       {
                                           _gpr.GroupPersonByRolDetail.Add(new DestinoListaProduccion.GroupPersonByRolDetail
                                           {
                                               id_person = dbProduccion.Person.FirstOrDefault(fod => fod.identification_number.Trim().Equals(det.identificacionPersona.Trim())).id
                                           });
                                       }
                                   }
                                   dbProduccion.GroupPersonByRol.Add(_gpr);
                                   dbProduccion.Entry(_gpr).State = EntityState.Added;
                                   _plProd.id_groupPersonByRol = _gpr.id;
                               }
                           }

                           #endregion
                           
                            _plProd.id_company = 2;
                            _plProd.id_processtype = dbProduccion.ProcessType.FirstOrDefault(fod => fod.code.Trim().Equals(_psDto.Lista.codigoTipoProceso))?.id;

                            _plProd.id_certification = dbProduccion.Certification.FirstOrDefault(fod => fod.code.Trim().Equals(_psDto.Lista.codCertificacion))?.id;

                            #region Detalles
                            var lstItSizeDest = _plProd.PriceListItemSizeDetail.ToList();
                            if (_psDto.Detalle != null && _psDto.Detalle.Count() > 0)
                            {
                                var lstItemSize = dbProduccion.ItemSize.ToList();

                                foreach (var det in _psDto.Detalle)
                                {
                                    DestinoListaProduccion.PriceListItemSizeDetail _plisDetail = lstItSizeDest.FirstOrDefault(fod => fod.ItemSize.code.Trim().Equals(det.codigoTalla) &&
                                                                                                                                     fod.ClassShrimp.code.Trim().Equals(det.codigoClaseCamaron) &&
                                                                                                                                     fod.Class.code.Trim().Equals(det.codigoCalidad));

                                    if (_plisDetail != null)
                                    {
                                        _plisDetail.price = det.valor;
                                        _plisDetail.commission = det.comision;
                                        dbProduccion.PriceListItemSizeDetail.Attach(_plisDetail);
                                        dbProduccion.Entry(_plisDetail).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        DestinoListaProduccion.PriceListItemSizeDetail _plis = new DestinoListaProduccion.PriceListItemSizeDetail
                                        {
                                            Id = det.id,
                                            Id_Itemsize = lstItemSize.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoTalla)).id,
                                            price = det.valor,
                                            commission = det.comision,
                                            id_classShrimp = dbProduccion.ClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoClaseCamaron)).id,
                                            id_class = dbProduccion.Class.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoCalidad)).id,
                                        };
                                    _plProd.PriceListItemSizeDetail.Add(_plis);
                                        dbProduccion.PriceListItemSizeDetail.Attach(_plis);
                                        dbProduccion.Entry(_plis).State = EntityState.Added;
                                    }
                                }
                            }

                            //Elimino tallas que no están en la base de destino
                            var lstCodigoTalla = _psDto.Detalle.Select(s => s.codigoTalla).ToList() ?? new List<string>();

                            var lstDelete = lstItSizeDest.Where(w => !lstCodigoTalla.Contains(w.ItemSize.code)).ToList();
                            if (lstDelete != null && lstDelete.Count() > 0)
                            {
                                foreach (var det in lstDelete)
                                {
                                    DestinoListaProduccion.PriceListItemSizeDetail _tallaEliminar = lstItSizeDest.FirstOrDefault(fod => fod.ItemSize != null && fod.ItemSize.code.Equals(det.ItemSize.code));
                                    //_plProd.PriceListItemSizeDetail.Remove(_tallaEliminar);
                                    //_plProd.PriceListItemSizeDetail.Add(_tallaEliminar);
                                    dbProduccion.PriceListItemSizeDetail.Attach(_tallaEliminar);
                                    dbProduccion.Entry(_tallaEliminar).State = EntityState.Deleted;
                                }
                            }
                            #endregion
                            
                            #region Penalización
                            var lstPenDest = _plProd.PriceListClassShrimp.ToList();
                            if (_psDto.DetallePenalizacion != null && _psDto.DetallePenalizacion.Count() > 0)
                            {
                                var lstClassShrimp = dbProduccion.ClassShrimp.ToList();
                                foreach (var det in _psDto.DetallePenalizacion)
                                {
                                    DestinoListaProduccion.PriceListClassShrimp _plcsDeta = lstPenDest.FirstOrDefault(fod => fod.ClassShrimp.code.Trim().Equals(det.codigoClaseCamaron));

                                    if (_plcsDeta != null)
                                    {
                                        _plcsDeta.value = det.valorClaseCamaron;
                                        dbProduccion.PriceListClassShrimp.Attach(_plcsDeta);
                                        dbProduccion.Entry(_plcsDeta).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        DestinoListaProduccion.PriceListClassShrimp _plcs = new DestinoListaProduccion.PriceListClassShrimp
                                        {
                                            id = det.idClaseCamaron,
                                            id_classShrimp = lstClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoClaseCamaron)).id,
                                            value = det.valorClaseCamaron
                                        };
                                        _plProd.PriceListClassShrimp.Add(_plcs);
                                        dbProduccion.PriceListClassShrimp.Attach(_plcs);
                                        dbProduccion.Entry(_plcs).State = EntityState.Added;
                                    }
                                }
                            }
                            //Elimino tallas que no están en la base de destino
                            var lstCodigoPen = _psDto.DetallePenalizacion.Select(s => s.codigoClaseCamaron).ToList() ?? new List<string>();

                            var lstDelete2 = lstPenDest.Where(w => !lstCodigoPen.Contains(w.ClassShrimp.code.Trim())).ToList();
                            if (lstDelete2 != null && lstDelete2.Count() > 0)
                            {
                                foreach (var det in lstDelete2)
                                {
                                    DestinoListaProduccion.PriceListClassShrimp _penEliminar = lstPenDest.FirstOrDefault(fod => fod.ClassShrimp.code.Trim().Equals(det.ClassShrimp.code));
                                    //_plProd.PriceListClassShrimp.Remove(_penEliminar);
                                    //_plProd.PriceListClassShrimp.Add(_penEliminar);
                                    dbProduccion.PriceListClassShrimp.Attach(_penEliminar);
                                    dbProduccion.Entry(_penEliminar).State = EntityState.Deleted;
                                }
                            }
                            #endregion

                            
                            #region Personas
                            var lstPersonPriceList = _plProd.PriceListPersonPersonRol.ToList();
                            if (_psDto.lstPersonaDetalle != null && _psDto.lstPersonaDetalle.Count() > 0)
                            {
                                var idsPersonPriceList = lstPersonPriceList
                                    .Select(e => e.id_Person)
                                    .ToList();

                                

                                var lstRol = dbProduccion.Rol.ToList();
                                foreach (var det in _psDto.lstPersonaDetalle)
                                {
                                    DestinoListaProduccion.PriceListPersonPersonRol _plpprDet = lstPersonPriceList.FirstOrDefault(fod => fod.Person.identification_number.Trim().Equals(det.identificacionPersona));

                                    if (_plpprDet != null)
                                    {
                                        _plpprDet.id_Rol = lstRol.FirstOrDefault(fod => fod.name.Trim().Equals(det.nombreRol)).id;
                                    }
                                    else
                                    {
                                        var lstPerson = dbProduccion.Person.FirstOrDefault(fod => fod.identification_number.Trim().Equals(det.identificacionPersona));
                                        DestinoListaProduccion.PriceListPersonPersonRol plppr = new DestinoListaProduccion.PriceListPersonPersonRol
                                        {
                                            id_Person = lstPerson.id,
                                            id_Rol = lstRol.FirstOrDefault(fod => fod.name.Trim().Equals(det.nombreRol)).id
                                        };
                                        _plProd.PriceListPersonPersonRol.Add(plppr);
                                        dbProduccion.PriceListPersonPersonRol.Attach(plppr);
                                        dbProduccion.Entry(plppr).State = EntityState.Added;
                                    }
                                }
                            }
                            
                            var lstCodigoPers = _psDto.lstPersonaDetalle.Select(s => s.identificacionPersona).ToList() ?? new List<string>();

                            var lstDelete3 = lstPersonPriceList.Where(w => !lstCodigoPers.Contains(w.Person.identification_number)).ToList();
                            if (lstDelete3 != null && lstDelete3.Count() > 0)
                            {
                                foreach (var det in lstDelete3)
                                {
                                    DestinoListaProduccion.PriceListPersonPersonRol _perEliminar = lstPersonPriceList.FirstOrDefault(fod => fod.Person !=null && fod.Person.identification_number.Trim().Equals(det.Person.identification_number));
                                    //_plProd.PriceListPersonPersonRol.Remove(_perEliminar);
                                    if (_perEliminar != null)
                                    {
                                        dbProduccion.PriceListPersonPersonRol.Attach(_perEliminar);
                                        dbProduccion.Entry(_perEliminar).State = EntityState.Deleted;
                                    }
                                }
                            }
                            
                            #endregion

                        }
                        else
                        {
                            isNew = true;
                            _plProd = new DestinoListaProduccion.PriceList();
                            _plProd.name = _psDto.Lista.nombreLista;
                            _plProd.startDate = _psDto.Lista.fechaInicio;
                            _plProd.endDate = (DateTime)_psDto.Lista.fechaFin;
                            _plProd.isForPurchase = _psDto.Lista.esCompra;
                            _plProd.isForSold = _psDto.Lista.esVenta;
                            _plProd.isQuotation = _psDto.Lista.esCotizacion;

                            #region Información Documento
                            DestinoListaProduccion.Document _docTmp = new DestinoListaProduccion.Document
                            {
                                number = _psDto.ListaDocumento.numero,
                                sequential = _psDto.ListaDocumento.sequencial,
                                emissionDate = _psDto.ListaDocumento.fechaEmision,
                                id_emissionPoint = _psDto.ListaDocumento.idPuntoEmision,
                                id_documentType = dbProduccion.DocumentType.FirstOrDefault(fod => fod.code.Trim().Equals(_psDto.ListaDocumento.codigoTipoDocumento.Trim())).id,
                                id_documentState = dbProduccion.DocumentState.FirstOrDefault(fod => fod.name.Trim().Equals(_psDto.ListaDocumento.nombreEstadoDocumento.Trim())).id,
                                id_userCreate = 1,
                                dateCreate = DateTime.Now,
                                id_userUpdate = 1,
                                dateUpdate = DateTime.Now
                            };
                            dbProduccion.Document.Add(_docTmp);
                            _plProd.Document = _docTmp;
                            #endregion

                            #region Dato Maestro Calendario
                            //DatoMaestro Calendario
                            int idRegisterCalendarSource = dbProduccion
                                                .ReplicationMasterProduction
                                                .FirstOrDefault(fod => fod.idPrincipalSchemaSource == _psDto.CalendarioLista.id
                                                && fod.nameSchema.Equals("CalendarPriceList")
                                                )?.idPrincipalSchemaDestination ?? 0;

                            var _CalendarioTmp = dbProduccion
                                                    .CalendarPriceList
                                                    .FirstOrDefault(fod => fod.id == idRegisterCalendarSource);

                            if (_CalendarioTmp != null)
                            {
                                _plProd.id_calendarPriceList = _CalendarioTmp.id;
                            }
                            else
                            {
                                var _TipoCalendarioTmp = dbProduccion
                                                        .CalendarPriceListType
                                                        .FirstOrDefault(fod => fod.name.Trim().Equals(_psDto.TipoCalendarioLista.nombre.Trim()));

                                _cpl = new DestinoListaProduccion.CalendarPriceList
                                {
                                    name = _psDto.CalendarioLista.nombre,
                                    id_calendarPriceListType = _TipoCalendarioTmp.id,
                                    startDate = _psDto.CalendarioLista.fechaInicio,
                                    endDate = _psDto.CalendarioLista.fechaFin,
                                    id_company = 2,
                                    isActive = _psDto.CalendarioLista.activo,
                                    id_userCreate = 1,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = 1,
                                    dateUpdate = DateTime.Now
                                };

                                dbProduccion.CalendarPriceList.Add(_cpl);
                                _plProd.id_calendarPriceList = _cpl.id;
                            }
                            #endregion

                            #region Dato Maestro Grupo Persona Calendario
                            _plProd.byGroup = _psDto.Lista.esGrupo;

                            if (_plProd.byGroup != null && (bool)_plProd.byGroup)
                            {

                                int idRegisterGroupSource = dbProduccion
                                                .ReplicationMasterProduction
                                                .FirstOrDefault(fod => fod.idPrincipalSchemaSource == _psDto.GrupoPersonaRolLista.id
                                                && fod.nameSchema.Equals("GroupPersonByRol")
                                                )?.idPrincipalSchemaDestination ?? 0;

                                var _GrupoPersonaTmp = dbProduccion
                                                        .GroupPersonByRol
                                                        .FirstOrDefault(fod => fod.id == idRegisterGroupSource);

                                if (_GrupoPersonaTmp != null)
                                {
                                    _plProd.id_groupPersonByRol = _GrupoPersonaTmp.id;
                                }
                                else
                                {
                                    var _RolTmp = dbProduccion
                                                    .Rol
                                                    .FirstOrDefault(fod => fod.name.Trim().Equals(_psDto.GrupoPersonaRolLista.nombreRol.Trim()));

                                    _gpr = new DestinoListaProduccion.GroupPersonByRol();

                                    _gpr.name = _psDto.GrupoPersonaRolLista.nombre;
                                    _gpr.description = _psDto.GrupoPersonaRolLista.descripcion;
                                    _gpr.id_company = 2;
                                    _gpr.id_rol = _RolTmp.id;
                                    _gpr.isActive = _psDto.GrupoPersonaRolLista.activo;
                                    _gpr.id_userCreate = 1;
                                    _gpr.dateCreate = DateTime.Now;
                                    _gpr.id_userUpdate = 1;
                                    _gpr.dateUpdate = DateTime.Now;


                                    if (_psDto.lstGrupoPersonaRolDetalle != null && _psDto.lstGrupoPersonaRolDetalle.Count() > 0)
                                    {
                                        _gpr.GroupPersonByRolDetail = new List<DestinoListaProduccion.GroupPersonByRolDetail>();
                                        foreach (var det in _psDto.lstGrupoPersonaRolDetalle)
                                        {
                                            _gpr.GroupPersonByRolDetail.Add(new DestinoListaProduccion.GroupPersonByRolDetail
                                            {
                                                id_person = dbProduccion.Person.FirstOrDefault(fod => fod.identification_number.Trim().Equals(det.identificacionPersona.Trim())).id
                                            });
                                        }
                                    }
                                    dbProduccion.GroupPersonByRol.Add(_gpr);
                                    dbProduccion.GroupPersonByRol.Attach(_gpr);
                                    dbProduccion.Entry(_gpr).State = EntityState.Added;
                                    _plProd.id_groupPersonByRol = _gpr.id;


                                }
                            }

                            #endregion

                            #region Dato Maestro Lista Base
                            int idRegisterSourceBase = dbProduccion
                                    .RegisterInfReplicationSource
                                    .FirstOrDefault(fod => fod.idRegisterSource == _psDto.Lista.idListaBase
                                    && fod.schemaRegister.Equals("PriceList"))?.idRegisterDestination ?? 0;

                            _plProd.id_priceListBase = dbProduccion.PriceList.FirstOrDefault(fod => fod.id == idRegisterSourceBase)?.id ?? null;

                            #endregion
                         
                            _plProd.id_company = 2;
                            _plProd.id_processtype = dbProduccion.ProcessType.FirstOrDefault(fod => fod.code.Trim().Equals(_psDto.Lista.codigoTipoProceso))?.id;

                            _plProd.id_certification = dbProduccion.Certification.FirstOrDefault(fod => fod.code.Trim().Equals(_psDto.Lista.codCertificacion))?.id;

                            #region Detalles
                            if (_psDto.Detalle != null && _psDto.Detalle.Count() > 0)
                            {
                                try
                                {
                                    _plProd.PriceListItemSizeDetail = new List<DestinoListaProduccion.PriceListItemSizeDetail>();
                                    var lstItemSize = dbProduccion.ItemSize.ToList();
                                    foreach (var det in _psDto.Detalle)
                                    {
                                        var aClassShrimp = dbProduccion.ClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoClaseCamaron));
                                        if (aClassShrimp == null)
                                        {
                                            var aClassShrimpOrigin = dbLp.ClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoClaseCamaron));
                                            aClassShrimp = new DestinoListaProduccion.ClassShrimp();
                                            aClassShrimp = AddClassShrimp(aClassShrimpOrigin, aClassShrimp, dbProduccion);
                                            repeat = true;
                                            continue;
                                        }
                                        var aClass = dbProduccion.Class.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoCalidad));
                                        if (aClass == null)
                                        {
                                            var aClassOrigin = dbLp.Class.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoCalidad));
                                            aClass = new DestinoListaProduccion.Class();
                                            aClass = AddClass(aClassOrigin, aClass, dbProduccion);
                                            repeat = true;
                                            continue;
                                        }
                                        var aItemSize = lstItemSize.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoTalla));
                                        if (aItemSize == null) {
                                            var aItemSizeOrigin = dbLp.ItemSize.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoTalla));
                                            aItemSize = new DestinoListaProduccion.ItemSize();
                                            aItemSize = AddItemsize(aItemSizeOrigin, aItemSize, dbProduccion);
                                            repeat = true;
                                            continue;
                                        }
                                        DestinoListaProduccion.PriceListItemSizeDetail _plis = new DestinoListaProduccion.PriceListItemSizeDetail
                                        {
                                            Id = det.id,
                                            Id_Itemsize = aItemSize.id,
                                            price = det.valor,
                                            commission = det.comision,
                                            id_classShrimp = aClassShrimp.id,
                                            id_class = aClass.id
                                        };
                                        _plProd.PriceListItemSizeDetail.Add(_plis);
                                        dbProduccion.PriceListItemSizeDetail.Attach(_plis);
                                        dbProduccion.Entry(_plis).State = EntityState.Added;
                                    }
                                }
                                catch (Exception e)
                                {

                                    throw e;
                                }
                               
                            }
                            #endregion

                            #region Penalización
                            if (_psDto.DetallePenalizacion != null && _psDto.DetallePenalizacion.Count() > 0)
                            {
                                _plProd.PriceListClassShrimp = new List<DestinoListaProduccion.PriceListClassShrimp>();
                                var lstClassShrimp = dbProduccion.ClassShrimp.ToList();
                                foreach (var det in _psDto.DetallePenalizacion)
                                {

                                    DestinoListaProduccion.PriceListClassShrimp _plcs = new DestinoListaProduccion.PriceListClassShrimp
                                    {
                                        id = det.idClaseCamaron,
                                        id_classShrimp = lstClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals(det.codigoClaseCamaron)).id,
                                        value = det.valorClaseCamaron
                                    };
                                    _plProd.PriceListClassShrimp.Add(_plcs);
                                    dbProduccion.PriceListClassShrimp.Attach(_plcs);
                                    dbProduccion.Entry(_plcs).State = EntityState.Added;
                                }
                            }
                            #endregion

                            #region Personas
                            if (_psDto.lstPersonaDetalle != null && _psDto.lstPersonaDetalle.Count() > 0)
                            {
                                _plProd.PriceListPersonPersonRol = new List<DestinoListaProduccion.PriceListPersonPersonRol>();
                                var lstRol = dbProduccion.Rol.ToList();
                                foreach (var det in _psDto.lstPersonaDetalle)
                                {
                                    var lstPerson = dbProduccion.Person.FirstOrDefault(fod => fod.identification_number.Trim().Equals(det.identificacionPersona));
                                    DestinoListaProduccion.PriceListPersonPersonRol _plppr = new DestinoListaProduccion.PriceListPersonPersonRol
                                    {
                                        id_Person = lstPerson.id,
                                        id_Rol = lstRol.FirstOrDefault(fod => fod.name.Trim().Equals(det.nombreRol)).id
                                    };
                                    _plProd.PriceListPersonPersonRol.Add(_plppr);
                                    dbProduccion.PriceListPersonPersonRol.Attach(_plppr);
                                    dbProduccion.Entry(_plppr).State = EntityState.Added;
                                }
                            }
                            #endregion

                        }

                        if (isNew)
                        {
                            dbProduccion.PriceList.Attach(_plProd);
                            dbProduccion.Entry(_plProd).State = EntityState.Added;
                        }
                        else
                        {
                            dbProduccion.PriceList.Attach(_plProd);
                            dbProduccion.Entry(_plProd).State = EntityState.Modified;
                        }

                        dbProduccion.SaveChanges();

                        if (isNew)
                        {
                            #region Información REplicación

                            DestinoListaProduccion.RegisterInfReplicationSource _rirs = new RegisterInfReplicationSource
                            {
                                idRegisterDestination = _plProd.id,
                                idRegisterSource = _psDto.Lista.idLista,
                                schemaRegister = "PriceList",
                                idGeneral = _psDto.Lista.idUsuarioResponsable,
                                nameGeneral = _psDto.Lista.nombreUsuarioResponsable,
                                idUserCreateSource = _psDto.Lista.idUsuarioCreacion,
                                nameUserCreateSource = _psDto.Lista.nombreUsuarioCreacion,
                                dateCreateSource = _psDto.Lista.dtUsuarioCreacion,
                                idUserUpdateSource = _psDto.Lista.idUsuarioModificacion,
                                nameUserUpdateSource = _psDto.Lista.nombreUsuarioModificacion,
                                dateUpdateSource = _psDto.Lista.dtUsuarioModificacion
                            };
                            dbProduccion.RegisterInfReplicationSource.Add(_rirs);
                            #endregion

                            #region Información Complementaria

                            if (_psDto.CambioEstadoDocumento != null && _psDto.CambioEstadoDocumento.Count() > 0)
                            {
                                foreach (var det in _psDto.CambioEstadoDocumento)
                                {
                                    dbProduccion.DocumentStateChangeReplicationSource.Add(new DocumentStateChangeReplicationSource
                                    {
                                        id_documentSource = _plProd.id,
                                        id_documentStateOldSource = det.idEstadoDocumentOldSource,
                                        codeDocumentStateOldSource = det.codigoEstadoDocumentoOldSource,
                                        nameDocumentStateOldSource = det.nombreEstadoDocumentoOldSource,
                                        id_documentStateNewSource = det.idEstadoDocumentNewSource,
                                        codeDocumentStateNewSource = det.codigoEstadoDocumentoNewSource,
                                        nameDocumentStateNewSource = det.nombreEstadoDocumentoNewSource,
                                        id_userSource = det.idUsuarioSource,
                                        nameUserSource = det.nombreUsuarioSource,
                                        id_userGroupSource = det.idUsuarioGrupoSource,
                                        nameUserGroupSource = det.nombreUsuarioGrupoSource,
                                        changeTimeSource = det.changeTimeSource
                                    });
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Información Replicación
                            DestinoListaProduccion.RegisterInfReplicationSource _rirs = dbProduccion.RegisterInfReplicationSource
                                                                                        .FirstOrDefault(fod => fod.idRegisterDestination == _plProd.id
                                                                                        && fod.idRegisterSource == _psDto.Lista.idLista
                                                                                        && fod.schemaRegister.Equals("PriceList"));

                            _rirs.idGeneral = _psDto.Lista.idUsuarioResponsable;
                            _rirs.nameGeneral = _psDto.Lista.nombreUsuarioResponsable;
                            _rirs.idUserCreateSource = _psDto.Lista.idUsuarioCreacion;
                            _rirs.nameUserCreateSource = _psDto.Lista.nombreUsuarioCreacion;
                            _rirs.dateCreateSource = _psDto.Lista.dtUsuarioCreacion;
                            _rirs.idUserUpdateSource = _psDto.Lista.idUsuarioModificacion;
                            _rirs.nameUserUpdateSource = _psDto.Lista.nombreUsuarioModificacion;
                            _rirs.dateUpdateSource = _psDto.Lista.dtUsuarioModificacion;

                            #endregion

                            #region Información Complementaria

                            var lstDocState = dbProduccion
                                                    .DocumentStateChangeReplicationSource
                                                    .Where(w => w.id_documentSource == _plProd.id).ToList();

                            foreach (var det in lstDocState)
                            {
                                DocumentStateChangeReplicationSource dscrs = dbProduccion.DocumentStateChangeReplicationSource.FirstOrDefault(fod => fod.id_documentSource == det.id_documentSource);

                                dbProduccion.DocumentStateChangeReplicationSource.Remove(dscrs);
                                dbProduccion.Entry(dscrs).State = EntityState.Deleted;
                            }

                            if (_psDto.CambioEstadoDocumento != null && _psDto.CambioEstadoDocumento.Count() > 0)
                            {
                                foreach (var det in _psDto.CambioEstadoDocumento)
                                {
                                    dbProduccion.DocumentStateChangeReplicationSource.Add(new DocumentStateChangeReplicationSource
                                    {
                                        id_documentSource = _plProd.id,
                                        id_documentStateOldSource = det.idEstadoDocumentOldSource,
                                        codeDocumentStateOldSource = det.codigoEstadoDocumentoOldSource,
                                        nameDocumentStateOldSource = det.nombreEstadoDocumentoOldSource,
                                        id_documentStateNewSource = det.idEstadoDocumentNewSource,
                                        codeDocumentStateNewSource = det.codigoEstadoDocumentoNewSource,
                                        nameDocumentStateNewSource = det.nombreEstadoDocumentoNewSource,
                                        id_userSource = det.idUsuarioSource,
                                        nameUserSource = det.nombreUsuarioSource,
                                        id_userGroupSource = det.idUsuarioGrupoSource,
                                        nameUserGroupSource = det.nombreUsuarioGrupoSource,
                                        changeTimeSource = det.changeTimeSource
                                    });
                                }
                            }
                            #endregion
                        }

                        #region Inserta Información sobre Calendario Registrado
                        if (_cpl != null)
                        {
                            DestinoListaProduccion.ReplicationMasterProduction repMP1 = new ReplicationMasterProduction();
                            repMP1.idPrincipalSchemaDestination = _cpl.id;
                            repMP1.idPrincipalSchemaSource = _psDto.CalendarioLista.id;
                            repMP1.nameSchema = "CalendarPriceList";
                            repMP1.dateAction = DateTime.Now;
                            dbProduccion.ReplicationMasterProduction.Add(repMP1);
                        }
                        #endregion

                        #region Inserta Información sobre Grupos 

                        if (_gpr != null)
                        {
                            DestinoListaProduccion.ReplicationMasterProduction repMP2 = new ReplicationMasterProduction();
                            repMP2.idPrincipalSchemaDestination = _gpr.id;
                            repMP2.idPrincipalSchemaSource = _psDto.GrupoPersonaRolLista.id;
                            repMP2.nameSchema = "GroupPersonByRol";
                            repMP2.dateAction = DateTime.Now;
                            dbProduccion.ReplicationMasterProduction.Add(repMP2);
                        }
                        
                        #endregion

                        dbProduccion.SaveChanges();

                        //trans.Commit();
                        if (repeat)
                        {
                            _resultado = 2;
                        }
                        else {
                            _resultado = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MetodosEscrituraLogs.EscribeExcepcionLog(ex, ruta, "InsertDataDestinationReplicationPriceList", "PROD");
                        _resultado = -1;
                        //trans.Rollback();
                        
                    }
                }

            }

            return _resultado;
        }

        private DestinoListaProduccion.ClassShrimp AddClassShrimp(OrigenListaLP.ClassShrimp aClassShrimpOrigin, DestinoListaProduccion.ClassShrimp aClassShrimp, DBContextoListaProduccion dbProduccion)
        {
            aClassShrimp.code = aClassShrimpOrigin.code;
            aClassShrimp.name = aClassShrimpOrigin.name;
            aClassShrimp.description = aClassShrimpOrigin.description;
            aClassShrimp.isActive = aClassShrimpOrigin.isActive;
            aClassShrimp.order = aClassShrimpOrigin.order;
            dbProduccion.ClassShrimp.Attach(aClassShrimp);
            dbProduccion.Entry(aClassShrimp).State = EntityState.Added;
            //dbProduccion.SaveChanges();
            return aClassShrimp;
        }

        private DestinoListaProduccion.Class AddClass(OrigenListaLP.Class aClassOrigin, DestinoListaProduccion.Class aClass, DBContextoListaProduccion dbProduccion)
        {
            aClass.code = aClassOrigin.code;
            aClass.description = aClassOrigin.description;
            aClass.id_company = aClassOrigin.id_company;
            aClass.isActive = aClassOrigin.isActive;
            aClass.id_userCreate = aClassOrigin.id_userCreate;
            aClass.dateCreate = aClassOrigin.dateCreate;
            aClass.id_userUpdate = aClassOrigin.id_userUpdate;
            aClass.dateUpdate = aClassOrigin.dateUpdate;
            dbProduccion.Class.Attach(aClass);
            dbProduccion.Entry(aClass).State = EntityState.Added;
            //dbProduccion.SaveChanges();
            return aClass;
        }

        private DestinoListaProduccion.ItemSize AddItemsize(OrigenListaLP.ItemSize aItemSizeOrigin, DestinoListaProduccion.ItemSize aItemSize, DBContextoListaProduccion dbProduccion)
        {
            aItemSize.code = aItemSizeOrigin.code;
            aItemSize.name = aItemSizeOrigin.name;
            aItemSize.description = aItemSizeOrigin.description;
            aItemSize.id_company = aItemSizeOrigin.id_company;
            aItemSize.isActive = aItemSizeOrigin.isActive;
            aItemSize.id_userCreate = aItemSizeOrigin.id_userCreate;
            aItemSize.dateCreate = aItemSizeOrigin.dateCreate;
            aItemSize.id_userUpdate = aItemSizeOrigin.id_userUpdate;
            aItemSize.dateUpdate = aItemSizeOrigin.dateUpdate;
            dbProduccion.ItemSize.Attach(aItemSize);
            dbProduccion.Entry(aItemSize).State = EntityState.Added;
            //dbProduccion.SaveChanges();
            return aItemSize;
        }
    }
}
