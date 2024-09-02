using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
    public class ServiceMachineProdOpening
    {
        public static void UpdateMachineProdOpeningDetailTimeEnd(DBContext db, LiquidationCartOnCart liquidationCartOnCart, bool reverse = false)
        {
            //string result = "";
           // WarehouseLocation warehouseLocation = new WarehouseLocation();
            try
            {
                var machineProdOpeningDetailaux = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id_MachineForProd == liquidationCartOnCart.id_MachineForProd && fod.id_MachineProdOpening == liquidationCartOnCart.id_MachineProdOpening);
                var timeEndMachineProdOpeningDetailaux = new DateTime(2000, 1, 1, machineProdOpeningDetailaux.timeEnd.Value.Hours, machineProdOpeningDetailaux.timeEnd.Value.Minutes, machineProdOpeningDetailaux.timeEnd.Value.Seconds);

                var time00Aux = new DateTime(2000, 1, 1, 0, 0, 0);
                var time11Aux = new DateTime(2000, 1, 1, 11, 59, 59);

                var diurnoTimeEndMachineProdOpeningDetailaux = false;

                var cumple = (DateTime.Compare(time00Aux, timeEndMachineProdOpeningDetailaux) <= 0);
                if (cumple)
                {
                    cumple = (DateTime.Compare(timeEndMachineProdOpeningDetailaux, time11Aux) <= 0);
                    if (cumple)
                    {
                        diurnoTimeEndMachineProdOpeningDetailaux = cumple;
                    }
                }

                var diurnoTimeEndliquidationCartOnCartAux = false;

                var mayorTimeEndliquidationCartOnCartAuxTimeEndMachineProdOpeningDetailaux = false;

                if (!reverse)
                {
                    var timeEndliquidationCartOnCartAux = new DateTime(2000, 1, 1, liquidationCartOnCart.timeEnd.Value.Hours, liquidationCartOnCart.timeEnd.Value.Minutes, liquidationCartOnCart.timeEnd.Value.Seconds);

                    cumple = (DateTime.Compare(time00Aux, timeEndliquidationCartOnCartAux) <= 0);
                    if (cumple)
                    {
                        cumple = (DateTime.Compare(timeEndliquidationCartOnCartAux, time11Aux) <= 0);
                        if (cumple)
                        {
                            diurnoTimeEndliquidationCartOnCartAux = cumple;
                        }
                    }

                    cumple = (DateTime.Compare(timeEndMachineProdOpeningDetailaux, timeEndliquidationCartOnCartAux) < 0);
                    mayorTimeEndliquidationCartOnCartAuxTimeEndMachineProdOpeningDetailaux = cumple;

                    cumple = ((diurnoTimeEndMachineProdOpeningDetailaux == diurnoTimeEndliquidationCartOnCartAux && mayorTimeEndliquidationCartOnCartAuxTimeEndMachineProdOpeningDetailaux) || diurnoTimeEndMachineProdOpeningDetailaux != diurnoTimeEndliquidationCartOnCartAux);

                    if (cumple)
                    {
                        machineProdOpeningDetailaux.timeEnd = liquidationCartOnCart.timeEnd;
                    }
                }
                else
                {
                    machineProdOpeningDetailaux.timeEnd = machineProdOpeningDetailaux.timeInit;
                    var liquidationCartOnCartAux = db.LiquidationCartOnCart.Where(w => w.id_MachineForProd == liquidationCartOnCart.id_MachineForProd && w.id_MachineProdOpening == liquidationCartOnCart.id_MachineProdOpening &&
                                                                                       w.id != liquidationCartOnCart.id && w.Document.DocumentState.code == "03");//APROBADA
                    foreach (var item in liquidationCartOnCartAux)
                    {
                        var timeEndliquidationCartOnCartAux = new DateTime(2000, 1, 1, item.timeEnd.Value.Hours, item.timeEnd.Value.Minutes, item.timeEnd.Value.Seconds);

                        cumple = (DateTime.Compare(time00Aux, timeEndliquidationCartOnCartAux) <= 0);
                        if (cumple)
                        {
                            cumple = (DateTime.Compare(timeEndliquidationCartOnCartAux, time11Aux) <= 0);
                            if (cumple)
                            {
                                diurnoTimeEndliquidationCartOnCartAux = cumple;
                            }
                        }

                        cumple = (DateTime.Compare(timeEndMachineProdOpeningDetailaux, timeEndliquidationCartOnCartAux) < 0);
                        mayorTimeEndliquidationCartOnCartAuxTimeEndMachineProdOpeningDetailaux = cumple;

                        cumple = ((diurnoTimeEndMachineProdOpeningDetailaux == diurnoTimeEndliquidationCartOnCartAux && mayorTimeEndliquidationCartOnCartAuxTimeEndMachineProdOpeningDetailaux) || diurnoTimeEndMachineProdOpeningDetailaux != diurnoTimeEndliquidationCartOnCartAux);

                        if (cumple)
                        {
                            machineProdOpeningDetailaux.timeEnd = item.timeEnd;
                            diurnoTimeEndMachineProdOpeningDetailaux = diurnoTimeEndliquidationCartOnCartAux;
                            diurnoTimeEndMachineProdOpeningDetailaux = diurnoTimeEndliquidationCartOnCartAux;
                            timeEndMachineProdOpeningDetailaux = timeEndliquidationCartOnCartAux;
                        }
                    }
                }
                db.MachineProdOpeningDetail.Attach(machineProdOpeningDetailaux);
                db.Entry(machineProdOpeningDetailaux).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                //result = e.Message;
                throw e;
            }

            //return warehouseLocation;
        }

    }
}