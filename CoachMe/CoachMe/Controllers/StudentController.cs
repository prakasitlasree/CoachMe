using COACHME.DATASERVICE;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace COACHME.WEB_PRESENT.Controllers
{
    public class StudentController : Controller
    {
        RESPONSE__MODEL resp = new RESPONSE__MODEL();
        StudentServices service = new StudentServices();
        // GET: Student
        public ActionResult Index()
        {
           
            if (Session["logon"] != null)
            { 
                var model = new CONTAINER_MODEL();
                var memberLogon = (MEMBERS)Session["logon"];
                model.MEMBERS = memberLogon;
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        // GET: Student/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
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

        // GET: Student/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Student/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Student/Delete/5
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

        public async Task<ActionResult> FindTeacher(SEARCH_TEACHER_MODEL dto)
        {
            if (dto.COURSE_NAME != null)
            {
                SEARCH_TEACHER_MODEL model = new SEARCH_TEACHER_MODEL();
                
                resp = await service.FindTeacher(dto);
                model.LIST_MEMBERS = resp.OUTPUT_DATA;
                return View(model);
            }
            return View(dto);
        }
        public ActionResult SearchTeacher (SEARCH_TEACHER_MODEL dto)
        {
            return View();
        }
    }
}
