using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using Dapper;
using DevExpress.XtraSpreadsheet.Model;
using DocumentFormat.OpenXml.Office2010.Excel;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Operations;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderFishingCustodian
    {
        const string SELECT_FISHINGCUSTODIAN = "SELECT id, code,patrol,semiComplete,truckDriver,changeHG,cabinHR,  " +
                                            " id_FishingSite ,isActive,id_company,id_userCreate,dateCreate,id_userUpdate,  " +
                                            " dateUpdate " +
                                            " FROM FishingCustodian ";

        const int EsActivo = 1;

        private static T[] execute<T>(string sentence)
        {
            T[] toReturn = null;
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

        private static DBContext db1 = null;

        public static IEnumerable FishingCustodianbyCompany(int? id_company)
        {
            //db = new DBContext();
            if (!id_company.HasValue) return Array.Empty<FishingCustodian>();
            return execute<FishingCustodian>($"{SELECT_FISHINGCUSTODIAN} WHERE id_company ={id_company} && isActive = {EsActivo}");
            //.Where(t => t.isActive && t.id_company == id_company).ToList();
        }

        public static FishingCustodian FishingCustodianById(int id)
        {
            //db = new DBContext();

            return execute<FishingCustodian>($"{SELECT_FISHINGCUSTODIAN} WHERE id = {id}").FirstOrDefault();
            //db.FishingCustodian.FirstOrDefault(i => i.id == id);
        }

        public static FishingCustodian FishingCustodianById(int? id)
        {
            //db = new DBContext();

            if (!id.HasValue) return new FishingCustodian
            {
              patrol = 0,
              cabinHR = 0,
              changeHG = 0,
              semiComplete = 0,
              truckDriver = 0,
              isActive = true
            };

            db1 = new DBContext();
            var fishingCustodian = execute<FishingCustodian>($"{SELECT_FISHINGCUSTODIAN} WHERE id = {id}").FirstOrDefault();
            var fishingSite = db1.FishingSite.FirstOrDefault(r => r.id == fishingCustodian.id_FishingSite);
            fishingCustodian.FishingSite = fishingSite;
            return fishingCustodian;
            //db.FishingCustodian.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable FishingCustodianByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db1 = new DBContext();
            string sentence_predicate = "";
            if (id_company.HasValue)
            {
                sentence_predicate = $" WHERE (id_company = {id_company.Value} AND isActive = {EsActivo} ) ";
            }
            if (id_current.HasValue)
            {
                sentence_predicate += string.IsNullOrEmpty(sentence_predicate) ? " WHERE " : " OR ";
                sentence_predicate += $" ( id = {id_current}) ";
            }
            var toRerturn = execute<FishingCustodian>($"{SELECT_FISHINGCUSTODIAN}{sentence_predicate}");
            int[] fishingsiteIds = toRerturn
                                          .Select(r => r.id_FishingSite)
                                          .ToArray();
            var fishingSites = db1.FishingSite.Include("FishingZone").Where(r => fishingsiteIds.Contains(r.id)).ToArray();
            

            return (from custodian in toRerturn
                    join site in fishingSites
                    on custodian.id_FishingSite equals site.id
                    select new
                    { 
                        id= custodian.id,
                        name= $"{site.FishingZone.name} | {site.name}" 
                    }
                    );
        }

        public static FishingCustodianValueDto[] FishingCustodianValuesCodes(int fishingCustodiaId)
        {
            var fishingCustodian = execute<FishingCustodian>($"{SELECT_FISHINGCUSTODIAN} WHERE id = {fishingCustodiaId}").FirstOrDefault();

            var toReturn = new FishingCustodianValueDto[]
            {
                new FishingCustodianValueDto
                {
                    FishingCustodianId = fishingCustodiaId,
                    FishingSiteId = fishingCustodian.id_FishingSite,
                    CodeValue = "patrol",
                    NameValue = $"Patrulla | {fishingCustodian.patrol.ToCurrencyFormat()}"
                },
                new FishingCustodianValueDto
                {
                    FishingCustodianId = fishingCustodiaId,
                    FishingSiteId = fishingCustodian.id_FishingSite,
                    CodeValue = "semiComplete",
                    NameValue = $"Semi Completa | {fishingCustodian.semiComplete.ToCurrencyFormat()}"
                },
                new FishingCustodianValueDto
                {
                    FishingCustodianId = fishingCustodiaId,
                    FishingSiteId = fishingCustodian.id_FishingSite,
                    CodeValue = "truckDriver",
                    NameValue = $"Camioneta | {fishingCustodian.truckDriver.ToCurrencyFormat()}"
                },
                new FishingCustodianValueDto
                {
                    FishingCustodianId = fishingCustodiaId,
                    FishingSiteId = fishingCustodian.id_FishingSite,
                    CodeValue = "changeHG",
                    NameValue = $"H/Cambina Gye | {fishingCustodian.changeHG.ToCurrencyFormat()}"
                },
                new FishingCustodianValueDto
                {
                    FishingCustodianId = fishingCustodiaId,
                    FishingSiteId = fishingCustodian.id_FishingSite,
                    CodeValue = "cabinHR",
                    NameValue = $"H/Cabina Ruta | {fishingCustodian.cabinHR.ToCurrencyFormat()}"
                }
            };
             
            return toReturn;
        }
    }
}