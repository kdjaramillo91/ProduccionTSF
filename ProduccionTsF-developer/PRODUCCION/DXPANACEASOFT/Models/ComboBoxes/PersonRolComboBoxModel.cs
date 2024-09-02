using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Operations;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Models.ComboBoxes
{
	public class PersonRolComboBoxModel : ComboBoxModel
	{
		public int? IdCompany { get; set; }
		public string[] Rols { get; set; }
		public int? IdPerson { get; set; }
        public bool? OpViewInvoice { get; set; }

        public void SetPersonRolComboBoxSettings(ComboBoxSettings settings)
		{
			this.SetComboBoxCommonSettings(settings);
			this.SetPersonRolComboBoxProperties(settings.Properties);
		}
		public void SetPersonRolColumnComboBoxProperties(MVCxColumnComboBoxProperties comboBox)
		{
			comboBox.SetDefaultSettings();

			comboBox.CallbackRouteValues = this.CallbackRouteValues;

			this.SetPersonRolComboBoxProperties(comboBox);

			comboBox.BindList(GetPersonRolsRange, GetPersonRolPorId);
		}

		private void SetPersonRolComboBoxProperties(ComboBoxProperties properties)
		{
			this.SetComboBoxCommonProperties(properties);

			properties.ValueField = "id";
			properties.TextField = "fullname_businessName";
			properties.ValueType = typeof(int);
			properties.TextFormatString = "{1}";

			properties.Columns.Add("identification_number", "Identificación", Unit.Percentage(40));
			properties.Columns.Add("fullname_businessName", "Nombre", Unit.Percentage(60));
            

        }

		public IEnumerable<Person> GetPersonRolsRange(ListEditItemsRequestedByFilterConditionEventArgs args)
		{
			var skip = args.BeginIndex;
			var take = args.EndIndex - args.BeginIndex + 1;

			return this.IdCompany.HasValue && this.Rols.Any()
				? DataProviderPerson.PersonsByCompanyRols(this.IdCompany.Value, this.Rols)
					.Where(e => e.identification_number.ToUpper().Contains(args.Filter.ToUpper())
							|| e.fullname_businessName.ToUpper().Contains(args.Filter.ToUpper()))
				: new Person[] { };
		}

		public Person GetPersonRolPorId(ListEditItemRequestedByValueEventArgs args)
		{
			var idPerson = this.IdPerson.HasValue 
				? this.IdPerson : (args.Value as int?);

			if (idPerson.HasValue)
			{
				return DataProviderPerson.PersonById(idPerson.Value);
			}
			else
			{
				return null;
			}
		}
	}
}