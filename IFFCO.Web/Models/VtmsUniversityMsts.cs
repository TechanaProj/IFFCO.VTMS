using IFFCO.HRMS.Repository.Pattern;
using System;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.Models
{
    public partial class VtmsUniversityMsts :Entity
    {
        public int UniversityId { get; set; }
        public string UniversityName { get; set; }
        public string DistrictName { get; set; }
        public decimal? CreatedBy { get; set; }
        public DateTime? CreationDatetime { get; set; }
    }
}
