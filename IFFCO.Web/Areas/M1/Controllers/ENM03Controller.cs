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
    public class ENM03Controller : BaseController<ENM03ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        CommonException<ENM03ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENM03Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENM03ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            primaryKeyGen = new PrimaryKeyGen();
            vTMSCommonService = new VTMSCommonService();
        }

        // GET: ENM03Controller
        public ActionResult Index(int id)
        {
            var UniversityLOV = dropDownListBindWeb.UniversityLOVBind();
            CommonViewModel.UniversityLOV = UniversityLOV;
            CommonViewModel.UniversityId = UniversityLOV.FirstOrDefault().Value;
            //var data = _context.VtmsInstituteMsts.Where(x => x.UniversityId == UniversityLOV.FirstOrDefault().Value).ToList();
            var data = vTMSCommonService.GetInstituteMaster(UniversityLOV.FirstOrDefault().Value);
            var StateLOV = dropDownListBindWeb.StateLOVBind();
            CommonViewModel.StateLOV = StateLOV;
            CommonViewModel.StateName = StateLOV.FirstOrDefault().Value;
            var DistrictLOV = dropDownListBindWeb.DistrictLOVBind();
            CommonViewModel.DistrictLOV = DistrictLOV;
            CommonViewModel.DistrictName = DistrictLOV.FirstOrDefault().Value;
            CommonViewModel.vtmsInstitiutelist = data;
            return View(CommonViewModel);
        }

       

        public JsonResult ddl1(string StateCd)
        {
            var DistrictLOV = _context.MDistrict.Where(X=>X.StateCd == StateCd).Select(x => new SelectListItem
            {
                Text = string.Concat(x.DisttCd, " - ", x.DisttName),
                Value = x.DisttCd.ToString()
            }).ToList();
            return Json(DistrictLOV);
        }

        public async Task<IActionResult> ViewMsts(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Model = PopulateSub(id);
            CommonViewModel = Model;
            ViewBag.UniversityLOV = dropDownListBindWeb.UniversityLOVBind();
            return View("Index", CommonViewModel);
        }

        public ENM03ViewModel PopulateSub(string id)
        {
            ENM03ViewModel view = new ENM03ViewModel();
            var LOV = dropDownListBindWeb.UniversityLOVBind();
            view.UniversityLOV = LOV;
            if (id == null) { id = LOV.FirstOrDefault().Value; }
            view.UniversityId = id;
            //view.vtmsInstitiutelist = _context.VtmsInstituteMsts.Where(x => x.UniversityId == id).ToList();
            view.vtmsInstitiutelist = vTMSCommonService.GetInstituteMaster(id);
            return view;
        }

        // GET: ENM03Controller/Create
        public ActionResult Create(string id)
        {

            var ObjIid = new VtmsInstituteMsts() { InstituteCd = _context.VtmsInstituteMsts.OrderByDescending(x => x.InstituteCd).FirstOrDefault().InstituteCd + 1 };
            //CommonViewModel.vtmsInstitiutelist = _context.VtmsInstituteMsts.Where(x => x.UniversityId == UniversityLOV.FirstOrDefault().Value).ToList();
            // CommonViewModel.vtmsInstitiutelist = vTMSCommonService.GetInstituteMaster(UniversityLOV.FirstOrDefault().Value);
            CommonViewModel.vtmsInstitiutelist = vTMSCommonService.GetInstituteMaster(id);
            CommonViewModel.vtmsInstitiute = ObjIid;
            CommonViewModel.UniversityId = id;
            CommonViewModel.UniversityLOV = dropDownListBindWeb.UniversityLOVBind();
            CommonViewModel.StateLOV = dropDownListBindWeb.StateLOVBind();
            CommonViewModel.DistrictLOV = dropDownListBindWeb.DistrictLOVBind();
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Status = "Create";
            return View("Index", CommonViewModel);
        }

        // POST: ENM03Controller/Create/INSERTION/UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ENM03ViewModel eNM03ViewModel, string id,IFormCollection collection)
        {
            try
            {
                int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                if (!string.IsNullOrWhiteSpace(eNM03ViewModel.vtmsInstitiute.InstituteName))
                {
                    eNM03ViewModel.vtmsInstitiute.InstituteCd = _context.VtmsInstituteMsts.OrderByDescending(x => x.InstituteCd).FirstOrDefault().InstituteCd+ 1;
                    eNM03ViewModel.vtmsInstitiute.CreatedDatetime = DateTime.UtcNow;
                    eNM03ViewModel.vtmsInstitiute.CreatedBy = PersonnelNumber.ToString();
                    eNM03ViewModel.vtmsInstitiute.UniversityId = id;
                    _context.VtmsInstituteMsts.Add(eNM03ViewModel.vtmsInstitiute);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Institute CD " + Convert.ToString(eNM03ViewModel.vtmsInstitiute.InstituteCd);
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Invalid Institute Details. Try Again!";
                    CommonViewModel.ErrorMessage = "Invalid Institute Details. Try Again!";
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


        // GET: M1/ENM03/Edit/
        public async Task<IActionResult> Edit(int id, string uni)
        {
            var UniversityLOV = dropDownListBindWeb.UniversityLOVBind();
            CommonViewModel.UniversityLOV = UniversityLOV;
            CommonViewModel.UniversityId = uni;
            var StateLOV = dropDownListBindWeb.StateLOVBind();
            CommonViewModel.StateLOV = StateLOV;
            var DistrictLOV = dropDownListBindWeb.DistrictLOVBind();
            CommonViewModel.DistrictLOV = DistrictLOV;
            int count = UniversityLOV.Where(x => x.Value == uni).Count();
            if (id != null)
            {
                 CommonViewModel.vtmsInstitiute = _context.VtmsInstituteMsts.FirstOrDefault(x => x.InstituteCd == id);
                //CommonViewModel.vtmsInstitiutelist = _context.VtmsInstituteMsts.Where(x => x.UniversityId == uni).ToList();
                CommonViewModel.vtmsInstitiutelist = vTMSCommonService.GetInstituteMaster(uni);
            }
            else
            {
                    CommonViewModel.Message = "Institute Cd Unavailable";
                    CommonViewModel.ErrorMessage = "Institute Cd Unavailable";
                    CommonViewModel.Alert = "Warning";
                    CommonViewModel.Status = "Warning";
            }

                    CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
                    CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
                    CommonViewModel.Status = "Edit";
                    return View("Index", CommonViewModel);
        }


        // POST: ENM04Controller/Edit/
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ENM03ViewModel eNM03ViewModel)
        {
            try
            {
                if (eNM03ViewModel.vtmsInstitiute.InstituteCd>0)
                {
                    VtmsInstituteMsts Obj = new VtmsInstituteMsts();
                    Obj = _context.VtmsInstituteMsts.FirstOrDefault(x => x.InstituteCd.Equals(eNM03ViewModel.vtmsInstitiute.InstituteCd));
                    Obj.InstituteName = eNM03ViewModel.vtmsInstitiute.InstituteName;
                    Obj.CreatedBy = Convert.ToString(Convert.ToInt32(HttpContext.Session.GetInt32("EmpID")));
                    Obj.CreatedDatetime = DateTime.Now;
                    Obj.InstituteCd = eNM03ViewModel.vtmsInstitiute.InstituteCd;
                    Obj.InstituteName = eNM03ViewModel.vtmsInstitiute.InstituteName;
                    Obj.InstituteType = eNM03ViewModel.vtmsInstitiute.InstituteType;
                    Obj.StateName = eNM03ViewModel.vtmsInstitiute.StateName;
                    Obj.DistrictName = eNM03ViewModel.vtmsInstitiute.DistrictName;
                    _context.VtmsInstituteMsts.Update(Obj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "InstituteCd" + eNM03ViewModel.vtmsInstitiute.InstituteCd;
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

        public async Task<IActionResult> Delete(int id)
        {
                if (id == null)
                {
                    return NotFound();
                }

                    var VtmsInstituteMsts = await _context.VtmsInstituteMsts.FirstOrDefaultAsync(m => m.InstituteCd == id);
                    if (VtmsInstituteMsts == null)
                {
                    return NotFound();
                }

                    return View(VtmsInstituteMsts);
        }

        //POST: M1/ENM03/Delete/
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id != null)
            {
                try
                {
                    if (id > 0)
                    {
                        int count = _context.VtmsEnrollEdu.Where(x => x.InstituteName.Equals(id)).ToList().Count;
                        var DeleteDataTemp = await _context.VtmsInstituteMsts.FindAsync(id);
                        if (DeleteDataTemp != null && count == 0)
                        {
                            _context.VtmsInstituteMsts.Remove(DeleteDataTemp);
                            await _context.SaveChangesAsync();
                            CommonViewModel.Message = "InstituteCd- " + id;
                            CommonViewModel.Alert = "Delete";
                            CommonViewModel.Status = "Delete";
                            CommonViewModel.ErrorMessage = "";
                        }
                        else
                        {
                            CommonViewModel.Message = "Cannot Perform Delete Operation. Institute Cd already used in Institute Master";
                            CommonViewModel.ErrorMessage = "Cannot Perform Delete Operation.Institute Cd already used in Institute Master";
                            CommonViewModel.Alert = "Warning";
                            CommonViewModel.Status = "Warning";
                        }

                    }
                        else
                        {
                            CommonViewModel.Message = "Institute Cd Unavailable";
                            CommonViewModel.ErrorMessage = "Institute Cd Unavailable";
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




