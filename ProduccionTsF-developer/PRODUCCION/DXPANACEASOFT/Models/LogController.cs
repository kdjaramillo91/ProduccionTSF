using System;
using System.Configuration;
using System.IO;

namespace DXPANACEASOFT.Models
{
    class LogController
    {
        public static void tratarFicheroLog()
        {
            try
            {
                //string sLogComprueba1MesAnt = ConfigurationManager.AppSettings["LogPath"] + ConfigurationManager.AppSettings["LogFileName"] + DateTime.Today.AddMonths(-1).ToString("yyyyMM") + ".txt";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogPath"]))
                {
                    if (!(Directory.Exists(ConfigurationManager.AppSettings["LogPath"])))
                    {
                        Directory.CreateDirectory(ConfigurationManager.AppSettings["LogPath"]);
                    }
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogFileName"]))
                    {
                        string sLogComprueba2MesAnt = ConfigurationManager.AppSettings["LogPath"] + "\\" + ConfigurationManager.AppSettings["LogFileName"] + DateTime.Today.AddMonths(-2).ToString("yyyyMM") + ".txt";

                        if (File.Exists(sLogComprueba2MesAnt))
                            File.Delete(sLogComprueba2MesAnt);

                        //if (!File.Exists(sLogComprueba1MesAnt))
                        //    File.Move(ConfigurationManager.AppSettings["LogPath"] + ConfigurationManager.AppSettings["LogFileName"] + ".txt", sLogComprueba1MesAnt);
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void WriteLog(string message)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogPath"]))
            {
                if (!(Directory.Exists(ConfigurationManager.AppSettings["LogPath"])))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["LogPath"]);
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogFileName"]))
                {
                    string path = ConfigurationManager.AppSettings["LogPath"] + "\\" + ConfigurationManager.AppSettings["LogFileName"] + DateTime.Today.ToString("yyyyMM") + ".txt";
                    StreamWriter stream = null;
                    try
                    {
                        //if (File.Exists(path)) {
                        stream = File.AppendText(path);
                        stream.WriteLine(string.Format("{0} - {1}.", DateTime.Now, message));
                        //}
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }
            }
            
        }
    }
}