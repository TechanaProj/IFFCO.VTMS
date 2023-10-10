using IFFCO.VTMS.Web.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using IFFCO.HRMS.Shared.Entities;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class ENSCR01ViewModel : BaseModel
    {
        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }
        public VCompleteVTInfo Msts { get; set; }
       
        public List<VCompleteVTInfo> List_msts { get; set; }

        public VtmsEnrollPi Pi_Msts { get; set; }

        public VtmsEnrollEdu Edu_Msts { get; set; }

        public DateTime VtStartDate { get; set; }

        public DateTime VtEndDate { get; set; }

        public string OthersRecommName { get; set; }
        public decimal? RecommPno { get; set; }

       
        public string ReportType { get; set; }
        

    }
}
