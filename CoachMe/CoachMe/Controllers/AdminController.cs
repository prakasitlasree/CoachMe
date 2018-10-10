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
    public class AdminController : Controller
    {
        private AdminServices service = new AdminServices();
        private TeacherProfileServices teacherservice = new TeacherProfileServices();
        // GET: Admin
        public async Task<ActionResult> Index()
        {
            if (Session["logon"] != null)
            {
                RESPONSE__MODEL resp = new RESPONSE__MODEL();
                 
                CONTAINER_MODEL model = new CONTAINER_MODEL();
                var memberLogon = (MEMBERS)Session["logon"];
                resp = await service.GetMemberPackage();
                model.LIST_MEMBER_PACKAGE = resp.OUTPUT_DATA;
                foreach (var item in model.LIST_MEMBER_PACKAGE)
                {
                     
                }
                 
                model.MEMBERS = memberLogon;
                model.MEMBERS.MEMBER_PACKAGE = null;
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        [HttpPost]
        // POST: Course/Create
        public async Task<ActionResult> Manage(CONTAINER_MODEL dto)
        {

            try
            {
                // TODO: Add update logic here
                var memberLogon = (MEMBER_LOGON)Session["logon"];
                dto.MEMBER_LOGON = memberLogon;
                dto.MEMBERS = memberLogon.MEMBERS;
                RESPONSE__MODEL result = await service.ActivatePackage(dto);

                if (result.STATUS)
                {
                    TempData["Message"] = "Activate successfully";
                    return RedirectToAction("index", "admin", new { member_id = dto.MEMBERS.AUTO_ID });
                }
                else
                {
                    TempData["Message"] = "Activate Fail";
                    return RedirectToAction("index", "admin", new { member_id = dto.MEMBERS.AUTO_ID });
                }
            }
            catch
            {
                TempData["Message"] = "Activate Fail";
                return RedirectToAction("index", "admin", new { member_id = dto.MEMBERS.AUTO_ID });

            }
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
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

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
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

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
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
