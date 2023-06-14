using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class ENSC01ViewModel : BaseModel

    {
        public VCompleteVTInfo Msts { get; set; }
        public List<VCompleteVTInfo> View_List { get; set; }
        public VtmsEnrollPi Pi_Msts { get; set; }
        public VtmsEnrollEdu Edu_Msts { get; set; }
        public MState mState { get; set; }  
        public MDistrict mDistrict { get; set; }

        public VtmsRecommMsts VtmsRecommMsts { get; set; }

        public string OthersRecommName { get; set; }
        public List<SelectList> ListOtherRecomm { get; set; }
        public decimal? RecommPno { get; set; }

        public string ReportType { get; set; }

        // For Loading the default list
        public List<SelectListItem> InstituteList { get; set; }
        public List<SelectListItem> Branchlist { get; set; }

        public List<VtmsCourseMsts> Courselist { get; set; }
    }
}
