using IFFCO.HRMS.Repository.Pattern;
using System;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.Models
{
    public partial class MDistrict : Entity
    {
        public string DisttCd { get; set; }
        public string DisttName { get; set; }
        public string StateCd { get; set; }
        public string Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModificationDt { get; set; }
        public string ModifiedBy { get; set; }
    }
}
