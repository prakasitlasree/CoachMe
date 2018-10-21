using COACHME.DAL;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.DATASERVICE
{
    public class StudentServices
    {
        public async Task<RESPONSE__MODEL> GetListCourseName()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    //var listCourse = await ctx.COURSES.ToListAsync(); //.Select(o => o.NAME).ToListAsync();
                    var listTeach = await ctx.MEMBER_TEACH_COURSE.Include(x => x.COURSES)
                                    .Include(x => x.MEMBER_ROLE.MEMBERS.MEMBER_LOGON)
                                    .Where(x => x.MEMBER_ROLE.MEMBERS.MEMBER_LOGON.FirstOrDefault().STATUS == 2).ToListAsync();
                    var listCourse = listTeach.Select(x => x.COURSES).ToList();

                    resp.OUTPUT_DATA = listCourse;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }
            return resp;
        }




        public async Task<RESPONSE__MODEL> AcceptTeacher(SEARCH_TEACHER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    var teachCourse = await ctx.MEMBER_TEACH_COURSE.Where(x => x.COURSE_ID == dto.COURSE_ID).FirstOrDefaultAsync();
                    var regisCourse = new MEMBER_REGIS_COURSE();
                    regisCourse.REGIS_MEMBER_ROLE_ID = memberRole.AUTO_ID;
                    regisCourse.TEACH_COURSE_ID = teachCourse.AUTO_ID;
                    regisCourse.DESCRIPTION = dto.COURSE_NAME;

                    ctx.MEMBER_REGIS_COURSE.Add(regisCourse);
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

        #region ===== BEFORE LOGIN ======

        public async Task<RESPONSE__MODEL> GetListAllTeacherBeforeLogin()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var listMemberCate = await ctx.MEMBER_CATEGORY.Include(x => x.CATEGORY).ToListAsync();

                    var list = await (from a in ctx.MEMBERS
                                      join b in ctx.MEMBER_ROLE on a.AUTO_ID equals b.MEMBER_ID
                                      join c in ctx.MEMBER_LOGON on a.AUTO_ID equals c.MEMBER_ID
                                      where b.ROLE_ID == 1 && c.STATUS == 2
                                      select new
                                      CUSTOM_MEMBERS
                                      {
                                          ROLE = b.ROLE_ID.ToString(),
                                          AUTO_ID = a.AUTO_ID,
                                          PROFILE_IMG_URL = a.PROFILE_IMG_URL,
                                          MEMBER_ROLE_ID = b.AUTO_ID,
                                          FULLNAME = a.FULLNAME ?? "ไม่ระบุ",
                                          SEX = a.SEX == "1" ? "ชาย" : "หญิง",
                                          AGE = a.AGE,
                                          LOCATION = a.LOCATION ?? "ไม่ระบุ",
                                          MOBILE = a.MOBILE ?? "ไม่ระบุ",
                                          ABOUT = a.ABOUT ?? "ไม่ระบุ",

                                      }).ToListAsync();

                    foreach (var item in list)
                    {
                        item.LIST_MEMBER_CETEGORY = listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID).ToList();
                    }

                    resp.OUTPUT_DATA = list;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }
            return resp;
        }
        public async Task<RESPONSE__MODEL> GetListAllCourseBeforeLogin()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listAllTeachCourse = await ctx.MEMBER_TEACH_COURSE
                                                    .Include(x => x.MEMBER_ROLE.MEMBERS)
                                                    .Include(x => x.COURSES)
                                                    .ToListAsync();

                    var listTeachCourse = (from item in listAllTeachCourse
                                           select new CUSTOM_MEMBERS
                                           {
                                               COURSE_BANNER = item.COURSES.BANNER_URL,
                                               COURSE = item.COURSES.NAME,
                                               FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME,
                                               AGE = item.MEMBER_ROLE.MEMBERS.AGE,
                                               ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT,
                                               REGISTER_STATUS = false
                                           }).ToList();

                    resp.OUTPUT_DATA = listTeachCourse;
                }

            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }
        public async Task<RESPONSE__MODEL> GetListSomeCourseBeforeLogin(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var listAllTeachCourse = await ctx.MEMBER_TEACH_COURSE
                                                    .Include(x => x.COURSES)
                                                    .Include(x => x.MEMBER_ROLE.MEMBERS)
                                                    .Include(x => x.MEMBER_ROLE.MEMBERS.MEMBER_LOGON)
                                                    .Where(o => o.MEMBER_ROLE.MEMBERS.MEMBER_LOGON.FirstOrDefault().STATUS == 2).ToListAsync();
                    var obj = dto.SEARCH_TEACHER_MODEL.LIST_COURSE;

                    #region === Advance Search ====
                    //TEACHING_TYPE
                    if (dto.SEARCH_TEACHER_MODEL.TEACHING_TYPE != null && dto.SEARCH_TEACHER_MODEL.TEACHING_TYPE.Count > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.TEACHING_TYPE.Contains(x.MEMBER_ROLE.MEMBERS.TEACHING_TYPE)).ToList();
                    }
                    //LIST_COURSE
                    if (dto.SEARCH_TEACHER_MODEL.LIST_COURSE != null && dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Count > 0)
                    {
                        if (!(dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Count == 1 && dto.SEARCH_TEACHER_MODEL.LIST_COURSE.ToString() != ""))
                        {
                            // listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Contains(x.COURSES.DESCRIPTION)).ToList();
                        }
                        else
                        {
                            if (dto.SEARCH_TEACHER_MODEL.LIST_COURSE[0].ToString() != "")
                            {
                                // listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Contains(x.COURSES.DESCRIPTION)).ToList();
                            }
                        }
                    }
                    //TEACH_GENDER
                    if (dto.SEARCH_TEACHER_MODEL.TEACH_GENDER != null && dto.SEARCH_TEACHER_MODEL.TEACH_GENDER.Count > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.TEACH_GENDER.Contains(x.MEMBER_ROLE.MEMBERS.SEX)).ToList();
                    }
                    //LOCATION
                    if (dto.SEARCH_TEACHER_MODEL.LOCATION != null)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.LOCATION.Contains(x.MEMBER_ROLE.MEMBERS.LOCATION)).ToList();
                    }
                    //STUDENT_LEVEL
                    if (dto.SEARCH_TEACHER_MODEL.STUDENT_LEVEL != null && dto.SEARCH_TEACHER_MODEL.STUDENT_LEVEL.Count > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.STUDENT_LEVEL.Contains(x.MEMBER_ROLE.MEMBERS.STUDENT_LEVEL)).ToList();
                    }
                    #endregion

                    var listTeachCourse = (from item in listAllTeachCourse
                                           select new CUSTOM_MEMBERS
                                           {
                                               COURSE_BANNER = item.COURSES.BANNER_URL,
                                               COURSE = item.COURSES.NAME,
                                               FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME,
                                               AGE = item.MEMBER_ROLE.MEMBERS.AGE,
                                               ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT,
                                               REGISTER_STATUS = false

                                           }).ToList();

                    resp.OUTPUT_DATA = listTeachCourse;
                }

            }
            catch (Exception ex)
            {

            }
            return resp;

        }
        public async Task<RESPONSE__MODEL> GetListSomeTeacherBeforeLogin(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listTeacher = new List<CUSTOM_MEMBERS>();
                    var listMemberCate = await ctx.MEMBER_CATEGORY.Include(x=>x.CATEGORY).ToListAsync();
                    var listCourse = await ctx.MEMBER_TEACH_COURSE.Include(x => x.COURSES).ToListAsync();

                    listTeacher = await (from a in ctx.MEMBERS
                                         join b in ctx.MEMBER_ROLE on a.AUTO_ID equals b.MEMBER_ID
                                         join c in ctx.MEMBER_LOGON on a.AUTO_ID equals c.MEMBER_ID

                                         where b.ROLE_ID == 1 && c.STATUS == 2

                                         select new
                                         CUSTOM_MEMBERS
                                         {
                                             ROLE = b.ROLE_ID.ToString(),
                                             AUTO_ID = a.AUTO_ID,
                                             PROFILE_IMG_URL = a.PROFILE_IMG_URL,
                                             MEMBER_ROLE_ID = b.AUTO_ID,
                                             FULLNAME = a.FULLNAME ?? "ไม่ระบุ",
                                             SEX = a.SEX == "1" ? "ชาย" : "หญิง",
                                             AGE = a.AGE,
                                             LOCATION = a.LOCATION ?? "ไม่ระบุ",
                                             MOBILE = a.MOBILE ?? "ไม่ระบุ",
                                             ABOUT = a.ABOUT ?? "ไม่ระบุ",
                                         }).ToListAsync();

                    foreach (var item in listTeacher)
                    {
                        item.LIST_MEMBER_CETEGORY = listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID).ToList();
                        item.LIST_MEMBER_TEACH_COURSE = listCourse.Where(x => x.MEMBER_ROLE_ID == item.MEMBER_ROLE_ID).ToList();
                    }

                    #region === Advance Search ====

                    if (dto.SEARCH_TEACHER_MODEL.SELECTED_CATEGORY != null)
                    {
                        int catId = Convert.ToInt32(dto.SEARCH_TEACHER_MODEL.SELECTED_CATEGORY);
                        listTeacher = listTeacher.Where(x => x.LIST_MEMBER_CETEGORY != null).Where(o=> o.LIST_MEMBER_CETEGORY.Any(s => s.CATEGORY_ID == catId)).ToList();
                    }
                    #endregion


                    resp.OUTPUT_DATA = listTeacher;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }
        #endregion

        #region ====== AFTER LOGIN=====
        public async Task<RESPONSE__MODEL> GetListAllTeacherAfterLogin(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                                                     .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                                                     .Select(p => p.TEACH_COURSE_ID).ToListAsync();

                    var listTeacher = await (from a in ctx.MEMBER_TEACH_COURSE
                                             join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                                             join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                                             join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                                             join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                                             where b.ROLE_ID == 1 && f.STATUS == 2 /*&& !memberRegisCourse.Contains(a.AUTO_ID)*/
                                             select new CUSTOM_MEMBERS
                                             {

                                                 ROLE = b.ROLE_ID.ToString(),
                                                 AUTO_ID = c.AUTO_ID,
                                                 PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                                                 COURSE_ID = d.AUTO_ID,
                                                 MEMBER_ROLE_ID = b.AUTO_ID,
                                                 FULLNAME = c.FULLNAME ?? "ไม่ระบุ",
                                                 SEX = c.SEX == "1" ? "ชาย" : "หญิง",
                                                 AGE = c.AGE,
                                                 LOCATION = c.LOCATION ?? "ไม่ระบุ",
                                                 MOBILE = c.MOBILE ?? "ไม่ระบุ",
                                                 USER_NAME = f.USER_NAME ?? "ไม่ระบุ",
                                                 COURSE = d.NAME ?? "ไม่ระบุ",
                                                 ABOUT = c.ABOUT ?? "ไม่ระบุ",
                                                 REGIS_COURSE_ID = a.AUTO_ID,
                                                 CATEGORY = "ไม่ระบุ",
                                                 REGISTER_STATUS = (memberRegisCourse.Contains(a.AUTO_ID)) ? true : false
                                             }).OrderByDescending(o => o.REGISTER_STATUS).ToListAsync();

                    resp.OUTPUT_DATA = listTeacher;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }
            return resp;


        }

        public async Task<RESPONSE__MODEL> GetListCategory()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var result = await ctx.CATEGORY.OrderBy(x => x.AUTO_ID).ToListAsync();
                    resp.OUTPUT_DATA = result;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }
            return resp;


        }
        public async Task<RESPONSE__MODEL> GetListAllCourseAfterLogin(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();

                    var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                                                     .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                                                     .Select(p => p.TEACH_COURSE_ID).ToListAsync();

                    var listAllTeachCourse = await ctx.MEMBER_TEACH_COURSE
                                                    .Include(x => x.MEMBER_ROLE.MEMBERS)
                                                    .Include(x => x.COURSES)
                                                    .ToListAsync();

                    var listTeachCourse = (from item in listAllTeachCourse
                                           select new CUSTOM_MEMBERS
                                           {
                                               AUTO_ID = item.AUTO_ID,
                                               COURSE_BANNER = item.COURSES.BANNER_URL,
                                               COURSE = item.COURSES.NAME,
                                               COURSE_ID = item.COURSES.AUTO_ID,
                                               FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME,
                                               AGE = item.MEMBER_ROLE.MEMBERS.AGE,
                                               ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT,
                                               REGISTER_STATUS = (memberRegisCourse.Contains(item.AUTO_ID)) ? true : false

                                           }).ToList();


                    resp.OUTPUT_DATA = listTeachCourse;
                }

            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<RESPONSE__MODEL> GetListSomeTeacherAfterLogin(CONTAINER_MODEL dto)
        {

            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                                                     .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                                                     .Select(p => p.TEACH_COURSE_ID).ToListAsync();
                    if (dto.SEARCH_TEACHER_MODEL.TEACH_GENDER == null)
                    {
                        var gender = new SEARCH_TEACHER_MODEL();
                        gender.TEACH_GENDER = new List<string> { "1", "2" };

                        dto.SEARCH_TEACHER_MODEL.TEACH_GENDER = gender.TEACH_GENDER;

                    }
                    if (dto.SEARCH_TEACHER_MODEL.TEACHING_TYPE == null)
                    {
                        var TEACHING_TYPE = new SEARCH_TEACHER_MODEL();
                        TEACHING_TYPE.TEACHING_TYPE = new List<int?> { 1, 2 };

                        dto.SEARCH_TEACHER_MODEL.TEACHING_TYPE = TEACHING_TYPE.TEACHING_TYPE;

                    }
                    if (dto.SEARCH_TEACHER_MODEL.STUDENT_LEVEL == null)
                    {
                        var STUDENT_LEVEL = new SEARCH_TEACHER_MODEL();
                        STUDENT_LEVEL.STUDENT_LEVEL = new List<int?> { 1, 2, 3, 4 };

                        dto.SEARCH_TEACHER_MODEL.STUDENT_LEVEL = STUDENT_LEVEL.STUDENT_LEVEL;

                    }

                    var listStu = await (from a in ctx.MEMBER_TEACH_COURSE
                                         join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                                         join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                                         join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                                         join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                                         where
                                         dto.SEARCH_TEACHER_MODEL.TEACH_GENDER.Contains(c.SEX)
                                         //&& dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Contains(d.NAME)
                                         && dto.SEARCH_TEACHER_MODEL.TEACHING_TYPE.Contains(c.TEACHING_TYPE)
                                         && dto.SEARCH_TEACHER_MODEL.STUDENT_LEVEL.Contains(c.STUDENT_LEVEL)
                                         && (b.ROLE_ID == 1 && f.STATUS == 2)
                                         select new CUSTOM_MEMBERS
                                         {
                                             AUTO_ID = c.AUTO_ID,
                                             COURSE_ID = d.AUTO_ID,
                                             PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                                             //STATUS = a.STATUS,
                                             FULLNAME = c.FULLNAME ?? "ไม่ระบุ",
                                             SEX = c.SEX == "1" ? "ชาย" : "หญิง",
                                             AGE = c.AGE,
                                             LOCATION = c.LOCATION ?? "ไม่ระบุ",
                                             MOBILE = c.MOBILE ?? "ไม่ระบุ",
                                             USER_NAME = f.USER_NAME ?? "ไม่ระบุ",
                                             COURSE = d.NAME ?? "ไม่ระบุ",
                                             ABOUT = c.ABOUT ?? "ไม่ระบุ",
                                             //LIST_STUDENT_COMMENT = a.MEMBER_REGIS_COURSE_COMMENT.Select(o => o.COMMENT).ToList(),
                                             REGIS_COURSE_ID = a.AUTO_ID,
                                             CATEGORY = "ไม่ระบุ",
                                             REGISTER_STATUS = (memberRegisCourse.Contains(a.AUTO_ID)) ? true : false
                                         }).ToListAsync();

                    resp.OUTPUT_DATA = listStu;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }
        public async Task<RESPONSE__MODEL> GetListSomeCourseAfterLogin(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();

                    var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                                                     .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                                                     .Select(p => p.TEACH_COURSE_ID).ToListAsync();

                    var listAllTeachCourse = await ctx.MEMBER_TEACH_COURSE
                                                    .Include(x => x.MEMBER_ROLE.MEMBERS)
                                                    .Include(x => x.COURSES)
                                                     // .Where(o => dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Contains(o.DESCRIPTION))
                                                     .ToListAsync();


                    var listTeachCourse = (from item in listAllTeachCourse
                                           select new CUSTOM_MEMBERS
                                           {
                                               AUTO_ID = item.AUTO_ID,
                                               COURSE_BANNER = item.COURSES.BANNER_URL,
                                               COURSE = item.COURSES.NAME,
                                               COURSE_ID = item.COURSES.AUTO_ID,
                                               FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME,
                                               AGE = item.MEMBER_ROLE.MEMBERS.AGE,
                                               ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT,
                                               REGISTER_STATUS = (memberRegisCourse.Contains(item.AUTO_ID)) ? true : false

                                           }).ToList();


                    resp.OUTPUT_DATA = listTeachCourse;
                }

            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        #endregion
    }
}
