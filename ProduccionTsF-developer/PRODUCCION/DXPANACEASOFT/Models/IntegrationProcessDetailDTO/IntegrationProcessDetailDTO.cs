
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Models.IntegrationProcessDetailDTO
{
    public class IntegrationProcessDetailFmiDTO
    {
        public static List<IntegrationProcessDetailFmiDTO> ListIntegrationProcessDetailFmiDTO { get; set; }

        public static IEnumerable<IntegrationProcessDetailFmiDTO> GetListIntegrationProcessDetailDTO(IEnumerable<IntegrationProcessDetail> details)
        {
            var detailsByProviders = new Dictionary<int, List<IntegrationProcessDetail>>();
            foreach (var detail in details)
            {
                var idProvider = detail.Document.LiquidationMaterialSupplies.idProvider;
                if (!detailsByProviders.ContainsKey(idProvider))
                {
                    detailsByProviders.Add(idProvider, new List<IntegrationProcessDetail> { detail });
                }
                else
                {
                    detailsByProviders[idProvider].Add(detail);
                }
            }

            ListIntegrationProcessDetailFmiDTO = new List<IntegrationProcessDetailFmiDTO>();
            foreach (var element in detailsByProviders)
            {
                ListIntegrationProcessDetailFmiDTO.AddRange(GetListByProvider(element.Value));
            }

            return ListIntegrationProcessDetailFmiDTO;
        }

        private static IEnumerable<IntegrationProcessDetailFmiDTO> GetListByProvider(IEnumerable<IntegrationProcessDetail> details)
        {
            var notaCredito = new IntegrationProcessDetailFmiDTO
            {
                Id = int.Parse("1" + details.First().Document.id.ToString()),
                TipoDocumento = "Nota de Credito",
                DetailsInfoFactProducto = new List<InfoFactProducto>()
            };
            var factura = new IntegrationProcessDetailFmiDTO
            {
                Id = int.Parse("2" + details.First().Document.id.ToString()),
                TipoDocumento = "Factura",
                DetailsInfoFactProducto = new List<InfoFactProducto>()
            };

            var result = new List<IntegrationProcessDetailFmiDTO>();

            foreach (var detail in details)
            {
                var liquidationDetail = detail.Document.LiquidationMaterialSupplies.LiquidationMaterialSuppliesDetail.ToList();

                var listInfoFactProduct = liquidationDetail.GroupBy(l => l.idItem).Select(e => new InfoFactProducto
                {
                    Item = e.First().Item,
                    CodPrincipal = "",
                    CodAuxiliar = "",
                    IdProducto = e.First().idItem,
                    Producto = e.First().Item.name,
                    PrecioUnitario = e.First().priceUnit,
                    NoLiquidacion = detail.Document.sequential,
                    Total = e.Sum(c => c.total),
                    Cantidad = e.Sum(c => c.quantity),
                    SubTotal = e.Sum(c => c.subTotal),
                    Iva = 0,
                }).ToList();


                foreach (var infoFactProduct in listInfoFactProduct)
                {
                    if (infoFactProduct.Cantidad > 0)
                    {
                        factura.TipoDocumento = "01";
                        factura.Proveedor = detail.Document.LiquidationMaterialSupplies.Provider;
                        factura.FechaEmision = detail.dateCreate;

                        if (string.IsNullOrEmpty(factura.Descripcion))
                        {
                            factura.Descripcion = "Proveedor: " + detail.Document.LiquidationMaterialSupplies.Provider
                                                      .Person.fullname_businessName + " No. Liquidaciones: " + detail.Document.sequential;
                        }
                        else if (!factura.Descripcion.Contains(detail.Document.sequential.ToString()))
                        {
                            factura.Descripcion += ", " + detail.Document.sequential;
                        }

                        factura.ValorDocumento += infoFactProduct.Total;
                        factura.NumeroLote = detail.IntegrationProcess.codeLote;
                        factura.EstadoLote = detail.IntegrationProcess.IntegrationState.name;
                        factura.FechaContabilizacion = detail.dateCreate;

                        factura.DetailsInfoFactProducto.Add(infoFactProduct);
                    }
                    else if (infoFactProduct.Cantidad < 0)
                    {
                        notaCredito.TipoDocumento = "04";
                        notaCredito.Proveedor = detail.Document.LiquidationMaterialSupplies.Provider;
                        notaCredito.FechaEmision = detail.dateCreate;

                        if (string.IsNullOrEmpty(notaCredito.Descripcion))
                        {
                            notaCredito.Descripcion = "Proveedor: " + detail.Document.LiquidationMaterialSupplies.Provider
                                                      .Person.fullname_businessName + " No. Liquidaciones: " + detail.Document.sequential;
                        }
                        else if (!notaCredito.Descripcion.Contains(detail.Document.sequential.ToString()))
                        {
                            notaCredito.Descripcion += ", " + detail.Document.sequential;
                        }

                        notaCredito.ValorDocumento += infoFactProduct.Total;
                        notaCredito.NumeroLote = detail.IntegrationProcess.codeLote;
                        notaCredito.EstadoLote = detail.IntegrationProcess.IntegrationState.name;
                        notaCredito.FechaContabilizacion = detail.dateCreate;

                        notaCredito.DetailsInfoFactProducto.Add(infoFactProduct);
                    }
                }
            }

            if(factura.ValorDocumento != 0)
                result.Add(factura);
            if (notaCredito.ValorDocumento != 0)
                result.Add(notaCredito);
            return result;
        }

        public int Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Descripcion { get; set; }
        public decimal ValorDocumento { get; set; }
        public string NumeroLote { get; set; }
        public string EstadoLote { get; set; }
        public string TipoDocumento { get; set; }
        public DateTime FechaContabilizacion { get; set; }
        public Provider Proveedor { get; set; }
        public List<InfoFactProducto> DetailsInfoFactProducto { get; set; }
    }

    public class InfoFactProducto
    {
        public Item Item { get; set; }
        public string CodPrincipal { get; set; }
        public string CodAuxiliar { get; set; }
        public int IdProducto { get; set; }
        public string Producto { get; set; }
        public int NoLiquidacion { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }
}