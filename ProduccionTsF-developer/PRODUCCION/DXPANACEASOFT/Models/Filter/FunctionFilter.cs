using System;
using System.Collections.Generic;
using System.Linq;
//using System.Linq.Dynamic;
using System.Reflection;
using System.Web;

namespace DXPANACEASOFT.Models.Filter
{
    public static class FunctionFilter
    {
        //public FunctionFilter()
        //{
        //    //this.octcodigo = "";
        //    //this.ListFilterType = new List<FilterType>();
        //    //this.ListFilterText = new List<FilterText>();
        //    //this.ListFilterNumber = new List<FilterNumber>();
        //    //this.ListFilterSelect = new List<FilterSelect>();
        //}


        //public static IQueryable<T> ExecuteFilter<T>(this IQueryable<T> model, FilterTypeWithCondition filterTypeWithCondition)
        //{
        //    var filterTypeName = filterTypeWithCondition.filterType.name;
        //    string[] separators = { "."};
        //    //string value = "The handsome, energetic, young dog was playing with his smaller, more lethargic litter mate.";
        //    string[] words = filterTypeName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        //    var nameNotNull = "";
        //    var wordCurrent = "";
        //    foreach (var word in words)
        //    {
        //        if (nameNotNull == "")
        //        {
        //            wordCurrent = word;
        //            nameNotNull = (word + "!= null");
        //        }else
        //        {
        //            wordCurrent += "." + word;
        //            nameNotNull += (" && " + wordCurrent + "!= null");
        //        }
        //    }
        //    //Console.WriteLine(word);

           

        //    if (filterTypeWithCondition.filterType.type == "Date")
        //    {
        //        #region Date

