using IFFCO.HRMS.Repository.Pattern;
using System;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.Models
{
    public partial class MState : Entity
    {
        public string StateCd { get; set; }
        public string StateName { get; set; }
        public string Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModificationDt { get; set; }
        public string ModifiedBy { get; set; }
        public string IsoStateCd { get; set; }
        public string IsoStateName { get; set; }
    }
}
