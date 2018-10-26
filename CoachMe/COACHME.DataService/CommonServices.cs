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
                    resp.STATUS = true;
                } 
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.STATUS = false; 
            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> GetListProvinceWithID()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            CONTAINER_MODEL model = new CONTAINER_MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var listProvince = await ctx.PROVINCE.OrderBy(x => x.PROVINCE_ID).ToListAsync();

                    resp.OUTPUT_DATA = listProvince;
                    resp.STATUS = true;
                }
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.STATUS = false;
            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> GetListAmphur(int provinceID)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    resp.OUTPUT_DATA = await ctx.AMPHUR
                                                .Where(o => o.PROVINCE_ID == provinceID)
                                                .ToListAsync();
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }

            return resp;
        }

        public async Task<RESPONSE__MODEL> GetListCategory()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    resp.OUTPUT_DATA = await ctx.CATEGORY
                                                .ToListAsync();
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }
            return resp;
        }
  
        public async Task<RESPONSE__MODEL> GetListCourse()
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var course = await ctx.COURSES.ToListAsync();

                    resp.OUTPUT_DATA = course;
                }
                resp.STATUS = true;

            }
            catch (Exception ex)
            {
                resp.STATUS = false;
                throw ex;
            }
            return resp;
        }
    }
}
