using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderAdvanceParameters
    {
        private static DBContext db = null;

        public static AdvanceParameters AdvanceParameter(int id)
        {
            db = new DBContext();
            return db.AdvanceParameters.Where(w => w.id == id).FirstOrDefault();
        }

        public static AdvanceParametersDetail AdvanceParametersDetailByid(int? id)
        {
            db = new DBContext();
            var model = db.AdvanceParametersDetail.Where(t => t.id == id).FirstOrDefault();



            return model;
        }

        public static IEnumerable< AdvanceParametersDetail> AdvanceParametersDetailByCode(String code)
        {
            db = new DBContext();
            var model = db.AdvanceParametersDetail.Where(t => t.AdvanceParameters.code == code);



            return model.ToList();
        }
        public static IEnumerable AdvanceParametersByCode( String code)
        {
            db = new DBContext();
            var model = db.AdvanceParametersDetail.Where(t => t.AdvanceParameters.code == code).Select(p => new { p.id, name = p.description }).ToList();
                      

            return model;
        }
        public static AdvanceParameters AdvanceParametersById(int? id_AdvanceParameters)
        {
            db = new DBContext(); ;
            return db.AdvanceParameters.FirstOrDefault(v => v.id == id_AdvanceParameters);
        }

        public static IEnumerable AdvanceParametersNormalPeriodByCode(String code)
        {
            db = new DBContext();
            var codigosPeriodo = new[] { "C", "A", "P" };

            var model = db.AdvanceParametersDetail
                .Where(t => t.AdvanceParameters.code == code
                    && codigosPeriodo.Contains(t.valueCode.Trim())
                 )
                .Select(p => new { p.id, name = p.description })
                .ToList();

            return model;
        }
    }
}