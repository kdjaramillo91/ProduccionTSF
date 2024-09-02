
function GetPenality(order) {
      
    var orderAux = 0;
    if (GridViewPriceListPenalty.pageRowCount > 0) {
        for (let rowDetail = 0; rowDetail < GridViewPriceListPenalty.pageRowCount; rowDetail++) {
            if (GridViewPriceListPenalty.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
            orderAux = GridViewPriceListPenalty.batchEditApi.GetCellValue(rowDetail, 1);
            if (orderAux === order) {
                return parseFloat(GridViewPriceListPenalty.batchEditApi.GetCellValue(rowDetail, 3));
            }
        }
    }
    return 0.00;
}

function OnBatchEditEndEditingPenalty(s, e) {
      

    var PriceColIndex = s.GetColumnByField("value").index;
    var OrderPriceColIndex = s.GetColumnByField("order").index;

    var priceValue = e.rowValues[PriceColIndex].value;
    var orderValue = e.rowValues[OrderPriceColIndex].value - 1;

    var DnCurrent = "";
    var Dn = "";
    var price0Aux = 0;
    var price1Aux = 0;
    var diff1Aux = 0;
    var price2Aux = 0;
    var diff2Aux = 0;
    var price3Aux = 0;
    var diff3Aux = 0;
    var price4Aux = 0;
    var diff4Aux = 0;
    var price5Aux = 0;
    var diff5Aux = 0;
    var price6Aux = 0;
    var diff6Aux = 0;
    var price7Aux = 0;
    var diff7Aux = 0;
    var price8Aux = 0;
    var diff8Aux = 0;
    var balanceDataCell = null;

    var commissionIniDn = 0.00;
    var price_PCIniAux = 0.00;
    var nameIni_PC_Dn = "";
    var price_RFIniDn = 0.00;
    var diff_F_RFIniAux = 0.00;
    var nameIni_Diff_F_RFDn = "";

    var idItemSizeAux = 0;
    var viewAllH = parseInt($("#NviewAllH").val());
    if (GridViewPriceListDetailsCOLB.pageRowCount > 0) {
        for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOLB.pageRowCount; rowDetail++) {
            if (GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
            //for (var j = 0; j < GridViewPriceListDetailsCOL.cpOrderClassShrimp.length; j++) {
            idItemSizeAux = parseInt(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0));
            switch (orderValue) {
                case 0:
                    price0Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 5);
                    price0Aux = price0Aux < priceValue ? 0.00 : price0Aux - priceValue;
                    DnCurrent = "D0.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux.toFixed(2), price0Aux.toFixed(2), true);

                    if (viewAllH !== 1)
                    {
                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 6);
                        price_PCIniAux = price0Aux < commissionIniDn ? 0.00 : price0Aux - commissionIniDn;
                        nameIni_PC_Dn = "D0.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 4);
                        diff_F_RFIniAux = price0Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D0.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price1Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 11));
                        if (!isNaN(price1Aux)) {
                            diff1Aux = price0Aux - price1Aux;
                            Dn = "D1.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff1Aux.toFixed(2), diff1Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff1Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff1Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }
                    }
                    
                    break;
                case 1:
                    price0Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 5));
                    price1Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 11);
                    price1Aux = price1Aux < priceValue ? 0.00 : price1Aux - priceValue;
                    DnCurrent = "D1.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price1Aux.toFixed(2), price1Aux.toFixed(2), true);

                    if (viewAllH != 1)
                    {
                        diff1Aux = price0Aux - price1Aux;
                        Dn = "D1.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff1Aux.toFixed(2), diff1Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff1Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff1Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 12);
                        price_PCIniAux = price1Aux < commissionIniDn ? 0.00 : price1Aux - commissionIniDn;
                        nameIni_PC_Dn = "D1.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 10);
                        diff_F_RFIniAux = price1Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D1.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price2Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 17));
                        if (!isNaN(price2Aux)) {
                            diff2Aux = price1Aux - price2Aux;
                            Dn = "D2.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff2Aux.toFixed(2), diff2Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff2Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff2Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }

                    }
                    
                    break;
                case 2:
                    price1Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 11));
                    price2Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 17);
                    price2Aux = price2Aux < priceValue ? 0.00 : price2Aux - priceValue;
                    DnCurrent = "D2.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price2Aux.toFixed(2), price2Aux.toFixed(2), true);
                    if (viewAllH != 1)
                    {
                        diff2Aux = price1Aux - price2Aux;
                        Dn = "D2.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff2Aux.toFixed(2), diff2Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff2Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff2Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 18);
                        price_PCIniAux = price2Aux < commissionIniDn ? 0.00 : price2Aux - commissionIniDn;
                        nameIni_PC_Dn = "D2.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 16);
                        diff_F_RFIniAux = price2Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D2.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price3Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 23));
                        if (!isNaN(price3Aux)) {
                            diff3Aux = price2Aux - price3Aux;
                            Dn = "D3.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff3Aux.toFixed(2), diff3Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff3Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff3Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }
                    }
                    
                    break;
                case 3:
                    price2Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 17));
                    price3Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 23);
                    price3Aux = price3Aux < priceValue ? 0.00 : price3Aux - priceValue;
                    DnCurrent = "D3.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price3Aux.toFixed(2), price3Aux.toFixed(2), true);
                    if (viewAllH != 1)
                    {
                        diff3Aux = price2Aux - price3Aux;
                        Dn = "D3.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff3Aux.toFixed(2), diff3Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff3Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff3Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 24);
                        price_PCIniAux = price3Aux < commissionIniDn ? 0.00 : price3Aux - commissionIniDn;
                        nameIni_PC_Dn = "D3.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 22);
                        diff_F_RFIniAux = price3Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D3.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price4Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 29));
                        if (!isNaN(price4Aux)) {
                            diff4Aux = price3Aux - price4Aux;
                            Dn = "D4.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff4Aux.toFixed(2), diff4Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff4Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff4Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }
                    }
                    
                    break;
                case 4:
                    price3Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 23));
                    price4Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 29);
                    price4Aux = price4Aux < priceValue ? 0.00 : price4Aux - priceValue;
                    DnCurrent = "D4.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price4Aux.toFixed(2), price4Aux.toFixed(2), true);
                    if (viewAllH != 1)
                    {
                        diff4Aux = price3Aux - price4Aux;
                        Dn = "D4.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff4Aux.toFixed(2), diff4Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff4Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff4Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 30);
                        price_PCIniAux = price4Aux < commissionIniDn ? 0.00 : price4Aux - commissionIniDn;
                        nameIni_PC_Dn = "D4.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 28);
                        diff_F_RFIniAux = price4Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D4.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price5Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 35));
                        if (!isNaN(price5Aux)) {
                            diff5Aux = price4Aux - price5Aux;
                            Dn = "D5.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff5Aux.toFixed(2), diff5Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff5Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff5Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }
                    }
                    
                    break;
                case 5:
                    price4Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 29));
                    price5Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 35);
                    price5Aux = price5Aux < priceValue ? 0.00 : price5Aux - priceValue;
                    DnCurrent = "D5.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price5Aux.toFixed(2), price5Aux.toFixed(2), true);
                    if (viewAllH != 1)
                    {
                        diff5Aux = price4Aux - price5Aux;
                        Dn = "D5.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff5Aux.toFixed(2), diff5Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff5Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff5Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 36);
                        price_PCIniAux = price5Aux < commissionIniDn ? 0.00 : price5Aux - commissionIniDn;
                        nameIni_PC_Dn = "D5.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 34);
                        diff_F_RFIniAux = price5Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D5.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price6Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 41));
                        if (!isNaN(price6Aux)) {
                            diff6Aux = price5Aux - price6Aux;
                            Dn = "D6.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff6Aux.toFixed(2), diff6Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff6Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff6Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }
                    }
                    
                    break;
                case 6:
                    price5Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 35));
                    price6Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 41);
                    price6Aux = price6Aux < priceValue ? 0.00 : price6Aux - priceValue;
                    DnCurrent = "D6.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price6Aux.toFixed(2), price6Aux.toFixed(2), true);
                    if (viewAllH != 1)
                    {
                        diff6Aux = price5Aux - price6Aux;
                        Dn = "D6.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff6Aux.toFixed(2), diff6Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff6Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff6Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 42);
                        price_PCIniAux = price6Aux < commissionIniDn ? 0.00 : price6Aux - commissionIniDn;
                        nameIni_PC_Dn = "D6.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 40);
                        diff_F_RFIniAux = price6Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D6.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price7Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 47));
                        if (!isNaN(price7Aux)) {
                            diff7Aux = price6Aux - price7Aux;
                            Dn = "D7.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff7Aux.toFixed(2), diff7Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff7Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff7Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }
                    }
                    
                    break;
                case 7:
                    price6Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 41));
                    price7Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 47);
                    price7Aux = price7Aux < priceValue ? 0.00 : price7Aux - priceValue;
                    DnCurrent = "D7.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price7Aux.toFixed(2), price7Aux.toFixed(2), true);
                    if (viewAllH != 1)
                    {
                        diff7Aux = price6Aux - price7Aux;
                        Dn = "D7.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff7Aux.toFixed(2), diff7Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff7Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff7Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 48);
                        price_PCIniAux = price7Aux < commissionIniDn ? 0.00 : price7Aux - commissionIniDn;
                        nameIni_PC_Dn = "D7.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 46);
                        diff_F_RFIniAux = price7Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D7.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        price8Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 53));
                        if (!isNaN(price8Aux)) {
                            diff8Aux = price7Aux - price8Aux;
                            Dn = "D8.difference";
                            GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff8Aux.toFixed(2), diff8Aux.toFixed(2), true);
                            balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                            if (diff8Aux.toFixed(2) !== "0.00") {
                                balanceDataCell.style.backgroundColor = diff8Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                            }
                            else {
                                balanceDataCell.style.backgroundColor = "";
                            }
                        }
                    }
                    
                    break;
                case 8:
                    price7Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 47));
                    price8Aux = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 53);
                    price8Aux = price8Aux < priceValue ? 0.00 : price8Aux - priceValue;
                    DnCurrent = "D8.price";
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, DnCurrent, price8Aux.toFixed(2), price8Aux.toFixed(2), true);
                    if (viewAllH != 1)
                    {
                        diff8Aux = price7Aux - price8Aux;
                        Dn = "D8.difference";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, Dn, diff8Aux.toFixed(2), diff8Aux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                        if (diff8Aux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff8Aux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }

                        commissionIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 54);
                        price_PCIniAux = price8Aux < commissionIniDn ? 0.00 : price8Aux - commissionIniDn;
                        nameIni_PC_Dn = "D8.price_PC";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);

                        price_RFIniDn = GetPriceInGridViewPriceListDetails(GridViewPriceListDetailsCOL, idItemSizeAux, 52);
                        diff_F_RFIniAux = price8Aux - price_RFIniDn;
                        nameIni_Diff_F_RFDn = "D8.difference_F_RF";
                        GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                        if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                            balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                        }
                        else {
                            balanceDataCell.style.backgroundColor = "";
                        }
                    }
                    

                    break;
                default:
                    break;
                //}
            }
        }
    }

}

function GetPriceInGridViewPriceListDetails(aGridViewPriceListDetails, aidItemSize, aIndexPrice) {
    var idItemSizeAux = 0;
    if (aGridViewPriceListDetails.pageRowCount > 0) {
        for (let rowDetail = 0; rowDetail < aGridViewPriceListDetails.pageRowCount; rowDetail++) {
            if (aGridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;

            idItemSizeAux = parseInt(aGridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 0));
            if (aidItemSize === idItemSizeAux) {
                return parseFloat(aGridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, aIndexPrice));
            }
        }
    }

    return 0.00;
}

