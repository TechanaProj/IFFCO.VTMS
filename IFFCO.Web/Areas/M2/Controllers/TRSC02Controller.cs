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
    public class TRSC02Controller : BaseController<TRSC02ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TRSC02ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public TRSC02Controller(ModelContext context)
        {
                _context = context;
                commonException = new CommonException<TRSC02ViewModel>();
                dropDownListBindWeb  = new DropDownListBindWeb();
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
               // CommonViewModel.vCompleteVTInfos = vTMSCommonService.VtCompleteDTl();
                CommonViewModel.FromDate = DateTime.Today.AddMonths(-1); // Declaring from-date as the 1st day of the current month
                CommonViewModel.ToDate = DateTime.Today;    // Declaring to-date as the current date.

            return View(CommonViewModel);
        }




        // GET: M2/TRSC02/Details/5
        public async Task<IActionResult> Details(string id)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            if (id == null)
            {
                return NotFound();
            }

            CommonViewModel.R_Msts = new VtmsVtReview();
            CommonViewModel.Status = "Details";
            CommonViewModel.VtCode = id;
            CommonViewModel.UnitCode = unit;
            CommonViewModel.ActionMode = "disabled";
            CommonViewModel.R_Msts = await _context.VtmsVtReview.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit) ?? new VtmsVtReview();
            var reviewList = dropDownListBindWeb.GET_Review();
            ViewBag.ReviewLOV = reviewList;
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return View("Edit", CommonViewModel);
        }

        public IActionResult Query(TRSC02ViewModel trsc02ViewModel, string Status)
        { // Called when the 'Query' button on index page is pressed.
            try
            {
                CommonViewModel = GetVtList(Convert.ToDateTime(trsc02ViewModel.FromDate), Convert.ToDateTime(trsc02ViewModel.ToDate), trsc02ViewModel.Status); // Populating the VT List using the 'GetVtList' function declared in same file 
                CommonViewModel.Status = trsc02ViewModel.Status;
                CommonViewModel.FromDate = trsc02ViewModel.FromDate;
                CommonViewModel.ToDate = trsc02ViewModel.ToDate;
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
        public TRSC02ViewModel GetVtList(DateTime? FromDate, DateTime? ToDate, string Status)
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
            TRSC02ViewModel CommonViewModel = new TRSC02ViewModel();
            CommonViewModel = JsonConvert.DeserializeObject<TRSC02ViewModel>(TempData["CommonViewModel"].ToString());
            return View("Index", CommonViewModel);
        }


        // GET: M2/TRSC02/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            if (id == null)
            {
                return NotFound();
            }

            CommonViewModel.R_Msts = new VtmsVtReview();
            CommonViewModel.Status = "Edit";
            CommonViewModel.VtCode = id;
            CommonViewModel.UnitCode = unit;
            CommonViewModel.ActionMode = "Edit";
            CommonViewModel.R_Msts = await _context.VtmsVtReview.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
            var reviewList = dropDownListBindWeb.GET_Review();
            ViewBag.ReviewLOV = reviewList;
            return View(CommonViewModel);
        }

        // POST: M2/TRSC02/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TRSC02ViewModel tRSC02ViewModel)
        {
            try
            {
                int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                int count = _context.VtmsVtReview.Where(x => x.VtCode.Equals(id)).ToList().Count;
               
                {
                    if (count == 0)
                    {
                        tRSC02ViewModel.R_Msts.VtCode = tRSC02ViewModel.VtCode;
                        tRSC02ViewModel.R_Msts.UnitCode = tRSC02ViewModel.UnitCode;
                        tRSC02ViewModel.R_Msts.CreatedOn = DateTime.UtcNow;
                        tRSC02ViewModel.R_Msts.CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                        _context.VtmsVtReview.Add(tRSC02ViewModel.R_Msts);
                        await _context.SaveChangesAsync();
                        CommonViewModel.Message = "VtCode" + tRSC02ViewModel.R_Msts.VtCode;
                        CommonViewModel.Alert = "Create";
                        CommonViewModel.Status = "Create";
                        CommonViewModel.ErrorMessage = "";
                    }
                    else if (tRSC02ViewModel.VtCode != null && count > 0) 
                    {

                        VtmsVtReview Obj = new VtmsVtReview();
                        Obj = _context.VtmsVtReview.FirstOrDefault(x => x.VtCode.Equals(tRSC02ViewModel.VtCode));
                        Obj.UnitCode = tRSC02ViewModel.UnitCode;
                        Obj.VtevaL0001 = tRSC02ViewModel.R_Msts.VtevaL0001;
                        Obj.VtevaL0002 = tRSC02ViewModel.R_Msts.VtevaL0002;
                        Obj.VtevaL0003 = tRSC02ViewModel.R_Msts.VtevaL0003;
                        Obj.VtevaL0004 = tRSC02ViewModel.R_Msts.VtevaL0004;
                        Obj.VtevaL0005 = tRSC02ViewModel.R_Msts.VtevaL0004;
                        Obj.VtevaL0005 = tRSC02ViewModel.R_Msts.VtevaL0005;
                        Obj.VtevaL0006 = tRSC02ViewModel.R_Msts.VtevaL0006;
                        Obj.VtevaL0007 = tRSC02ViewModel.R_Msts.VtevaL0007;
                        Obj.VtevaL0008 = tRSC02ViewModel.R_Msts.VtevaL0008;
                        Obj.VtevaL0009 = tRSC02ViewModel.R_Msts.VtevaL0009;
                        Obj.VtevaL0010 = tRSC02ViewModel.R_Msts.VtevaL0010;
                        Obj.VtevaL0011 = tRSC02ViewModel.R_Msts.VtevaL0011;
                        Obj.VtevaL0012 = tRSC02ViewModel.R_Msts.VtevaL0012;
                        Obj.VtevaL0013 = tRSC02ViewModel.R_Msts.VtevaL0013;
                        Obj.VtevaL0014 = tRSC02ViewModel.R_Msts.VtevaL0014;
                        Obj.VtevaL0015 = tRSC02ViewModel.R_Msts.VtevaL0015;
                        Obj.VtevaL0016 = tRSC02ViewModel.R_Msts.VtevaL0016;
                        Obj.VtevaL0017 = tRSC02ViewModel.R_Msts.VtevaL0017;
                        Obj.VtevaL0018 = tRSC02ViewModel.R_Msts.VtevaL0018;
                        Obj.VtevaL0019 = tRSC02ViewModel.R_Msts.VtevaL0019;
                        Obj.CreatedOn = DateTime.Now;
                        Obj.CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                        _context.VtmsVtReview.Update(Obj);
                        await _context.SaveChangesAsync();
                        CommonViewModel.Message = "VtCode" + tRSC02ViewModel.VtCode;
                        CommonViewModel.Alert = "Update";
                        CommonViewModel.Status = "Update";
                        CommonViewModel.ErrorMessage = "";
                    }
                    else
                    {
                        CommonViewModel.Message = "Invalid Category";
                        CommonViewModel.ErrorMessage = "Invalid Category";
                        CommonViewModel.Alert = "Warning";
                        CommonViewModel.Status = "Warning";
                     }

                }
            }
            catch (Exception ex)
            {
                        commonException.GetCommonExcepton(CommonViewModel, ex);
                        CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                        CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                        return Json(CommonViewModel);
            }

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

    }

}
