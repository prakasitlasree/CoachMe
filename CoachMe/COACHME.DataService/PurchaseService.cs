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
        public async Task<RESPONSE__MODEL> PurchasePackage(MEMBERS dto,string plan ,HttpPostedFileBase slipImage)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var member = new MEMBERS();
                    member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    //Create Directory
                    string myDir = "D:\\PXProject\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\Slip\\";
                    //string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\Slip\\";

                    //Deploy
                    //myDir = @"C:\\WebApplication\\coachme.asia\\Content\\images\\Profile\\Slip\\";
                    string path = "";
                    var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    //string[] FolderProfile = memberUsername.USER_NAME.Split('@');

                    myDir += memberUsername.USER_NAME;
                    System.IO.Directory.CreateDirectory(myDir);

                    //Upload Pic
                    if (slipImage.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(slipImage.FileName);
                        path = Path.Combine(myDir, fileName);
                        slipImage.SaveAs(path);
                    }
                    //updateMemberProfileUrl

                    var memberPackage = new MEMBER_PACKAGE();
                    memberPackage.MEMBER_ID = dto.AUTO_ID;
                    memberPackage.EFFECTIVE_DATE = DateTime.Now;
                    memberPackage.STATUS = "DRAFT";
                    if(plan == "basic")
                    {
                        memberPackage.PACKAGE_NAME = "Basic Plan";
                        memberPackage.PACKAGE_DETAIL = "Basic Plan Detail";
                    }
                    if (plan == "Pro")
                    {
                        memberPackage.PACKAGE_NAME = "Professional Plan";
                        memberPackage.PACKAGE_DETAIL = "Professional Plan Detail";
                    }
                    if (plan == "Advance")
                    {
                        memberPackage.PACKAGE_NAME = "Advance Plan";
                        memberPackage.PACKAGE_DETAIL = "Advance Plan Detail";
                    }

                    memberPackage.EXPIRE_DATE = DateTime.Now.AddDays(30);
                    var uploadSlip = ctx.MEMBER_PACKAGE.Add(memberPackage);



                    //add activity
                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Update Profile";
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
                throw ex;
                resp.STATUS = false;
            }

            return resp;
        }
    }
}
