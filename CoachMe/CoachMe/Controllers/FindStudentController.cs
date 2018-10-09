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
    public class FindStudentController : Controller
    {
        private TeacherProfileServices service = new TeacherProfileServices();
        // GET: FindStudent
        public ActionResult Index(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
          
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBER_LOGON)Session["logon"];
                resp =  service.GetMemberProfileNotAsync(memberLogon);
                model.MEMBERS = resp.OUTPUT_DATA;
                resp =  service.FindStudent_new(model.MEMBERS);
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

        [HttpPost]
        public async Task<ActionResult> AcceptStudent(CONTAINER_MODEL dto, string AcceptStudent)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();

            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBER_LOGON)Session["logon"];
                resp = await service.AcceptStudent(dto, AcceptStudent);
                if (resp.STATUS)
                {
                    return RedirectToAction("index", "findstudent", new { MEMBERS = dto.MEMBERS});
                }
                return RedirectToAction("login", "account");
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        // GET: FindStudent/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FindStudent/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FindStudent/Create
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

        // GET: FindStudent/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FindStudent/Edit/5
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

        // GET: FindStudent/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FindStudent/Delete/5
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
