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
        public async Task<ActionResult> GetTeacher(CONTAINER_MODEL dto)

        {
            CONTAINER_MODEL contianer = new CONTAINER_MODEL();
            SEARCH_TEACHER_MODEL model = new SEARCH_TEACHER_MODEL();

            model.LIST_PROVINCE = new List<string>() { "กรุงเทพมหานคร", "สมุทรปราการ", "นนทบุรี" };
            resp = await service.GetListCourse();
            model.LIST_COURSE = resp.OUTPUT_DATA;
            contianer.SEARCH_TEACHER_MODEL = model;

            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                dto.MEMBERS = memberLogon;
                contianer.MEMBERS = memberLogon;
                if (dto.SEARCH_TEACHER_MODEL == null)
                {
                    resp = await service.GetListTeacherFromAutoID(dto);
                    contianer.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                    return View(contianer);
                }
                else
                {
                    resp = await service.FindTeacherFromAutoID(dto);
                    contianer.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;                  
                    return View(contianer);
                }

            }
            else
            {               
                if (dto.SEARCH_TEACHER_MODEL == null)
                {
                    resp = await service.GetListTeacher();
                    contianer.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                    return View(contianer);
                }
                else
                {
                    resp = await service.FindTeacher(dto);
                    contianer.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                    return View(contianer);
                }
            }


        }

        public async Task<ActionResult> AcceptTeacher(SEARCH_TEACHER_MODEL dto)
        {
            resp = await service.AcceptTeacher(dto);
            if (resp.STATUS)
            {
                return RedirectToAction("getteacher", "student");
            }
            else
            {
                return RedirectToAction("errorpage", "index");
            }
        }


    }
}
