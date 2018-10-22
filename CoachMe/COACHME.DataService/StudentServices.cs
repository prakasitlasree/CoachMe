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
        public async Task<RESPONSE__MODEL> MatchTeacher(SEARCH_TEACHER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();

                    var teachRoleId = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.TEACHER_ROLE_ID).FirstOrDefaultAsync();
                    var match = new MEMBER_MATCHING();
                    match.STUDENT_ROLE_ID = memberRole.AUTO_ID;
                    match.TEACHER_ROLE_ID = teachRoleId.AUTO_ID;
                    match.CREATED_BY = dto.MEMBERS.FULLNAME;
                    match.CREATED_DATE = DateTime.Now;
                    match.UPDATED_BY = dto.MEMBERS.FULLNAME;
                    match.UPDATED_DATE = DateTime.Now;

                    ctx.MEMBER_MATCHING.Add(match);
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
        private string GetTeachtingTypeValue(int? id)
        {
            string result = string.Empty;
            if (id == 1)
            {
                result = "เรียนเดี่ยว";
            }
            else if (id == 2)
            {
                result = "เรียนกลุ่ม";
            }
            else if (id == 3)
            {
                result = "เรียนเดี่ยว,เรียนกลุ่ม";
            }
            //< option value = 1 > เรียนเดี่ยว </ option >
            //< option value = 2 > เรียนกลุ่ม </ option >
            //< option value = 3 > เรียนเดี่ยว,เรียนกลุ่ม </ option >
            return result;
        }

        private string GetStudentLevelValue(int? id)
        {
            string result = string.Empty;
            if (id == 1)
            {
                result = "เริ่มต้น";
            }
            else if (id == 2)
            {
                result = "ปานกลาง";
            }
            else if (id == 3)
            {
                result = "ขั้นสูง";
            }
            else if (id == 4)
            {
                result = "ทุกระดับผู้เรียน";
            }
            //< option value = 1 > เริ่มต้น </ option >

            //< option value = 2 > ปานกลาง </ option >

            //< option value = 3 > ขั้นสูง </ option >

            //< option value = 4 > ทุกระดับผู้เรียน </ option >
            return result;
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
                                          TEACHING_TYPE = a.TEACHING_TYPE,
                                          STUDENT_LEVEL = a.STUDENT_LEVEL,
                                          ABOUT_IMG_1 = a.ABOUT_IMG_URL1,
                                          ABOUT_IMG_2 = a.ABOUT_IMG_URL2,
                                          ABOUT_IMG_3 = a.ABOUT_IMG_URL3,
                                          ABOUT_IMG_4 = a.ABOUT_IMG_URL4,

                                      }).ToListAsync();

                    foreach (var item in list)
                    {
                        item.LIST_MEMBER_CETEGORY = listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID).ToList();
                        if (item.TEACHING_TYPE != null)
                        {
                            item.TEACHING_TYPE_NAME = GetTeachtingTypeValue(item.TEACHING_TYPE);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                        if (item.STUDENT_LEVEL != null)
                        {
                            item.STUDENT_LEVEL_NAME = GetStudentLevelValue(item.STUDENT_LEVEL);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
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
                                                    .Where(o => o.MEMBER_ROLE.MEMBERS.MEMBER_LOGON.FirstOrDefault().STATUS == 2).ToListAsync();
                                                   

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
                    if (dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault() > 0 && (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == 0 || dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == null))
                    {
                        var listAmphur = await ctx.AMPHUR
                                                  .Where(o => o.PROVINCE_ID == dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault())
                                                  .Select(o => (int?)o.AMPHUR_ID).ToListAsync();
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = listAmphur;
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();


                    }
                    else if (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID != null )
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }
                    if ((dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID == null || dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == 0) && dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault() == 0)
                    {
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = new List<int?> { 0 };
                        listAllTeachCourse = listAllTeachCourse.Where(x => !dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }
                    if(dto.LIST_COURSE.FirstOrDefault() > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => x.COURSE_ID == dto.LIST_COURSE.FirstOrDefault()).ToList();
                    }


                    #endregion

                    var listTeachCourse = (from item in listAllTeachCourse
                                           select new CUSTOM_MEMBERS
                                           {
                                               COURSE_BANNER = item.COURSES.BANNER_URL,
                                               COURSE_ID = item.COURSES.AUTO_ID,
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
                    if (dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault() > 0 && (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == 0 || dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == null))
                    {
                        var listAmphur = await ctx.AMPHUR
                                                  .Where(o => o.PROVINCE_ID == dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault())
                                                  .Select(o => (int?)o.AMPHUR_ID).ToListAsync();
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = listAmphur;

                    }
                    if (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID == null)
                    {
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = new List<int?> { 0 };
                    }
                    var listTeacher = new List<CUSTOM_MEMBERS>();
                    var listMemberCate = await ctx.MEMBER_CATEGORY.Include(x=>x.CATEGORY).ToListAsync();
                    var listCourse = await ctx.MEMBER_TEACH_COURSE.Include(x => x.COURSES).ToListAsync();

                    listTeacher = await (from a in ctx.MEMBERS
                                         join b in ctx.MEMBER_ROLE on a.AUTO_ID equals b.MEMBER_ID
                                         join c in ctx.MEMBER_LOGON on a.AUTO_ID equals c.MEMBER_ID
                                         where b.ROLE_ID == 1 && c.STATUS == 2
                                          && (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() != 0 ? dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(a.AMPHUR_ID) : !dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(a.AMPHUR_ID))

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
                                             TEACHING_TYPE = a.TEACHING_TYPE,
                                             STUDENT_LEVEL = a.STUDENT_LEVEL,
                                             ABOUT_IMG_1 = a.ABOUT_IMG_URL1,
                                             ABOUT_IMG_2 = a.ABOUT_IMG_URL2,
                                             ABOUT_IMG_3 = a.ABOUT_IMG_URL3,
                                             ABOUT_IMG_4 = a.ABOUT_IMG_URL4,
                                         }).ToListAsync();

                    foreach (var item in listTeacher)
                    {
                        item.LIST_MEMBER_CETEGORY = listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID).ToList();
                        item.LIST_MEMBER_TEACH_COURSE = listCourse.Where(x => x.MEMBER_ROLE_ID == item.MEMBER_ROLE_ID).ToList();
                        if (item.TEACHING_TYPE != null)
                        {
                            item.TEACHING_TYPE_NAME = GetTeachtingTypeValue(item.TEACHING_TYPE);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                        if (item.STUDENT_LEVEL != null)
                        {
                            item.STUDENT_LEVEL_NAME = GetStudentLevelValue(item.STUDENT_LEVEL);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                    }

                    #region === Advance Search ====

                    if (dto.LIST_CATE.FirstOrDefault() >0)
                    {
                        int catId = dto.LIST_CATE.FirstOrDefault();
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
                #region old code 
                //using (var ctx = new COACH_MEEntities())
                //{
                //    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                //    var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                //                                     .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                //                                     .Select(p => p.TEACH_COURSE_ID).ToListAsync();

                //    var listTeacher = await (from a in ctx.MEMBER_TEACH_COURSE
                //                             join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                //                             join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                //                             join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                //                             join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                //                             where b.ROLE_ID == 1 && f.STATUS == 2 /*&& !memberRegisCourse.Contains(a.AUTO_ID)*/
                //                             select new CUSTOM_MEMBERS
                //                             { 
                //                                 ROLE = b.ROLE_ID.ToString(),
                //                                 AUTO_ID = c.AUTO_ID,
                //                                 PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                //                                 COURSE_ID = d.AUTO_ID,
                //                                 MEMBER_ROLE_ID = b.AUTO_ID,
                //                                 FULLNAME = c.FULLNAME ?? "ไม่ระบุ",
                //                                 SEX = c.SEX == "1" ? "ชาย" : "หญิง",
                //                                 AGE = c.AGE,
                //                                 LOCATION = c.LOCATION ?? "ไม่ระบุ",
                //                                 MOBILE = c.MOBILE ?? "ไม่ระบุ",
                //                                 USER_NAME = f.USER_NAME ?? "ไม่ระบุ",
                //                                 COURSE = d.NAME ?? "ไม่ระบุ",
                //                                 ABOUT = c.ABOUT ?? "ไม่ระบุ",
                //                                 REGIS_COURSE_ID = a.AUTO_ID,
                //                                 CATEGORY = "ไม่ระบุ",
                //                                 REGISTER_STATUS = (memberRegisCourse.Contains(a.AUTO_ID)) ? true : false
                //                             }).OrderByDescending(o => o.REGISTER_STATUS).ToListAsync();

                //    resp.OUTPUT_DATA = listTeacher;
                //}
                #endregion
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    var listMemberCate = await ctx.MEMBER_CATEGORY.Include(x => x.CATEGORY).ToListAsync();
                    var listMatching = await ctx.MEMBER_MATCHING.Where(x => x.STUDENT_ROLE_ID == memberRole.AUTO_ID).ToListAsync();
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
                                          TEACHING_TYPE = a.TEACHING_TYPE,
                                          STUDENT_LEVEL = a.STUDENT_LEVEL,
                                          ABOUT_IMG_1 = a.ABOUT_IMG_URL1,
                                          ABOUT_IMG_2 = a.ABOUT_IMG_URL2,
                                          ABOUT_IMG_3 = a.ABOUT_IMG_URL3,
                                          ABOUT_IMG_4 = a.ABOUT_IMG_URL4,
                                      }).ToListAsync();

                    foreach (var item in list)
                    {
                        item.LIST_MEMBER_CETEGORY = listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID).ToList();
                        if (item.TEACHING_TYPE != null)
                        {
                            item.TEACHING_TYPE_NAME = GetTeachtingTypeValue(item.TEACHING_TYPE);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                        if (item.STUDENT_LEVEL != null)
                        {
                            item.STUDENT_LEVEL_NAME = GetStudentLevelValue(item.STUDENT_LEVEL);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }

                        var match = listMatching.Where(x => x.TEACHER_ROLE_ID == item.MEMBER_ROLE_ID).ToList();
                        if (match.Count > 0)
                        {
                            item.REGISTER_STATUS = true;
                        }
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
                                                    .Where(o => o.MEMBER_ROLE.MEMBERS.MEMBER_LOGON.FirstOrDefault().STATUS == 2).ToListAsync();
                                                   

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
                    #region ===========PROVINCE & AMPHUR SEARCH===========
                    if (dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault() > 0 && (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == 0 || dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == null))
                    {
                        var listAmphur = await ctx.AMPHUR
                                                  .Where(o => o.PROVINCE_ID == dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault())
                                                  .Select(o => (int?)o.AMPHUR_ID).ToListAsync();
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = listAmphur;

                    }
                    if (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID == null)
                    {
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = new List<int?> { 0 };
                    }
                    #endregion

                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    var listMemberCate = await ctx.MEMBER_CATEGORY.Include(x => x.CATEGORY).ToListAsync();
                    var listMatching = await ctx.MEMBER_MATCHING.Where(x => x.STUDENT_ROLE_ID == memberRole.AUTO_ID).ToListAsync();
                    var list = await (from a in ctx.MEMBERS
                                      join b in ctx.MEMBER_ROLE on a.AUTO_ID equals b.MEMBER_ID
                                      join c in ctx.MEMBER_LOGON on a.AUTO_ID equals c.MEMBER_ID
                                      where b.ROLE_ID == 1 && c.STATUS == 2
                                       && (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() != 0 ? dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(a.AMPHUR_ID) : !dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(a.AMPHUR_ID))
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
                                          TEACHING_TYPE = a.TEACHING_TYPE,
                                          STUDENT_LEVEL = a.STUDENT_LEVEL,
                                          ABOUT_IMG_1 = a.ABOUT_IMG_URL1,
                                          ABOUT_IMG_2 = a.ABOUT_IMG_URL2,
                                          ABOUT_IMG_3 = a.ABOUT_IMG_URL3,
                                          ABOUT_IMG_4 = a.ABOUT_IMG_URL4,

                                      }).ToListAsync();

                    foreach (var item in list)
                    {
                        item.LIST_MEMBER_CETEGORY = listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID).ToList();
                        if (item.TEACHING_TYPE != null)
                        {
                            item.TEACHING_TYPE_NAME = GetTeachtingTypeValue(item.TEACHING_TYPE);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                        if (item.STUDENT_LEVEL != null)
                        {
                            item.STUDENT_LEVEL_NAME = GetStudentLevelValue(item.STUDENT_LEVEL);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                        var match = listMatching.Where(x => x.TEACHER_ROLE_ID == item.MEMBER_ROLE_ID).ToList();
                        if (match.Count > 0)
                        {
                            item.REGISTER_STATUS = true;
                        }
                    }
                    #region === Advance Search ====

                   

                    if (dto.LIST_CATE.FirstOrDefault() > 0)
                    {
                        int catId = dto.LIST_CATE.FirstOrDefault();
                        list = list.Where(x => x.LIST_MEMBER_CETEGORY != null).Where(o => o.LIST_MEMBER_CETEGORY.Any(s => s.CATEGORY_ID == catId)).ToList();
                    }
                    #endregion
                   

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
        public async Task<RESPONSE__MODEL> GetListSomeCourseAfterLogin(CONTAINER_MODEL dto)
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

                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();

                    var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                                                     .Include(x => x.MEMBER_ROLE.MEMBERS)
                                                     .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                                                     .Select(p => p.TEACH_COURSE_ID).ToListAsync();


                    #region === Advance Search ====

                    if (dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault() > 0 && (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == 0 || dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == null))
                    {
                        var listAmphur = await ctx.AMPHUR
                                                  .Where(o => o.PROVINCE_ID == dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault())
                                                  .Select(o => (int?)o.AMPHUR_ID).ToListAsync();
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = listAmphur;
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();


                    }
                    else if (dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID != null)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }
                    if ((dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID == null || dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.FirstOrDefault() == 0) && dto.SEARCH_TEACHER_MODEL.LIST_PROVINCE_ID.FirstOrDefault() == 0)
                    {
                        dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID = new List<int?> { 0 };
                        listAllTeachCourse = listAllTeachCourse.Where(x => !dto.SEARCH_TEACHER_MODEL.LIST_AMPHUR_ID.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }
                    if (dto.LIST_COURSE.FirstOrDefault() > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => x.COURSE_ID == dto.LIST_COURSE.FirstOrDefault()).ToList();
                    }
                    #endregion

                    var listTeachCourse = (from item in listAllTeachCourse
                                           select new CUSTOM_MEMBERS
                                           {
                                               AUTO_ID = item.AUTO_ID,
                                               COURSE_BANNER = item.COURSES.BANNER_URL,
                                               COURSE_ID = item.COURSES.AUTO_ID,
                                               COURSE = item.COURSES.NAME,
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
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }
        #endregion

    }
}


