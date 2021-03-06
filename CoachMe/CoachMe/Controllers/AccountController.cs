﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using COACHME.MODEL;
using COACHME.DATASERVICE;
using CoachMe.Models;
using COACHME.MODEL.CUSTOM_MODELS;

namespace CoachMe.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public const string PARTIAL_VIEW_FOLDER = "~/Views/Account/ForgotPasswordPartial.cshtml";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private AuthenticationServices service = new AuthenticationServices();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Fo(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(MEMBER_LOGON dto, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            if (string.IsNullOrEmpty(dto.USER_NAME) || string.IsNullOrEmpty(dto.PASSWORD))
            {
                ModelState.AddModelError("", "Please input username/password");
                return View(dto);
            }

            var result = await service.GetLogOnAll(dto);
            MEMBERS param = result.OUTPUT_DATA;
            if (result.STATUS)
            {
                Session["logon"] = param;
                if (param.MEMBER_ROLE.FirstOrDefault() != null)
                {
                    if (param.MEMBER_ROLE.FirstOrDefault().ROLE_ID == 1 || param.MEMBER_ROLE.FirstOrDefault().ROLE_ID == 3)
                    {
                        return RedirectToAction("profile", "home");
                    }
                    else
                    {
                        return RedirectToAction("index", "mainhome");
                    }
                   
                }
                return View(dto);
            }
            else if (result.STATUS == false && result.Message == "not active")
            {
                ViewBag.ActiveFail = "This email has been register. Do you want to send active mail again ?";
                return View(dto);
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(dto);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChooseTeacherLogin(MEMBER_LOGON dto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("login", "account");
            }
            if (string.IsNullOrEmpty(dto.USER_NAME) || string.IsNullOrEmpty(dto.PASSWORD))
            {
                ModelState.AddModelError("", "Please input username/password");
                return View(dto);
            }

            var result = await service.Login(dto);
            MEMBERS param = result.OUTPUT_DATA;
            if (result.STATUS)
            {
                Session["logon"] = param;
                if (param.MEMBER_ROLE.FirstOrDefault() != null && param.MEMBER_ROLE.FirstOrDefault().ROLE_ID == 2 || param.MEMBER_ROLE.FirstOrDefault().ROLE_ID == 3)
                {
                    return RedirectToAction("index", "mainhome");
                }
                else if (param.MEMBER_ROLE.FirstOrDefault() != null && param.MEMBER_ROLE.FirstOrDefault().ROLE_ID == 1)
                {
                    return RedirectToAction("index", "mainhome");//return RedirectToAction("index", "teacher");
                }
                else
                {
                    return RedirectToAction("login", "account");
                }

            }
            else if (result.STATUS == false && result.Message == "not active")
            {
                ViewBag.ActiveFail = "This email has been register. Do you want to send active mail again ?";
                return RedirectToAction("login", "account");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return RedirectToAction("login", "account");
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            //var registerModel = new RegisterModel();
            return View();
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> RegisterVerify(MEMBER_LOGON dto)
        {
            //var registerModel = new RegisterModel();
            var result = await service.RegisterVerify(dto);
            if (result.STATUS)
            {
                TempData["RegisterVerify"] = "บัญชีคุณยินยันตัวตนเรียบร้อย";
                return RedirectToAction("login", "account");
            }
            else
            {

                return RedirectToAction("errorpage", "home");
            }
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> Register(REGISTER_MODEL dto, string buttonType)
        {
            var model = new RegisterViewModel();
            var source = new REGISTER_MODEL();
            if (ModelState.IsValid)
            {
                if (buttonType == "Save")
                {
                    var result = await service.Register(dto);
                    if (result.STATUS)
                    {
                        ViewBag.Success = "สมัครเรียบร้อย กรุณาตรวจสอบอีเมลล์ เพื่อยืนยันตัวตนภายใน 3 ชั่วโมง";
                        return View(dto);
                    }
                    else if (result.STATUS == false && result.Message == "active")
                    {
                        ViewBag.Fail = "อีเมลล์นี้อยู่ในระบบเรียบร้อยแล้ว";
                        return View(dto);
                    }
                    else if (result.STATUS == false && result.Message == "not active")
                    {
                        ViewBag.ActiveFail = "อีเมลล์นี้ยังไม่ได้ยืนยันตัวตน คุณต้องการส่งอีเมลล์เพื่อยืนยันตัวตนอีกครั้งหรือไม่ ?";
                        return View(dto);
                    }
                    return View(dto);
                }

                if (buttonType == "Cancel")
                {
                    return RedirectToAction("login", "account");
                }

            }
            return View(dto);
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendMailConfirm(REGISTER_MODEL dto)
        {
            var result = await service.SendMailConfirm(dto);
            if (result.STATUS)
            {
                return RedirectToAction("login", "account");
            }
            else
            {
                return RedirectToAction("errorpage", "home");
            }
        }


        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(FORGOT_PASSWORD_MODEL dto, string buttonType)
        {

            if (ModelState.IsValid)
            {
                if (buttonType == "Request")
                {
                    var result = await service.ForgotPassword(dto);
                    if (result.STATUS)
                    {

                        ViewBag.Success = "Please check your email to reset your password.";
                        return View(dto);
                    }
                    else
                    {
                        ViewBag.Fail = "This email did not registerd yet.";
                        return View(dto);
                    }
                }
                else
                {

                }

                var user = await UserManager.FindByNameAsync(dto.EMAIL);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {


                }

            }

            //if (buttonType == "Cancel")
            //{

            //    return RedirectToAction("login", "account");
            //}

            // If we got this far, something failed, redisplay form
            //return PartialView(PARTIAL_VIEW_FOLDER);
            return View(dto);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> ResetPassword(RESET_PASSWORD_VALIDATE_MODEL dto_resetPassword, RESET_PASSWORD_MODEL dto_newPassword, string buttonType)
        {

            if (dto_newPassword.NEW_PASSWORD == null)
            {
                var result = await service.ResetPasswordValidate(dto_resetPassword);
                if (result.STATUS)
                {
                    ViewBag.FirstCall = true;
                    dto_newPassword.EMAIL = dto_resetPassword.USER_NAME;
                    return View(dto_newPassword);
                }
                else
                {
                    return RedirectToAction("errorpage", "home");
                }
            }
            else
            {
                return RedirectToAction("errorpage", "home");

            }

            return View();

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> ResetPassword(RESET_PASSWORD_MODEL dto)
        {
            if (ModelState.IsValid)
            {
                var result = await service.ResetPassword(dto);
                if (result.STATUS)
                {
                    return RedirectToAction("login", "account");
                }
                else
                {
                    return RedirectToAction("login", "account");
                }

            }
            else
            {
                return View(dto);
            }

        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Session["logon"] = null;
            return RedirectToAction("index", "mainhome");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion


        public ActionResult FacebookLogin(string id,string email)
        {
            if (true)//เข้าด้วยเฟสครั้งแรก
            {
                return RedirectToAction("index", "mainhome");
            }
            else
            {
                return RedirectToAction("index", "mainhome");
            }
        }
    }
}