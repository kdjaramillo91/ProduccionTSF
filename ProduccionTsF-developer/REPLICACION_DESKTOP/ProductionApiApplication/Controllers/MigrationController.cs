using MigracionProduccionCIWebApi.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ProductionApiApplication.Controllers
{
	public class MigrationController : ApiController
	{
		[HttpPost]
		public List<AnswerMigration> AddGeneral(string sMigrar)
		{
			try
			{
                if (sMigrar == "Productos")
                {
                    return (new ItemRepository()).AddItems();
                }
                else if (sMigrar == "Proveedores")
                {
                    return (new ProviderRepository()).AddProviders();
                }
                else if (sMigrar == "Clientes")
                {
                    return (new CustomerRepository()).AddCustomers();
                }
                else
                {
                    return new List<AnswerMigration>();
                }
            }
				catch (Exception ex)
				{
					// Capturar el error y devolver una lista de AnswerMigration con información de error
					return new List<AnswerMigration>
				{
				new AnswerMigration
				{
					resultado = false,
					message = ex.InnerException.Message
					// Puedes agregar más propiedades según sea necesario para proporcionar información detallada
				}
				};
            }
        }

		[HttpDelete]
		public void DeleteGeneral(string sMigrar)
		{
			(new CustomerRepository()).DeleteCustomers();
		}


		[HttpPost]
		public string AddModifyPerson(int id)
		{
			var respuestaProveedor = (new ProviderRepository())
				.AddModifyProvider(id);

			var respuestaCliente = (new CustomerRepository())
				.AddModifyCustomer(id);

			return $"respuestaProveedor:{respuestaProveedor} respuestaCliente:{respuestaCliente}";
		}

		[HttpPost]
		public string AddModifyItem(int id)
		{
			var respuestaProducto = (new ItemRepository())
				.AddModifyItem(id);

			return $"respuestaProducto:{respuestaProducto}";
		}

		[HttpGet]
		public PersonProvider GetProvider(int id)
		{
			return (new ProviderRepository())
				.GetProvider(id);
		}

		[HttpGet]
		public IEnumerable<PersonProvider> GetProviders()
		{
			return (new ProviderRepository())
				.GetAllProviders();
		}
	}
}
