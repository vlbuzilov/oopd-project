using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace oopd_project.Controllers
{
    public class ProfileController : Controller
    {
        private static string _email;
        private static string _password;
        private static int _userType;

        public IActionResult Profile()
        {
            _email = HttpContext.Session.GetString("Email");
            _password = HttpContext.Session.GetString("Password");
            _userType = (int)HttpContext.Session.GetInt32("UserType");

            switch (_userType)
            {
                case 1: 
                    var adminProfileModel = GetAdminProfileModel(_email, _password);
                    return View("AdminProfile", adminProfileModel);

                case 2:
                    var coachProfileModel = GetCoachProfileModel(_email, _password);
                    return View("CoachProfile", coachProfileModel);

                case 3:
                    var clientProfileModel = GetClientProfileModel(_email, _password);
                    return View("ClientProfile", clientProfileModel);

                default:
                    return RedirectToAction("Profile", "Profile"); 
            }
        }

        [HttpPost]
        public IActionResult SaveChanges(string jsonResponse)
        {
            var editedData = PrepareEditedDate(jsonResponse);

            switch (_userType)
            {
                case 1:
                    UpdateAdmin(editedData);
                    return View("AdminProfile");

                case 2:
                    UpdateCoach(editedData);
                    return View("CoachProfile");

                case 3:
                    UpdateClient(editedData);
                    return View("ClientProfile");

                default:
                    return RedirectToAction("Profile", "Profile");
            }
        }       

        private void UpdateAdmin(Models.EditedDataDTO editedAdmin)
        {
            DBContext.DBModels.Administrator admin = null;

            using (DataBaseContext db = new DataBaseContext())
            {
                var user = db.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefault(u => u.Email == _email && u.Password == _password);

                if (user != null)
                {
                    admin = db.Administrators.FirstOrDefault(admin => admin.User.User_ID == user.User_ID);
                }
                if (admin != null)
                {
                    admin.Name = editedAdmin.Name;
                    admin.Last_Name = editedAdmin.LastName;
                    user.Phone_Number = editedAdmin.PhoneNumber;
                    user.Birthdate = editedAdmin.Birthdate;
                    db.SaveChanges();
                }
            }
        }

        private void UpdateCoach(Models.EditedDataDTO editedCoach)
        {
            DBContext.DBModels.Coach coach = null;

            using (DataBaseContext db = new DataBaseContext())
            {
                var user = db.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefault(u => u.Email == _email && u.Password == _password);

                if (user != null)
                {
                    coach = db.Coaches.FirstOrDefault(coach => coach.User.User_ID == user.User_ID);
                }
                if (coach != null)
                {
                    coach.Name = coach.Name;
                    coach.Last_Name = editedCoach.LastName;
                    coach.Specialization = editedCoach.Specialization;
                    user.Phone_Number = editedCoach.PhoneNumber;
                    user.Birthdate = editedCoach.Birthdate;
                    db.SaveChanges();
                }
            }
        }

        private void UpdateClient(Models.EditedDataDTO editedClient)
        {
            DBContext.DBModels.Client client = null;

            using (DataBaseContext db = new DataBaseContext())
            {
                var user = db.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefault(u => u.Email == _email && u.Password == _password);

                if (user != null)
                {
                    client = db.Clients.FirstOrDefault(client => client.User.User_ID == user.User_ID);
                }
                if (client != null)
                {
                    client.Name = editedClient.Name;
                    client.Last_Name = editedClient.LastName;
                    user.Phone_Number = editedClient.PhoneNumber;
                    user.Birthdate = editedClient.Birthdate;
                    db.SaveChanges();
                }
            }
        }

        private Models.Administrator GetAdminProfileModel(string email, string password)
        {
            DBContext.DBModels.Administrator admin = null;
            Models.Administrator adminDTO = new Models.Administrator();

            using (DataBaseContext db = new DataBaseContext())
            {
                var user = db.Users
                    .Include(u => u.UserRole) 
                    .FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    admin = db.Administrators.FirstOrDefault(admin => admin.User.User_ID == user.User_ID);
                }
                if (admin != null)
                {
                    adminDTO.UserId = user.User_ID;
                    adminDTO.FirstName = admin.Name;
                    adminDTO.LastName = admin.Last_Name;
                    adminDTO.Email = user.Email;
                    adminDTO.PhoneNumber = user.Phone_Number;
                    adminDTO.Birthdate = user.Birthdate;
                }

                return adminDTO;
            }
        }

        private Models.Coach GetCoachProfileModel(string email, string password)
        {
            DBContext.DBModels.Coach coach = null;
            Models.Coach coachDTO = new Models.Coach();

            using (DataBaseContext db = new DataBaseContext())
            {
                var user = db.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    coach = db.Coaches.FirstOrDefault(coach => coach.User.User_ID == user.User_ID);
                }
                if (coach != null)
                {
                    coachDTO.UserId = user.User_ID;
                    coachDTO.FirstName = coach.Name;
                    coachDTO.LastName = coach.Last_Name;
                    coachDTO.Specialization = coach.Specialization;
                    coachDTO.Email = user.Email;
                    coachDTO.PhoneNumber = user.Phone_Number;
                    coachDTO.Birthdate = user.Birthdate;
                }

                return coachDTO;
            }
        }

        private Models.Client GetClientProfileModel(string email, string password)
        {
            DBContext.DBModels.Client client = null;
            Models.Client clientDTO = new Models.Client();

            using (DataBaseContext db = new DataBaseContext())
            {
                var user = db.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    client = db.Clients.FirstOrDefault(client => client.User.User_ID == user.User_ID);
                }
                if (client != null)
                {
                    clientDTO.UserId = user.User_ID;
                    clientDTO.FirstName = client.Name;
                    clientDTO.LastName = client.Last_Name;
                    clientDTO.RegisteredDate = client.Registration_Date;
                    clientDTO.Email = user.Email;
                    clientDTO.PhoneNumber = user.Phone_Number;
                    clientDTO.Birthdate = user.Birthdate;
                }

                return clientDTO;
            }
        }

        private Models.EditedDataDTO PrepareEditedDate(string jsonResponse)
        {
            var convertedJson = JsonConvert.DeserializeObject<Models.EditedDataDTO>(jsonResponse);

            if (convertedJson.Name != null)
            {
                string[] name = convertedJson.Name.Split(' ');
                convertedJson.Name = name[0];
                if (name[1] != null)
                {
                    convertedJson.LastName = name[1];
                }
            }

            return convertedJson;
        }
    }
}


