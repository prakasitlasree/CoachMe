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
                    // myDir = @"C://Users//Prakasit//Source//Repos//CoachMe//CoachMe//CoachMe//Content//images//Profile//Slip//";
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

                    //Set hold all purchase package.
                    foreach (var item in member.MEMBER_PACKAGE)
                    {
                        if (item.STATUS == StandardEnums.PurchaseStatus.DRAFT.ToString())
                        { 
                            item.STATUS = StandardEnums.PurchaseStatus.HOLD.ToString();
                        } 
                    }
                    //var memberOldPackage = member.MEMBER_PACKAGE 
                    //if (memberOldPackage != null)
                    //{
                    //    int remainDays = Convert.ToInt32((memberOldPackage.EXPIRE_DATE.Value - memberOldPackage.EFFECTIVE_DATE.Value).TotalDays);
                    //    memberOldPackage.EFFECTIVE_DATE = DateTime.Now.AddDays(30);
                    //    memberOldPackage.EXPIRE_DATE = DateTime.Now.AddDays(30 + remainDays);
                    //    memberOldPackage.STATUS = StandardEnums.PurchaseStatus.HOLD.ToString() ;
                    //}
                    #endregion

                    #region ==== ADD NEW PACKAGE ====

                    var memberPackage = new MEMBER_PACKAGE();
                    memberPackage.MEMBER_ID = dto.AUTO_ID;
                    memberPackage.EFFECTIVE_DATE = DateTime.Now;
                    memberPackage.CREATED_DATE = DateTime.Now;
                    memberPackage.UPDATED_DATE = DateTime.Now;
                    memberPackage.CREATED_BY = member.FULLNAME;
                    memberPackage.UPDATED_BY = member.FULLNAME;
                    memberPackage.SLIP_URL1 = "//" + path.Substring(index);
                    memberPackage.STATUS = StandardEnums.PurchaseStatus.DRAFT.ToString();
                    if (plan == StandardEnums.PackageName.Basic.ToString())
                    {
                        memberPackage.PACKAGE_NAME = StandardEnums.PackageName.Basic.ToString();
                        memberPackage.PACKAGE_DETAIL = "1 Month Package";
                        memberPackage.PRICE = (decimal)StandardEnums.PackageName.Basic;
                        memberPackage.EXPIRE_DATE = DateTime.Now.AddDays(31);
                    }
                    if (plan == StandardEnums.PackageName.Professional.ToString())
                    {
                        memberPackage.PACKAGE_NAME = StandardEnums.PackageName.Professional.ToString();
                        memberPackage.PACKAGE_DETAIL = "3 Months Package";
                        memberPackage.PRICE = (decimal)StandardEnums.PackageName.Professional;
                        memberPackage.EXPIRE_DATE = DateTime.Now.AddDays(93);
                    }
                    if (plan == StandardEnums.PackageName.Advance.ToString())
                    {
                        memberPackage.PACKAGE_NAME = StandardEnums.PackageName.Advance.ToString();
                        memberPackage.PACKAGE_DETAIL = "Advance Plan Detail";
                        memberPackage.PRICE = (decimal)StandardEnums.PackageName.Advance;
                        memberPackage.EXPIRE_DATE = DateTime.Now.AddDays(365);
                    }
                     
                    ctx.MEMBER_PACKAGE.Add(memberPackage);
                    #endregion
                    
                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Purchase Package";
                    activity.FULLNAME = dto.FULLNAME; 
                    activity.STATUS = true;
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
