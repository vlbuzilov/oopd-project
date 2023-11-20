using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using oopd_project.DBContext.DBModels;
using oopd_project.Models;
using Class = oopd_project.DBContext.DBModels.Class;
using Coach = oopd_project.Models.Coach;
using Schedule = oopd_project.Models.Schedule;
using Subscription = oopd_project.DBContext.DBModels.Subscription;

namespace oopd_project.Controllers
{
    public class MainPageController : Controller
    {
        public static int? ClientId;
        public IActionResult Index()
        {
            MakeSubscriptionsUnavailable();
            
            ClientId = HttpContext.Session.GetInt32("UserId");

            var coaches = GetCoaches();
            var subscriptions = GetSubscriptionTypes();
            
            var homePage = new HomePage
            {
                Subscriptions = subscriptions,
                Coaches = coaches
            };
            return View("Index", homePage);
        }

        public IActionResult GetSchedule()
        {
            var schedule = GetCurrentScheduleForClient();
            return View("Schedule", schedule);
        }

        private List<ClientScheduleItem> GetCurrentScheduleForClient()
        {
            var currentSchedule = new List<ClientScheduleItem>();

            using DataBaseContext db = new DataBaseContext();

            int subscriptionTypeId = db.Subscriptions
                .Where(s => s.Client_ID == ClientId)
                .Select(s => s.Subscription_Type_ID)
                .FirstOrDefault();

            if (subscriptionTypeId != 0)
            {
                var classesId = db.Subscription_Classes
                    .Where(s => s.Subscription_ID == subscriptionTypeId)
                    .Select(s => s.Class_ID).ToList();

                DateTime today = DateTime.Today;
                DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek); 
                DateTime endOfWeek = startOfWeek.AddDays(7);

                foreach (var classId in classesId)
                {
                    var clientScheduleItem = new ClientScheduleItem
                    {
                        Class = db.Classes
                            .Where(c => c.Class_ID == classId)
                            .Select(c => new Models.Class
                            {
                                Id = c.Class_ID,
                                Name = c.Class_Name,
                                Description = c.Description,
                                Duration = TimeSpan.FromHours(c.Duration),
                                MaxNumberOfPeople = c.Max_Participants,
                            })
                            .FirstOrDefault(),
        
                        DateTimes = db.Schedule
                            .Where(s => s.Class_ID == classId && s.Date_Time >= startOfWeek && s.Date_Time < endOfWeek)
                            .Select(s => s.Date_Time)
                            .Distinct()
                            .ToList()
                    };
                    if (clientScheduleItem.Class != null && clientScheduleItem.DateTimes.Count != 0)
                    {
                        currentSchedule.Add(clientScheduleItem);
                    }
                }


            }
            
            return currentSchedule;
        }
        
        public IActionResult Subscriptions()
        {
            GetSubscriptionTypeId();
            
            Subscriptions subscriptions = new Subscriptions
            {
                SubscriptionTypes = GetAllClassesFromSubscriptions(GetSubscriptionTypes()),
            };
            return View("Subscriptions", subscriptions);
        }

        public IActionResult BuySubscription(int subscriptionId)
        {
            return View("BuyingPage", GetSubscriptionTypeById(subscriptionId));
        }

        public IActionResult SetSubscription(int subscriptionId)
        {
            SetSubscriptionToClient(subscriptionId);
            return Subscriptions();
        }

        private void MakeSubscriptionsUnavailable()
        {
            using DataBaseContext db = new DataBaseContext();
            var subscriptionsToUpdate = db.Subscriptions
                .Where(s => s.Starting_Date.AddDays(30) <= DateTime.Today && s.isActive)
                .ToList();

            foreach (var subscription in subscriptionsToUpdate)
            {
                subscription.isActive = false;
            }

            db.SaveChanges();
        }
        
