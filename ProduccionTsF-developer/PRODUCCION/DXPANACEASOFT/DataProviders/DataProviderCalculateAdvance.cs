using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models.AdvanceProviderDTO;
using Utilitarios.Logs;
using System.Configuration;
using DXPANACEASOFT.Models.PO;


namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCalculateAdvance
    {
        private static DBContext db = null;


        public static List<AdvanceProviderPLDetail> CalculateAdvanceProvider(AdvanceProviderPLGeneralParameters apPLgp)
        {
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            List<AdvanceProviderPLDetail> lstAPPLdetail = new List<AdvanceProviderPLDetail>();
            db = new DBContext();
            try
            {
                decimal poundsHead = 0;
                decimal poundsTail = 0;

                decimal percentage = 100;

                if (apPLgp.codeProcessType == "ENT")
                {
                    poundsHead = Math.Round(((apPLgp.performanceLot * apPLgp.totalPoundsLot) / percentage), 3);
                    poundsTail = Math.Round((((apPLgp.totalPoundsLot - poundsHead) * apPLgp.percentageTailPerformanceUsed) / percentage), 3);
                }
                else if (apPLgp.codeProcessType == "ENC")
                {
                    poundsTail = Math.Round(((apPLgp.totalPoundsLot) * (apPLgp.percentageTailPerformanceUsed) / percentage), 3);
                }
                else if (apPLgp.codeProcessType == "COL")
                {
                    poundsTail = apPLgp.totalPoundsLot;
                }
                if (apPLgp.codeProcessType == "ENC")
                {
                    var i = db.ProcessType.FirstOrDefault(fod => fod.code == apPLgp.codeProcessType).id;
                    apPLgp.codeProcessType = "COL";
                    apPLgp.id_processType = i;
                }
                int idClassShrimpB = db.ClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals("B"))?.id ?? 0;

                decimal valB = db.PriceListClassShrimp
                                  .FirstOrDefault(fod => fod.id_priceList == apPLgp.id_priceList
                                  && fod.id_classShrimp == idClassShrimpB).value;

                var fsgList = db.FanSeriesGrammage.Where(w => w.id_grammage == apPLgp.id_grammage && w.id_processtype == apPLgp.id_processType).ToList();
                var fsgEncList = db.FanSeriesGrammage.Where(w => w.id_grammage == apPLgp.id_grammage && w.ProcessType.code == "ENC").ToList();

                //valida Lista de Precio y Gramaje
                if (apPLgp.id_priceList > 0 && apPLgp.id_grammage > 0 && fsgList != null && fsgEncList != null && (poundsHead + poundsTail) > 0)
                {
                    ///valida  el Proceso
                    if (db.ProcessType.FirstOrDefault(fod => fod.id == apPLgp.id_processType) != null)
                    {
                        //Verifica el Proceso
                        if (apPLgp.codeProcessType == "ENT" || apPLgp.codeProcessType == "COL")
                        {
                            if (fsgList.Count > 0)
                            {
                                #region "Obtengo las Lista del abanico segun Proceso: Entero o Cola, Además de las listas de precios"
                                var generalList = fsgList
                                                    .Where(w => w.percentage > 0)
                                                    .Select(s => new
                                                    {
                                                        valuePoundsHead = (Decimal)s.percentage * poundsHead,
                                                        valuePoundsTail = (Decimal)s.percentage * poundsTail,
                                                        id_processType = s.id_processtype,
                                                        id_itemSize = s.id_itemsize,
                                                        id_class = s.id_class
                                                    }).ToList();

                                var generalListHeadTail = fsgEncList
                                                            .Where(w => w.percentage > 0)
                                                            .Select(s => new
                                                            {
                                                                valuePoundsTail = (Decimal)s.percentage * poundsTail,
                                                                s.id_processtype,
                                                                s.id_itemsize,
                                                                s.id_class
                                                            });

                                List<PriceListDet> priceListISD = db.PriceListItemSizeDetail
                                                        .Where(w => w.Id_PriceList == apPLgp.id_priceList && w.ClassShrimp.code == "D0")
                                                        .Select(s => new PriceListDet
                                                        {
                                                            //Id_Itemsize = s.Id_Itemsize,
                                                            //PriceA = s.price,
                                                            //PriceB = s.price - valB
                                                            Id_Itemsize = s.Id_Itemsize,
                                                            PriceA = s.price - (s.commission),
                                                            //PriceB = ((s.price - valB - s.commission) >= 0) ? (s.price - valB - s.commission) : 0,
                                                            Price = s.price - s.commission,
                                                            idClass = s.id_class,
                                                            codeClass = s.Class.code,
                                                            idClassShrimp = s.id_classShrimp,
                                                            codeClassShrimp = s.ClassShrimp.code
                                                        }).ToList();
                                #endregion

                                if (priceListISD != null && generalList != null && generalList.Count > 0 && priceListISD.Count > 0)
                                {
                                    #region Se complementa la información cuando es una lista Referencial
                                    //int lisBase = db.PriceList
                                    //                .FirstOrDefault(fod => fod.id == apPLgp.id_priceList)?
                                    //                .id_priceListBase ?? 0;

                                    //if (lisBase > 0)
                                    //{
                                    //    var lisPreRef = (from e in db.PriceListItemSizeDetail.Where(X => X.Id_PriceList == lisBase)
                                    //                     select new { e.Id_Itemsize, precioA = e.price, precioB = (e.price - valB) }
                                    //                    ).ToList();

                                    //    foreach (var i in priceListISD)
                                    //    {
                                    //        if (i.PriceA == 0)
                                    //        {
                                    //            i.PriceA = lisPreRef
                                    //                        .FirstOrDefault(fod => fod.Id_Itemsize == i.Id_Itemsize)?
                                    //                        .precioA ?? 0;
                                    //            i.PriceB = lisPreRef
                                    //                        .FirstOrDefault(fod => fod.Id_Itemsize == i.Id_Itemsize)?
                                    //                        .precioB ?? 0;
                                    //        }
                                    //    }
                                    //}
                                    #endregion

                                    #region"Divido si el proceso es Entero o si es cola"
                                    if (apPLgp.codeProcessType == "ENT")
                                    {
                                        #region"Region Entero"
                                        //var listHeadFinal = generalList
                                        //                        .Join(priceListISD,
                                        //                        lp1 => lp1.id_itemSize,
                                        //                        lp2 => lp2.Id_Itemsize,
                                        //                        (lp1, lp2) => new
                                        //                        {
                                        //                            value = (decimal)lp1.valuePoundsHead *
                                        //                                                        Math.Round((decimal)
                                        //                                                            (apPLgp.codeProcessType == "ENT" ? (lp2.PriceA > 0 ? (lp2.PriceA.Value / (decimal)2.2046) : 0) : lp2.PriceA), 3),
                                        //                            Price = Math.Round((decimal)(apPLgp.codeProcessType == "ENT" ? (lp2.PriceA > 0 ? (lp2.PriceA.Value / (decimal)2.2046) : 0) : lp2.PriceA), 3),
                                        //                            valuePounds = (decimal)lp1.valuePoundsHead,
                                        //                            id_itemSize = lp1.id_itemSize,
                                        //                            id_class = lp1.id_class,
                                        //                            id_processType = lp1.id_processType
                                        //                        }).ToList();
                                        var listHeadFinal = (from liseries in generalList
                                                             join j in priceListISD on liseries.id_itemSize equals j.Id_Itemsize
                                                             where liseries.id_class == j.idClass
                                                             select new
                                                             {
                                                                 value = (decimal)liseries.valuePoundsHead *
                                                                                              Math.Round((decimal)
                                                                                                  (apPLgp.codeProcessType == "ENT" ? (j.Price > 0 ? (j.Price / (decimal)2.2046) : 0) : j.Price), 3),
                                                                 Price = Math.Round((decimal)(apPLgp.codeProcessType == "ENT" ? (j.Price > 0 ? (j.Price / (decimal)2.2046) : 0) : j.Price), 3),
                                                                 valuePounds = (decimal)liseries.valuePoundsHead,
                                                                 id_itemSize = liseries.id_itemSize,
                                                                 id_class = liseries.id_class,
                                                                 id_processType = liseries.id_processType
                                                             }).ToList();

                                        //var listClass = db.Class
                                        //                    .Select(s => new
                                        //                    {
                                        //                        s.id,
                                        //                        s.code
                                        //                    }).ToList();

                                        var listTailFinal = (from a in generalListHeadTail
                                                             join b in priceListISD on a.id_itemsize equals b.Id_Itemsize
                                                             where b.idClass == a.id_class
                                                             //join c in listClass on a.id_class equals c.id
                                                             select new
                                                             {
                                                                 value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.Price, 3)), 3),
                                                                 //value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.precioB, 3)), 3),
                                                                 Price = (Math.Round((decimal)b.Price, 3)),
                                                                 //Price = (c.code == "PRIM" || c.code == "BROK") ? Math.Round((decimal)b.precioA, 3) : Math.Round((decimal)b.precioB, 3),
                                                                 valuePounds = Math.Round((a.valuePoundsTail), 3),
                                                                 a.id_itemsize,
                                                                 a.id_processtype,
                                                                 a.id_class
                                                             }).ToList();

                                        #region"Armo listas de detalles de precios finales para entero y cola"
                                        if (listHeadFinal != null && listHeadFinal.Count > 0)
                                        {
                                            foreach (var det in listHeadFinal)
                                            {
                                                lstAPPLdetail.Add(new AdvanceProviderPLDetail
                                                {
                                                    id_class = det.id_class,
                                                    id_itemSize = det.id_itemSize,
                                                    id_processType = det.id_processType,
                                                    value = det.value,
                                                    valuePounds = det.valuePounds,
                                                    price = det.Price
                                                });
                                            }
                                        }
                                        if (listTailFinal != null && listTailFinal.Count > 0)
                                        {
                                            foreach (var det in listTailFinal)
                                            {
                                                lstAPPLdetail.Add(new AdvanceProviderPLDetail
                                                {
                                                    id_class = det.id_class,
                                                    id_itemSize = det.id_itemsize,
                                                    id_processType = det.id_processtype,
                                                    value = det.value,
                                                    valuePounds = det.valuePounds,
                                                    price = det.Price
                                                });
                                            }
                                        }
                                        #endregion

                                        #endregion
                                    }
                                    else
                                    {
                                        #region"Cola"
                                        //var listClassC = db.Class
                                        //                    .Select(s => new
                                        //                    {
                                        //                        s.id,
                                        //                        s.code
                                        //                    }).ToList();

                                        var listFinalTail = (from a in generalList
                                                             join b in priceListISD on a.id_itemSize equals b.Id_Itemsize
                                                             where a.id_class == b.idClass
                                                             //join c in listClassC on a.id_class equals c.id
                                                             select new
                                                             {
                                                                 value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.Price, 3)), 3),
                                                                 //value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.precioB, 3)), 3),
                                                                 Price = Math.Round((decimal)b.Price, 3),
                                                                 //Price = (c.code == "PRIM" || c.code == "BROK") ? Math.Round((decimal)b.precioA, 3) : Math.Round((decimal)b.precioB, 3),
                                                                 valuePounds = Math.Round((a.valuePoundsTail), 3),
                                                                 a.id_itemSize,
                                                                 a.id_processType,
                                                                 a.id_class
                                                             }).ToList();
                                        if (listFinalTail != null && listFinalTail.Count > 0)
                                        {
                                            foreach (var det in listFinalTail)
                                            {
                                                lstAPPLdetail.Add(new AdvanceProviderPLDetail
                                                {
                                                    id_class = det.id_class,
                                                    id_itemSize = det.id_itemSize,
                                                    id_processType = det.id_processType,
                                                    value = det.value,
                                                    valuePounds = det.valuePounds,
                                                    price = det.Price
                                                });
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "CALCULO ANTICIPO ", "PROD");
            }

            return lstAPPLdetail;
        }

        public static List<AdvanceProviderPLDetail> CalculateAdvanceProviderFixed(AdvanceProviderPLGeneralParameters apPLgp)
        {
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            List<AdvanceProviderPLDetail> lstAPPLdetail = new List<AdvanceProviderPLDetail>();
            db = new DBContext();
            try
            {
                decimal poundsHead = 0;
                decimal poundsTail = 0;

                decimal percentage = 100;

                //Busco proveedor en algún grupo
                int idGroupPerson = db.GroupPersonByRolDetail
                                        .FirstOrDefault(fod => fod.id_person == apPLgp.idProvider)?
                                        .id_groupPersonByRol ?? 0;

                List<ItemSizePriceClass> lstPreciosClases = null;
                List<Class> lstClass = db.Class.ToList();

                int idBrok = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("BROK")).id;
                int idVetl = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("VETL")).id;

                if (idGroupPerson > 0)
                {
                    lstPreciosClases = db.ItemSizePriceClass
                                            .Where(w => w.idGroupPersonByRol == idGroupPerson)
                                            .ToList();
                }

                if (lstPreciosClases == null || lstPreciosClases.Count == 0)
                {
                    lstPreciosClases = db.ItemSizePriceClass
                                            .Where(w => w.idGroupPersonByRol == null)
                                            .ToList();
                }

                if (apPLgp.codeProcessType == "ENT")
                {
                    poundsHead = Math.Round(((apPLgp.performanceLot * apPLgp.totalPoundsLot) / percentage), 3);
                    poundsTail = Math.Round((((apPLgp.totalPoundsLot - poundsHead) * apPLgp.percentageTailPerformanceUsed) / percentage), 3);
                }
                else if (apPLgp.codeProcessType == "ENC")
                {
                    poundsTail = Math.Round(((apPLgp.totalPoundsLot) * (apPLgp.percentageTailPerformanceUsed) / percentage), 3);
                }
                else if (apPLgp.codeProcessType == "COL")
                {
                    poundsTail = apPLgp.totalPoundsLot;
                }
                if (apPLgp.codeProcessType == "ENC")
                {
                    var i = db.ProcessType.FirstOrDefault(fod => fod.code == apPLgp.codeProcessType).id;
                    apPLgp.codeProcessType = "COL";
                    apPLgp.id_processType = i;
                }

                List<FanSeriesGrammage> fsgList = new List<FanSeriesGrammage>();
                List<FanSeriesGrammage> fsgEncList = new List<FanSeriesGrammage>();

                if (apPLgp.codeProcessType == "ENT")
                {
                    fsgList = db.FanSeriesGrammage
                                .Where(w => w.id_grammage == apPLgp.id_grammage
                                            && w.id_processtype == apPLgp.id_processType
                                            && w.ProcessType1.code == "ENT")
                                .ToList();

                    fsgEncList = db.FanSeriesGrammage
                                            .Where(w => w.id_grammage == apPLgp.id_grammage
                                            && w.id_processtype == apPLgp.id_processType
                                            && w.ProcessType1.code == "COL")
                                            .ToList();
                }
                else
                {
                    fsgList = db.FanSeriesGrammage
                                .Where(w => w.id_grammage == apPLgp.id_grammage
                                            && w.id_processtype == apPLgp.id_processType)
                                .ToList();
                }
                //int idClassShrimpB = db.ClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals("B"))?.id ?? 0;

                //decimal valB = db.PriceListClassShrimp
                //                  .FirstOrDefault(fod => fod.id_priceList == apPLgp.id_priceList
                //                  && fod.id_classShrimp == idClassShrimpB).value;

                //valida Lista de Precio y Gramaje
                if (apPLgp.id_priceList > 0 && apPLgp.id_grammage > 0 && (poundsHead + poundsTail) > 0)
                {
                    ///valida  el Proceso
                    if (db.ProcessType.FirstOrDefault(fod => fod.id == apPLgp.id_processType) != null)
                    {
                        //Verifica el Proceso
                        if (apPLgp.codeProcessType == "ENT" || apPLgp.codeProcessType == "COL")
                        {
                            if (fsgList.Count > 0)
                            {
                                #region Obtengo las Lista del abanico segun Proceso: Entero o Cola, Además de las listas de precios
                                var generalList = fsgList
                                                    .Where(w => w.percentage > 0)
                                                    .Select(s => new
                                                    {
                                                        valuePoundsHead = (Decimal)s.percentage * poundsHead,
                                                        valuePoundsTail = (Decimal)s.percentage * poundsTail,
                                                        id_processType = s.id_processtype,
                                                        id_itemSize = s.id_itemsize,
                                                        id_class = s.id_class
                                                    }).ToList();

                                var generalListHeadTail = fsgEncList
                                                            .Where(w => w.percentage > 0)
                                                            .Select(s => new
                                                            {
                                                                valuePoundsTail = (Decimal)s.percentage * poundsTail,
                                                                s.id_processtype,
                                                                s.id_itemsize,
                                                                s.id_class
                                                            });

                                List<PriceListDet> priceListISD = db.PriceListItemSizeDetail
                                                                    .Where(w => w.Id_PriceList == apPLgp.id_priceList && w.ClassShrimp.code == "D0")
                                                                    .Select(s => new PriceListDet
                                                                    {
                                                                        Id_Itemsize = s.Id_Itemsize,
                                                                        //PriceA = s.price - s.commission,
                                                                        //PriceB = ((s.price - valB - s.commission) >= 0) ? s.price - valB - s.commission : 0
                                                                        Price = s.price - s.commission,
                                                                        idClass = s.id_class,
                                                                        codeClass = s.Class.code,
                                                                        idClassShrimp = s.id_classShrimp,
                                                                        codeClassShrimp = s.ClassShrimp.code,
                                                                        id_processtype = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.Id_Itemsize)
                                                                                        ? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.Id_Itemsize)
                                                                                        .id_ProcessType : null,
                                                                        code_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.Id_Itemsize)
                                                                                        ? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.Id_Itemsize)
                                                                                        .ProcessType.code : "",
                                                                        name_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.Id_Itemsize)
                                                                                        ? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.Id_Itemsize)
                                                                                        .ProcessType.name : "",
                                                                    }).ToList();

                                //Actualizo Valores de Broken y Venta Local
                                //foreach (var det in priceListISD)
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

                                #endregion

                                if (priceListISD != null && generalList != null && generalList.Count > 0 && priceListISD.Count > 0)
                                {
                                    #region Se complementa la información cuando es una lista Referencial
                                    int lisBase = db.PriceList
                                                    .FirstOrDefault(fod => fod.id == apPLgp.id_priceList)?
                                                    .id_priceListBase ?? 0;

                                    //if (lisBase > 0)
                                    //{
                                    //    var lisPreRef = (from e in db.PriceListItemSizeDetail.Where(X => X.Id_PriceList == lisBase)
                                    //                     select new { e.Id_Itemsize, precioA = e.price, precioB = (e.price - valB) }
                                    //                    ).ToList();

                                    //    foreach (var i in priceListISD)
                                    //    {
                                    //        if (i.PriceA == 0)
                                    //        {
                                    //            i.PriceA = lisPreRef
                                    //                        .FirstOrDefault(fod => fod.Id_Itemsize == i.Id_Itemsize)?
                                    //                        .precioA ?? 0;
                                    //            i.PriceB = lisPreRef
                                    //                        .FirstOrDefault(fod => fod.Id_Itemsize == i.Id_Itemsize)?
                                    //                        .precioB ?? 0;
                                    //        }
                                    //    }
                                    //}
                                    #endregion

                                    #region"Divido si el proceso es Entero o si es cola"
                                    if (apPLgp.codeProcessType == "ENT")
                                    {
                                        #region"Region Entero"
                                        //var listHeadFinal = generalList
                                        //                        .Join(priceListISD,
                                        //                        lp1 => lp1.id_itemSize,
                                        //                        lp2 => lp2.Id_Itemsize,
                                        //                        (lp1, lp2) => new
                                        //                        {
                                        //                            value = (decimal)lp1.valuePoundsHead *
                                        //                                                        Math.Round((decimal)
                                        //                                                            (apPLgp.codeProcessType == "ENT" ? (lp2.PriceA > 0 ? (lp2.PriceA.Value / (decimal)2.2046) : 0) : lp2.PriceA), 3),
                                        //                            Price = Math.Round((decimal)(apPLgp.codeProcessType == "ENT" ? (lp2.PriceA > 0 ? (lp2.PriceA.Value / (decimal)2.2046) : 0) : lp2.PriceA), 3),
                                        //                            valuePounds = (decimal)lp1.valuePoundsHead,
                                        //                            id_itemSize = lp1.id_itemSize,
                                        //                            id_class = lp1.id_class,
                                        //                            id_processType = lp1.id_processType
                                        //                        }).ToList();
                                        var listHeadFinal = (from liseries in generalList
                                                             join j in priceListISD on liseries.id_itemSize equals j.Id_Itemsize
                                                             where liseries.id_class == j.idClass
                                                             select new
                                                             {
                                                                 value = (decimal)liseries.valuePoundsHead *
                                                                                              Math.Round((decimal)
                                                                                                  (apPLgp.codeProcessType == "ENT" ? (j.Price > 0 ? (j.Price / (decimal)2.2046) : 0) : j.Price), 3),
                                                                 Price = Math.Round((decimal)(apPLgp.codeProcessType == "ENT" ? (j.Price > 0 ? (j.Price / (decimal)2.2046) : 0) : j.Price), 3),
                                                                 valuePounds = (decimal)liseries.valuePoundsHead,
                                                                 id_itemSize = liseries.id_itemSize,
                                                                 id_class = liseries.id_class,
                                                                 id_processType = j.id_processtype
                                                             }).ToList();
                                        //var listClass = db.Class
                                        //                    .Select(s => new
                                        //                    {
                                        //                        s.id,
                                        //                        s.code
                                        //                    }).ToList();

                                        var listTailFinal = (from a in generalListHeadTail
                                                             join b in priceListISD on a.id_itemsize equals b.Id_Itemsize
                                                             where b.idClass == a.id_class
                                                             //join c in listClass on a.id_class equals c.id
                                                             select new
                                                             {
                                                                 value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.Price, 3)), 3),
                                                                 //value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.precioB, 3)), 3),
                                                                 Price = (Math.Round((decimal)b.Price, 3)),
                                                                 //Price = (c.code == "PRIM" || c.code == "BROK") ? Math.Round((decimal)b.precioA, 3) : Math.Round((decimal)b.precioB, 3),
                                                                 valuePounds = Math.Round((a.valuePoundsTail), 3),
                                                                 a.id_itemsize,
                                                                 id_processType = b.id_processtype,
                                                                 a.id_class
                                                             }).ToList();

                                        #region"Armo listas de detalles de precios finales para entero y cola"
                                        if (listHeadFinal != null && listHeadFinal.Count > 0)
                                        {
                                            foreach (var det in listHeadFinal)
                                            {
                                                lstAPPLdetail.Add(new AdvanceProviderPLDetail
                                                {
                                                    id_class = det.id_class,
                                                    id_itemSize = det.id_itemSize,
                                                    id_processType = det.id_processType.Value,
                                                    value = det.value,
                                                    valuePounds = det.valuePounds,
                                                    price = det.Price
                                                });
                                            }
                                        }
                                        if (listTailFinal != null && listTailFinal.Count > 0)
                                        {
                                            foreach (var det in listTailFinal)
                                            {
                                                lstAPPLdetail.Add(new AdvanceProviderPLDetail
                                                {
                                                    id_class = det.id_class,
                                                    id_itemSize = det.id_itemsize,
                                                    id_processType = det.id_processType.Value,
                                                    value = det.value,
                                                    valuePounds = det.valuePounds,
                                                    price = det.Price
                                                });
                                            }
                                        }
                                        #endregion

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Cola O Descabezar
                                        //var listClassC = db.Class
                                        //                    .Select(s => new
                                        //                    {
                                        //                        s.id,
                                        //                        s.code
                                        //                    }).ToList();

                                        var listFinalTail = (from a in generalList
                                                             join b in priceListISD on a.id_itemSize equals b.Id_Itemsize
                                                             where a.id_class == b.idClass
                                                             //join c in listClassC on a.id_class equals c.id
                                                             select new
                                                             {
                                                                 value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.Price, 3)), 3),
                                                                 //value = Math.Round((a.valuePoundsTail * Math.Round((decimal)b.precioB, 3)), 3),
                                                                 Price = Math.Round((decimal)b.Price, 3),
                                                                 //Price = (c.code == "PRIM" || c.code == "BROK") ? Math.Round((decimal)b.precioA, 3) : Math.Round((decimal)b.precioB, 3),
                                                                 valuePounds = Math.Round((a.valuePoundsTail), 3),
                                                                 a.id_itemSize,
                                                                 id_processType = b.id_processtype,
                                                                 a.id_class
                                                             }).ToList();
                                        if (listFinalTail != null && listFinalTail.Count > 0)
                                        {
                                            foreach (var det in listFinalTail)
                                            {
                                                lstAPPLdetail.Add(new AdvanceProviderPLDetail
                                                {
                                                    id_class = det.id_class,
                                                    id_itemSize = det.id_itemSize,
                                                    id_processType = det.id_processType.Value,
                                                    value = det.value,
                                                    valuePounds = det.valuePounds,
                                                    price = det.Price
                                                });
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "CALCULO ANTICIPO ", "PROD");
            }

            return lstAPPLdetail;
        }
    }
}



