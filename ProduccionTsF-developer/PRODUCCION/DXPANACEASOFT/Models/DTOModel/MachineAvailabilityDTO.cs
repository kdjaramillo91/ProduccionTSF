using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class MachineAvailabilityResultConsultDTO
	{
		public int id { get; set; }
		public string code { get; set; }
		public string nameTbsysTypeMachineForProd { get; set; }
		public string nameRol { get; set; }

        public List<MachineAvailabilityDTO> MachineAvailabilityDetails { get; set; }
    }

	public class MachineAvailabilityDTO
	{
		public int id { get; set; }
		public string code { get; set; }
		public string nameMachineForProd { get; set; }
		public string nameTbsysTypeMachineForProd { get; set; }
		public string namePersonProcessPlant { get; set; }
		public bool isActive { get; set; }
		public bool available { get; set; }
		public string reason { get; set; }
	}
}