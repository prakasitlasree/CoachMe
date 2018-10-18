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
using System.Drawing;
using System.Drawing.Imaging;

namespace COACHME.DATASERVICE
{
    public class TeacherProfileServices
    {
        public string package = "";
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
                                                   .Where(x => x.STATUS != "DRAFT" && x.STATUS != "HOLD")
                                                   .OrderByDescending(x => x.AUTO_ID)
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

        public async Task<RESPONSE__MODEL> ManageSurvey(MEMBERS dto)
        {
            var resp = new RESPONSE__MODEL();
            var member_id = dto.AUTO_ID;
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = await ctx.MEMBERS.Where(x => x.AUTO_ID == member_id).FirstOrDefaultAsync();

                    var obj = new SURVEYS();
                    obj.NAME = StandardEnums.SurveyType.Interest.ToString();
                    obj.RESPONSE_TYPE = "Survey";
                    obj.DESCRIPTION = "Dashboard-Announcements";
                    obj.RESPONSE_BY = memberProfile.FULLNAME;
                    obj.RESPONSE_DATE = DateTime.Now;
                    obj.RESPONSE_COUNT = 1;
                    ctx.SURVEYS.Add(obj);
                    var result = await ctx.SaveChangesAsync();
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
                    string myDir = @"C://WebApplication//coachme.asia//Content//images//Profile//";
                    #endregion
                    #region ==== ROCK PATH ====
                    myDir = "D://PXProject//CoachMe//CoachMe//CoachMe//Content//images//Profile//";
                    #endregion
                    #region ==== P'X PATH ====
                    // myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\";
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
                        Bitmap bimage = new Bitmap(path);
                        resizeImage(bimage, path);
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

                    var memberLogon = await ctx.MEMBERS
                                        .Include("MEMBER_ROLE")
                                        .Include("MEMBER_PACKAGE")
                                        .Include("MEMBER_LOGON")
                                        .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

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
                    member = await ctx.MEMBERS
                                        .Include("MEMBER_ROLE")
                                        .Include("MEMBER_PACKAGE")
                                        .Include("MEMBER_LOGON")
                                        .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    #region ===== ABOUT IMAGE ====
                    for (int i = 0; i < about_img.Count; i++)
                    {
                        var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                        string myDir = "D://PXProject//CoachMe//CoachMe//CoachMe//Content//images//About//";
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
                    resp.OUTPUT_DATA = member;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> UpdateMemberCategoryProfile(MEMBERS dto, string[] category)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    foreach (var item in category)
                    {
                        var memberCategory = new MEMBER_CATEGORY();
                        memberCategory.MEMBER_ID = dto.AUTO_ID;
                        memberCategory.NAME = item;
                        memberCategory.CREATED_BY = dto.AUTO_ID.ToString();
                        memberCategory.UPDATED_BY = dto.AUTO_ID.ToString();
                        memberCategory.CREATED_DATE = DateTime.Now;
                        memberCategory.UPDATED_DATE = DateTime.Now;
                        ctx.MEMBER_CATEGORY.Add(memberCategory);
                    }
                   
                                    
                    //add activity
                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Update Member Category";
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
                //using (var ctx = new COACH_MEEntities())
                //{
                //    var memberProfile = await ctx.MEMBERS
                //       .Include("MEMBER_LOGON")
                //       .Include("MEMBER_PACKAGE")
                //       .Include(a => a.MEMBER_ROLE.Select(c => c.MEMBER_TEACH_COURSE))
                //       .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                //    var teachCourse = memberProfile.MEMBER_ROLE.FirstOrDefault()
                //                                        .MEMBER_TEACH_COURSE.Select(o => o.COURSE_ID).ToArray();
                
                //    var listStu = await (from a in ctx.MEMBER_REGIS_COURSE
                //                         join b in ctx.MEMBER_ROLE on a.REGISTER_ID equals b.AUTO_ID
                //                         join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                //                         //join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                //                         join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                //                         where teachCourse.Contains(d.AUTO_ID)
                //                         select new CUSTOM_MEMBERS
                //                         {
                //                             AUTO_ID = c.AUTO_ID,
                //                             PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                //                             STATUS = a.STATUS,
                //                             FULLNAME = c.FULLNAME ?? "",
                //                             SEX = c.SEX == "1" ? "ชาย" : "หญิง",
                //                             AGE = c.AGE,
                //                             LOCATION = c.LOCATION ?? "",
                //                             MOBILE = c.MOBILE ?? "",
                //                             USER_NAME = f.USER_NAME ?? "",
                //                             COURSE = d.NAME ?? "",
                //                             ABOUT = c.ABOUT ?? "",
                //                             //LIST_STUDENT_COMMENT = a.MEMBER_REGIS_COURSE_COMMENT.Select(o => o.COMMENT).ToList(),
                //                             REGIS_COURSE_ID = a.AUTO_ID 
                //                         }).ToListAsync(); 
                //    model.LIST_CUSTOM_MEMBERS = listStu;
                //    model.MEMBERS = memberProfile;
                 
                //    var regisCourse = ctx.MEMBER_REGIS_COURSE.Where(o => teachCourse.Contains(o.COURSE_ID))
                //                                             .Select(o => o.MEMBER_ROLE_ID)
                //                                             .ToList();

                //    var courseName = await ctx.COURSES
                //                              .Where(o => teachCourse.Contains(o.AUTO_ID)).ToListAsync();

                //    var listStudent = await ctx.MEMBERS
                //                                .Include(x => x.MEMBER_ROLE.Select(o => o.MEMBER_REGIS_COURSE))
                //                                .Include(x => x.MEMBER_LOGON)
                //                                .Where(MEMBERS => MEMBERS.MEMBER_ROLE.Any(o => regisCourse.Contains(o.AUTO_ID)))
                //                                .ToListAsync();
                 
                //    package = memberProfile.MEMBER_PACKAGE
                //                                .Where(x => x.STATUS != "DRAFT" && x.EXPIRE_DATE > DateTime.Now)
                //                                .Select(o => o.PACKAGE_NAME).LastOrDefault();

                //    if (package == null)
                //    {
                //        listStudent = listStudent.Take(3).ToList();
                //    }
                //    if (package == "Basic Plan")
                //    {
                //        listStudent = listStudent.Take(5).ToList();
                //    }
                //    if (package == "Professional Plan")
                //    {
                //        listStudent = listStudent.Take(10).ToList();
                //    }
                //    if (package == "Advance Plan")
                //    {
                //        listStudent = listStudent.ToList();
                //    }
                //    model.LIST_MEMBERS = listStudent;

                //    #region === Activity ===
                //    var activity = new LOGON_ACTIVITY();
                //    activity.DATE = DateTime.Now;
                //    activity.ACTION = "Find Student";
                //    activity.FULLNAME = dto.FULLNAME;
                //    activity.USER_NAME = dto.FULLNAME;
                //    activity.PASSWORD = dto.FULLNAME;
                //    activity.STATUS = resp.STATUS;
                //    ctx.LOGON_ACTIVITY.Add(activity);
                //    #endregion
                 
                //    resp.STATUS = true;
                //    resp.OUTPUT_DATA = model;
                //}
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }

        public RESPONSE__MODEL FindStudent_new(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = ctx.MEMBERS
                        .Include("MEMBER_LOGON")
                        .Include("MEMBER_PACKAGE")
                        .Include(a => a.MEMBER_ROLE.Select(c => c.MEMBER_TEACH_COURSE))
                        .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefault();
                    var teachCourse = memberProfile.MEMBER_ROLE.FirstOrDefault();

                    var listStudent = ctx.MEMBERS.ToList();

                    var listCustom = new List<CUSTOM_MEMBERS>();

                    if (teachCourse != null)
                    {
                        foreach (var item in teachCourse.MEMBER_TEACH_COURSE)
                        {
                            var registerList = ctx.MEMBER_REGIS_COURSE.Where(x => x.TEACH_COURSE_ID == item.AUTO_ID).ToList();
                            foreach (var register in registerList)
                            {
                                var obj = new CUSTOM_MEMBERS();
                                obj.AUTO_ID = register.REGIS_MEMBER_ROLE_ID;
                                var student = ctx.MEMBER_ROLE.Where(x => x.AUTO_ID == register.REGIS_MEMBER_ROLE_ID).FirstOrDefault(); 
                                obj.PROFILE_IMG_URL = student.MEMBERS.PROFILE_IMG_URL;
                                obj.STATUS = register.STATUS;
                                obj.FULLNAME = student.MEMBERS.FULLNAME ?? "";
                                obj.SEX = student.MEMBERS.SEX == "1" ? "ชาย" : "หญิง";
                                obj.AGE = student.MEMBERS.AGE;
                                obj.LOCATION = student.MEMBERS.LOCATION ?? "";
                                obj.MOBILE = student.MEMBERS.MOBILE ?? "";
                                var course = ctx.COURSES.Where(x => x.AUTO_ID == item.COURSE_ID).FirstOrDefault();
                                obj.COURSE = course.NAME ?? "";
                                obj.ABOUT = student.MEMBERS.ABOUT ?? "";
                                obj.LIST_STUDENT_COMMENT = ctx.COURSE_COMMENT.Where(x => x.COURSE_ID == item.COURSE_ID && x.USER_ROLE_ID == student.AUTO_ID).Select(p => p.COMMENT).ToList();
                                obj.REGIS_COURSE_ID = register.AUTO_ID;

                                listCustom.Add(obj);
                            }
                        }
                    } 
                    model.MEMBERS = memberProfile; 
                     
                    package = memberProfile.MEMBER_PACKAGE
                                                .Where(x => x.STATUS != "DRAFT" && x.EXPIRE_DATE > DateTime.Now)
                                                .Select(o => o.PACKAGE_NAME).LastOrDefault();

                    if (package == null)
                    {
                        listCustom = listCustom.Take(3).ToList();
                    }
                    if (package == "Basic Plan")
                    {
                        listCustom = listCustom.Take(5).ToList();
                    }
                    if (package == "Professional Plan")
                    {
                        listCustom = listCustom.Take(10).ToList();
                    }
                    if (package == "Advance Plan")
                    {
                        listCustom = listCustom.ToList();
                    }
                    model.LIST_CUSTOM_MEMBERS = listCustom;
                    model.LIST_MEMBERS = listStudent;

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

        public async Task<RESPONSE__MODEL> AcceptStudent(CONTAINER_MODEL dto, string AcceptStudent)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            int ID = Convert.ToInt32(AcceptStudent);
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var regisCourse = await ctx.MEMBER_REGIS_COURSE.Where(o => o.AUTO_ID == ID).FirstOrDefaultAsync();
                    var member = await ctx.MEMBERS
                                        .Include("MEMBER_LOGON")
                                        .Where(o => o.AUTO_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    regisCourse.STATUS = "ACTIVE";
                    regisCourse.CREATED_BY = member.FULLNAME;
                    regisCourse.CREATED_DATE = DateTime.Now;

                    #region === Activity ===
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "Update Profile Pic";
                    activity.FULLNAME = member.FULLNAME;
                    activity.USER_NAME = member.MEMBER_LOGON.Select(x => x.USER_NAME).FirstOrDefault();
                    activity.PASSWORD = member.MEMBER_LOGON.Select(x => x.PASSWORD).FirstOrDefault();
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

        public async Task<RESPONSE__MODEL> GetCourseByTeacherID(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = ctx.MEMBER_ROLE.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefault();
                    var checkPackage = ctx.MEMBER_PACKAGE.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefault();

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
                    myDir = @"C:\\WebApplication\\coachme.asia\\Content\\images\\course\\";
                    string path = "";
                    var name = ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefault();
                    myDir += name.USER_NAME;
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
                    var memRole = ctx.MEMBER_ROLE.Where(x => x.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefault();
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
                        //activity.USER_NAME = dto.MEMBER_LOGON.USER_NAME;
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
                        teachCourse.CREATED_BY = dto.MEMBERS.FULLNAME;
                        teachCourse.CREATED_DATE = DateTime.Now;
                        teachCourse.UPDATED_BY = dto.MEMBERS.FULLNAME;
                        teachCourse.UPDATED_DATE = DateTime.Now;
                        ctx.MEMBER_TEACH_COURSE.Add(teachCourse);

                        //add activity
                        #region === Activity ===
                        var activity = new LOGON_ACTIVITY();
                        activity.DATE = DateTime.Now;
                        activity.ACTION = "Create Course";
                        activity.FULLNAME = dto.MEMBERS.FULLNAME;
                        activity.USER_NAME = dto.MEMBERS.FIRST_NAME;
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

        private void resizeImage(Image imgToResize, string path)
        {
            try
            {
                var imageResized = new Bitmap(imgToResize, 800, 500);
                imgToResize.Dispose();
                imgToResize = null;
                imageResized.Save(path, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {

            }
        }


    }
}
