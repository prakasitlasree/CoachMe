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

        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> GetTeacher(SEARCH_TEACHER_MODEL dto)
        {
            SEARCH_TEACHER_MODEL model = new SEARCH_TEACHER_MODEL();
            resp = await service.GetListCourse();
            model.LIST_COURSE = resp.OUTPUT_DATA;

            model.LIST_PROVINCE = new List<string>() { "กรุงเทพมหานคร", "สมุทรปราการ", "นนทบุรี" };


            if (dto.LIST_COURSE != null)
            {              
                resp = await service.FindTeacher(dto);
                model.LIST_MEMBERS = resp.OUTPUT_DATA;
                return View(model);
            }
            else
            {
                resp = await service.GetListTeacher();
                model.LIST_MEMBERS = resp.OUTPUT_DATA;
                return View(model);
            }
            
        }
       
    }
}
