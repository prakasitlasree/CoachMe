using COACHME.DAL;
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
        public async Task<RESPONSE__MODEL> FindTeacher(SEARCH_TEACHER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listStu = await (from a in ctx.MEMBER_REGIS_COURSE.Include("MEMBER_REGIS_COURSE_COMMENT")
                                         join b in ctx.MEMBER_ROLE on a.MEMBER_ROLE_ID equals b.AUTO_ID
                                         join c in ctx.MEMBERS on b.MEMBER_ID equals c.AUTO_ID
                                         join d in ctx.COURSES on a.COURSE_ID equals d.AUTO_ID
                                         join f in ctx.MEMBER_LOGON on c.AUTO_ID equals f.MEMBER_ID
                                         //where dto.TEACH_GENDER.Contains(c.SEX) && d.NAME == dto.COURSE_NAME
                                         select new CUSTOM_MEMBERS
                                         {
                                             AUTO_ID = c.AUTO_ID,
                                             PROFILE_IMG_URL = c.PROFILE_IMG_URL,
                                             STATUS = a.STATUS,
                                             FULLNAME = c.FULLNAME ?? "",
                                             SEX = c.SEX, //== "1" ? "ชาย" : "หญิง",
                                             AGE = c.AGE,
                                             LOCATION = c.LOCATION ?? "",
                                             MOBILE = c.MOBILE ?? "",
                                             USER_NAME = f.USER_NAME ?? "",
                                             COURSE = d.NAME ?? "",
                                             ABOUT = c.ABOUT ?? "",
                                             LIST_STUDENT_COMMENT = a.MEMBER_REGIS_COURSE_COMMENT.Select(o => o.COMMENT).ToList(),
                                             REGIS_COURSE_ID = a.AUTO_ID,
                                             CATEGORY = "1",
                                            
                                             

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
    }
}
