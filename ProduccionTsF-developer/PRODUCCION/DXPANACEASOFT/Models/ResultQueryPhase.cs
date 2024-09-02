using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class ResultQueryPhase
    {
        public string filterDocumentType { get; set; }

        public string filterAddress { get; set; }

        public string filterItemSizes { get; set; }

        public string filterPersons { get; set; }

        public string filterExecutivePersons { get; set; }

        public string  filterRangeStartDate { get; set; }

        public string filterRangeEndDate { get; set; }

        public string filterPhases { get; set; }

        public string filterAmounts { get; set; }

        public string filterPercents { get; set; }

        public string filterDocumentState { get; set; }

        public string filterBusinessOportunityState { get; set; }

        public List<QueryPhase> listQueryPhase { get; set; }

	    public Company company { get; set; }

    }
}