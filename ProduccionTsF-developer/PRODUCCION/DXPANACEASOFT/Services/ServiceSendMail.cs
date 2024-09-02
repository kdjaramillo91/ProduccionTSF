using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
    public static class ServiceSendMail
    {
        public static void SendEmail(string emailSubject, string emailBody, List<string> listEmailAdress)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var mailConfiguration = db.MailConfiguration.FirstOrDefault(c => c.isActive);
                    if (mailConfiguration == null)
                        throw new Exception("No se encontró una configuración de correo");
                        
                    var oMail = new MailMessage
                    {
                        From = new MailAddress(mailConfiguration.mail),
                        Subject = emailSubject,
                        Body = emailBody,
                        IsBodyHtml = true
                    };

                    foreach (var emailAdress in listEmailAdress)
                        oMail.To.Add(new MailAddress(emailAdress));
                    
                    var oSmtp = new SmtpClient()
                    {
                        Host = mailConfiguration.host,
                        Port = mailConfiguration.port,
                        Credentials = new NetworkCredential(mailConfiguration.userName, mailConfiguration.password),
                        EnableSsl = mailConfiguration.enableSsl
                    };

                    if (oSmtp.EnableSsl)
                    {
                        //Para evitar el error, debes utilizar este método que permite  
                        //la validación personalizada por el cliente del certificado de servidor.
                        ServicePointManager.ServerCertificateValidationCallback =
                            delegate (object s
                                , X509Certificate certificate
                                , X509Chain chai
                                , SslPolicyErrors sslPolicyErrors)
                            {
                                return true;
                            };
                    }

                    oSmtp.Send(oMail);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar enviar el correo. " + e.Message);
            }
        }
    }
}