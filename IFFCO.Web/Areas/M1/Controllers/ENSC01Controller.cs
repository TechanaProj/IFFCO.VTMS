using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Service;
using IFFCO.VTMS.Web.CommonFunctions;
using IFFCO.VTMS.Web.Controllers;
using IFFCO.VTMS.Web.Models;
using IFFCO.VTMS.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.VTMS.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENSC01Controller : BaseController<ENSC01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENSC01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENSC01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENSC01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }

        public IActionResult Index()
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));

            if (Convert.ToString(TempData["Message"]) != "")
            {
                CommonViewModel.Message = Convert.ToString(TempData["Message"]);
                CommonViewModel.Alert = Convert.ToString(TempData["Alert"]);
                CommonViewModel.Status = Convert.ToString(TempData["Status"]);
                CommonViewModel.ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
            }

            CommonViewModel.View_List = new List<VCompleteVTInfo>();
            CommonViewModel.View_List = vTMSCommonService.VtCompleteDTl();      
            return View(CommonViewModel);
        }

        // GET: ENSC01Controller/CREATE
        public IActionResult Create()
        {

            var CourseLOV = dropDownListBindWeb.ListCourseBind();
            //var UniversityLOV = dropDownListBindWeb.ListUniversityBind();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var RecommLOV = dropDownListBindWeb.ListRecommBind(unit); 
            var StateLov = dropDownListBindWeb.ListStateBind();
            ViewBag.CourseList = CourseLOV.ToList();
            //ViewBag.UniversityList = UniversityLOV.ToList();
            ViewBag.RecommList = RecommLOV.ToList();
            ViewBag.StateList = StateLov.ToList();

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Status = "Create";
            return View(CommonViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ENSC01ViewModel eNSC01ViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    eNSC01ViewModel.Pi_Msts.VtCode = primaryKeyGen.Get_EnrolledVTCode_PK(Convert.ToInt32(HttpContext.Session.GetString("UnitCode")));
                    eNSC01ViewModel.Pi_Msts.EnrolledOn = eNSC01ViewModel.Edu_Msts.EnrolledOn = DateTime.Now;
                    eNSC01ViewModel.Pi_Msts.EnrolledBy = eNSC01ViewModel.Edu_Msts.EnrolledBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    eNSC01ViewModel.Edu_Msts.EnrolledOn = eNSC01ViewModel.Edu_Msts.EnrolledOn = DateTime.Now;
                    eNSC01ViewModel.Edu_Msts.EnrolledBy = eNSC01ViewModel.Edu_Msts.EnrolledBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    eNSC01ViewModel.Edu_Msts.VtCode = eNSC01ViewModel.Pi_Msts.VtCode;
                    eNSC01ViewModel.Edu_Msts.UnitCode = eNSC01ViewModel.Pi_Msts.UnitCode = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
                    eNSC01ViewModel.Pi_Msts.OthersRecommName = eNSC01ViewModel.OthersRecommName;
                    eNSC01ViewModel.Pi_Msts.Status = "N";
                    if (eNSC01ViewModel.Edu_Msts.BranchName == "null") { eNSC01ViewModel.Edu_Msts.BranchName = String.Empty; }
                    _context.Add(eNSC01ViewModel.Pi_Msts);
                    _context.Add(eNSC01ViewModel.Edu_Msts);
                   
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = eNSC01ViewModel.Pi_Msts.VtCode + " | " + eNSC01ViewModel.Pi_Msts.Name;
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Invalid Course Details. Try Again!";
                    CommonViewModel.ErrorMessage = "Invalid Course Details. Try Again!";
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
                // return View(eNSC01ViewModel);
            }
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
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

        public List<SelectListItem> BranchLOVBindJSON(string CourseCode)
        {
            List<SelectListItem> BranchCodeLOV = new List<SelectListItem>();
            BranchCodeLOV = dropDownListBindWeb.ListBranchBind(CourseCode);
            return BranchCodeLOV;
        }

        public List<SelectListItem> InstituteLOVBindJSON(string UniversityId)
        {
            List<SelectListItem> InstituteCodeLOV = new List<SelectListItem>();
            InstituteCodeLOV = dropDownListBindWeb.ListInstituteBind(UniversityId);
            return InstituteCodeLOV;
        }

        // GET: ENSC01Controller/Edit
        public IActionResult Edit(string id, ENSC01ViewModel eNSC01ViewModel)
        {
            if (id != null)
            {
               
                int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
                CommonViewModel.Pi_Msts = new VtmsEnrollPi();
                CommonViewModel.Status = "edit";
                CommonViewModel.Pi_Msts = _context.VtmsEnrollPi.FirstOrDefault(x => x.VtCode == id && x.UnitCode == unit);
                CommonViewModel.Edu_Msts = new VtmsEnrollEdu();
                CommonViewModel.Edu_Msts = _context.VtmsEnrollEdu.FirstOrDefault(x => x.VtCode == id && x.UnitCode == unit);

                var statelist = dropDownListBindWeb.ListStateBind();
                ViewBag.StateList = statelist;
                
                var courselist = dropDownListBindWeb.ListCourseBind();
                ViewBag.CourseList = courselist;

                var UniversityList = dropDownListBindWeb.ListUniversityBind();
                ViewBag.UniversityList = UniversityList;

                var RecommLOV = dropDownListBindWeb.ListRecommBind(unit);
                ViewBag.RecommList = RecommLOV.ToList();

            }
            else
            {
                CommonViewModel.Message = "District Cd Unavailable";
                CommonViewModel.ErrorMessage = "District Cd Unavailable";
                CommonViewModel.Alert = "Warning";
                CommonViewModel.Status = "Warning";
            }

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
            CommonViewModel.Status = "Edit";
            return View("Edit", CommonViewModel);
        }

        // POST: ENSC01Controller/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(ENSC01ViewModel eNSC01ViewModel)
        {
            try
            {
                if (eNSC01ViewModel.Pi_Msts != null || eNSC01ViewModel.Edu_Msts != null)   
                    {
                    VtmsEnrollPi PiObj = _context.VtmsEnrollPi.FirstOrDefault(x => x.VtCode  == eNSC01ViewModel.Pi_Msts.VtCode);       
                    VtmsEnrollEdu EdObj = _context.VtmsEnrollEdu.FirstOrDefault(x => x.VtCode == eNSC01ViewModel.Pi_Msts.VtCode);       
                    PiObj.Name = eNSC01ViewModel.Pi_Msts.Name;
                    PiObj.FatherName = eNSC01ViewModel.Pi_Msts.FatherName;
                    PiObj.Address = eNSC01ViewModel.Pi_Msts.Address;
                    PiObj.ContactNo = eNSC01ViewModel.Pi_Msts.ContactNo;
                    PiObj.StateName = eNSC01ViewModel.Pi_Msts.StateName;
                    PiObj.DistrictName = eNSC01ViewModel.Pi_Msts.DistrictName;
                    PiObj.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    PiObj.ModifiedOn = DateTime.Now;
                    EdObj.CourseName = eNSC01ViewModel.Edu_Msts.CourseName;
                    EdObj.BranchName = eNSC01ViewModel.Edu_Msts.BranchName;
                    EdObj.UniversityName = eNSC01ViewModel.Edu_Msts.UniversityName;
                    EdObj.InstituteName = eNSC01ViewModel.Edu_Msts.InstituteName;
                    EdObj.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    EdObj.ModifiedOn = DateTime.Now;
                    PiObj.RecommendationType = eNSC01ViewModel.Pi_Msts.RecommendationType;
                    PiObj.RecommPno = eNSC01ViewModel.Pi_Msts.RecommPno;
                    PiObj.OthersRecommName = eNSC01ViewModel.Pi_Msts.OthersRecommName;



                    _context.VtmsEnrollPi.Update(PiObj);
                    _context.SaveChanges();
                    _context.VtmsEnrollEdu.Update(EdObj);
                    _context.SaveChanges();
                  


                   
                    // CommonViewModel.Message = "District Code" + eNSC01ViewModel.objDistrict.DisttCd;
                    CommonViewModel.Alert = "Update";
                    CommonViewModel.Status = "Update";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Invalid District";
                    CommonViewModel.ErrorMessage = "Invalid District";
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


        public async Task<IActionResult> DeleteConfirmed(string id)
        {
           
                try
                {
                    if (id != null)
                    {
                        int count = _context.VtmsEnrollPi.Where(x => x.VtCode.Equals(id)).ToList().Count;
                        int EdCount = _context.VtmsEnrollEdu.Where(x => x.VtCode.Equals(id)).ToList().Count;
                        var DeleteDataTemp = _context.VtmsEnrollPi.Find(id);
                        var DeleteData = _context.VtmsEnrollEdu.Find(id);
                        
                        if (DeleteDataTemp != null && DeleteData != null && count == 0 && EdCount == 0)
                        {
                            _context.VtmsEnrollPi.Remove(DeleteDataTemp);
                            _context.SaveChanges();
                            _context.VtmsEnrollEdu.Remove(DeleteData);
                            _context.SaveChanges();
                            CommonViewModel.Message = "Districtcode - " + id;
                            CommonViewModel.Alert = "Delete";
                            CommonViewModel.Status = "Delete";
                            CommonViewModel.ErrorMessage = "";
                        }
                        else
                        {
                            CommonViewModel.Message = "Cannot Perform Delete Operation. VT code already used in other Master";
                            CommonViewModel.ErrorMessage = "Cannot Perform Delete Operation.VT code already used in other Master";
                            CommonViewModel.Alert = "Warning";
                            CommonViewModel.Status = "Warning";
                        }

                    }
                    else
                    {
                        CommonViewModel.Message = "District Cd Unavailable";
                        CommonViewModel.ErrorMessage = "District Cd Unavailable";
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
            

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }
    }
}