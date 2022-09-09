using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFFCO.HRMS.Entities.Models;
using IFFCO.HRMS.Service;
using IFFCO.HRMS.Shared.Entities;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using IFFCO.VTMS.Web.Areas.M2.Controllers;
using IFFCO.HRMS.Entities.AppConfig;
using System.Net;
using IFFCO.VTMS.Web.Controllers;
using IFFCO.VTMS.Web.ViewModels;
using IFFCO.VTMS.Web.Models;
using IFFCO.VTMS.Web.CommonFunctions;
using ModelContext = IFFCO.VTMS.Web.Models.ModelContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace IFFCO.VTMS.Web.Areas.M2.Controllers
{

    [Area("M2")]
    public class TRSCR01Controller : BaseController<TRSCR01ViewModel>
    {
        private readonly ModelContext _context;
        public ReportRepositoryWithParameters reportRepository = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TRSCR01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public TRSCR01Controller(ModelContext context)
        {
            _context = context;
            reportRepository = new ReportRepositoryWithParameters();
            commonException = new CommonException<TRSCR01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }

        public ActionResult Index()
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var statuslist = dropDownListBindWeb.GET_VTSTATUS();
            ViewBag.StatusLOV = statuslist;
            if (Convert.ToString(TempData["Message"]) != "")
            {

                CommonViewModel.Message = Convert.ToString(TempData["Message"]);
                CommonViewModel.Alert = Convert.ToString(TempData["Alert"]);
                CommonViewModel.Status = Convert.ToString(TempData["Status"]);
                CommonViewModel.ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
            }

            CommonViewModel.vCompleteVTInfos = new List<VCompleteVTInfo>();
            CommonViewModel.vCompleteVTInfos = vTMSCommonService.VtCompleteDTl();
            CommonViewModel.FromDate = DateTime.Today.AddMonths(-1); // Declaring from-date as the 1st day of the current month
            CommonViewModel.ToDate = DateTime.Today;    // Declaring to-date as the current date.

            return View(CommonViewModel);
        }

        public IActionResult Query(TRSCR01ViewModel tRSCR01ViewModel, string Status)
        { // Called when the 'Query' button on index page is pressed.
            try
            {
                CommonViewModel = GetVtList(Convert.ToDateTime(tRSCR01ViewModel.FromDate), Convert.ToDateTime(tRSCR01ViewModel.ToDate), tRSCR01ViewModel.Status); // Populating the VT List using the 'GetVtList' function declared in same file 
                TempData["CommonViewModel"] = JsonConvert.SerializeObject(CommonViewModel); // Serializing the entire view model
                CommonViewModel.IsAlertBox = false;
                CommonViewModel.SelectedAction = "GetListSearch";   // Method which will be called after this method gets complete. The form will be de-serialized in Get-List-Search
                CommonViewModel.ErrorMessage = "";
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString(); // Populating Area name for forming the page URL
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL

            }
            catch (Exception ex)
            {

            }
            return Json(CommonViewModel);
        }

        [HttpPost]
        public TRSCR01ViewModel GetVtList(DateTime? FromDate, DateTime? ToDate, string Status)
        {
            var str = string.Empty;
            int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            if (Status == null) { Status = dropDownListBindWeb.GET_VTSTATUS().FirstOrDefault().Value; }
            if (FromDate == null) { var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); FromDate = first; }
            if (ToDate == null) { ToDate = DateTime.Today; }
            CommonViewModel.vCompleteVTInfos = new List<VCompleteVTInfo>();
            CommonViewModel.vCompleteVTInfos = vTMSCommonService.VtCompleteDTl().Where(x => x.Status == Status).Where(x => x.VtStartDate >= FromDate && x.VtEndDate <= ToDate).ToList();
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return CommonViewModel;
        }

        public async Task<IActionResult> GetListSearch()
        {
            int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            var statuslist = dropDownListBindWeb.GET_VTSTATUS();
            ViewBag.StatusLOV = statuslist;
            TRSCR01ViewModel CommonViewModel = new TRSCR01ViewModel();
            CommonViewModel = JsonConvert.DeserializeObject<TRSCR01ViewModel>(TempData["CommonViewModel"].ToString());
            return View("Index", CommonViewModel);
        }

        public JsonResult GetReport(string id, string report)
        {
            string Report = "";
            string QueryString = String.Empty;
            var ReportName = string.Empty;
            if (!string.IsNullOrWhiteSpace(report) && report == "ApplicationForm")
            {
                ReportName = "ApplicantForm".ToString() + ".rep";
            }
            else if (!string.IsNullOrWhiteSpace(report) && report == "ReviewSheet")
            {
                ReportName = "ReviewSheet".ToString() + ".rep";
            }

            var ReportFormat = "pdf";
            string data = ReportName + "+destype=cache+desformat=" + ReportFormat;
            QueryString = "P_VTCODE=" + id;
            Report = reportRepository.GenerateSalaryCardReport(QueryString, data);
            return Json(Report);
            //return View("GetFormQuery");

        }


    }
}
