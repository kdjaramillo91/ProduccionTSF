using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderLogicalOperator
    {
        private static DBContext db = null;

        public static IEnumerable LogicalOperators()
        {
            db = new DBContext();
            var model = new List<LogicalOperator>();

            model.Add(new LogicalOperator
            {
                id = 1,
                code = "=",
                name = "="
            });
            model.Add(new LogicalOperator
            {
                id = 2,
                code = ">",
                name = ">"
            });
            model.Add(new LogicalOperator
            {
                id = 3,
                code = "<",
                name = "<"
            });
            model.Add(new LogicalOperator
            {
                id = 4,
                code = "!=",
                name = "!="
            });
            model.Add(new LogicalOperator
            {
                id = 5,
                code = ">=",
                name = ">="
            });
            model.Add(new LogicalOperator
            {
                id = 6,
                code = "<=",
                name = "<="
            });
            return model;
        }
        public static LogicalOperator LogicalOperatorById(int? id_logicalOperator)
        {
            List<LogicalOperator> model = (List<LogicalOperator>)LogicalOperators();
            foreach (var m in model)
            {
                if (m.id == id_logicalOperator)
                    return m;
            }
            return null;
        }
        public static IEnumerable LogicalOperatorDateNumbers()
        {
            db = new DBContext();
            var model = new List<LogicalOperator>();

            model.Add(new LogicalOperator
            {
                id = 1,
                code = "==",
                name = "="
            });
            model.Add(new LogicalOperator
            {
                id = 2,
                code = ">",
                name = ">"
            });
            model.Add(new LogicalOperator
            {
                id = 3,
                code = "<",
                name = "<"
            });
            model.Add(new LogicalOperator
            {
                id = 4,
                code = "!=",
                name = "!="
            });
            model.Add(new LogicalOperator
            {
                id = 5,
                code = ">=",
                name = ">="
            });
            model.Add(new LogicalOperator
            {
                id = 6,
                code = "<=",
                name = "<="
            });
            model.Add(new LogicalOperator
            {
                id = 7,
                code = "Rango",
                name = "Rango"
            });
            return model;
        }


        public static IEnumerable LogicalBoleam()
        {
            db = new DBContext();
            var model = new List<LogicalOperator>();

            model.Add(new LogicalOperator
            {
                id = 1,
                code = "True",
                name = "SI"
            });
            model.Add(new LogicalOperator
            {
                id = 2,
                code = "False",
                name = "No"
            });
       
            return model;
        }

        public static LogicalOperator LogicalOperatorDateNumbersById(int? id_logicalOperator)
        {
            List<LogicalOperator> model = (List<LogicalOperator>)LogicalOperatorDateNumbers();
            foreach (var m in model)
            {
                if (m.id == id_logicalOperator)
                    return m;
            }
            return null;
        }
        public static IEnumerable LogicalOperatorTexts()
        {
            db = new DBContext();
            var model = new List<LogicalOperator>();

            model.Add(new LogicalOperator
            {
                id = 1,
                code = "Igual",
                name = "Igual"
            });
            model.Add(new LogicalOperator
            {
                id = 2,
                code = "Distinto",
                name = "Distinto"
            });
            model.Add(new LogicalOperator
            {
                id = 3,
                code = "Contiene",
                name = "Contiene"
            });
            model.Add(new LogicalOperator
            {
                id = 4,
                code = "No Contiene",
                name = "No Contiene"
            });
            model.Add(new LogicalOperator
            {
                id = 5,
                code = "Comienza",
                name = "Comienza"
            });
            model.Add(new LogicalOperator
            {
                id = 6,
                code = "Termina",
                name = "Termina"
            });
            return model;
        }

        public static LogicalOperator LogicalOperatorTextsById(int? id_logicalOperator)
        {
            List<LogicalOperator> model = (List<LogicalOperator>)LogicalOperatorTexts();
            foreach (var m in model)
            {
                if (m.id == id_logicalOperator)
                    return m;
            }
            return null;
        }
        public static IEnumerable LogicalOperatorSelects()
        {
            db = new DBContext();
            var model = new List<LogicalOperator>();

            model.Add(new LogicalOperator
            {
                id = 1,
                code = "Contiene",
                name = "Contiene"
            });
            model.Add(new LogicalOperator
            {
                id = 2,
                code = "No Contiene",
                name = "No Contiene"
            });
            return model;
        }

        public static LogicalOperator LogicalOperatorSelectsById(int? id_logicalOperator)
        {
            List<LogicalOperator> model = (List<LogicalOperator>)LogicalOperatorSelects();
            foreach (var m in model)
            {
                if (m.id == id_logicalOperator)
                    return m;
            }
            return null;
        }

        public static LogicalOperator LogicalBoleambyID(int? id_logicalOperator)
        {
            List<LogicalOperator> model = (List<LogicalOperator>)LogicalBoleam();
            foreach (var m in model)
            {
                if (m.id == id_logicalOperator)
                    return m;
            }
            return null;
        }
    }
}