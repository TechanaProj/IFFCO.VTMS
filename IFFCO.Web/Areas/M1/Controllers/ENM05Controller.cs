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

namespace IFFCO.VTMS.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENM05Controller : BaseController<ENM05ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENM05ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENM05Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENM05ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            primaryKeyGen = new PrimaryKeyGen();
        }

        // GET: ENM05Controller
        public ActionResult Index(string id)
        {
            int UnitCode = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var data = _context.VtmsRecommMsts.Where(x=>x.UnitCode == UnitCode).ToList();
            CommonViewModel.vtmsRecommendationlist = data;
            return View(CommonViewModel);
        }

        // GET: ENM04Controller/Create
        public ActionResult Create(string id)
        {
           
            int UnitCode = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var ObjRid = new VtmsRecommMsts() {RecommId = Convert.ToString(_context.VtmsRecommMsts.OrderByDescending(x => x.RecommId).FirstOrDefault().RecommId + 1) };
           
            CommonViewModel.vtmsRecommendationlist = _context.VtmsRecommMsts.Where(x => x.UnitCode == UnitCode).ToList();
            CommonViewModel.vtmsRecommendation = ObjRid;
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Status = "Create";
            return View("Index", CommonViewModel);
        }

       
        // POST: ENM05Controller/Create/INSERTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ENM05ViewModel eNM05ViewModel, string id, IFormCollection collection)
        {
            try
            {
                int UnitCode = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
                int PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                if (!string.IsNullOrWhiteSpace(eNM05ViewModel.vtmsRecommendation.RecommName))
                {
                    string RecommIdGen = _context.VtmsRecommMsts.OrderByDescending(x => x.RecommId).FirstOrDefault().RecommId;
                    if (!string.IsNullOrWhiteSpace(RecommIdGen) && RecommIdGen.Length == 8)
                    {
                        RecommIdGen = "RECOM" + Convert.ToString(Convert.ToInt32(RecommIdGen.Substring(5)) + 1).PadLeft(3, '0');
                    }
                    eNM05ViewModel.vtmsRecommendation.RecommId = Convert.ToString(_context.VtmsRecommMsts.OrderByDescending(x => x.RecommId).FirstOrDefault().RecommId + 1);
                    eNM05ViewModel.vtmsRecommendation.RecommId = RecommIdGen;
                    eNM05ViewModel.vtmsRecommendation.UnitCode = UnitCode;

                    _context.VtmsRecommMsts.Add(eNM05ViewModel.vtmsRecommendation);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Recommendation No " + Convert.ToString(eNM05ViewModel.vtmsRecommendation.RecommId);
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Invalid RecommendationDetails. Try Again!";
                    CommonViewModel.ErrorMessage = "Invalid Recommendation Details. Try Again!";
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

        // GET: ENM04Controller/Edit/
        public ActionResult Edit(string id)
        {

            int UnitCode = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            if (id != null)
            {
                    CommonViewModel.vtmsRecommendation = _context.VtmsRecommMsts.FirstOrDefault(x => x.RecommId == id);
                    CommonViewModel.vtmsRecommendationlist = _context.VtmsRecommMsts.Where(x => x.UnitCode == UnitCode).ToList();
            }

            else
            {
                    CommonViewModel.Message = "Recommendation ID Unavailable";
                    CommonViewModel.ErrorMessage = "Recommendation ID Unavailable";
                    CommonViewModel.Alert = "Warning";
                    CommonViewModel.Status = "Warning";
            }

                    CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();  // Populating Area name for forming the page URL
                    CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString(); // Populating Menu name for forming the page URL
                    CommonViewModel.Status = "Edit";
                    return View("Index", CommonViewModel);

        }

        // POST: ENM05Controller/Edit/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ENM05ViewModel eNM05ViewModel)
        {
            try
            {
                int UnitCode = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
                if (!string.IsNullOrWhiteSpace(eNM05ViewModel.vtmsRecommendation.RecommId))
                {
                    VtmsRecommMsts Obj = new VtmsRecommMsts();
                    Obj = _context.VtmsRecommMsts.FirstOrDefault(x => x.RecommId.Equals(eNM05ViewModel.vtmsRecommendation.RecommId));
                    Obj.RecommName = eNM05ViewModel.vtmsRecommendation.RecommName;
                    Obj.UnitCode = UnitCode;
                    Obj.Status = eNM05ViewModel.vtmsRecommendation.Status;
                    _context.VtmsRecommMsts.Update(Obj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Recommendation Id " + eNM05ViewModel.vtmsRecommendation.RecommId;
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

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var VtmsRecommMsts = await _context.VtmsRecommMsts.FirstOrDefaultAsync(m => m.RecommId == id);
            if (VtmsRecommMsts == null)
            {
                return NotFound();
            }

            return View(VtmsRecommMsts);
        }

        //POST: M1/ENM05/Delete/
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id != null)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        int count = _context.VtmsEnrollPi.Where(x => x.OthersRecommName.Equals(id)).ToList().Count;
                        var DeleteDataTemp = await _context.VtmsRecommMsts.FindAsync(id);
                        if (DeleteDataTemp != null && count == 0)
                        {
                            _context.VtmsRecommMsts.Remove(DeleteDataTemp);
                            await _context.SaveChangesAsync();
                            CommonViewModel.Message = "Recommendation Id - " + id;
                            CommonViewModel.Alert = "Delete";
                            CommonViewModel.Status = "Delete";
                            CommonViewModel.ErrorMessage = "";
                        }
                        else
                        {
                            CommonViewModel.Message = "Cannot Perform Delete Operation. Recommendation already used in Records";
                            CommonViewModel.ErrorMessage = "Cannot Perform Delete Operation. Recommendation already used in Records";
                            CommonViewModel.Alert = "Warning";
                            CommonViewModel.Status = "Warning";
                        }

                    }
                    else
                    {
                            CommonViewModel.Message = "Recommendation Unavailable";
                            CommonViewModel.ErrorMessage = "Recommendation Unavailable";
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



























