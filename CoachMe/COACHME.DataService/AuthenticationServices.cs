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
    public class AuthenticationServices
    {
        #region ========= SERVICE&BUSINESS =========
        public async Task<RESPONSE__MODEL> GetLogOnAll(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            var memberObj = new MEMBERS();
            var result = false;
            var fullname = string.Empty;
            try
            {
                using (var ctx = new COACH_MEEntities())
                {

                    var member = await ctx.MEMBER_LOGON.Include("MEMBERS").Where(x => x.USER_NAME.ToUpper() == dto.USER_NAME.ToUpper() && x.PASSWORD == dto.PASSWORD).FirstOrDefaultAsync();
                    if (member != null && member.STATUS == 2)
                    {
                        fullname = member.MEMBERS.FULLNAME;
                        memberObj = ctx.MEMBERS
                                       .Include("MEMBER_ROLE")
                                       .Include("MEMBER_PACKAGE")
                                       .Where(x => x.AUTO_ID == member.MEMBER_ID).FirstOrDefault();
                        result = true;
                    }
                    else if(member != null && member.STATUS == 1)
                    {
                        result = false;
                        resp.Message = "not active";
                    }
                    else
                    {
                        
                        result = false;
                    }

                    var activity = new LOGON_ACTIVITY();
                    activity.DATE = DateTime.Now;
                    activity.ACTION = "LOGON";
                    activity.FULLNAME = fullname;
                    activity.USER_NAME = dto.USER_NAME;
                    activity.PASSWORD = dto.PASSWORD;
                    activity.STATUS = result;
                    ctx.LOGON_ACTIVITY.Add(activity);
                    var act_result = await ctx.SaveChangesAsync();

                    resp.STATUS = result;
                    resp.OUTPUT_DATA = memberObj;
                    return resp;
                }

            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
                resp.STATUS = result;
                throw ex;
            }
        }

        public async Task<RESPONSE__MODEL> Register(REGISTER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            var result = false;
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var member = new MEMBERS();
                    var checkMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == dto.EMAIL.ToUpper()).FirstOrDefaultAsync();
                    if (checkMember == null )
                    {
                        #region ==== SET DETAIL ====
                        //1.Master 
                        member.FULLNAME = dto.FULLNAME;
                        member.CREATED_DATE = DateTime.Now;
                        member.CREATED_BY = dto.FULLNAME;
                        member.UPDATED_DATE = DateTime.Now;
                        member.UPDATED_BY = dto.FULLNAME;
                        member.SEX = dto.GENDER;

                        //2. Details 
                        MEMBER_ROLE memberRole = new MEMBER_ROLE();
                        memberRole.ROLE_ID = dto.ROLE;
                        memberRole.CREATED_DATE = DateTime.Now;
                        memberRole.CREATED_BY = dto.FULLNAME;
                        memberRole.UPDATED_DATE = DateTime.Now;
                        memberRole.UPDATED_BY = dto.FULLNAME;

                        //3. Details 
                        MEMBER_LOGON memberLogon = new MEMBER_LOGON();
                        memberLogon.USER_NAME = dto.EMAIL.ToUpper();
                        memberLogon.PASSWORD = dto.PASSWORD;
                        memberLogon.STATUS = 1;
                        memberLogon.TOKEN_HASH = GenUniqueKey(dto.EMAIL.ToUpper());
                        memberLogon.TOKEN_USED = false;
                        memberLogon.TOKEN_EXPIRATION = DateTime.Now.AddHours(3);
                        memberLogon.CREATED_DATE = DateTime.Now;
                        memberLogon.CREATED_BY = dto.FULLNAME;
                        memberLogon.UPDATED_DATE = DateTime.Now;
                        memberLogon.UPDATED_BY = dto.FULLNAME;


                        //4. Add detail to master 
                        member.MEMBER_ROLE.Add(memberRole);
                        member.MEMBER_LOGON.Add(memberLogon);

                        //5. Save master
                        ctx.MEMBERS.Add(member);
                        await ctx.SaveChangesAsync();
                        #endregion

                        #region === SEND VERIFY MAIL ===
                        var listConfig = await ctx.CONFIGURATION.Where(x => x.CONTROLER_NAME == "AccountController").ToListAsync();
                        string smtpAcc = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.SMTP_ACCOUNT.ToString()).FirstOrDefault().VALUE.ToString();
                        var fromAddress = new MailAddress(smtpAcc, "No-reply@CoachMe.asia");//Create Mail
                        string smtpPassword = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.SMTP_PASSWORD.ToString()).FirstOrDefault().VALUE.ToString();
                        string from = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_SENDER.ToString()).FirstOrDefault().VALUE.ToString();
                        string subject = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_SUBJECT_REGISTER.ToString()).FirstOrDefault().VALUE.ToString();
                        string footer = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_FOOTER.ToString()).FirstOrDefault().VALUE.ToString();
                        //string link = "http://localhost:1935/Account/RegisterVerify?";
                        //link = link + @"USER_NAME=" + dto.EMAIL + @"&TOKEN_HASH=" + memberLogon.TOKEN_HASH;
                        //Open When Deploy.  
                        string link = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.REGISTER_VERIFY_URL.ToString()).FirstOrDefault().VALUE + @"USER_NAME=" + dto.EMAIL + @"&TOKEN_HASH=" + memberLogon.TOKEN_HASH;
                        string body = ReplaceMailBody(listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_BODY_REGISTER.ToString()).FirstOrDefault().VALUE.ToString(), dto.EMAIL, link);
                        try
                        {
                            var toAddress = new MailAddress(dto.EMAIL);
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

                        }
                        catch (Exception ex)
                        {
                            result = false;
                            throw ex;
                        }

                        #endregion

                        #region === Activity ===
                        var activity = new LOGON_ACTIVITY();
                        activity.DATE = DateTime.Now;
                        activity.ACTION = "Register";
                        activity.FULLNAME = dto.FULLNAME;
                        activity.USER_NAME = dto.EMAIL;
                        activity.PASSWORD = dto.PASSWORD;
                        activity.STATUS = result;
                        ctx.LOGON_ACTIVITY.Add(activity);
                        #endregion

                        var act_result = await ctx.SaveChangesAsync();

                       
                        result = true;
                    }
                    else if(checkMember != null && checkMember.TOKEN_USED == true)
                    {
                        resp.Message = "active";
                        result = false;
                    }
                    else
                    {
                        resp.Message = "not active";
                        result = false;
                    }

                }

            }
            catch (Exception ex)
            {
                result = false;
                throw ex;

            }
            resp.STATUS = result;
            return resp;
        }

        public async Task<RESPONSE__MODEL> RegisterVerify(MEMBER_LOGON dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            var result = false;
            try
            {
                using (var ctx = new COACH_MEEntities())
                {
                    var tokenValidate = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == dto.USER_NAME.ToString().ToUpper() && x.TOKEN_HASH == dto.TOKEN_HASH && x.TOKEN_EXPIRATION > DateTime.Now && x.TOKEN_USED != true).FirstOrDefaultAsync();
                    if (tokenValidate != null)
                    {
                        //Update Member Status
                        var member = new MEMBERS();
                        var checkMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == dto.USER_NAME.ToUpper() && x.TOKEN_USED != true).FirstOrDefaultAsync();
                        if (checkMember != null)
                        {
                            checkMember.STATUS = 2;
                            checkMember.TOKEN_USED = true;
                            await ctx.SaveChangesAsync();
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

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
            resp.STATUS = result;
            return resp;
        }

        public async Task<RESPONSE__MODEL> ForgotPassword(FORGOT_PASSWORD_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            var result = false;
            var resetPassword = new RESET_PASSWORD();
            var act_result = 0;
            using (var ctx = new COACH_MEEntities())
            {
                //1. Check with exiting email logon
                var member = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == dto.EMAIL.ToUpper()).FirstOrDefaultAsync();
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

                    var hash = GenUniqueKey(dto.EMAIL);
                    link = link + @"USER_NAME=" + dto.EMAIL + @"&TOKEN_HASH=" + hash;
                    //Open When Deploy.  
                    link = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.FORGET_PASSWORD_URL.ToString()).FirstOrDefault().VALUE + @"USER_NAME=" + dto.EMAIL + @"&TOKEN_HASH=" + hash; ;

                    //mail body  
                    string body = ReplaceMailBody(listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_BODY.ToString()).FirstOrDefault().VALUE.ToString(), member.USER_NAME, link);
                    #endregion

                    #region ===== Add Activity====
                    try
                    {
                        resetPassword.USER_NAME = dto.EMAIL.ToUpper();
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
                    var emailTo = dto;

                    try
                    {
                        var toAddress = new MailAddress(dto.EMAIL);
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
                        activity.USER_NAME = dto.EMAIL.ToUpper();
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
            resp.STATUS = result;
            return resp;
        }

        public async Task<RESPONSE__MODEL> ResetPasswordValidate(RESET_PASSWORD_VALIDATE_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            using (var ctx = new COACH_MEEntities())
            {
                var tokenValidate = await ctx.RESET_PASSWORD.Where(x => x.USER_NAME.ToUpper() == dto.USER_NAME.ToString().ToUpper() && x.TOKEN_HASH == dto.TOKEN_HASH && x.TOKEN_EXPIRATION > DateTime.Now && x.TOKEN_USED == false).FirstOrDefaultAsync();
                try
                {
                    if (tokenValidate != null)
                    {
                        resp.STATUS = true;
                    }
                    else
                    {
                        resp.STATUS = false;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return resp;
            }
        }

        public async Task<RESPONSE__MODEL> ResetPassword(RESET_PASSWORD_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
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
                            activity.STATUS = resp.STATUS;
                            ctx.LOGON_ACTIVITY.Add(activity);
                            #endregion

                            await ctx.SaveChangesAsync();
                            resp.STATUS = true;
                        }
                        else
                        {
                            resp.STATUS = false;
                        }

                    }

                }
                catch (Exception ex)
                {
                    resp.STATUS = false;
                    throw ex;

                }
            }
            return resp;
        }

        public async Task<RESPONSE__MODEL> SendMailConfirm(REGISTER_MODEL dto)
        {
            RESPONSE__MODEL resp = new RESPONSE__MODEL();
            using (var ctx = new COACH_MEEntities())
            {
                var checkMember = await ctx.MEMBER_LOGON.Where(x => x.USER_NAME.ToUpper() == dto.EMAIL.ToUpper() && x.TOKEN_USED != true).FirstOrDefaultAsync();
                #region === SEND VERIFY MAIL ===
                var listConfig = await ctx.CONFIGURATION.Where(x => x.CONTROLER_NAME == "AccountController").ToListAsync();
                string smtpAcc = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.SMTP_ACCOUNT.ToString()).FirstOrDefault().VALUE.ToString();
                var fromAddress = new MailAddress(smtpAcc, "No-reply@CoachMe.asia");//Create Mail
                string smtpPassword = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.SMTP_PASSWORD.ToString()).FirstOrDefault().VALUE.ToString();
                string from = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_SENDER.ToString()).FirstOrDefault().VALUE.ToString();
                string subject = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_SUBJECT_REGISTER.ToString()).FirstOrDefault().VALUE.ToString();
                string footer = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_FOOTER.ToString()).FirstOrDefault().VALUE.ToString();
                //string link = "http://localhost:1935/Account/RegisterVerify?";
                //link = link + @"USER_NAME=" + dto.EMAIL + @"&TOKEN_HASH=" + checkMember.TOKEN_HASH;
                //Open When Deploy.
                string link = listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.REGISTER_VERIFY_URL.ToString()).FirstOrDefault().VALUE + @"USER_NAME=" + dto.EMAIL + @"&TOKEN_HASH=" + checkMember.TOKEN_HASH;
                string body = ReplaceMailBody(listConfig.Where(x => x.SETING_NAME == StandardEnums.ConfigurationSettingName.MAIL_BODY_REGISTER.ToString()).FirstOrDefault().VALUE.ToString(), dto.EMAIL, link);
                try
                {
                    var toAddress = new MailAddress(dto.EMAIL);
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
                    resp.STATUS = true;
                }
                catch (Exception ex)
                {
                    resp.STATUS = false;
                    throw ex;
                }

                #endregion
            }

            return resp;
        }


        #endregion

        #region ====== METHOD ======
        private string ReplaceMailBody(string mailbody, string user, string actionUrl)
        {
            mailbody = mailbody.Replace("[Product Name]", "CoachMe");
            mailbody = mailbody.Replace("{{name}}", user);
            mailbody = mailbody.Replace("{{action_url}}", actionUrl);
            return mailbody;
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
        #endregion
    }
}