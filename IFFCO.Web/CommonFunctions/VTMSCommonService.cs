using Devart.Data.Oracle;
using IFFCO.VTMS.Web.Models;
using IFFCO.VTMS.Web.ViewModels;
using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFFCO.VTMS.Web;
using IFFCO.HRMS.Service;


namespace IFFCO.VTMS.Web.CommonFunctions
{
    public class VTMSCommonService
    {
        private readonly ModelContext _context;

        public VTMSCommonService()
        {
            _context = new ModelContext();
        }
        
        public string Get_AcceptedVTCode_PK(int unit)
        {
            string a = string.Empty;
            try
            {

                var max = _context.VtmsEnrollPi.AsEnumerable().Where(x => x.Status == "A").Where(x => x.EnrollmentStatus == "Enrolled").Where(x => x.UnitCode == unit).OrderByDescending(x => x.VtCode).FirstOrDefault();
                a = DateTime.Today.AddMonths(-3).ToString("yy") + DateTime.Today.AddMonths(+9).ToString("yy") + "VT" + unit + (Convert.ToInt32(max.VtCode.Substring(7, 3)) + 1).ToString().PadLeft(3, '0');

                return a;

            }
            catch (NullReferenceException)
            {
                a = DateTime.Today.AddMonths(-3).ToString("yy") + DateTime.Today.AddMonths(+9).ToString("yy") + "VT" + unit + "001";
                return a;
            }
        }
	
	 public string GetVTProfileImage(string EmpId, int unit)
        {
            try
            {
                return this.ConvertBlobToString(_context.VtmsEnrollDoc.Where(x => x.VtCode == EmpId).Where(x => x.UnitCode == unit).FirstOrDefault().VtPhoto);
            }
            catch (NullReferenceException)
            {

                return null;
            }

        }

        public byte[] GetVTIdProof(string EmpId, int unit)       
        {
            try
            {
                return _context.VtmsEnrollDoc.Where(x => x.VtCode == EmpId).Where(x => x.UnitCode == unit).FirstOrDefault().VtIdUpload;
                //return this.ConvertPdfBlobToString(_context.VtmsEnrollDoc.Where(x => x.Vtcode == EmpId).Where(x => x.UnitCode == unit).FirstOrDefault().VtIdUpload);
            }
            catch (NullReferenceException)
            {

                return null;
            }

        }


        public string ConvertBlobToString(byte[] blob)
        {
            try
            {
                string result;
                string imgString = Convert.ToBase64String(blob);
                result = Convert.ToString("data:image/.jpg;base64,") + imgString;
                return result;
            }
            catch (Exception)
            {
                return null;
            }

        }
	 public List<VtmsUniversityMsts> getuniversity()
        {
            string query = "SELECT A.UNIVERSITY_ID,A.UNIVERSITY_NAME , A.DISTRICT_NAME , B.DISTT_CD , B.DISTT_NAME FROM  VTMS_UNIVERSITY_MSTS A, M_DISTRICT B WHERE A.DISTRICT_NAME = TRIM(B.DISTT_CD(+))";
            DataTable dt = new DataTable();
            dt = _context.GetSQLQuery(query);
            List<VtmsUniversityMsts> dt_value = new List<VtmsUniversityMsts>();
            dt_value = (from DataRow dr in dt.Rows
                        select new VtmsUniversityMsts()
                        {
                            UniversityName = Convert.ToString(dr["UNIVERSITY_NAME"]),
                            DistrictName = Convert.ToString(dr["DISTT_NAME"]),
                            UniversityId = Convert.ToInt32(dr["UNIVERSITY_ID"]),
                            //DisttName = Convert.ToString(dr["DISTT_NAME"])
                        }).ToList();
            return dt_value;
        }

