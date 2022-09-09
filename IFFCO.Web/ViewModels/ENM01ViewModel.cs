using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class ENM01ViewModel:BaseModel

    {
        public List<VtmsBranchMsts> VtmsBranchMstsList { get; set; }
        public VtmsBranchMsts Branch { get; set; }
        public int BranchId { get; set; }
        public string BranchDesc { get; set; }
        public string BranchCode { get; set; }
       
        public List<SelectListItem> CourseLOVBind { get; set; }
        public string CourseCode { get; set; }

       

    }

}

