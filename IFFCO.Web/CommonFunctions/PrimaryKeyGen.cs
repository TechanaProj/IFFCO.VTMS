using IFFCO.VTMS.Web.Models;
using System;
using System.Linq;

namespace IFFCO.VTMS.Web.CommonFunctions
{
    public class PrimaryKeyGen
    {
        private readonly ModelContext _context;



        public PrimaryKeyGen()
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
        public string Get_EnrolledVTCode_PK(int unit)
        {
            string a = string.Empty;
            int AMax;
            int BMax;
            try
            {

                var max = _context.VtmsEnrollPi.AsEnumerable().Where(x => (x.Status == null) || (x.Status == "I") || (x.Status == "N")).Where(x => (x.EnrollmentStatus == null) || (x.EnrollmentStatus == "Rejected")).Where(x => x.UnitCode == unit).OrderByDescending(x => x.VtCode).FirstOrDefault();
                // a = unit + DateTime.Today.AddMonths(-3).ToString("yy") + (Convert.ToInt32(max.Vtcode.Substring(3, 3)) + 1).ToString().PadLeft(3, '0');
                AMax = (Convert.ToInt32(max.VtCode.Substring(3, 3)));


                //  return a;

            }
            catch (NullReferenceException)
            {
                //   a = unit + DateTime.Today.AddMonths(-3).ToString("yy") + "001";
                AMax = 1;
                // return a;
            }


            try
            {
                var max = _context.VtmsEnrollPi.AsEnumerable().Where(x => x.UnitCode == unit).OrderByDescending(x => x.PrevVtCode).FirstOrDefault().PrevVtCode;
                BMax = (Convert.ToInt32(max.Substring(3, 3)));
            }
            catch (NullReferenceException)
            {
                //   a = unit + DateTime.Today.AddMonths(-3).ToString("yy") + "001";
                BMax = 1;
                // return a;
            }

            if (BMax >= AMax)
            {
                return unit + DateTime.Today.AddMonths(-3).ToString("yy") + (Convert.ToInt32(BMax + 1).ToString().PadLeft(3, '0'));
            }
            else
            {
                return unit + DateTime.Today.AddMonths(-3).ToString("yy") + (Convert.ToInt32(AMax + 1).ToString().PadLeft(3, '0'));
            }
        }
    }
}
