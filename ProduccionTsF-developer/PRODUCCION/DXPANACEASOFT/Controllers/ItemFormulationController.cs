using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ItemFormulationController : DefaultController
    {
        [HttpPost, ValidateInput(false)]
        public ActionResult Index()
        {
            this.SetCommonViewBagData();
            return PartialView();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Query(int? id_itemOrigen, TipoBusquedaProducto tipoBusquedaProducto, bool isCallBack = false)
        {
            List<Item> query = new List<Item>();
            List<Item> queryAux = new List<Item>();

            // Busca los productos de acuerdo a los parámetros
            query = tipoBusquedaProducto == TipoBusquedaProducto.SinFormulacion 
                ? db.Item.Where(e => e.id_company == this.ActiveCompanyId && !e.hasFormulation && e.isActive).ToList() 
                : db.Item.Where(e => e.id_company == this.ActiveCompanyId && e.hasFormulation && e.isActive).ToList();

            // Recorre los elementos, descartando los elementos que tienen los mismos al producto origen
            var itemIngredientOrigin = db.Item.FirstOrDefault(e => e.id == id_itemOrigen).ItemIngredient;
            
            foreach (var itemAceptar in query)
            {
                bool esIngredienteIgual = itemIngredientOrigin.SequenceEqual(itemAceptar.ItemIngredient);

                if (!esIngredienteIgual)
                {
                    queryAux.Add(itemAceptar);
                }
            }

            //Permisos de usuario
            this.SetCommonViewBagData();

            this.ViewBag.QueryCriteria = new Dictionary<string, object>()
            {
                { "ItemOrigen", id_itemOrigen },
                { "TipoBusquedaProducto", tipoBusquedaProducto },
            };

            // Retorna la vista
            return this.PartialView("_ItemFormulationCopyGridViewPartial", queryAux);
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ItemFormulationPartial(int? id_itemOrigen, TipoBusquedaProducto tipoBusquedaProducto)
        //{
        //    List<Item> query = new List<Item>();
        //    List<Item> queryAux = new List<Item>();
        //    Item tempItem = (TempData["item"] as Item);
        //    if (tempItem != null)
        //    {
        //        TempData.Remove("item");
        //    }

        //    // Busca los productos de acuerdo a los parámetros
        //    query = tipoBusquedaProducto == TipoBusquedaProducto.SinFormulacion
        //        ? db.Item.Where(e => e.id_company == this.ActiveCompanyId && !e.hasFormulation && e.isActive).ToList()
        //        : db.Item.Where(e => e.id_company == this.ActiveCompanyId && e.hasFormulation && e.isActive).ToList();

        //    // Recorre los elementos, descartando los elementos que tienen los mismos al producto origen
        //    var itemIngredientOrigin = db.Item.FirstOrDefault(e => e.id == id_itemOrigen).ItemIngredient;

        //    foreach (var itemAceptar in query)
        //    {
        //        bool esIngredienteIgual = itemIngredientOrigin.SequenceEqual(itemAceptar.ItemIngredient);

        //        if (!esIngredienteIgual)
        //        {
        //            queryAux.Add(itemAceptar);
        //        }
        //    }

        //    //Permisos de usuario
        //    this.SetCommonViewBagData();
        //    this.ViewBag.QueryCriteria = new Dictionary<string, object>()
        //    {
        //        { "ItemOrigen", id_itemOrigen },
        //        { "TipoBusquedaProducto", tipoBusquedaProducto },
        //    };
        //    return PartialView("_ItemFormulationCopyGridViewPartial", queryAux);
        //}

        [HttpPost, ValidateInput(false)]
        public ActionResult CopyFormulationSelectedItem(int? idItemOrigen, TipoBusquedaProducto tipoBusquedaProducto, int[] idsFormulationItems)
        {
            // Genero i nuevo modelo
            var _copyItemHeadFormulation = new ItemHeadIngredient();
            var _copyItemFormulation = new List<ItemIngredient>();

            // datos a devolver para la vista 
            bool isValid = false;
            string message = string.Empty;

            // Busco la información del ítem de orígen
            var itemOrigen = db.Item.FirstOrDefault(e => e.id == idItemOrigen);

            if (itemOrigen != null  && (idsFormulationItems != null && idsFormulationItems.Length > 0))
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Busco la cabecera del item a copiar fórmula en la db
                        _copyItemHeadFormulation = db.Item.FirstOrDefault(e => e.id == itemOrigen.id).ItemHeadIngredient;

                        // Busco las formulaciones del item a copiar 
                        _copyItemFormulation = db.Item.FirstOrDefault(e => e.id == itemOrigen.id).ItemIngredient.ToList();

                        if(!_copyItemFormulation.Any())
                        {
                            throw new Exception("Producto de origen sin fórmulas a replicar");
                        }

                        //Busco los productos a reemplazar fórmula
                        var itemsCopied = db.Item.Where(e => idsFormulationItems.Contains(e.id));

                        foreach (var item in itemsCopied)
                        {
                            // Activo el check del producto a replicar
                            if (tipoBusquedaProducto == TipoBusquedaProducto.SinFormulacion && !item.hasFormulation)
                                item.hasFormulation = true;

                            #region Actualización de cabecera de producto

                            if (item.ItemHeadIngredient == null)
                            {
                                item.ItemHeadIngredient = new ItemHeadIngredient
                                {
                                    id_Item = _copyItemHeadFormulation.id_Item,
                                    amount = _copyItemHeadFormulation.amount,
                                    id_metricUnit = _copyItemHeadFormulation.id_metricUnit,
                                };

                                db.ItemHeadIngredient.Attach(item.ItemHeadIngredient);
                                db.Entry(item.ItemHeadIngredient).State = EntityState.Added;
                            }
                            else
                            {
                                item.ItemHeadIngredient.amount = _copyItemHeadFormulation.amount;
                                item.ItemHeadIngredient.id_metricUnit = _copyItemHeadFormulation.id_metricUnit;

                                db.ItemHeadIngredient.Attach(item.ItemHeadIngredient);
                                db.Entry(item.ItemHeadIngredient).State = EntityState.Modified;
                            }

                            #endregion

                            #region Actualización de detalles de producto

                            // Elimino los ingredientes anteriores
                            if (item.ItemIngredient.Any())
                            {
                                foreach (var ingredienteEliminar in item.ItemIngredient.ToList())
                                {
                                    db.ItemIngredient.Remove(ingredienteEliminar);
                                    db.Entry(ingredienteEliminar).State = EntityState.Deleted;
                                } 
                            }

                            // Creo los nuevos ingredientes
                            foreach (var ingredient in _copyItemFormulation)
                            {

                                var tempIngredient = new ItemIngredient
                                {
                                    id_compoundItem = item.id,
                                    id_ingredientItem = ingredient.id_ingredientItem,
                                    id_metricUnit = ingredient.id_metricUnit,
                                    amount = ingredient.amount,
                                    id_metricUnitMax = ingredient.id_metricUnitMax,
                                    amountMax = ingredient.amountMax,
                                    manual = ingredient.manual,
                                    id_costumerItem = ingredient.id_costumerItem,
                                };

                                db.ItemIngredient.Add(tempIngredient);
                                db.Entry(tempIngredient).State = EntityState.Added;
                            }

                            #endregion

                            // Modifico los datos de auditoria
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;                            

                            db.Item.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            // guardo los cambios 
                            db.SaveChanges();
                        }

                        isValid = true;
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage($"Fórmula de producto copiado exitosamente");
                        message = "Fórmula del Producto: " + itemOrigen.masterCode + " copiado exitosamente";
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        isValid = false;
                        ViewData["EditError"] = ex.Message;
                        ViewData["EditMessage"] = ErrorMessage();
                        message = ex.GetBaseException().Message;
                    }
                }
            }

            var result = new
            {
                isValid,
                message,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Diccionarios para Tipo de producto
        public static Dictionary<TipoBusquedaProducto, string> GetTipoBusquedaReporte()
        {
            return new Dictionary<TipoBusquedaProducto, string>()
            {
                { TipoBusquedaProducto.SinFormulacion, "Producto sin formulación" },
                { TipoBusquedaProducto.ConFormulacion, "Producto con formulación" },
            };
        }

        public enum TipoBusquedaProducto
        {
            SinFormulacion,
            ConFormulacion
        } 
        #endregion
    }
}