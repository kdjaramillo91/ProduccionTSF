using BibliotecaReporte.Dataset;
using BibliotecaReporte.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Auxiliares
{
    internal class CompaniaInfoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_CompaniaInfo";

        public static CompaniaInfoDataSet PrepareLogo(SqlConnection sqlConnection)
        {
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            var detailsResult = sqlConnection.Query<CompaniaInfoModel>(m_StoredProcedureName, null, commandType: CommandType.StoredProcedure)
                    .ToList();

            foreach (var detailResult in detailsResult)
            {
                var cabeceraRow = rptCompaniaInfo.CompaniaInfo.NewCompaniaInfoRow();
                cabeceraRow.Codigo = detailResult.Codigo;
                cabeceraRow.Nombre = detailResult.Nombre;
                cabeceraRow.NombreComercial = detailResult.NombreComercial;
                cabeceraRow.Logo = detailResult.Logo;
                cabeceraRow.Direccion = detailResult.Direccion;
                cabeceraRow.Correo = detailResult.Correo;
                cabeceraRow.NumeroTelefono = detailResult.NumeroTelefono;
                rptCompaniaInfo.CompaniaInfo.AddCompaniaInfoRow(cabeceraRow);
            }

            return rptCompaniaInfo;
        }
    }
}