        private void SetSubscriptionToClient(int id)
        {
            using DataBaseContext db = new DataBaseContext();
            
            var newSubscription = new Subscription
            {
                Client_ID = ClientId ?? 0,
                Subscription_Type_ID = id,
                Starting_Date = DateTime.Today,
                isActive = true
            };

            db.Subscriptions.Add(newSubscription);
            db.SaveChanges();
        }

        private SubscriptionType GetSubscriptionTypeById(int id)
        {
            using DataBaseContext db = new DataBaseContext();
            SubscriptionType subscriptionTypeDTO = new SubscriptionType();

            var subscriptionType = db.Subscription_Types.FirstOrDefault(s => s.Subscription_Type_ID == id);

            if (subscriptionType != null)
            {
                subscriptionTypeDTO.Subscription_Type_Name = subscriptionType.Subscription_Type_Name;
                subscriptionTypeDTO.Subscription_Type_ID = id; 
                subscriptionTypeDTO.Price = subscriptionType.Price;
            }

            return subscriptionTypeDTO;
        }

        private void GetSubscriptionTypeId()
        {
            using DataBaseContext db = new DataBaseContext();

            int subscriptionId = db.Subscriptions
                .Where(s => s.Client_ID == ClientId && s.isActive == true)
                .Select(s => s.Subscription_Type_ID)
                .FirstOrDefault();
            
            HttpContext.Session.SetString("SubscriptionType",GetSubscriptionTypeById(subscriptionId).Subscription_Type_Name);

            if (subscriptionId != 0)
            {
                var startingDate = db.Subscriptions
                    .Where(s => s.Client_ID == ClientId)
                    .Select(s => s.Starting_Date)
                    .FirstOrDefault();
                
                HttpContext.Session.SetString("SubscriptionStartingDate",startingDate.ToString());
            }
        }
        
        private List<SubscriptionType> GetAllClassesFromSubscriptions(List<SubscriptionType> subscriptionTypes)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                var subscription_classses = db.Subscription_Classes.ToList();

                foreach (var subscriptionType in subscriptionTypes)
                {
                    var classes = new List<Models.Class>();

                    foreach (var subscriptionClass in subscription_classses)
                    {
                        if (subscriptionType.Subscription_Type_ID == subscriptionClass.Subscription_ID)
                        {
                            var _class = db.Classes.FirstOrDefault(c => c.Class_ID == subscriptionClass.Class_ID);

                            if (_class != null)
                            {
                                Models.Class classDTO = new Models.Class
                                {
                                    Id = _class.Class_ID,
                                    Name = _class.Class_Name,
                                    Description = _class.Description,
                                    Duration = TimeSpan.FromHours(_class.Duration),
                                    MaxNumberOfPeople = _class.Max_Participants,
                                    
                                };

                                classes.Add(classDTO);
                            }
                        }
                    }

                    subscriptionType.Duration = 30;
                    subscriptionType.Classes = classes;
                }

                return subscriptionTypes;
            }
        }

        private List<SubscriptionType> GetSubscriptionTypes()
        {
            var subscriptionTypes = new List<SubscriptionType>();
            using DataBaseContext db = new DataBaseContext();
            
            var currentSubscriptionTypes = db.Subscription_Types.ToList();
            foreach (var item in currentSubscriptionTypes)
            {
                var subscriptionType = new SubscriptionType
                {
                    Subscription_Type_ID = item.Subscription_Type_ID,
                    Subscription_Type_Name = item.Subscription_Type_Name,
                    Price = item.Price,
                };
                if (item.isAvailable)
                {
                    subscriptionTypes.Add(subscriptionType);
                }
            }

            return subscriptionTypes;
        }
        
        private List<Coach> GetCoaches()
        {
            var coaches = new List<Coach>();

            using DataBaseContext db = new DataBaseContext();

            var coachList = db.Coaches.ToList();
            foreach (var item in coachList)
            {
                var coach = new Coach
                {
                    FirstName = item.Name,
                    LastName = item.Last_Name,
                    Specialization = item.Specialization
                };
                
                coaches.Add(coach);
            }

            return coaches;
        }
        
    }
}

