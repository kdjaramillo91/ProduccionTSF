using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderVehicle
    {
        private static DBContext db = null;

        public static IEnumerable Vechiles()
        {
            db = new DBContext(); ;
            return db.Vehicle.Where(v => v.isActive).ToList();
        }

        public static Vehicle Vehicle(int? id_vehicle)
        {
            db = new DBContext();
            return db.Vehicle.FirstOrDefault(v => v.id == id_vehicle);
        }

		public static string VehicleRucProviderTransportis(int? id_vehicle)
		{
			string rucNameProvider = "";
			db = new DBContext();
			var vpt = db.VeicleProviderTransport.FirstOrDefault(fod => fod.id_vehicle == id_vehicle && fod.Estado && fod.datefin == null);

			if (vpt != null)
			{
				var id_provider = vpt.id_Provider;
				if (id_provider != null && id_provider != 0)
				{
					var provider = db.Person.FirstOrDefault(fod => fod.id == id_provider);
					if (provider != null)
					{
						rucNameProvider = provider.identification_number;
					}
				}
			}
			return rucNameProvider;
		}
		public static string VehicleProviderTransportis(int? id_vehicle)
        {
            string nameProvider = "";
            db = new DBContext();
            var vpt = db.VeicleProviderTransport.FirstOrDefault(fod => fod.id_vehicle == id_vehicle && fod.Estado && fod.datefin == null);

            if (vpt != null)
            {
                var id_provider = vpt.id_Provider;
                if (id_provider != null && id_provider != 0)
                {
                    var provider = db.Person.FirstOrDefault(fod => fod.id == id_provider);
                    if (provider != null)
                    {
                        nameProvider = provider.fullname_businessName;
                    }
                }
            }
            return nameProvider;
        }
        public static int VehicleProviderTransportistId(int? id_vehicle)
        {
            int idProvider = 0;
            db = new DBContext();
            var vpt = db.VeicleProviderTransport.FirstOrDefault(fod => fod.id_vehicle == id_vehicle && fod.Estado && fod.datefin == null);

            if (vpt != null)
            {
                idProvider = (int)vpt.id_Provider;
                
            }
            return idProvider;
        }
        public static int VehicleProviderTransportistIdVehicleProvider(int? id)
        {
            int idProvider = 0;
            db = new DBContext();
            VeicleProviderTransport vpt = new VeicleProviderTransport();
            var idVeicleProviderTransport = db.DriverVeicleProviderTransport.FirstOrDefault(fod => fod.Estado.Value && fod.id == id).idVeicleProviderTransport;
            if(idVeicleProviderTransport != null)
            {
                vpt = db.VeicleProviderTransport.FirstOrDefault(fod => fod.id == idVeicleProviderTransport && fod.Estado && fod.datefin == null);
            }

            if (vpt != null)
            {
                idProvider = (int)vpt.id_Provider;

            }
            return idProvider;
        }

        public static string VehicleRucProviderTransportistBilling(int? id_vehicle)
		{
			string rucNameProvider = "";
			db = new DBContext();
			var vpt = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == id_vehicle && fod.state && fod.datefin == null);

			if (vpt != null)
			{
				var id_provider = vpt.id_provider;
				if (id_provider != 0)
				{
					var provider = db.Person.FirstOrDefault(fod => fod.id == id_provider);
					if (provider != null)
					{
						rucNameProvider = provider.identification_number;
					}
				}
			}
			return rucNameProvider;
		}
		public static string VehicleProviderTransportistBilling(int? id_vehicle)
        {
            string nameProvider = "";
            db = new DBContext();
            var vpt = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == id_vehicle && fod.state && fod.datefin == null);

            if (vpt != null)
            {
                var id_provider = vpt.id_provider;
                if (id_provider != 0)
                {
                    var provider = db.Person.FirstOrDefault(fod => fod.id == id_provider);
                    if (provider != null)
                    {
                        nameProvider = provider.fullname_businessName;
                    }
                }
            }
            return nameProvider;
        }
        public static string VehicleProviderTransportistBillingNameProvider(int? id_p)
        {
            string nameProvider = "";
            db = new DBContext();

            nameProvider = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id == id_p)?.Person?.fullname_businessName ?? "";

            return nameProvider;
        }
        public static int VehicleProviderTransportistBillingId(int? id_vehicle)
        {
            int idProvider = 0;
            db = new DBContext();
            var vpt = db.VehicleProviderTransportBilling
                            .FirstOrDefault(fod => fod.id_vehicle == id_vehicle 
                                                && fod.state && fod.datefin == null);

            if (vpt != null)
            {
                idProvider = (int)vpt.id_provider;

            }
            return idProvider;
        }

        public static IEnumerable VehiclesForRemissionGuide(int? idTtt, int? id_vehicleCurrent)
        {
            db = new DBContext();
            List<int> listVehicleType = new List<int>();


            var TransportTariffTyperVehicleType = db.TransportTariffType_VehicleType.Where(t => t.isActive && t.id_transportTariffType == idTtt).ToList();

            if (TransportTariffTyperVehicleType != null && TransportTariffTyperVehicleType.Count > 0)
            {
                var listVehicleTypeax = (from e in TransportTariffTyperVehicleType
                                         select e.id_vehicleType).ToList();

                if (listVehicleTypeax != null && listVehicleTypeax.Count > 0)
                {
                    listVehicleType.AddRange(listVehicleTypeax);
                }
            }
            var Vehicle = db.Vehicle.Where(v => v.isActive && listVehicleType.Contains(v.id_VehicleType)).ToList();

            var VehicleCurrentAux = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehicleCurrent && listVehicleType.Contains(fod.id_VehicleType));
            if (VehicleCurrentAux != null && !Vehicle.Contains(VehicleCurrentAux)) Vehicle.Add(VehicleCurrentAux);
            var _VehicleProviderTransportBilling = db.VehicleProviderTransportBilling.Where(r => r.state).ToList();
            var listVehicleProviderTransportBillin = (from e in _VehicleProviderTransportBilling
                                                      join f in Vehicle on e.id_vehicle equals f.id
                                                      where e.state && f.isActive
                                                      select new
                                                      {
                                                          id = f.id,
                                                          carRegistration = f.carRegistration,
                                                          id_providerBilling = e.id_provider,
                                                          ciaFactura = e.Person.fullname_businessName
                                                      }).ToList();

            return listVehicleProviderTransportBillin.ToList();
            //return Vehicle.OrderBy(t => t.id).ToList();

        }

    }
}