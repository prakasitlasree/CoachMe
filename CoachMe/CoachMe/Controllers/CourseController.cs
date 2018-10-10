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
    public class CourseController : Controller
    {
        private TeacherProfileServices service = new TeacherProfileServices();
        // GET: Course
        public async Task<ActionResult> Index()
        {
            
            if (Session["logon"] != null)
            {
                var resp = new RESPONSE__MODEL();
                var model = new CONTAINER_MODEL(); 
                var memberLogon = (MEMBERS)Session["logon"];
                model.MEMBERS = memberLogon;
                resp = await service.GetCourseByTeacherID(memberLogon);
                model.LIST_COURSES = resp.OUTPUT_DATA;
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            } 
        }

        // GET: Course/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        // POST: Course/Create
        public async Task<ActionResult> Manage(CONTAINER_MODEL dto, HttpPostedFileBase banner_img )
        {

            try
            { 
                // TODO: Add update logic here
                var member = (MEMBERS)Session["logon"];
                //dto.MEMBER_LOGON = member.MEMBER_LOGON;
                dto.MEMBERS = member;
                RESPONSE__MODEL result = await service.MangeCourse(dto, banner_img);

                if (result.STATUS)
                {
                    TempData["Message"] = "Save course successfully";
                    return RedirectToAction("index", "course", new { member_id = dto.MEMBERS.AUTO_ID });
                }
                else
                {
                    TempData["Message"] = "Save course Fail";
                    return RedirectToAction("index", "course", new { member_id = dto.MEMBERS.AUTO_ID });
                }
            }
            catch
            {
                TempData["Message"] = "Save course Fail";
                return RedirectToAction("index", "course", new { member_id = dto.MEMBERS.AUTO_ID });

            } 
        }

        //// POST: Course/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        [HttpPost]
        // GET: Course/Edit/5
        public  ActionResult Edit(CONTAINER_MODEL dto, HttpPostedFileBase banner_img)
        {
            return  View();
        }

        // POST: Course/Edit/5
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

        // GET: Course/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Course/Delete/5
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

        private void CheckAuthen(MEMBERS dto)
        {

        }
    }
}
