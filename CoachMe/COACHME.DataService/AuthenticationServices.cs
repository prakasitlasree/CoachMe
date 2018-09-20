using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.DAL;
using COACHME.CustomModels;

namespace COACHME.DATASERVICE
{
    public  class AuthenticationServices
    {
        public async  Task<bool> GetLogOnAll(string email, string password)
        {
            var result = false;
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                   var  member = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME == email && x.PASSWORD == password).SingleOrDefaultAsync();
                    if (member != null)
                    {
                        result= true;
                    }
                    else
                    {
                        result = false;
                    }
                    
                }
                return  result;
                 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Register(RegisterModel source)
        {
            var result = false;
            MEMBERS member = new MEMBERS();
            MEMBER_LOGON memberLogon = new MEMBER_LOGON();

           
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var checkMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME == source.USER_NAME).FirstOrDefaultAsync();
                    if(checkMember != null)
                    {

                    }
                    else
                    {
                        memberLogon.USER_NAME = source.USER_NAME;
                        memberLogon.PASSWORD = source.PASSWORD;

                        member.FULLNAME = source.FULLNAME;
                        member.MOBILE = source.MOBILE;

                        ctx.MEMBERS.Add(member);
                        ctx.MEMBER_LOGON.Add(memberLogon);
                        //await ctx.SaveChangesAsync();
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
