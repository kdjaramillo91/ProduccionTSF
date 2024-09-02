using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models.PO;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderFanSeriesGrammage
    {
        private static DBContext db = null;

        public static decimal FanSeriesGrammageGetValue(int id_grammage, decimal Libras, int? id_processtype, int? id_ListPrice, int? id_Item)
        {
            db = new DBContext();
            decimal LibrasCola = 0;
            decimal LibrasEntero = 0;
            decimal? resultEntero = 0;
            decimal? resultCola = 0;



            decimal result = 0;
            //valida Lista de Precio y Gramaje
            if (id_ListPrice != null && id_ListPrice > 0 && id_grammage > 0)
            {
                //Busca el tipo de Proceso
                if (id_processtype == null || id_processtype <= 0)
                {
                    if (id_Item != null || id_Item > 0)
                    {
                        id_processtype = db.ItemProcessType
                                            .Where(x => x.Id_Item == id_Item)
                                            .FirstOrDefault()?.Id_ProcessType;
                    }
                }
                //int idClassShrimpB = db.ClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals("B"))?.id ?? 0;

                //decimal valB = db.PriceListClassShrimp
                //                  .FirstOrDefault(fod => fod.id_priceList == id_ListPrice
                //                  && fod.id_classShrimp == idClassShrimpB).value;
                ///valida  el Proceso
                if (id_processtype != null && id_processtype > 0)
                {
                    ProcessType processtype = db.ProcessType.Where(x => x.id == id_processtype).FirstOrDefault();

                    //Verifica el Proceso
                    if (processtype.code == "ENT" || processtype.code == "COL")
                    {

                        var ListFanSeriesGrammage = db.FanSeriesGrammage.Where(i => i.id_grammage == id_grammage && i.id_processtype == id_processtype).ToList();
                        var ListFanSeriesGrammageENC = db.FanSeriesGrammage.Where(i => i.id_grammage == id_grammage && i.ProcessType.code == "ENC").ToList();

                        if (ListFanSeriesGrammage != null && ListFanSeriesGrammage.ToList().Count > 0 && Libras > 0)
                        {
                            var setting = db.Setting.Where(X => X.code == "PRENTC").FirstOrDefault()?.value;

                            if (setting != null)
                            {
                                LibrasEntero = Libras * (decimal.Parse(setting) / 100);
                                LibrasCola = Libras - LibrasEntero;
                            }
                            else
                            {
                                LibrasEntero = Libras;
                            }

                            var list = (from e in ListFanSeriesGrammage
                                        where e.percentage > 0
                                        select new { valueLibras = (Decimal)e.percentage * LibrasEntero, valueLibrasCola = (Decimal)e.percentage * LibrasCola, e.id_processtype, e.id_itemsize, e.id_class }).ToList();

                            List<PriceListDet> lispre = db.PriceListItemSizeDetail
                                                            .Where(w => w.Id_PriceList == id_ListPrice && w.ClassShrimp.code == "D0")
                                                            .Select(s => new PriceListDet
                                                            {
                                                                Id_Itemsize = s.Id_Itemsize,
                                                                PriceA = s.price,
                                                                //PriceB = s.price - valB
                                                                Price = s.price - s.commission,
                                                                idClass = s.id_class,
                                                                codeClass = s.Class.code,
                                                                idClassShrimp = s.id_classShrimp,
                                                                codeClassShrimp = s.ClassShrimp.code
                                                            }).ToList();

                            //if (lispre != null && lispre.Count > 0)
                            //{
                            //    int lisBase = db.PriceList
                            //                    .FirstOrDefault(fod => fod.id == id_ListPrice)?
                            //                    .id_priceListBase ?? 0;

                            //    if (lisBase > 0)
                            //    {
                            //        var lisPreRef = (from e in db.PriceListItemSizeDetail.Where(X => X.Id_PriceList == lisBase)
                            //                         select new { e.Id_Itemsize, precioA = e.price, precioB = (e.price - valB) }
                            //                        ).ToList();

                            //        foreach (var i in lispre)
                            //        {
                            //            if (i.PriceA == 0)
                            //            {
                            //                i.PriceA = lisPreRef
                            //                            .FirstOrDefault(fod => fod.Id_Itemsize == i.Id_Itemsize)?
                            //                            .precioA ?? 0;
                            //                i.PriceB = lisPreRef
                            //                            .FirstOrDefault(fod => fod.Id_Itemsize == i.Id_Itemsize)?
                            //                            .precioB ?? 0;
                            //            }
                            //        }
                            //    }

                                if (processtype.code == "ENT")
                                {
                                    var lisresultEntero = (from liseries in list
                                                           join j in lispre on liseries.id_itemsize equals j.Id_Itemsize
                                                           where liseries.id_class == j.idClass
                                                           select new
                                                           {
                                                               valor = (decimal)liseries.valueLibras *
                                                          Math.Round((decimal)
                                                              (processtype.code == "ENT" ? (j.Price > 0 ? (j.Price.Value / (decimal)2.2046) : 0) : j.Price), 3),
                                                               Price = Math.Round((decimal)(processtype.code == "ENT" ? (j.Price > 0 ? (j.Price.Value / (decimal)2.2046) : 0) : j.Price), 3),
                                                               valueLibras = (decimal)liseries.valueLibras
                                                           }).ToList();

                                    resultEntero = (from liseries in lisresultEntero
                                                    select liseries.valor).Sum();

                                }
                                else
                                {



                                    //var listClasC = (from cl in db.Class
                                    //                 select new { cl.code, cl.id }).ToList();

                                    var lisresultColaPro = (from liseries in list
                                                            join j in lispre on liseries.id_itemsize equals j.Id_Itemsize
                                                           where liseries.id_class == j.idClass
                                                            //join d in listClasC on liseries.id_class equals d.id
                                                            select new
                                                            {
                                                                valor = Math.Round((liseries.valueLibras * (Math.Round((decimal)j.Price, 3))), 3),
                                                                //valor = Math.Round((liseries.valueLibras * ((d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3))), 3),
                                                                Price = (Math.Round((decimal)j.Price, 3)),
                                                                //Price = (d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3),
                                                                valueLibrasCola = Math.Round((liseries.valueLibras), 3),
                                                                liseries.id_itemsize,
                                                                liseries.id_processtype

                                                            }).ToList();

                                    resultEntero = (from liseries in lisresultColaPro

                                                    select new { valor = Math.Round((liseries.valueLibrasCola * Math.Round((decimal)liseries.Price, 3)), 3) }).ToList().Sum(x => x.valor);




                                }


                                //si el proceso es entero
                                if (processtype.code == "ENT")
                                {

                                    var listENC = (from e in ListFanSeriesGrammageENC
                                                   where e.percentage > 0
                                                   select new { valueLibrasCola = (Decimal)e.percentage * LibrasCola, e.id_processtype, e.id_itemsize, e.id_class }).ToList();


                                    //var listClas = (from cl in db.Class
                                    //                select new { cl.code, cl.id }).ToList();

                                    var lisresultCola = (from liseries in listENC
                                                         join j in lispre on liseries.id_itemsize equals j.Id_Itemsize
                                                           where liseries.id_class == j.idClass
                                                         //join d in listClas on liseries.id_class equals d.id
                                                         select new
                                                         {
                                                             valor = Math.Round((liseries.valueLibrasCola * (Math.Round((decimal)j.Price, 3))), 3),
                                                             //valor = Math.Round((liseries.valueLibrasCola * ((d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3))), 3),
                                                             Price = (Math.Round((decimal)j.Price, 3)),
                                                             //Price = (d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3),
                                                             valueLibrasCola = Math.Round((liseries.valueLibrasCola), 3)

                                                         }).ToList();


                                    resultCola = (from liseries in lisresultCola

                                                  select new { valor = Math.Round((liseries.valueLibrasCola * Math.Round((decimal)liseries.Price, 3)), 3) }).ToList().Sum(x => x.valor);


                                    //resultCola = (from liseries in listENC
                                    //              join j in lispre on liseries.id_itemsize equals j.Id_Itemsize
                                    //                      select new { valor = Math.Round((liseries.valueLibrasCola * Math.Round((decimal)j.PriceB,3)),3) }).ToList().Sum(x => x.valor);
                                }
                                result = (resultEntero ?? 0) + (resultCola ?? 0);
                            }

                        }
                    }


                }
            //}


            return result;




        }

        public static decimal FanSeriesGrammageGetValueFixed(int id_grammage
            , decimal Libras, int? id_processtype
            , int? id_ListPrice, int? id_Item
            , int? idProvider)
        {
            db = new DBContext();
            decimal LibrasCola = 0;
            decimal LibrasEntero = 0;
            decimal? resultEntero = 0;
            decimal? resultCola = 0;



            decimal result = 0;
            //valida Lista de Precio y Gramaje
            if (id_ListPrice != null && id_ListPrice > 0 && id_grammage > 0)
            {
                //Busco proveedor en algún grupo
                int idGroupPerson = db.GroupPersonByRolDetail
                                        .FirstOrDefault(fod => fod.id_person == idProvider)?
                                        .id_groupPersonByRol ?? 0;

                List<ItemSizePriceClass> lstPreciosClases = null;
                List<Class> lstClass = db.Class.ToList();

                int idBrok = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("BROK")).id;
                int idVetl = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("VETL")).id;

                if (idGroupPerson > 0)
                {
                    lstPreciosClases =  db.ItemSizePriceClass
                                            .Where(w => w.idGroupPersonByRol == idGroupPerson)
                                            .ToList();
                }

                if (lstPreciosClases == null || lstPreciosClases.Count == 0)
                {
                    lstPreciosClases = db.ItemSizePriceClass
                                            .Where(w => w.idGroupPersonByRol == null)
                                            .ToList();
                }

                //Busca el tipo de Proceso
                if (id_processtype == null || id_processtype <= 0)
                {
                    if (id_Item != null || id_Item > 0)
                    {
                        id_processtype = db.ItemProcessType
                                            .Where(x => x.Id_Item == id_Item)
                                            .FirstOrDefault()?
                                            .Id_ProcessType;
                    }
                }

                //int idClassShrimpB = db.ClassShrimp
                //                        .FirstOrDefault(fod => fod.code.Trim().Equals("B"))?
                //                        .id ?? 0;

                //decimal valB = db.PriceListClassShrimp
                //                  .FirstOrDefault(fod => fod.id_priceList == id_ListPrice
                //                  && fod.id_classShrimp == idClassShrimpB).value;
                ///valida  el Proceso
                if (id_processtype != null && id_processtype > 0)
                {
                    ProcessType processtype = db.ProcessType.Where(x => x.id == id_processtype).FirstOrDefault();

                    //Verifica el Proceso
                    if (processtype.code != "")
                    {
                        List<FanSeriesGrammage> ListFanSeriesGrammage = null;
                        List<FanSeriesGrammage> ListFanSeriesGrammageENC = null;

                        ListFanSeriesGrammage = db.FanSeriesGrammage
                                                    .Where(i => i.id_grammage == id_grammage
                                                                && i.id_processtype == id_processtype)
                                                    .ToList();

                        ListFanSeriesGrammageENC = db.FanSeriesGrammage
                                                    .Where(i => i.id_grammage == id_grammage
                                                                && i.ProcessType.code == "ENC")
                                                    .ToList();

                        if (processtype.code == "ENT")
                        {
                            ListFanSeriesGrammage = db.FanSeriesGrammage
                                                        .Where(w => w.id_grammage == id_grammage
                                                                    && w.id_processtype == id_processtype
                                                                    && w.ProcessType1.code == "ENT")
                                                        .ToList();

                            ListFanSeriesGrammageENC = db.FanSeriesGrammage
                                                    .Where(w => w.id_grammage == id_grammage
                                                    && w.id_processtype == id_processtype
                                                    && w.ProcessType1.code == "COL")
                                                    .ToList();
                        }
                        else
                        {
                            ListFanSeriesGrammageENC = db.FanSeriesGrammage
                                        .Where(w => w.id_grammage == id_grammage
                                                    && w.id_processtype == id_processtype)
                                        .ToList();
                        }

                        if (ListFanSeriesGrammage != null && ListFanSeriesGrammage.ToList().Count > 0 && Libras > 0)
                        {
                            var setting = db.Setting.Where(X => X.code == "PRENTC").FirstOrDefault()?.value;

                            if (processtype.code != "COL")
                            {
                                LibrasEntero = Libras * (decimal.Parse(setting) / 100);
                                LibrasCola = Libras - LibrasEntero;
                            }
                            else
                            {
                                LibrasEntero = Libras;
                            }

                            var list = (from e in ListFanSeriesGrammage
                                        where e.percentage > 0
                                        select new { valueLibras = (Decimal)e.percentage * LibrasEntero
                                        , valueLibrasCola = (Decimal)e.percentage * LibrasCola
                                        , e.id_processtype, e.id_itemsize, e.id_class })
                                        .ToList();

                            List<PriceListDet> lispre = db.PriceListItemSizeDetail
                                                            .Where(w => w.Id_PriceList == id_ListPrice && w.ClassShrimp.code == "D0")
                                                            .Select(s => new PriceListDet
                                                            {
                                                                Id_Itemsize = s.Id_Itemsize,
                                                                PriceA = s.price - (s.commission),
                                                                //PriceB = ((s.price - valB - s.commission) >= 0) ? (s.price - valB - s.commission) : 0,
                                                                Price = s.price - s.commission,
                                                                idClass = s.id_class,
                                                                codeClass = s.Class.code,
                                                                idClassShrimp = s.id_classShrimp,
                                                                codeClassShrimp = s.ClassShrimp.code
                                                            }).ToList();

                            //Actualizo Valores de Broken y Venta Local
                            //foreach (var det in lispre)
                            //{
                            //    var PrecioTalla = lstPreciosClases
                            //                        .FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize);

                            //    if (PrecioTalla != null)
                            //    {
                                    
                            //        //Actualizo Precio A y Precio B
                            //        det.PriceA = lstPreciosClases
                            //                        .FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize && fod.idClass == idBrok)?.price ?? 0;

                            //        det.PriceB = lstPreciosClases
                            //                        .FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize && fod.idClass == idVetl)?.price ?? 0;

                            //    }
                            //}


                            if (lispre != null && lispre.Count > 0)
                            {
                                int lisBase = db.PriceList
                                                .FirstOrDefault(fod => fod.id == id_ListPrice)?
                                                .id_priceListBase ?? 0;

                                if (processtype.code == "ENT")
                                {
                                    var lisresultEntero = (from liseries in list
                                                           join j in lispre on liseries.id_itemsize equals j.Id_Itemsize
                                                           where liseries.id_class == j.idClass
                                                           select new
                                                           {
                                                               valor = (decimal)liseries.valueLibras *
                                                          Math.Round((decimal)
                                                              (processtype.code == "ENT" ? (j.Price > 0 ? (j.Price.Value / (decimal)2.2046) : 0) : j.Price), 3),
                                                              //(processtype.code == "ENT" ? (j.PriceA > 0 ? (j.PriceA.Value / (decimal)2.2046) : 0) : j.PriceA), 3),
                                                               Price = Math.Round((decimal)(processtype.code == "ENT" ? (j.Price > 0 ? (j.Price.Value / (decimal)2.2046) : 0) : j.Price), 3),
                                                               //Price = Math.Round((decimal)(processtype.code == "ENT" ? (j.PriceA > 0 ? (j.PriceA.Value / (decimal)2.2046) : 0) : j.PriceA), 3),
                                                               valueLibras = (decimal)liseries.valueLibras
                                                           }).ToList();

                                    resultEntero = (from liseries in lisresultEntero
                                                    select liseries.valor).Sum();

                                }
                                else
                                {



                                    //var listClasC = (from cl in db.Class
                                    //                 select new { cl.code, cl.id }).ToList();

                                    var lisresultColaPro = (from liseries in list
                                                            join j in lispre on liseries.id_itemsize equals j.Id_Itemsize
                                                            where liseries.id_class == j.idClass
                                                            //join d in listClasC on liseries.id_class equals d.id
                                                            select new
                                                            {
                                                                valor = Math.Round((liseries.valueLibras * (Math.Round((decimal)j.Price, 3))), 3),
                                                                //valor = Math.Round((liseries.valueLibras * ((d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3))), 3),
                                                                Price = (Math.Round((decimal)j.Price, 3)),
                                                                //Price = (d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3),
                                                                valueLibrasCola = Math.Round((liseries.valueLibras), 3),
                                                                liseries.id_itemsize,
                                                                liseries.id_processtype

                                                            }).ToList();

                                    resultEntero = (from liseries in lisresultColaPro

                                                    select new { valor = Math.Round((liseries.valueLibrasCola * Math.Round((decimal)liseries.Price, 3)), 3) }).ToList().Sum(x => x.valor);




                                }


                                //si el proceso es entero
                                if (processtype.code != "COL")
                                {

                                    var listENC = (from e in ListFanSeriesGrammageENC
                                                   where e.percentage > 0
                                                   select new { valueLibrasCola = (Decimal)e.percentage * LibrasCola, e.id_processtype, e.id_itemsize, e.id_class }).ToList();


                                    //var listClas = (from cl in db.Class
                                    //                select new { cl.code, cl.id }).ToList();

                                    var lisresultCola = (from liseries in listENC
                                                         join j in lispre on liseries.id_itemsize equals j.Id_Itemsize
                                                        where liseries.id_class == j.idClass
                                                         //join d in listClas on liseries.id_class equals d.id
                                                         select new
                                                         {
                                                             valor = Math.Round((liseries.valueLibrasCola * (Math.Round((decimal)j.Price, 3))), 3),
                                                             //valor = Math.Round((liseries.valueLibrasCola * ((d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3))), 3),
                                                             Price = (Math.Round((decimal)j.Price, 3)),
                                                             //Price = (d.code == "PRIM" || d.code == "BROK") ? Math.Round((decimal)j.PriceA, 3) : Math.Round((decimal)j.PriceB, 3),
                                                             valueLibrasCola = Math.Round((liseries.valueLibrasCola), 3)

                                                         }).ToList();


                                    resultCola = (from liseries in lisresultCola

                                                  select new { valor = Math.Round((liseries.valueLibrasCola * Math.Round((decimal)liseries.Price, 3)), 3) }).ToList().Sum(x => x.valor);

                                }
                                result = (resultEntero ?? 0) + (resultCola ?? 0);
                            }

                        }
                    }


                }
            }


            return result;




        }
    }




    



   
}


