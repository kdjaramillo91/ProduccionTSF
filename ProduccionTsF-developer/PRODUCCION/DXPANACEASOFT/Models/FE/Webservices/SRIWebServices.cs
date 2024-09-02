using System;
using System.IO;
using System.Net;
using System.Xml;
using Newtonsoft.Json;

namespace DXPANACEASOFT.Models.FE.Webservices
{
    public class SRIWebServices
    {
        public class RespuestaSRI
        {
            public string Estado { get; set; }
            public string ClaveAcceso { get; set; }
            public string ErrorIdentificador { get; set; }
            public string ErrorMensaje { get; set; }
            public string ErrorInfoAdicional { get; set; }
            public string ErrorTipo { get; set; }
            public string NumeroAutorizacion { get; set; }
            public string FechaAutorizacion { get; set; }
            public string Ambiente { get; set; }
            public RespuestaSRI() { }

        }

        private class XMLHelper
        {

            public static XmlDocument ConvertStringToDocument(string xml_string)
            {
                XmlDocument result = new XmlDocument();
                result.LoadXml(xml_string);

                return result;
            }

            public static XmlDocument ConvertFileToDocument(string file_path)
            {
                XmlDocument result = new XmlDocument();
                result.Load(file_path);

                return result;
            }


            /// <summary>
            /// Convierte el documento en string Base64
            /// </summary>
            /// <param name="file_path">Ruta del archivo a aconvertir</param>
            public static string ConvertToBase64String(string file_path)
            {
                byte[] binarydata = File.ReadAllBytes(file_path);
                return Convert.ToBase64String(binarydata, 0, binarydata.Length);
            }

            private static string GetNodeValue(string rootNodo, string infoNodo, XmlDocument doc)
            {
                string result = null;
                string node_path = "//" + rootNodo + "//" + infoNodo;

                XmlNode node = doc.SelectSingleNode(node_path);

                if (node != null)
                {
                    result = node.InnerText;
                }

                return result;
            }

            /// <summary>
            /// Devuelve la respuesta de la solicitud de recepción del comprobante en una estructura detallada.
            /// </summary>
            /// <param name="xml_doc">Documento XML de respuesta</param>
            public static RespuestaSRI GetRespuestaRecepcion(XmlDocument xml_doc)
            {
                RespuestaSRI result = new RespuestaSRI();
                result.Estado = GetNodeValue("RespuestaRecepcionComprobante", "estado", xml_doc);

                if (result.Estado != "RECIBIDA")
                {
                    result.ClaveAcceso = GetNodeValue("comprobante", "claveAcceso", xml_doc);
                    result.ErrorIdentificador = GetNodeValue("mensaje", "identificador", xml_doc);
                    result.ErrorMensaje = GetNodeValue("mensaje", "mensaje", xml_doc);
                    result.ErrorInfoAdicional = GetNodeValue("mensaje", "informacionAdicional", xml_doc);
                    result.ErrorTipo = GetNodeValue("mensaje", "tipo", xml_doc);
                }

                return result;
            }

            /// <summary>
            /// Devuelve la respuesta de la solicitud de autorización del comprobante en una estructura detallada.
            /// </summary>
            /// <param name="xml_doc">Documento XML de respuesta</param>
            public static RespuestaSRI GetRespuestaAutorizacion(XmlDocument xml_doc)
            {
                RespuestaSRI result = new RespuestaSRI();
                string pathLevelAutorizacion = "RespuestaAutorizacionComprobante/autorizaciones/autorizacion[last()]";
                string pathLevelMensajes = "RespuestaAutorizacionComprobante/autorizaciones/autorizacion/mensajes[last()]/mensaje";

                result.Estado = GetNodeValue(pathLevelAutorizacion, "estado", xml_doc);

                if (result.Estado == "AUTORIZADO")
                {
                    result.NumeroAutorizacion = GetNodeValue(pathLevelAutorizacion, "numeroAutorizacion", xml_doc);
                    result.FechaAutorizacion = GetNodeValue(pathLevelAutorizacion, "fechaAutorizacion", xml_doc);
                    result.Ambiente = GetNodeValue(pathLevelAutorizacion, "ambiente", xml_doc);
                }
                else if (result.Estado == "NO AUTORIZADO")
                {
                    result.FechaAutorizacion = GetNodeValue(pathLevelAutorizacion, "fechaAutorizacion", xml_doc);
                    result.Ambiente = GetNodeValue(pathLevelAutorizacion, "ambiente", xml_doc);
                    result.ErrorIdentificador = GetNodeValue(pathLevelMensajes, "identificador", xml_doc);
                    result.ErrorMensaje = GetNodeValue(pathLevelMensajes, "mensaje", xml_doc);
                    result.ErrorTipo = GetNodeValue(pathLevelMensajes, "tipo", xml_doc);
                }

                return result;
            }

            public static string GetInfoTributaria(string info, XmlDocument xml_doc)
            {
                return GetNodeValue("infoTributaria", info, xml_doc);
            }

            public static string GetInfoFactura(string info, XmlDocument xml_doc)
            {
                return GetNodeValue("infoFactura", info, xml_doc);
            }

        }

        private class WSHelper
        {
            public static string URL_Envio;
            public static string URL_Autorizacion;
            public static string RutaXML;
            public static string ClaveAcceso;


