using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using COACHME.MODEL;
using COACHME.MODEL.CUSTOM_MODELS;
using System.Net.Mail;
using System.Net;
using COACHME.DAL;
using System.Configuration;

namespace COACHME.DATASERVICE
{
    public class AuthenticationServices
    {
        public async Task<bool> GetLogOnAll(string email, string password)
        {
            var result = false;
            var fullname = string.Empty;
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var member = await ctx.MEMBER_LOGON.Include("MEMBERS").Where(x => x.USER_NAME == email && x.PASSWORD == password).FirstOrDefaultAsync();
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
                return result;

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
                    if (checkMember != null)
                    {

                    }
                    else
                    {
                        //1.Master 
                        member.FULLNAME = dto.Fullname;
                        member.FIRST_NAME = dto.Fullname;
                        member.CREATED_DATE = DateTime.Now;
                        member.CREATED_BY = dto.Fullname;
                        member.UPDATED_DATE = DateTime.Now;
                        member.UPDATED_BY = dto.Fullname;

                        //2. Details 
                        MEMBER_ROLE memberRole = new MEMBER_ROLE();
                        memberRole.ROLE_ID = 1;
                        memberRole.CREATED_DATE = DateTime.Now;
                        memberRole.CREATED_BY = dto.Fullname;
                        memberRole.UPDATED_DATE = DateTime.Now;
                        memberRole.UPDATED_BY = dto.Fullname;

                        //3. Details 
                        MEMBER_LOGON memberLogon = new MEMBER_LOGON();
                        memberLogon.USER_NAME = dto.Email;
                        memberLogon.PASSWORD = dto.Password;
                        memberLogon.CREATED_DATE = DateTime.Now;
                        memberLogon.CREATED_BY = dto.Fullname;
                        memberLogon.UPDATED_DATE = DateTime.Now;
                        memberLogon.UPDATED_BY = dto.Fullname;

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

        public async Task<bool> ForgotPassword(ForgotPasswordModel email)
        {
            var result = false;
            var resetPassword = new RESET_PASSWORD();
            var act_result = 0;
            using (var ctx = new COACH_MEEntities()) 
            {
                var config = await ctx.CONFIGURATION.Where(x => x.CONTROLER_NAME == "AccountController").ToListAsync();
                var emailFrom = ConfigurationManager.AppSettings["Email"];
                var fromAddress = new MailAddress(emailFrom, "No-reply@CoachMe.asia");
                var fromPassword = ConfigurationManager.AppSettings["Password"];
                string from = config[0].VALUE.ToString();
                string subject = config[1].VALUE.ToString();
                string footer = config[3].VALUE.ToString();
                string link = "http://localhost:1935/Account/ResetPassword?";
                //Open When Deploy.
                //link = "http://119.59.122.206/Account/ResetPassword?";

                var hash = GenUniqueKey(email.Email);

                string body = "Reset password link : " + link + "USER_NAME=" + email.Email + "&TOKEN_HASH=" + hash + Environment.NewLine + footer;
                
                #region ===== GEN HASH CODE====
                try
                { 
                    resetPassword.USER_NAME = email.Email;
                    resetPassword.TOKEN_HASH = hash;
                    resetPassword.TOKEN_USED = false;
                    resetPassword.TOKEN_EXPIRATION = DateTime.Now.AddMinutes(30);
                    ctx.RESET_PASSWORD.Add(resetPassword);
                    act_result = await ctx.SaveChangesAsync();
                }
                catch (Exception)
                {
                }
                #endregion
            
                //1. Check with exiting email logon
                var member = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME == email.Email).FirstOrDefaultAsync();

                if (member != null)
                {
                    //2. Send New password to email
                    #region =========SEND EMAIL TEST=========
                    var emailTo = email;
                    

                    try
                    {
                        var toAddress = new MailAddress(email.Email);
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                        };

                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = subject,
                            Body = body
                        })
                        {
                            smtp.Send(message);
                        }
                        #endregion

                        result = true;

                        //3. Add activity
                        #region =========Add Activity=========
                        var activity = new LOGON_ACTIVITY();
                        activity.DATE = DateTime.Now;
                        activity.ACTION = "FORGET PASSWORD";
                        activity.FULLNAME = "SYSTEM";
                        activity.USER_NAME = email.Email;
                        activity.PASSWORD = "SYSTEM";
                        activity.STATUS = result;
                        ctx.LOGON_ACTIVITY.Add(activity);
                        act_result = await ctx.SaveChangesAsync();
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }

                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        private string GenUniqueKey(string email)
        {
            string str = email + DateTime.Now.ToString();
            int hash = str.GetHashCode();
            return hash.ToString();
        }

        public async Task<bool> ResetPasswordValidate(ResetPasswordValidateModel resetPassword)
        {
            var result = false;
            using (var ctx = new COACH_MEEntities())
            {
                var tokenValidate = await ctx.RESET_PASSWORD.Where(x => x.USER_NAME == resetPassword.USER_NAME.ToString() && x.TOKEN_HASH == resetPassword.TOKEN_HASH && x.TOKEN_EXPIRATION > DateTime.Now && x.TOKEN_USED == false).FirstOrDefaultAsync();
                try
                {
                    if (tokenValidate != null)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordModel dto)
        {
            var result = false;
            if (dto != null)
            {

                try
                {
                    using (var ctx = new COACH_MEEntities())
                    {


                        var updateMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME == dto.EMAIL).FirstOrDefaultAsync();
                        updateMember.PASSWORD = dto.NEW_PASSWORD;

                        var updateToken = await ctx.RESET_PASSWORD.Where(x => x.TOKEN_HASH == dto.TOKEN_HASH).FirstOrDefaultAsync();
                        updateToken.TOKEN_USED = true;

                        #region =========Add Activity=========
                        var activity = new LOGON_ACTIVITY();
                        activity.DATE = DateTime.Now;
                        activity.ACTION = "RESET PASSWORD";
                        activity.FULLNAME = dto.EMAIL;
                        activity.USER_NAME = dto.EMAIL;
                        activity.PASSWORD = dto.NEW_PASSWORD;
                        activity.STATUS = result;
                        ctx.LOGON_ACTIVITY.Add(activity);
                        #endregion

                        await ctx.SaveChangesAsync();
                        result = true;
                    }

                }
                catch (Exception ex)
                {
                    result = false;
                    throw ex;

                }
            }
            return result;
        }
    }
}
