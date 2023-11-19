using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using oopd_project.Models;
using Class = oopd_project.DBContext.DBModels.Class;

namespace oopd_project.Controllers
{
    public class MainPageController : Controller
    {
        public IActionResult Index()
        {
            var coaches = GetCoaches();
            var subscriptions = GetSubscriptionTypes();
            
            var homePage = new HomePage
            {
                Subscriptions = subscriptions,
                Coaches = coaches
            };
            return View("Index", homePage);
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