function RePaintDetailsENT(s, e) {
    //btnReplicateDetailsENT.SetEnabled(GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === true ||
    //    GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === "true" ||
    //    GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === "True");
    
    var DnCurrent = "";
    var Dn = "";
    var priceAux = 0.00;
    var commissionAux = 0.00;
    var price_PCAux = 0.00;
    var commissionIniDn = 0.00;
    //var price_PCIniAux = 0.00;
    var nameIni_PC_Dn = "";
    var price_RFIniDn = 0.00;
    var diff_F_RFIniAux = 0.00;
    var nameIni_Diff_F_RFDn = "";
    var balanceDataCell = null;
    var viewAllH = parseInt($("#NviewAllH").val());
    if (viewAllH == 1) return;
    for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsENT.pageRowCount; rowDetail++) {
        if (GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
        for (var j = 0; j < GridViewPriceListDetailsENT.cpOrderClassShrimp.length; j++) {
            switch (j) {
                case 0:
                    //price0Aux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(2);
                    //commission0Aux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(2);
                    //price_PC0Aux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(2);

                    nameIni_Diff_F_RFDn = "D0.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 8)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 1:

                    Dn = "D1.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 9)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D1.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 14)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 2:
                    Dn = "D2.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 15)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D2.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 20)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 3:
                    Dn = "D3.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 21)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D3.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 26)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 4:
                    Dn = "D4.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 27)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D4.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 32)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 5:
                    Dn = "D5.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 33)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D5.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 38)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 6:
                    Dn = "D6.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 39)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D6.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 44)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 7:
                    Dn = "D7.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 45)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D7.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 50)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 8:
                    Dn = "D8.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 51)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D8.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 56)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                default:
                    break;
            }
        }
    }
}

function ReplicateDetailsENT() {
    debugger;
    
    if (GridViewPriceListDetailsENT.pageRowCount > 0) {
        var result = DevExpress.ui.dialog.confirm("Desea Replicar los valores de D0 al resto de valores?", "Confirmar");
        result.done(function (dialogResult) {
            if (dialogResult) {


                var DnCurrent = "";
                var Dn = "";
                var price0Aux = 0;
                var commission0Aux = 0;
                var price_PC0Aux = 0;
                var commissionIniDn = 0.00;
                //var price_PCIniAux = 0.00;
                var nameIni_PC_Dn = "";
                var price_RFIniDn = 0.00;
                var diff_F_RFIniAux = 0.00;
                var nameIni_Diff_F_RFDn = "";
                var balanceDataCell = null;
                var viewAllH = parseInt($("#NviewAllH").val());
                for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsENT.pageRowCount; rowDetail++) {
                    if (GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
                    for (var j = 0; j < GridViewPriceListDetailsENT.cpOrderClassShrimp.length; j++) {
                        switch (j) {
                            case 0:
                                price0Aux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(2);
                                if (viewAllH !== 1)
                                {
                                    commission0Aux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(2);
                                    price_PC0Aux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(2);
                                }
                                break;
                            case 1:
                                DnCurrent = "D1.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D1.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D1.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D1.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 10)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D1.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }

                                break;
                            case 2:
                                DnCurrent = "D2.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D2.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D2.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D2.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 16)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D2.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }

                                break;
                            case 3:
                                DnCurrent = "D3.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D3.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D3.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D3.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 22)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D3.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }
                                break;
                            case 4:
                                DnCurrent = "D4.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D4.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D4.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D4.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 28)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D4.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }

                                break;
                            case 5:
                                DnCurrent = "D5.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D5.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D5.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D5.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 34)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D5.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }
                                

                                break;
                            case 6:
                                DnCurrent = "D6.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D6.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D6.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D6.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 40)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D6.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }
                                break;
                            case 7:
                                DnCurrent = "D7.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D7.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D7.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D7.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 46)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D7.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }
                                

                                break;
                            case 8:
                                DnCurrent = "D8.price";
                                GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH !== 1)
                                {
                                    Dn = "D8.difference";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);

                                    commissionIniDn = "D8.commission";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, commissionIniDn, commission0Aux, commission0Aux, true);

                                    nameIni_PC_Dn = "D8.price_PC";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 52)).toFixed(2);
                                    diff_F_RFIniAux = price0Aux - price_RFIniDn;
                                    nameIni_Diff_F_RFDn = "D8.difference_F_RF";
                                    GridViewPriceListDetailsENT.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                                    balanceDataCell = GridViewPriceListDetailsENT.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell.style.backgroundColor = "";
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        });
    } else {
        NotifyWarning("No hay detalle para replicar");
    }
}

//function OnEndCallbackCOL(s, e) {
//      
//    btnReplicateDetailsCOL.SetEnabled(GridViewPriceListDetailsCOL.cpCanReplicateDetailsCOL === true ||
//        GridViewPriceListDetailsCOL.cpCanReplicateDetailsCOL === "true" ||
//        GridViewPriceListDetailsCOL.cpCanReplicateDetailsCOL === "True");
//}

function UpdateDetailsCOLB() {
    
    if (GridViewPriceListDetailsCOL.pageRowCount > 0) {
        
        var idItemSizeAux = 0;
        var priceDnCurrent = 0.00;
        var commissionDnCurrent = 0.00;
        var priceDnLast = 0.00;
        var commissionDnLast = 0.00;


        for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOL.pageRowCount; rowDetail++) {
            if (GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
            idItemSizeAux = parseInt(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 0));
            var viewAllH = parseInt($("#NviewAllH").val());
            if (viewAllH == 1) continue;

            for (var j = 0; j < GridViewPriceListDetailsCOL.cpOrderClassShrimp.length; j++) {
                switch (j) {
                    case 0:
                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(2);
                        if (GridViewPriceListDetailsCOL.cpOrderClassShrimp.length === 1) {
                            UpdateGridViewPriceListDetailsCOLB0(parseFloat(priceDnCurrent), parseFloat(commissionDnCurrent), 4, idItemSizeAux);
                        }
                        break;
                    case 1:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 11)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 12)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D0.price", parseFloat(commissionDnLast), "D0.commission", 4, 0, "D0.price_PC", "D0.difference_F_RF",
                            parseFloat(priceDnCurrent), "D1.price", parseFloat(commissionDnCurrent), "D1.commission", 10, 1, "D1.price_PC", "D1.difference_F_RF",
                            "D1.difference", idItemSizeAux);

                        break;
                    case 2:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 11)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 12)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 17)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 18)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D1.price", parseFloat(commissionDnLast), "D1.commission", 10, 1, "D1.price_PC", "D1.difference_F_RF",
                            parseFloat(priceDnCurrent), "D2.price", parseFloat(commissionDnCurrent), "D2.commission", 16, 2, "D2.price_PC", "D2.difference_F_RF",
                            "D2.difference", idItemSizeAux);
                        break;
                    case 3:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 17)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 18)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 23)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 24)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D2.price", parseFloat(commissionDnLast), "D2.commission", 16, 2, "D2.price_PC", "D2.difference_F_RF",
                            parseFloat(priceDnCurrent), "D3.price", parseFloat(commissionDnCurrent), "D3.commission", 22, 3, "D3.price_PC", "D3.difference_F_RF",
                            "D3.difference", idItemSizeAux);
                        break;
                    case 4:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 23)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 24)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 29)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 30)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D3.price", parseFloat(commissionDnLast), "D3.commission", 22, 3, "D3.price_PC", "D3.difference_F_RF",
                            parseFloat(priceDnCurrent), "D4.price", parseFloat(commissionDnCurrent), "D4.commission", 28, 4, "D4.price_PC", "D4.difference_F_RF",
                            "D4.difference", idItemSizeAux);
                        break;
                    case 5:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 29)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 30)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 35)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 36)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D4.price", parseFloat(commissionDnLast), "D4.commission", 28, 4, "D4.price_PC", "D4.difference_F_RF",
                            parseFloat(priceDnCurrent), "D5.price", parseFloat(commissionDnCurrent), "D5.commission", 34, 5, "D5.price_PC", "D5.difference_F_RF",
                            "D5.difference", idItemSizeAux);
                        break;
                    case 6:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 35)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 36)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 41)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 42)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D5.price", parseFloat(commissionDnLast), "D5.commission", 34, 5, "D5.price_PC", "D5.difference_F_RF",
                            parseFloat(priceDnCurrent), "D6.price", parseFloat(commissionDnCurrent), "D6.commission", 40, 6, "D6.price_PC", "D6.difference_F_RF",
                            "D6.difference", idItemSizeAux);
                        break;
                    case 7:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 41)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 42)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 47)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 48)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D6.price", parseFloat(commissionDnLast), "D6.commission", 40, 6, "D6.price_PC", "D6.difference_F_RF",
                            parseFloat(priceDnCurrent), "D7.price", parseFloat(commissionDnCurrent), "D7.commission", 46, 7, "D7.price_PC", "D7.difference_F_RF",
                            "D7.difference", idItemSizeAux);
                        break;
                    case 8:
                        priceDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 47)).toFixed(2);
                        commissionDnLast = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 48)).toFixed(2);

                        priceDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 53)).toFixed(2);
                        commissionDnCurrent = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 54)).toFixed(2);

                        UpdateGridViewPriceListDetailsCOLB(parseFloat(priceDnLast), "D7.price", parseFloat(commissionDnLast), "D7.commission", 46, 7, "D7.price_PC", "D7.difference_F_RF",
                            parseFloat(priceDnCurrent), "D8.price", parseFloat(commissionDnCurrent), "D8.commission", 52, 8, "D8.price_PC", "D8.difference_F_RF",
                            "D8.difference", idItemSizeAux);
                        break;
                    default:
                        break;
                }
            }
        }
    } 
}

