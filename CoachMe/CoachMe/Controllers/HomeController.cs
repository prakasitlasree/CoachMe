﻿using System;
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

        RESPONSE__MODEL resp = new RESPONSE__MODEL();
        StudentServices service = new StudentServices();
        CommonServices commonService = new CommonServices();
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

        public async Task<ActionResult> GetTeacher(CONTAINER_MODEL dto)
        {
            CONTAINER_MODEL container = new CONTAINER_MODEL();
            SEARCH_TEACHER_MODEL model = new SEARCH_TEACHER_MODEL();


            resp = await service.GetListCourseName();
            model.LIST_COURSE = resp.OUTPUT_DATA;

            resp = await service.GetListCategory();
            model.LIST_CATEGORY = resp.OUTPUT_DATA;

            resp = await commonService.GetListProvince();
            model.LIST_PROVINCE = resp.OUTPUT_DATA;

            model.LIST_SEARCH_TYPE = new List<string>() { "ครู", "คอร์ส" };
            container.SEARCH_TEACHER_MODEL = model;
             
            #region ===== SESSION NOT NUll หลัง LOGIN ====== 
            if (Session["logon"] != null)
            {
                var memberLogon = (MEMBERS)Session["logon"];
                dto.MEMBERS = memberLogon;
                container.MEMBERS = memberLogon;

                if (dto.SEARCH_TEACHER_MODEL == null)//เข้าครั้งแรก
                {
                    resp = await service.GetListAllTeacherAfterLogin(dto);
                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                    container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู";

                    return View(container);

                }
                else
                {
                    if (dto.SEARCH_TEACHER_MODEL.SEARCH_ALL == "1")//ค้นหาทั้งหมดหลัง login
                    {
                        if (dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0] == "ครู")//ค้นหาครูทั้งหมดหลังlogin
                        {
                            resp = await service.GetListAllTeacherAfterLogin(dto);
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                            container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู"; 
                            return View(container);
                        }
                        else//ค้นหาคอร์สทั้งหมดหลังlogin
                        {
                            container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "คอร์ส";
                            resp = await service.GetListAllCourseAfterLogin(dto);
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                            return View(container);
                        }
                    }
                    else//ค้นหาบางอย่างหลังล็อกอิน
                    {
                        if (dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0] == "ครู")//ค้นหาครูบางคนหลังล็อกอิน
                        {
                            resp = await service.GetListSomeTeacherAfterLogin(dto);
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                            container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู";
                            return View(container);
                        }
                        else//ค้นหาบางคอร์สทหลังล๊อกอิน
                        {
                            container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "คอร์ส";
                            resp = await service.GetListSomeCourseAfterLogin(dto);//ค้นหาบางคอร์สหลังล็อกอิน
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                            return View(container);
                        }
                    }

                }

            }
            #endregion//หลังล็อกอิน/
            #region ===== SESSION NULL ก่อน LOGIN ====
            else
            {

                if (dto.SEARCH_TEACHER_MODEL == null)
                {
                    //Event On Load
                    resp = await service.GetListAllTeacherBeforeLogin();
                    container.SEARCH_TEACHER_MODEL.TEACH_TYPE = "ครู";
                    container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                     
                    return View(container);
                }
                else
                {
                    if (dto.SEARCH_TEACHER_MODEL.SEARCH_ALL == "1")
                    {
                        container.SEARCH_TEACHER_MODEL.TEACH_TYPE = dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0];
                       
                        if (container.SEARCH_TEACHER_MODEL.TEACH_TYPE == "ครู")
                        {
                            //Search Teacher
                            resp = await service.GetListAllTeacherBeforeLogin();
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;  
                            return View(container);
                        }
                        else
                        {
                            //Search Course
                            resp = await service.GetListAllCourseBeforeLogin();
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                            return View(container);
                        }
                    }
                    else
                    {
                        container.SEARCH_TEACHER_MODEL.TEACH_TYPE = dto.SEARCH_TEACHER_MODEL.LIST_SEARCH_TYPE[0];
                        if (container.SEARCH_TEACHER_MODEL.TEACH_TYPE == "ครู")
                        {
                            resp = await service.GetListSomeTeacherBeforeLogin(dto);
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                            return View(container);
                        }
                        else
                        {

                            resp = await service.GetListSomeCourseBeforeLogin(dto);
                            container.LIST_CUSTOM_MEMBERS = resp.OUTPUT_DATA;
                            return View(container);
                        }
                    }
                }
            }
            #endregion


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