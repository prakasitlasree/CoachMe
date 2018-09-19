using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COACHME.MODEL;

namespace COACHME.DataService
{
    public class AuthenticationServices
    {
        public List<LOGON> GetAll()
        {
            var result = new List<LOGON>();
            try
            {
                using (var ctx = new BIG_RY_DBEntities())
                {
                    result = ctx.LOGONs.ToList();
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
