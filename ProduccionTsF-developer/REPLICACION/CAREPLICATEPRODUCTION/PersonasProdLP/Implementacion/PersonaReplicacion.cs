using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrigenPersonaProduccion;
using DestinoPersonaLP;
using CAREPLICATEPRODUCTION.PersonasProdLP.Modelos;
using System.Data.Entity;
using Utilitarios.Logs;

namespace CAREPLICATEPRODUCTION.PersonasProdLP.Implementacion
{
    public class PersonaReplicacion
    {
        #region ATTRIBUTES
        private int idPersonReplicate { get; set; }

        private string idsPersonReplicate { get; set; }
        private DBContextoPersonaLP dbLp { get; }
        private DBContextoPersonaProduccion dbProduccion { get; }

        private string ruta { get; set; }
        #endregion

        public PersonaReplicacion(int _idPersonReplicate, string _ruta, string _idsPersonReplicate)
        {
            this.idPersonReplicate = _idPersonReplicate;
            this.idsPersonReplicate = _idsPersonReplicate;
            dbLp = new DBContextoPersonaLP();
            dbProduccion = new DBContextoPersonaProduccion();
            this.ruta = _ruta;
        }

        public void ReplicateInformation()
        {
            #region Obtiene Información Origen
            PersonSourceDTO _psDto = ObtienePersonaOrigen(this.idPersonReplicate);
            #endregion

            #region Valida y Carga
            ValidaCargaDatos(_psDto);
            #endregion
        }
        private PersonSourceDTO ObtienePersonaOrigen(int _idPerson)
        {
            PersonSourceDTO psDTO = new PersonSourceDTO();
            var _person = dbProduccion.Person.FirstOrDefault(fod => fod.id == _idPerson);

            psDTO.persona = new ProducionPersona
                                {
                                    id = _person.id,
                                    idTipoPersona = _person.id_personType,
                                    idTipoIdentificacion = _person.id_identificationType,
                                    numeroIdentificacion = _person.identification_number,
                                    nombreCompleto = _person.fullname_businessName,
                                    foto = _person.photo,
                                    direccion = _person.address,
                                    email = _person.email,
                                    idCompania = _person.id_company,
                                    activo = _person.isActive,
                                    idUsuarioCreacion = _person.id_userCreate,
                                    fechaCreacion = _person.dateCreate,
                                    idUsuarioModificacion = _person.id_userUpdate,
                                    fechaModificacion = _person.dateUpdate,
                                    codigoCI = _person.codeCI,
                                    bCC = _person.bCC,
                                    nombreComercial = _person.tradeName,
                                    numeroCelular = _person.cellPhoneNumberPerson,
                                    emailCC = _person.emailCC
                                };

            psDTO.tipoIdenficacion = dbProduccion.IdentificationType
                                        .Where(w => w.id == _person.id_identificationType)
                                        .Select(s => new TipoIdentificacion
                                        {
                                            id = s.id,
                                            codigo = s.code,
                                            nombre = s.name,
                                            codigoSRI  = s.codeSRI,
                                            descripcion = s.description,
                                            activo  = s.is_Active,
                                            idCompania = s.id_company
                                        }).FirstOrDefault();


            if (_person.Department != null)
            {
                psDTO.departamento = dbProduccion.Department
                                                    .Where(w => w.id == _person.Department.id)
                                                        .Select(s => new Departamento
                                                        {
                                                            id = s.id,
                                                            codigo = s.code,
                                                            nombre = s.name,
                                                            descripcion = s.description,
                                                            activo = s.isActive,
                                                            idCompania = s.id_company
                                                        }).FirstOrDefault();
            }

            psDTO.tipoPersona = dbProduccion
                                    .PersonType
                                    .Where(w => w.id == _person.id_personType)
                                    .Select(s => new TipoPersona
                                    {
                                        id = s.id,
                                        codigo = s.code,
                                        nombre = s.name,
                                        descripcion = s.description,
                                        activo = s.isActive,
                                        idCompania = s.id_company
                                    }).FirstOrDefault();

            psDTO.lstRol = _person.Rol
                                    .Select(s => new RolP
                                    {
                                        id = s.id,
                                        nombre = s.name,
                                        descripcion = s.description,
                                        idCompania = s.id_company,
                                        activo = s.isActive
                                    }).ToList();

            return psDTO;
        }
        private int ValidaCargaDatos(PersonSourceDTO _psDto)
        {
            int _resultado = 0;
            bool isNew = false;

            if (_psDto != null)
            {
                //using (DbContextTransaction trans = dbLp.Database.BeginTransaction())
                {
                    try
                    {
                        //Busco identificación de persona
                        DestinoPersonaLP.Person _perLP = dbLp.Person.FirstOrDefault(fod => fod.identification_number.Trim().Equals(_psDto.persona.numeroIdentificacion));
                        if (_perLP != null)
                        {
                            //Mapeo datos persona
                            _perLP.id_personType = _psDto.persona.idTipoPersona;
                            _perLP.id_identificationType = _psDto.persona.idTipoIdentificacion;
                            _perLP.fullname_businessName = _psDto.persona.nombreCompleto;
                            _perLP.photo = _psDto.persona.foto;
                            _perLP.address = _psDto.persona.direccion;
                            _perLP.email = _psDto.persona.email;
                            _perLP.isActive = _psDto.persona.activo;
                            _perLP.id_userUpdate = 1;
                            _perLP.dateUpdate = DateTime.Now;
                            _perLP.codeCI = _psDto.persona.codigoCI;
                            _perLP.bCC = _psDto.persona.bCC;
                            _perLP.tradeName = _psDto.persona.nombreComercial;
                            _perLP.cellPhoneNumberPerson = _psDto.persona.numeroCelular;

                            var _perRol = _perLP.Rol.ToList();
                            var lstRolLP = dbLp.Rol.ToList();

                            //Agrego los roles que no tiene
                            var lstRolProd = _psDto.lstRol.ToList();
                            if (lstRolProd != null && lstRolProd.Count() > 0)
                            {
                                foreach (var det in lstRolProd)
                                {
                                    var _rlp = _perRol.FirstOrDefault(fod => fod.name.Trim().Equals(det.nombre));
                                    if (_rlp == null)
                                    {
                                        DestinoPersonaLP.Rol _rolAddLp = lstRolLP.FirstOrDefault(fod => fod.name.Trim().Equals(det.nombre));
                                        if (_rolAddLp != null)
                                        {
                                            _perLP.Rol.Add(_rolAddLp);
                                            if (det.nombre.Equals("Empleado"))
                                            {
                                                DestinoPersonaLP.Department _d = dbLp.Department.FirstOrDefault(fod => fod.code.Trim().Equals(_psDto.departamento.nombre));
                                                _perLP.Department = _d;
                                            }
                                        }
                                    }
                                }
                            }

                            //Elimino roles de la persona de LP que no están en Producción.
                            var lstNameRolProd = lstRolProd.Select(s => s.nombre).ToList();
                            var lstDelete = _perRol.Where(w => !lstNameRolProd.Contains(w.name)).ToList();
                            if (lstDelete != null && lstDelete.Count() > 0)
                            {
                                foreach (var det in lstDelete)
                                {
                                    DestinoPersonaLP.Rol _rolDelete = lstRolLP.FirstOrDefault(fod => fod.name.Trim().Equals(det.name));
                                    _perLP.Rol.Remove(_rolDelete);
                                }
                            }
                        }
                        else
                        {
                            isNew = true;
                            _perLP = new DestinoPersonaLP.Person();
                            //Mapeo datos persona
                            _perLP.id_personType = _psDto.persona.idTipoPersona;
                            _perLP.id_identificationType = _psDto.persona.idTipoIdentificacion;
                            _perLP.identification_number = _psDto.persona.numeroIdentificacion;
                            _perLP.fullname_businessName = _psDto.persona.nombreCompleto;
                            _perLP.photo = _psDto.persona.foto;
                            _perLP.address = _psDto.persona.direccion;
                            _perLP.email = _psDto.persona.email;
                            _perLP.id_company = _psDto.persona.idCompania;
                            _perLP.isActive = _psDto.persona.activo;
                            _perLP.id_userCreate = 1;
                            _perLP.dateCreate = DateTime.Now;
                            _perLP.id_userUpdate = 1;
                            _perLP.dateUpdate = DateTime.Now;
                            _perLP.codeCI = _psDto.persona.codigoCI;
                            _perLP.bCC = _psDto.persona.bCC;
                            _perLP.tradeName = _psDto.persona.nombreComercial;
                            _perLP.cellPhoneNumberPerson = _psDto.persona.numeroCelular;

                            //AgregoRoles
                            var lstRol = _psDto.lstRol.ToList();
                            var lstRolLP = dbLp.Rol.ToList();
                            if (lstRol != null && lstRol.Count() > 0)
                            {
                                foreach (var det in lstRol)
                                {
                                    DestinoPersonaLP.Rol _r = lstRolLP.FirstOrDefault(fod => fod.name.Trim().Equals(det.nombre));
                                    _perLP.Rol.Add(_r);
                                    if (det.nombre.Equals("Empleado"))
                                    {
                                        DestinoPersonaLP.Department _d = dbLp.Department.FirstOrDefault(fod => fod.code.Trim().Equals(_psDto.departamento.codigo));
                                        _perLP.Department = _d;
                                    }
                                }
                            }
                        }
                        if (isNew)
                        {
                            dbLp.Person.Attach(_perLP);
                            dbLp.Entry(_perLP).State = EntityState.Added;
                        }
                        else
                        {
                            dbLp.Person.Attach(_perLP);
                            dbLp.Entry(_perLP).State = EntityState.Modified;
                        }
                        dbLp.SaveChanges();
                        //trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        MetodosEscrituraLogs.EscribeExcepcionLog(ex, ruta, "InsertDataDestinationReplication", "PROD");
                        //trans.Rollback();
                    }
                }
                
            }

            return _resultado;
        }

    }
}
