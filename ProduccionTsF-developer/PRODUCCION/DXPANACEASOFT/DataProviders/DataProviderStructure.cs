using System.Linq;
using DXPANACEASOFT.Models;
using System.Collections;
using DXPANACEASOFT.Models.Estructure.EstructureModels;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderStructure
    {
        private static DBContext db = null;

        public static IEnumerable AllCompanies()
        {
            db = new DBContext();
            return db.Company
                        .Where(w => w.isActive)
                        .Select(s => new CompanyModelP
                        {
                            idCompanyModelP = s.id,
                            nameCompanyModelP = s.businessName
                        }).ToList();
        }
        public static IEnumerable AllDivisions()
        {
            db = new DBContext();
            return db.Division
                        .Where(w => w.isActive)
                        .Select(s => new DivisionModelP
                        {
                            idDivisionModelP = s.id,
                            idCompanyModelP = s.id_company,
                            nameDivisionModelP = s.name
                        }).ToList();
        }

        public static IEnumerable AllBranchOffices()
        {
            db = new DBContext();
            return db.BranchOffice
                        .Where(w => w.isActive)
                        .Select(s => new BranchOfficeModelP
                        {
                            idBranchOfficeModelP = s.id,
                            idDivisionModelP = s.id_division,
                            nameBranchOfficeModelP = s.name
                        }).ToList();

        }

    }
}