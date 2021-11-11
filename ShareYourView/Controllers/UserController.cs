using ShareYourView.Hash;
using ShareYourView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ShareYourView.Controllers
{
    public class UserController : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        //Registration POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "user_IsVerified,user_ActivationCode")]UserDetail userDetail)
        {
            bool Status = false;
            string Message = "";
            //
            //Model Validation
            if (ModelState.IsValid)
            {
                #region //Email exist already or not
                var isExist = isEmailExist(userDetail.user_Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exists. Please Try Again");
                }
                #endregion

                #region //Activation Code generate
                userDetail.user_ActivationCode = Guid.NewGuid();
                #endregion

                #region //Password Hash - Security
                    userDetail.user_Password = Crypto.Hash(userDetail.user_Password);
                    userDetail.Confirm_Password = Crypto.Hash(userDetail.Confirm_Password);
                #endregion

                userDetail.user_IsVerified = false;

                #region //Save data to database
                    using (shareYourView_DBEntities db = new shareYourView_DBEntities())
                    {
                        db.UserDetails.Add(userDetail);
                        db.SaveChanges();

                        //Send email to User
                        SendVerifactionLinkEmail(userDetail.user_Email, userDetail.user_ActivationCode.ToString());
                        Message = "Registration successfully done. Account verification link has been send to your email address. Please go and verify your account: " + userDetail.user_Email;
                        Status = true;
                    }
                #endregion

            }
            else
            {
                Message = "Invalid Request";
            }

            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(userDetail);
        }

        //Verify User
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false;

                var v = db.UserDetails.Where(a => a.user_ActivationCode == new Guid(id)).FirstOrDefault();
                if(v != null)
                {
                    v.user_IsVerified = true;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string Message = "";

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var v = db.UserDetails.Where(a => a.user_Username == login.user_Username).FirstOrDefault();
                if(v != null)
                {
                    if(string.Compare(Crypto.Hash(login.user_Password), v.user_Password) == 0)
                    {
                        //int timeout = login.RememberMe ? 525600 : 1;
                        int timeout = login.RememberMe ? 5 : 1;
                        var ticket = new FormsAuthenticationTicket(login.user_Username, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if(Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        Message = "Invalid Password provided";
                    }
                }
                else
                {
                    Message = "Imvalid credentials provided";
                }
            }

                ViewBag.Message = Message;
            return View(login);
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
                

        [NonAction]
        public bool isEmailExist(string email)
        {
            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var v = db.UserDetails.Where(a => a.user_Email == email).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerifactionLinkEmail(string email, string verifactionCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + verifactionCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("share.your.view.ind@gmail.com", "Share Your View");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "24TinTin24";
            string subject = "Your account is successfully created!";

            string body = "<br/><br/>Your account has been craeted succefully. We are excited to have you on board. Please click on the link below to verify yout account, so that you can GO and Share Your Amazing Pictures" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
             smtp.Send(message);

        }

    }   

}