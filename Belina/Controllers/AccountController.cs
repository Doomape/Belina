﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Belina.Models;
using System.Web.Routing;

namespace Belina.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        BelinaEntities2 db = new BelinaEntities2();
        
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult ShowAdmins()
        {
            var all_admins = db.Administrator.ToList();
            List<Admin> admins = new List<Admin>();
            foreach(var admin in all_admins)
            {
                Admin new_admin = new Admin()
                {
                    user_id = admin.user_id,
                    user_name = admin.user_name,
                    user_email = admin.user_email
                };
                admins.Add(new_admin);
            }
            return View(admins);
        }

        public ActionResult EditAdmin(int id)
        {
            var admin = db.Administrator.FirstOrDefault(x => x.user_id == id);
            EditAdmin edit_admin = new EditAdmin()
            {
                ID = id,
                Mail = admin.user_email,
                Name = admin.user_name
            };
            return View(edit_admin);
        }

        public ActionResult SaveAdmin(EditAdmin admin_model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var admin = db.Administrator.FirstOrDefault(x => x.user_id == admin_model.ID);
                    if (admin != null)
                    {
                        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                        byte[] bs = System.Text.Encoding.UTF8.GetBytes(admin_model.NewPassword);
                        bs = md5.ComputeHash(bs);

                        admin.user_email = admin_model.Mail;
                        admin.user_name = admin_model.Name;
                        admin.user_pass = bs;

                        db.SaveChanges();
                    }

                    return RedirectToAction("Index", "Admin");
                }
                catch(Exception ex)
                {
                    return View("EditAdmin", admin_model);
                }
            }
            return View("EditAdmin", admin_model);
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    var user = (from x in db.Administrator where x.user_name == model.UserName select x).FirstOrDefault();

                    if (user == null)
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("", ("Nema takov korisnik."));
                        return View(model);
                    }

                    FormsService.SignIn(model.UserName, model.RememberMe);

                    return Redirect("/Admin/Index");
                }
                else
                {
                    ModelState.Clear();
                    ModelState.AddModelError("", ("Pogresni podatoci."));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        public ActionResult LogOff()
        {
            //WebSecurity.Logout();
            FormsService.SignOut();
            return RedirectToAction("Index", "Admin");
        }

        //
        // GET: /Account/Register

        [Authorize]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public JsonResult RegisterUser(RegisterModel regmodel)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                MembershipCreateStatus createStatus;
                // Attempt to register the user
                if (regmodel.Password == regmodel.ConfirmPassword)
                {
                    createStatus = MembershipService.CreateUser(regmodel.UserName, regmodel.Password, regmodel.Email);
                }
                else
                {
                    res["msg"] = "Не се исти лозинките";
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                    

                if (createStatus == MembershipCreateStatus.Success)
                {
                    res["msg"] = "OK";
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    res["msg"] = AccountValidation.ErrorCodeToString(createStatus);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                res["msg"] = "Грешка!";
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
