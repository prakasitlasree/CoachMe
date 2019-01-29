using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using COACHME.DATASERVICE;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System.Threading.Tasks;

namespace CoachMe.Controllers
{
    public class HomeController : Controller
    {

        private RESPONSE__MODEL resp = new RESPONSE__MODEL();
        private HomeServices service = new HomeServices();
        private CommonServices commonService = new CommonServices();
        private TeacherProfileServices teacherService = new TeacherProfileServices();
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

        #region ============OldCode=============
        //public async Task<ActionResult> GetTeacher(CONTAINER_MODEL dto)
        //{
        //    CONTAINER_MODEL container = new CONTAINER_MODEL();
        //    SEARCH_TEACHER_MODEL model = new SEARCH_TEACHER_MODEL();


        //    resp = await service.GetListCourseName();
        //    model.LIST_COURSE = resp.OUTPUT_DATA;

        //    resp = await service.GetListCategory();
        //    model.LIST_CATEGORY = resp.OUTPUT_DATA;

        //    resp = await commonService.GetListProvince();
        //    model.LIST_PROVINCE = resp.OUTPUT_DATA;

        //    model.LIST_SEARCH_TYPE = new List<string>() { "ครู", "คอร์ส" };
        //    container.SEARCH_TEACHER_MODEL = model;

        //    #region ===== SESSION NOT NUll หลัง LOGIN ====== 
        //    if (Session["logon"] != null)
        //    {
        //        var memberLogon = (MEMBERS)Session["logon"];
        //        dto.MEMBERS = memberLogon;
        //        container.MEMBERS = memberLogon;

        //        if (dto.SEARCH_TEACHER_MODEL == null)//เข้าครั้งแรก
        //        {
        //            resp = await service.GetListAllTeacherAfterLogin(dto);
        //            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //            container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู";

        //            return View(container);

        //        }
        //        else
        //        {
        //            if (dto.SEARCH_TEACHER_MODEL.SEARCH_ALL == "1")//ค้นหาทั้งหมดหลัง login
        //            {
        //                if (dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0] == "ครู")//ค้นหาครูทั้งหมดหลังlogin
        //                {
        //                    resp = await service.GetListAllTeacherAfterLogin(dto);
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู";
        //                    return View(container);
        //                }
        //                else//ค้นหาคอร์สทั้งหมดหลังlogin
        //                {
        //                    container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "คอร์ส";
        //                    resp = await service.GetListAllCourseAfterLogin(dto);
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    return View(container);
        //                }
        //            }
        //            else//ค้นหาบางอย่างหลังล็อกอิน
        //            {
        //                if (dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0] == "ครู")//ค้นหาครูบางคนหลังล็อกอิน
        //                {
        //                    resp = await service.GetListSomeTeacherAfterLogin(dto);
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู";
        //                    return View(container);
        //                }
        //                else//ค้นหาบางคอร์สทหลังล๊อกอิน
        //                {
        //                    container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "คอร์ส";
        //                    resp = await service.GetListSomeCourseAfterLogin(dto);//ค้นหาบางคอร์สหลังล็อกอิน
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    return View(container);
        //                }
        //            }

        //        }

        //    }
        //    #endregion//หลังล็อกอิน/

        //    #region ===== SESSION NULL ก่อน LOGIN ====
        //    else
        //    {

        //        if (dto.SEARCH_TEACHER_MODEL == null)
        //        {
        //            //Event On Load
        //            resp = await service.GetListAllTeacherBeforeLogin();
        //            container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู";
        //            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;

        //            return View(container);
        //        }
        //        else
        //        {
        //            if (dto.SEARCH_TEACHER_MODEL.SEARCH_ALL == "1")
        //            {
        //                container.SEARCH_TEACHER_MODEL.TEACH_TYPE = dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0];

        //                if (container.SEARCH_TEACHER_MODEL.TEACH_TYPE == "ครู")
        //                {
        //                    //Search Teacher
        //                    resp = await service.GetListAllTeacherBeforeLogin();
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    return View(container);
        //                }
        //                else
        //                {
        //                    //Search Course
        //                    resp = await service.GetListAllCourseBeforeLogin();
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    return View(container);
        //                }
        //            }
        //            else
        //            {
        //                container.SEARCH_TEACHER_MODEL.TEACH_TYPE = dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0];
        //                if (container.SEARCH_TEACHER_MODEL.TEACH_TYPE == "ครู")
        //                {
        //                    resp = await service.GetListSomeTeacherBeforeLogin(dto);
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    container.SEARCH_TEACHER_MODEL.PAGE_NUMBER = dto.SEARCH_TEACHER_MODEL.PAGE_NUMBER;
        //                    return View(container);
        //                }
        //                else
        //                {

        //                    resp = await service.GetListSomeCourseBeforeLogin(dto);
        //                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
        //                    return View(container);
        //                }
        //            }
        //        }
        //    }
        //    #endregion


        //}

        //public async Task<ActionResult> MatchTeacher(SEARCH_TEACHER_MODEL dto)
        //{
        //    resp = await service.MatchTeacher(dto);
        //    if (resp.STATUS)
        //    {
        //        return RedirectToAction("getteacher", "home");
        //    }
        //    else
        //    {
        //        return RedirectToAction("errorpage", "index");
        //    }
        //}

        
        #endregion


        public async Task<JsonResult> GetListProvince()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            var memberLogon = (MEMBERS)Session["logon"];
            resp = await commonService.GetListProvinceWithID();
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

        public async Task<JsonResult> GetListAmphur(int provinceID)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            var memberLogon = (MEMBERS)Session["logon"];
            resp = await commonService.GetListAmphur(provinceID);
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

        public async Task<JsonResult> GetListCategory()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            resp = await commonService.GetListCategory();
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

        public async Task<JsonResult> GetListCourseCategory()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            resp = await commonService.GetListCourseCategory();
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


        public async Task<JsonResult> GetListTeacher(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            var memberLogon = (MEMBERS)Session["logon"];
            if (memberLogon == null)
            {
                resp = await service.GetListTeacherBeforeLogin(dto);
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
                dto.MEMBER_AUTO_ID = memberLogon.AUTO_ID;
                resp = await service.GetListTeacherAfterLogin(dto);
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

        }

        public async Task<JsonResult> GetListCourse(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            var memberLogon = (MEMBERS)Session["logon"];
            if (memberLogon == null)
            {
                resp = await service.GetListCourseBeforeLogin(dto);
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
                dto.MEMBER_AUTO_ID = memberLogon.AUTO_ID;
                resp = await service.GetListCourseAfterLogin(dto);
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

        }

        public async Task<JsonResult> SelectTeacher(HOME_MODEL dto)
        {
            resp = await service.SelectTeacher(dto);
            if (resp.STATUS)
            {
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> CancelTeacher(HOME_MODEL dto)
        {
            resp = await service.CancelTeacher(dto);
            if (resp.STATUS)
            {
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> SelectCourse(HOME_MODEL dto)
        {
            resp = await service.SelectCourse(dto);
            if (resp.STATUS)
            {
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Profile()
        {
            if (Session["logon"] != null)
            {
                RESPONSE__MODEL resp = new RESPONSE__MODEL();
                CONTAINER_MODEL model = new CONTAINER_MODEL();
                var member = (MEMBERS)Session["logon"];

                member = teacherService.GetMemberProfileFromAutoID(member).OUTPUT_DATA;
                model.MEMBERS = member;
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}