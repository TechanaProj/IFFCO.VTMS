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
    public class ENSC02Controller : BaseController<ENSC02ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENSC02ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENSC02Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENSC02ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }
     
        // GET: ENSC02Controller
        public ActionResult Index()
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));

            if (Convert.ToString(TempData["Message"]) != "")
            {
                CommonViewModel.Message = Convert.ToString(TempData["Message"]);
                CommonViewModel.Alert = Convert.ToString(TempData["Alert"]);
                CommonViewModel.Status = Convert.ToString(TempData["Status"]);
                CommonViewModel.ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
            }

            CommonViewModel.vCompleteVTInfos = new List<VCompleteVTInfo>();
            CommonViewModel.vCompleteVTInfos = vTMSCommonService.VtCompleteDTl();
            //CommonViewModel.vtpinfo = new VtmsEnrollPi();

            return View(CommonViewModel);
        }

        public string Get_AcceptedVTCode_PK(int unit)
        {
            string a = string.Empty;
            try
            {
               var max = _context.VtmsEnrollPi.AsEnumerable().Where(x => x.Status == "A").Where(x => x.EnrollmentStatus == "Accepted").Where(x => x.UnitCode == unit).OrderByDescending(x => x.VtCode).FirstOrDefault();
               // var max2= _context.VtmsEnrollPi.Where(x=>x.Status=="I").Where(x=>x.EnrollmentStatus=="Rejected").Where(x=>x.UnitCode==unit).OrderByDescending(x => x.VtCode).FirstOrDefault();
                // if (max.ToString().Length > 6)
                //var vali = max.VtCode.Substring(9);
                //vali = max.VtCode.Substring(8);
                //{
                    a = DateTime.Today.AddMonths(-3).ToString("yy") + DateTime.Today.AddMonths(+9).ToString("yy") + "VT" + unit + (Convert.ToInt32(max.VtCode.Substring(7,3)) + 3).ToString().PadLeft(3, '0');
                  
               // }
                return a;
            }
            catch (NullReferenceException)
            {
               
                a = DateTime.Today.AddMonths(-3).ToString("yy") + DateTime.Today.AddMonths(+9).ToString("yy") + "VT" + unit + "001";
                string vtcod = a;
                if (!string.IsNullOrWhiteSpace(vtcod) && vtcod.Length == 10)
                {
                    vtcod = "2223VT3" + Convert.ToString(Convert.ToInt32(vtcod.Substring(7)) + 1).PadLeft(3, '0');
                }
                return vtcod;
            }
        }
        public async Task<IActionResult> AcceptConfirmed(string id)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            int user = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            var code = Get_AcceptedVTCode_PK(unit);
            // Updating PI

            string sqlquery = "UPDATE VTMS_ENROLL_PI ";
            sqlquery = sqlquery + "SET VT_CODE = '" + code + "',";
            sqlquery = sqlquery + "STATUS = 'A',";
            sqlquery = sqlquery + "ENROLLED_BY = '" + user + "',";
            sqlquery = sqlquery + "PREV_VT_CODE = '" + id + "',";
            sqlquery = sqlquery + "ENROLLED_ON = SYSDATE,";
            sqlquery = sqlquery + "ENROLLMENT_STATUS = 'Accepted' ";
            sqlquery = sqlquery + "WHERE VT_CODE = '" + id + "' AND  UNIT_CODE = '" + unit + "' ";
            int responsepi = _context.insertUpdateToDB(sqlquery);
            // Updating Edu

            sqlquery = "UPDATE VTMS_ENROLL_EDU ";
            sqlquery = sqlquery + "SET VT_CODE = '" + code + "', ";
            sqlquery = sqlquery + "PREV_VT_CODE = '" + id + "',";
            sqlquery = sqlquery + "ENROLLED_BY = '" + user + "',";
            sqlquery = sqlquery + "ENROLLED_ON = SYSDATE ";
            sqlquery = sqlquery + "WHERE VT_CODE = '" + id + "' AND  UNIT_CODE = '" + unit + "' ";
            int responseedu = _context.insertUpdateToDB(sqlquery);
            // Inserting in Doc

            sqlquery = "insert into VTMS_ENROLL_DOC(VT_CODE, UNIT_CODE) VALUES('";
            sqlquery = sqlquery + code + "', ";
            sqlquery = sqlquery + unit + ") ";
            int responsedoc = _context.insertUpdateToDB(sqlquery);
            CommonViewModel.vCompleteVTInfos = new List<VCompleteVTInfo>();
            CommonViewModel.vCompleteVTInfos = vTMSCommonService.VtCompleteDTl();
            TempData["Status"] = "General";
            TempData["Alert"] = "Accepted";
            TempData["Message"] = id + " has been accepted for VT with VT code - " + code;
            return View("Index", CommonViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> RejectConfirmed(string id, ENSC02ViewModel eNSC02ViewModel)
        {
            try 
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
                    int user = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    var vtmsenrollpi = await _context.VtmsEnrollPi.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
                    vtmsenrollpi.ModifiedOn = DateTime.Now;
                    vtmsenrollpi.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    vtmsenrollpi.Status = "I";
                    vtmsenrollpi.EnrollmentStatus = "Rejected";
                    vtmsenrollpi.PrevVtCode = id;
                    //vtmsenrollpi.VtCode = eNSC02ViewModel.vtpinfo.PrevVtCode;
                    vtmsenrollpi.VtCode = vtmsenrollpi.PrevVtCode;

                    _context.Update(vtmsenrollpi);
                    await _context.SaveChangesAsync();

                    CommonViewModel.vCompleteVTInfos = new List<VCompleteVTInfo>();
                    CommonViewModel.vCompleteVTInfos = vTMSCommonService.VtCompleteDTl();

                    CommonViewModel.Message = id + " has been rejected for Vocational Training";
                    CommonViewModel.Alert = "Rejected";
                    CommonViewModel.Status = "General";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Invalid Category Details. Try Again!";
                    CommonViewModel.ErrorMessage = "Invalid Category Details. Try Again!";
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
            return View("Index", CommonViewModel);
        }
            
            
            //TempData["Status"] = "General";
            //TempData["Alert"] = "Rejected";
            //TempData["Message"] = id + " has been rejected for Vocational Training";
        //    return RedirectToAction(nameof(Index));

        //}

        private bool VtmsEnrollPiExists(string id)
        {
            return _context.VtmsEnrollPi.Any(e => e.VtCode == id);
        }


        // GET: ENSC02Controller/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ENSC02Controller/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ENSC02Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: ENSC02Controller/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ENSC02Controller/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: ENSC02Controller/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ENSC02Controller/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
