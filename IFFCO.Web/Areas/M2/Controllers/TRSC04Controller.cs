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
using IFFCO.VTMS.Web.CommonFunctions;
using ModelContext = IFFCO.VTMS.Web.Models.ModelContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Newtonsoft.Json;


namespace IFFCO.VTMS.Web.Areas.M2.Controllers
{
    [Area("M2")]
    public class TRSC04Controller : BaseController<TRSC04ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        public ReportRepositoryWithParameters reportRepository = null;
        CommonException<TRSC04ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public TRSC04Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TRSC04ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
            reportRepository = new ReportRepositoryWithParameters();

        }



        public IActionResult Search(TRSC04ViewModel tRSC04ViewModel )
        { // Called when the 'Query' button on index page is pressed.
            try
            {
                
                CommonViewModel = GetVtList(tRSC04ViewModel);// Populating the VT List using the 'GetVtList' function declared in same file

                CommonViewModel.FromDate = tRSC04ViewModel.FromDate;
                CommonViewModel.ToDate = tRSC04ViewModel.ToDate;

                if (tRSC04ViewModel.Pi_Msts != null)
                {
                    CommonViewModel.Pi_Msts = new VtmsEnrollPi()
                    {
                        StateName = tRSC04ViewModel.Pi_Msts.StateName,
                        DistrictName = tRSC04ViewModel.Pi_Msts.DistrictName
                    };
                };
                
                if (tRSC04ViewModel.Edu_Msts != null)
                {
                    CommonViewModel.Edu_Msts = new VtmsEnrollEdu()
                    {
                        UniversityName = tRSC04ViewModel.Edu_Msts.UniversityName,
                        InstituteName = tRSC04ViewModel.Edu_Msts.InstituteName,
                        BranchName = tRSC04ViewModel.Edu_Msts.BranchName,
                        //CourseName = tRSC04ViewModel.Edu_Msts.CourseName,
                    };
                };
                
                
                TempData["CommonViewModel"] = JsonConvert.SerializeObject(CommonViewModel); // Serializing the entire view model
                CommonViewModel.IsAlertBox = false;
                CommonViewModel.SelectedAction = "GetListSearch";  // Method which will be called after this method gets complete. The form will be de-serialized in Get-List-Search
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
        public TRSC04ViewModel GetVtList(TRSC04ViewModel tRSC04ViewModel)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));

            string query = "SELECT A.BRANCH_NAME, A.COURSE_DESC, A.NAME, A.VT_CODE, A.VT_END_DATE, A.VT_START_DATE " +
               "FROM V_COMPLETE_VT_INFO A, VTMS_ENROLL_PI B " +
               "WHERE A.VT_CODE = B.VT_CODE AND A.STATUS IN ('A','N','I') " +
               "AND B.ENROLLED_ON BETWEEN '" + tRSC04ViewModel.FromDate.ToString("dd-MMM-yyyy") + "' AND '" + tRSC04ViewModel.ToDate.ToString("dd-MMM-yyyy") + "' ";

            if (!string.IsNullOrEmpty(tRSC04ViewModel.Edu_Msts.InstituteName)) { query += "AND A.INSTITUTE_NAME LIKE '%" + tRSC04ViewModel.Edu_Msts.InstituteName + "%' "; }
            if (!string.IsNullOrEmpty(tRSC04ViewModel.Edu_Msts.UniversityName)) { query += "AND A.UNIVERSITY_NAME LIKE '%" + tRSC04ViewModel.Edu_Msts.UniversityName + "%' "; }
            if (!string.IsNullOrEmpty(tRSC04ViewModel.Edu_Msts.BranchName)) { query += "AND A.BRANCH_NAME LIKE '%" + tRSC04ViewModel.Edu_Msts.BranchName + "%' "; }
            if (!string.IsNullOrEmpty(tRSC04ViewModel.Edu_Msts.CourseName)) { query += "AND A.COURSE_DESC LIKE '%" + tRSC04ViewModel.Edu_Msts.CourseName + "%' "; }

            query += "AND A.UNIT_CODE LIKE '%" + unit + "%' ";



            DataTable dt = _context.GetSQLQuery(query);
            CommonViewModel.View_InActive_List = new List<VCompleteVTInfo>();
            CommonViewModel.View_InActive_List = (from DataRow dr in dt.Rows
                                                  select new VCompleteVTInfo
                                                  {
                                                      Vtcode = Convert.ToString(dr["VT_CODE"]),
                                                      Name = Convert.ToString(dr["NAME"]),
                                                      BranchName = Convert.ToString(dr["BRANCH_NAME"]),
                                                      COURSE_DESC = dr["COURSE_DESC"].ToString(),

                                                      VtStartDate = string.IsNullOrEmpty(dr["VT_START_DATE"].ToString()) ? (DateTime?)null : Convert.ToDateTime(dr["VT_START_DATE"]),
                                                      VtEndDate = string.IsNullOrEmpty(dr["VT_END_DATE"].ToString()) ? (DateTime?)null : Convert.ToDateTime(dr["VT_END_DATE"]),
                                                  }).ToList();

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return CommonViewModel;
        }

        public async Task<IActionResult> GetListSearch()
        {
            int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));

            TRSC04ViewModel CommonViewModel = new TRSC04ViewModel();
            CommonViewModel = JsonConvert.DeserializeObject<TRSC04ViewModel>(TempData["CommonViewModel"].ToString());
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var statuslist = dropDownListBindWeb.GET_VTSTATUS();
            ViewBag.StatusLOV = statuslist;
            var state = dropDownListBindWeb.ListStateBind();
            ViewBag.StateLOV = state;
            var course = dropDownListBindWeb.ListCourseBind();
            ViewBag.CourseLOV = course;
            ViewBag.StatusLOV = statuslist;
            return View("Index", CommonViewModel);
        }

        public async Task<IActionResult> Index()
        {
            //return View(await _context.VtmsEnrollPi.ToListAsync());
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var statuslist = dropDownListBindWeb.GET_VTSTATUS();
            ViewBag.StatusLOV = statuslist;
            var state = dropDownListBindWeb.ListStateBind();
            ViewBag.StateLOV = state;
            var course = dropDownListBindWeb.ListCourseBind();
            ViewBag.CourseLOV = course;

            if (Convert.ToString(TempData["Message"]) != "")
            {
                CommonViewModel.Message = Convert.ToString(TempData["Message"]);
                CommonViewModel.Alert = Convert.ToString(TempData["Alert"]);
                CommonViewModel.Status = Convert.ToString(TempData["Status"]);
                CommonViewModel.ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
            }

            CommonViewModel.View_Active_List = new List<VCompleteVTInfo>();
            CommonViewModel.View_InActive_List = vTMSCommonService.VtCompleteDTl().Where(x => x.Status == "I").Where(x => x.EnrollmentStatus == "Enrolled").Where(x => x.VtStartDate >= DateTime.Today.AddMonths(-3)).Where(x => x.VtEndDate <= DateTime.Today.AddMonths(+9)).Where(x => x.UnitCode == unit).ToList();
            CommonViewModel.FromDate = DateTime.Today.AddMonths(-1); // Declaring from-date as the 1st day of the current month
            CommonViewModel.ToDate = DateTime.Today; // Declaring to-date as the current date.
                                                     //var statelist = dropDownListBindWeb.GetState();
                                                     //ViewBag.StateLOV = statelist;
                                                     //var courselist = dropDownListBindWeb.GetCourse();
                                                     //ViewBag.CourseLOV = courselist;
                                                     //var universities = dropDownListBindWeb.Getuniveristy();
                                                     //ViewBag.UniversityLOV = universities;


            
               return View("Index", CommonViewModel);
            
        }

        public List<SelectListItem> DistrictLOVBindJSON(string StateCd)
        {
            List<SelectListItem> disttCDLOV = new List<SelectListItem>();
            disttCDLOV = dropDownListBindWeb.ListDistrictBind(StateCd);
            return disttCDLOV;
        }

        public List<SelectListItem> UniversityLOVBindJSON(string DistrictName)
        {
            List<SelectListItem> universityCDLOV = new List<SelectListItem>();
            universityCDLOV = dropDownListBindWeb.ListUniversityBind(DistrictName);
            return universityCDLOV;
        }

        public List<SelectListItem> InstituteLOVBindJSON(string UniversityId)
        {
            List<SelectListItem> InstituteCodeLOV = new List<SelectListItem>();
            InstituteCodeLOV = dropDownListBindWeb.ListInstituteBind(UniversityId);
            return InstituteCodeLOV;
        }

        public List<SelectListItem> BranchLOVBindJSON(string CourseCode)
        {
            List<SelectListItem> BranchCodeLOV = new List<SelectListItem>();
            BranchCodeLOV = dropDownListBindWeb.ListBranchBind(CourseCode);
            return BranchCodeLOV;
        }


        public ActionResult GenerateReport(string vtcode)
        {
            var tRSC04ViewModel = new TRSC04ViewModel();
            tRSC04ViewModel.Pi_Msts = new VtmsEnrollPi()
            {
                VtCode = vtcode
            };
            bool rdlc = true;
            string separator = "&";
            string extension = "aspx";
            var fullClientIp = HttpContext.Session.GetString("fullClientIp");
            var clientIp = HttpContext.Session.GetString("clientIp");

            string Report = "";
            string QueryString = String.Empty;

            tRSC04ViewModel.CallingReport = "APPLICANTFORM." + extension;


            Report reportobj = GenerateReportData(tRSC04ViewModel, separator);
            string ReportFormat = "pdf";
            string data = reportobj.ReportName + "+destype=cache+desformat=" + ReportFormat;

            Report = reportRepository.GenerateReportRdlc(HttpContext.Session.GetString("ReportServer"),
                      reportobj.Query,
                      reportobj.ReportName,
                      this.ControllerContext.RouteData.Values["area"].ToString(),
                      this.ControllerContext.RouteData.Values["controller"].ToString(),
                      HttpContext.Session.GetInt32("EmpID").ToString(), fullClientIp, clientIp);

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }

        public Report GenerateReportData(TRSC04ViewModel tRSC04ViewModel, string separator)
        {
            Report ReportData = new Report();
            ReportData.ReportName = tRSC04ViewModel.CallingReport;
            ReportData.Query =
                "P_VTCODE=" + tRSC04ViewModel.Pi_Msts.VtCode;
            return ReportData;
        }
    }
}

