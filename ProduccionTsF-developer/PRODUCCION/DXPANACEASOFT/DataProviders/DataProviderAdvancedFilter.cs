using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderAdvancedFilter
    {
        private static DBContext db = null;
        public static IEnumerable CheckDataSource()
        {
            db = new DBContext();
            var model = new List<LogicalOperator>();

            model.Add(new LogicalOperator
            {
                id = 1,
                code = "1",
                name = "Verdadero"
            });
            model.Add(new LogicalOperator
            {
                id = 2,
                code = "0",
                name = "Falso"
            });
            return model;
        }
        public static IEnumerable Logical()
        {
            db = new DBContext();
            var model = new List<LogicalOperator>();

            model.Add(new LogicalOperator
            {
                id = 1,
                code = "Or",
                name = "O"
            });
            model.Add(new LogicalOperator
            {
                id = 2,
                code = "And",
                name = "Y"
            });
            model.Add(new LogicalOperator
            {
                id = 3,
                code = "",
                name = "Fin"
            });
            return model;
        }
        public static LogicalOperator LogicalOperatorById(int? id_logicalOperator)
        {
            List<LogicalOperator> model = (List<LogicalOperator>)Logical();
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

        public static LogicalOperator LogicalOperatorDateNumbersById(int? id_logicalOperator)
        {
            List<LogicalOperator> model = (List<LogicalOperator>)LogicalOperatorDateNumbers();
            foreach (var m in model)
            {
                if(m.id == id_logicalOperator)
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
    }
}