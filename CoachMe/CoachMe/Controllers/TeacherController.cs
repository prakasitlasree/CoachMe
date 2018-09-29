using COACHME.DATASERVICE;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using COACHME.MODEL;

namespace COACHME.WEB_PRESENT.Controllers
{
    public class TeacherController : Controller
    {
        private AuthenticationServices service = new AuthenticationServices();
        // GET: Teacher
        public async Task<ActionResult> Index(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            resp = await service.GetMemberProfile(dto);
            if (resp.STATUS)
            {
                return View(resp.OUTPUT_DATA);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }           
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase profileImage, MEMBERS dto)
        {
            try
            {
                RESPONSE__MODEL resp = new RESPONSE__MODEL();
                resp = await service.UpdateProfilePic(profileImage, dto);
                ViewBag.Message = "File Uploaded Successfully!!";
                return RedirectToAction("Index", "Teacher",new { MEMBER_ID = dto.AUTO_ID});
            }
            catch(Exception ex)
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
        public async Task<ActionResult> Edit(MEMBERS dto, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                RESPONSE__MODEL result = await service.UpdateMemberProfile(dto);
                if (result.STATUS)
                {
                    return RedirectToAction("Index", "Teacher", new { MEMBER_ID = dto.AUTO_ID });
                }
                else
                {
                    ViewBag.MSG = "Update Field";
                    return RedirectToAction("Index", "Teacher", new { MEMBER_ID = dto.AUTO_ID });
                }
            }
            catch
            {
                ViewBag.MSG = "Update Field";
                return RedirectToAction("Index", "Teacher", new { MEMBER_ID = dto.AUTO_ID });
                
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

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