        public List<VCompleteVTInfo> VtCompleteDTl()
        {
            
            string sqlquery = "SELECT VT_CODE, UNIT_CODE, NAME, FATHER_NAME, CONTACT_NO, ADDRESS, DISTRICT_NAME, STATE_NAME, PINCODE, ENROLLED_ON, BRANCH_DESC , COURSE_DESC,   ";
            sqlquery += " DOC_NAME, DOC_REGISTRATION_NO, RECOMMENDATION_TYPE, VT_START_DATE, VT_END_DATE, STATUS, STATUS_DESC, YEAR, SEMESTER, BRANCH_NAME, INSTITUTE_NAME, UNIVERSITY_NAME,   ";
            sqlquery += " RECOMM_PNO, CERT_FLAG FROM V_COMPLETE_VT_INFO WHERE STATUS IN ('A','N','I')";

            DataTable dtDTL_VALUE = new DataTable();
            dtDTL_VALUE = _context.GetSQLQuery(sqlquery);
            List<VCompleteVTInfo> DTL_VALUE = new List<VCompleteVTInfo>();
            DTL_VALUE = (from DataRow dr in dtDTL_VALUE.Rows
                         select new VCompleteVTInfo()
                         {
                             
                             Vtcode = Convert.ToString(dr["VT_CODE"]),
                             UnitCode = (dr["UNIT_CODE"] == DBNull.Value) ? (Decimal?)null : Convert.ToDecimal(dr["UNIT_CODE"]),//Convert.toint64
                             Name = Convert.ToString(dr["NAME"]),
                             FatherName = Convert.ToString(dr["FATHER_NAME"]),
                             COURSE_DESC = Convert.ToString(dr["COURSE_DESC"]),
                             BRANCH_DESC = Convert.ToString(dr["BRANCH_DESC"]),
                             ContactNo = Convert.ToDouble(dr["CONTACT_NO"]),
                             Address = Convert.ToString(dr["ADDRESS"]),
                             DistrictName = Convert.ToString(dr["DISTRICT_NAME"]),
                             StateName = Convert.ToString(dr["STATE_NAME"]),
                             Pincode = (dr["PINCODE"] == DBNull.Value) ? (Decimal?)null : Convert.ToDecimal(dr["PINCODE"]),//Convert.toint64
                             EnrolledOn = Convert.ToString(dr["ENROLLED_ON"]),
                             DocName = Convert.ToString(dr["DOC_NAME"]),
                             DocRegistrationNo = Convert.ToString(dr["DOC_REGISTRATION_NO"]),
                             RecommendationType = Convert.ToString(dr["RECOMMENDATION_TYPE"]),
                             VtStartDate = (dr["VT_START_DATE"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(dr["VT_START_DATE"]),
                             VtEndDate = (dr["VT_END_DATE"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(dr["VT_END_DATE"]),
                             Status = Convert.ToString(dr["STATUS"]),
                             STATUS_DESC = Convert.ToString(dr["STATUS_DESC"]),
                             Year = (dr["YEAR"] == DBNull.Value) ? (Decimal?)null : Convert.ToDecimal(dr["YEAR"]),//Convert.toint64
                             Semester = (dr["SEMESTER"] == DBNull.Value) ? (Decimal?)null : Convert.ToDecimal(dr["SEMESTER"]),//Convert.toint64
                             BranchName = Convert.ToString(dr["BRANCH_NAME"]),
                             InstituteName = Convert.ToString(dr["INSTITUTE_NAME"]),
                             UniversityName = Convert.ToString(dr["UNIVERSITY_NAME"]),
                             RecommendedBy = Convert.ToString(dr["RECOMM_PNO"]),
                             CertFlag = Convert.ToString(dr["CERT_FLAG"])
                         }).ToList();
            return DTL_VALUE;
        }

        public List<VtmsInstituteMsts> GetInstituteMaster(string UniversityId)
        {
            string sqlquery = "SELECT A.INSTITUTE_CD, A.INSTITUTE_NAME, A.INSTITUTE_TYPE, D.DISTT_CD, D.DISTT_NAME, A.UNIVERSITY_ID, B.UNIVERSITY_NAME, C.STATE_CD, C.STATE_NAME ";
            sqlquery += " FROM VTMS_INSTITUTE_MSTS A, VTMS_UNIVERSITY_MSTS B, M_STATE C, M_DISTRICT D ";
            sqlquery += " WHERE A.UNIVERSITY_ID = B.UNIVERSITY_ID AND A.DISTRICT_NAME = TRIM(D.DISTT_CD) AND A.STATE_NAME = C.STATE_CD AND A.UNIVERSITY_ID= B.UNIVERSITY_ID  ";
            sqlquery += " AND A.UNIVERSITY_ID = '" + UniversityId + "'  ";

            DataTable dtDTL_VALUE = new DataTable();
            dtDTL_VALUE = _context.GetSQLQuery(sqlquery);
            List<VtmsInstituteMsts> DTL_VALUE = new List<VtmsInstituteMsts>();
            DTL_VALUE = (from DataRow dr in dtDTL_VALUE.Rows
                         select new VtmsInstituteMsts()
                         {
                             InstituteCd = Convert.ToInt32(dr["INSTITUTE_CD"]),
                             InstituteName =  Convert.ToString(dr["INSTITUTE_NAME"]),
                             InstituteType = Convert.ToString(dr["INSTITUTE_TYPE"]),
                             UniversityId = Convert.ToString(dr["UNIVERSITY_NAME"]),
                             StateName = Convert.ToString(dr["STATE_NAME"]),
                             DistrictName = Convert.ToString(dr["DISTT_NAME"])
                         }).ToList();

            return DTL_VALUE;
        }
		
		public List<VtmsBranchMsts> GetBranchMaster(string StateCd)
        {
            

            string sqlquery = " select B.BRANCH_ID, B.BRANCH_CODE, B.BRANCH_DESC, C.COURSE_DESC FROM VTMS_BRANCH_MSTS B JOIN VTMS_COURSE_MSTS C ON B.COURSE_CODE = C.COURSE_ID and c.course_id = '"+StateCd+"' ";

            DataTable dtDTL_VALUE = new DataTable();
            dtDTL_VALUE = _context.GetSQLQuery(sqlquery);
            List<VtmsBranchMsts> DTL_VALUE = new List<VtmsBranchMsts>();
            DTL_VALUE = (from DataRow dr in dtDTL_VALUE.Rows
                         select new VtmsBranchMsts()
                         {
                             BranchId = (dr["BRANCH_ID"] == DBNull.Value) ? (int?) null : Convert.ToInt16(dr["BRANCH_ID"]),//Convert.toint64
                             BranchCode = Convert.ToString(dr["BRANCH_CODE"]),
                             BranchDesc = Convert.ToString(dr["BRANCH_DESC"]),
                             CourseCode = Convert.ToString(dr["COURSE_DESC"])
                         }).ToList();
            return DTL_VALUE;
        }
    }
}