function ReplicateDetailsCOL() {
    debugger;
    if (GridViewPriceListDetailsCOL.pageRowCount > 0) {
        var result = DevExpress.ui.dialog.confirm("Desea Replicar los valores de D0 al resto de valores?", "Confirmar");
        result.done(function (dialogResult) {
            if (dialogResult) {
                  
                var DnCommission = "";
                var DnCurrent = "";
                var Dn = "";
                var price0Aux = 0.00;
                var idItemSizeAux = 0;
                var commission0Aux = 0;
                var price_PC0Aux = 0;
                var price_PCFinAux = 0.00;
                var nameFin_PC_Dn = "";
                var price_RFIniDn = 0.00;
                var price_RFFinDn = 0.00;
                var diff_F_RFFinAux = 0.00;
                var nameFin_Diff_F_RFDn = "";
                var viewAllH = parseInt($("#NviewAllH").val());

                for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOL.pageRowCount; rowDetail++) {
                    if (GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
                    idItemSizeAux = parseInt(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 0));
                    for (var j = 0; j < GridViewPriceListDetailsCOL.cpOrderClassShrimp.length; j++) {
                        switch (j) {
                            case 0:
                                price0Aux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(2);
                                if (viewAllH !== 1)
                                {
                                    commission0Aux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(2);
                                    price_PC0Aux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(2);
                                }
                                break;
                            case 1:
                                DnCurrent = "D1.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH == 1)
                                {
                                    commission0Aux = 0;
                                }
                                else
                                {
                                    Dn = "D1.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell11 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell11.style.backgroundColor = "";

                                    DnCommission = "D1.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D1.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 10)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D1.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell1 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell1.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell1.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 4)).toFixed(2);
                                }
                                

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D0.price",
                                    parseFloat(commission0Aux),
                                    "D0.commission", 4, 0,
                                    "D0.price_PC",
                                    "D0.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D1.price",
                                    parseFloat(commission0Aux),
                                    "D1.commission", 10, 1,
                                    "D1.price_PC",
                                    "D1.difference_F_RF",
                                    "D1.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D0.price", 0, price0Aux, "D1.price", 1, "D1.difference", idItemSizeAux);

                                break;
                            case 2:
                                DnCurrent = "D2.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH == 1) {
                                    commission0Aux = 0;
                                }
                                else
                                {
                                    Dn = "D2.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell22 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell22.style.backgroundColor = "";

                                    DnCommission = "D2.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D2.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 16)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D2.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell2 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell2.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell2.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 10)).toFixed(2);
                                }
                                

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D1.price",
                                    parseFloat(commission0Aux),
                                    "D1.commission", 10, 1,
                                    "D1.price_PC",
                                    "D1.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D2.price",
                                    parseFloat(commission0Aux),
                                    "D2.commission", 16, 2,
                                    "D2.price_PC",
                                    "D2.difference_F_RF",
                                    "D2.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D1.price", 1, price0Aux, "D2.price", 2, "D2.difference", idItemSizeAux);
                                break;
                            case 3:
                                DnCurrent = "D3.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH == 1) {
                                    commission0Aux = 0;
                                }
                                else
                                {
                                    Dn = "D3.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell33 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell33.style.backgroundColor = "";

                                    DnCommission = "D3.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D3.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 22)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D3.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell3 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell3.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell3.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 16)).toFixed(2);
                                }

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D2.price",
                                    parseFloat(commission0Aux),
                                    "D2.commission", 16, 2,
                                    "D2.price_PC",
                                    "D2.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D3.price",
                                    parseFloat(commission0Aux),
                                    "D3.commission", 22, 3,
                                    "D3.price_PC",
                                    "D3.difference_F_RF",
                                    "D3.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D2.price", 2, price0Aux, "D3.price", 3, "D3.difference", idItemSizeAux);
                                break;
                            case 4:
                                DnCurrent = "D4.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH == 1) {
                                    commission0Aux = 0;
                                }
                                else
                                {
                                    Dn = "D4.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell44 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell44.style.backgroundColor = "";

                                    DnCommission = "D4.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D4.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 28)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D4.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell4 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell4.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell4.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 22)).toFixed(2);
                                }

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D3.price",
                                    parseFloat(commission0Aux),
                                    "D3.commission", 22, 3,
                                    "D3.price_PC",
                                    "D3.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D4.price",
                                    parseFloat(commission0Aux),
                                    "D4.commission", 28, 4,
                                    "D4.price_PC",
                                    "D4.difference_F_RF",
                                    "D4.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D3.price", 3, price0Aux, "D4.price", 4, "D4.difference", idItemSizeAux);
                                break;
                            case 5:
                                DnCurrent = "D5.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH == 1) {
                                    commission0Aux = 0;
                                } else
                                {
                                    Dn = "D5.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell55 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell55.style.backgroundColor = "";

                                    DnCommission = "D5.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D5.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);


                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 34)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D5.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell5 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell5.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell5.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 28)).toFixed(2);
                                }
                                

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D4.price",
                                    parseFloat(commission0Aux),
                                    "D4.commission", 28, 4,
                                    "D4.price_PC",
                                    "D4.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D5.price",
                                    parseFloat(commission0Aux),
                                    "D5.commission", 34, 5,
                                    "D5.price_PC",
                                    "D5.difference_F_RF",
                                    "D5.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D4.price", 4, price0Aux, "D5.price", 5, "D5.difference", idItemSizeAux);
                                break;
                            case 6:
                                DnCurrent = "D6.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);

                                if (viewAllH == 1) {
                                    commission0Aux = 0;
                                }
                                else
                                {
                                    Dn = "D6.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell66 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell66.style.backgroundColor = "";

                                    DnCommission = "D6.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D6.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 40)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D6.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell6 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell6.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell6.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 34)).toFixed(2);
                                }
                                

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D5.price",
                                    parseFloat(commission0Aux),
                                    "D5.commission", 34, 5,
                                    "D5.price_PC",
                                    "D5.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D6.price",
                                    parseFloat(commission0Aux),
                                    "D6.commission", 40, 6,
                                    "D6.price_PC",
                                    "D6.difference_F_RF",
                                    "D6.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D5.price", 5, price0Aux, "D6.price", 6, "D6.difference", idItemSizeAux);
                                break;
                            case 7:
                                DnCurrent = "D7.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH == 1) {
                                    commission0Aux = 0;
                                }
                                else
                                {
                                    Dn = "D7.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell77 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell77.style.backgroundColor = "";

                                    DnCommission = "D7.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D7.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 46)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D7.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell7 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell7.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell7.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 40)).toFixed(2);
                                }
                                

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D6.price",
                                    parseFloat(commission0Aux),
                                    "D6.commission", 40, 6,
                                    "D6.price_PC",
                                    "D6.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D7.price",
                                    parseFloat(commission0Aux),
                                    "D7.commission", 46, 7,
                                    "D7.price_PC",
                                    "D7.difference_F_RF",
                                    "D7.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D6.price", 6, price0Aux, "D7.price", 7, "D7.difference", idItemSizeAux);
                                break;
                            case 8:
                                DnCurrent = "D8.price";
                                GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCurrent, price0Aux, price0Aux, true);
                                if (viewAllH == 1) {
                                    commission0Aux = 0;
                                }
                                else
                                {
                                    Dn = "D8.difference";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, Dn, "0.00", "0.00", true);
                                    var balanceDataCell88 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                                    balanceDataCell88.style.backgroundColor = "";

                                    DnCommission = "D8.commission";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, DnCommission, commission0Aux, commission0Aux, true);

                                    nameFin_PC_Dn = "D8.price_PC";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PC0Aux, price_PC0Aux, true);

                                    price_RFFinDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 52)).toFixed(2);
                                    diff_F_RFFinAux = price0Aux - price_RFFinDn;
                                    nameFin_Diff_F_RFDn = "D8.difference_F_RF";
                                    GridViewPriceListDetailsCOL.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                                    var balanceDataCell8 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                                        balanceDataCell8.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                                    }
                                    else {
                                        balanceDataCell8.style.backgroundColor = "";
                                    }
                                    price_RFIniDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 46)).toFixed(2);
                                }
                                

                                UpdateGridViewPriceListDetailsCOLB(
                                    parseFloat(price0Aux),
                                    "D7.price",
                                    parseFloat(commission0Aux),
                                    "D7.commission", 46, 7,
                                    "D7.price_PC",
                                    "D7.difference_F_RF",
                                    parseFloat(price0Aux),
                                    "D8.price",
                                    parseFloat(commission0Aux),
                                    "D8.commission", 52, 8,
                                    "D8.price_PC",
                                    "D8.difference_F_RF",
                                    "D8.difference",
                                    idItemSizeAux);

                                //UpdateGridViewPriceListDetailsCOLB(price0Aux, "D7.price", 7, price0Aux, "D8.price", 8, "D8.difference", idItemSizeAux);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        });
    } else {
        NotifyWarning("No hay detalle para replicar");
    }
}

function RePaintDetailsCOL(s, e) {
    //btnReplicateDetailsENT.SetEnabled(GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === true ||
    //    GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === "true" ||
    //    GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === "True");
    
    var DnCurrent = "";
    var Dn = "";
    var priceAux = 0.00;
    var commissionAux = 0.00;
    var price_PCAux = 0.00;
    var commissionIniDn = 0.00;
    //var price_PCIniAux = 0.00;
    var nameIni_PC_Dn = "";
    var price_RFIniDn = 0.00;
    var diff_F_RFIniAux = 0.00;
    var nameIni_Diff_F_RFDn = "";
    var balanceDataCell = null;
    var viewAllH = parseInt($("#NviewAllH").val());
    if (viewAllH == 1) return;
    for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOL.pageRowCount; rowDetail++) {
        if (GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
        for (var j = 0; j < GridViewPriceListDetailsCOL.cpOrderClassShrimp.length; j++) {
            switch (j) {
                case 0:
                    //price0Aux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(2);
                    //commission0Aux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(2);
                    //price_PC0Aux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(2);

                    nameIni_Diff_F_RFDn = "D0.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 8)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 1:

                    Dn = "D1.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 9)).toFixed(2);
                    var balanceDataCell1 = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell1.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell1.style.backgroundColor = "";
                    }
                    nameIni_Diff_F_RFDn = "D1.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 14)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 2:
                    Dn = "D2.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 15)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D2.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 20)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 3:
                    Dn = "D3.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 21)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D3.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 26)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 4:
                    Dn = "D4.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 27)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D4.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 32)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 5:
                    Dn = "D5.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 33)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D5.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 38)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 6:
                    Dn = "D6.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 39)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D6.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 44)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 7:
                    Dn = "D7.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 45)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }


                    nameIni_Diff_F_RFDn = "D7.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 50)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }


                    break;
                case 8:
                    Dn = "D8.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 51)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D8.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 56)).toFixed(2);
                    balanceDataCell = GridViewPriceListDetailsCOL.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    } else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                default:
                    break;
            }
        }
    }
}

