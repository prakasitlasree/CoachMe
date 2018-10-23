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
    public class PurchaseController : Controller
    {
        private TeacherProfileServices service = new TeacherProfileServices();
        private PurchaseService purchase_service = new PurchaseService();
        // GET: Purchase
        public  ActionResult  Index(MEMBER_LOGON dto)
        {
            
            if (Session["logon"] != null)
            { 
                var model = new CONTAINER_MODEL();
                var member = (MEMBERS)Session["logon"];
                var mem = service.GetMemberProfileNotAsync(member.MEMBER_LOGON.FirstOrDefault());
                var package = (MEMBERS)mem.OUTPUT_DATA;
                member.MEMBER_PACKAGE = package.MEMBER_PACKAGE;
                model.MEMBERS = member;// course.OUTPUT_DATA;

                if (model.MEMBERS.MEMBER_PACKAGE.Count > 0)
                {
                    foreach (var item in model.MEMBERS.MEMBER_PACKAGE)
                    {
                        if (item.PACKAGE_NAME == StandardEnums.PackageName.Basic.ToString())
                        {
                            TempData["Plan1"] = item.PACKAGE_NAME;
                            TempData["Plan1Status"] = item.STATUS;
                            if (item.EXPIRE_DATE.Value.Date < DateTime.Now.Date)
                            {
                                TempData["Plan1Status"] = StandardEnums.PurchaseStatus.EXPIRED;
                            }
                        }
                        else if (item.PACKAGE_NAME == StandardEnums.PackageName.Professional.ToString())
                        {
                            TempData["Plan2"] = item.PACKAGE_NAME;
                            TempData["Plan2Status"] = item.STATUS;
                            if (item.EXPIRE_DATE.Value.Date < DateTime.Now.Date)
                            {
                                TempData["Plan2Status"] = StandardEnums.PurchaseStatus.EXPIRED;
                            }
                        }
                        else if (item.PACKAGE_NAME == StandardEnums.PackageName.Advance.ToString())
                        {
                            TempData["Plan3"] = item.PACKAGE_NAME;
                            TempData["Plan3Status"] = item.STATUS;
                            if (item.EXPIRE_DATE.Value.Date < DateTime.Now.Date )
                            {
                                TempData["Plan3Status"] = StandardEnums.PurchaseStatus.EXPIRED;
                            }
                        }
                    }

                    //var hasPackage = model.MEMBERS.MEMBER_PACKAGE.OrderByDescending(o=>o.AUTO_ID).FirstOrDefault();
                    //if (hasPackage != null)
                    //{
                    //    TempData["Plan"] =  hasPackage.PACKAGE_NAME; 
                    //}
                    //else
                    //{
                    //    TempData["Plan"] = "No Plan";
                    //}
                }
                else
                {
                    TempData["Plan"] = "No Plan";
                }


                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PurchasePackage(CONTAINER_MODEL dto,string btnPlan,HttpPostedFileBase slipImage)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            resp = await purchase_service.PurchasePackage(dto.MEMBERS,btnPlan,slipImage);
            model.MEMBERS = resp.OUTPUT_DATA;
            if (resp.STATUS)
            {
                TempData["MessagePurchase"] = "CoachME กำลังตรวจสอบหลักฐานการโอนเงิน กรุณารอซักครุ่";
                return RedirectToAction("index", "purchase", new { member_id = dto.MEMBERS.AUTO_ID });
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        // GET: Purchase/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Purchase/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Purchase/Create
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

        // GET: Purchase/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Purchase/Edit/5
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

        // GET: Purchase/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Purchase/Delete/5
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
    }
}
