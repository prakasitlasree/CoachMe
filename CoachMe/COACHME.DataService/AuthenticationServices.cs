using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.DAL;
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
    }
}
