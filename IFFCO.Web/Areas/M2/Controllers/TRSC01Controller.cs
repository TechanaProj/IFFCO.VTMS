﻿using System;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.IO;
using NPOI.SS.Formula.Functions;

namespace IFFCO.VTMS.Web.Areas.M2.Controllers
{
    [Area("M2")]
    public class TRSC01Controller : BaseController<TRSC01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly VTMSCommonService vTMSCommonService = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TRSC01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public TRSC01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TRSC01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            vTMSCommonService = new VTMSCommonService();
            primaryKeyGen = new PrimaryKeyGen();
        }

        //[HttpPost]
        //public IActionResult Capture(string id)
        //{
        //    var files = HttpContext.Request.Form.Files;
        //    if (files != null)
        //    {
        //        foreach (var file in files)
        //        {
        //            if (file.Length > 0)
        //            {
        //                // Getting Filename  
        //                var fileName = file.FileName;
        //                // Unique filename "Guid"  
        //                //var myUniqueFileName = Convert.ToString(Guid.NewGuid());
        //                // Getting Extension  
        //                var fileExtension = Path.GetExtension(fileName);
        //                // Concating filename + fileExtension (unique filename)  
        //                var newFileName = string.Empty;
        //                if (fileExtension == ".pdf" || fileExtension == ".PDF")
        //                {
        //                    newFileName = string.Concat(id + "-IDProof", fileExtension);
        //                }
        //                else if (fileExtension == ".jpg")
        //                {
        //                    newFileName = string.Concat(id + "-Photograph", fileExtension);
        //                }

        //                //  Generating Path to store photo   
        //                var filepath = Path.Combine(_environment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";

        //                if (!string.IsNullOrEmpty(filepath))
        //                {
        //                    // Storing Image in Folder  
        //                    StoreInFolder(file, filepath);
        //                }

        //                var imageBytes = System.IO.File.ReadAllBytes(filepath);
        //                if (imageBytes != null)
        //                {
        //                    if (fileExtension == ".jpg")
        //                    {
        //                        StoreImageInDatabase(id, imageBytes);
        //                    }
        //                    else if (fileExtension == ".pdf")
        //                    {
        //                        StorePdfInDatabase(id, imageBytes);
        //                    }


        //                }
        //                System.IO.File.Delete(filepath);

        //            }
        //        }
        //        return Json(true);
        //    }
        //    else
        //    {
        //        return Json(false);
        //    }

        //}



        //private void StoreInFolder(IFormFile file, string fileName)
        //{
        //    using (FileStream fs = System.IO.File.Create(fileName))
        //    {
        //        file.CopyTo(fs);
        //        fs.Flush();
        //    }
        //}

        //private void StoreImageInDatabase(string id, byte[] imageBytes)
        //{
        //    int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
        //    try
        //    {
        //        if (imageBytes != null)
        //        {
        //            var content = _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
        //            VtmsEnrollDoc vtmsEnrollDoc = content.Result;
        //            vtmsEnrollDoc.VtPhoto = imageBytes;

        //            _context.VtmsEnrollDoc.Update(vtmsEnrollDoc);

        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private void StorePdfInDatabase(string id, byte[] imageBytes)
        //{
        //    int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
        //    try
        //    {
        //        if (imageBytes != null)
        //        {
        //            var content = _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
        //            VtmsEnrollDoc vtmsEnrollDoc = content.Result;
        //            vtmsEnrollDoc.VtIdUpload = imageBytes;

        //            _context.VtmsEnrollDoc.Update(vtmsEnrollDoc);

        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public async Task<IActionResult> Details(string id, string menu)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            if (id == null)
            {
                return NotFound();
            }
            CommonViewModel.Pi_Msts = await _context.VtmsEnrollPi.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit) ?? new VtmsEnrollPi();
            CommonViewModel.Edu_Msts = await _context.VtmsEnrollEdu.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit) ?? new VtmsEnrollEdu();
            //CommonViewModel.Doc_Msts = await _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit) ?? new VtmsEnrollDoc();
            var Doc_Msts = await _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit) ?? new VtmsEnrollDoc();
            CommonViewModel.Doc_Msts = Doc_Msts;
            CommonViewModel.ProfileImage = vTMSCommonService.ConvertBlobToString(Doc_Msts.VtPhoto);
            CommonViewModel.Status = "details";
            CommonViewModel.VTCode = id;
            CommonViewModel.UnitCode = unit;
            CommonViewModel.ActionMode = "disabled";
            byte[] idproof = vTMSCommonService.GetVTIdProof(id, unit);
            if (idproof != null && idproof.Length > 0) { CommonViewModel.IdProof = vTMSCommonService.GetVTIdProof(id, unit); } else { CommonViewModel.IdProof = new byte[] { }; };
            ViewBag.StateLOV = dropDownListBindWeb.GetState();
            ViewBag.CourseLOV = dropDownListBindWeb.GetCourse();
            ViewBag.UniversityLOV = dropDownListBindWeb.Getuniveristy();
            ViewBag.RecommendationLOV = dropDownListBindWeb.Getrecommendation();
            ViewBag.ReturnMenu = menu;

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return View("details", CommonViewModel);
        }


        // GET: M2/TRSC01
        public async Task<IActionResult> Index()
        {
            //return View(await _context.VtmsEnrollPi.ToListAsync());
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
            // CommonViewModel.View_List = vTMSCommonService.VtCompleteDTl().Where(x => x.Status == "A")/*.Where(x => x.EnrollmentStatus == "Enrolled")*/.Where(x => x.VtEndDate == null || x.VtEndDate >= DateTime.Today.AddDays(-7)).Where(x => x.UnitCode == unit).ToList();
            //CommonViewModel.Edu_Msts = new VtmsEnrollEdu();
            CommonViewModel.Pi_Msts = new VtmsEnrollPi();
            return View(CommonViewModel);
        }
        // GET: M2/TRSC01/edit
        public async Task<IActionResult> Edit(string id)
        {
            CommonService commonService = new CommonService();
            if (id == null)
            {
                return NotFound();
            }
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            CommonViewModel.Pi_Msts = new VtmsEnrollPi();
            CommonViewModel.Status = "edit";
            CommonViewModel.Pi_Msts = await _context.VtmsEnrollPi.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
            CommonViewModel.Edu_Msts = new VtmsEnrollEdu();
            CommonViewModel.Edu_Msts = await _context.VtmsEnrollEdu.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);

            var dosc = _context.VtmsEnrollDoc.Where(x => x.VtCode == id).Where(x => x.UnitCode == unit).FirstOrDefault();

            if (dosc != null)
            {
                CommonViewModel.ProfileImage = vTMSCommonService.ConvertBlobToString(dosc.VtPhoto);
                CommonViewModel.IdProof = vTMSCommonService.ConvertBlobToString(dosc.VtIdUpload);
            }

            //CommonViewModel.ProfileImage = vTMSCommonService.GetVTProfileImage(id, unit);
            //byte[] idproof = vTMSCommonService.GetVTIdProof(id, unit);
            //if (idproof != null && idproof.Length > 0)
            //{ CommonViewModel.IdProof = vTMSCommonService.GetVTIdProof(id, unit); }
            //else { CommonViewModel.IdProof = new byte[] { }; };
            CommonViewModel.Doc_Msts = new VtmsEnrollDoc();
            //var Doc_Msts = await _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == id && x.UnitCode == unit);
            //CommonViewModel.Doc_Msts = Doc_Msts;
            //CommonViewModel.ProfileImage = vTMSCommonService.ConvertBlobToString(Doc_Msts.VtPhoto);
            //CommonViewModel.ProfileImage = vTMSCommonService.GetVTIdProofStr(id, unit);
            var statelist = dropDownListBindWeb.GetState();
            ViewBag.StateLOV = statelist;
            var courselist = dropDownListBindWeb.GetCourse();
            ViewBag.CourseLOV = courselist;
            var universities = dropDownListBindWeb.Getuniveristy();
            ViewBag.UniversityLOV = universities;
            var recommendation = dropDownListBindWeb.Getrecommendation();
            ViewBag.RecommendationLOV = recommendation;

            return View("Edit", CommonViewModel);
        }


        //POST/
        [HttpPost]
        public async Task<IActionResult> UploadFile(string vtCode)
        {
            var files = HttpContext.Request.Form.Files;

            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));

            if (files != null && files.Count > 0 && files.Any(x => x.Name == "Vt_uploadPhoto"))
            {
                var obj = _context.VtmsEnrollDoc.Where(x => x.VtCode == vtCode && x.UnitCode == unit).FirstOrDefault();

                var file = files.Where(x => x.Name == "Vt_uploadPhoto").FirstOrDefault();

                if (obj != null)
                {
                    if (file != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);

                            obj.VtPhoto = ms.ToArray();
                        }

                        _context.Entry(obj).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                }
                else
                {
                    if (file != null)
                    {
                        obj = new VtmsEnrollDoc();

                        obj.VtCode = vtCode;
                        obj.UnitCode = unit;

                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);

                            obj.VtPhoto = ms.ToArray();
                        }

                        _context.VtmsEnrollDoc.Add(obj);
                        _context.SaveChanges();
                    }

                }

            }

            if (files != null && files.Count > 0 && files.Any(x => x.Name == "Id_ProofFile"))
            {
                var obj = _context.VtmsEnrollDoc.Where(x => x.VtCode == vtCode && x.UnitCode == unit).FirstOrDefault();

                var file = files.Where(x => x.Name == "Id_ProofFile").FirstOrDefault();

                if (obj != null)
                {
                    if (file != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);

                            obj.VtIdUpload = ms.ToArray();

                        }

                        _context.Entry(obj).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                }
                else
                {
                    if (file != null)
                    {
                        obj = new VtmsEnrollDoc();

                        obj.VtCode = vtCode;
                        obj.UnitCode = unit;

                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);

                            obj.VtIdUpload = ms.ToArray();
                        }

                        _context.VtmsEnrollDoc.Add(obj);
                        _context.SaveChanges();
                    }

                }

            }

            CommonViewModel.Message = "Upload Image successfully";
            CommonViewModel.Alert = "Update";
            CommonViewModel.Status = "Update";
            CommonViewModel.ErrorMessage = "";

            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);

        }


        //POST/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int unit, TRSC01ViewModel tRSC01ViewModel)
        {
            // string unit = tRSC01ViewModel.Pi_Msts.VtCode;
            unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            if (id != tRSC01ViewModel.Pi_Msts.VtCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                try
                {
                    if (tRSC01ViewModel.Pi_Msts.ManagedBy == null)
                    {
                        tRSC01ViewModel.Pi_Msts.ManagedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                        tRSC01ViewModel.Pi_Msts.ManagedOn = DateTime.Now;
                    }
                    else
                    {
                        tRSC01ViewModel.Pi_Msts.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                        tRSC01ViewModel.Pi_Msts.ModifiedOn = DateTime.Now;
                    }

                    if (tRSC01ViewModel.Doc_Msts.EnteredBy == null)
                    {

                        tRSC01ViewModel.Doc_Msts.EnteredBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
                        tRSC01ViewModel.Doc_Msts.EnteredOn = DateTime.Now;
                    }
                    else
                    {
                        tRSC01ViewModel.Doc_Msts.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                        tRSC01ViewModel.Doc_Msts.ModifiedOn = DateTime.Now;
                    }
                    // VtmsEnrollPi objpi = new VtmsEnrollPi();
                    VtmsEnrollPi objpi = _context.VtmsEnrollPi.FirstOrDefault(x => x.VtCode.Equals(tRSC01ViewModel.Pi_Msts.VtCode));

                    objpi.Status = tRSC01ViewModel.Status;
                    objpi.StateName = tRSC01ViewModel.Pi_Msts.StateName;
                    objpi.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    objpi.ModifiedOn = DateTime.Now;
                    objpi.Address = tRSC01ViewModel.Pi_Msts.Address;
                    objpi.DocName = tRSC01ViewModel.Pi_Msts.DocName;
                    objpi.FatherName = tRSC01ViewModel.Pi_Msts.FatherName;
                    objpi.ContactNo = tRSC01ViewModel.Pi_Msts.ContactNo;
                    objpi.DistrictName = tRSC01ViewModel.Pi_Msts.DistrictName;
                    objpi.DocRegistrationNo = tRSC01ViewModel.Pi_Msts.DocRegistrationNo;
                    objpi.EnrolledBy = tRSC01ViewModel.Pi_Msts.EnrolledBy;
                    objpi.EnrolledOn = tRSC01ViewModel.Pi_Msts.EnrolledOn;
                    objpi.EnrollmentStatus = tRSC01ViewModel.Pi_Msts.EnrollmentStatus;
                    objpi.RecommendationType = tRSC01ViewModel.Pi_Msts.RecommendationType;
                    objpi.RecommPno = tRSC01ViewModel.Pi_Msts.RecommPno;
                    objpi.PrevVtCode = tRSC01ViewModel.Pi_Msts.PrevVtCode;
                    objpi.Pincode = tRSC01ViewModel.Pi_Msts.Pincode;
                    objpi.VtEndDate = tRSC01ViewModel.Pi_Msts.VtEndDate;
                    objpi.VtStartDate = tRSC01ViewModel.Pi_Msts.VtStartDate;
                    objpi.VtCode = tRSC01ViewModel.Pi_Msts.VtCode;
                    //  _context.VtmsEnrollPi.Update(objpi);
                    _context.Entry(objpi).State = EntityState.Modified;
                    _context.SaveChanges();

                    VtmsEnrollEdu objedu = new VtmsEnrollEdu();
                    objedu = _context.VtmsEnrollEdu.FirstOrDefault(x => x.VtCode.Equals(tRSC01ViewModel.Pi_Msts.VtCode));


                    if (objedu != null)
                    {
                        objedu.VtCode = tRSC01ViewModel.Pi_Msts.VtCode;
                        objedu.UnitCode = unit;
                        objedu.UniversityName = tRSC01ViewModel.Edu_Msts.UniversityName.ToString();
                        objedu.InstituteName = tRSC01ViewModel.Edu_Msts.InstituteName;
                        objedu.CourseName = tRSC01ViewModel.Edu_Msts.CourseName;
                        objedu.BranchName = tRSC01ViewModel.Edu_Msts.BranchName;
                        objedu.ModifiedOn = DateTime.Now;
                        objedu.Year = tRSC01ViewModel.Edu_Msts.Year;
                        objedu.Semester = tRSC01ViewModel.Edu_Msts.Semester;
                        objedu.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                        objedu.ModifiedOn = tRSC01ViewModel.Edu_Msts.ModifiedOn;
                        //_context.VtmsEnrollEdu.Update(objedu);
                        _context.Entry(objedu).State = EntityState.Modified;
                        _context.SaveChanges();


                    }
                    else
                    {
                        objedu = new VtmsEnrollEdu();

                        objedu.VtCode = tRSC01ViewModel.Pi_Msts.VtCode;
                        objedu.UnitCode = unit;
                        objedu.UniversityName = tRSC01ViewModel.Edu_Msts.UniversityName.ToString();
                        objedu.InstituteName = tRSC01ViewModel.Edu_Msts.InstituteName;
                        objedu.CourseName = tRSC01ViewModel.Edu_Msts.CourseName;
                        objedu.BranchName = tRSC01ViewModel.Edu_Msts.BranchName;
                        objedu.Year = tRSC01ViewModel.Edu_Msts.Year;
                        objedu.Semester = tRSC01ViewModel.Edu_Msts.Semester;
                        objedu.ModifiedOn = DateTime.Now;
                        objedu.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                        objedu.ModifiedOn = tRSC01ViewModel.Edu_Msts.ModifiedOn;

                        _context.VtmsEnrollEdu.Add(objedu);
                        _context.SaveChanges();

                    }

                    //VtmsEnrollDoc objdoc = new VtmsEnrollDoc();
                    //objdoc = await _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == tRSC01ViewModel.Pi_Msts.VtCode && x.UnitCode == tRSC01ViewModel.Pi_Msts.UnitCode);
                    //// objdoc.VtCode= tRSC01ViewModel.Doc_Msts.VtCode;
                    //objdoc.VtIdType = tRSC01ViewModel.Doc_Msts.VtIdType;
                    //// objdoc.
                    //// objdoc.VtPhoto = tRSC01ViewModel.Doc_Msts.VtPhoto;
                    //objdoc.VtIdDtl = tRSC01ViewModel.Doc_Msts.VtIdDtl;
                    //objdoc.VtIdUpload = tRSC01ViewModel.Doc_Msts.VtIdUpload;
                    //objdoc.ModifiedBy = (int)Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                    //objdoc.ModifiedOn = tRSC01ViewModel.Doc_Msts.ModifiedOn;
                    //_context.VtmsEnrollDoc.Update(objdoc);


                    if (files != null && files.Count > 0 && files.Any(x => x.Name == "Vt_uploadPhoto"))
                    {
                        var obj = _context.VtmsEnrollDoc.Where(x => x.VtCode == tRSC01ViewModel.Pi_Msts.VtCode && x.UnitCode == unit).FirstOrDefault();

                        var filePhoto = files.Where(x => x.Name == "Vt_uploadPhoto").FirstOrDefault();

                        var fileId = files.Where(x => x.Name != "Vt_uploadPhoto").FirstOrDefault();

                        if (obj != null)
                        {
                            if (filePhoto != null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    filePhoto.CopyTo(ms);

                                    obj.VtPhoto = ms.ToArray();
                                }

                            }

                            if (fileId != null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileId.CopyTo(ms);

                                    obj.VtIdUpload = ms.ToArray();
                                }

                            }

                            _context.Entry(obj).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                        else
                        {
                            obj = new VtmsEnrollDoc();

                            obj.VtCode = tRSC01ViewModel.Pi_Msts.VtCode;
                            obj.UnitCode = unit;

                            if (filePhoto != null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    filePhoto.CopyTo(ms);

                                    obj.VtPhoto = ms.ToArray();
                                }

                            }

                            if (fileId != null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileId.CopyTo(ms);

                                    obj.VtIdUpload = ms.ToArray();
                                }

                            }

                            _context.VtmsEnrollDoc.Add(obj);
                            _context.SaveChanges();
                        }

                    }


                    //var vtmsdocmsts = await _context.VtmsEnrollDoc.FirstOrDefaultAsync(x => x.VtCode == tRSC01ViewModel.Pi_Msts.VtCode && x.UnitCode == tRSC01ViewModel.Pi_Msts.UnitCode); 
                    //vtmsdocmsts.VtIdType = tRSC01ViewModel.Doc_Msts.VtIdType;
                    // vtmsdocmsts.VtIdDtl = tRSC01ViewModel.Doc_Msts.VtIdDtl;
                    await _context.SaveChangesAsync();
                    //_context.SaveChanges();



                    CommonViewModel.Message = tRSC01ViewModel.Pi_Msts.Name;
                    CommonViewModel.Alert = "Update";
                    CommonViewModel.Status = "Update";
                    CommonViewModel.ErrorMessage = "";

                    CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                    CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();

                    return Json(CommonViewModel);
                }

                // catch (DbUpdateConcurrencyException)
                //{
                //    if (!vTPersonalInfoExists(tRSC01ViewModel.Pi_Msts.VtCode))
                //    {
                //        return NotFound();
                //    }
                //    else
                //    {
                //        throw;
                //    }
                //}
                //return RedirectToAction(nameof(Index));
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
            return Json(tRSC01ViewModel);
        }
        private bool vTPersonalInfoExists(string id)
        {
            return _context.VtmsEnrollPi.Any(e => e.VtCode == id);
        }


        public List<SelectListItem> DistrictLOVBind(string StateCd)
        {
            List<SelectListItem> disttCDLOV = new List<SelectListItem>();
            disttCDLOV = dropDownListBindWeb.ListDistrictBind(StateCd);
            return disttCDLOV;
        }


        //public List<SelectListItem> DistrictLOVBind(string StateCd)
        //{
        //    List<SelectListItem> disttCDLOV = new List<SelectListItem>();
        //    disttCDLOV = dropDownListBindWeb.GET_District(StateCd);
        //    return disttCDLOV;
        //}

        public List<SelectListItem> BranchLOVBind(string Course_Code)
        {
            List<SelectListItem> disttCDLOV = new List<SelectListItem>();
            disttCDLOV = dropDownListBindWeb.Getbranch(Course_Code);
            return disttCDLOV;
        }

        public List<SelectListItem> InstitueLOVBind(string UNIVERSITY_ID)
        {
            List<SelectListItem> disttCDLOV = new List<SelectListItem>();
            disttCDLOV = dropDownListBindWeb.Getinstitue(UNIVERSITY_ID);
            return disttCDLOV;
        }

    }
}
