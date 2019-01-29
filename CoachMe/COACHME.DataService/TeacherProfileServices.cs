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
                                             .Include(o => o.MEMBER_ROLE)
                                             .Include(o => o.MEMBER_LOGON)
                                             .Include(o => o.MEMBER_PACKAGE)
                                             .Where(x => x.AUTO_ID == member_id).FirstOrDefault();

                    memberProfile.MEMBER_PACKAGE = memberProfile.MEMBER_PACKAGE
                                                   .Where(x => x.STATUS != "HOLD")
                                                   .OrderByDescending(x => x.AUTO_ID)
                                                   .Take(1)
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

        public RESPONSE__MODEL GetMemberProfileFromAutoID(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = ctx.MEMBERS.Include("MEMBER_LOGON")
                                                   .Include("MEMBER_ROLE")
                                                   .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefault();
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
                    string fullDir = @"C://WebApplication//coachme.asia//Content//images//ProfileFull//";
                    #endregion
                    #region ==== ROCK PATH ====
                    //myDir = "D://PXProject//CoachMe//CoachMe//CoachMe//Content//images//Profile//";
                    #endregion
                    #region ==== P'X PATH ====
                    //myDir = @"C:\\Users\\Rock\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\Profile\\";
                    #endregion

                    string path = "";
                    string fullPath = "";
                    var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    //string[] FolderProfile = memberUsername.USER_NAME.Split('@');

                    myDir += memberUsername.USER_NAME;
                    System.IO.Directory.CreateDirectory(myDir);
                    fullDir += memberUsername.USER_NAME;
                    System.IO.Directory.CreateDirectory(fullDir);

                    //Upload Pic
                    if (profileImage.ContentLength > 0)
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-profile-image" + Path.GetExtension(profileImage.FileName);
                        path = Path.Combine(myDir, fileName);
                        profileImage.SaveAs(path);

                        fullPath = Path.Combine(fullDir, fileName).Replace("\\", "//");
                        profileImage.SaveAs(fullPath);

                        Bitmap bimage = new Bitmap(path);
                        resizeImage(bimage, path);
                    }
                    //updateMemberProfileUrl
                    var member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    int index = path.IndexOf("Content");
                    member.PROFILE_IMG_URL = @"//" + path.Substring(index);
                    member.PROFILE_IMG_URL_FULL = @"//" + fullPath.Substring(index);
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
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-1" + Path.GetExtension(about_img[0].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[0].SaveAs(path);
                                changeCorrectOrientation(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL1 = "//" + path.Substring(index);
                        }
                        if (i == 1 && about_img[1] != null)
                        {

                            if (about_img[1].ContentLength > 0)
                            {
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-2" + Path.GetExtension(about_img[1].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[1].SaveAs(path);
                                changeCorrectOrientation(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL2 = "\\" + path.Substring(index);
                        }
                        if (i == 2 && about_img[2] != null)
                        {

                            if (about_img[2].ContentLength > 0)
                            {
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-3" + Path.GetExtension(about_img[2].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[2].SaveAs(path);
                                changeCorrectOrientation(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL3 = "\\" + path.Substring(index);
                        }
                        if (i == 3 && about_img[3] != null)
                        {

                            if (about_img[3].ContentLength > 0)
                            {
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-4" + Path.GetExtension(about_img[3].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[3].SaveAs(path);
                                changeCorrectOrientation(path);
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
                    if (dto.LINE_ID != null)
                    {
                        member.LINE_ID = dto.LINE_ID;
                    }
                    if (dto.FACEBOOK_URL != null)
                    {
                        member.FACEBOOK_URL = dto.FACEBOOK_URL;
                    }
                    member.TEACHING_TYPE = dto.TEACHING_TYPE;
                    member.STUDENT_LEVEL = dto.STUDENT_LEVEL;
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
                    List<MEMBER_CATEGORY> userCate = ctx.MEMBER_CATEGORY.Where(o => o.MEMBER_ID == dto.AUTO_ID).ToList();
                    ctx.MEMBER_CATEGORY.RemoveRange(userCate);
                    foreach (var item in category)
                    {

                        var cateID = await ctx.CATEGORY.Where(o => o.NAME == item).Select(o => o.AUTO_ID).FirstAsync();
                        var memberCategory = new MEMBER_CATEGORY();
                        memberCategory.MEMBER_ID = dto.AUTO_ID;
                        memberCategory.CATEGORY_ID = cateID;
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
                                obj.ACCEPT_BY = "COURSE";
                                listCustom.Add(obj);
                            }
                        }
                    }
                    var memberRoleID = memberProfile.MEMBER_ROLE.Select(o => o.AUTO_ID).FirstOrDefault();
                    var listIDAcceptByTeacher = ctx.MEMBER_MATCHING.Where(o => o.TEACHER_ROLE_ID == memberRoleID)
                                             .Select(k => k.STUDENT_ROLE_ID).ToList();

                    var listStudentAcceptByTeacher = ctx.MEMBERS.Where(o => listIDAcceptByTeacher.Contains(o.MEMBER_ROLE.FirstOrDefault().AUTO_ID)).ToList();
                    foreach (var member in listStudentAcceptByTeacher)
                    {
                        var obj = new CUSTOM_MEMBERS();
                        obj.AUTO_ID = member.AUTO_ID;

                        obj.PROFILE_IMG_URL = member.PROFILE_IMG_URL;
                        //obj.STATUS = member.sta
                        obj.FULLNAME = member.FULLNAME ?? "ไม่ระบุ";
                        obj.SEX = member.SEX == "1" ? "ชาย" : "หญิง";
                        obj.AGE = member.AGE ?? 0;
                        obj.LOCATION = member.LOCATION ?? "ไม่ระบุ";
                        obj.MOBILE = member.MOBILE ?? "ไม่ระบุ";
                        obj.COURSE = "Match by teacher";
                        obj.ABOUT = member.ABOUT ?? "ไม่ระบุ";
                        obj.LIST_STUDENT_COMMENT = new List<string> { "ไม่ระบุ" };
                        obj.REGIS_COURSE_ID = 999;
                        obj.ACCEPT_BY = "TEACHER";
                        listCustom.Add(obj);

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
                        //แบบฟรี or expire
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

        public async Task<RESPONSE__MODEL> GetMemberPackageByTeacherID(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberPackage = await ctx.MEMBER_PACKAGE.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                    resp.STATUS = true;
                    resp.OUTPUT_DATA = memberPackage;
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

                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-banner-image" + Path.GetExtension(banner_img.FileName);
                            path = Path.Combine(myDir, fileName);
                            banner_img.SaveAs(path);
                            changeCorrectOrientation(path);
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
                        objEdit.CREATED_BY = dto.MEMBERS.FIRST_NAME;
                        objEdit.CREATED_DATE = DateTime.Now;
                        objEdit.UPDATED_BY = dto.MEMBERS.FIRST_NAME;
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
                        activity.FULLNAME = dto.MEMBERS.FIRST_NAME;
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
                resp.Message = ex.Message;
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        private void resizeImage(Image imgToResize, string path)
        {
            try
            {


                foreach (var prop in imgToResize.PropertyItems)
                {
                    if ((prop.Id == 0x0112 || prop.Id == 5029 || prop.Id == 274))
                    {
                        var value = (int)prop.Value[0];
                        if (value == 6)
                        {
                            imgToResize.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        }
                        else if (value == 8)
                        {
                            imgToResize.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        }
                        else if (value == 3)
                        {
                            imgToResize.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        }
                    }
                }

                var imageResized = new Bitmap(imgToResize, 800, 500);

                imgToResize.Dispose();
                imgToResize = null;
                imageResized.Save(path, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {

            }
        }

        private void changeCorrectOrientation(string path)
        {
            try
            {
                Bitmap imgToResize = new Bitmap(path);

                foreach (var prop in imgToResize.PropertyItems)
                {
                    if ((prop.Id == 0x0112 || prop.Id == 5029 || prop.Id == 274))
                    {
                        var value = (int)prop.Value[0];
                        if (value == 6)
                        {
                            imgToResize.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        }
                        else if (value == 8)
                        {
                            imgToResize.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        }
                        else if (value == 3)
                        {
                            imgToResize.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        }
                    }
                }


                imgToResize.Dispose();
                imgToResize = null;
                imgToResize.Save(path, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {

            }
        }


        #region ======================= ANGULAR JS =======================
        public async Task<RESPONSE__MODEL> GetListTeacherCategory(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    resp.OUTPUT_DATA = await ctx.MEMBER_CATEGORY
                                                .Where(o => o.MEMBER_ID == dto.AUTO_ID)

                                                .ToListAsync();
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> GetListAvailableCategory(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    //List<string> category = new List<string> { "ดนตรี", "ภาษา", "ธุรกิจและการเงิน", "ศิลปะ", "เทคโนโลยี", "วิชาชีพ/หลักสูตรพิเศษ", "สุขภาพ/ความงาม" };
                    List<int> category = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                    var memberCategory = await ctx.MEMBER_CATEGORY
                                                 .Where(o => o.MEMBER_ID == dto.AUTO_ID)
                                                 .Select(o => o.CATEGORY_ID)
                                                 .ToListAsync();
                    resp.OUTPUT_DATA = category.Where(x => memberCategory.Contains(x)).ToList();
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> GetTeacherProfile(MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberProfile = await ctx.MEMBERS
                                                .Include(o => o.MEMBER_ROLE)
                                                .Where(o => o.AUTO_ID == dto.AUTO_ID)
                                                .ToListAsync();


                    var listItem = (from item in memberProfile
                                    select new CUSTOM_MEMBERS
                                    {
                                        AUTO_ID = memberProfile.FirstOrDefault().AUTO_ID,
                                        FULLNAME = memberProfile.FirstOrDefault().FULLNAME ?? "ไม่ระบุ",
                                        FIRST_NAME = memberProfile.FirstOrDefault().FIRST_NAME ?? "ไม่ระบุ",
                                        LAST_NAME = memberProfile.FirstOrDefault().LAST_NAME ?? "ไม่ระบุ",
                                        MOBILE = memberProfile.FirstOrDefault().MOBILE ?? "ไม่ระบุ",
                                        NICKNAME = memberProfile.FirstOrDefault().NICKNAME ?? "ไม่ระบุ",
                                        DATE_OF_BIRTH_TEXT = (memberProfile.FirstOrDefault().DATE_OF_BIRTH.HasValue) ? memberProfile.FirstOrDefault().DATE_OF_BIRTH.Value.ToShortDateString() : "ไม่ระบุ",
                                        SEX = memberProfile.FirstOrDefault().SEX ?? "ไม่ระบุ",
                                        ABOUT = memberProfile.FirstOrDefault().ABOUT ?? "ไม่ระบุ",
                                        AMPHUR_ID = memberProfile.FirstOrDefault().AMPHUR_ID ?? 0,
                                        TEACHING_TYPE = memberProfile.FirstOrDefault().TEACHING_TYPE ?? 0,
                                        STUDENT_LEVEL = memberProfile.FirstOrDefault().STUDENT_LEVEL ?? 0,
                                        LOCATION = memberProfile.FirstOrDefault().LOCATION ?? "ไม่ระบุ",
                                        FACEBOOK_URL = memberProfile.FirstOrDefault().FACEBOOK_URL,
                                        LINE_ID = memberProfile.FirstOrDefault().LINE_ID,
                                        ROLE = memberProfile.FirstOrDefault().MEMBER_ROLE.FirstOrDefault().ROLE_ID.ToString(),
                                        ABOUT_IMG_1 = memberProfile.FirstOrDefault().ABOUT_IMG_URL1,
                                        ABOUT_IMG_2 = memberProfile.FirstOrDefault().ABOUT_IMG_URL2,
                                        ABOUT_IMG_3 = memberProfile.FirstOrDefault().ABOUT_IMG_URL3,
                                        ABOUT_IMG_4 = memberProfile.FirstOrDefault().ABOUT_IMG_URL4,
                                        PROFILE_IMG_URL = memberProfile.FirstOrDefault().PROFILE_IMG_URL,

                                    }).ToList();

                    
                    foreach (var item in listItem)
                    {
                        if (item.ROLE == "2")
                        {
                            item.STAGE = 4;
                        }
                        else if (item.PROFILE_IMG_URL == null)
                        {
                            item.STAGE = 1;
                        }
                        else if (
                            (item.FIRST_NAME == null || item.FIRST_NAME == "ไม่ระบุ") ||
                            (item.LAST_NAME == null || item.LAST_NAME == "ไม่ระบุ") ||
                            (item.MOBILE == null || item.MOBILE == "ไม่ระบุ") ||
                            //(item.NICKNAME == null || item.NICKNAME == "ไม่ระบุ") ||
                            (item.DATE_OF_BIRTH_TEXT == null || item.DATE_OF_BIRTH_TEXT == "ไม่ระบุ") ||
                            (item.SEX == null || item.SEX == "ไม่ระบุ") ||
                            (item.ABOUT == null || item.ABOUT == "ไม่ระบุ") ||
                            (item.AMPHUR_ID == null || item.AMPHUR_ID == 0) ||
                            (item.TEACHING_TYPE == null || item.TEACHING_TYPE == 0) ||
                            (item.STUDENT_LEVEL == null || item.STUDENT_LEVEL == 0) ||
                            (item.LOCATION == null || item.LOCATION == "ไม่ระบุ") ||
                            (item.FACEBOOK_URL == null || item.FACEBOOK_URL == "ไม่ระบุ") ||
                            (item.LINE_ID == null || item.LINE_ID == "ไม่ระบุ")
                            )
                        {
                            item.MESSAGE = new List<string>();
                            if (item.FIRST_NAME == null || item.FIRST_NAME == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("ชื่อ");
                                item.STAGE = 2;
                            }
                            if (item.LAST_NAME == null || item.LAST_NAME == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("นามสกุล");
                                item.STAGE = 2;
                            }
                            if (item.MOBILE == null || item.MOBILE == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("เบอร์มือถือ");
                                item.STAGE = 2;
                            }
                            if (item.NICKNAME == null || item.NICKNAME == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("ชื่อเล่น");
                                item.STAGE = 2;
                            }
                            if (item.DATE_OF_BIRTH_TEXT == null || item.DATE_OF_BIRTH_TEXT == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("วันเกิด");
                                item.STAGE = 2;
                            }
                            if (item.SEX == null || item.SEX == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("เพศ");
                                item.STAGE = 2;
                            }
                            if (item.ABOUT == null || item.ABOUT == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("เกี่ยวกับ");
                                item.STAGE = 2;
                            }
                            if (item.AMPHUR_ID == null || item.AMPHUR_ID == 0)
                            {
                                item.MESSAGE.Add("ที่อยู่");
                                item.STAGE = 2;
                            }
                            if (item.TEACHING_TYPE == null || item.TEACHING_TYPE == 0)
                            {
                                item.MESSAGE.Add("ประเภทการสอน");
                                item.STAGE = 2;
                            }
                            if (item.STUDENT_LEVEL == null || item.STUDENT_LEVEL == 0)
                            {
                                item.MESSAGE.Add("ประเถทนักเรียน");
                                item.STAGE = 2;
                            }
                            if (item.LOCATION == null || item.LOCATION == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("สถานที่สอน");
                                item.STAGE = 2;
                            }
                            if (item.FACEBOOK_URL == null || item.FACEBOOK_URL == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("เฟสบุ๊ค");
                                item.STAGE = 2;
                            }
                            if (item.LINE_ID == null || item.LINE_ID == "ไม่ระบุ")
                            {
                                item.MESSAGE.Add("ไลน์");
                                item.STAGE = 2;
                            }
                        }
                        else if (
                            (item.ABOUT_IMG_1 == null || item.ABOUT_IMG_1 == "ไม่ระบุ") &&
                            (item.ABOUT_IMG_2 == null || item.ABOUT_IMG_2 == "ไม่ระบุ") &&
                            (item.ABOUT_IMG_3 == null || item.ABOUT_IMG_3 == "ไม่ระบุ") &&
                            (item.ABOUT_IMG_4 == null || item.ABOUT_IMG_4 == "ไม่ระบุ")
                            )
                        {
                            item.STAGE = 3;
                        }
                        else
                        {
                            item.STAGE = 4;
                        }




                    }

                    resp.OUTPUT_DATA = listItem;

                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> UpdateMemberProfile(CUSTOM_MEMBERS dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var member = await ctx.MEMBERS
                                          .Include(s => s.MEMBER_LOGON)
                                          .Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    #region =====Profile=====
                    //Update Profile
                    if (dto.FIRST_NAME != null && dto.LAST_NAME != null)
                    {
                        member.FULLNAME = dto.FIRST_NAME + " " + dto.LAST_NAME;
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
                    if (dto.SEX_RADIO != null)
                    {
                        member.SEX = dto.SEX_RADIO;
                    }
                    if (dto.DATE_OF_BIRTH_TEXT != null)
                    {
                        DateTime value;
                        if (DateTime.TryParse(dto.DATE_OF_BIRTH_TEXT, out value))
                        {
                            member.DATE_OF_BIRTH = Convert.ToDateTime(dto.DATE_OF_BIRTH_TEXT);
                        }
                    }
                    if (dto.LOCATION != null)
                    {
                        member.LOCATION = dto.LOCATION;
                    }
                    if (dto.AMPHUR_ID != null && dto.AMPHUR_ID > 0)
                    {
                        member.AMPHUR_ID = dto.AMPHUR_ID;
                    }
                    if (dto.TEACHING_TYPE != null)
                    {
                        member.TEACHING_TYPE = dto.TEACHING_TYPE;
                    }
                    if (dto.STUDENT_LEVEL != null)
                    {
                        member.STUDENT_LEVEL = dto.STUDENT_LEVEL;
                    }

                    if (dto.LINE_ID != null)
                    {
                        member.LINE_ID = dto.LINE_ID;
                    }
                    if (dto.FACEBOOK_URL != null)
                    {
                        member.FACEBOOK_URL = dto.FACEBOOK_URL;
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
                throw ex;
            }


            return resp;
        }

        public async Task<RESPONSE__MODEL> UpdateMemberProfileAboutImg(MEMBERS dto, List<HttpPostedFileBase> about_img)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var member = await ctx.MEMBERS.Where(x => x.AUTO_ID == dto.AUTO_ID).FirstOrDefaultAsync();

                    #region ===== ABOUT IMAGE ====
                    for (int i = 0; i < about_img.Count; i++)
                    {
                        var memberUsername = await ctx.MEMBER_LOGON.Where(x => x.MEMBER_ID == dto.AUTO_ID).FirstOrDefaultAsync();
                        //string myDir = "D://PXProject//CoachMe//CoachMe//CoachMe//Content//images//About//";
                        //string myDir = @"C:\\Users\\Prakasit\\Source\\Repos\\CoachMe\\CoachMe\\CoachMe\\Content\\images\\About\\";
                        //Deploy
                        string myDir = @"C://WebApplication//coachme.asia//Content//images//About//";
                        string path = "";
                        string[] FolderProfile = memberUsername.USER_NAME.Split('@');
                        myDir += memberUsername.USER_NAME; //FolderProfile[0].ToUpper() + " " + FolderProfile[1].ToUpper();
                        System.IO.Directory.CreateDirectory(myDir);

                        if (i == 0 && about_img[0] != null)
                        {

                            if (about_img[0].ContentLength > 0)
                            {
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-1" + Path.GetExtension(about_img[0].FileName);//Path.GetFileName(about_img[0].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[0].SaveAs(path);
                                changeCorrectOrientation(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL1 = "//" + path.Substring(index);
                        }
                        if (i == 1 && about_img[1] != null)
                        {

                            if (about_img[1].ContentLength > 0)
                            {
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-2" + Path.GetExtension(about_img[1].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[1].SaveAs(path);
                                changeCorrectOrientation(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL2 = "//" + path.Substring(index);
                        }
                        if (i == 2 && about_img[2] != null)
                        {

                            if (about_img[2].ContentLength > 0)
                            {
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-3" + Path.GetExtension(about_img[2].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[2].SaveAs(path);
                                changeCorrectOrientation(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL3 = "//" + path.Substring(index);
                        }
                        if (i == 3 && about_img[3] != null)
                        {

                            if (about_img[3].ContentLength > 0)
                            {
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-about-image-4" + Path.GetExtension(about_img[3].FileName);
                                path = Path.Combine(myDir, fileName);
                                about_img[3].SaveAs(path);
                                changeCorrectOrientation(path);
                            }
                            int index = path.IndexOf("Content");
                            member.ABOUT_IMG_URL4 = "//" + path.Substring(index);
                        }



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
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> GetGeography()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    resp.OUTPUT_DATA = await ctx.GEOGRAPHY.ToListAsync();
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> GetListProvince(int geoID)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    resp.OUTPUT_DATA = await ctx.PROVINCE
                                                .Where(o => o.GEO_ID == geoID)
                                                .ToListAsync();
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;

        }

        public async Task<RESPONSE__MODEL> GetListAmphur(int provinceID)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    resp.OUTPUT_DATA = await ctx.AMPHUR
                                                .Where(o => o.PROVINCE_ID == provinceID)
                                                .ToListAsync();
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> GetMemberAdress(MEMBERS dto)
        {

            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {

                using (var ctx = new COACH_MEEntities())
                {
                    var memberAmpherID = await ctx.MEMBERS.Where(o => o.AUTO_ID == dto.AUTO_ID).Select(o => o.AMPHUR_ID).FirstOrDefaultAsync();

                    if (memberAmpherID != null)
                    {
                        var ampher = await ctx.AMPHUR.Where(o => o.AMPHUR_ID == memberAmpherID).FirstOrDefaultAsync();
                        var province = await ctx.PROVINCE.Where(o => o.PROVINCE_ID == ampher.PROVINCE_ID).FirstOrDefaultAsync();
                        var geography = await ctx.GEOGRAPHY.Where(o => o.GEO_ID == ampher.GEO_ID).FirstOrDefaultAsync();

                        List<string> address = new List<string>();

                        address.Add(ampher.AMPHUR_ID.ToString());
                        address.Add(ampher.AMPHUR_NAME);

                        address.Add(ampher.PROVINCE_ID.ToString());
                        address.Add(province.PROVINCE_NAME);

                        address.Add(ampher.GEO_ID.ToString());
                        address.Add(geography.GEO_NAME);
                        resp.OUTPUT_DATA = address;
                    }
                    else
                    {
                        resp.OUTPUT_DATA = null;
                    }


                }

                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        #endregion

    }
}
