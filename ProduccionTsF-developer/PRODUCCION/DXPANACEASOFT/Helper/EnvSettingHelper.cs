using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Helper
{
    

    public static class EnvSettingHelper
    {

        private static Dictionary<string, string> Settings = new Dictionary<string, string>(); 


        public static string GetSetting(string setting)
        {
            
            var model = Settings
                            .FirstOrDefault(r=> r.Key == setting);


            if (string.IsNullOrWhiteSpace( model.Key ))
            {
                string ruta = ConfigurationManager.AppSettings[setting];
                model = new KeyValuePair<string,string>(setting, ruta);
                Settings.Add(setting, ruta);
            }
            return model.Value;
        }

    }
}