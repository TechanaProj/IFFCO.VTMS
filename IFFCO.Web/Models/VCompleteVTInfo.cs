using System;
using System.Collections.Generic;

namespace IFFCO.VTMS.Web.Models
{ 
     public class VCompleteVTInfo 
     {
          public string Vtcode { get; set; }
          public decimal? UnitCode { get; set; }
          public string Name { get; set; }
          public string FatherName { get; set; }
          public Double? ContactNo { get; set; }
          public string Address { get; set; }
          public string DistrictName { get; set; }
          public string StateName { get; set; }
          public decimal? Pincode { get; set; }
          public string EnrolledOn { get; set; }
          public string DocName { get; set; }
          public string DocRegistrationNo { get; set; }
          public string RecommendationType { get; set; }
          public string BRANCH_DESC { get; set; }
          public string COURSE_DESC { get; set; }
          public DateTime? VtStartDate { get; set; }
          public DateTime? VtEndDate { get; set; }
          public string Status { get; set; }
          public string EnrollmentStatus { get; set; }
          public string CourseName { get; set; }
          public decimal? Year { get; set; }
          public decimal? Semester { get; set; }
          public string BranchName { get; set; }
          public string InstituteName { get; set; }
          public string UniversityName { get; set; }
          public string RecommendedBy { get; set; }
          public string CertFlag { get; set; }
        public string STATUS_DESC { get; internal set; }
    }
}


