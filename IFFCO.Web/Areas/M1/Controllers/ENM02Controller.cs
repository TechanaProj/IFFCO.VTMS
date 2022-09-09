using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Service;
using IFFCO.VTMS.Web.CommonFunctions;
using IFFCO.VTMS.Web.Controllers;
using IFFCO.VTMS.Web.Models;
using IFFCO.VTMS.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelContext = IFFCO.VTMS.Web.Models.ModelContext;
using System;

namespace IFFCO.VTMS.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENM02Controller : BaseController<ENM02ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENM02ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENM02Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENM02ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            primaryKeyGen = new PrimaryKeyGen();
        }

        // GET: ENM02Controller
        public ActionResult Index()
        {
           var course = _context.VtmsCourseMsts.ToList();
            CommonViewModel.listVtmsCourseMsts = course;
            return View(CommonViewModel);
          
        }

        // GET: ENM02Controller/Details
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ENM02Controller/Create
        public ActionResult Create()
        {
            var ObjCat = new VtmsCourseMsts() { CourseId = _context.VtmsCourseMsts.OrderByDescending(x => x.CourseId).FirstOrDefault().CourseId + 1 };
            CommonViewModel.listVtmsCourseMsts = _context.VtmsCourseMsts.ToList();
            CommonViewModel.objvtmsCourseMsts = ObjCat;
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
            CommonViewModel.Status = "Create";
            return View("Index", CommonViewModel);
        }

        // POST: ENM02Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ENM02ViewModel eNM02ViewModel)
        {
            try
            {
               
                int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                if (!string.IsNullOrWhiteSpace(eNM02ViewModel.objvtmsCourseMsts.CourseDesc))
                {
                    eNM02ViewModel.objvtmsCourseMsts.CourseId = _context.VtmsCourseMsts.OrderByDescending(x => x.CourseId).FirstOrDefault().CourseId + 1;
                    eNM02ViewModel.objvtmsCourseMsts.CreatedDateTime = DateTime.UtcNow;
                    eNM02ViewModel.objvtmsCourseMsts.CreatedBy = Convert.ToString(PersonnelNumber);
                    _context.VtmsCourseMsts.Add(eNM02ViewModel.objvtmsCourseMsts);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Course Name " + Convert.ToString(eNM02ViewModel.objvtmsCourseMsts.CourseCode);
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
            }


            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        // GET: ENM02Controller/Edit/5
        public ActionResult Edit(int id)
        {
            if (id!= null)
            {
                CommonViewModel.objvtmsCourseMsts = _context.VtmsCourseMsts.FirstOrDefault(x => x.CourseId == id); 
                CommonViewModel.listVtmsCourseMsts = _context.VtmsCourseMsts.ToList();
            }
            else
            {
                CommonViewModel.Message = "Course ID Unavailable";
                CommonViewModel.ErrorMessage = "Course ID Unavailable";
                CommonViewModel.Alert = "Warning";
                CommonViewModel.Status = "Warning";
            }

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
            CommonViewModel.Status = "Edit";
            return View("Index", CommonViewModel);
            
        }

        // POST: ENM02Controller/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ENM02ViewModel eNM02ViewModel)
        {
            try
            {
                if (eNM02ViewModel.objvtmsCourseMsts.CourseId>0)
                {
                    VtmsCourseMsts Obj = new VtmsCourseMsts();
                    Obj = _context.VtmsCourseMsts.FirstOrDefault(x => x.CourseId.Equals(eNM02ViewModel.objvtmsCourseMsts.CourseId));
                    Obj.CourseDesc = eNM02ViewModel.objvtmsCourseMsts.CourseDesc;
                    Obj.CourseCode = eNM02ViewModel.objvtmsCourseMsts.CourseCode;
                    Obj.CreatedBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
                    Obj.CreatedDateTime = DateTime.Now;
                    _context.VtmsCourseMsts.Update(Obj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Course No " + eNM02ViewModel.objvtmsCourseMsts.CourseId;
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


        // GET: ENM02Controller/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var VtmsCourseMsts = await _context.VtmsCourseMsts.FirstOrDefaultAsync(m => m.CourseId == id);
            if (VtmsCourseMsts == null)
            {
                return NotFound();
            }

            return View(VtmsCourseMsts);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            if (id != null)
            {
                try
                {
                    if (id > 0)
                    {
                        int count = _context.VtmsEnrollEdu.Where(x => x.CourseName.Equals(id)).ToList().Count;
                        var DeleteDataTemp = await _context.VtmsCourseMsts.FindAsync(id);
                        if (DeleteDataTemp != null && count == 0)
                        {
                            _context.VtmsCourseMsts.Remove(DeleteDataTemp);
                            await _context.SaveChangesAsync();
                            CommonViewModel.Message = "Course No- " + id;
                            CommonViewModel.Alert = "Delete";
                            CommonViewModel.Status = "Delete";
                            CommonViewModel.ErrorMessage = "";
                        }
                        else
                        {
                            CommonViewModel.Message = "Cannot Perform Delete Operation.";
                            CommonViewModel.ErrorMessage = "Cannot Perform Delete Operation.";
                            CommonViewModel.Alert = "Warning";
                            CommonViewModel.Status = "Warning";
                        }


                    }
                    else
                    {
                        CommonViewModel.Message = "Course Unavailable";
                        CommonViewModel.ErrorMessage = "Course Unavailable";
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

        //// POST: ENM02Controller/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
       
    }
}





