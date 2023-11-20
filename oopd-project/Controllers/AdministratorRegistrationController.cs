using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace oopd_project
{
    public class AdministratorRegistrationController : Controller
    {
        public IActionResult AdminSignUp()
        {
           return View("AdministratorRegistration");
        }

        [HttpPost]
        public IActionResult AdministratorRegister(Models.Administrator admin)
        {
            bool isClientExists = IsAdminExists(admin);
            bool isConfirmedPassword = (admin.Password == admin.ConfirmationPassword) ? true : false;
            bool isPasswordUniqe = IsPasswordUniqe(admin);
            bool isSecretKeyUniqe = IsSecretKeyUnique(admin);

            if (isClientExists)
            {
                ModelState.AddModelError(string.Empty, "These credential are already used, try other.");
            }
            if (!isPasswordUniqe)
            {
                ModelState.AddModelError(string.Empty, "Passwords are not unique, try other.");
            }
            if (!isConfirmedPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords are different.");
            }
            if (!isSecretKeyUniqe)
            {
                ModelState.AddModelError(string.Empty, "Secret key is not unique, try another.");
            }
            if (!isClientExists && isPasswordUniqe && isConfirmedPassword && isSecretKeyUniqe)
            {
                AddNewAdmin(admin);
                return RedirectToAction("Login", "Login");
            }
            return View("AdministratorRegistration", admin);
        }

        private void AddNewAdmin(Models.Administrator admin)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {
                        var newUser = new DBContext.DBModels.User
                        {
                            User_Role_ID = 1,
                            Email = admin.Email,
                            Password = admin.Password,
                            Phone_Number = admin.PhoneNumber,
                            Birthdate = admin.Birthdate
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();

                        var newAdmin = new DBContext.DBModels.Administrator
                        {
                            Name = admin.FirstName,
                            Last_Name = admin.LastName,
                            Secret_Key = admin.SecretKey,
                            User = newUser
                        };

                        db.Administrators.Add(newAdmin);
                        db.SaveChanges();

                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private bool IsAdminExists(Models.Administrator admin)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var foundAdmin = db.Users.FirstOrDefault(user => user.Email == admin.Email);

                return (foundAdmin != null) ? true : false;
            }
        }

        private bool IsPasswordUniqe(Models.Administrator admin)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var foundPassword = db.Users.FirstOrDefault(user => user.Password == admin.Password);

                return (foundPassword == null) ? true : false;
            }
        }

        private bool IsSecretKeyUnique(Models.Administrator administrator)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var foundSecretKey = db.Administrators.FirstOrDefault(admin => admin.Secret_Key == administrator.SecretKey);

                return (foundSecretKey == null) ? true : false;
            }
        }
    }
}


