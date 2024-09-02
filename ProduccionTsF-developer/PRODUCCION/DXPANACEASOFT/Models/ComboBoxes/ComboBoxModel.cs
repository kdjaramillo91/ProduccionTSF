using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Operations;
using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.ComboBoxes
{
    public class ComboBoxModel
    {
        public string Name { get; set; }
        public string ValidationGroup { get; set; }
        public object CallbackRouteValues { get; set; }
        public bool IsForOptionalQuery { get; set; }
        public bool IsClientEnabled { get; set; }
        public ComboBoxClientSideEvents ClientSideEvents { get; set; }
        public Dictionary<string, object> CustomProperties { get; set; }
        public Display Display { get; set; }

        public ComboBoxModel()
        {
            this.IsClientEnabled = true;
        }

        protected void SetComboBoxCommonSettings(ComboBoxSettings settings)
        {
            settings.Name = this.Name;
            settings.ClientEnabled = this.IsClientEnabled;

            settings.SetDefaultSettings();

            if (this.CustomProperties != null)
            {
                settings.SetClientJSProperties(null, null, this.CustomProperties);
            }

            settings.CallbackRouteValues = this.CallbackRouteValues;
        }

        protected void SetComboBoxCommonProperties(ComboBoxProperties properties)
        {
            properties.ClientInstanceName = this.Name;

            if (!String.IsNullOrEmpty(this.ValidationGroup))
            {
                properties.ValidationSettings.ValidationGroup = this.ValidationGroup;
            }

            if (this.IsForOptionalQuery)
            {
                properties.NullText = "Todos";
                properties.NullDisplayText = "Todos";
            }

            properties.ClientSideEvents.Assign(this.ClientSideEvents);
        }
    }
}