using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using DevExpress.Utils.Extensions;
using DocumentFormat.OpenXml.Office2010.Excel;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Extensions;
using DXPANACEASOFT.Models;



namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTransportSize
    {

        private static string SENTENCE_SUBJECT = "SELECT id,code,name,description,id_poundsRange,id_iceBagRange,isActive, " +
                                                  "id_company,id_userCreate,dateCreate,id_userUpdate,dateUpdate, " +
                                                  "id_transportTariffType, binRangeMinimum,binRangeMaximun FROM TransportSize ";

        private static T[] execute<T>(string sentence)
        {
            T[] toReturn =  null;
            using (var db = DapperConnection.Connection())
            {
                db.Open();
                try
                {
                    toReturn = db.Query<T>(sentence).ToArray();
                }
                finally
                {
                    db.Close();
                }

            }
            return toReturn;
        }
        
        public static IEnumerable TransportSizebyCompany(IDbConnection db,int? id_company)
        {
            //db = new DBContext();
            if (!id_company.HasValue) return Array.Empty<TransportSize>();
            return execute<TransportSize>($"{SENTENCE_SUBJECT} WHERE id_company = {id_company}");
            //db.TransportSize.Where(t => t.isActive && t.id_company == id_company).ToList();
        }

        public static TransportSize TrasportSizeById(int id)
        {
            //db = new DBContext();
            //return db.TransportSize.FirstOrDefault(i => i.id == id);
            return execute<TransportSize>($"{SENTENCE_SUBJECT} WHERE id = {id}").FirstOrDefault();
        }
        public static TransportSize TrasportSizeById(int? id)
        {
            //db = new DBContext();
            //return db.TransportSize.FirstOrDefault(i => i.id == id);
            if (!id.HasValue) return new TransportSize();
            return execute<TransportSize>($"{SENTENCE_SUBJECT} WHERE id = {id}").FirstOrDefault();
        }
        public static TransportSizeDto TrasportSizeByIdDto(int? id)
        {
            if (!id.HasValue) return new TransportSizeDto();
            //var transporSize = db.TransportSize.FirstOrDefault(i => i.id == id);
            var transporSize = execute<TransportSize>($"{SENTENCE_SUBJECT} WHERE id = {id}").FirstOrDefault();
            if (transporSize == null) return new TransportSizeDto();
            DBContext dbc = new DBContext();
            return transporSize.toDTO(dbc);
        }

        public static IEnumerable TransportSizeSingletonTransportTariff(int? id_company)
        {
            if (!id_company.HasValue) return Array.Empty<TransportSize>();
            //db = new DBContext();
            //return db.TransportSize.Where(t => t.isActive && t.id_company == id_company).ToList();
            return execute<TransportSize>($"{SENTENCE_SUBJECT} WHERE isActive = 1 AND id_company = {id_company}");
        }

        //

        public static IEnumerable Transports()
        {
            var aTransports = new List<SelectListItem>();
            aTransports.Add(new SelectListItem
            {
                Value = "Pre-Paid",
                Text = "Pre-Paid",
                //Selected = true
            });
            aTransports.Add(new SelectListItem
            {
                Value = "Collect",
                Text = "Collect",
                //Selected = true
            });
            aTransports.Add(new SelectListItem
            {
                Value = string.Empty,
                Text = string.Empty,
                //Selected = true
            });
            return aTransports;
        }

        
    }
}