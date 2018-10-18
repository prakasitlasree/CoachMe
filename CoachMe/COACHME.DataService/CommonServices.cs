using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System.Net.Mail;
using System.Net;
using COACHME.DAL;

namespace COACHME.DATASERVICE
{
   public class CommonServices
    {
        public async Task<RESPONSE__MODEL> GetListProvince()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listProvince = await ctx.PROVINCE.OrderBy(x=>x.PROVINCE_ID).Select(o => o.PROVINCE_NAME).ToListAsync();
                   
                    resp.OUTPUT_DATA = listProvince;
                } 
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.STATUS = false; 
            }
            return resp;
        }
    }
}
