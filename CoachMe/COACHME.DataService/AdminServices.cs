using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System.Collections.Generic;
using System.Web;
using System.IO;
using COACHME.DAL;


namespace COACHME.DATASERVICE
{
    public class AdminServices
    {
        public async Task<RESPONSE__MODEL> GetMemberPackage()
        {
            var resp = new RESPONSE__MODEL(); 
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memPackage = await  ctx.MEMBER_PACKAGE.Include("MEMBERS").OrderByDescending(x => x.CREATED_DATE).ToListAsync();
                    var list = new List<MEMBER_PACKAGE>();
                    foreach (var item in memPackage)
                    {
                        item.MEMBERS.MEMBER_PACKAGE = null;
                        list.Add(item);
                    }
                    resp.OUTPUT_DATA = list;
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

        public async Task<RESPONSE__MODEL> ActivatePackage(CONTAINER_MODEL dto)
        {
            var resp = new RESPONSE__MODEL();
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var memPackage = await ctx.MEMBER_PACKAGE.Where(x => x.AUTO_ID == dto.MEMBER_PACKAGE.AUTO_ID).FirstOrDefaultAsync();
                    memPackage.STATUS = StandardEnums.ActivatePurchaseStatus.ACTIVE.ToString();

                   var output = await ctx.SaveChangesAsync();
                    resp.OUTPUT_DATA = memPackage;
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
    }
}
