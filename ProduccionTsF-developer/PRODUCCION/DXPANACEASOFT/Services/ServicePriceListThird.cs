using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Utilitarios.Logs;
using System.Configuration;

using DXPANACEASOFT.Models.FE;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.DataProviders;
using XmlGuiaRemision = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.GuiaRemision2;

using System.Xml.Serialization;

using System.Xml;
using System.Xml.Linq;
using System.Text;
using AccesoDatos.MSSQL;

namespace DXPANACEASOFT.Services
{
    public class ServicePriceListThird
    {
        public static void HomologationUpdatePriceList(string cadenaConexion, string rutaLog)
        {
            //Replica Listas
            MetodosDatos.EjecutarProcesoProcedimientoAlmacenado("pap_ReplicaListasTerceros", cadenaConexion, rutaLog, "PriceListThird", "PROD");
            //Homologa Parte 0
            MetodosDatos.EjecutarProcesoProcedimientoAlmacenado("pap_HomologaListaPrecios", cadenaConexion, rutaLog, "PriceListThird", "PROD");
            //Homologa Parte 1
            MetodosDatos.EjecutarProcesoProcedimientoAlmacenado("pap_HomologaListaPrecios_1", cadenaConexion, rutaLog, "PriceListThird", "PROD");
            //Homologa Parte 2
            MetodosDatos.EjecutarProcesoProcedimientoAlmacenado("pap_HomologaListaPrecios_2", cadenaConexion, rutaLog, "PriceListThird", "PROD");
            //Homologa Parte 3
            MetodosDatos.EjecutarProcesoProcedimientoAlmacenado("pap_HomologaListaPrecios_3", cadenaConexion, rutaLog, "PriceListThird", "PROD");
        }
    }
}