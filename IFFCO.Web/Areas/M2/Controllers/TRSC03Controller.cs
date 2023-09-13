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
    public class TRSC03Controller : BaseController<TRSC03ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TRSC03ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public TRSC03Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TRSC03ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }


        public IActionResult Query(TRSC03ViewModel tRSC03ViewModel, String Stauts)
        { // Called when the 'Query' button on index page is pressed.
            try
            {
                              
                CommonViewModel = GetVtList(Convert.ToDateTime(tRSC03ViewModel.FromDate), Convert.ToDateTime(tRSC03ViewModel.ToDate), tRSC03ViewModel.Status);// Populating the VT List using the 'GetVtList' function declared in same file
                CommonViewModel.Status = tRSC03ViewModel.Status;
                CommonViewModel.FromDate = tRSC03ViewModel.FromDate;
                CommonViewModel.ToDate = tRSC03ViewModel.ToDate;
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
        public TRSC03ViewModel GetVtList(DateTime? FromDate, DateTime? ToDate, string Status)
        {
            var str = string.Empty;
            int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            if (Status == null) { Status = dropDownListBindWeb.GET_VTSTATUS().FirstOrDefault().Value; }
            if (FromDate == null) { var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); FromDate = first; }
            if (ToDate == null) { ToDate = DateTime.Today; }
            CommonViewModel.View_Active_List = new List<VCompleteVTInfo>();
            CommonViewModel.View_Active_List = vTMSCommonService.VtCompleteDTl().Where(x => x.Status == Status).Where(x => x.VtStartDate >= FromDate && x.VtEndDate <= ToDate).ToList();
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return CommonViewModel;
        }

        public async Task<IActionResult> GetListSearch()
        {
            int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            TRSC03ViewModel CommonViewModel = new TRSC03ViewModel();
            CommonViewModel = JsonConvert.DeserializeObject<TRSC03ViewModel>(TempData["CommonViewModel"].ToString());
            var statuslist = dropDownListBindWeb.GET_VTSTATUS();
            ViewBag.StatusLOV = statuslist;
            return View("Index", CommonViewModel);
        }

        public async Task<IActionResult> Index()
        {
            //return View(await _context.VtmsEnrollPi.ToListAsync());
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

            CommonViewModel.View_Active_List = new List<VCompleteVTInfo>();
            
            CommonViewModel.FromDate =DateTime.Today.AddMonths(-1); // Declaring from-date as the 1st day of the current month
            CommonViewModel.ToDate = DateTime.Today;    // Declaring to-date as the current date.

           // CommonViewModel.View_Active_List = vTMSCommonService.VtCompleteDTl()/*.Where(x => x.Status == "A").Where(x => x.EnrollmentStatus == "Enrolled").Where(x => x.VtStartDate != null).Where(x => x.VtEndDate >= DateTime.Today.AddDays(-17)).Where(x => x.UnitCode == unit)*/.ToList();
            //CommonViewModel.Edu_Msts = new VtmsEnrollEdu();
            
           // ViewBag.vtstatus = dropDownListBindWeb.GET_VTSTATUS();
           // CommonViewModel.Pi_Msts = new VtmsEnrollPi();
            return View(CommonViewModel);
        }
        public async Task<IActionResult> Details(string id)
        {
            CommonService commonService = new CommonService();
            if (id == null)
            {
                return NotFound();
            }

            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            CommonViewModel.Pi_Msts = new VtmsEnrollPi();
            CommonViewModel.Status = "edit";
            CommonViewModel.Pi_Msts = await _context.VtmsEnrollPi.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);

            CommonViewModel.Edu_Msts = new VtmsEnrollEdu();
            CommonViewModel.Edu_Msts = await _context.VtmsEnrollEdu.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit); 

            CommonViewModel.ProfileImage = vTMSCommonService.GetVTProfileImage(id, unit);

            CommonViewModel.Doc_Msts = new VtmsEnrollDoc();
            CommonViewModel.Doc_Msts = await _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);

            byte[] idproof = vTMSCommonService.GetVTIdProof(id, unit);

            if (idproof != null && idproof.Length > 0) { CommonViewModel.IdProof = vTMSCommonService.GetVTIdProof(id, unit); } else { CommonViewModel.IdProof = new byte[] { }; };
            if (CommonViewModel.Pi_Msts == null)
            {
                return NotFound();
            }

            return View("Details", CommonViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));

            var vtmsdocmsts = await _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
            CommonViewModel.Pi_Msts = await _context.VtmsEnrollPi.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
            var Edu_Msts = await _context.VtmsEnrollEdu.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
            CommonViewModel.Edu_Msts = Edu_Msts;
            CommonViewModel.InstituteName = _context.VtmsInstituteMsts.FirstOrDefault(X=> Convert.ToString(X.InstituteCd) == Edu_Msts.InstituteName).InstituteName ??  string.Empty;

            //vtmsdocmsts.CertFlag = "Y";
            _context.Update(vtmsdocmsts);
            await _context.SaveChangesAsync();

            return View(CommonViewModel);
        }
        // POST: M3/FP3/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VtCode,UnitCode,Name,FatherName,ContactNo,Address,DistrictName,StateName,Pincode,DocName,DocRegistrationNo,RecommendationType,OthersRecommName,RecommPno,VtStartDate,VtEndDate,Status,EnrollmentStatus,EnrolledBy,EnrolledOn,ManagedBy,ManagedOn,ModifiedBy,ModifiedOn,PrevVtCode")] VtmsEnrollPi vtmsEnrollPi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vtmsEnrollPi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vtmsEnrollPi);
        }

        //RDLC
    

        public JsonResult GetReport(string id)
        {
            string Report = "";
            string QueryString = String.Empty;
            var ReportName = string.Empty;
            if (!string.IsNullOrWhiteSpace(id))
            {
                ReportName = "ApplicantForm".ToString() + ".rep";
            }

            var ReportFormat = "pdf";
            string data = ReportName + "+destype=cache+desformat=" + ReportFormat;
            QueryString = "P_VtCode=" + id;
            return Json(Report);

        }


        // GET: M3/FP3/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vtmsEnrollPi = await _context.VtmsEnrollPi.FindAsync(id);
            if (vtmsEnrollPi == null)
            {
                return NotFound();
            }
            return View(vtmsEnrollPi);
        }

        // POST: M3/FP3/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("VtCode,UnitCode,Name,FatherName,ContactNo,Address,DistrictName,StateName,Pincode,DocName,DocRegistrationNo,RecommendationType,OthersRecommName,RecommPno,VtStartDate,VtEndDate,Status,EnrollmentStatus,EnrolledBy,EnrolledOn,ManagedBy,ManagedOn,ModifiedBy,ModifiedOn,PrevVtCode")] VtmsEnrollPi vtmsEnrollPi)
        {
            if (id != vtmsEnrollPi.VtCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vtmsEnrollPi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VtmsEnrollPiExists(vtmsEnrollPi.VtCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vtmsEnrollPi);
        }

        // GET: M3/FP3/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vtmsEnrollPi = await _context.VtmsEnrollPi
                .FirstOrDefaultAsync(m => m.VtCode == id);
            if (vtmsEnrollPi == null)
            {
                return NotFound();
            }

            return View(vtmsEnrollPi);
        }

        // POST: M3/FP3/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vtmsEnrollPi = await _context.VtmsEnrollPi.FindAsync(id);
            _context.VtmsEnrollPi.Remove(vtmsEnrollPi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VtmsEnrollPiExists(string id)
        {
            return _context.VtmsEnrollPi.Any(e => e.VtCode == id);
        }


        // chart test - returns JSON
        //[HttpPost]
        //public JsonResult ColumnChart1()
        //{
        //    int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
        //    string query = "SELECT MONTH_YEAR, ENROLLED,REJECTED FROM ";
        //    query = query + "( SELECT TRIM(TO_CHAR(ENROLLED_ON,'MONTH'))||'-'||TO_CHAR(SYSDATE,'YYYY') MONTH_YEAR, ENROLLMENT_STATUS  ";
        //    query = query + "  FROM VTMS_ENROLL_PI  ";
        //    query = query + " WHERE ENROLLED_ON BETWEEN ADD_MONTHS(SYSDATE,-3) AND ADD_MONTHS(SYSDATE,+9) AND UNIT_CODE = '" + unit + "' ) ";
        //    query = query + " PIVOT ( ";
        //    query = query + "   COUNT(ENROLLMENT_STATUS) ";
        //    query = query + "   FOR ENROLLMENT_STATUS IN ('Enrolled' ENROLLED, 'Rejected' REJECTED)) ";
        //    query = query + "   ORDER BY MONTH_YEAR ASC ";


        //    DataTable dt = _context.GetSQLQuery(query);
        //    var chartsdata = new List<ChartModel>();
        //    chartsdata = (from DataRow dr in dt.Rows
        //                  select new ChartModel
        //                  {
        //                      MonthYear = Convert.ToString(dr["MONTH_YEAR"]),
        //                      Enrolled = Convert.ToInt32(dr["ENROLLED"]),
        //                      Rejected = Convert.ToInt32(dr["REJECTED"]),
        //                  }).ToList();


        //    return Json(chartsdata);
        //}


    }
}
