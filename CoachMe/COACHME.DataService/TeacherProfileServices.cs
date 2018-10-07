﻿using System;
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
    public class TeacherProfileServices
    {
        public async Task<RESPONSE__MODEL> GetMemberProfile(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            var member_id = dto.MEMBER_ID;
            if (member_id == 0)
            {
                member_id = dto.MEMBERS.AUTO_ID;
            }
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = await ctx.MEMBERS
                                             .Include("MEMBER_LOGON")
                                             .Include("MEMBER_PACKAGE")
                                             .Where(x => x.AUTO_ID == member_id).FirstOrDefaultAsync();
                    memberProfile.MEMBER_PACKAGE = memberProfile.MEMBER_PACKAGE.Skip(Math.Max(0, memberProfile.MEMBER_PACKAGE.Count() - 1)).ToList();
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

        public RESPONSE__MODEL GetMemberProfileNotAsync(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            var member_id = dto.MEMBER_ID;
            if (member_id == 0)
            {
                member_id = dto.MEMBERS.AUTO_ID;
            }
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = ctx.MEMBERS
                                             .Include("MEMBER_ROLE")
                                             .Include("MEMBER_LOGON")
                                             .Include("MEMBER_PACKAGE")
                                             .Where(x => x.AUTO_ID == member_id).FirstOrDefault();

                    memberProfile.MEMBER_PACKAGE = memberProfile.MEMBER_PACKAGE
                                                   .Where(x => x.STATUS != "DRAFT")
                                                   .OrderBy(x => x.EFFECTIVE_DATE)
                                                   .ToList();

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
                    //Deploy
                    #region ==== DEPLOY PATH ====
                    //myDir = @"C:\\WebApplication\\coachme.asia\\Content\\images\\Profile\\Slip\\";
                    #endregion
                    #region ==== ROCK PATH ====
                    string myDir = "D://PXProject//CoachMe//CoachMe//CoachMe//Content//images//Profile//";
                    #endregion
                    #region ==== P'X PATH ====
                    //string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\Slip\\";
                    #endregion

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
                    member.PROFILE_IMG_URL = @"//" + path.Substring(index);

                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Update Profile Pic";
                    activity.FULLNAME = dto.FULLNAME;
                    activity.USER_NAME = dto.FULLNAME;
                    activity.PASSWORD = dto.FULLNAME;
                    activity.STATUS = resp.STATUS;
                    ctx.LOGON_ACTIVITY.Add(activity);
                    #endregion

                    await ctx.SaveChangesAsync();
                    var memberLogon = await ctx.MEMBER_LOGON.Include("MEMBERS").Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    resp.OUTPUT_DATA = memberLogon;
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

        public async Task<RESPONSE__MODEL> UpdateMemberProfile(MEMBERS dto, List<HttpPostedFileBase> about_img)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {



                    var member = new MEMBERS();
                    member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    #region ===== ABOUT IMAGE ====
                    for (int i = 0; i < about_img.Count; i++)
                    {
                        var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                        string myDir = "D://PXProject//CoachMe//CoachMe//CoachMe//Content//images//About//";
                        //string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\About\\";
                        //Deploy
                        //myDir = @"C:\\WebApplication\\coachme.asia\\Content\\images\\About\\";
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
                            member.ABOUT_IMG_URL1 = "//" + path.Substring(index);
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
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> UpdateMemberCategoryProfile(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var member = new MEMBERS();
                    member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    #region =====Profile=====
                    //Update Profile
                    if (dto.CATEGORY != null)
                    {
                        member.CATEGORY = dto.CATEGORY;
                    }
                    if (dto.LOCATION != null)
                    {
                        member.LOCATION = dto.LOCATION;
                    }

                    #endregion
                    //add activity
                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Update Profile Pic";
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
                resp.STATUS = false;
                resp.ErrorMessage = ex.Message;
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
                        .Include("MEMBER_PACKAGE")
                        .Include(a => a.MEMBER_ROLE.Select(c => c.MEMBER_TEACH_COURSE))
                        .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    model.MEMBERS = memberProfile;

                    var teachCourse = memberProfile.MEMBER_ROLE.FirstOrDefault()
                                                        .MEMBER_TEACH_COURSE.Select(o => o.COURSE_ID).ToArray();

                    var regisCourse = ctx.MEMBER_REGIS_COURSE.Where(o => teachCourse.Contains(o.COURSE_ID)).Select(o => o.MEMBER_ROLE_ID).ToList();


                    var listStudent = await ctx.MEMBERS
                                            .Include(x => x.MEMBER_ROLE)
                                            .Include(x => x.MEMBER_LOGON)
                                            .Where(MEMBERS => MEMBERS.MEMBER_ROLE.Any(o => regisCourse.Contains(o.MEMBER_ID)))
                                            .ToListAsync();


                    var package = memberProfile.MEMBER_PACKAGE
                                                .Where(x => x.STATUS != "DRAFT" && x.EXPIRE_DATE > DateTime.Now)
                                                .Select(o => o.PACKAGE_NAME).LastOrDefault();
                    if (package == null)
                    {
                        listStudent = listStudent.Take(3).ToList();
                    }
                    if (package == "Basic Plan")
                    {
                        listStudent = listStudent.Take(5).ToList();
                    }
                    if (package == "Professional Plan")
                    {
                        listStudent = listStudent.Take(10).ToList();
                    }
                    if (package == "Advance Plan")
                    {
                        listStudent = listStudent.ToList();
                    }
                    model.LIST_MEMBERS = listStudent;
                    
                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Find Student";
                    activity.FULLNAME = dto.FULLNAME;
                    activity.USER_NAME = dto.FULLNAME;
                    activity.PASSWORD = dto.FULLNAME;
                    activity.STATUS = resp.STATUS;
                    ctx.LOGON_ACTIVITY.Add(activity);
                    #endregion



                    resp.STATUS = true;
                    resp.OUTPUT_DATA = model;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> GetCourseByTeacherID(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = ctx.MEMBER_ROLE.Where(x => x.MEMBER_ID == dto.MEMBER_ID).FirstOrDefault();
                    var checkPackage = ctx.MEMBER_PACKAGE.Where(x => x.MEMBER_ID == dto.MEMBER_ID).FirstOrDefault();

                    var listTeacher = await ctx.MEMBER_TEACH_COURSE.Include("COURSES").Where(x => x.MEMBER_ROLE_ID == memberRole.AUTO_ID).OrderByDescending(x => x.AUTO_ID).ToListAsync();
                    var listCourse = new List<COURSES>();
                    if (checkPackage != null)
                    {
                        //แบบซื้อ package แล้ว
                        foreach (var item in listTeacher)
                        {
                            item.COURSES.MEMBER_TEACH_COURSE = null; //alway clear null
                            listCourse.Add(item.COURSES);
                        }
                    }
                    else
                    {
                        //แบบฟรี
                        foreach (var item in listTeacher)
                        {
                            if (listCourse.Count == 2)
                            {
                                break;
                            }
                            else
                            {
                                item.COURSES.MEMBER_TEACH_COURSE = null; //alway clear null
                                listCourse.Add(item.COURSES);
                            }
                        }
                    }
                    resp.STATUS = true;
                    resp.OUTPUT_DATA = listCourse;
                }
            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                resp.ErrorMessage = ex.Message;

            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> MangeCourse(CONTAINER_MODEL dto, HttpPostedFileBase banner_img)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var course = new COURSES();
                    var teachCourse = new MEMBER_TEACH_COURSE();
                    #region ===== COVER IMAGE ====

                    //string myDir = "D:\\PXProject\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\course\\";
                    string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\course\\";
                    //Deploy
                    //myDir = @"C:\\WebApplication\\coachme.asia\\Content\\images\\course\\";
                    string path = "";
                    myDir += dto.MEMBER_LOGON.USER_NAME;
                    System.IO.Directory.CreateDirectory(myDir);

                    if (banner_img != null)
                    {
                        if (banner_img.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(banner_img.FileName);
                            path = Path.Combine(myDir, fileName);
                            banner_img.SaveAs(path);
                        }
                        int index = path.IndexOf("Content");
                        course.BANNER_URL = @"\\" + path.Substring(index);
                    }
                    #endregion
                    var memRole = ctx.MEMBER_ROLE.Where(x => x.MEMBER_ID == dto.MEMBER_LOGON.MEMBER_ID).FirstOrDefault();
                    var memTeach = ctx.MEMBER_TEACH_COURSE.Where(x => x.MEMBER_ROLE_ID == memRole.AUTO_ID).ToList();
                    #region =====Course===== 
                    if (dto.COURSES.AUTO_ID > 0)
                    {
                        //Edit
                        var objEdit = ctx.COURSES.Where(x => x.AUTO_ID == dto.COURSES.AUTO_ID).FirstOrDefault();
                        objEdit.NAME = dto.COURSES.NAME;
                        objEdit.DESCRIPTION = dto.COURSES.DESCRIPTION;
                        objEdit.CREATED_BY = dto.MEMBERS.FULLNAME;
                        objEdit.CREATED_DATE = DateTime.Now;
                        objEdit.UPDATED_BY = dto.MEMBERS.FULLNAME;
                        objEdit.UPDATED_DATE = DateTime.Now;
                        objEdit.PRICE = dto.COURSES.PRICE;

                        if (course.BANNER_URL != null)
                        {
                            objEdit.BANNER_URL = course.BANNER_URL;
                        }
                        //add activity
                        #region === Activity ===
                        var activity = new LOGON_ACTIVITY();
                        activity.DATE = DateTime.Now;
                        activity.ACTION = "Update Course";
                        activity.FULLNAME = dto.MEMBERS.FULLNAME;
                        activity.USER_NAME = dto.MEMBER_LOGON.USER_NAME;
                        activity.STATUS = resp.STATUS;
                        ctx.LOGON_ACTIVITY.Add(activity);
                        #endregion
                    }
                    else
                    {
                        //Add
                        course.NAME = dto.COURSES.NAME;
                        course.DESCRIPTION = dto.COURSES.DESCRIPTION;
                        course.CREATED_BY = dto.MEMBERS.FULLNAME;
                        course.CREATED_DATE = DateTime.Now;
                        course.UPDATED_BY = dto.MEMBERS.FULLNAME;
                        course.UPDATED_DATE = DateTime.Now;
                        course.PRICE = dto.COURSES.PRICE;
                        course.SEQ = memTeach.Count + 1;
                        teachCourse.COURSES = course;
                        var role = ctx.MEMBER_ROLE.Where(x => x.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefault();
                        teachCourse.MEMBER_ROLE_ID = role.AUTO_ID;
                        ctx.MEMBER_TEACH_COURSE.Add(teachCourse);

                        //add activity
                        #region === Activity ===
                        var activity = new LOGON_ACTIVITY();
                        activity.DATE = DateTime.Now;
                        activity.ACTION = "Create Course";
                        activity.FULLNAME = dto.MEMBERS.FULLNAME;
                        activity.USER_NAME = dto.MEMBER_LOGON.USER_NAME;
                        activity.STATUS = resp.STATUS;
                        ctx.LOGON_ACTIVITY.Add(activity);
                        #endregion

                    }
                    #endregion

                    await ctx.SaveChangesAsync();
                    resp.STATUS = true;

                }
            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

    }
}
