﻿using COACHME.DATASERVICE;
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
        public async Task<ActionResult> Index(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            var member_id = dto.MEMBER_ID;
            if (member_id == 0)
            {
                if (dto.MEMBERS == null)
                {
                    return RedirectToAction("login", "account");
                } 
            }

            resp = await service.GetMemberProfile(dto);
            
            model.MEMBERS = resp.OUTPUT_DATA;
            resp = new RESPONSE__MODEL();
            resp = await service.GetCourseByTeacherID(dto);
            model.LIST_MEMBER_TEACH_COURSE = resp.OUTPUT_DATA;
           
            if (resp.STATUS)
            {
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

        // GET: Course/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
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

        // GET: Course/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
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