using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels

{
    public class ENSC02ViewModel : BaseModel
    {
        public List<VtmsEnrollEdu> vteinfos { get; set; }
        public VtmsEnrollEdu vteinfo { get; set; }

        public VtmsEnrollPi vtpinfo { get; set; }
        public List<VtmsEnrollPi> vtpinfos { get; set; }

        public List<VCompleteVTInfo> vCompleteVTInfos { get; set; }

        public VCompleteVTInfo VCompleteVTInfo { get; set; }
    }
}




