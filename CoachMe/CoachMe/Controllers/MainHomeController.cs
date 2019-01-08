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
    public class MainHomeController : Controller
    {
        HomeServices service = new HomeServices();

        public ActionResult Index()
        {
            if (Session["logon"] != null)
            {
                var model = new HOME_MODEL();
                var memberLogon = (MEMBERS)Session["logon"];
                model = service.GetMemberLogin(memberLogon);
                return View(model);
            }
            else
            {
                var model = new HOME_MODEL();
                return View(model);
            }

        }

        public async Task<JsonResult> FacebookLogin(FACEBOOK_MODEL dto)
        {
            var resp = await service.FacebookLogin(dto);
            if (resp.STATUS)
            {
                MEMBERS param = resp.OUTPUT_DATA;
                Session["logon"] = param;
                resp.OUTPUT_DATA = null;
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            else//เข้าด้วยเฟสครั้งแรก
            {
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LogOff()
        {
            Session["logon"] = null;
            var resp = new RESPONSE__MODEL();
            resp.STATUS = true;
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FacebookRegister(FACEBOOK_MODEL dto)
        {
            return View(dto);
        }

        public async Task<JsonResult> FacebookRegisterConfirm(MEMBER member)
        {
            var resp = new RESPONSE__MODEL();
            resp = await service.FacebookRegisterConfirm(member);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
    }

}