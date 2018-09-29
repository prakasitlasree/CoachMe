using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System.Net.Mail;
using System.Net;
using COACHME.DAL;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Globalization;

namespace COACHME.DATASERVICE
{
   public class TeacherProfileServices
    {
        public async Task<RESPONSE__MODEL> GetMemberProfile(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.MEMBER_ID).FirstOrDefaultAsync();
                    resp.OUTPUT_DATA = memberProfile;
                    resp.STATUS = true;
                }

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
            }
            return resp;

        }

        public async Task<RESPONSE__MODEL> UpdateProfilePic(HttpPostedFileBase profileImage, MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    //Create Directory
                    // string myDir = "D:\\PXProject\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\";
                    string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\";

                    //Deploy
                    myDir = @"C:\\WebApplication\\coachme.asiaContent\\images\\Profile\\";
                    string path = "";
                    var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    string[] FolderProfile = memberUsername.USER_NAME.Split('@');

                    myDir += FolderProfile[0].ToUpper();
                    System.IO.Directory.CreateDirectory(myDir);

                    //Upload Pic
                    if (profileImage.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(profileImage.FileName);
                        path = Path.Combine(myDir, fileName);
                        profileImage.SaveAs(path);
                    }
                    //updateMemberProfileUrl
                    var member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    int index = path.IndexOf("Content");
                    member.PROFILE_IMG_URL = "\\" + path.Substring(index);

                    await ctx.SaveChangesAsync();
                    resp.STATUS = true;
                }
            }
            catch (Exception ex)
            {
                resp.STATUS = false;
            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> UpdateMemberProfile(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var member = new MEMBERS();
                    member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    //Update Profile
                    if (dto.FULLNAME != null)
                    {
                        member.FULLNAME = dto.FULLNAME;
                    }
                    if (dto.FIRST_NAME != null)
                    {
                        member.FIRST_NAME = dto.FIRST_NAME;
                    }
                    if (dto.LAST_NAME != null)
                    {
                        member.LAST_NAME = dto.LAST_NAME;
                    }
                    if (dto.MOBILE != null)
                    {
                        member.MOBILE = dto.MOBILE;
                    }
                    if (dto.DATE_OF_BIRTH != null)
                    {
                        member.DATE_OF_BIRTH = Convert.ToDateTime(dto.DATE_OF_BIRTH.Value.ToShortDateString());
                    }
                    if (dto.NICKNAME != null)
                    {
                        member.NICKNAME = dto.NICKNAME;
                    }
                    if (dto.ID_CARD != null)
                    {
                        member.ID_CARD = dto.ID_CARD;
                    }
                    if (dto.SEX != null)
                    {
                        member.SEX = dto.SEX;
                    }
                    if (dto.ABOUT != null)
                    {
                        member.ABOUT = dto.ABOUT;
                    }

                    //add activity
                    await ctx.SaveChangesAsync();
                    resp.STATUS = true;

                }
            }
            catch (Exception ex)
            {
                resp.STATUS = false;
            }

            return resp;
        }
    }
}
