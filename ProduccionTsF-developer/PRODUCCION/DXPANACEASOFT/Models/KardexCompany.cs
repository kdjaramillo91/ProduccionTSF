using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class KardexCompany
    {
        public List<int> list_id_kardex { get; set; }
        public List<ResultKardex> listResultKardex { get; set; }
        public Company company { get; set; }

    }
}