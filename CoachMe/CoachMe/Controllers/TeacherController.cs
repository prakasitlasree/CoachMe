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
        public async Task<ActionResult> Index(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            resp = await service.GetMemberProfile(dto);
            model.MEMBERS = resp.OUTPUT_DATA;
            if (resp.STATUS)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
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
                return RedirectToAction("Index", "Teacher", new { member_id = dto.MEMBERS.AUTO_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
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

                return RedirectToAction("Index");
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
            try
            {
                // TODO: Add update logic here

                RESPONSE__MODEL result = await service.UpdateMemberProfile(dto.MEMBERS,about_img);

                if (result.STATUS)
                {
                    TempData["Message"] = "Profile Updated Successfully";
                    return RedirectToAction("Index", "Teacher", new { member_id = dto.MEMBERS.AUTO_ID });
                }
                else
                {
                    TempData["Message"] = "Profile Update Fail";
                    return RedirectToAction("Index", "Teacher", new { member_id = dto.MEMBERS.AUTO_ID });
                }
            }
            catch
            {
                TempData["Message"] = "Profile Update Fail";
                return RedirectToAction("Index", "Teacher", new { member_id = dto.MEMBERS.AUTO_ID });

            }
        }

        //// GET:
        //public ActionResult FindStudent(CONTAINER_MODEL dto)
        //{
        //    return View();
        //}

        // POST:

        [HttpPost]
        public ActionResult FindStudent(CONTAINER_MODEL dto)
        {
            //var result = service.FindStudent(dto.MEMBERS);
            return View();
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

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
