using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using oopd_project.Models;

namespace oopd_project.Controllers
{
    public class ClientRegistrationController : Controller
    {
        public IActionResult SignUp()
        {
            return View("Registration");
        }

        [HttpPost]
        public IActionResult Register(Client client)
        {
            bool isClientExists = IsClientExists(client);
            bool isConfirmedPassword = (client.Password == client.ConfirmationPassword) ? true : false;
            bool isPasswordUniqe = IsPasswordUniqe(client);

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
            if(!isClientExists && isPasswordUniqe && isConfirmedPassword)
            {
                AddNewClient(client);
                return RedirectToAction("Login", "Login");
            }
            return View("Registration", client);
        }

        private void AddNewClient(Client client)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {
                        var newUser = new DBContext.DBModels.User
                        {
                            User_Role_ID = 3,
                            Email = client.Email,
                            Password = client.Password,
                            Phone_Number = client.PhoneNumber,
                            Birthdate = client.Birthdate
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();

                        var newClient = new DBContext.DBModels.Client
                        {
                            Name = client.FirstName,
                            Last_Name = client.LastName,
                            Registration_Date = DateTime.Today,
                            Subscription_ID = null,
                            User = newUser
                        };

                        db.Clients.Add(newClient);
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
        
        private bool IsClientExists(Client client)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var foundClient = db.Users.FirstOrDefault(user => user.Email == client.Email);

                return (foundClient != null) ? true : false;
            }
        }

        private bool IsPasswordUniqe(Client client)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var foundPassword = db.Users.FirstOrDefault(user => user.Password == client.Password);

                return (foundPassword == null) ? true : false;
            }
        }
    }
}

