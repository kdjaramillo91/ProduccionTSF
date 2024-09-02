using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Web.ASPxScheduler.Forms.Internal;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.BackgroundProcessManagement;
using DXPANACEASOFT.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderBackgroundProcessManagement
    {
        public static string ProcessState(string code)
        {
            var state = new DBContext()
                               .BackgroundProcess
                               .FirstOrDefault(fod => fod.code == code)?.state ?? string.Empty;
            return state;                  
        }


        public static IEnumerable<ComboboxGeneral> PeriodsFixed()
        {
            var db = new DBContext();
            string codigoParametrSaldoInicial = "PSALINI";
            string codigoTipoPeriodoMes = "M";
            string descripcionTipoPeriodoMes = "Periodo: Mensual";



            List<ComboboxGeneral> periodos = new List<ComboboxGeneral>();

            
            var settingSaldoInicial = db.Setting.FirstOrDefault(r => r.code == codigoParametrSaldoInicial);
            if (settingSaldoInicial == null)
            {
                throw new System.Exception($"No se ha definido el parámetro: {codigoParametrSaldoInicial}");
            }

            int anio = 0;
            int mes = 0;
            int periodoInicio = 0;

            int anioFin = 0;
            int mesFin = 0;
            int periodoFin = 0;


            int.TryParse(settingSaldoInicial.value.Substring(0, 4), out anio);
            int.TryParse(settingSaldoInicial.value.Substring(4, 2), out mes);

            if (anio == 0 || mes == 0)
            {
                throw new System.Exception($"Valores incorrectos en el parámetro: {codigoParametrSaldoInicial}");
            }

            

            DateTime lastAnioMes = DateTime.Now.AddMonths(-1);
            anioFin = lastAnioMes.Year;
            mesFin = lastAnioMes.Month;

            int.TryParse($"{anio}{mes.ToString().PadLeft(2,'0')}", out periodoInicio);
            int.TryParse($"{anioFin}{mesFin.ToString().PadLeft(2, '0')}", out periodoFin);

            for (int i = periodoInicio; i<= periodoFin; i++)
            {

                int currentAnio  = int.Parse( i.ToString().Substring(0,4));
                int currentMes = int.Parse(i.ToString().Substring(4, 2));

                periodos.Add(new ComboboxGeneral
                {
                    code = $"{currentAnio.ToString()}-{currentMes.ToString().PadLeft(2, '0')}-{codigoTipoPeriodoMes}-{descripcionTipoPeriodoMes}",
                    description = $"{descripcionTipoPeriodoMes}: {currentAnio.ToString()}-{GlobalUtils.ConvertToMonthName(currentMes)}"
                });

                if (currentMes == 12)
                {
                    i = int.Parse( $"{(currentAnio + 1).ToString()}00" );
                }

            }

            return periodos;

        }

        public static IEnumerable<ComboboxGeneral> Periods(string[] codeType, int idWarehouse)
		{
            var db = new DBContext();
            
            List<ComboboxGeneral> periodos = new List<ComboboxGeneral>();

             
            var parametrosTipo = db.AdvanceParametersDetail
                                                    .Where(r => r.AdvanceParameters.code == "TPGV1" && codeType.Contains(r.valueCode))
                                                    .ToArray();

            var lsPeriods_initial = db
                                        .InventoryPeriodDetail
                                        .Include("InventoryPeriod")
                                        .Where(r => r.InventoryPeriod.id_warehouse == idWarehouse)
                                        .Select(s => new
                                        {
                                            year = s.InventoryPeriod.year,
                                            period = s.periodNumber,
                                            tipo = s.InventoryPeriod.id_PeriodType
                                        })
                                        .Distinct()
                                        .ToList();

            var preList = (from detail in lsPeriods_initial
                           join tipos in parametrosTipo
                           on detail.tipo equals tipos.id
                           select new
                           {
                               year = detail.year,
                               period = detail.period,
                               tipoPeriodoCodigo = tipos.valueCode.Trim(),
                               tipoPeriodoDescrip = tipos.description.Trim(),
                               fechaPeridoTexto = (tipos.valueCode.Trim() == "M") ? $"{detail.year}-{GlobalUtils.ConvertToMonthName(detail.period)}" : GlobalUtils.DateFromDayYear(detail.year, detail.period).ToString("yyyy-MM-dd"),
                           }).ToArray();

            periodos = preList.OrderByDescending(o => o.year).ThenByDescending(o => o.tipoPeriodoCodigo).ThenBy(o => o.period).Select(s => new ComboboxGeneral
            {
                code = $"{s.year.ToString()}-{s.period.ToString()}-{s.tipoPeriodoCodigo}-{s.tipoPeriodoDescrip}",
                description = $"{s.tipoPeriodoDescrip}: {s.fechaPeridoTexto}",
            }).ToList();
            

            return periodos;

        }


		
        public static IEnumerable<ComboboxGeneral>Warehouses(int? idCompany) 
        {
            return idCompany.HasValue
                ? new DBContext()
                    .Warehouse
                    .Where(w => w.id_company == idCompany && w.isActive == true)
                    .Select(s => new ComboboxGeneral
                    {
                        code = s.id.ToString(),
                        description = (s.description??s.name),
                    }).ToArray()
                : new ComboboxGeneral[] { };
        }
        public static IEnumerable<ComboboxGeneral> Items(int? idCompany)
        {
            return idCompany.HasValue
                ? new DBContext()
                    .Item
                    .Where(w => w.id_company == idCompany && w.isActive == true)
                    .Select(s => new ComboboxGeneral
                    {
                        code = s.id.ToString(),
                        description = s.masterCode + " " + s.name,
                    }).ToArray()
                : new ComboboxGeneral[] { };
        }
    }
}