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
using System.Collections.Generic;

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

                    var member = await ctx.MEMBER_LOGON.Include("MEMBERS").Where(x => x.USER_NAME.ToUpper() == email.ToUpper() && x.PASSWORD == password).FirstOrDefaultAsync();
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
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var member = new MEMBERS();
                    var checkMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == dto.Email.ToUpper()).FirstOrDefaultAsync();
                    if (checkMember == null)
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
                        memberLogon.USER_NAME = dto.Email.ToUpper();
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
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

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
                //1. Check with exiting email logon
                var member = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == email.Email.ToUpper()).FirstOrDefaultAsync();
                if (member != null)
                {
                    #region ===== Create Mail====
                    var listConfig = await ctx.CONFIGURATION.Where(x => x.CONTROLER_NAME == "AccountController").ToListAsync();
                    string smtpAcc = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.SMTP_ACCOUNT.ToString()).FirstOrDefault().VALUE.ToString();
                    var fromAddress = new MailAddress(smtpAcc, "No-reply@CoachMe.asia");//Create Mail

                    string smtpPassword = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.SMTP_PASSWORD.ToString()).FirstOrDefault().VALUE.ToString();
                    string from = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_SENDER.ToString()).FirstOrDefault().VALUE.ToString();
                    string subject = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_SUBJECT.ToString()).FirstOrDefault().VALUE.ToString();
                    string footer = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_FOOTER.ToString()).FirstOrDefault().VALUE.ToString();
                    string link = "http://localhost:1935/Account/ResetPassword?";

                    var hash = GenUniqueKey(email.Email);
                    link = link + @"USER_NAME=" + email.Email + @"&TOKEN_HASH=" + hash;
                    //Open When Deploy.  
                    link = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.FORGET_PASSWORD_URL.ToString()).FirstOrDefault().VALUE + @"USER_NAME=" + email.Email + @"&TOKEN_HASH=" + hash; ;

                    //mail body  
                    string body = ReplaceMailBody(listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_BODY.ToString()).FirstOrDefault().VALUE.ToString(), member.USER_NAME, link);
                    #endregion

                    #region ===== Add Activity====
                    try
                    {
                        resetPassword.USER_NAME = email.Email.ToUpper();
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



                    //2. Send New password to email
                    #region =========SEND EMAIL =========
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
                            Credentials = new NetworkCredential(fromAddress.Address, smtpPassword)
                        };

                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
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
                        activity.USER_NAME = email.Email.ToUpper();
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
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string[] reserved = new string[] { ";", "/", "?", ":", "@", "&", "=", "+", ",", "$" };
            foreach (var item in reserved)
            {
                token = token.Replace(item, "");
            }
            return token;

            // string str = email + DateTime.Now.ToString();
            //int hash = str.GetHashCode();
            //return hash.ToString();
        }

        public async Task<bool> ResetPasswordValidate(ResetPasswordValidateModel resetPassword)
        {
            var result = false;
            using (var ctx = new COACH_MEEntities())
            {
                var tokenValidate = await ctx.RESET_PASSWORD.Where(x => x.USER_NAME.ToUpper() == resetPassword.USER_NAME.ToString().ToUpper() && x.TOKEN_HASH == resetPassword.TOKEN_HASH && x.TOKEN_EXPIRATION > DateTime.Now && x.TOKEN_USED == false).FirstOrDefaultAsync();
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
                        //1. check isvalid token
                        var resetIoKen = await ctx.RESET_PASSWORD.Where(x => x.TOKEN_HASH == dto.TOKEN_HASH && x.TOKEN_USED == false).FirstOrDefaultAsync();
                        if (resetIoKen != null)
                        {
                            //2. update new password
                            var updateMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == dto.EMAIL.ToUpper()).FirstOrDefaultAsync();
                            updateMember.PASSWORD = dto.NEW_PASSWORD;

                            //3. Update status token used
                            resetIoKen.TOKEN_USED = true;

                            //4. Update log
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
                        else
                        {
                            result = false;
                        }

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

        private string ReplaceMailBody(string mailbody, string user, string actionUrl)
        {
            mailbody = mailbody.Replace("[Product Name]", "CoachMe");
            mailbody = mailbody.Replace("{{name}}", user);
            mailbody = mailbody.Replace("{{action_url}}", actionUrl);
            return mailbody;
        }
    }
}