function OnBatchEditEndEditingCOLB(s, e) {

    debugger;
    var priceDnCurrent = 0.00;
    var priceDnLast = 0.00;
    var price_PC_DnLast = 0.00;
    var CommissionDnLast = 0.00;
    var price_RF_DnLast = 0.00;
    var difference_F_RFDnLast = 0.00;
    var priceDn = "0.00";
    var Price_RF_DnIndex = 0;
    var Price_PC_DnIndex = 0;
    var CommissionDnIndex = 0;
    var PriceDnIndex = 0;
    var Dn = "";
    var balanceDataCell = null;
    var viewAllH = parseInt($("#NviewAllH").val());
    //if (viewAllH == 1) return;
    for (var i = 0; i < GridViewPriceListDetailsCOLB.cpOrderClassShrimp.length; i++) {
        switch (i) {
            case 0:
                PriceDnIndex = s.GetColumnByField("D0.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D0.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnLast < CommissionDnLast ? 0.00 : priceDnLast - CommissionDnLast;

                    Dn = "D0.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D0.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnLast - price_RF_DnLast;

                    Dn = "D0.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 1:
                PriceDnIndex = s.GetColumnByField("D1.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;

                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D1.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D1.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D1.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D1.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D0.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D1.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 2:
                PriceDnIndex = s.GetColumnByField("D2.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D2.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D2.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D2.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D2.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D1.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D2.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 3:
                PriceDnIndex = s.GetColumnByField("D3.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D3.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D3.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D3.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D3.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D2.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D3.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 4:
                PriceDnIndex = s.GetColumnByField("D4.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D4.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D4.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D4.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D4.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D3.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D4.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 5:
                PriceDnIndex = s.GetColumnByField("D5.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;

                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D5.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D5.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D5.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D5.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D4.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D5.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 6:
                PriceDnIndex = s.GetColumnByField("D6.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D6.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D6.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D6.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D6.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D5.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D6.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 7:
                PriceDnIndex = s.GetColumnByField("D7.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D7.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D7.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D7.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D7.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D6.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D7.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            case 8:
                PriceDnIndex = s.GetColumnByField("D8.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH != 1)
                {
                    CommissionDnIndex = s.GetColumnByField("D8.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                    Dn = "D8.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D8.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                    Dn = "D8.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D7.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    //PriceDnIndex = s.GetColumnByField("D1.price").index;
                    //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                    Dn = "D8.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                break;
            default:
                break;
        }
    }
}

function RePaintDetailsCOLB(s, e) {
    //btnReplicateDetailsENT.SetEnabled(GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === true ||
    //    GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === "true" ||
    //    GridViewPriceListDetailsENT.cpCanReplicateDetailsENT === "True");

    
    var DnCurrent = "";
    var Dn = "";
    var priceAux = 0.00;
    var commissionAux = 0.00;
    var price_PCAux = 0.00;
    var commissionIniDn = 0.00;
    //var price_PCIniAux = 0.00;
    var nameIni_PC_Dn = "";
    var price_RFIniDn = 0.00;
    var diff_F_RFIniAux = 0.00;
    var nameIni_Diff_F_RFDn = "";
    var balanceDataCell = null;
    var viewAllH = parseInt($("#NviewAllH").val());
    if (viewAllH == 1) return;
    for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOLB.pageRowCount; rowDetail++) {
        if (GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
        for (var j = 0; j < GridViewPriceListDetailsCOLB.cpOrderClassShrimp.length; j++) {
            switch (j) {
                case 0:
                    //price0Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(2);
                    //commission0Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(2);
                    //price_PC0Aux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(2);

                    nameIni_Diff_F_RFDn = "D0.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 8)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 1:

                    Dn = "D1.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 9)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D1.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 14)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 2:
                    Dn = "D2.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 15)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }


                    nameIni_Diff_F_RFDn = "D2.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 20)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }


                    break;
                case 3:
                    Dn = "D3.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 21)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }


                    nameIni_Diff_F_RFDn = "D3.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 26)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }


                    break;
                case 4:
                    Dn = "D4.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 27)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D4.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 32)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 5:
                    Dn = "D5.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 33)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D5.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 38)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 6:
                    Dn = "D6.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 39)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D6.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 44)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 7:
                    Dn = "D7.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 45)).toFixed(2);
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D7.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 50)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                case 8:
                    Dn = "D8.difference";
                    price_differenceDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 51)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, Dn).parentNode;
                    if (price_differenceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_differenceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    nameIni_Diff_F_RFDn = "D8.difference_F_RF";
                    price_difference_F_RFDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 56)).toFixed(2);
                        balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (price_difference_F_RFDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = price_difference_F_RFDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    break;
                default:
                    break;
            }
        }
    }
}

function OnBatchEditEndEditingCOL(s, e) {
    
    debugger;
    var priceDnCurrent = 0.00;
    var priceDnLast = 0.00;
    var price_PC_DnLast = 0.00;
    var price_PC_DnCurrent = 0.00;
    var CommissionDnLast = 0.00;
    var CommissionDnCurrent = 0.00;
    var price_RF_DnLast = 0.00;
    var price_RF_DnCurrent = 0.00;
    var difference_F_RFDnLast = 0.00;
    var difference_F_RFDnCurrent = 0.00;
    var priceDn = "0.00";
    var Price_RF_DnIndex = 0;
    var Price_PC_DnIndex = 0;
    var CommissionDnIndex = 0;
    var PriceDnIndex = 0;
    var Dn = "";
    var balanceDataCell = null;
    var idItemSizeIndex = s.GetColumnByField("idItemSize").index;
    var idItemSizeAux = e.rowValues[idItemSizeIndex].value;
    var viewAllH = parseInt($("#NviewAllH").val());
    //if (viewAllH == 1) return;
    debugger;
    for (var i = 0; i < GridViewPriceListDetailsCOL.cpOrderClassShrimp.length; i++) {

        switch (i) {
            case 0:
                
                PriceDnIndex = s.GetColumnByField("D0.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                if (viewAllH == 1) {

                    CommissionDnLast = 0;

                } else
                {
                    CommissionDnIndex = s.GetColumnByField("D0.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnLast = priceDnLast < CommissionDnLast ? 0.00 : priceDnLast - CommissionDnLast;
                    Dn = "D0.price_PC";
                    priceDn = price_PC_DnLast.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}
                    Price_RF_DnIndex = s.GetColumnByField("D0.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnLast = priceDnLast - price_RF_DnLast;

                    Dn = "D0.difference_F_RF";
                    priceDn = difference_F_RFDnLast.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }

                if (viewAllH == 1) {
                    UpdateGridViewPriceListDetailsCOLB0(parseFloat(priceDnLast), parseFloat(CommissionDnLast), 4, idItemSizeAux);
                }
                else
                {
                    if (GridViewPriceListDetailsCOL.cpOrderClassShrimp.length === 1) {
                        UpdateGridViewPriceListDetailsCOLB0(parseFloat(priceDnLast), parseFloat(CommissionDnLast), 4, idItemSizeAux);
                        //UpdateGridViewPriceListDetailsCOLB0(parseFloat(priceDnLast), parseFloat(CommissionDnLast), numberColumn, idItemSizeAux);
                    }
                }
                
                break;
            case 1:
                PriceDnIndex = s.GetColumnByField("D1.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH == 1)
                {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D1.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D1.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D1.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D1.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D0.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D0.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D0.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D1.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D0.price",
                    CommissionDnLast,
                    "D0.commission", 4, 0,
                    "D0.price_PC",
                    "D0.difference_F_RF",
                    priceDnCurrent,
                    "D1.price",
                    CommissionDnCurrent,
                    "D1.commission", 10, 1,
                    "D1.price_PC",
                    "D1.difference_F_RF",
                    "D1.difference",
                    idItemSizeAux);
                break;
            case 2:
                PriceDnIndex = s.GetColumnByField("D2.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH == 1) {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D2.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D2.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D2.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D2.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D1.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D1.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D1.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D2.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                

                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D1.price",
                    CommissionDnLast,
                    "D1.commission", 10, 1,
                    "D1.price_PC",
                    "D1.difference_F_RF",
                    priceDnCurrent,
                    "D2.price",
                    CommissionDnCurrent,
                    "D2.commission", 16, 2,
                    "D2.price_PC",
                    "D2.difference_F_RF",
                    "D2.difference",
                    idItemSizeAux);
                break;
            case 3:
                
                PriceDnIndex = s.GetColumnByField("D3.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH == 1) {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D3.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D3.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D3.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D3.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D2.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D2.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D2.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D3.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                

                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D2.price",
                    CommissionDnLast,
                    "D2.commission", 16, 2,
                    "D2.price_PC",
                    "D2.difference_F_RF",
                    priceDnCurrent,
                    "D3.price",
                    CommissionDnCurrent,
                    "D3.commission", 22, 3,
                    "D3.price_PC",
                    "D3.difference_F_RF",
                    "D3.difference",
                    idItemSizeAux);
                break;
            case 4:
                PriceDnIndex = s.GetColumnByField("D4.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                if (viewAllH == 1) {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D4.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D4.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D4.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D4.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D3.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D3.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D3.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D4.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                

                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D3.price",
                    CommissionDnLast,
                    "D3.commission", 22, 3,
                    "D3.price_PC",
                    "D3.difference_F_RF",
                    priceDnCurrent,
                    "D4.price",
                    CommissionDnCurrent,
                    "D4.commission", 28, 4,
                    "D4.price_PC",
                    "D4.difference_F_RF",
                    "D4.difference",
                    idItemSizeAux);
                break;
            case 5:
                PriceDnIndex = s.GetColumnByField("D5.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;

                if (viewAllH == 1) {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D5.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D5.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D5.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D5.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D4.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D4.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D4.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D5.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                

                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D4.price",
                    CommissionDnLast,
                    "D4.commission", 28, 4,
                    "D4.price_PC",
                    "D4.difference_F_RF",
                    priceDnCurrent,
                    "D5.price",
                    CommissionDnCurrent,
                    "D5.commission", 34, 5,
                    "D5.price_PC",
                    "D5.difference_F_RF",
                    "D5.difference",
                    idItemSizeAux);
                break;
            case 6:
                PriceDnIndex = s.GetColumnByField("D6.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;

                if (viewAllH == 1) {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D6.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D6.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D6.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D6.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D5.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D5.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D5.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D6.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                

                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D5.price",
                    CommissionDnLast,
                    "D5.commission", 34, 5,
                    "D5.price_PC",
                    "D5.difference_F_RF",
                    priceDnCurrent,
                    "D6.price",
                    CommissionDnCurrent,
                    "D6.commission", 40, 6,
                    "D6.price_PC",
                    "D6.difference_F_RF",
                    "D6.difference",
                    idItemSizeAux);
                break;
            case 7:
                PriceDnIndex = s.GetColumnByField("D7.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;

                if (viewAllH == 1) {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D7.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D7.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D7.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D7.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D6.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D6.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D6.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D7.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                

                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D6.price",
                    CommissionDnLast,
                    "D6.commission", 40, 6,
                    "D6.price_PC",
                    "D6.difference_F_RF",
                    priceDnCurrent,
                    "D7.price",
                    CommissionDnCurrent,
                    "D7.commission", 46, 7,
                    "D7.price_PC",
                    "D7.difference_F_RF",
                    "D7.difference",
                    idItemSizeAux);
                break;
            case 8:
                PriceDnIndex = s.GetColumnByField("D8.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;

                if (viewAllH == 1) {
                    CommissionDnLast = 0;
                    priceDnLast = priceDnCurrent;
                    CommissionDnCurrent = 0;
                }
                else
                {
                    CommissionDnIndex = s.GetColumnByField("D8.commission").index;
                    CommissionDnCurrent = e.rowValues[CommissionDnIndex].value;
                    price_PC_DnCurrent = priceDnCurrent < CommissionDnCurrent ? 0.00 : priceDnCurrent - CommissionDnCurrent;

                    Dn = "D8.price_PC";
                    priceDn = price_PC_DnCurrent.toFixed(2);
                    //if (priceDn !== "0.00") {
                    Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[Price_PC_DnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    //}

                    Price_RF_DnIndex = s.GetColumnByField("D8.price_RF").index;
                    price_RF_DnCurrent = e.rowValues[Price_RF_DnIndex].value;
                    difference_F_RFDnCurrent = priceDnCurrent - price_RF_DnCurrent;

                    Dn = "D8.difference_F_RF";
                    priceDn = difference_F_RFDnCurrent.toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    PriceDnIndex = s.GetColumnByField("D7.price").index;
                    priceDnLast = e.rowValues[PriceDnIndex].value;
                    CommissionDnIndex = s.GetColumnByField("D7.commission").index;
                    CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                    Price_RF_DnIndex = s.GetColumnByField("D7.price_RF").index;
                    price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;

                    Dn = "D8.difference";
                    priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                    PriceDnIndex = s.GetColumnByField(Dn).index;
                    e.rowValues[PriceDnIndex].value = priceDn;
                    s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                    if (priceDn !== "0.00") {
                        balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                

                UpdateGridViewPriceListDetailsCOLB(
                    priceDnLast,
                    "D7.price",
                    CommissionDnLast,
                    "D7.commission", 46, 7,
                    "D7.price_PC",
                    "D7.difference_F_RF",
                    priceDnCurrent,
                    "D8.price",
                    CommissionDnCurrent,
                    "D8.commission", 52, 8,
                    "D8.price_PC",
                    "D8.difference_F_RF",
                    "D8.difference",
                    idItemSizeAux);
                break;
            default:
                break;
        }
    }
}
         
function UpdateGridViewPriceListDetailsCOLB0(priceIniDn, commissionIniDn, pos_RFIniDn, idItemSize) {

    debugger;
    var idItemSizeAux = 0;
    var nameIniDn = "D0.price";
    var nameIni_PC_Dn = "D0.price_PC";
    var nameIni_Diff_F_RFDn = "D0.difference_F_RF";
    var orderIniDn = 0;
    var nameCommissionIniDn = "D0.commission";
    var viewAllH = parseInt($("#NviewAllH").val());
    if (GridViewPriceListDetailsCOLB.pageRowCount > 0) {
        for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOLB.pageRowCount; rowDetail++) {
            if (GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
            debugger;
            idItemSizeAux = parseInt(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0));
            if (idItemSize === idItemSizeAux) {

                debugger;
                var priceValueOrderIniDn = GetPenality(++orderIniDn);
                var priceIniAux = priceIniDn < priceValueOrderIniDn ? 0.00 : priceIniDn - priceValueOrderIniDn;
                priceIniAux = parseFloat(priceIniAux.toFixed(2));
                debugger;
                GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIniDn, priceIniAux.toFixed(2), priceIniAux.toFixed(2), true);
                if (viewAllH !== 1)
                {
                    
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameCommissionIniDn, commissionIniDn.toFixed(2), commissionIniDn.toFixed(2), true);
                    var price_PCIniAux = priceIniAux < commissionIniDn ? 0.00 : priceIniAux - commissionIniDn;
                    price_PCIniAux = parseFloat(price_PCIniAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);
                    var price_RFIniDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, pos_RFIniDn));
                    var diff_F_RFAux = priceIniAux - price_RFIniDn;
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFAux.toFixed(2), diff_F_RFAux.toFixed(2), true);
                    var balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (diff_F_RFAux.toFixed(2) !== "0.00") {
                        balanceDataCell.style.backgroundColor = diff_F_RFAux < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }
                }
                
            }
        }
    }
}
         
function UpdateGridViewPriceListDetailsCOLB(
    priceIniDn,
    nameIniDn,
    commissionIniDn,
    nameCommissionIniDn,
    pos_RFIniDn,// 4
    orderIniDn, // 0
    nameIni_PC_Dn,
    nameIni_Diff_F_RFDn,
    priceFinDn,
    nameFinDn, // "D1.price"
    commissionFinDn,
    nameCommissionFinDn,
    pos_RFFinDn, // 10
    orderFinDn,  // 1
    nameFin_PC_Dn,
    nameFin_Diff_F_RFDn,
    diffFinDn,
    idItemSize) {
    
    var viewAllH = parseInt($("#NviewAllH").val());
    var idItemSizeAux = 0;
    if (GridViewPriceListDetailsCOLB.pageRowCount > 0) {
        for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOLB.pageRowCount; rowDetail++) {

            if (GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;

            idItemSizeAux = parseInt(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0));
            if (idItemSize === idItemSizeAux) {
                
                if (viewAllH != 1) {
                    var priceValueOrderIniDn = GetPenality(++orderIniDn);
                    var priceIniAux = parseFloat(priceIniDn) < parseFloat(priceValueOrderIniDn) ? 0.00 : parseFloat(priceIniDn) - parseFloat(priceValueOrderIniDn);
                    priceIniAux = parseFloat(priceIniAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIniDn, priceIniAux.toFixed(2), priceIniAux.toFixed(2), true);

                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameCommissionIniDn, parseFloat(commissionIniDn).toFixed(2), parseFloat(commissionIniDn).toFixed(2), true);
                    var price_PCIniAux = priceIniAux < parseFloat(commissionIniDn) ? 0.00 : priceIniAux - parseFloat(commissionIniDn);
                    price_PCIniAux = parseFloat(price_PCIniAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_PC_Dn, price_PCIniAux.toFixed(2), price_PCIniAux.toFixed(2), true);
                    var price_RFIniDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, pos_RFIniDn));
                    var diff_F_RFIniAux = priceIniAux - price_RFIniDn;
                    diff_F_RFIniAux = parseFloat(diff_F_RFIniAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameIni_Diff_F_RFDn, diff_F_RFIniAux.toFixed(2), diff_F_RFIniAux.toFixed(2), true);
                    var balanceDataCell = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameIni_Diff_F_RFDn).parentNode;
                    if (diff_F_RFIniAux.toFixed(2) !== "0.00") {
                        balanceDataCell.style.backgroundColor = diff_F_RFIniAux < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell.style.backgroundColor = "";
                    }

                    var priceValueOrderFinDn = GetPenality(++orderFinDn);
                    var priceFinAux = parseFloat(priceFinDn) < parseFloat(priceValueOrderFinDn) ? 0.00 : parseFloat(priceFinDn) - parseFloat(priceValueOrderFinDn);
                    priceFinAux = parseFloat(priceFinAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameFinDn, priceFinAux.toFixed(2), priceFinAux.toFixed(2), true);
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameCommissionFinDn, parseFloat(commissionFinDn).toFixed(2), parseFloat(commissionFinDn).toFixed(2), true);
                    var price_PCFinAux = priceFinAux < parseFloat(commissionFinDn) ? 0.00 : priceFinAux - parseFloat(commissionFinDn);
                    price_PCFinAux = parseFloat(price_PCFinAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameFin_PC_Dn, price_PCFinAux.toFixed(2), price_PCFinAux.toFixed(2), true);
                    var price_RFFinDn = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, pos_RFFinDn));
                    var diff_F_RFFinAux = priceFinAux - price_RFFinDn;
                    diff_F_RFFinAux = parseFloat(diff_F_RFFinAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameFin_Diff_F_RFDn, diff_F_RFFinAux.toFixed(2), diff_F_RFFinAux.toFixed(2), true);
                    var balanceDataCell2 = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, nameFin_Diff_F_RFDn).parentNode;
                    if (diff_F_RFFinAux.toFixed(2) !== "0.00") {
                        balanceDataCell2.style.backgroundColor = diff_F_RFFinAux < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell2.style.backgroundColor = "";
                    }

                    var diffAux = priceIniAux - priceFinAux;
                    diffAux = parseFloat(diffAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, diffFinDn, diffAux.toFixed(2), diffAux.toFixed(2), true);
                    var balanceDataCell3 = GridViewPriceListDetailsCOLB.batchEditApi.GetCellTextContainer(rowDetail, diffFinDn).parentNode;
                    if (diffAux.toFixed(2) !== "0.00") {
                        balanceDataCell3.style.backgroundColor = diffAux < 0 ? "#C6EFCE" : "#FFC7CE";
                    }
                    else {
                        balanceDataCell3.style.backgroundColor = "";
                    }
                }
                else
                {
                    var priceValueOrderFinDn = GetPenality(++orderFinDn);
                    var priceFinAux = parseFloat(priceFinDn) < parseFloat(priceValueOrderFinDn) ? 0.00 : parseFloat(priceFinDn) - parseFloat(priceValueOrderFinDn);
                    priceFinAux = parseFloat(priceFinAux.toFixed(2));
                    GridViewPriceListDetailsCOLB.batchEditApi.SetCellValue(rowDetail, nameFinDn, priceFinAux.toFixed(2), priceFinAux.toFixed(2), true);

                }
                
            }
        }
    }
}

        
function OnBatchEditEndEditingENT(s, e) {

    debugger;
    var priceDnCurrent = 0.00;
    var priceDnLast = 0.00;
    var price_PC_DnLast = 0.00;
    var CommissionDnLast = 0.00;
    var price_RF_DnLast = 0.00;
    var difference_F_RFDnLast = 0.00;
    var priceDn = "0.00";
    var Price_RF_DnIndex = 0;
    var Price_PC_DnIndex = 0;
    var CommissionDnIndex = 0;
    var PriceDnIndex = 0;
    var Dn = "";
    var balanceDataCell = null;
    var viewAllH = parseInt($("#NviewAllH").val());
    if (viewAllH == 1) return;
    for (var i = 0; i < GridViewPriceListDetailsENT.cpOrderClassShrimp.length; i++) {
        switch (i) {
            case 0:
                PriceDnIndex = s.GetColumnByField("D0.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;

                CommissionDnIndex = s.GetColumnByField("D0.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnLast < CommissionDnLast ? 0.00 : priceDnLast - CommissionDnLast;

                Dn = "D0.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D0.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnLast - price_RF_DnLast;

                Dn = "D0.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 1:
                PriceDnIndex = s.GetColumnByField("D1.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D1.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D1.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D1.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D1.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D0.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D1.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 2:
                PriceDnIndex = s.GetColumnByField("D2.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D2.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D2.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D2.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D2.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D1.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D2.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 3:
                PriceDnIndex = s.GetColumnByField("D3.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D3.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D3.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D3.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D3.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D2.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D3.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 4:
                PriceDnIndex = s.GetColumnByField("D4.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D4.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D4.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D4.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D4.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D3.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D4.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 5:
                PriceDnIndex = s.GetColumnByField("D5.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D5.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D5.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D5.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D5.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D4.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D5.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 6:
                PriceDnIndex = s.GetColumnByField("D6.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D6.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D6.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D6.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D6.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D5.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D6.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 7:
                PriceDnIndex = s.GetColumnByField("D7.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D7.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D7.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D7.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D7.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D6.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D7.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            case 8:
                PriceDnIndex = s.GetColumnByField("D8.price").index;
                priceDnCurrent = e.rowValues[PriceDnIndex].value;
                CommissionDnIndex = s.GetColumnByField("D8.commission").index;
                CommissionDnLast = e.rowValues[CommissionDnIndex].value;
                price_PC_DnLast = priceDnCurrent < CommissionDnLast ? 0.00 : priceDnCurrent - CommissionDnLast;

                Dn = "D8.price_PC";
                priceDn = price_PC_DnLast.toFixed(2);
                //if (priceDn !== "0.00") {
                Price_PC_DnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[Price_PC_DnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                //}

                Price_RF_DnIndex = s.GetColumnByField("D8.price_RF").index;
                price_RF_DnLast = e.rowValues[Price_RF_DnIndex].value;
                difference_F_RFDnLast = priceDnCurrent - price_RF_DnLast;

                Dn = "D8.difference_F_RF";
                priceDn = difference_F_RFDnLast.toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }

                PriceDnIndex = s.GetColumnByField("D7.price").index;
                priceDnLast = e.rowValues[PriceDnIndex].value;
                //PriceDnIndex = s.GetColumnByField("D1.price").index;
                //priceDnCurrent = e.rowValues[PriceDnIndex].value;

                Dn = "D8.difference";
                priceDn = (priceDnLast - priceDnCurrent).toFixed(2);
                PriceDnIndex = s.GetColumnByField(Dn).index;
                e.rowValues[PriceDnIndex].value = priceDn;
                s.batchEditApi.SetCellValue(e.visibleIndex, Dn, priceDn, priceDn, true);
                    balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, Dn).parentNode;
                if (priceDn !== "0.00") {
                    balanceDataCell.style.backgroundColor = priceDn < 0 ? "#C6EFCE" : "#FFC7CE";
                }
                else {
                    balanceDataCell.style.backgroundColor = "";
                }
                break;
            default:
                break;
        }
    }
}

function OnBatchEditEndEditing(s, e) {
      
    //GridViewPriceListDetails.cpOrderClassShrimp
    if (CheckBoxEsCotizacion.GetValue()) {

        var PriceColIndex = s.GetColumnByField("price").index;
        var BasePriceColIndex = s.GetColumnByField("basePrice").index;
        var CommissionColIndex = s.GetColumnByField("commission").index;

        var priceValue = e.rowValues[PriceColIndex].value;
        var basePriceValue = e.rowValues[BasePriceColIndex].value;
        var commissionValue = e.rowValues[CommissionColIndex].value;

        if (commissionValue > priceValue) {
            commissionValue = priceValue;

            e.itemValues[4].value = commissionValue;
            e.itemValues[4].text = commissionValue;
            s.batchEditApi.SetCellValue(e.visibleIndex, "commission", commissionValue);
        }

        var pricePurchase = priceValue - commissionValue;
        s.batchEditApi.SetCellValue(e.visibleIndex, "pricePurchase", pricePurchase, null, true);

        var distint = pricePurchase - basePriceValue;
        s.batchEditApi.SetCellValue(e.visibleIndex, "distint", distint, null, true);

        var balanceDataCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, "distint").parentNode;
        balanceDataCell.style.backgroundColor = (distint < 0) ? "#C6EFCE" : "#FFC7CE";
    }
}

function PageControl_Init(s, e) {
    var aDivGridViewPriceListDetailsENT = document.getElementById("divGridViewPriceListDetailsENT");
    if ($("#code_processtype").val() === "ENT") {
        aDivGridViewPriceListDetailsENT.style.display = "block";
        pcFeatures.tabs[0].SetText("Precio por Talla Entero    ");
        pcFeatures.tabs[0].SetEnabled(true);
    } else {
        aDivGridViewPriceListDetailsENT.style.display = "none";
        pcFeatures.tabs[0].SetText("");
        pcFeatures.tabs[0].SetEnabled(false);
    }
}

function PrintItem() {
    var data = { id_priceList: $('#id_listPrice').val() };
      
    $.ajax({
        url: 'PriceListResponsive/PrintPriceListReport',
        type: 'post',
        data: data,
        async: true,
        cache: false,
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                  
                if (result != undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                    newWindow.focus();
                    hideLoading();
                }
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
            hideLoading();
        }
    });

}

function AddNewItem() {

    showLoading();

    var data = {
        id: 0,
        enabled: true
    }

    showPage("PriceListResponsive/Edit", data);
}

function AprovedItem() {

    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            showLoading();

            if (!Validate()) {
                hideLoading();
                return;
            }

            $.ajax({
                url: 'PriceListResponsive/Save',
                type: 'post',
                data: SaveDataUser(),
                async: true,
                cache: false,
                error: function (resultSave) {
                    hideLoading();
                    NotifyError("Error. " + resultSave.Message);
                },
                success: function (resultSave) {
                    if (resultSave.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Guardar. " + resultSave.Message);
                        return;
                    }

                    var id = resultSave.Data;
                    $('#id_listPrice').val(id);

                    $.ajax({
                        url: 'PriceListResponsive/Approve',
                        type: 'post',
                        data: { id: $('#id_listPrice').val() },
                        async: true,
                        cache: false,
                        error: function (resultApprove) {
                            hideLoading();
                            NotifyError("Error. " + resultApprove.Message);
                        },
                        success: function (resultApprove) {
                            if (resultApprove.Code !== 0) {
                                hideLoading();
                                NotifyError("Error al Aprobar. " + resultApprove.Message);
                                return;
                            }

                            ShowCurrentItem(false);
                            //hideLoading();
                            NotifySuccess("Elemento Aprobado Satisfactoriamente. " + "Estado: " + resultApprove.Data);
                        },
                    });
                },
            });
        }
    });
}

function ShowCurrentItem(enable) {
    showLoading();

    var data = {
        id: $('#id_listPrice').val(),
        enabled: enable
    }

    showPage("PriceListResponsive/Edit", data);
}

function RedirecBack() {
    if ($("#from").val() == "pending")
        showPage("PriceListResponsive/PendingApprove");
    else
        showPage("PriceListResponsive/Index");
}

function CloseItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Cerrar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            showLoading();

            var id_listPrice = $('#id_listPrice').val();
            if (id_listPrice == 0) {
                hideLoading();
                NotifyDialog("No puede cerrar un elemento que no este guardado");
                return;
            }

            $.ajax({
                url: 'PriceListResponsive/Close',
                type: 'post',
                data: { id: $('#id_listPrice').val() },
                async: true,
                cache: false,
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    //hideLoading();
                    NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                },
                error: function (result) {
                    hideLoading();
                },
            });
        }
    });
}

function AnnulItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Anular el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            showLoading();

            var id_listPrice = $('#id_listPrice').val();
            if (id_listPrice == 0) {
                hideLoading();
                NotifyDialog("No puede anular un elemento que no este guardado");
                return;
            }

            $.ajax({
                url: 'PriceListResponsive/Annul',
                type: 'post',
                data: { id: $('#id_listPrice').val() },
                async: true,
                cache: false,
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    //hideLoading();
                    NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                },
                error: function (result) {
                    hideLoading();
                },
            });
        }
    });
}

function ReverseItem() {

    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            showLoading();

            var id_listPrice = $('#id_listPrice').val();
            if (id_listPrice === 0) {
                hideLoading();
                NotifyDialog("No puede reversar un elemento que no este guardado");
                return;
            }

            $.ajax({
                url: 'PriceListResponsive/Reverse',
                type: 'post',
                data: { id: $('#id_listPrice').val() },
                async: true,
                cache: false,
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    //hideLoading();
                    NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                },
                error: function (result) {
                    hideLoading();
                },
            });
        }
    });
}

function EditCurrentItem() {
    showLoading();

    var data = {
        id: $('#id_listPrice').val(),
        enabled: true
    }

    showPage("PriceListResponsive/Edit", data);
}

function DuplicateCurrentItem() {
    showLoading();

    var data = {
        id: $('#id_listPrice').val(),
    }

    showPage("PriceListResponsive/Duplicate", data);
}

