using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Service;
using IFFCO.VTMS.Web.CommonFunctions;
using IFFCO.VTMS.Web.Controllers;
using IFFCO.VTMS.Web.Models;
using IFFCO.VTMS.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.VTMS.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENSCR01Controller : BaseController<ENSCR01ViewModel>
    {

        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENSCR01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ENSCR01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENSCR01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }

        public IActionResult Index()
        {
            var CourseLOV = dropDownListBindWeb.ListCourseBind();
            //var UniversityLOV = dropDownListBindWeb.ListUniversityBind();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var RecommLOV = dropDownListBindWeb.ListRecommBind(unit);
            var StateLov = dropDownListBindWeb.ListStateBind();
            ViewBag.CourseList = CourseLOV.ToList();
           // ViewBag.UniversityList = UniversityLOV.ToList();
            ViewBag.RecommList = RecommLOV.ToList();
            ViewBag.StateList = StateLov.ToList();
            ViewBag.Status = dropDownListBindWeb.GET_Status();
            return View(CommonViewModel);
        }

        [HttpPost]
        public JsonResult GetFormQuery(ENSCR01ViewModel eNSCR01ViewModel)
        {
            var str = string.Empty;
            CommonViewModel.List = new List<VCompleteVTInfo>();
            if (eNSCR01ViewModel.ReportType == "CountOfBranchTrainee")
            {
                CommonViewModel.List = SQLStringBranchWise(eNSCR01ViewModel);
            }
            else if (eNSCR01ViewModel.ReportType == "CountOfCourseTrainee")
            {
                CommonViewModel.List = SQLStringCourseWise(eNSCR01ViewModel);
            }
            else if (eNSCR01ViewModel.ReportType == "CountOfInstituteTrainee")
            {
                CommonViewModel.List = SQLStringInstituteWise(eNSCR01ViewModel);
            }
            else if (eNSCR01ViewModel.ReportType == "CountOfRecommendationTrainee")
            {
                CommonViewModel.List = SQLStringRecommendationWise(eNSCR01ViewModel);
            }
            else if (eNSCR01ViewModel.ReportType == "ListOfTrainee")
            {
                CommonViewModel.List = SQLStringListofTrainee(eNSCR01ViewModel);
            }
            return Json(CommonViewModel);
        }



        public List<VCompleteVTInfo> SQLStringBranchWise(ENSCR01ViewModel eNSCR01ViewModel)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var str = "SELECT COURSE_NAME, BRANCH_NAME, COUNT(BRANCH_NAME) COUNT";
            if (string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) || eNSCR01ViewModel.Msts.InstituteName == "null") { str += ",'' INSTITUTE_NAME"; } else { str += ",INSTITUTE_NAME"; }
            if (string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) || eNSCR01ViewModel.Msts.UniversityName == "null") { str += ",'' UNIVERSITY_NAME"; } else { str += ",UNIVERSITY_NAME"; }
            str += " FROM V_COMPLETE_VT_INFO WHERE (ENROLLED_ON BETWEEN '" + eNSCR01ViewModel.VtStartDate.ToString("dd-MMM-yyyy") + "' AND '" + eNSCR01ViewModel.VtEndDate.ToString("dd-MMM-yyyy") + "') AND BRANCH_NAME IS NOT NULL AND UNIT_CODE = " + unit + " AND ENROLLMENT_STATUS LIKE '%" + eNSCR01ViewModel.Msts.EnrollmentStatus + "%' AND COURSE_NAME LIKE '%" + eNSCR01ViewModel.Msts.CourseName + "%' ";
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) && eNSCR01ViewModel.Msts.InstituteName != "null") { str += "AND INSTITUTE_NAME LIKE '%" + eNSCR01ViewModel.Msts.InstituteName + "%'"; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) && eNSCR01ViewModel.Msts.UniversityName != "null") { str += "AND UNIVERSITY_NAME LIKE '%" + eNSCR01ViewModel.Msts.UniversityName + "%'"; }
            str += " GROUP BY COURSE_NAME, BRANCH_NAME  ";
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) && eNSCR01ViewModel.Msts.InstituteName != "null") { str += ", INSTITUTE_NAME "; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) && eNSCR01ViewModel.Msts.UniversityName != "null") { str += ", UNIVERSITY_NAME"; }
            str += " ORDER BY COURSE_NAME, BRANCH_NAME ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(str);
            //List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new VCompleteVTInfo
                             {
                                 CourseName = Convert.ToString(dr["COURSE_NAME"]),
                                 BranchName = Convert.ToString(dr["BRANCH_NAME"]),
                                 UniversityName = Convert.ToString(dr["UNIVERSITY_NAME"]),
                                 InstituteName = Convert.ToString(dr["INSTITUTE_NAME"]),
                                 DocRegistrationNo = Convert.ToString(dr["COUNT"]),
                             }).ToList();

            return DRP_VALUE;

        }

        public List<VCompleteVTInfo> SQLStringCourseWise(ENSCR01ViewModel eNSCR01ViewModel)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var str = "SELECT COURSE_NAME, COUNT(COURSE_NAME) COUNT";
            if (string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) || eNSCR01ViewModel.Msts.InstituteName == "null") { str += ",'' INSTITUTE_NAME"; } else { str += ",INSTITUTE_NAME"; }
            if (string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) || eNSCR01ViewModel.Msts.UniversityName == "null") { str += ",'' UNIVERSITY_NAME"; } else { str += ",UNIVERSITY_NAME"; }
            str += " FROM V_COMPLETE_VT_INFO WHERE  (ENROLLED_ON BETWEEN '" + eNSCR01ViewModel.VtStartDate.ToString("dd-MMM-yyyy") + "' AND '" + eNSCR01ViewModel.VtEndDate.ToString("dd-MMM-yyyy") + "') AND UNIT_CODE = " + unit + " AND ENROLLMENT_STATUS LIKE '%" + eNSCR01ViewModel.Msts.EnrollmentStatus + "%' AND COURSE_NAME LIKE '%" + eNSCR01ViewModel.Msts.CourseName + "%' ";
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) && eNSCR01ViewModel.Msts.InstituteName != "null") { str += "AND INSTITUTE_NAME LIKE '%" + eNSCR01ViewModel.Msts.InstituteName + "%'"; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) && eNSCR01ViewModel.Msts.UniversityName != "null") { str += "AND UNIVERSITY_NAME LIKE '%" + eNSCR01ViewModel.Msts.UniversityName + "%'"; }
            str += " GROUP BY COURSE_NAME  ";
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) && eNSCR01ViewModel.Msts.InstituteName != "null") { str += ", INSTITUTE_NAME "; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) && eNSCR01ViewModel.Msts.UniversityName != "null") { str += ", UNIVERSITY_NAME"; }
            str += " ORDER BY COURSE_NAME ";


            DataTable dtDRP_VALUE = _context.GetSQLQuery(str);
            //List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new VCompleteVTInfo
                             {
                                 CourseName = Convert.ToString(dr["COURSE_NAME"]),
                                 UniversityName = Convert.ToString(dr["UNIVERSITY_NAME"]),
                                 InstituteName = Convert.ToString(dr["INSTITUTE_NAME"]),
                                 DocRegistrationNo = Convert.ToString(dr["COUNT"]),
                             }).ToList();

            return DRP_VALUE;

        }

        public List<VCompleteVTInfo> SQLStringInstituteWise(ENSCR01ViewModel eNSCR01ViewModel)
        {

            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var str = "SELECT INSTITUTE_NAME, COUNT(INSTITUTE_NAME) COUNT, UNIVERSITY_NAME";
            //     if (string.IsNullOrWhiteSpace(ENSCR01ViewModel.Msts.UniversityName) || ENSCR01ViewModel.Msts.UniversityName == "null") { str += ",'' UNIVERSITY_NAME"; } else { str += ",UNIVERSITY_NAME"; }
            str += " FROM V_COMPLETE_VT_INFO WHERE  (ENROLLED_ON BETWEEN '" + eNSCR01ViewModel.VtStartDate.ToString("dd-MMM-yyyy") + "' AND '" + eNSCR01ViewModel.VtEndDate.ToString("dd-MMM-yyyy") + "') AND UNIT_CODE = " + unit + " AND STATUS_DESC LIKE '%" + eNSCR01ViewModel.Msts.EnrollmentStatus + "%'  ";
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) && eNSCR01ViewModel.Msts.InstituteName != "null") { str += "AND INSTITUTE_NAME LIKE '%" + eNSCR01ViewModel.Msts.InstituteName + "%'"; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) && eNSCR01ViewModel.Msts.UniversityName != "null") { str += "AND UNIVERSITY_NAME LIKE '%" + eNSCR01ViewModel.Msts.UniversityName + "%'"; }
            str += " GROUP BY INSTITUTE_NAME, UNIVERSITY_NAME  ";
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) && eNSCR01ViewModel.Msts.UniversityName != "null") { str += ", UNIVERSITY_NAME"; }
            str += " ORDER BY INSTITUTE_NAME ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(str);
            //List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new VCompleteVTInfo
                             {
                                 InstituteName = Convert.ToString(dr["INSTITUTE_NAME"]),
                                 UniversityName = Convert.ToString(dr["UNIVERSITY_NAME"]),
                                 DocRegistrationNo = Convert.ToString(dr["COUNT"]),
                             }).ToList();

            return DRP_VALUE;

        }

        public List<VCompleteVTInfo> SQLStringRecommendationWise(ENSCR01ViewModel eNSCR01ViewModel)
        {

            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var str = "SELECT RECOMMENDED_BY, count(RECOMMENDED_BY) COUNT, RECOMMENDATION_TYPE ";
            str += " FROM V_COMPLETE_VT_INFO WHERE  (ENROLLED_ON BETWEEN '" + eNSCR01ViewModel.VtStartDate.ToString("dd-MMM-yyyy") + "' AND '" + eNSCR01ViewModel.VtEndDate.ToString("dd-MMM-yyyy") + "') AND UNIT_CODE = " + unit + " AND RECOMMENDATION_TYPE LIKE '%" + eNSCR01ViewModel.Msts.RecommendationType + "%'   ";
            str += " GROUP BY RECOMMENDED_BY, RECOMMENDATION_TYPE ";
            str += " ORDER BY RECOMMENDED_BY ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(str);
            //List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new VCompleteVTInfo
                             {
                                 RecommendedBy = Convert.ToString(dr["RECOMMENDED_BY"]),
                                 RecommendationType = Convert.ToString(dr["RECOMMENDATION_TYPE"]),
                                 DocRegistrationNo = Convert.ToString(dr["COUNT"]),
                             }).ToList();

            return DRP_VALUE;

        }

        public List<VCompleteVTInfo> SQLStringListofTrainee(ENSCR01ViewModel eNSCR01ViewModel)
        {

            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            var str = "SELECT BRANCH_NAME, COURSE_NAME, INSTITUTE_NAME, NAME, ENROLLMENT_STATUS, CASE STATUS WHEN 'A' THEN 'Active' WHEN 'I' THEN 'Inactive' END STATUS, RECOMMENDED_BY, UNIVERSITY_NAME, VTCODE, VT_END_DATE, VT_START_DATE ";
            str += " FROM V_COMPLETE_VTINFO WHERE (ENROLLED_ON BETWEEN '" + eNSCR01ViewModel.VtStartDate.ToString("dd-MMM-yyyy") + "' AND '" + eNSCR01ViewModel.VtEndDate.ToString("dd-MMMM-yy") + "') AND UNIT_CODE = " + unit + " AND ENROLLMENT_STATUS LIKE '%" + eNSCR01ViewModel.Msts.EnrollmentStatus + "%'  ";
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.InstituteName) && eNSCR01ViewModel.Msts.InstituteName != "null") { str += "AND INSTITUTE_NAME LIKE '%" + eNSCR01ViewModel.Msts.InstituteName + "%'"; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.UniversityName) && eNSCR01ViewModel.Msts.UniversityName != "null") { str += "AND UNIVERSITY_NAME LIKE '%" + eNSCR01ViewModel.Msts.UniversityName + "%'"; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.CourseName) && eNSCR01ViewModel.Msts.CourseName != "null") { str += "AND COURSE_NAME LIKE '%" + eNSCR01ViewModel.Msts.CourseName + "%'"; }
            if (!string.IsNullOrWhiteSpace(eNSCR01ViewModel.Msts.BranchName) && eNSCR01ViewModel.Msts.BranchName != "null") { str += "AND BRANCH_NAME LIKE '%" + eNSCR01ViewModel.Msts.BranchName + "%'"; }

            str += " ORDER BY VTCODE ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(str);
            //List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new VCompleteVTInfo
                             {
                                 BranchName = Convert.ToString(dr["BRANCH_NAME"]),
                                 CourseName = Convert.ToString(dr["COURSE_NAME"]),
                                 InstituteName = Convert.ToString(dr["INSTITUTE_NAME"]),
                                 Name = Convert.ToString(dr["NAME"]),
                                 EnrollmentStatus = Convert.ToString(dr["ENROLLMENT_STATUS"]),
                                 Status = Convert.ToString(dr["STATUS"]),
                                 RecommendedBy = Convert.ToString(dr["RECOMMENDED_BY"]),
                                 UniversityName = Convert.ToString(dr["UNIVERSITY_NAME"]),
                                 Vtcode = Convert.ToString(dr["VTCODE"]),
                                 VtStartDate = Convert.ToDateTime(dr["VT_START_DATE"]),
                                 VtEndDate = Convert.ToDateTime(dr["VT_END_DATE"]),

                             }).ToList();

            return DRP_VALUE;

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateListReport(ENSCR01ViewModel eNSCR01ViewModel)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(vtmsEnrollPi);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            return View(eNSCR01ViewModel);
        }

        public JsonResult GetReportQueryDetail(ENSCR01ViewModel eNSCR01ViewModel) 
        {
            if (eNSCR01ViewModel.VtStartDate != null && eNSCR01ViewModel.VtEndDate != null)
            {
                var Vtcode = eNSCR01ViewModel.OthersRecommName;
                CommonViewModel.ReportType = eNSCR01ViewModel.ReportType;
                switch (eNSCR01ViewModel.ReportType)
                {
                    case "CountOfBranchTrainee":                       
                            CommonViewModel.ReportType = SQLStringBranchWise(eNSCR01ViewModel);                       
                        break;
                    case "CountOfCourseTrainee":                    
                            CommonViewModel.ReportType = SQLStringCourseWise(eNSCR01ViewModel);                     
                        break;
                    case "CountOfInstituteTrainee":
                            CommonViewModel.ReportType = SQLStringInstituteWise(eNSCR01ViewModel);
                        break;
                    case "CountOfRecommendationTrainee":
                            CommonViewModel.ReportType = SQLStringRecommendationWise(eNSCR01ViewModel);
                        break;
                    default:
                            CommonViewModel.ReportType = SQLStringListofTrainee(eNSCR01ViewModel);
                        break;

                }
                //string PendingParameter = (Pending == "Y") ? string.Empty : "NOT";
                //var ObjList = despatchCommonService.GetDAListForMDABifurcation(PlantCd, FromDate.ToString("dd/MMM/yy"), ToDate.ToString("dd/MMM/yy"), PendingParameter);
                //CommonViewModel.ListSummary = despatchCommonService.GetDASummaryForMDABifurcation(PlantCd, FromDate.ToString("dd/MMM/yy"), ToDate.ToString("dd/MMM/yy"));
                //if (ObjList.Any() && Pending.Equals("N"))
                //{
                //    string MDA = ObjList.OrderByDescending(c => c.MdaNo).FirstOrDefault().MdaNo;
                //    CommonViewModel.DdaCrr = _context.DDa.FirstOrDefault(x => x.MdaNo.Equals(MDA));
                //}
                //CommonViewModel.ObjList = ObjList;
                //CommonViewModel.Pending = Pending;
            }
            return Json(CommonViewModel);
        }

    }
}
