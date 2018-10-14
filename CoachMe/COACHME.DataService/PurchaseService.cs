using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System.Collections.Generic;
using System.Web;
using System.IO;
using COACHME.DAL;

namespace COACHME.DATASERVICE
{
    public class PurchaseService
    {
        public async Task<RESPONSE__MODEL> PurchasePackage(MEMBERS dto, string plan, HttpPostedFileBase slipImage)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    //Create Directory
                    //Deploy
                    #region ==== DEPLOY PATH ====
                    string myDir = @"C://WebApplication//coachme.asia//Content//images//Profile//Slip//";
                    #endregion
                   
                    #region ==== ROCK PATH ====
                    //string myDir = "D:\\PXProject\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\Slip\\";
                    #endregion
                    
                    #region ==== P'X PATH ====
                     myDir = @"C://Users//Prakasit//Source//Repos//CoachMe//CoachMe//CoachMe//Content//images//Profile//Slip//";
                    #endregion

                    string path = "";

                    var member = new MEMBERS();
                    member = await ctx.MEMBERS
                                        .Include("MEMBER_LOGON")
                                        .Include("MEMBER_PACKAGE")
                                        .Include("MEMBER_ROLE")
                                        .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    #region ==== UPLOAD SLIP ====
                    var memberUsername = member.MEMBER_LOGON.Select(x => x.USER_NAME).FirstOrDefault();
                    myDir += memberUsername;
                    int index = 0;
                    System.IO.Directory.CreateDirectory(myDir);
                    //Upload Pic
                    if (slipImage.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(slipImage.FileName);
                        path = Path.Combine(myDir, fileName);
                        slipImage.SaveAs(path);
                        index = path.IndexOf("Content");
                       
                    }
                    //updateMemberProfileUrl
                    #endregion
                   
                    #region ==== UPDATE OLD PACKAGE====
                    var memberOldPackage = member.MEMBER_PACKAGE
                                                  .Where(o => o.EXPIRE_DATE > DateTime.Now && o.STATUS == "ACTIVE")
                                                  .OrderByDescending(p => p.AUTO_ID)
                                                  .FirstOrDefault();

                    if (memberOldPackage != null)
                    {
                        int remainDays = Convert.ToInt32((memberOldPackage.EXPIRE_DATE.Value - memberOldPackage.EFFECTIVE_DATE.Value).TotalDays);
                        memberOldPackage.EFFECTIVE_DATE = DateTime.Now.AddDays(30);
                        memberOldPackage.EXPIRE_DATE = DateTime.Now.AddDays(30 + remainDays);
                        memberOldPackage.STATUS = "HOLD";

                    }
                    #endregion

                    #region ==== ADD NEW PACKAGE ====

                    var memberPackage = new MEMBER_PACKAGE();
                    memberPackage.MEMBER_ID = dto.AUTO_ID;
                    memberPackage.EFFECTIVE_DATE = DateTime.Now;
                    memberPackage.SLIP_URL1 = "//" + path.Substring(index);
                    memberPackage.STATUS = "DRAFT";
                    if (plan == "basic")
                    {
                        memberPackage.PACKAGE_NAME = "Basic Plan";
                        memberPackage.PACKAGE_DETAIL = "Basic Plan Detail";
                        memberPackage.PRICE = 200;
                    }
                    if (plan == "Pro")
                    {
                        memberPackage.PACKAGE_NAME = "Professional Plan";
                        memberPackage.PACKAGE_DETAIL = "Professional Plan Detail";
                        memberPackage.PRICE = 400;
                    }
                    if (plan == "Advance")
                    {
                        memberPackage.PACKAGE_NAME = "Advance Plan";
                        memberPackage.PACKAGE_DETAIL = "Advance Plan Detail";
                        memberPackage.PRICE = 500;
                    }

                    memberPackage.EXPIRE_DATE = DateTime.Now.AddDays(120);

                    var uploadSlip = ctx.MEMBER_PACKAGE.Add(memberPackage);
                    #endregion
                    
                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Purchase Package";
                    activity.FULLNAME = dto.FULLNAME;
                    activity.USER_NAME = dto.FULLNAME;
                    activity.PASSWORD = dto.FULLNAME;
                    activity.STATUS = resp.STATUS;
                    ctx.LOGON_ACTIVITY.Add(activity);
                    #endregion

                    await ctx.SaveChangesAsync();
                    resp.STATUS = true;

                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }
    }
}
