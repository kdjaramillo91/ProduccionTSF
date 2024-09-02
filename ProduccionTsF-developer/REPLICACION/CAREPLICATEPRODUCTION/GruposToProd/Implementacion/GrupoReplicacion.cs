using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrigenGrupoLP;
using DestinoGrupoProduccion;
using CAREPLICATEPRODUCTION.GruposToProd.Modelos;
using System.Data.Entity;
using Utilitarios.Logs;

namespace CAREPLICATEPRODUCTION.GruposToProd.Implementacion
{
    public class GrupoReplicacion
    {
        #region ATTRIBUTES
        private int idGroupReplicate { get; set; }

        private string idsListReplicate { get; set; }
        private DBContextoGrupoLP dbLp { get; }
        private DBContextoGrupoProduccion dbProduccion { get; }
        private string ruta { get; set; }
        #endregion
        public GrupoReplicacion(int _idLpReplicate, string _ruta, string _idsListReplicate)
        {
            this.idGroupReplicate = _idLpReplicate;
            this.idsListReplicate = _idsListReplicate;
            dbLp = new DBContextoGrupoLP();
            dbProduccion = new DBContextoGrupoProduccion();
            this.ruta = _ruta;
        }
        public void ReplicateInformation()
        {
            #region Obtiene Información Origen
            GrupoPersonaDTO _psDto = ObtieneGrupoOrigen(this.idGroupReplicate);
            #endregion

            #region Valida y Carga
            ValidaCargaDatos(_psDto);
            #endregion
        }
        private GrupoPersonaDTO ObtieneGrupoOrigen(int _idGroupPerson)
        {
            GrupoPersonaDTO psDTO = null;
            var _group = dbLp
                            .GroupPersonByRol
                            .FirstOrDefault(fod => fod.id == _idGroupPerson);

            if (_group != null)
            {
                psDTO = new GrupoPersonaDTO();
                #region Recupero Grupo

                psDTO.grupoPersona = new GrupoPersonaRol
                {
                    id = _group.id,
                    nombre = _group.name,
                    descripcion = _group.description,
                    idCompania = _group.id_company,
                    idRol = _group.id_rol,
                    nombreRol = _group.Rol.name,
                    activo = _group.isActive
                };

                #endregion

                #region Recupero Detalle

                psDTO.grupoPersonaDetalle = _group
                                            .GroupPersonByRolDetail
                                            .Select(s => new GrupoPersonaRolDetalle
                                            {
                                                id = s.id,
                                                idPersona = s.id_person,
                                                identificacionPersona = s.Person.identification_number
                                            }).ToList();

                #endregion
            }
            return psDTO;
        }
        private int ValidaCargaDatos(GrupoPersonaDTO _psDto)
        {
            int _resultado = 0;
            bool isNew = false;

            #region Variables
            #endregion

            if (_psDto != null)
            {
                //using (DbContextTransaction trans = dbProduccion.Database.BeginTransaction())
                {
                    try
                    {
                        //Busco Lista en ReplicationMasterProduction
                        int idRegisterSource = dbProduccion
                                                .ReplicationMasterProduction
                                                .FirstOrDefault(fod => fod.idPrincipalSchemaSource == _psDto.grupoPersona.id
                                                && fod.nameSchema.Equals("GroupPersonByRol"))?.idPrincipalSchemaDestination ?? 0;

                        DestinoGrupoProduccion.GroupPersonByRol _groupProd = dbProduccion
                                                                                .GroupPersonByRol
                                                                                .FirstOrDefault(fod => fod.name == _psDto.grupoPersona.nombre);
                        if (_groupProd != null)
                        {
                            _groupProd.description = _psDto.grupoPersona.descripcion;
                            _groupProd.id_company = _psDto.grupoPersona.idCompania;
                            _groupProd.id_rol = dbProduccion.Rol.FirstOrDefault(fod => fod.name.Equals(_psDto.grupoPersona.nombreRol)).id;
                            _groupProd.isActive = _psDto.grupoPersona.activo;
                            _groupProd.id_userUpdate = 1;
                            _groupProd.dateUpdate = DateTime.Now;

                            #region Detalles
                            var lstPersonDestin = _groupProd.GroupPersonByRolDetail.ToList();
                            if (_psDto.grupoPersonaDetalle != null && _psDto.grupoPersonaDetalle.Count() > 0)
                            {
                                var lstPerson = dbProduccion.Person.Where(w => w.identification_number != null).ToList();

                                foreach (var det in _psDto.grupoPersonaDetalle)
                                {
                                    DestinoGrupoProduccion.GroupPersonByRolDetail _detail = lstPersonDestin
                                                                                                .FirstOrDefault(fod => fod.Person.identification_number.Trim().Equals(det.identificacionPersona));

                                    if (_detail == null)
                                    {
                                        DestinoGrupoProduccion.GroupPersonByRolDetail _plis = new DestinoGrupoProduccion.GroupPersonByRolDetail
                                        {
                                            id = det.id,
                                            id_person = lstPerson.FirstOrDefault(fod => fod.identification_number.Trim().Equals(det.identificacionPersona)).id,
                                        };
                                        _groupProd.GroupPersonByRolDetail.Add(_plis);
                                        dbProduccion.GroupPersonByRolDetail.Attach(_plis);
                                        dbProduccion.Entry(_plis).State = EntityState.Added;
                                    }
                                }
                            }

                            //Elimino personas que no están en la base de destino
                            var lstIdPersona = _psDto.grupoPersonaDetalle
                                                        .Select(s => s.identificacionPersona)
                                                        .ToList() ?? new List<string>();

                            var lstDelete = lstPersonDestin
                                            .Where(w => !lstIdPersona.Contains(w.Person.identification_number))
                                            .ToList();

                            if (lstDelete != null && lstDelete.Count() > 0)
                            {
                                foreach (var det in lstDelete)
                                {
                                    DestinoGrupoProduccion.GroupPersonByRolDetail _personaEliminar = lstPersonDestin
                                                                                                        .FirstOrDefault(fod => fod.Person.identification_number.Trim().Equals(det.Person.identification_number));
                                    
                                    dbProduccion.GroupPersonByRolDetail.Attach(_personaEliminar);
                                    dbProduccion.Entry(_personaEliminar).State = EntityState.Deleted;
                                }
                            }
                            #endregion

                        }
                        else
                        {
                            isNew = true;
                            _groupProd = new DestinoGrupoProduccion.GroupPersonByRol();
                            _groupProd.name = _psDto.grupoPersona.nombre;
                            _groupProd.description = _psDto.grupoPersona.descripcion;
                            _groupProd.id_company = _psDto.grupoPersona.idCompania;
                            _groupProd.id_rol = dbProduccion.Rol.FirstOrDefault(fod => fod.name.Equals(_psDto.grupoPersona.nombreRol)).id;
                            _groupProd.isActive = _psDto.grupoPersona.activo;
                            _groupProd.id_userCreate = 1;
                            _groupProd.dateCreate = DateTime.Now;
                            _groupProd.id_userUpdate = 1;
                            _groupProd.dateUpdate = DateTime.Now;

                            #region Detalles
                            if (_psDto.grupoPersonaDetalle != null && _psDto.grupoPersonaDetalle.Count() > 0)
                            {
                                _groupProd.GroupPersonByRolDetail = new List<DestinoGrupoProduccion.GroupPersonByRolDetail>();
                                var lstPerson = dbProduccion.Person.Where(w => w.identification_number != null).ToList();
                                foreach (var det in _psDto.grupoPersonaDetalle)
                                {
                                    DestinoGrupoProduccion.GroupPersonByRolDetail _plis = new DestinoGrupoProduccion.GroupPersonByRolDetail
                                    {
                                        id = det.id,
                                        id_person = lstPerson.FirstOrDefault(fod => fod.identification_number.Equals(det.identificacionPersona)).id
                                    };

                                    _groupProd.GroupPersonByRolDetail.Add(_plis);
                                    dbProduccion.GroupPersonByRolDetail.Attach(_plis);
                                    dbProduccion.Entry(_plis).State = EntityState.Added;
                                }
                            }
                            #endregion
                        }

                        if (isNew)
                        {
                            dbProduccion.GroupPersonByRol.Attach(_groupProd);
                            dbProduccion.Entry(_groupProd).State = EntityState.Added;
                        }
                        else
                        {
                            dbProduccion.GroupPersonByRol.Attach(_groupProd);
                            dbProduccion.Entry(_groupProd).State = EntityState.Modified;
                        }

                        dbProduccion.SaveChanges();

                        #region Inserta Información sobre Grupos 

                        if (idRegisterSource == 0)
                        {
                            DestinoGrupoProduccion.ReplicationMasterProduction repMP2 = new ReplicationMasterProduction();
                            repMP2.idPrincipalSchemaDestination = _groupProd.id;
                            repMP2.idPrincipalSchemaSource = _psDto.grupoPersona.id;
                            repMP2.nameSchema = "GroupPersonByRol";
                            repMP2.dateAction = DateTime.Now;
                            dbProduccion.ReplicationMasterProduction.Add(repMP2);
                        }
                        
                        #endregion

                        dbProduccion.SaveChanges();

                        //trans.Commit();
                        _resultado = 1;
                    }
                    catch (Exception ex)
                    {
                        MetodosEscrituraLogs.EscribeExcepcionLog(ex, ruta, "InsertDataDestinationReplicationGroup", "PROD");
                        _resultado = -1;
                        //trans.Rollback();
                    }
                }

            }

            return _resultado;
        }
    }
}