        //        //if (startEmissionDate != null && endEmissionDate != null)
        //        //{
        //        if (filterTypeWithCondition.logicalOperator.code == ">" && filterTypeWithCondition.logicalOperator.code == "<")
        //        {
        //            //model.OrderBy("Category.CategoryName, UnitPrice descending");
        //            model = model.Where((nameNotNull + " && " +
        //                                 filterTypeWithCondition.filterType.name + " " + filterTypeWithCondition.logicalOperator.code + " @0"), filterTypeWithCondition.valueConditionFromDateTime.Value.Date);
        //            //model = model.Where(("DateTime.Compare(@0,@1) @2 0"), detail.filterType.name, detail.valueConditionFromDateTime.ToString(), detail.logicalOperator.code);
        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "==")
        //        {
        //            var valueDateMoreADay = filterTypeWithCondition.valueConditionFromDateTime.Value.Date.AddDays(1);
        //            model = model.Where((nameNotNull + " && " +
        //                                 filterTypeWithCondition.filterType.name + " >= @0 &&" + filterTypeWithCondition.filterType.name + " < @1"), filterTypeWithCondition.valueConditionFromDateTime.Value.Date, valueDateMoreADay);

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "!=")
        //        {
        //            var valueDateMoreADay = filterTypeWithCondition.valueConditionFromDateTime.Value.Date.AddDays(1);
        //            model = model.Where((nameNotNull + " && " +
        //                                 "!(" + filterTypeWithCondition.filterType.name + " >= @0 &&" + filterTypeWithCondition.filterType.name + " < @1" + ")"), filterTypeWithCondition.valueConditionFromDateTime.Value.Date, valueDateMoreADay);

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == ">=")
        //        {
        //            var valueDateMoreADay = filterTypeWithCondition.valueConditionFromDateTime.Value.Date.AddDays(1);
        //            model = model.Where((nameNotNull + " && " +
        //                                 "(" + filterTypeWithCondition.filterType.name + " > @0" + " || (" + filterTypeWithCondition.filterType.name + " >= @0 &&" + filterTypeWithCondition.filterType.name + " < @1" + "))"), filterTypeWithCondition.valueConditionFromDateTime.Value.Date, valueDateMoreADay);

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "<=")
        //        {
        //            var valueDateMoreADay = filterTypeWithCondition.valueConditionFromDateTime.Value.Date.AddDays(1);
        //            model = model.Where((nameNotNull + " && " +
        //                                 "(" + filterTypeWithCondition.filterType.name + " < @0" + " || (" + filterTypeWithCondition.filterType.name + " >= @0 &&" + filterTypeWithCondition.filterType.name + " < @1" + "))"), filterTypeWithCondition.valueConditionFromDateTime.Value.Date, valueDateMoreADay);

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "Rango")
        //        {
        //            var valueDateMoreADay = filterTypeWithCondition.valueConditionFromDateTime.Value.Date.AddDays(1);
        //            model = model.Where((nameNotNull + " && " +
        //                                 "(" + filterTypeWithCondition.filterType.name + " > @0" + " || (" + filterTypeWithCondition.filterType.name + " >= @0 &&" + filterTypeWithCondition.filterType.name + " < @1" + "))"), filterTypeWithCondition.valueConditionFromDateTime.Value.Date, valueDateMoreADay);
        //            valueDateMoreADay = filterTypeWithCondition.valueConditionToDateTime.Value.Date.AddDays(1);
        //            model = model.Where((nameNotNull + " && " +
        //                                 "(" + filterTypeWithCondition.filterType.name + " < @0" + " || (" + filterTypeWithCondition.filterType.name + " >= @0 &&" + filterTypeWithCondition.filterType.name + " < @1" + "))"), filterTypeWithCondition.valueConditionToDateTime.Value.Date, valueDateMoreADay);

        //        }
        //        #endregion
        //    }
        //    else
        //    if(filterTypeWithCondition.filterType.type == "Text")
        //    {
        //        #region Text

        //        if (filterTypeWithCondition.logicalOperator.code == "Igual")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                model = model.Where((nameNotNull + " && " +
        //                                 "(" + filterTypeWithCondition.filterType.name + " == @0)"), filterTypeWithCondition.valueConditionTextOrSelect);
        //            }
        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "Distinto")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                model = model.Where((nameNotNull + " && " +
        //                                 "(" + filterTypeWithCondition.filterType.name + " != @0)"), filterTypeWithCondition.valueConditionTextOrSelect);
        //            }
        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "Contiene")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                model = model.Where((nameNotNull + " && " +
        //                                 "(" + filterTypeWithCondition.filterType.name + ".ToLower().Contains(@0))"), filterTypeWithCondition.valueConditionTextOrSelect.ToLower());
        //            }

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "No Contiene")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                model = model.Where((nameNotNull + " && " +
        //                                 "!(" + filterTypeWithCondition.filterType.name + ".ToLower().Contains(@0))"), filterTypeWithCondition.valueConditionTextOrSelect.ToLower());
        //            }else
        //            {
        //                var modelAux = model.ToList();
        //                for (int i = modelAux.Count - 1; i >= 0; i--)
        //                {
        //                    var detail = modelAux.ElementAt(i);

        //                    modelAux.Remove(detail);
        //                }
        //                model = modelAux.AsQueryable(); 
        //            }

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "Comienza")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                model = model.Where((nameNotNull + " && " + "(" + filterTypeWithCondition.filterType.name + ".Length >= @0 && " +
        //                                     filterTypeWithCondition.filterType.name + ".Substring(0,@0).ToLower() == @1)"), filterTypeWithCondition.valueConditionTextOrSelect.Length, filterTypeWithCondition.valueConditionTextOrSelect.ToLower());
        //            }
        //            //else
        //            //{
        //            //    var modelAux = model.ToList();
        //            //    for (int i = modelAux.Count - 1; i >= 0; i--)
        //            //    {
        //            //        var detail = modelAux.ElementAt(i);

        //            //        modelAux.Remove(detail);
        //            //    }
        //            //    model = modelAux.AsQueryable();
        //            //}

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "Termina")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                model = model.Where((nameNotNull + " && " + "(" + filterTypeWithCondition.filterType.name + ".Length >= @0 && " +
        //                                     filterTypeWithCondition.filterType.name + ".Substring((" + filterTypeWithCondition.filterType.name + ".Length - @0),@0).ToLower() == @1)"), filterTypeWithCondition.valueConditionTextOrSelect.Length, filterTypeWithCondition.valueConditionTextOrSelect.ToLower());
        //            }
        //            //else
        //            //{
        //            //    var modelAux = model.ToList();
        //            //    for (int i = modelAux.Count - 1; i >= 0; i--)
        //            //    {
        //            //        var detail = modelAux.ElementAt(i);

        //            //        modelAux.Remove(detail);
        //            //    }
        //            //    model = modelAux.AsQueryable();
        //            //}

        //        }
        //        #endregion
        //    }
        //    else
        //    if (filterTypeWithCondition.filterType.type == "Number")
        //    {
        //        #region Number

        //        //if (startEmissionDate != null && endEmissionDate != null)
        //        //{
        //        if (filterTypeWithCondition.logicalOperator.code != "Rango")
        //        {
        //            //model.OrderBy("Category.CategoryName, UnitPrice descending");
        //            if (filterTypeWithCondition.valueConditionFromDecimal != null)
        //            {
        //                model = model.Where((nameNotNull + " && " +
        //                                 filterTypeWithCondition.filterType.name + " " + filterTypeWithCondition.logicalOperator.code + " @0"), filterTypeWithCondition.valueConditionFromDecimal.Value);
        //            }
        //            //model = model.Where(("DateTime.Compare(@0,@1) @2 0"), detail.filterType.name, detail.valueConditionFromDateTime.ToString(), detail.logicalOperator.code);
        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "Rango")
        //        {
        //            if(filterTypeWithCondition.valueConditionFromDecimal != null)
        //            {
        //                model = model.Where((nameNotNull + " && " +
        //                                filterTypeWithCondition.filterType.name + " >= @0"), filterTypeWithCondition.valueConditionFromDecimal.Value);
        //            }
        //            if (filterTypeWithCondition.valueConditionToDecimal != null)
        //            {
        //                model = model.Where((nameNotNull + " && " +
        //                                filterTypeWithCondition.filterType.name + " <= @0"), filterTypeWithCondition.valueConditionToDecimal.Value);
        //            }
        //        }
        //        #endregion
        //    }
        //    else
        //    if (filterTypeWithCondition.filterType.type == "Select")
        //    {
        //        #region Select

        //        if (filterTypeWithCondition.logicalOperator.code == "Contiene")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                //var valueConditionTextOrSelectAux = filterTypeWithCondition.valueConditionTextOrSelect;
        //                //string[] selectSeparators = { ",", "[", "]", " " };
        //                ////string value = "The handsome, energetic, young dog was playing with his smaller, more lethargic litter mate.";
        //                //string[] selectWords = valueConditionTextOrSelectAux.Split(selectSeparators, StringSplitOptions.RemoveEmptyEntries);
        //                var newModel = model;
        //                newModel = null;
        //                foreach (var sw in filterTypeWithCondition.valueConditionSelectValue)
        //                {
        //                    if (newModel == null)
        //                    {
        //                        newModel = model.Where((nameNotNull + " && " +
        //                        "(" + filterTypeWithCondition.filterType.name + " == @0)"), sw);
        //                    }
        //                    else
        //                    {
        //                        var modelAux = model.Where((nameNotNull + " && " +
        //                       "(" + filterTypeWithCondition.filterType.name + " == @0)"), sw).ToList();
                                
        //                        if(modelAux.Count() > 0) {
        //                            var aux = newModel.ToList();
        //                            aux.AddRange(modelAux);
        //                            newModel = aux.AsQueryable();
        //                        }
        //                    }
                           
        //                    //var modelAux = model.Where((nameNotNull + " && " +
        //                    //            "(@0.Any(" + filterTypeWithCondition.filterType.name + ".ToString())"), selectWords);
        //                }
        //                model = newModel;
        //            }

        //        }
        //        else
        //        if (filterTypeWithCondition.logicalOperator.code == "No Contiene")
        //        {
        //            if (!string.IsNullOrEmpty(filterTypeWithCondition.valueConditionTextOrSelect))
        //            {
        //                //var valueConditionTextOrSelectAux = filterTypeWithCondition.valueConditionTextOrSelect;
        //                //string[] selectSeparators = { ",", "[", "]", " " };
        //                ////string value = "The handsome, energetic, young dog was playing with his smaller, more lethargic litter mate.";
        //                //string[] selectWords = valueConditionTextOrSelectAux.Split(selectSeparators, StringSplitOptions.RemoveEmptyEntries);

        //                //var newModel = model;
        //                //newModel = null;
        //                foreach (var sw in filterTypeWithCondition.valueConditionSelectValue)
        //                {
        //                    var modelAux = model.Where((nameNotNull + " && " +
        //                       "(" + filterTypeWithCondition.filterType.name + " == @0)"), sw).ToList();
        //                    foreach (var m in modelAux)
        //                    {
        //                        var aux = model.ToList();
        //                        aux.Remove(m);
        //                        model = aux.AsQueryable();
        //                    }

        //                    //var modelAux = model.Where((nameNotNull + " && " +
        //                    //            "(@0.Any(" + filterTypeWithCondition.filterType.name + ".ToString())"), selectWords);
        //                }

        //                //var listModelAux = model.ToList();
        //                //var listNewModelAux = newModel.ToList();
        //                //for (int i = listModelAux.Count - 1; i >= 0; i--)
        //                //{
        //                //    var detail = listModelAux.ElementAt(i);
        //                //    listNewModelAux.Find(detail);
        //                //    modelAux.Remove(detail);
        //                //}
        //                //listModelAux.RemoveRange(listNewModelAux);
        //                //model = model.Where((nameNotNull + " && " +
        //                //                 "!(@0.Any(" + filterTypeWithCondition.filterType.name + ".ToString())"), selectWords);
        //            }
        //            else
        //            {
        //                var modelAux = model.ToList();
        //                for (int i = modelAux.Count - 1; i >= 0; i--)
        //                {
        //                    var detail = modelAux.ElementAt(i);

        //                    modelAux.Remove(detail);
        //                }
        //                model = modelAux.AsQueryable();
        //            }

        //        }
                
        //        #endregion
        //    }
        //    else
        //    if (filterTypeWithCondition.filterType.type == "Check")
        //    {
        //        #region Check

        //        model = model.Where((nameNotNull + " && " +
        //                         "(" + filterTypeWithCondition.filterType.name + " == @0)"), filterTypeWithCondition.valueConditionCheck.Value);
        //        #endregion
        //    }

        //    return model;

        //}
    }

    public static class Invoker
    {
        public static object CreateAndInvoke(string typeName, object[] constructorArgs, string methodName, object[] methodArgs)
        {
            Type type = Type.GetType(typeName);
            object instance = Activator.CreateInstance(type, constructorArgs);

            MethodInfo method = type.GetMethod(methodName);
            return method.Invoke(instance, methodArgs);
        }
    }
}