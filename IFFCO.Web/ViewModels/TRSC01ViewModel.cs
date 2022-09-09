using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class TRSC01ViewModel:BaseModel
    {
        public VtmsEnrollPi Pi_Msts { get; set; }
        
        public VtmsEnrollEdu Edu_Msts { get; set; }
        public List<VCompleteVTInfo> View_List { get; set; }
        public VCompleteVTInfo Vtinfo { get; set; } 
        public VtmsEnrollDoc Doc_Msts { get; set; }
        public string ProfileImage { get; set; }
        public byte[] IdProof { get; set; }
        public string VTCode { get; set; }
        public string Pincode { get; set; }
        public int UnitCode { get; set; }
      //  public string ActionMode { get; set; }
    }
}