            private static string xmlEnvioRequestTemplate =
              String.Concat(
              "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
              " <SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"",
              " xmlns:ns1=\"http://ec.gob.sri.ws.recepcion\">",
              "  <SOAP-ENV:Body>",
              "    <ns1:validarComprobante>",
              "      <xml>{0}</xml>",
              "    </ns1:validarComprobante>",
              "  </SOAP-ENV:Body>",
              "</SOAP-ENV:Envelope>");

            private static string xmlAutorizacionRequestTemplate =
              String.Concat(
              "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
              " <SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"",
              " xmlns:ns1=\"http://ec.gob.sri.ws.autorizacion\">",
              "  <SOAP-ENV:Body>",
              "    <ns1:autorizacionComprobante>",
              "      <claveAccesoComprobante>{0}</claveAccesoComprobante>",
              "    </ns1:autorizacionComprobante>",
              "  </SOAP-ENV:Body>",
              "</SOAP-ENV:Envelope>");


            /// <summary>
            /// Envía el xml firmado a los webs services del SRI para su recepción.
            /// </summary>
            public static RespuestaSRI EnvioComprobante()
            {
                RespuestaSRI result = null;
                string ws_url = URL_Envio;

                //Codifica el archivo a base 64
                string bytesEncodeBase64 = XMLHelper.ConvertToBase64String(RutaXML);

                //Crea el request del web service
                HttpWebRequest request = CreateWebRequest(ws_url, "POST");

                //Arma la cadena xml ara el envío al web service
                string stringRequest = string.Format(xmlEnvioRequestTemplate, bytesEncodeBase64);

                //Convierte la cadena en un documeto xml
                XmlDocument xmlRequest = XMLHelper.ConvertStringToDocument(stringRequest);

                //Crea un flujo de datos (stream) y escribe el xml en la solicitud de respuesta del web service
                using (Stream stream = request.GetRequestStream())
                {
                    xmlRequest.Save(stream);
                }

                //Obtiene la respuesta del web service
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        //Lee el flujo de datos (stream) de respuesta del web service
                        string soapResultStr = rd.ReadToEnd();

                        //Convierte la respuesta de string a xml para extraer el detalle de la respuesta del web service
                        XmlDocument soapResultXML = XMLHelper.ConvertStringToDocument(soapResultStr);

                        //Obtiene la respuesta detallada
                        result = XMLHelper.GetRespuestaRecepcion(soapResultXML);
                    }
                }

                return result;
            }

            /// <summary>
            /// Envía la clave de acceso a los webs services del SRI para consultar ele estado de autorización.
            /// </summary>
            public static RespuestaSRI AutorizacionComprobante(out XmlDocument xml_doc)
            {
                RespuestaSRI result = null;
                string ws_url = URL_Autorizacion;

                //Crea el request del web service
                HttpWebRequest request = CreateWebRequest(ws_url, "POST");

                //Arma la cadena xml ara el envío al web service
                string stringRequest = string.Format(xmlAutorizacionRequestTemplate, ClaveAcceso);

                //Convierte la cadena en un documeto xml
                XmlDocument xmlRequest = XMLHelper.ConvertStringToDocument(stringRequest);
                xml_doc = xmlRequest;

                //Crea un flujo de datos (stream) y escribe el xml en la solicitud de respuesta del web service
                using (Stream stream = request.GetRequestStream())
                {
                    xmlRequest.Save(stream);
                }

                //Obtiene la respuesta del web service
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        //Lee el flujo de datos (stream) de respuesta del web service
                        string soapResultStr = rd.ReadToEnd();

                        //Convierte la respuesta de string a xml para extraer el detalle de la respuesta del web service
                        XmlDocument soapResultXML = XMLHelper.ConvertStringToDocument(soapResultStr);

                        //Obtiene la respuesta detallada
                        result = XMLHelper.GetRespuestaAutorizacion(soapResultXML);
                    }
                }

                return result;
            }

            /// <summary>
            /// Crea y devuelve una instancia de objeto para la solicitud de respuesta desde una URI.
            /// </summary>
            /// <param name="uri">URI del recurso de internet</param>
            /// <param name="method">Método de solicitud</param>
            public static HttpWebRequest CreateWebRequest(string uri, string method)
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Headers.Add("SOAP:Action");
                webRequest.ContentType = "application/soap+xml;charset=utf-8";
                webRequest.Accept = "text/xml";
                webRequest.Method = method;

                return webRequest;
            }

            
        }

        public static string EnviarComprobante(string signedXml)
        {
            WSHelper.RutaXML = signedXml;
            WSHelper.URL_Envio = @"https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantes?wsdl";
            RespuestaSRI respuesta = WSHelper.EnvioComprobante();
            return JsonConvert.SerializeObject(respuesta);
        }

        public static string AutorizarComprobante(string claveAcceso)
        {
            WSHelper.ClaveAcceso = claveAcceso;
            WSHelper.URL_Autorizacion = @"https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantes?wsdl";
            XmlDocument doc = new XmlDocument();
            RespuestaSRI respuesta = WSHelper.AutorizacionComprobante(out doc);
            return JsonConvert.SerializeObject(respuesta);
        }
    }
}