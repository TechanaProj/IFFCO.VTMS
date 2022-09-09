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
using IFFCO.VTMS.Web.Models;
using IFFCO.VTMS.Web.CommonFunctions;
using ModelContext = IFFCO.VTMS.Web.Models.ModelContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IFFCO.VTMS.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENM04Controller : BaseController<ENM04ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        CommonException<ENM04ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENM04Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENM04ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            primaryKeyGen = new PrimaryKeyGen();
            vTMSCommonService = new VTMSCommonService();
        }


        // GET: ENM04Controller// int in model (check at insertion )
        public ActionResult Index(ENM04ViewModel eNM04ViewModel)
        {
            //var data = _context.VtmsUniversityMsts.ToList();
            eNM04ViewModel.fstatecd = null;
            var data = vTMSCommonService.getuniversity();
            ViewBag.State = dropDownListBindWeb.GetState();
            CommonViewModel.vtmsUniversities = data;
            return View(CommonViewModel);
        }

        // GET: ENM04Controller/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ENM04Controller/Create
        public ActionResult Create(ENM04ViewModel eNM04ViewModel)
        {           
            ViewBag.State = dropDownListBindWeb.GetState();
            ViewBag.districtlov = dropDownListBindWeb.GET_District();
            var ObjUid = new VtmsUniversityMsts() { UniversityId = _context.VtmsUniversityMsts.OrderByDescending(x => x.UniversityId).FirstOrDefault().UniversityId + 1 };
            CommonViewModel.vtmsUniversities = vTMSCommonService.getuniversity();
            CommonViewModel.vtmsUniversity = ObjUid;
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Status = "Create";
            return View("Index", CommonViewModel);
        }

        // POST: ENM04Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ENM04ViewModel eNM04ViewModel, IFormCollection collection)
        {
            try
            {
                int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                if (!string.IsNullOrWhiteSpace(eNM04ViewModel.vtmsUniversity.UniversityName))
                {
                    VtmsUniversityMsts oobj = new VtmsUniversityMsts();
                    oobj.UniversityId = _context.VtmsUniversityMsts.OrderByDescending(x => x.UniversityId).FirstOrDefault().UniversityId + 1;
                    // eNM04ViewModel.vtmsUniversity.DistrictName=_context.VtmsUniversityMsts.
                    oobj.UniversityName = eNM04ViewModel.vtmsUniversity.UniversityName;
                    oobj.DistrictName=eNM04ViewModel.vtmsUniversity.DistrictName;   
                    oobj.CreationDatetime = DateTime.UtcNow;
                    oobj.CreatedBy = PersonnelNumber;
                    _context.VtmsUniversityMsts.Add(oobj);
                    await _context.SaveChangesAsync();
                   // CommonViewModel.vtmsUniversities = _context.VtmsUniversityMsts.ToList();
                    CommonViewModel.Message = "University Id " + Convert.ToString(eNM04ViewModel.vtmsUniversity.UniversityId);
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Invalid University Details. Try Again!";
                    CommonViewModel.ErrorMessage = "Invalid University Details. Try Again!";
                    CommonViewModel.Alert = "Warning";
                    CommonViewModel.Status = "Warning";
                }

            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return View(CommonViewModel);
            }


            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        // GET: ENM04Controller/Edit/5
        public ActionResult Edit(int id , ENM04ViewModel eNM04ViewModel , string district)
        {
            if (id != null)
            {
                ViewBag.State = dropDownListBindWeb.GetState();
                //ViewBag.districtlov = dropDownListBindWeb.GET_District();
                string Disttcd = _context.VtmsUniversityMsts.FirstOrDefault(x => x.UniversityId == id).DistrictName;
                CommonViewModel.fstatecd = _context.MDistrict.FirstOrDefault(x => x.DisttCd == Disttcd).StateCd;
                CommonViewModel.vtmsUniversity = _context.VtmsUniversityMsts.SingleOrDefault(x => x.UniversityId == id);
                CommonViewModel.vtmsUniversities = vTMSCommonService.getuniversity();
                //CommonViewModel.vtmsUniversities = data; 
            }
            else
            {
                CommonViewModel.Message = "University ID Unavailable";
                CommonViewModel.ErrorMessage = "University ID Unavailable";
                CommonViewModel.Alert = "Warning";
                CommonViewModel.Status = "Warning";
            }
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
            CommonViewModel.Status = "Edit";
            return View("Index", CommonViewModel);
        }

        // POST: ENM04Controller/Edit/5
        // POST: M1/SUGGM01/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ENM04ViewModel eNM04ViewModel)
         {
            try
            {
                if (eNM04ViewModel.vtmsUniversity.UniversityId > 0)
                {
                    VtmsUniversityMsts Obj = new VtmsUniversityMsts();
                    Obj = _context.VtmsUniversityMsts.FirstOrDefault(x => x.UniversityId.Equals(eNM04ViewModel.vtmsUniversity.UniversityId));
                    Obj.UniversityName = eNM04ViewModel.vtmsUniversity.UniversityName;
                    Obj.DistrictName = eNM04ViewModel.vtmsUniversity.DistrictName;
                    Obj.CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    Obj.CreationDatetime = DateTime.Now;
                    _context.VtmsUniversityMsts.Update(Obj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "University Id " + eNM04ViewModel.vtmsUniversity.UniversityId;
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
        // GET: ENM04Controller/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var University = await _context.VtmsUniversityMsts.FirstOrDefaultAsync(m => m.UniversityId == id);
            if (University == null)
            {
                return NotFound();
            }

            return View(University);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id != null)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        int count = _context.VtmsEnrollEdu.Where(x => x.UniversityName == id).ToList().Count;
                        var DeleteDataTemp =  _context.VtmsUniversityMsts.FirstOrDefault(x=>x.UniversityId == Convert.ToInt32(id));
                        if (DeleteDataTemp != null && count == 0)
                        {
                            _context.VtmsUniversityMsts.Remove(DeleteDataTemp);
                            await _context.SaveChangesAsync();
                            CommonViewModel.Message = "University Id - " + id;
                            CommonViewModel.Alert = "Delete";
                            CommonViewModel.Status = "Delete";
                            CommonViewModel.ErrorMessage = "";
                        }
                        else
                        {
                            CommonViewModel.Message = "Cannot Perform Delete Operation. University already used in Records";
                            CommonViewModel.ErrorMessage = "Cannot Perform Delete Operation. University already used in Records";
                            CommonViewModel.Alert = "Warning";
                            CommonViewModel.Status = "Warning";
                        }


                    }
                    else
                    {
                        CommonViewModel.Message = "University Unavailable";
                        CommonViewModel.ErrorMessage = "University Unavailable";
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

        public List<SelectListItem> DistrictLOVBind(string StateCd)
        {
            List<SelectListItem> disttCDLOV = new List<SelectListItem>();
            disttCDLOV = dropDownListBindWeb.GET_District(StateCd);
            return disttCDLOV;
        }

    }  
}
