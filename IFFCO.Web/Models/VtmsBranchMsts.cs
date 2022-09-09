using IFFCO.HRMS.Repository.Pattern;
using System;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.Models
{
    public partial class VtmsBranchMsts :Entity
    {
        public int? BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchDesc { get; set; }
        public string CourseCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }
}
