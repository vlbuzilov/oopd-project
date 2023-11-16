using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oopd_project.Models;

namespace oopd_project.Controllers
{
    public class LoginController : Controller
    {

        [HttpGet]
        public IActionResult Login()
        {
            var loginModel = new Login();
            return View(loginModel);
        }

        [HttpPost]
        public IActionResult SubmitLogin(Login login)
        {
            int userType = FindUserAndReturnType(login);
            if (userType != -1 && userType != -2)
            {
                HttpContext.Session.SetString("Email", login.Email);
                HttpContext.Session.SetString("Password", login.Password);
                HttpContext.Session.SetInt32("UserType", userType);
                HttpContext.Session.SetInt32("UserId", GetId(login));
                switch (userType)
                {
                    case 1:
                        if (string.IsNullOrEmpty(login.SecretKey))
                        {
                            login.SecretKey = GetSecretKey(login);
                            login.IsAdmin = true;
                            ViewData["ShowSecretKeyField"] = true;
                            ViewData["SecretKey"] = login.SecretKey;
                            return View("Login", login);
                        }
                        else
                        {
                            if (login.SecretKey == GetSecretKey(login))
                            {
                                HttpContext.Session.SetInt32("IsAuthorized", 1);
                                return RedirectToAction("Index", "AdministratorMainPage");
                            }
                            return View("Login", login);
                        }
                    case 2:
                        HttpContext.Session.SetInt32("IsAuthorized", 1);
                        return RedirectToAction("Index", "CoachMainPage");
                    case 3:
                        HttpContext.Session.SetInt32("IsAuthorized", 1);
                        return RedirectToAction("Index", "MainPage");
                }
                return RedirectToAction("Login", "Login");
            }
            else if(userType == -1)
            {
                ModelState.AddModelError(string.Empty, "Wrong credentials was inputed. Try again.");
                return View("Login", login);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "You are new client in our gym, please sign in:)");
                return View("Login", login);
            }
        }

        private int FindUserAndReturnType(Login login)
        {
            int userType = -1;

            using (DataBaseContext db = new DataBaseContext())
            {
                var foundUser = db.Users.FirstOrDefault(user => user.Email == login.Email);

                if (foundUser == null)
                {
                    userType = -2;
                }
                else if(foundUser.Password == login.Password && foundUser.Email == login.Email)
                {
                    userType = foundUser.User_Role_ID;
                }
            }

            return userType;
        }

        private string GetSecretKey(Login login)
        {
            string secretKey = string.Empty;

            using (DataBaseContext db = new DataBaseContext())
            {
                var foundUser = db.Administrators
                    .Include(admin => admin.User)
                    .ThenInclude(user => user.UserRole)
                    .Where(admin => admin.User.Email == login.Email && admin.User.Password == login.Password)
                    .FirstOrDefault();

                if (foundUser != null)
                {
                    secretKey = foundUser.Secret_Key;
                }
            }

            return secretKey;
        }

        private int GetId(Login login)
        {
            int id = 0;

            using (DataBaseContext db = new DataBaseContext())
            {
                var foundId = db.Users
                    .Where(user => user.Email == login.Email && user.Password == login.Password)
                    .FirstOrDefault();
                if(foundId != null)
                {
                    id = foundId.User_ID;
                }
            }

            return id;
        }
    }
}

