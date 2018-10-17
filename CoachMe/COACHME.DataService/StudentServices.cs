﻿using COACHME.DAL;
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
        public async Task<RESPONSE__MODEL> FindTeacher(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    if (dto.SEARCH_TEACHER_MODEL.TEACH_GENDER == null)
                    {
                        var gender = new SEARCH_TEACHER_MODEL();
                        gender.TEACH_GENDER = new List<string> { "1", "2" };

                        dto.SEARCH_TEACHER_MODEL.TEACH_GENDER = gender.TEACH_GENDER;

                    }
                    var listStu = await (from a in ctx.MEMBER_TEACH_COURSE
                                         join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                                         join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                                         join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                                         join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                                         where dto.SEARCH_TEACHER_MODEL.TEACH_GENDER.Contains(c.SEX) && dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Contains(d.NAME) && (b.ROLE_ID == 1 && f.STATUS == 2)
                                         select new CUSTOM_MEMBERS
                                         {
                                             AUTO_ID = c.AUTO_ID,
                                             PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                                             //STATUS = a.STATUS,
                                             COURSE_ID = d.AUTO_ID,
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

        public async Task<RESPONSE__MODEL> FindTeacherFromAutoID(CONTAINER_MODEL dto)
        {

            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    //var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                    //                                 .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                    //                                 .Select(p => p.COURSE_ID).ToListAsync();
                    //if(dto.SEARCH_TEACHER_MODEL.TEACH_GENDER == null)
                    //{
                    //    var gender = new SEARCH_TEACHER_MODEL();
                    //    gender.TEACH_GENDER = new List<string> { "1", "2" };
                        
                    //    dto.SEARCH_TEACHER_MODEL.TEACH_GENDER = gender.TEACH_GENDER;
                       
                    //}
                    //var listStu = await (from a in ctx.MEMBER_TEACH_COURSE
                    //                     join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                    //                     join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                    //                     join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                    //                     join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                    //                     where dto.SEARCH_TEACHER_MODEL.TEACH_GENDER.Contains(c.SEX) && dto.SEARCH_TEACHER_MODEL.LIST_COURSE.Contains(d.NAME) && (b.ROLE_ID == 1 && f.STATUS == 2) &&  !memberRegisCourse.Contains(d.AUTO_ID)
                    //                     select new CUSTOM_MEMBERS
                    //                     {
                    //                         AUTO_ID = c.AUTO_ID,
                    //                         COURSE_ID = d.AUTO_ID,
                    //                         PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                    //                         //STATUS = a.STATUS,
                    //                         FULLNAME = c.FULLNAME ?? "ไม่ระบุ",
                    //                         SEX = c.SEX == "1" ? "ชาย" : "หญิง",
                    //                         AGE = c.AGE,
                    //                         LOCATION = c.LOCATION ?? "ไม่ระบุ",
                    //                         MOBILE = c.MOBILE ?? "ไม่ระบุ",
                    //                         USER_NAME = f.USER_NAME ?? "ไม่ระบุ",
                    //                         COURSE = d.NAME ?? "ไม่ระบุ",
                    //                         ABOUT = c.ABOUT ?? "ไม่ระบุ",
                    //                         //LIST_STUDENT_COMMENT = a.MEMBER_REGIS_COURSE_COMMENT.Select(o => o.COMMENT).ToList(),
                    //                         REGIS_COURSE_ID = a.AUTO_ID,
                    //                         CATEGORY = "ไม่ระบุ",
                    //                     }).ToListAsync();

                   // resp.OUTPUT_DATA = listStu;
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

                    var  memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    var teachCourse = await ctx.MEMBER_TEACH_COURSE.Where(x => x.COURSE_ID == dto.COURSE_ID).FirstOrDefaultAsync();
                    var regisCourse = new MEMBER_REGIS_COURSE();
                    regisCourse.REGISTER_ID = memberRole.AUTO_ID;
                    regisCourse.TEACHER_ID = teachCourse.AUTO_ID;
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

        public async Task<RESPONSE__MODEL> GetListTeacher()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listTeacher = await (from a in ctx.MEMBER_TEACH_COURSE
                                             join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                                             join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                                             join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                                             join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                                             where b.ROLE_ID == 1 && f.STATUS == 2
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
                                                 //LIST_STUDENT_COMMENT = a.MEMBER_REGIS_COURSE_COMMENT.Select(o => o.COMMENT).ToList(),
                                                 REGIS_COURSE_ID = a.AUTO_ID,
                                                 CATEGORY = "ไม่ระบุ",
                                             }).ToListAsync();

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

        public async Task<RESPONSE__MODEL> GetListTeacherFromAutoID(CONTAINER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    //var memberRole = await ctx.MEMBER_ROLE.Where(o => o.MEMBER_ID == dto.MEMBERS.AUTO_ID).FirstOrDefaultAsync();
                    //var memberRegisCourse = await ctx.MEMBER_REGIS_COURSE
                    //                                 .Where(o => o.MEMBER_ROLE.AUTO_ID == memberRole.AUTO_ID)
                    //                                 .Select(p => p.COURSE_ID).ToListAsync();

                    //var listTeacher = await (from a in ctx.MEMBER_TEACH_COURSE
                    //                         join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                    //                         join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                    //                         join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                    //                         join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                    //                         where b.ROLE_ID == 1 && f.STATUS == 2 && !memberRegisCourse.Contains(d.AUTO_ID)
                    //                         select new CUSTOM_MEMBERS
                    //                         {
                    //                             ROLE = b.ROLE_ID.ToString(),
                    //                             AUTO_ID = c.AUTO_ID,
                    //                             PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                    //                             COURSE_ID = d.AUTO_ID,
                    //                             MEMBER_ROLE_ID = b.AUTO_ID,
                    //                             FULLNAME = c.FULLNAME ?? "ไม่ระบุ",
                    //                             SEX = c.SEX == "1" ? "ชาย" : "หญิง",
                    //                             AGE = c.AGE,
                    //                             LOCATION = c.LOCATION ?? "ไม่ระบุ",
                    //                             MOBILE = c.MOBILE ?? "ไม่ระบุ",
                    //                             USER_NAME = f.USER_NAME ?? "ไม่ระบุ",
                    //                             COURSE = d.NAME ?? "ไม่ระบุ",
                    //                             ABOUT = c.ABOUT ?? "ไม่ระบุ",
                    //                             //LIST_STUDENT_COMMENT = a.MEMBER_REGIS_COURSE_COMMENT.Select(o => o.COMMENT).ToList(),
                    //                             REGIS_COURSE_ID = a.AUTO_ID,
                    //                             CATEGORY = "ไม่ระบุ",
                    //                         }).ToListAsync();

                    //resp.OUTPUT_DATA = listTeacher;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = false;
            }
            return resp;


        }

        public async Task<RESPONSE__MODEL> GetListCourse()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listCourse = await ctx.COURSES.Select(o => o.NAME).ToListAsync();
                    listCourse = listCourse.Distinct().OrderBy(o => o).ToList();
                    resp.OUTPUT_DATA = listCourse;
                }

            }
            catch (Exception ex)
            {

            }
            return resp;
        }

       
    }
}
