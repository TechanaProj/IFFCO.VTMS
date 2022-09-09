using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using IFFCO.HRMS.Shared.CommonFunction;
using IFFCO.VTMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace IFFCO.VTMS.Web.CommonFunctions
{
    public class DropDownListBindWeb : DropDownListBind
    {
        private readonly IRepositoryProvider _repositoryProvider = new RepositoryProvider(new RepositoryFactories());

        private readonly IUnitOfWorkAsync _unitOfWork;

        //IDataContextAsync context;
        private readonly ModelContext _context;
        DataTable _dt = new DataTable();
        public DropDownListBindWeb()
        {
            _context = new ModelContext();
        }

        public List<SelectListItem> UniversityLOVBind()
        {
            var UniversityLOV = _context.VtmsUniversityMsts.OrderBy(x=>x.UniversityId).Select(x => new SelectListItem
            {
                Text = string.Concat(x.UniversityId, " - ", x.UniversityName),
                Value = x.UniversityId.ToString()

            }).ToList();

            return UniversityLOV;
        }

        public List<SelectListItem> StateLOVBind()
        {
            var StateLOV = _context.MState.OrderBy(x=>x.StateName).Select(x => new SelectListItem
            {
                Text = string.Concat(x.StateCd, " - ", x.StateName),
                Value = x.StateCd.ToString()

            }).ToList();

            return StateLOV;
        }

        public List<SelectListItem> DistrictLOVBind()
        {
            var DistrictLOV = _context.MDistrict.OrderBy(x => x.DisttCd).Select(x => new SelectListItem
            {
                Text = string.Concat(x.DisttCd, " - ", x.DisttName),
                Value = x.DisttCd.ToString()

            }).ToList();

            return DistrictLOV;
        }

        public List<SelectListItem> GET_Review()
        {
            var Status = new List<SelectListItem>();
            Status = new List<SelectListItem>();
            Status.Add(new SelectListItem { Text = "--Select--", Value = "" });
            Status.Add(new SelectListItem { Text = "Excellent", Value = "Excellent" });
            Status.Add(new SelectListItem { Text = "Good", Value = "Good" });
            Status.Add(new SelectListItem { Text = "Average", Value = "Average" });
            Status.Add(new SelectListItem { Text = "Not Applicable", Value = "Not Applicable" });

            return Status;

        }

        public List<SelectListItem> GET_VTSTATUS()  
        {
            var Status = new List<SelectListItem>();
            Status = new List<SelectListItem>();
            Status.Add(new SelectListItem { Text = "--Select--", Value = "" });
            Status.Add(new SelectListItem { Text = "A - Active", Value = "A" });
            Status.Add(new SelectListItem { Text = "I - In-Active", Value = "I" });
            return Status;
        }

 public List<SelectListItem> Getinstitue(string UNIVERSITY_ID)
        {
            string sqlquery = "SELECT  * FROM VTMS_INSTITUTE_MSTS WHERE UNIVERSITY_ID='"+ UNIVERSITY_ID + "'";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["INSTITUTE_NAME"]),
                                 Value = Convert.ToString(dr["INSTITUTE_CD"])
                             }).ToList();
            return DRP_VALUE;
        }

 public List<SelectListItem> Getbranch(string Course_Code)
        {
            string sqlquery = "SELECT  * FROM VTMS_branch_MSTS where course_code='"+ Course_Code + "'";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);

            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["BRANCH_DESC"]),
                                 Value = Convert.ToString(dr["BRANCH_ID"])
                             }).ToList();
            return DRP_VALUE;
        }

public List<SelectListItem> Getuniveristy()
        {
            string sqlquery = "SELECT  * FROM VTMS_UNIVERSITY_MSTS ORDER BY UNIVERSITY_NAME";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["UNIVERSITY_NAME"]),
                                 Value = Convert.ToString(dr["UNIVERSITY_ID"])
                             }).ToList();
            return DRP_VALUE;
        }
 public List<SelectListItem> GetCourse()
        {
            string sqlquery = "SELECT  * FROM VTMS_COURSE_MSTS ORDER BY COURSE_DESC";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["COURSE_DESC"]),
                                 Value = Convert.ToString(dr["COURSE_ID"])
                             }).ToList();
            return DRP_VALUE;
        }
public List<SelectListItem> GetState()
        {
            string sqlquery = "SELECT * FROM  M_STATE ORDER BY STATE_NAME";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["STATE_NAME"]),
                                 Value = Convert.ToString(dr["STATE_CD"])
                             }).ToList();
            return DRP_VALUE;
        }
public List<SelectListItem> GET_District()
        {
            string sqlquery = "SELECT * FROM M_DISTRICT ORDER BY DISTT_NAME";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            //List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["DISTT_NAME"]),
                                 Value = Convert.ToString(dr["DISTT_CD"])
                             }).ToList();
            return DRP_VALUE;
        }
public List<SelectListItem> GET_District(string StateCd)
        {           
            string sqlquery = "select * from M_DISTRICT where  STATE_CD='" + StateCd + "'";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            //List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["DISTT_NAME"]),
                                 Value = Convert.ToString(dr["DISTT_CD"])


                             }).ToList();
            return DRP_VALUE;
        }
 public List<SelectListItem> Getrecommendation()
        {
            string sqlquery = "SELECT * FROM  VTMS_RECOMM_MSTS ORDER BY RECOMM_NAME";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            var DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                             select new SelectListItem
                             {
                                 Text = Convert.ToString(dr["RECOMM_NAME"]),
                                 Value = Convert.ToString(dr["RECOMM_ID"])
                             }).ToList();
            return DRP_VALUE;
        }
    }
}