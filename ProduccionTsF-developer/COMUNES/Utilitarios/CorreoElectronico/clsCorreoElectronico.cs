using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using Utilitarios.Logs;
using System.Threading.Tasks;

namespace Utilitarios.CorreoElectronico
{
    public static class clsCorreoElectronico
    {

        /// <summary>
        /// Método para enviar correo electronico
        /// </summary>
        /// <param name="mailTo">Correos Destinatario</param>
        /// <param name="mailFrom">Correo de Envio</param>
        /// <param name="asunto">Asunto para el correo</param>
        /// <param name="smtpHost">Host SMTP</param>
        /// <param name="puerto">Numero de puerto</param>
        /// <param name="contraseña">Credenciales correo envio</param>
        /// <param name="cuerpoMensaje">Estructura mensaje</param>
        /// <param name="delimitador">Delimitador Correos Destinatarios</param>
        /// <returns></returns>
        public static string EnviarCorreoElectronico(   string mailTo, 
                                                        string mailFrom, 
                                                        string asunto,
                                                        string smtpHost, 
                                                        int puerto, 
                                                        string contraseña, 
                                                        string cuerpoMensaje, 
                                                        char delimitador, 
                                                        Attachment adjunto = null, 
                                                        string bCC = "",
                                                        string rutalog= ""
                                                        )
        {
            string respuesta = string.Empty;

            MailMessage email;
            SmtpClient smtp;

            string[] lstMailTo = mailTo.Split(delimitador);

            if (lstMailTo == null)
            {
                respuesta = "Error: No existe correo destinatario";
            }
            else if (lstMailTo.Count() <= 0)
            {
                respuesta = "Error: No existe correo destinatario";
            }
            else if (string.IsNullOrEmpty(asunto))
            {
                respuesta = "Error: El asunto no puede estar vacío";
            }


            if (string.IsNullOrEmpty(respuesta))
            {
                try
                {
                    email = new MailMessage();
                    smtp = new SmtpClient();

                    foreach (var i in lstMailTo)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            email.To.Add(new MailAddress(i));
                        }
                    }

                    string[] lstBCC = bCC.Split(delimitador);
                    foreach (var i in lstBCC)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            email.Bcc.Add(new MailAddress(i));
                        }
                    }

                    email.From = new MailAddress(mailFrom);
                    email.Subject = asunto;
                    email.Body = "<p>" + cuerpoMensaje + "</p>";
                    email.IsBodyHtml = true;
                    email.Priority = MailPriority.Normal;

                    smtp.Host = smtpHost;
                    smtp.Port = puerto;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(mailFrom, contraseña);

                    if (adjunto != null)
                    {
                        email.Attachments.Add(adjunto);
                    }

                    smtp.Send(email);
                    email.Dispose();
                    respuesta = "OK";
                }
                catch (SmtpException ex)
                {
                    respuesta = respuesta + "Excepcion SMTP: " + ex.Message;
                    if(!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    respuesta = respuesta + "-Excepcion SOCKET: " + ex.Message;
                    if (!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    respuesta = respuesta + "-Excepcion Win32: " + ex.Message;
                    if (!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);
                }
                catch (Exception ex)
                {
                    respuesta = "-Error: " + ex.Message;
                    if (!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);

                }
            }

            return respuesta;
        }


        
        public static async Task<string> EnviarCorreoElectronicoAsync(string mailTo,
                                                        string mailFrom,
                                                        string asunto,
                                                        string smtpHost,
                                                        int puerto,
                                                        string contraseña,
                                                        string cuerpoMensaje,
                                                        char delimitador,
                                                        Attachment adjunto = null,
                                                        string bCC = "",
                                                        string rutalog = ""
                                                        )
        {


            

            string respuesta = string.Empty;

            MailMessage email;
            SmtpClient smtp;

            string[] lstMailTo = mailTo.Split(delimitador);

            if (lstMailTo == null)
            {
                respuesta = "Error: No existe correo destinatario";
            }
            else if (lstMailTo.Count() <= 0)
            {
                respuesta = "Error: No existe correo destinatario";
            }
            else if (string.IsNullOrEmpty(asunto))
            {
                respuesta = "Error: El asunto no puede estar vacío";
            }


            if (string.IsNullOrEmpty(respuesta))
            {
                try
                {
                    email = new MailMessage();
                    smtp = new SmtpClient();

                    foreach (var i in lstMailTo)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            email.To.Add(new MailAddress(i));
                        }
                    }

                    string[] lstBCC = bCC.Split(delimitador);
                    foreach (var i in lstBCC)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            email.Bcc.Add(new MailAddress(i));
                        }
                    }

                    email.From = new MailAddress(mailFrom);
                    email.Subject = asunto;
                    email.Body = "<p>" + cuerpoMensaje + "</p>";
                    email.IsBodyHtml = true;
                    email.Priority = MailPriority.Normal;

                    smtp.Host = smtpHost;
                    smtp.Port = puerto;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(mailFrom, contraseña);

                    if (adjunto != null)
                    {
                        email.Attachments.Add(adjunto);
                    }

                    smtp.Send(email);
                    email.Dispose();
                    respuesta = "OK";
                }
                catch (SmtpException ex)
                {
                    respuesta = respuesta + "Excepcion SMTP: " + ex.Message;
                    if (!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    respuesta = respuesta + "-Excepcion SOCKET: " + ex.Message;
                    if (!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    respuesta = respuesta + "-Excepcion Win32: " + ex.Message;
                    if (!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);
                }
                catch (Exception ex)
                {
                    respuesta = "-Error: " + ex.Message;
                    if (!string.IsNullOrEmpty(rutalog)) FullLog(rutalog, ex);

                }
            }

            return respuesta;
        }

        public static void FullLog(    string rutalog,
                                        Exception e,
                                        string seccion = null,
                                        string extraInfo = null,
                                        [CallerFilePath] string callFilePath = "",
                                        [CallerMemberName] string memberName = "",
                                        [CallerLineNumber] int lineNumber = 0)
        {
            
            string origen = "clsCorreoElectronico";
            string app = "Produccion";
            MetodosEscrituraLogs.EscribeExcepcionLogNest(   e,
                                                            rutalog,
                                                            origen,
                                                            app,
                                                            seccion: seccion,
                                                            extraInfo: extraInfo,
                                                            callFilePath: callFilePath,
                                                            memberName: memberName,
                                                            lineNumber: lineNumber);
        }
    }
}
