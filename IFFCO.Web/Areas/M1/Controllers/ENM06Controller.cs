using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Entities.Models;
using IFFCO.HRMS.Service;
using IFFCO.VTMS.Web.CommonFunctions;
using IFFCO.VTMS.Web.Controllers;
using IFFCO.VTMS.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using IFFCO.VTMS.Web.Models;
using System.Collections.Generic;
using ModelContext = IFFCO.VTMS.Web.Models.ModelContext;
using System;

namespace IFFCO.VTMS.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENM06Controller : BaseController<ENM06ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        CommonException<ENM06ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENM06Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENM06ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }
    
       

        public ActionResult Index(int id)
        {
            var listState = dropDownListBindWeb.ListStateBind();
            CommonViewModel.listStateBind = listState;
            CommonViewModel.StateCd = listState.FirstOrDefault().Value;
           // var data = _context.MDistrict.Where(x => x.StateCd == listState.FirstOrDefault().Value).ToList();
            var data = vTMSCommonService.GetDistrictMaster(listState.FirstOrDefault().Value);

            CommonViewModel.listDistrict = data;
            return View(CommonViewModel);
        }


        public async Task<IActionResult> ViewMsts(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var Model = PopulateSub(id);
                CommonViewModel = Model;
                CommonViewModel.listStateBind = dropDownListBindWeb.ListStateBind();
                return View("Index", CommonViewModel);
            }
        }

        public ENM06ViewModel PopulateSub(string id)
        {
            
            ENM06ViewModel view = new ENM06ViewModel();
            var LOV = dropDownListBindWeb.ListStateBind();
            if (id == null) { id = LOV.FirstOrDefault().Value; }
            view.StateCd = id;
            //view.listDistrict = _context.MDistrict.Where(x => x.StateCd.Equals(id) ).ToList();
            view.listDistrict = vTMSCommonService.GetDistrictMaster(id);
            return view;
        }

        // GET: ENM06Controller/Details
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ENM06Controller/Create
        public ActionResult Create(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var ObjIid = new MDistrict() { StateCd = id };
                CommonViewModel.listDistrict = _context.MDistrict.Where(x => x.StateCd == id).ToList();
                CommonViewModel.listDistrict = vTMSCommonService.GetDistrictMaster(id);
                CommonViewModel.objDistrict = ObjIid;
                CommonViewModel.listStateBind = dropDownListBindWeb.ListStateBind();
                CommonViewModel.StateCd = id;
            }
            
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Status = "Create";
            return View("Index", CommonViewModel);
        }
        // POST: ENM06Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ENM06ViewModel eNM06ViewModel, char id, IFormCollection collection)
        {
            try
            {
                int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                if (!string.IsNullOrWhiteSpace(eNM06ViewModel.objDistrict.DisttCd))
                {
                MDistrict ObjDis = new MDistrict();
                ObjDis.DisttCd = eNM06ViewModel.objDistrict.DisttCd;
                ObjDis.DisttName = eNM06ViewModel.objDistrict.DisttName;
                ObjDis.CreationDate = eNM06ViewModel.objDistrict.CreationDate;
                ObjDis.Status = eNM06ViewModel.objDistrict.Status;
                ObjDis.CreationDate = DateTime.Now;
                ObjDis.StateCd = eNM06ViewModel.objDistrict.StateCd;
                _context.MDistrict.Add(ObjDis);

            
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "District Code " + Convert.ToString(eNM06ViewModel.objDistrict.DisttCd);
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Invalid District Details. Try Again!";
                    CommonViewModel.ErrorMessage = "Invalid District Details. Try Again!";
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


        // GET: ENM06Controller/Edit
        public ActionResult Edit(string mod)
        {
          
            if (mod != null)

            {

                var listState = dropDownListBindWeb.ListStateBind();
                var stateCd = _context.MDistrict.FirstOrDefault(e => e.DisttCd == mod).StateCd;
                //var data = _context.MDistrict.Where(x => x.StateCd == stateCd).ToList();
                var data = vTMSCommonService.GetDistrictMaster(stateCd);
                CommonViewModel.StateCd = stateCd;
                CommonViewModel.listStateBind = listState;
                CommonViewModel.objDistrict = _context.MDistrict.FirstOrDefault(x => x.DisttCd == mod);
                CommonViewModel.listDistrict = data;
            }
            else
            {
                CommonViewModel.Message = "District Code Unavailable";
                CommonViewModel.ErrorMessage = "District Code Unavailable";
                CommonViewModel.Alert = "Warning";
                CommonViewModel.Status = "Warning";
            }

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
            CommonViewModel.Status = "Edit";
            return View("Index", CommonViewModel);
        }

        // POST: ENM06Controller/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ENM06ViewModel eNM06ViewModel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(eNM06ViewModel.objDistrict.DisttCd))
                {
                    MDistrict Obj = new MDistrict();
                    Obj = _context.MDistrict.FirstOrDefault(x => x.DisttCd.Equals(eNM06ViewModel.objDistrict.DisttCd));
                    Obj.DisttName = eNM06ViewModel.objDistrict.DisttName;
                    Obj.ModifiedBy = Convert.ToString(Convert.ToInt32(HttpContext.Session.GetInt32("EmpID")));
                    Obj.ModificationDt = DateTime.Now;
                    _context.MDistrict.Update(Obj);
                    await _context.SaveChangesAsync();

                    CommonViewModel.Message = "District Code" + eNM06ViewModel.objDistrict.DisttCd;
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


        // GET: ENM06Controller/Delete

        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id != null)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        int count = _context.VtmsInstituteMsts.Where(x => x.DistrictName.Equals(id)).ToList().Count;   
                        var DeleteDataTemp = _context.MDistrict.Find(id); 
                        if (DeleteDataTemp != null && count == 0)
                        {
                            _context.MDistrict.Remove(DeleteDataTemp);
                            _context.SaveChanges();
                            CommonViewModel.Message = "District Code - " + id;
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
                        CommonViewModel.Message = "District Code Unavailable";
                        CommonViewModel.ErrorMessage = "District Code Unavailable";
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
