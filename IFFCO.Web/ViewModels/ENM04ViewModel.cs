using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels

{
    public class ENM04ViewModel:BaseModel
    {
      public List<VtmsUniversityMsts> vtmsUniversities { get; set; }
      public VtmsUniversityMsts vtmsUniversity { get; set; }      
        public List<MDistrict> mDistrict { get; set; }
      public MDistrict mDistricts { get; set; }      
      public string  fstatecd { get; set; }      
        
    }
}




