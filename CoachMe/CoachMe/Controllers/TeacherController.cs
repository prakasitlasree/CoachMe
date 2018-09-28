using COACHME.DATASERVICE;
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
            ResponseModel resp = new ResponseModel();
            resp = await service.GetMemberProfile(dto);
            if (resp.STATUS)
            {
                return View(resp.OUTPUT_DATA);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
       
      
        [HttpPost]
        public async void UploadFile(HttpPostedFileBase profileImage, MEMBERS dto)
        {
            try
            {
                ResponseModel resp = new ResponseModel();
                resp = await service.UpdateProfilePic(profileImage, dto);
                ViewBag.Message = "File Uploaded Successfully!!";
                
            }
            catch(Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
               
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
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
