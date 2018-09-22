using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.DAL;
using COACHME.CUSTOM_MODELS;

namespace COACHME.DATASERVICE
{
    public  class AuthenticationServices
    {
        public async  Task<bool> GetLogOnAll(string email, string password)
        {
            var result = false;
            var fullname = string.Empty;
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                   var  member = await ctx.MEMBER_LOGON.Include("MEMBERS").Where(x => x.USER_NAME == email && x.PASSWORD == password).FirstOrDefaultAsync();
                    if (member != null)
                    {
                        fullname = member.MEMBERS.FULLNAME;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                     
                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "LOGON";
                    activity.FULLNAME = fullname;
                    activity.USER_NAME = email;
                    activity.PASSWORD = password;
                    activity.STATUS = result;
                    ctx.LOGON_ACTIVITY.Add(activity);
                    var act_result = await ctx.SaveChangesAsync();

                }
                return  result;
                 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Register(RegisterModel dto)
        {
            var result = false;
            var member = new MEMBERS();
             
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var checkMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME == dto.Email).FirstOrDefaultAsync();
                    if(checkMember != null)
                    {

                    }
                    else
                    {
                        //1.Master 
                        member.FULLNAME = dto.Fullname;
                        member.FIRST_NAME = dto.Fullname;
                        member.MOBILE = dto.Mobile;

                        //2. Details 
                        MEMBER_ROLE memberRole = new MEMBER_ROLE();
                        memberRole.ROLE_ID = 1;

                        //3. Details 
                        MEMBER_LOGON memberLogon = new MEMBER_LOGON();
                        memberLogon.USER_NAME = dto.Email;
                        memberLogon.PASSWORD = dto.Password; 

                        //4. Add detail to master 
                        member.MEMBER_ROLE.Add(memberRole);
                        member.MEMBER_LOGON.Add(memberLogon);

                        //5. Save master
                        ctx.MEMBERS.Add(member); 
                        await ctx.SaveChangesAsync();
                    }
                  
                    result = true;                            

                }
               

            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
               
            }
            return result;
        }
    }
}
