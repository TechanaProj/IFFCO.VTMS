using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.ViewModels
{
    public class ENM06ViewModel : BaseModel
    {
        public MDistrict objDistrict { get; set; }
        public List<MDistrict> listDistrict { get; set; }

        public MState objState { get; set; }
        public List<MState> listState { get; set; }
        public string StateCd { get; set; }

        public List<SelectListItem> listStateBind { get; set; }       
    }
}
