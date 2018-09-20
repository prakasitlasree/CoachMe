using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COACHME.MODEL;
using COACHME.DAL;
namespace COACHME.DATASERVICE
{
    public static class AuthenticationServices
    {
        public static List<MEMBER_LOGON> GetLogOnAll()
        {
            var result = new List<MEMBER_LOGON>();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    
                    result = ctx.MEMBER_LOGON.ToList();

                    //test
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
