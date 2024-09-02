using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using System.Web;

namespace DXPANACEASOFT.Auxiliares
{
    public static class FilterBuilder
    {

        public enum FilterOperation
        {

            Equal,
            DoesNotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual,
            Contains
        };


        public static Filter<O> BuildFilter<O>(string fieldName, object expression, Type expressionType, FilterOperation operation )
        {
            ParameterExpression selectedRow = Expression.Parameter(typeof(O), "selectedRow");
            MemberExpression field  = Expression.Property(selectedRow, fieldName);

            var converter = TypeDescriptor.GetConverter(expressionType);
            var result = converter.ConvertFrom(expression);
            ConstantExpression expr = Expression.Constant(result, expressionType);
            Expression notNull = Expression.NotEqual(field, Expression.Constant(null, field.Type));
            Expression op;

            switch (operation)
            {
                case FilterOperation.LessThan:
                    op = Expression.AndAlso(notNull, Expression.LessThan(field, expr));
                    break;
                case FilterOperation.LessThanOrEqual:
                    op = Expression.AndAlso(notNull, Expression.LessThanOrEqual(field, expr));
                    break;
                case FilterOperation.GreaterThan:
                    op = Expression.AndAlso(notNull, Expression.GreaterThan(field, expr));
                    break;
                case FilterOperation.GreaterThanOrEqual:
                    op = Expression.AndAlso(notNull, Expression.GreaterThanOrEqual(field, expr));
                    break;
                case FilterOperation.Contains:
                    Type stringType = typeof(string);
                    MethodInfo method = stringType.GetMethod("Contains", new[] { stringType });
                    op = Expression.AndAlso(notNull, Expression.Call(field, method, expr));
                    break;
                case FilterOperation.DoesNotEqual:
                    op = Expression.NotEqual(field, expr);
                    break;
                default:
                    op = Expression.Equal(field, expr);
                    break;
            }

            var fe = Expression.Lambda<Func<O, bool>>(op, new ParameterExpression[] { selectedRow }).Compile();

            return new Filter<O>(fe, operation, fieldName, expression);
        }


        public class Filter<O>
        {
            public Func<O, bool> FilterExpression { get; set; }
            public FilterOperation Operation { get; set; }
            public string Fieldname { get; set; }
            public object Expression { get; set; }

            public Filter(Func<O, bool> fe, FilterOperation fo, string fn, object ex)
            {
                this.FilterExpression = fe;
                this.Operation = fo;
                this.Fieldname = fn;
                this.Expression = ex;
            }

            public override string ToString()
            {
                return string.Format("'{0}' {1} '{2}'", Fieldname, Operation, Expression);
            }
        }
    }
}