function SaveCurrentItem() {
    SaveItem();
}

function EditItem() {

}

function RemoveItems() {
}

function RefreshReplicateEdit() {
    showLoading();

    var id_listPrice = $('#id_listPrice').val();
    if (id_listPrice == 0) {
        hideLoading();
        NotifyDialog("No se puede refrescar la replicación de una lista que no este guardada");
        return;
    }

    $.ajax({
        url: 'PriceListResponsive/RefreshReplicate',
        type: 'post',
        data: { id: id_listPrice },
        async: true,
        cache: false,
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            hideLoading();
            NotifySuccess("Se refresco la replicación de la lista de manera Satisfactoria.");
        },
        error: function (result) {
            hideLoading();
        }
    });

}

function RefreshGrid() {

}

function Print() {

}

function VisibleCotizacion(visible) {
    if (visible) {
        document.getElementById("checkParaProveedores").style.display = "block";
        document.getElementById("checkParaGrupos").style.display = "block";
        document.getElementById("divLabelListaBase").style.display = "block";
        document.getElementById("divComboBoxListasPrecio").style.display = "block";
        document.getElementById("rowProveedorGrupo").style.display = "block";
        document.getElementById("divLabelCertificacion").style.display = "block";
        document.getElementById("divComboBoxCertificaciones").style.display = "block";
    }
    else {
        document.getElementById("checkParaProveedores").style.display = "none";
        document.getElementById("checkParaGrupos").style.display = "none";
        document.getElementById("divLabelListaBase").style.display = "none";
        document.getElementById("divComboBoxListasPrecio").style.display = "none";
        document.getElementById("rowProveedorGrupo").style.display = "none";
        document.getElementById("divLabelCertificacion").style.display = "none";
        document.getElementById("divComboBoxCertificaciones").style.display = "none";
    }
}

function VisibleProveedor(visible) {
    if (visible) {
        document.getElementById("divLabelProveedor").style.display = "block";
        document.getElementById("divComboBoxProveedores").style.display = "block";
    }
    else {
        document.getElementById("divLabelProveedor").style.display = "none";
        document.getElementById("divComboBoxProveedores").style.display = "none";
    }
}

function VisibleGrupo(visible) {
    if (visible) {
        document.getElementById("divLabelGrupo").style.display = "block";
        document.getElementById("divComboBoxGrupos").style.display = "block";
        document.getElementById("divGruposEye").style.display = "block";
    }
    else {
        document.getElementById("divLabelGrupo").style.display = "none";
        document.getElementById("divComboBoxGrupos").style.display = "none";
        document.getElementById("divGruposEye").style.display = "none";
    }
}

function init() {
    if ($("#isQuotation").val() === "True" || $("#isQuotation").val() === true) {
        VisibleCotizacion(true);
    }

    if ($("#paraProveedor").val() === "True" || $("#paraProveedor").val() === true) {
        VisibleProveedor(true);
    }

    if ($("#paraGrupo").val() === "True" || $("#paraGrupo").val() === true) {
        VisibleGrupo(true);
    }

}

function setRangeDateForm() {

    var str = ComboBoxPeriodoCalendario.GetText();
    if (str == null || str == "" || str == "undefined")
        return;

    var res = str.split(" ");

    var dateFrom = res[1].split("/");
    var yearFrom = dateFrom[2];
    var monthFrom = dateFrom[1];
    var dayFrom = dateFrom[0];

    var dateTo = res[3].split("/");
    var yearTo = dateTo[2];
    var monthTo = dateTo[1];
    var dayTo = dateTo[0];

    DateEditFechaDesde.SetMinDate(new Date(yearFrom, monthFrom - 1, dayFrom));
    DateEditFechaDesde.SetMaxDate(new Date(yearTo, monthTo - 1, dayTo));
}

function setRangeDateTo() {

    var str = ComboBoxPeriodoCalendario.GetText();
    if (str == null || str == "" || str == "undefined" || str == undefined)
        return;

    var res = str.split(" ");

    var dateFrom = res[1].split("/");
    var yearFrom = dateFrom[2];
    var monthFrom = dateFrom[1];
    var dayFrom = dateFrom[0];

    var dateTo = res[3].split("/");
    var yearTo = dateTo[2];
    var monthTo = dateTo[1];
    var dayTo = dateTo[0];

    DateEditFechaHasta.SetMinDate(new Date(yearFrom, monthFrom - 1, dayFrom));
    DateEditFechaHasta.SetMaxDate(new Date(yearTo, monthTo - 1, dayTo));
}

function setRangeDate() {

    var str = ComboBoxPeriodoCalendario.GetText();
    if (str == "" || str == null || str == "undefined" || str == undefined)
        return;

    var res = str.split(" ");
    var dateFrom = res[1].split("/");

    var yearFrom = dateFrom[2];
    var monthFrom = dateFrom[1];
    var dayFrom = dateFrom[0];
    DateEditFechaDesde.SetDate(new Date(yearFrom, monthFrom - 1, dayFrom));

    var dateTo = res[3].split("/");
    var yearTo = dateTo[2];
    var monthTo = dateTo[1];
    var dayTo = dateTo[0];
    DateEditFechaHasta.SetDate(new Date(yearTo, monthTo - 1, dayTo));

    DateEditFechaDesde.SetMinDate(new Date(yearFrom, monthFrom - 1, dayFrom));
    DateEditFechaHasta.SetMinDate(new Date(yearFrom, monthFrom - 1, dayFrom));
    DateEditFechaDesde.SetMaxDate(new Date(yearTo, monthTo - 1, dayTo));
    DateEditFechaHasta.SetMaxDate(new Date(yearTo, monthTo - 1, dayTo));

    UpdateNamePriceList();
}

