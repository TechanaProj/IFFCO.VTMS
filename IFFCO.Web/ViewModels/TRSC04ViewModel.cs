﻿using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class TRSC04ViewModel : BaseModel

    {
        public VtmsEnrollPi Pi_Msts { get; set; }
        public VtmsEnrollEdu Edu_Msts { get; set; }
        public VtmsEnrollDoc Doc_Msts { get; set; }
        public VtmsVtReview R_Msts { get; set; }
        public List<VCompleteVTInfo> View_Active_List { get; set; }
        public List<VCompleteVTInfo> View_InActive_List { get; set; }
        public string ProfileImage { get; set; }
        public byte[] IdProof { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Status { get; set; }

        public string Report { get; set; }

        public string CallingReport { get; set; }
        // test for charts
        //public List<ChartModel> ChartList { get; set; }
        //public ChartModel ChartMsts { get; set; }

    }

}
