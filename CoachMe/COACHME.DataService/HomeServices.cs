using COACHME.DAL;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.DATASERVICE
{
    public class HomeServices
    {
        private string GetTeachingTypeName(int? id)
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

            return result;
        }

        private string GetTeachinLevelName(int? id)
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
            else if (id == null)
            {
                result = "ไม่ระบุ";
            }
            //< option value = 1 > เริ่มต้น </ option >

            //< option value = 2 > ปานกลาง </ option >

            //< option value = 3 > ขั้นสูง </ option >

            //< option value = 4 > ทุกระดับผู้เรียน </ option >
            return result;
        }

        public HOME_MODEL GetMemberLogin(MEMBERS dto)
        {
            HOME_MODEL model = new HOME_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var data = ctx.MEMBERS
                        .Include(o => o.MEMBER_ROLE)
                        .Include(o => o.MEMBER_LOGON).Where(o => o.AUTO_ID == dto.AUTO_ID).FirstOrDefault();
                    model.MEMBER = new MEMBER();
                    model.MEMBER.FULLNAME = data.FULLNAME;
                    model.MEMBER.PROFILE_IMG_URL = data.PROFILE_IMG_URL;
                    model.MEMBER.ROLE_ID = data.MEMBER_ROLE.FirstOrDefault().ROLE_ID;
                    model.MEMBER.USER_NAME = data.MEMBER_LOGON.FirstOrDefault().USER_NAME;

                }
            }
            catch (Exception ex)
            {

            }
            return model;
        }

        public async Task<RESPONSE__MODEL> SelectTeacher(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBER_AUTO_ID)
                                                          .Include(o => o.MEMBERS).FirstOrDefaultAsync();

                    var teachRoleId = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.TEACHER_AUTO_ID).FirstOrDefaultAsync();
                    var match = new MEMBER_MATCHING();
                    match.STUDENT_ROLE_ID = memberRole.AUTO_ID;
                    match.TEACHER_ROLE_ID = teachRoleId.AUTO_ID;
                    match.CREATED_BY = memberRole.MEMBERS.FULLNAME;
                    match.CREATED_DATE = DateTime.Now;
                    match.UPDATED_BY = memberRole.MEMBERS.FULLNAME;
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

        public async Task<RESPONSE__MODEL> CancelTeacher(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBER_AUTO_ID)
                                                          .Include(o => o.MEMBERS).FirstOrDefaultAsync();

                    var teachRoleId = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.TEACHER_AUTO_ID).FirstOrDefaultAsync();

                    var obj = await ctx.MEMBER_MATCHING.Where(o => o.STUDENT_ROLE_ID == memberRole.AUTO_ID
                                                          && o.TEACHER_ROLE_ID == teachRoleId.AUTO_ID).FirstOrDefaultAsync();
                    ctx.MEMBER_MATCHING.Remove(obj);
                  
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

        public async Task<RESPONSE__MODEL> SelectCourse(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBER_AUTO_ID).FirstOrDefaultAsync();
                    var teachCourse = await ctx.MEMBER_TEACH_COURSE.Where(x => x.COURSE_ID == dto.COURSE_ID).FirstOrDefaultAsync();
                    var course = await ctx.COURSES.Where(x => x.AUTO_ID == dto.COURSE_ID).FirstOrDefaultAsync();
                    var regisCourse = new MEMBER_REGIS_COURSE();
                    regisCourse.REGIS_MEMBER_ROLE_ID = memberRole.AUTO_ID;
                    regisCourse.TEACH_COURSE_ID = teachCourse.AUTO_ID;
                    regisCourse.DESCRIPTION = course.NAME;

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

        public async Task<RESPONSE__MODEL> GetListCourseBeforeLogin(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var listAllTeachCourse = await ctx.MEMBER_TEACH_COURSE
                                                    .Include(x => x.COURSES)
                                                    .Include(x => x.MEMBER_ROLE.MEMBERS)
                                                    .Include(x => x.MEMBER_ROLE.MEMBERS.MEMBER_LOGON)
                                                    .Where(o => o.MEMBER_ROLE.ROLE_ID == 1 && o.MEMBER_ROLE.MEMBERS.MEMBER_LOGON.FirstOrDefault().STATUS == 2).ToListAsync();
                    var listMemberPackage = await ctx.MEMBER_PACKAGE.ToListAsync();

                    #region === Advance Search ====
                    var listAmphur = new List<int?>();
                    if (dto.PROVINCE_ID > 0 && dto.AMPHUR_ID == 0)
                    {
                        listAmphur = await ctx.AMPHUR
                                                  .Where(o => o.PROVINCE_ID == dto.PROVINCE_ID)
                                                  .Select(o => (int?)o.AMPHUR_ID).ToListAsync();

                        listAllTeachCourse = listAllTeachCourse.Where(x => listAmphur.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();


                    }
                    else if (listAmphur.Count > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => listAmphur.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }
                    if ((listAmphur.Count == 0 || listAmphur.FirstOrDefault() == 0) && dto.PROVINCE_ID == 0)
                    {
                        listAmphur.Add(0);
                        listAllTeachCourse = listAllTeachCourse.Where(x => !listAmphur.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }

                    if (dto.CATEGORY_ID > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => x.COURSE_ID == dto.CATEGORY_ID).ToList();
                    }


                    #endregion

                    var listTeachCourse2 = (from item in listAllTeachCourse
                                            select new LIST_MEMBERS
                                            {
                                                COURSE_BANNER = item.COURSES.BANNER_URL,
                                                COURSE_ID = item.COURSES.AUTO_ID,
                                                COURSE = item.COURSES.NAME,
                                                FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME,
                                                AGE = item.MEMBER_ROLE.MEMBERS.AGE,
                                                ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT,
                                                REGISTER_STATUS = false

                                            }).ToList();

                    var listTeachCourse = new List<LIST_MEMBERS>();

                    foreach (var item in listAllTeachCourse)
                    {
                        var memberPackage = listMemberPackage.Where(x => x.MEMBER_ID == item.MEMBER_ROLE.MEMBERS.AUTO_ID
                                                                         && x.STATUS == "ACTIVE"
                                                                         && x.EXPIRE_DATE > DateTime.Now).ToList();
                        if (memberPackage.Count > 0)
                        {
                            var memberCourse = new LIST_MEMBERS();
                            memberCourse.COURSE_BANNER = item.COURSES.BANNER_URL;
                            memberCourse.COURSE = item.COURSES.NAME;
                            memberCourse.FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME;
                            memberCourse.AGE = item.MEMBER_ROLE.MEMBERS.AGE;
                            memberCourse.ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT;
                            memberCourse.AUTO_ID = item.MEMBER_ROLE.MEMBERS.AUTO_ID;
                            memberCourse.REGISTER_STATUS = false;
                            memberCourse.VERIFY = true;
                            listTeachCourse.Add(memberCourse);
                        }
                        else
                        {
                            var memberCourse = new LIST_MEMBERS();
                            memberCourse.COURSE_BANNER = item.COURSES.BANNER_URL;
                            memberCourse.COURSE = item.COURSES.NAME;
                            memberCourse.FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME;
                            memberCourse.AGE = item.MEMBER_ROLE.MEMBERS.AGE;
                            memberCourse.ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT;
                            memberCourse.AUTO_ID = item.MEMBER_ROLE.MEMBERS.AUTO_ID;
                            memberCourse.REGISTER_STATUS = false;
                            memberCourse.VERIFY = false;
                            listTeachCourse.Add(memberCourse);
                        }
                    }

                    dto.PAGE_COUNT = Math.Ceiling(listTeachCourse.Count() / Convert.ToDecimal(dto.PAGE_SIZE));
                    listTeachCourse = listTeachCourse.OrderByDescending(x => x.VERIFY).ToList();
                    listTeachCourse = listTeachCourse.Skip(dto.PAGE_SIZE * (dto.PAGE_NUMBER - 1)).Take(dto.PAGE_SIZE).ToList();

                    dto.LIST_MEMBERS = listTeachCourse;
                    resp.STATUS = true;
                    resp.OUTPUT_DATA = dto;
                }

            }
            catch (Exception ex)
            {

            }
            return resp;

        }

        public async Task<RESPONSE__MODEL> GetListCourseAfterLogin(HOME_MODEL dto)
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
                                                    .Where(o => o.MEMBER_ROLE.ROLE_ID == 1 && o.MEMBER_ROLE.MEMBERS.MEMBER_LOGON.FirstOrDefault().STATUS == 2).ToListAsync();


                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBER_AUTO_ID).FirstOrDefaultAsync();

                    var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                                                     .Include(x => x.MEMBER_ROLE.MEMBERS)
                                                     .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                                                     .Select(p => p.TEACH_COURSE_ID).ToListAsync();




                    #region === Advance Search ====
                    var listAmphur = new List<int?>();
                    if (dto.PROVINCE_ID > 0 && dto.AMPHUR_ID == 0)
                    {
                        listAmphur = await ctx.AMPHUR
                                                  .Where(o => o.PROVINCE_ID == dto.PROVINCE_ID)
                                                  .Select(o => (int?)o.AMPHUR_ID).ToListAsync();

                        listAllTeachCourse = listAllTeachCourse.Where(x => listAmphur.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();


                    }
                    else if (listAmphur.Count > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => listAmphur.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }
                    if ((listAmphur.Count == 0 || listAmphur.FirstOrDefault() == 0) && dto.PROVINCE_ID == 0)
                    {
                        listAmphur.Add(0);
                        listAllTeachCourse = listAllTeachCourse.Where(x => !listAmphur.Contains(x.MEMBER_ROLE.MEMBERS.AMPHUR_ID)).ToList();
                    }

                    if (dto.CATEGORY_ID > 0)
                    {
                        listAllTeachCourse = listAllTeachCourse.Where(x => x.COURSE_ID == dto.CATEGORY_ID).ToList();
                    }


                    #endregion


                    var listTeachCourse = (from item in listAllTeachCourse
                                           select new LIST_MEMBERS
                                           {
                                               AUTO_ID = item.AUTO_ID,
                                               COURSE_BANNER = item.COURSES.BANNER_URL,
                                               COURSE_ID = item.COURSES.AUTO_ID,
                                               COURSE = item.COURSES.NAME,
                                               FULLNAME = item.MEMBER_ROLE.MEMBERS.FULLNAME,
                                               AGE = item.MEMBER_ROLE.MEMBERS.AGE,
                                               ABOUT = item.MEMBER_ROLE.MEMBERS.ABOUT ?? "ไม่ระบุ",
                                               REGISTER_STATUS = (memberRegisCourse.Contains(item.AUTO_ID)) ? true : false

                                           }).ToList();


                    dto.PAGE_COUNT = Math.Ceiling(listTeachCourse.Count() / Convert.ToDecimal(dto.PAGE_SIZE));
                    listTeachCourse = listTeachCourse.OrderByDescending(x => x.VERIFY).ToList();
                    listTeachCourse = listTeachCourse.Skip(dto.PAGE_SIZE * (dto.PAGE_NUMBER - 1)).Take(dto.PAGE_SIZE).ToList();

                    dto.LIST_MEMBERS = listTeachCourse;
                    resp.STATUS = true;
                    resp.OUTPUT_DATA = dto;


                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> GetListTeacherAfterLogin(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    #region ===========PROVINCE & AMPHUR SEARCH===========
                    var listAmphur = new List<int?>();
                    if (dto.PROVINCE_ID > 0 && dto.AMPHUR_ID == 0)
                    {
                        listAmphur = await ctx.AMPHUR
                                              .Where(o => o.PROVINCE_ID == dto.PROVINCE_ID)
                                              .Select(o => (int?)o.AMPHUR_ID).ToListAsync();

                    }
                    if (dto.PROVINCE_ID == 0 && dto.AMPHUR_ID == 0)
                    {
                        listAmphur.Add(0);
                    }
                    #endregion
                    var listMemberPackage = await ctx.MEMBER_PACKAGE.ToListAsync();
                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBER_AUTO_ID).FirstOrDefaultAsync();
                    var listMemberCate = await ctx.MEMBER_CATEGORY.Include(x => x.CATEGORY).ToListAsync();
                    var listMatching = await ctx.MEMBER_MATCHING.Where(x => x.STUDENT_ROLE_ID == memberRole.AUTO_ID).ToListAsync();
                    var list = await (from a in ctx.MEMBERS
                                      join b in ctx.MEMBER_ROLE on a.AUTO_ID equals b.MEMBER_ID
                                      join c in ctx.MEMBER_LOGON on a.AUTO_ID equals c.MEMBER_ID
                                      where b.ROLE_ID == 1 && c.STATUS == 2
                                       && (listAmphur.FirstOrDefault() != 0 ? listAmphur.Contains(a.AMPHUR_ID) : !listAmphur.Contains(a.AMPHUR_ID))
                                      select new
                                      LIST_MEMBERS
                                      {
                                          ROLE = b.ROLE_ID.ToString(),
                                          AUTO_ID = a.AUTO_ID,
                                          PROFILE_IMG_URL = a.PROFILE_IMG_URL,
                                          PROFILE_IMG_URL_FULL = a.PROFILE_IMG_URL_FULL,
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
                    list = list.OrderByDescending(x => x.VERIFY).ToList();
                    foreach (var item in list)
                    {
                        var memberPackage = listMemberPackage.Where(x => x.MEMBER_ID == item.AUTO_ID
                                                                          && x.STATUS == "ACTIVE"
                                                                          && x.EXPIRE_DATE > DateTime.Now).ToList();
                        if (memberPackage.Count > 0)
                        {
                            item.VERIFY = true;
                        }
                        else
                        {
                            item.VERIFY = false;
                        }
                        item.LIST_MEMBER_CETEGORY = (from p in listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID)
                                                     select new DATA_MEMBER_CATEGORY
                                                     {
                                                         AUTO_ID = p.AUTO_ID,
                                                         NAME = p.NAME,
                                                         CATEGORY_ID = p.CATEGORY_ID

                                                     }).ToList();
                        if (item.TEACHING_TYPE != null)
                        {
                            item.TEACHING_TYPE_NAME = GetTeachingTypeName(item.TEACHING_TYPE);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                        if (item.STUDENT_LEVEL != null)
                        {
                            item.STUDENT_LEVEL_NAME = GetTeachinLevelName(item.STUDENT_LEVEL);
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



                    if (dto.CATEGORY_ID > 0)
                    {

                        int catId = dto.CATEGORY_ID;
                        list = list.Where(x => x.LIST_MEMBER_CETEGORY != null).Where(o => o.LIST_MEMBER_CETEGORY.Any(s => s.CATEGORY_ID == catId)).ToList();

                    }
                    #endregion

                    dto.PAGE_COUNT = Math.Ceiling(list.Count() / Convert.ToDecimal(dto.PAGE_SIZE));
                    list = list.OrderByDescending(x => x.VERIFY).ToList();
                    list = list.Skip(dto.PAGE_SIZE * (dto.PAGE_NUMBER - 1)).Take(dto.PAGE_SIZE).ToList();
                    dto.LIST_MEMBERS = list;
                    resp.STATUS = true;
                    resp.OUTPUT_DATA = dto;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;

            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> GetListTeacherBeforeLogin(HOME_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listAmphur = new List<int?>();
                    if (dto.PROVINCE_ID > 0 && dto.AMPHUR_ID == 0)
                    {
                        listAmphur = await ctx.AMPHUR
                                              .Where(o => o.PROVINCE_ID == dto.PROVINCE_ID)
                                              .Select(o => (int?)o.AMPHUR_ID).ToListAsync();

                    }
                    if (dto.PROVINCE_ID == 0 && dto.AMPHUR_ID == 0)
                    {
                        listAmphur.Add(0);
                    }

                    var listTeacher = new List<LIST_MEMBERS>();
                    var listMemberCate = await ctx.MEMBER_CATEGORY.Include(x => x.CATEGORY).ToListAsync();
                    var listCourse = await ctx.MEMBER_TEACH_COURSE.Include(x => x.COURSES).ToListAsync();
                    var listMemberPackage = await ctx.MEMBER_PACKAGE.ToListAsync();

                    listTeacher = await (from a in ctx.MEMBERS
                                         join b in ctx.MEMBER_ROLE on a.AUTO_ID equals b.MEMBER_ID
                                         join c in ctx.MEMBER_LOGON on a.AUTO_ID equals c.MEMBER_ID
                                         where b.ROLE_ID == 1 && c.STATUS == 2
                                          && (listAmphur.FirstOrDefault() != 0 ? listAmphur.Contains(a.AMPHUR_ID) : !listAmphur.Contains(a.AMPHUR_ID))
                                         select new
                                         LIST_MEMBERS
                                         {
                                             ROLE = b.ROLE_ID.ToString(),
                                             AUTO_ID = a.AUTO_ID,
                                             PROFILE_IMG_URL = a.PROFILE_IMG_URL,
                                             PROFILE_IMG_URL_FULL = a.PROFILE_IMG_URL_FULL,
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
                        var memberPackage = listMemberPackage.Where(x => x.MEMBER_ID == item.AUTO_ID
                                                                          && x.STATUS == "ACTIVE"
                                                                          && x.EXPIRE_DATE > DateTime.Now).ToList();
                        item.LIST_MEMBER_CETEGORY = (from p in listMemberCate.Where(x => x.MEMBER_ID == item.AUTO_ID)
                                                     select new DATA_MEMBER_CATEGORY
                                                     {
                                                         AUTO_ID = p.AUTO_ID,
                                                         NAME = p.NAME,
                                                         CATEGORY_ID = p.CATEGORY_ID

                                                     }).ToList();

                        //item.LIST_MEMBER_TEACH_COURSE = listCourse.Where(x => x.MEMBER_ROLE_ID == item.MEMBER_ROLE_ID).ToList();
                        if (memberPackage.Count > 0)
                        {
                            item.VERIFY = true;
                        }
                        else
                        {
                            item.VERIFY = false;
                        }
                        if (item.TEACHING_TYPE != null)
                        {
                            item.TEACHING_TYPE_NAME = GetTeachingTypeName(item.TEACHING_TYPE);
                        }
                        else
                        {
                            item.TEACHING_TYPE_NAME = "ไม่ระบุ";
                        }
                        if (item.STUDENT_LEVEL != null)
                        {
                            item.STUDENT_LEVEL_NAME = GetTeachinLevelName(item.STUDENT_LEVEL);
                        }
                        else
                        {
                            item.STUDENT_LEVEL_NAME = "ไม่ระบุ";
                        }
                    }

                    #region === Advance Search ====
                    if (dto.CATEGORY_ID > 0)
                    {

                        int catId = dto.CATEGORY_ID;
                        listTeacher = listTeacher.Where(x => x.LIST_MEMBER_CETEGORY != null).Where(o => o.LIST_MEMBER_CETEGORY.Any(s => s.CATEGORY_ID == catId)).ToList();

                    }
                    #endregion

                    dto.PAGE_COUNT = Math.Ceiling(listTeacher.Count() / Convert.ToDecimal(dto.PAGE_SIZE));
                    listTeacher = listTeacher.OrderByDescending(x => x.VERIFY).ToList();
                    listTeacher = listTeacher.Skip(dto.PAGE_SIZE * (dto.PAGE_NUMBER - 1)).Take(dto.PAGE_SIZE).ToList();



                    dto.LIST_MEMBERS = listTeacher;
                    resp.OUTPUT_DATA = dto;
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

        public async Task<RESPONSE__MODEL> FacebookLogin(FACEBOOK_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var checkMember = await ctx.MEMBER_LOGON
                                         .Include(o => o.MEMBERS)
                                         .Where(o => o.PASSWORD == dto.ID && o.TOKEN_HASH == "FACEBOOK").FirstOrDefaultAsync();

                    if (checkMember != null)
                    {
                        var memberObj = ctx.MEMBERS
                                        .Include("MEMBER_ROLE")
                                        .Include("MEMBER_PACKAGE")
                                        .Where(x => x.AUTO_ID == checkMember.MEMBER_ID).FirstOrDefault();

                        #region Activity
                        var activity = new LOGON_ACTIVITY();
                        activity.DATE = DateTime.Now;
                        activity.ACTION = "LOGON";
                        activity.FULLNAME = checkMember.MEMBERS.FULLNAME;
                        activity.USER_NAME = checkMember.USER_NAME;
                        activity.PASSWORD = checkMember.PASSWORD;
                        activity.STATUS = true;
                        ctx.LOGON_ACTIVITY.Add(activity);
                        var act_result = await ctx.SaveChangesAsync();
                        #endregion

                        resp.STATUS = true;
                        resp.OUTPUT_DATA = memberObj;
                    }
                    else
                    {
                        resp.STATUS = false;
                        resp.OUTPUT_DATA = dto;
                    }
                }
            }
            catch (Exception ex)
            {
                resp.STATUS = false;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> FacebookRegisterConfirm(MEMBER dto, string IMAGE_URL)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();

            try
            {
                var member = new MEMBERS();
                using (var ctx = new COACH_MEEntities())
                {
                    #region ==== SET DETAIL ====
                    //1.Master 
                    member.FULLNAME = dto.FIRST_NAME + " " + dto.LAST_NAME;
                    member.NICKNAME = dto.FIRST_NAME;
                    member.FIRST_NAME = dto.FIRST_NAME;
                    member.LAST_NAME = dto.LAST_NAME;
                    member.CREATED_DATE = DateTime.Now;
                    member.CREATED_BY = dto.FULLNAME;
                    member.UPDATED_DATE = DateTime.Now;
                    member.UPDATED_BY = dto.FULLNAME;
                    member.SEX = dto.GENDER;

                    //2. Details 
                    MEMBER_ROLE memberRole = new MEMBER_ROLE();
                    memberRole.ROLE_ID = dto.ROLE_ID;
                    memberRole.CREATED_DATE = DateTime.Now;
                    memberRole.CREATED_BY = dto.FULLNAME;
                    memberRole.UPDATED_DATE = DateTime.Now;
                    memberRole.UPDATED_BY = dto.FULLNAME;

                    //3. Details 
                    MEMBER_LOGON memberLogon = new MEMBER_LOGON();
                    memberLogon.USER_NAME = dto.EMAIL.ToUpper();
                    memberLogon.PASSWORD = dto.USER_NAME;
                    memberLogon.STATUS = 2;
                    memberLogon.TOKEN_HASH = "FACEBOOK";
                    memberLogon.TOKEN_USED = true;
                    memberLogon.TOKEN_EXPIRATION = DateTime.Now.AddHours(3);
                    memberLogon.CREATED_DATE = DateTime.Now;
                    memberLogon.CREATED_BY = dto.FULLNAME;
                    memberLogon.UPDATED_DATE = DateTime.Now;
                    memberLogon.UPDATED_BY = dto.FULLNAME;


                    //4. Add detail to master 
                    member.MEMBER_ROLE.Add(memberRole);
                    member.MEMBER_LOGON.Add(memberLogon);

                    //5. Save master
                    ctx.MEMBERS.Add(member);
                    await ctx.SaveChangesAsync();
                    resp.STATUS = true;
                    #endregion

                    #region ==== DEPLOY PATH ====
                    string myDir = @"C://WebApplication//coachme.asia//Content//images//Profile//";
                    string fullDir = @"C://WebApplication//coachme.asia//Content//images//ProfileFull//";
                    #endregion

                    string path = "";
                    string fullPath = "";

                    myDir += memberLogon.USER_NAME;
                    System.IO.Directory.CreateDirectory(myDir);
                    fullDir += memberLogon.USER_NAME;
                    System.IO.Directory.CreateDirectory(fullDir);

                    using (WebClient webClient = new WebClient())
                    {
                        byte[] data = webClient.DownloadData(IMAGE_URL);

                        using (MemoryStream mem = new MemoryStream(data))
                        {
                            using (var yourImage = Image.FromStream(mem))
                            {
                                var name = "ProfileImage.jpeg";
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-profile-image" + name;
                                //yourImage.Save("C:\\Users\\Rock\\Desktop\\path_to_your_file.jpg", ImageFormat.Jpeg);
                                path = Path.Combine(myDir, fileName);
                                yourImage.Save(path, ImageFormat.Jpeg);

                                fullPath = Path.Combine(fullDir, fileName).Replace("\\", "//");
                                yourImage.Save(fullPath, ImageFormat.Jpeg);

                                Bitmap bimage = new Bitmap(path);
                                resizeImage(bimage, path);
                            }
                        }

                    }

                    var memberProfile = await ctx.MEMBERS.Where(x => x.AUTO_ID == member.AUTO_ID).FirstOrDefaultAsync();
                    int index = path.IndexOf("Content");
                    member.PROFILE_IMG_URL = @"//" + path.Substring(index);
                    member.PROFILE_IMG_URL_FULL = @"//" + fullPath.Substring(index);

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

    }
}
