using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
 
namespace Utilitarios.Logs
{
    public static class MetodosEscrituraLogs
    {
        private static string rutaLog = "";
        const int MAX_DEPTH = 5;

        private static void verificaruta(string ruta)
        {
            try
            {
                try
                {

                    rutaLog = ruta;
                }
                catch (Exception)
                {

                    rutaLog = AppDomain.CurrentDomain.BaseDirectory;
                }

                if (!Directory.Exists(rutaLog))
                {
                    Directory.CreateDirectory(rutaLog);
                }


            }
            catch (Exception ex)
            {
                throw new DataException("Error : " + ex.Message.ToString());
            }
        }

        private static void extractInnerException(Exception e, ref string mensaje, ref int depth, string fechaError)
        {
            if (e == null) return;

            mensaje += $"L:{depth}-{e.Message} {Environment.NewLine}";
            
            if (e.Source != null) 
            {
                mensaje += $"{fechaError} : Exception Source: {e.Source.ToString().Trim()} {Environment.NewLine}";
            }
            if (e.Message != null)
            {
                mensaje += $"{fechaError} : Exception Message: {e.Message.ToString().Trim()} {Environment.NewLine}";
            }
            if (e.StackTrace != null)
            {
                mensaje += $"{fechaError} : Exception StackTrace: {e.StackTrace.ToString().Trim()} {Environment.NewLine}";
            }
            if (e.GetType().Name == "EntityException")
            {
                var entityException = (EntityException)e;
                if (entityException.Data != null)
                {
                    string dataError = $"Informacion Entity Exception {Environment.NewLine}";
                    foreach (DictionaryEntry data in entityException.Data)
                    {
                        dataError += $"KEY: {data.Key}, VALOR: {data.Value} {Environment.NewLine}";

                    }

                    if (!string.IsNullOrEmpty(dataError))
                    {
                        mensaje += dataError;
                    }
                }
            }
            //if (e.TargetSite != null)
            //{
            //    mensaje += $"{fechaError} : Exception TargetSite: {e.TargetSite.ToString().Trim()} {Environment.NewLine}";
            //}

            depth++;
             if (e.InnerException == null) return;
             if (depth == MAX_DEPTH) return;
             if (e.InnerException != null) extractInnerException(e.InnerException, ref mensaje, ref depth, fechaError);
        }


        public static string GetExcepcionNestMessage(Exception ex)
        {
            string fechaError = DateTime.Now.ToString();
            string mensaje = string.Empty;
            int depth = 1;

            

            extractInnerException(ex, ref mensaje, ref depth, fechaError);


            return mensaje;

        }

