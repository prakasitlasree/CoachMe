using COACHME.DATASERVICE;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace COACHME.WEB_PRESENT.Controllers
{
    public class TeacherController : Controller
    {
        private TeacherProfileServices service = new TeacherProfileServices();
        // GET: Teacher
        public ActionResult Index()
        {
            if (Session["logon"] != null)
            {
                RESPONSE__MODEL resp = new RESPONSE__MODEL();
                CONTAINER_MODEL model = new CONTAINER_MODEL();
                var memeber = (MEMBERS)Session["logon"];
                model.MEMBERS = memeber;
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase profileImage, CONTAINER_MODEL dto)
        {
            try
            {
                RESPONSE__MODEL resp = new RESPONSE__MODEL();
                resp = await service.UpdateProfilePic(profileImage, dto.MEMBERS);
                ViewBag.Message = "File Uploaded Successfully!!";
                MEMBERS param = resp.OUTPUT_DATA;

                Session["logon"] = null;
                Session["logon"] = param;
                return RedirectToAction("index", "teacher", new { member_id = dto.MEMBERS.AUTO_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!" + "<br>" + ex.Message;
                return Redirect(Request.Url.AbsoluteUri);
            }
        }

        // GET: Teacher/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Teacher/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Teacher/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(CONTAINER_MODEL dto, List<HttpPostedFileBase> about_img)
        {
            MEMBERS param = new MEMBERS();
            try
            {
                // TODO: Add update logic here

                RESPONSE__MODEL result = await service.UpdateMemberProfile(dto.MEMBERS, about_img);
                param = result.OUTPUT_DATA;
                if (result.STATUS)
                {
                    TempData["Message"] = "Profile Updated Successfully";
                    Session["logon"] = null;
                    Session["logon"] = param;
                    return RedirectToAction("index", "teacher", new { member_id = dto.MEMBERS.AUTO_ID });
                }
                else
                {
                    TempData["Message"] = "Profile Update Fail";
                    Session["logon"] = null;
                    Session["logon"] = param;
                    return RedirectToAction("index", "teacher", new { member_id = dto.MEMBERS.AUTO_ID });
                }
            }
            catch
            {
                TempData["Message"] = "Profile Update Fail";
                Session["logon"] = null;
                Session["logon"] = param;
                return RedirectToAction("index", "teacher", new { member_id = dto.MEMBERS.AUTO_ID });

            }
        }

        public async Task<ActionResult> EditCategory(string[] categoryList)
        {
            if (Session["logon"] != null)
            {
                CONTAINER_MODEL container = new CONTAINER_MODEL();
                var memberLogon = (MEMBERS)Session["logon"];

                try
                {
                    RESPONSE__MODEL result = await service.UpdateMemberCategoryProfile(memberLogon, categoryList);

                    if (result.STATUS)
                    {
                        TempData["MessageCate"] = "Profile Updated Successfully";
                        return RedirectToAction("index", "teacher", new { member_id = memberLogon.AUTO_ID });
                    }
                    else
                    {
                        TempData["MessageCate"] = "Profile Update Fail";
                        return RedirectToAction("index", "teacher", new { member_id = memberLogon.AUTO_ID });
                    }
                }
                catch
                {
                    TempData["Message"] = "Profile Update Fail";
                    return RedirectToAction("index", "teacher", new { member_id = memberLogon.AUTO_ID });

                }
            }
            else
            {

                return RedirectToAction("errorpage", "home");
            }

        }

        //// GET:
        //public ActionResult FindStudent(CONTAINER_MODEL dto)
        //{
        //    return View();
        //}

        public ActionResult Dashboard()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                //resp = service.GetMemberProfileNotAsync(memberLogon);
                model.MEMBERS = memberLogon;
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Survey()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.ManageSurvey(memberLogon);
                //model.MEMBERS = resp.OUTPUT_DATA;
                TempData["Message"] = "Success";
                return RedirectToAction("dashboard", "teacher");
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        // POST:

        [HttpPost]
        public async Task<ActionResult> FindStudent(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.FindStudent(dto.MEMBERS);
                model = resp.OUTPUT_DATA;
                if (service.package != null)
                {
                    TempData["NOADS"] = true;
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        public async Task<ActionResult> AcceptStudent(CONTAINER_MODEL dto, string AcceptStudent)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();

            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.AcceptStudent(dto, AcceptStudent);
                if (resp.STATUS)
                {
                    return RedirectToAction("index", "teacher", new { member_id = dto.MEMBERS.AUTO_ID });
                }
                return RedirectToAction("login", "account");
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        // GET: Teacher/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Teacher/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("index");
            }
            catch
            {
                return View();
            }
        }



        public async Task<JsonResult> GetListTeacherCategory()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.GetListTeacherCategory(memberLogon);
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<JsonResult> GetListAvailableCategory()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.GetListAvailableCategory(memberLogon);
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<JsonResult> UpdateMemberCategory(string[] categoryList)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.UpdateMemberCategoryProfile(memberLogon, categoryList);
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> GetTeacherProfile()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.GetTeacherProfile(memberLogon);
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<JsonResult> UpdateMemberProfile(CUSTOM_MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                dto.AUTO_ID = memberLogon.AUTO_ID;
                resp = await service.UpdateMemberProfile(dto);
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> GetGeography()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.GetGeography();
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> GetListProvince(int geoID)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.GetListProvince(geoID);
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> GetListAmphur(int provinceID)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.GetListAmphur(provinceID);
                if (resp.STATUS)
                {
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    resp.STATUS = false;
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resp.STATUS = false;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> UpdateMemberProfileAboutImg(List<HttpPostedFileBase> about_img)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
               
                resp = await service.UpdateMemberProfileAboutImg(memberLogon, about_img);
                if (resp.STATUS)
                {
                    return RedirectToAction("index", "teacher", new { member_id = memberLogon.AUTO_ID });
                }
                else
                {
                    resp.STATUS = false;
                    return RedirectToAction("errorpage", "home");
                }
            }
            else
            {
                resp.STATUS = false;
                return RedirectToAction("errorpage", "home");
            }
        }



    }
}
