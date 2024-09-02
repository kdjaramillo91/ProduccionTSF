using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using DXPANACEASOFT.Models.ModelExtension;

namespace DXPANACEASOFT.Auxiliares
{
    public static class SecurityControl
    {

        // se envia tipo de documento y la clase 
        // Se llenan los valores con las propiedades
        public static T SetSecurityControlDocument<T>( object o,string codeDocumentType, string codeDocumentState,int idUser) 
        {

            DBContext db = new DBContext();

            List<tbsysDocumentDocumentStateControlsState> securityControl = db.tbsysDocumentDocumentStateControlsState
                                                                                    .Where(r => r.DocumentType.code == codeDocumentType 
                                                                                                && r.DocumentState.code == codeDocumentState).ToList();


            List<tbsysDocumentDocumentStateControlsStateUser> securityControlUser = db.tbsysDocumentDocumentStateControlsStateUser
                                                                                        .Where(r => r.DocumentType.code == codeDocumentType
                                                                                                && r.DocumentState.code == codeDocumentState
                                                                                                && r.id_user == idUser).ToList();

            IList<PropertyInfo> propiedades = o.GetType().GetProperties().ToList();

            if (securityControlUser != null)
            {
                
                securityControlUser.ForEach(r =>
                {

                    tbsysDocumentDocumentStateControlsState _controlMod =  securityControl
                                                                                .First(s => s.controlName == r.controlName);
                    if(_controlMod != null)
                    {
                        _controlMod.isReadOnly = r.isReadOnly;
                        _controlMod.isRequired = r.isRequired;
                    }

                });

            }



            foreach(tbsysDocumentDocumentStateControlsState controlState in securityControl)
            {
                string nameCSI = controlState.controlName + "_CSI";
                string namePropertyReadOnly = controlState.controlName + "_ReadOnly";
                string namePropertyRequired = controlState.controlName + "_Required";
                // ReadOnly
                PropertyInfo _propertyCSI = propiedades.First(r => r.Name == nameCSI);
                if (_propertyCSI != null)
                {
                    SecurityControlInfo _securityControlInfo = new SecurityControlInfo
                    {
                         isReadOnly = controlState.isReadOnly,
                         isRequired = controlState.isRequired
                    };

                    _propertyCSI.SetValue(o, _securityControlInfo);
                }

                // Requerid
                //PropertyInfo _propertyRequerid = propiedades.First(r => r.Name == namePropertyRequired);
                //if (_propertyRequerid != null)
                //{
                //    _propertyRequerid.SetValue(o, controlState.isRequired);
                //}

            }

            return (T)o;
        }
    }
}