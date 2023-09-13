using IFFCO.VTMS.Web.ViewModels;
using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using IFFCO.VTMS.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class TRSC02ViewModel : BaseModel
    {
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime FromDate { get; set; }

        //[DataType(DataType.Date)]
       // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime ToDate { get; set; }

        public List<VCompleteVTInfo> vCompleteVTInfos { get; set; }
        public VCompleteVTInfo VCompleteVTInfo { get; set; }
        public VtmsEnrollPi Pi_Msts { get; set; }
        public VtmsEnrollEdu Edu_Msts { get; set; }
        public VtmsVtReview R_Msts { get; set; }

       // public DateTime FromDate { get; set; }
       // public DateTime ToDate { get; set; }
        public string Status { get; set; }
        public string VtCode { get; set; }
        public decimal? UnitCode { get; set; }
        public double? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set;}
        public double? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }

}
