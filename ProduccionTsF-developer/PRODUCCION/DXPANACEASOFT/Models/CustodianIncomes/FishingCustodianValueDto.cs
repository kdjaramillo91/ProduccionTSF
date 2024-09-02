namespace DXPANACEASOFT.Models.Dto
{

    public class FishingCustodianValueDto
    { 
        public string ControlName { get; set; }
        public string ControlDependName { get; set; }
        public int FishingCustodianId { get; set; }
        public int FishingSiteId { get; set; }
        public string CodeValue { get; set; } 
        public string NameValue { get; set; }
    }

    public class FishinngCustodianFieldValue
    {
        public int Id { get; set; }
        public int id_FishingSite { get; set; }
        public decimal FieldValue { get; set; }

    }

    public static class FishinngCustodianFieldConvert
    {
        public static string GetDescrip(string fieldValue)
        {
            string descrip = null;
            switch (fieldValue)
            {
                case "patrol":
                    descrip = "Patrulla";
                    break;
                case "semiComplete":
                    descrip = "Semi Completa";
                    break;
                case "truckDriver":
                    descrip = "Camioneta";
                    break;
                case "changeHG":
                    descrip = "H/Cambina Gye";
                    break;
                case "cabinHR":
                    descrip = "H/Cabina Ruta";
                    break;
            }
            return descrip;
        }
    }





}
