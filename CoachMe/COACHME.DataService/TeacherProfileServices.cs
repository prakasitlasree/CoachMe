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
            var member_id = dto.MEMBER_ID;
            if (member_id == null || member_id == 0)
            {
                member_id = dto.MEMBERS.AUTO_ID;
            }
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = await ctx.MEMBERS.Include("MEMBER_LOGON").Where(x => x.AUTO_ID == member_id).FirstOrDefaultAsync();
                    resp.OUTPUT_DATA = memberProfile;
                    resp.STATUS = true;
                }

            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }
            return resp;

        }

        public async Task<RESPONSE__MODEL> GetMemberProfileFromAutoID(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = await ctx.MEMBERS.Include("MEMBER_LOGON").Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    resp.OUTPUT_DATA = memberProfile;
                    resp.STATUS = true;
                }

            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
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
                    string myDir = "D:\\PXProject\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\";
                    //string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\";

                    //Deploy
                    myDir = @"C:\\WebApplication\\coachme.asia\\Content\\images\\Profile\\";
                    string path = "";
                    var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    //string[] FolderProfile = memberUsername.USER_NAME.Split('@');

                    myDir += memberUsername.USER_NAME;
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
                    member.PROFILE_IMG_URL = @"\\" + path.Substring(index);

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

        public async Task<RESPONSE__MODEL> UpdateMemberProfile(MEMBERS dto,List<HttpPostedFileBase> about_img)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                   

                   
                    var member = new MEMBERS();
                    member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    #region ===== ABOUT IMAGE ====
                    for (int i = 0; i<about_img.Count;i++)
                    {
                        var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                        string myDir = "D:\\PXProject\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\About\\";
                        //string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\About\\";
                        //Deploy
                        myDir = @"C:\\WebApplication\\coachme.asia\\Content\\images\\About\\";
                        string path = "";
                        string[] FolderProfile = memberUsername.USER_NAME.Split('@');
                        myDir += FolderProfile[0].ToUpper() + " " + FolderProfile[1].ToUpper();
                        System.IO.Directory.CreateDirectory(myDir);

                        if (i == 0 && about_img[0] != null)
                        {

                            if (about_img[0].ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(about_img[0].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[0].SaveAs(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL1 = "\\" + path.Substring(index);
                        }
                        if (i == 1 && about_img[1] != null)
                        {

                            if (about_img[1].ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(about_img[1].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[1].SaveAs(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL2 = "\\" + path.Substring(index);
                        }
                        if (i == 2 && about_img[2] != null)
                        {

                            if (about_img[2].ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(about_img[2].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[2].SaveAs(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL3 = "\\" + path.Substring(index);
                        }
                        if (i == 3 && about_img[3] != null)
                        {

                            if (about_img[3].ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(about_img[3].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[3].SaveAs(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL4 = "\\" + path.Substring(index);
                        }



                    }
                    #endregion

                    #region =====Profile=====
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
                    #endregion
                    //add activity

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

        public async Task<RESPONSE__MODEL> FindStudent(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    
                    var memberProfile = await ctx.MEMBERS
                        .Include("MEMBER_LOGON")
                        .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    model.MEMBERS = memberProfile;

                    var listStudent = await ctx.MEMBERS
                                            .Include(x => x.MEMBER_ROLE)
                                            .Include(x=>x.MEMBER_LOGON)
                                            .Where(MEMBERS => MEMBERS.MEMBER_ROLE.Any(o=>o.ROLE_ID == 2))
                                            .ToListAsync();

                    model.LIST_MEMBERS = listStudent;
                    

                    resp.STATUS = true;
                    resp.OUTPUT_DATA = model;
                }
            }
            catch(Exception ex)
            {
                resp.STATUS = false;
                throw ex;
                
            }
            return resp;
        }
    }
}
