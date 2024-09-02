using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class SimpleFormulaController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Simple Formula GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult SimpleFormulaPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.SimpleFormula.FirstOrDefault(b => b.id == keyToCopy);
            }
            var ds = DataProviderSimpleFormula.Datasources(null);
            var ls = ds as List<SimpleComboBox>;

            var model = db.SimpleFormula
                .Where(whl => whl.id_company == this.ActiveCompanyId).ToList();

            List<SimpleFormula> lsFormula = new List<SimpleFormula>();
            if (model != null)
            {
                foreach (var det in model)
                {
                    lsFormula.Add(new SimpleFormula
                    {
                        id = det.id,
                        code = det.code,
                        name = det.name,
                        description = det.description,
                        type = det.type,
                        dataSources = det.dataSources,
                        formula = det.formula,
                        formulaTranslated = det.formulaTranslated,
                        id_company = det.id_company,
                        isActive = det.isActive,
                        datasourcedescription = ls.FirstOrDefault(fod => fod.id == det.dataSources).name
                    });
                }
            }

            return PartialView("_SimpleFormulaPartial", lsFormula.OrderByDescending(ob => ob.id));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SimpleFormulaPartialAddNew(SimpleFormula item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.code = item.code;
                        item.name = item.name;
                        item.description = item.description;

                        item.type = item.type;
                        item.dataSources = item.dataSources;
                        item.formula = item.formula;
                        item.formulaTranslated = TransformarValoresFormulaEnMetadata(item.formula, item.dataSources);
                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.SimpleFormula.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Fórmula: " + item.name + " guardada exitosamente");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(ex.Message);
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.SimpleFormula.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SimpleFormulaPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SimpleFormulaPartialUpdate(SimpleFormula item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.SimpleFormula.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            //modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;

                            modelItem.type = item.type;
                            modelItem.dataSources = item.dataSources;
                            modelItem.formula = item.formula;
                            modelItem.formulaTranslated = TransformarValoresFormulaEnMetadata(item.formula, item.dataSources);
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.SimpleFormula.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Fórmula: " + item.name + " guardada exitosamente");
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(ex.Message);
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.SimpleFormula.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SimpleFormulaPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SimpleFormulaPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.SimpleFormula.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.SimpleFormula.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Fórmula: " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.SimpleFormula.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SimpleFormulaPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        [HttpPost]
        public ActionResult DeleteSelectedSimpleFormula(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.SimpleFormula.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.SimpleFormula.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Fórmulas desactivadas exitosamente");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.SimpleFormula.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SimpleFormulaPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        #endregion Simple Formula GRIDVIEW

        #region REPORT

        [HttpPost]
        public ActionResult SimpleFormulaReport()
        {
            WarehouseLocationReport report = new WarehouseLocationReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_SimpleFormulaReport", report);
        }

        #endregion REPORT

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult GetDataSourceByName(string id_datasource)
        {
            List<SimpleComboBox> ls = null;

            var result = new
            {
                isValid = false,
                lsData = ls
            };

            try
            {
                ls = DataProviders.DataProviderSimpleFormula.SimpleDataSourceDataById(id_datasource);
                result = new
                {
                    isValid = true,
                    lsData = ls
                };
            }
            catch
            {
                ls = null;
                result = new
                {
                    isValid = false,
                    lsData = ls
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion AUXILIAR FUNCTIONS

        #region Private Methods

        private string TransformarValoresFormulaEnMetadata(string formula, string dataSource = "")
        {
            string resultado = "";

            if ((string.IsNullOrEmpty(dataSource) || string.IsNullOrWhiteSpace(dataSource)))
                return resultado;

            var lsCmbSimpleFormula = DataProviders.DataProviderSimpleFormula.SimpleDataSourceDataById(dataSource);

            if (lsCmbSimpleFormula == null)
                return resultado;

            if (lsCmbSimpleFormula.Count == 0)
                return resultado;

            string formulaCopia = formula;
            string porcionFormula = "";
            int indicePrimeraAparicionFormulaInicio = -1;
            int indicePrimeraAparicionFormulaFin = -1;
            // buscar indice de coincidencia de caracte {{
            var motivos = new List<string>();
            indicePrimeraAparicionFormulaInicio = formulaCopia.IndexOf("{{");
            while (indicePrimeraAparicionFormulaInicio >= 0)
            {
                // Acumulo la primera porcion de la formula
                resultado += formulaCopia.Substring(0, (indicePrimeraAparicionFormulaInicio > 0 ? (indicePrimeraAparicionFormulaInicio) : indicePrimeraAparicionFormulaInicio));

                formulaCopia = formulaCopia.Substring(indicePrimeraAparicionFormulaInicio);

                // Buscamos Caracter Final
                indicePrimeraAparicionFormulaFin = formulaCopia.IndexOf("}}");

                if (indicePrimeraAparicionFormulaFin > 0)
                {
                    // Hacemos SubString de Espacio de Formula
                    porcionFormula = formulaCopia.Substring(0, ((indicePrimeraAparicionFormulaFin + 2) > formulaCopia.Length ? formulaCopia.Length : (indicePrimeraAparicionFormulaFin + 2)));

                    if (!(string.IsNullOrEmpty(porcionFormula) || string.IsNullOrWhiteSpace(porcionFormula)))
                    {
                        porcionFormula = porcionFormula.Replace("{{", "");
                        porcionFormula = porcionFormula.Replace("}}", "");
                        porcionFormula = porcionFormula.Trim();

                        var scmb = lsCmbSimpleFormula.FirstOrDefault(fod => fod.name.Trim() == porcionFormula.Trim());
                        if (scmb != null)
                        {
                            resultado += string.Concat("{{", scmb.id, "||", dataSource, "}}");
                        }

                        motivos.Add(porcionFormula.Trim());
                    }

                    //Quitamos esta parte de formulaCopia
                    formulaCopia = formulaCopia.Substring((indicePrimeraAparicionFormulaFin + 2));
                    formulaCopia = formulaCopia.Trim();
                }

                indicePrimeraAparicionFormulaInicio = formulaCopia.IndexOf("{{");
            }

            if (!(string.IsNullOrEmpty(formulaCopia) || string.IsNullOrWhiteSpace(formulaCopia)))
            {
                resultado = string.Concat(resultado, formulaCopia);
            }

            // verificamos que no existan motivos repetidos
            var motivosRepetidos = motivos
                .GroupBy(e => e)
                .Select(e => new
                {
                    motivo = e.Key,
                    numero = e.Count(),
                })
                .Where(e => e.numero > 1)
                .ToList();

            if (motivosRepetidos.Any())
            {
                throw new Exception("Existen motivos de inventarios repetidos dentro de la formulación." +
                    $"Motivos: {string.Join(", ", motivosRepetidos.Select(e => e.motivo))}.");
            }

            return resultado;
        }

        #endregion Private Methods
    }
}