function IsValid(object) {
    return (object != null && object != undefined && object != "undefined");
}

function Validate() {

    var validate = true;
    var errors = "";

    if (!IsValid(DateEditFechaDesde) || DateEditFechaDesde.GetValue() == null) {
        errors += "Fecha Desde es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(DateEditFechaHasta) || DateEditFechaHasta.GetValue() == null) {
        errors += "Fecha Hasta es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxPeriodoCalendario) || ComboBoxPeriodoCalendario.GetValue() == null) {
        errors += "Calendario es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxTipoListaCamaron) || ComboBoxTipoListaCamaron.GetValue() == null) {
        errors += "Tipo de Camaron es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxCompradores) || ComboBoxCompradores.GetValue() == null) {
        errors += "Responsable es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (IsValid(CheckBoxEsCotizacion) && CheckBoxEsCotizacion.GetValue()) {
        if (!IsValid(ComboBoxListasPrecio) || ComboBoxListasPrecio.GetValue() == null) {
            errors += "Lista Base es un campo Obligatorio. \n\r";
            validate = false;
        }
        if ((!IsValid(RadioButtonParaProveedor) || RadioButtonParaProveedor.GetValue()) &&
            (!IsValid(ComboBoxProveedores) || ComboBoxProveedores.GetValue() == null)) {
            errors += "Proveedor es un campo Obligatorio. \n\r";
            validate = false;
        }
        if ((!IsValid(RadioButtonParaGrupo) || RadioButtonParaGrupo.GetValue()) &&
            (!IsValid(ComboBoxGrupos) || ComboBoxGrupos.GetValue() == null)) {
            errors += "Grupo es un campo Obligatorio. \n\r";
            validate = false;
        }
    }

    if (validate == false) {
        NotifyError("Error. " + errors);
    }

    return validate;
}

function SaveDataUser() {
    var userData = {
        id: $('#id_listPrice').val(),
        name: IsValid(TextBoxNombre) ? TextBoxNombre.GetText() : "",
        startDate: IsValid(DateEditFechaDesde) ? DateEditFechaDesde.GetValue() : null,
        endDate: IsValid(DateEditFechaHasta) ? DateEditFechaHasta.GetValue() : null,
        id_calendarPriceList: IsValid(ComboBoxPeriodoCalendario) ? ComboBoxPeriodoCalendario.GetValue() : 0,
        id_processtype: IsValid(ComboBoxTipoListaCamaron) ? ComboBoxTipoListaCamaron.GetValue() : 0,
        id_userResponsable: IsValid(ComboBoxCompradores) ? ComboBoxCompradores.GetValue() : 0,
        isQuotation: IsValid(CheckBoxEsCotizacion) ? CheckBoxEsCotizacion.GetValue() : false,
        id_priceListBase: IsValid(ComboBoxListasPrecio) ? ComboBoxListasPrecio.GetValue() : 0,
        byGroup: IsValid(RadioButtonParaGrupo) ? (RadioButtonParaGrupo.GetValue() === null ? false : RadioButtonParaGrupo.GetValue()) : false,
        id_provider: IsValid(ComboBoxProveedores) ? ComboBoxProveedores.GetValue() : 0,
        id_groupPersonByRol: IsValid(ComboBoxGrupos) ? ComboBoxGrupos.GetValue() : 0,
        id_certification: IsValid(ComboBoxCertificaciones) ? ComboBoxCertificaciones.GetValue() : null,

        priceListDetails: [],
        priceListPenalty: []
    };
    //  
    var priceAux = 0.00;
    var commissionAux = 0.00;
    var price_PCAux = 0.00;
    var price_RFAux = 0.00;
    var cpPriceListCountAux = 0;
    if ($("#code_processtype").val() === "ENT") {
        try {
            //cpPriceListCountAux = GridViewPriceListDetailsENT.pageRowCount - GridViewPriceListDetailsENT.pageCount;
            for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsENT.pageRowCount; rowDetail++) {
                if (GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
                for (var i = 0; i < GridViewPriceListDetailsENT.cpOrderClassShrimp.length; i++) {
                    switch (i) {
                        case 0:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 4)).toFixed(4);
                            break;
                        case 1:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 11)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 12)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 13)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 10)).toFixed(4);
                            break;
                        case 2:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 17)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 18)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 19)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 16)).toFixed(4);
                            break;
                        case 3:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 23)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 24)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 25)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 22)).toFixed(4);
                            break;
                        case 4:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 29)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 30)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 31)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 28)).toFixed(4);
                            break;
                        case 5:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 35)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 36)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 37)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 34)).toFixed(4);
                            break;
                        case 6:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 41)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 42)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 43)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 40)).toFixed(4);
                            break;
                        case 7:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 47)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 48)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 49)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 46)).toFixed(4);
                            break;
                        case 8:
                            priceAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 53)).toFixed(4);
                            commissionAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 54)).toFixed(4);
                            price_PCAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 55)).toFixed(4);
                            price_RFAux = parseFloat(GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 52)).toFixed(4);
                            break;
                        default:
                            break;
                    }
                    userData.priceListDetails.push({
                        id: GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 0),
                        id_class: GridViewPriceListDetailsENT.batchEditApi.GetCellValue(rowDetail, 1),
                        //name: GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 2),
                        price: priceAux,
                        code_classShrimp: GridViewPriceListDetailsENT.cpOrderClassShrimp[i].Value,
                        commission: commissionAux,
                        price_PC: price_PCAux,
                        price_RF: price_RFAux
                        //basePrice: parseFloat(GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(4),
                        //distint: GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 6),
                    });
                }
            }
        } catch (e) {
            //Nada
        }
    }

      
    //for (let rowDetail = 1; rowDetail <= GridViewPriceListDetailsCOL.cpPriceListCount + 1; rowDetail++) {
    for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOL.pageRowCount; rowDetail++) {
        if (GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
        for (var j = 0; j < GridViewPriceListDetailsCOL.cpOrderClassShrimp.length; j++) {
            switch (j) {
                case 0:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 4)).toFixed(4);
                    break;
                case 1:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 11)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 12)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 13)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 10)).toFixed(4);
                    break;
                case 2:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 17)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 18)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 19)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 16)).toFixed(4);
                    break;
                case 3:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 23)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 24)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 25)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 22)).toFixed(4);
                    break;
                case 4:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 29)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 30)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 31)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 28)).toFixed(4);
                    break;
                case 5:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 35)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 36)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 37)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 34)).toFixed(4);
                    break;
                case 6:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 41)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 42)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 43)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 40)).toFixed(4);
                    break;
                case 7:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 47)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 48)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 49)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 46)).toFixed(4);
                    break;
                case 8:
                    priceAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 53)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 54)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 55)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 52)).toFixed(4);
                    break;
                default:
                    break;
            }
            userData.priceListDetails.push({
                id: GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 0),
                id_class: GridViewPriceListDetailsCOL.batchEditApi.GetCellValue(rowDetail, 1),
                //name: GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 2),
                price: priceAux,
                code_classShrimp: GridViewPriceListDetailsCOL.cpOrderClassShrimp[j].Value,
                commission: commissionAux,
                price_PC: price_PCAux,
                price_RF: price_RFAux
                //basePrice: parseFloat(GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(4),
                //distint: GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 6),
            });
        }
    }

    for (let rowDetail = 0; rowDetail < GridViewPriceListDetailsCOLB.pageRowCount; rowDetail++) {
        if (GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
        for (var k = 0; k < GridViewPriceListDetailsCOLB.cpOrderClassShrimp.length; k++) {
            switch (k) {
                case 0:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 6)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 7)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 4)).toFixed(4);
                    break;
                case 1:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 11)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 12)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 13)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 10)).toFixed(4);
                    break;
                case 2:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 17)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 18)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 19)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 16)).toFixed(4);
                    break;
                case 3:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 23)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 24)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 25)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 22)).toFixed(4);
                    break;
                case 4:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 29)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 30)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 31)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 28)).toFixed(4);
                    break;
                case 5:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 35)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 36)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 37)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 34)).toFixed(4);
                    break;
                case 6:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 41)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 42)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 43)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 40)).toFixed(4);
                    break;
                case 7:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 47)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 48)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 49)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 46)).toFixed(4);
                    break;
                case 8:
                    priceAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 53)).toFixed(4);
                    commissionAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 54)).toFixed(4);
                    price_PCAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 55)).toFixed(4);
                    price_RFAux = parseFloat(GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 52)).toFixed(4);
                    break;
                default:
                    break;
            }
            userData.priceListDetails.push({
                id: GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 0),
                id_class: GridViewPriceListDetailsCOLB.batchEditApi.GetCellValue(rowDetail, 1),
                //name: GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 2),
                price: priceAux,
                code_classShrimp: GridViewPriceListDetailsCOLB.cpOrderClassShrimp[k].Value,
                commission: commissionAux,
                price_PC: price_PCAux,
                price_RF: price_RFAux
                //basePrice: parseFloat(GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 5)).toFixed(4),
                //distint: GridViewPriceListDetails.batchEditApi.GetCellValue(rowDetail, 6),
            });
        }
    }

    for (let rowPenalty = 0; rowPenalty < GridViewPriceListPenalty.pageRowCount; rowPenalty++) {
        userData.priceListPenalty.push({
            id_classShrimp: GridViewPriceListPenalty.batchEditApi.GetCellValue(rowPenalty, 0),
            value: parseFloat(GridViewPriceListPenalty.batchEditApi.GetCellValue(rowPenalty, 3)).toFixed(4)
        });
    }

    var priceList = {
        jsonPriceList: JSON.stringify(userData)
    };

    return priceList;
}

function SaveItem() {

    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'PriceListResponsive/Save',
        type: 'post',
        data: SaveDataUser(),
        async: true,
        cache: false,
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            var id = result.Data;
            $('#id_listPrice').val(id);

            ShowCurrentItem(true);

            //hideLoading();
            NotifySuccess("Elemento Guardado Satisfactoriamente.");
        },
        error: function (result) {
            hideLoading();
        },
    });
}

function ExitItem() {
    RedirecBack();
}

function ButtonCancel_Click() {
    RedirecBack();
}

function ComboBoxPeriodoCalendario_Change(s, e) {
    setRangeDate();
    ComboBoxListasPrecio.PerformCallback();
}

function ComboBoxListasPrecio_Begin(s, e) {
    e.customArgs["startDate"] = DateEditFechaDesde.GetText();
    e.customArgs["endDate"] = DateEditFechaHasta.GetText();
}

function ComboBoxListasPrecio_Change() {

    showLoading();
	  
    if (ComboBoxListasPrecio.GetValue() !== null) {
        $.ajax({
            url: 'PriceListResponsive/InfoListaBase',
            type: 'post',
            data: {
                id_priceListBase: ComboBoxListasPrecio.GetValue()
            },
            async: true,
            cache: false,
            success: function (result) {
                ComboBoxTipoListaCamaron.SetValue(result.Data);
                UpdateNamePriceList();
                UpdateSizeDetails(true);
            },
            error: function (result) {
                hideLoading();
            },
        });
    } else {
        UpdateNamePriceList();
        UpdateSizeDetails(true);
    }
    
}

