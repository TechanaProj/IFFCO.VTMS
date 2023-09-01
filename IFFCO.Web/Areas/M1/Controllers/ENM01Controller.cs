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
using IFFCO.VTMS.Web.Areas.M1.Controllers;
using IFFCO.HRMS.Entities.AppConfig;
using System.Net;
using IFFCO.VTMS.Web.Controllers;
using IFFCO.VTMS.Web.ViewModels;
using IFFCO.VTMS.Web.CommonFunctions;
using ModelContext = IFFCO.VTMS.Web.Models.ModelContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using IFFCO.HRMS.Shared.CommonFunction;
using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace IFFCO.VTMS.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENM01Controller : BaseController<ENM01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENM01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENM01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENM01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }

        // GET: ENM01Controllern
        public ActionResult Index(string id)
        {
            var CourseLOV = dropDownListBindWeb.CourseLOVBind();
            CommonViewModel.CourseLOVBind = CourseLOV;

            CommonViewModel.CourseCode = CourseLOV.FirstOrDefault().Value;
            //var data = _context.VtmsBranchMsts.Where(x => x.CourseCode == CourseLOV.FirstOrDefault().Value).ToList();
            var data = vTMSCommonService.GetBranchMaster(CourseLOV.FirstOrDefault().Value);
            CommonViewModel.VtmsBranchMstsList = data;
            return View(CommonViewModel);
        }

        public async Task<IActionResult> ViewMsts(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Model = PopulateSub(id);
            CommonViewModel = Model;
            ViewBag.CourseLOV = dropDownListBindWeb.CourseLOVBind();
            return View("Index", CommonViewModel);
        }

        public ENM01ViewModel PopulateSub(string id)
        {
            ENM01ViewModel view = new ENM01ViewModel();
            var LOV = dropDownListBindWeb.CourseLOVBind();
            view.CourseLOVBind = LOV;
            if (id == null) { id = LOV.FirstOrDefault().Value; }
            view.CourseCode = id;
            //view.VtmsBranchMstsList = _context.VtmsBranchMsts.Where(x => x.CourseCode == id).ToList();
            view.VtmsBranchMstsList = vTMSCommonService.GetBranchMaster(id);
            return view;
        }


        //GET: ENM01Controller/Create
        [HttpGet]
        public ActionResult Create(string id)
        {
            
            var ObjBid = new VtmsBranchMsts() { BranchId = _context.VtmsBranchMsts.OrderByDescending(x => x.BranchId).FirstOrDefault().BranchId + 1 };
            CommonViewModel.VtmsBranchMstsList = vTMSCommonService.GetBranchMaster(id);
            CommonViewModel.Branch = ObjBid;
            CommonViewModel.CourseCode = id;
            CommonViewModel.CourseLOVBind = dropDownListBindWeb.CourseLOVBind();
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Status = "Create";
            return View("Index", CommonViewModel);
        }

        //// POST: ENM01Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ENM01ViewModel ENM01ViewModel, string id, IFormCollection collection)
        {
            //convertDateTostring(date)
            try
            {
                int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                if (!string.IsNullOrWhiteSpace(ENM01ViewModel.Branch.BranchDesc))
                {
                    ENM01ViewModel.Branch.BranchId = _context.VtmsBranchMsts.OrderByDescending(x => x.BranchId).FirstOrDefault().BranchId + 1;
                    ENM01ViewModel.Branch.CreatedDateTime = DateTime.UtcNow;
                    ENM01ViewModel.Branch.CreatedBy = PersonnelNumber;
                   // ENM01ViewModel.Branch.CourseCode = id;
                    _context.VtmsBranchMsts.Add(ENM01ViewModel.Branch);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Branch ID " + Convert.ToString(ENM01ViewModel.Branch.BranchId);
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
                    CommonViewModel.ErrorMessage = "";
                    ENM01ViewModel.BranchCode = null;
                }
                else
                {    
                   
                    CommonViewModel.Message = "Invalid Branch Details. Try Again!";
                    CommonViewModel.ErrorMessage = "Invalid Branch Details. Try Again!";
                    CommonViewModel.Alert = "Warning";
                    CommonViewModel.Status = "Warning";
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

        // GET: ENM01Controller/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {


            var CourseLOV = dropDownListBindWeb.CourseLOVBind();
            CommonViewModel.CourseLOVBind = CourseLOV;
            
            //var data = _context.VtmsBranchMsts.Where(x => x.CourseCode == CourseLOV.FirstOrDefault().Value).ToList();
            var data = vTMSCommonService.GetBranchMaster(CourseLOV.FirstOrDefault().Value);
            if (id != null)
            {
                var Branch = _context.VtmsBranchMsts.SingleOrDefault(x => x.BranchId == id);
                CommonViewModel.Branch = Branch;
                CommonViewModel.CourseCode = Branch.CourseCode;
                CommonViewModel.VtmsBranchMstsList = _context.VtmsBranchMsts.ToList();
            }



            else
            {
                CommonViewModel.Message = "Branch ID Unavailable";
                CommonViewModel.ErrorMessage = "Branch ID Unavailable";
                CommonViewModel.Alert = "Warning";
                CommonViewModel.Status = "Warning";
            }


            
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
            CommonViewModel.Status = "Edit";
            return View("Index", CommonViewModel);



        }



        //// POST: ENM01Controller/Edit/5

        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ENM01ViewModel ENM01ViewModel)
        {
            try
            {
                if (ENM01ViewModel.Branch.BranchId > 0)
                {
                    VtmsBranchMsts Obj = new VtmsBranchMsts();
                    Obj = _context.VtmsBranchMsts.FirstOrDefault(x => x.BranchId.Equals(ENM01ViewModel.Branch.BranchId));
                    Obj.BranchCode = ENM01ViewModel.Branch.BranchCode;
                    Obj.BranchDesc = ENM01ViewModel.Branch.BranchDesc;
                    Obj.CourseCode = ENM01ViewModel.Branch.CourseCode;
                    Obj.CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    //Obj.CreationDatetime = DateTime.Now;
                    _context.VtmsBranchMsts.Update(Obj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Branch Id " + ENM01ViewModel.Branch.BranchId;
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

        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var VtmsBranchMsts = await _context.VtmsBranchMsts.FirstOrDefaultAsync(m => m.BranchId == id);
            if (VtmsBranchMsts == null)
            {
                return NotFound();
            }

            return View(VtmsBranchMsts);
        }

        //POST: M1/ENM01/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id != null)
            {
                try
                {
                    if (id > 0)
                    {
                        int count = _context.VtmsEnrollEdu.Where(x => x.BranchName.Equals(id)).ToList().Count;
                        var DeleteDataTemp = await _context.VtmsBranchMsts.FindAsync(id);
                        if (DeleteDataTemp != null && count == 0)
                        {
                            _context.VtmsBranchMsts.Remove(DeleteDataTemp);
                            await _context.SaveChangesAsync();
                            CommonViewModel.Message = "BranchId- " + id;
                            CommonViewModel.Alert = "Delete";
                            CommonViewModel.Status = "Delete";
                            CommonViewModel.ErrorMessage = "";
                        }
                        else
                        {
                            CommonViewModel.Message = "Cannot Perform Delete Operation. Branch Id already used in Branch Master";
                            CommonViewModel.ErrorMessage = "Cannot Perform Delete Operation.Branch Id already used in Branch Master";
                            CommonViewModel.Alert = "Warning";
                            CommonViewModel.Status = "Warning";
                        }

                    }
                    else
                    {
                        CommonViewModel.Message = "Branch Id Unavailable";
                        CommonViewModel.ErrorMessage = "Branch Id Unavailable";
                        CommonViewModel.Alert = "Warning";
                        CommonViewModel.Status = "Warning";
                    }

                }
                catch (Exception ex)
                {
                    commonException.GetCommonExcepton(CommonViewModel, ex);
                    CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                    CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();

                }
            }

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        



    }
}