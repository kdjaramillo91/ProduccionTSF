using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
// using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.ModelExtension;

namespace DXPANACEASOFT.Models.GenericProcess
{
    internal static class AdvanceParametersOperationExtension
    {

        internal static IList<EntityParameters> FindParameters(this DbSet<AdvanceParameters> thisObj, string codeAdvanceParameters)
        {


            IList <AdvanceParametersDetail> advanceParametersDetailList = thisObj
                                                                            .FirstOrDefault(r => r.code == codeAdvanceParameters)
                                                                            .AdvanceParametersDetail
                                                                            .ToList();

            IList<EntityParameters> returnFindParameters = advanceParametersDetailList
                                                                            .Select( s=> new EntityParameters
                                                                                    {
                                                                                        code = s.valueCode,
                                                                                        valueString = s.valueName,
                                                                                        valueDate = s.valueDate,
                                                                                        valueDecimal = s.valueDecimal,
                                                                                        valueInteger = s.valueInteger,
                                                                                        valueVarchar = s.valueVarchar
                                                                                    }).ToList();
                                                                
                                                                
            return returnFindParameters;
        }



    }
}