using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.Extensions
{

    public static class TransportSizeMapper
    {
        public static TransportSizeDto toDTO(this TransportSize input, DBContext db)
        {
            return new TransportSizeDto
            {
                code = input.code,
                dateCreate = input.dateCreate,
                dateUpdate = input.dateUpdate,
                description = input.description,
                id = input.id,
                id_company = input.id_company,
                id_iceBagRange = input.id_iceBagRange,
                id_poundsRange = input.id_poundsRange,
                id_transportTariffType = input.id_transportTariffType,
                id_userCreate = input.id_userCreate,
                id_userUpdate = input.id_userUpdate,
                isActive = input.isActive,
                name = input.name,
                binRangeMaximun = input.binRangeMaximun,
                binRangeMinimum= input.binRangeMinimum,
                IceBagRange = db.IceBagRange.FirstOrDefault(r => r.id == input.id_iceBagRange),
                PoundsRange = db.PoundsRange.FirstOrDefault(r => r.id == input.id_poundsRange),
                TransportTariffType = db.TransportTariffType.FirstOrDefault(r => r.id == input.id_transportTariffType)

            };
        }

    }

}