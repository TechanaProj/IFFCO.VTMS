using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class ENM02ViewModel: BaseModel
    {
        public VtmsCourseMsts objvtmsCourseMsts { get; set; }
        public List<VtmsCourseMsts> listVtmsCourseMsts { get; set; }
    }
}
