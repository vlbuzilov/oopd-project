using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace oopd_project.Controllers
{
    public class CoachRegistrationController : Controller
    {
        public IActionResult CoachSignUp()
        {
            return View("CoachRegistration");
        }

        [HttpPost]
        public IActionResult CoachRegister(Models.Coach coach)
        {
            bool isClientExists = IsCoachExists(coach);
            bool isConfirmedPassword = (coach.Password == coach.ConfirmationPassword) ? true : false;
            bool isPasswordUniqe = IsPasswordUniqe(coach);
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
            if (!isClientExists && isPasswordUniqe && isConfirmedPassword)
            {
                AddNewCoach(coach);
                return RedirectToAction("Login", "Login");
            }
            return View("CoachRegistration", coach);
        }

        private void AddNewCoach(Models.Coach coach)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                try
                {
                var newUser = new DBContext.DBModels.User
                {
                    User_Role_ID = 2,
                    Email = coach.Email,
                    Password = coach.Password,
                    Phone_Number = coach.PhoneNumber,
                    Birthdate = coach.Birthdate
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                var newCoach = new DBContext.DBModels.Coach
                {
                    Name = coach.FirstName,
                    Last_Name = coach.LastName,
                    Specialization = coach.Specialization,
                    Experience = coach.Experience,
                    User = newUser
                };
                
                    db.Coaches.Add(newCoach);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw new Exception($"Error occurred while trying to add new admin to database: {ex.Message}");
                }
            }
        }

        private bool IsCoachExists(Models.Coach coach)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var foundAdmin = db.Users.FirstOrDefault(user => user.Email == coach.Email);

                return (foundAdmin != null) ? true : false;
            }
        }

        private bool IsPasswordUniqe(Models.Coach coach)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var foundPassword = db.Users.FirstOrDefault(user => user.Password == coach.Password);

                return (foundPassword == null) ? true : false;
            }
        }
    }
}