        public static void EscribeExcepcionLogNest( Exception ex, 
                                                    string ruta, 
                                                    string origen, 
                                                    string app,
                                                    string seccion = null, 
                                                    string extraInfo = null,
                                                    [CallerFilePath] string callFilePath = "",
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerLineNumber] int lineNumber = 0
                                                    
            )
        {
            StreamWriter sw = null;
            try
            {
                verificaruta(ruta);
                //EventLog(app, ex.Message.ToString());
                string fileLog = "\\" + origen + DateTime.Now.ToString("yyyyMMdd").ToString() + ".txt";
                string fechaError = DateTime.Now.ToString();

                sw = new StreamWriter(rutaLog + fileLog, true);
                string mensaje = string.Empty;
                int depth = 1;

                if (!string.IsNullOrEmpty(callFilePath))
                {
                    mensaje += $"File Exception Path:{callFilePath}{Environment.NewLine}";
                }
                if (!string.IsNullOrEmpty(memberName))
                {
                    mensaje += $"Method Exception:{memberName}{Environment.NewLine}";
                }
                if (lineNumber !=0)
                {
                    mensaje += $"Line Exception:{lineNumber}{Environment.NewLine}";
                }

                extractInnerException(ex, ref mensaje, ref depth, fechaError);
                if (!string.IsNullOrEmpty(seccion))
                {
                    mensaje += $"{fechaError} : Seccion Error: {seccion} {Environment.NewLine}";
                }
                if (!string.IsNullOrEmpty(extraInfo))
                {
                    mensaje += $"{fechaError} : Extra Info: {extraInfo} {Environment.NewLine}";
                }

                sw.WriteLine(mensaje);

            }
            catch(Exception e)
            {
                EventLog(app, e.Message.ToString());
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// Escribe Source, Message, StackTrace y TargetSite de un Objeto excepción en un archivo de texto.
        /// Hay que enviarle la ruta.
        /// </summary>
        /// <param name="ex">Recibe el Objeto Exception</param>
        /// <param name="ruta">Se define la ruta de los archivos</param>
        /// <param name="origen">Se define la opción en donde se origina el problema (puede ser el nombre de un método)</param>
        /// <param name="app">Se define el Nombre de la aplicación</param>
        public static void EscribeExcepcionLog(Exception ex, string ruta, string origen, string app)
        {
            StreamWriter sw = null;
            try
            {
                verificaruta(ruta);
                EventLog(app, ex.Message.ToString());
                string fileLog = "\\" + origen + DateTime.Now.ToString("yyyyMMdd").ToString() + ".txt";
                string fechaError = DateTime.Now.ToString();

                sw = new StreamWriter(rutaLog + fileLog, true);

                if (ex.Source != null)
                {
                    sw.WriteLine(fechaError + ": Exception Source: " + ex.Source.ToString().Trim());
                }
                if (ex.Message != null)
                {
                    sw.WriteLine(fechaError + ": Exception Message: " + ex.Message.ToString().Trim());
                }
                if (ex.StackTrace != null)
                {
                    sw.WriteLine(fechaError + ": Exception StackTrace: " + ex.StackTrace.ToString().Trim());
                }
                if (ex.TargetSite != null)
                {
                    sw.WriteLine(fechaError + ": Exception TargetSite: " + ex.TargetSite.ToString().Trim());
                }

                sw.Flush();
                sw.Close();
            }
            catch
            {
                sw.Flush();
                sw.Close();
                EventLog(app, ex.Message.ToString());
            }
        }

        public static void EscribeMensajeLog(string Mensaje, string ruta, string origen, string app)
        {
            StreamWriter sw = null;
            try
            {
                verificaruta(ruta);
                EventLog(app, Mensaje);
                string fileLog = "\\" + origen + DateTime.Now.ToString("yyyyMMdd").ToString() + ".txt";
                //string fileLog = "vic.txt";
                sw = new StreamWriter(rutaLog + fileLog, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Mensaje);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                sw.Flush();
                sw.Close();
                EventLog(app, Mensaje);
            }
        }

        public static void LogWrite(Exception e, string Mensaje, string ruta, string origen, string app, string xtraInfo)
        {

            // xtraInfo : Informacion de Programa + Metodo/Funcion

            StreamWriter sw = null;
            try
            {
                verificaruta(ruta);
                string fechaError = DateTime.Now.ToString();
                string fileLog = "\\" + origen + DateTime.Now.ToString("yyyyMMdd").ToString() + ".txt";
                sw = new StreamWriter(rutaLog + fileLog, true);
                
                if (e != null)
                {

                    EventLog(app, e.Message);
                    sw.WriteLine(String.Concat(Enumerable.Repeat("-", 40)));
                    sw.WriteLine(DateTime.Now.ToString() + ": Exception Stack:");
                    sw.WriteLine(DateTime.Now.ToString() + "          ====> "+ xtraInfo);

                    if (e.Source != null)
                    {
                        sw.WriteLine(fechaError + ": Exception Source: " + e.Source.ToString().Trim());
                    }
                    if (e.Message != null)
                    {
                        sw.WriteLine(fechaError + ": Exception Message: " + e.Message.ToString().Trim());
                    }
                    if (e.StackTrace != null)
                    {
                        sw.WriteLine(fechaError + ": Exception StackTrace: " + e.StackTrace.ToString().Trim());
                    }
                    if (e.TargetSite != null)
                    {
                        sw.WriteLine(fechaError + ": Exception TargetSite: " + e.TargetSite.ToString().Trim());
                    }

                      

                    int maxDepthError = 10;
                    Exception eDepth = e;
                    for (int i = maxDepthError; i >= 0; i--)
                    {
                        if (eDepth.InnerException != null)
                        {
                            sw.WriteLine(fechaError + ": Inner Exception: " + eDepth.InnerException.Message);                            
                            eDepth = eDepth.InnerException;
                        }
                        else
                        {
                            break;
                        }

                    }

                    sw.WriteLine(": Exception Stack End:");
                    sw.WriteLine(String.Concat(Enumerable.Repeat("-", 40)));
                }
                else
                {

                    sw.WriteLine(DateTime.Now.ToString() + ": "+xtraInfo+" :" + Mensaje);
                }

                
                sw.Flush();
                sw.Close();
            }
            catch
            {
                sw.Flush();
                sw.Close();
                EventLog(app, Mensaje);
            }
        }

        private static void EventLog(string app, string Mensaje)
        {
            try
            {
                System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();

                if (!System.Diagnostics.EventLog.SourceExists(app))
                {
                    System.Diagnostics.EventLog.CreateEventSource(app, "Application");
                }

                eventLog.Source = app;
                eventLog.WriteEntry(Mensaje, System.Diagnostics.EventLogEntryType.Error);
                eventLog.Close();
            }
            catch //(Exception ex)
            {

            }
        }


    }

}