function DateEditFechaDesde_Changed() {
    DateEditFechaHasta.SetMinDate(DateEditFechaDesde.GetDate());
    if (DateEditFechaDesde.GetDate() > DateEditFechaHasta.GetDate()) {
        DateEditFechaHasta.SetDate(DateEditFechaDesde.GetDate());
    }

    UpdateNamePriceList();
}

function UpdateNamePriceList() {

    var str = ComboBoxPeriodoCalendario.GetText();
    if (str === "" || str === null || str === "undefined" || str === undefined)
        return;

    var res = str.split(" ");
    var nameCout = TextBoxNombre.GetText().split('.')[0];

    var newtext = (nameCout + ". ") +
        (CheckBoxEsCotizacion.GetValue() ? "COT-" : "REF-") +
        (CheckBoxEsCotizacion.GetValue() && ComboBoxCertificaciones.GetValue() !== null ? IdLote + "-" : "") +
        (ComboBoxTipoListaCamaron.GetText() !== "" ? ComboBoxTipoListaCamaron.GetText() + "-" : "") +
        (res[0] + " [") +
        (DateEditFechaDesde.GetText() !== "" ? DateEditFechaDesde.GetText() + "-" : "") +
        (DateEditFechaHasta.GetText() !== "" ? DateEditFechaHasta.GetText() + "]" : "");

    if (CheckBoxEsCotizacion.GetValue()) {
        if (RadioButtonParaProveedor.GetValue()) {
            newtext += "-" + ComboBoxProveedores.GetText();
        }
        else if (RadioButtonParaGrupo.GetValue()) {
            newtext += "-" + ComboBoxGrupos.GetText();
        }
    }

    TextBoxNombre.SetText(newtext);
    hideLoading();
}

function CheckBoxEsCotizacion_Click() {

    showLoading();

    UpdateNamePriceList();
    UpdatePenalityDetails();

    if (CheckBoxEsCotizacion.GetValue()) {

        VisibleCotizacion(true);

        RadioButtonParaProveedor.SetValue(false);
        RadioButtonParaGrupo.SetValue(true);

        RadioProveedoresGrupo_Click();

        var userData = {
            id_processtype: ComboBoxTipoListaCamaron.GetValue(),
            isQuotation: true,
            id_priceListBase: ComboBoxListasPrecio.GetValue()
        };

        document.getElementById("divComboBoxTipoListaCamaron").style.display = "none";
        document.getElementById("divLabelComboBoxTipoListaCamaron").style.display = "none";
    }
    else {
        VisibleCotizacion(false);

        ComboBoxListasPrecio.SetValue(null);
        ComboBoxProveedores.SetValue(null);
        ComboBoxGrupos.SetValue(null);

        RadioButtonParaProveedor.SetValue(false);
        RadioButtonParaGrupo.SetValue(false);
        RadioProveedoresGrupo_Click();

        var id_processtypeAux = ComboBoxTipoListaCamaron.GetValue();
        var userData2 = {
            id_processtype: id_processtypeAux,
            isQuotation: false,
            id_priceListBase: null,
            code_processtype: "COL"
        };
        if (id_processtypeAux !== null) {
            $.ajax({
                url: 'PriceListResponsive/GetCodeProcessType',
                type: 'post',
                data: { id_processtype: id_processtypeAux },
                async: true,
                cache: false,
                success: function (result) {
                    $("#code_processtype").val(result.code_processtype);
                    if (result.code_processtype === "ENT") {
                        UpdateSizeDetailsENT(userData2, false);
                    } else {
                        UpdateSizeDetailsCOL(userData2, false);
                    }
                },
                error: function (result) {
                    hideLoading();
                }
            });
        } else {
            UpdateSizeDetailsCOL(userData2, false);
        }
        document.getElementById("divComboBoxTipoListaCamaron").style.display = "block";
        document.getElementById("divLabelComboBoxTipoListaCamaron").style.display = "block";
    }

    //$.ajax({
    //    url: 'PriceListResponsive/GridViewPriceListDetails',
    //    type: 'post',
    //    data: userData,
    //    async: true,
    //    cache: false,
    //    success: function (result) {
    //        $("#divGridViewPriceListDetails").html(result);
    //        hideLoading();
    //    },
    //    error: function (result) {
    //        hideLoading();
    //    },
    //});
}

function RadioProveedoresGrupo_Click() {

    if (RadioButtonParaProveedor.GetValue()) {
        ComboBoxGrupos.SetValue(null);
        document.getElementById("divLabelProveedor").style.display = "block";
        document.getElementById("divComboBoxProveedores").style.display = "block";

        document.getElementById("divLabelGrupo").style.display = "none";
        document.getElementById("divComboBoxGrupos").style.display = "none";

        document.getElementById("divGruposEye").style.display = "none";

    }
    else {
        ComboBoxProveedores.SetValue(null);
        document.getElementById("divLabelProveedor").style.display = "none";
        document.getElementById("divComboBoxProveedores").style.display = "none";

        document.getElementById("divLabelGrupo").style.display = "block";
        document.getElementById("divComboBoxGrupos").style.display = "block";

        document.getElementById("divGruposEye").style.display = "block";
    }

    UpdatePenalityDetails();
}

function UpdateSizeDetailsENT(userData, byPriceListBase) {
    userData.code_processtype = "ENT";
    $.ajax({
        url: 'PriceListResponsive/GridViewPriceListDetailsENT',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        success: function (result) {
            $("#divGridViewPriceListDetailsENT").html(result);
            UpdateSizeDetailsCOL(userData, byPriceListBase);
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function UpdateSizeDetailsCOL(userData, byPriceListBase) {
    userData.code_processtype = "COL";
    $.ajax({
        url: 'PriceListResponsive/GridViewPriceListDetailsCOL',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        success: function (result) {
            $("#divGridViewPriceListDetailsCOL").html(result);
            UpdateSizeDetailsCOLB(userData, byPriceListBase);
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function UpdateSizeDetailsCOLB(userData, byPriceListBase) {
    userData.code_processtype = "COL";
    $.ajax({
        url: 'PriceListResponsive/GridViewPriceListDetailsCOLB',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        success: function (result) {
            $("#divGridViewPriceListDetailsCOLB").html(result);
            //if (!byPriceListBase)
            //hideLoading();
        },
        error: function (result) {
            hideLoading();
        },
        complete: function (result) {
            UpdateDetailsCOLB();
            hideLoading();
        }
    });
}

function UpdateSizeDetails(byPriceListBase) {
    var id_processtypeAux = ComboBoxTipoListaCamaron.GetValue();
    var userData = {
        id_processtype: id_processtypeAux,
        isQuotation: CheckBoxEsCotizacion.GetValue(),
        id_priceListBase: ComboBoxListasPrecio.GetValue(),
        code_processtype: "COL"
    };
    if (id_processtypeAux !== null) {
        $.ajax({
            url: 'PriceListResponsive/GetCodeProcessType',
            type: 'post',
            data: { id_processtype: id_processtypeAux },
            async: true,
            cache: false,
            success: function (result) {
                var aDivGridViewPriceListDetailsENT = document.getElementById("divGridViewPriceListDetailsENT");
                if (result.code_processtype === "ENT") {
                    aDivGridViewPriceListDetailsENT.style.display = "block";
                    pcFeatures.tabs[0].SetText("Precio por Talla Entero    ");
                    pcFeatures.tabs[0].SetEnabled(true);
                    $("#code_processtype").val("ENT");
                    UpdateSizeDetailsENT(userData, byPriceListBase);
                } else {
                    aDivGridViewPriceListDetailsENT.style.display = "none";
                    pcFeatures.tabs[0].SetText("");
                    pcFeatures.tabs[0].SetEnabled(false);
                    $("#code_processtype").val("COL");
                    UpdateSizeDetailsCOL(userData, byPriceListBase);
                }
            },
            error: function (result) {
                hideLoading();
            }
        });
    } else {
        var aDivGridViewPriceListDetailsENT = document.getElementById("divGridViewPriceListDetailsENT");
        aDivGridViewPriceListDetailsENT.style.display = "none";
        pcFeatures.tabs[0].SetText("");
        pcFeatures.tabs[0].SetEnabled(false);
        UpdateSizeDetailsCOL(userData, byPriceListBase);
    }


}

function ComboBoxTipoListaCamaron_Change() {

    showLoading();
    UpdateNamePriceList();

    UpdateSizeDetails(false);
}

function UpdatePenalityDetails() {

    showLoading();

    userData = null;
    if (RadioButtonParaProveedor.GetValue()) {
        userData = {
            id_provider: ComboBoxProveedores.GetValue(),
            id_group: null,
            enabled: $("#enabled").val(),
            IsOwner: $("#IsOwner").val(),
            isQuotation: CheckBoxEsCotizacion.GetValue()
        };
    }
    else if (RadioButtonParaGrupo.GetValue()) {
        userData = {
            id_provider: null,
            id_group: ComboBoxGrupos.GetValue(),
            enabled: $("#enabled").val(),
            IsOwner: $("#IsOwner").val(),
            isQuotation: CheckBoxEsCotizacion.GetValue()
        };
    }
    else {
        userData = {
            id_provider: null,
            id_group: null,
            enabled: $("#enabled").val(),
            IsOwner: $("#IsOwner").val(),
            isQuotation: CheckBoxEsCotizacion.GetValue()
        };
    }

    $.ajax({
        url: 'PriceListResponsive/GridViewPriceListPenalty',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        success: function (result) {
            $("#divGridViewPriceListPenalty").html(result);
        },
        error: function (result) {
            hideLoading();
        },
        complete: function (result) {
            UpdateDetailsCOLB();
            hideLoading();
        }
    });
}

function ComboBoxGrupos_Change() {
    UpdatePenalityDetails();
    UpdateNamePriceList();
}

function ComboBoxProveedores_Change() {
    UpdatePenalityDetails();
    UpdateNamePriceList();
}

var IdLote = "";
function ComboBoxCertificaciones_Change() {
    var id_certificationAux = ComboBoxCertificaciones.GetValue();
    if (id_certificationAux !== null) {
        $.ajax({
            url: 'PriceListResponsive/GetIdLoteCertificacion',
            type: 'post',
            data: { id_certification: id_certificationAux },
            async: true,
            cache: false,
            success: function (result) {
                IdLote = result.IdLote;
                UpdateNamePriceList();
            },
            error: function (result) {
                hideLoading();
            }
        });
    } else {
        IdLote = "";
        UpdateNamePriceList();
    }


}

function CheckBoxEsCotizacion_OnInit() {
    if (CheckBoxEsCotizacion.GetValue()) {
        document.getElementById("divComboBoxTipoListaCamaron").style.display = "none";
        document.getElementById("divLabelComboBoxTipoListaCamaron").style.display = "none";
    }
    else {
        document.getElementById("divComboBoxTipoListaCamaron").style.display = "block";
        document.getElementById("divLabelComboBoxTipoListaCamaron").style.display = "block";
    }
}

$(function () {
    init();

    $("#btnGroupEye").click(function () {

        showLoading();

        $.ajax({
            url: 'PriceListResponsive/ProvidersByGroup',
            type: 'post',
            data: { id_grupo: ComboBoxGrupos.GetValue(), id_priceList: $("#id_listPrice").val() },
            async: true,
            cache: false,
            success: function (result) {
                if (result.Code !== 0) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                    return;
                }

                hideLoading();
                var result = DevExpress.ui.dialog.alert(result.Data, "Proveedores");
            },
            error: function (result) {
                hideLoading();
            },
        });

    });
});
