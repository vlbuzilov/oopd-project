using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oopd_project.DBContext.DBModels;
using oopd_project.Models;
using Class = oopd_project.Models.Class;
using Schedule = oopd_project.Models.Schedule;

namespace oopd_project.Controllers
{
    public class AdministratorMainPageController : Controller
    {
        public IActionResult Index()
        {
            ClearUnavailableScheduleItems();
            var schedule = GetCurrentClubSchedule();
            schedule._AvailableClasses = GetAvailableClassesForSchedule();
            return View("Index", schedule);
        }
        
        public IActionResult Classes()
        {
            var availableClasses = GetAvailableClasses();
            var classes = new Classes();
            classes.AvailableClasses = availableClasses;
            return View("Classes", classes);
        }

        public IActionResult Subscriptions()
        {
            var availableSubscriptionTypes = GetSubscriptionTypes();
            var availableClasses = GetAvailableClasses();
            var subscriptions = new Subscriptions
            {
                SubscriptionTypes = availableSubscriptionTypes,
                AvailableClasses = availableClasses
            };
           
            return View("Subscriptions", subscriptions);
        }

        public IActionResult CreateSubscription(Subscriptions subscriptions, int[] SelectedClasses)
        {
            subscriptions.SubscriptionTypeToAdd.ClassesInSubscription = SelectedClasses.ToList();

            AddNewSubscriptionType(subscriptions);
            
            return Subscriptions();
        }

        public IActionResult AddClass(Classes classes)
        {
            using DataBaseContext db = new DataBaseContext();
            DBContext.DBModels.Class classToAdd = new DBContext.DBModels.Class
            {
                Class_Name = classes.ClassToAdd.Name,
                Description = classes.ClassToAdd.Description,
                Duration = classes.ClassToAdd.Duration.TotalHours,
                Max_Participants = classes.ClassToAdd.MaxNumberOfPeople
            };
            db.Classes.Add(classToAdd);
            db.SaveChanges();
            return Classes();
        }
        
        public IActionResult DeclineClass(int scheduleItemId)
        {
            DeclineScheduleItemFromDB(scheduleItemId);
            return Index();
        }

        public IActionResult AddToSchedule(Schedule updatedSchedule)
        {
            if (updatedSchedule._ClassToAdd != null)
            {
                using (DataBaseContext db = new DataBaseContext())
                {
                    var date = updatedSchedule._ClassToAdd.SelectedDate;
                    var time = updatedSchedule._ClassToAdd.SelectedTime;

                    var scheduleItem = new DBContext.DBModels.Schedule
                    {
                        Class_ID = updatedSchedule._ClassToAdd.ClassId,
                        Date_Time = date.Add(time),
                        IsAvailable = true
                    };

                    db.Schedule.Add(scheduleItem);
                    db.SaveChanges();
                }
            }
            var schedule = GetCurrentClubSchedule();
            schedule._AvailableClasses = GetAvailableClassesForSchedule();
            return Index();
        }

        public IActionResult DeleteClass(int classId)
        {
            DeleteClassFromDB(classId);
            
            return Classes();
        }

        public IActionResult DeleteSubscription(int subscriptionTypeId)
        {
            DeleteSubscriptionFromDB(subscriptionTypeId);
            return Subscriptions();
        }

        private List<Class> GetAvailableClassesForSchedule()
        {
            List<Class> availableClasses = new List<Class>();

            using DataBaseContext db = new DataBaseContext();
            var currentClassesList = db.Classes.ToList();
            foreach (var item in currentClassesList)
            {
                if (item.Coach_ID != null)
                {
                    var availableClass = new Class
                    {
                        Id = item.Class_ID,
                        Name = item.Class_Name,
                        MaxNumberOfPeople = item.Max_Participants,
                        Description = item.Description,
                        Duration = TimeSpan.FromHours(item.Duration),
                    };  
                    availableClasses.Add(availableClass);
                }
            }

            return availableClasses;
        }
        
        private List<Class> GetAvailableClasses()
        {
            List<Class> availableClasses = new List<Class>();

            using DataBaseContext db = new DataBaseContext();
            var currentClassesList = db.Classes.ToList();
            foreach (var item in currentClassesList)
            {
                    var availableClass = new Class
                    {
                        Id = item.Class_ID,
                        Name = item.Class_Name,
                        MaxNumberOfPeople = item.Max_Participants,
                        Description = item.Description,
                        Duration = TimeSpan.FromHours(item.Duration),
                    };  
                    availableClasses.Add(availableClass);
            }

            return availableClasses;
        }
        
        private void DeclineScheduleItemFromDB(int id)
        {
            using (DataBaseContext db = new DataBaseContext())
            {
                try
                {
                    var existingClass = db.Schedule.Find(id);
                    if (existingClass != null)
                    {
                        existingClass.IsAvailable = false;
                        db.SaveChanges();
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        
        private Schedule GetCurrentClubSchedule()
        {
            Schedule Schedule = new Schedule
            {
                _Schedule = new List<ScheduleItem>()
            };

            using (DataBaseContext db = new DataBaseContext())
            {
                var CurrentScheduleList = db.Schedule
                    .Include(s => s.Class).ThenInclude(@class => @class.Coach)
                    .Where(s => s.IsAvailable == true)
                    .OrderBy(s => s.Date_Time)
                    .ToList();

                foreach (var item in CurrentScheduleList)
                {
                    var scheduleItem = new ScheduleItem()
                    {
                        Class = new Class()
                        {
                            Name = item.Class.Class_Name,
                            Duration = TimeSpan.FromHours(item.Class.Duration),
                            Description = item.Class.Description,
                            MaxNumberOfPeople = item.Class.Max_Participants
                        },
                        ScheduleItemId = item.Schedule_Item_ID,
                        DateTime = item.Date_Time
                    };

                    Schedule._Schedule.Add(scheduleItem);
                }

                return Schedule;
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
                    IsAvailable = item.isAvailable
                };  
                subscriptionTypes.Add(subscriptionType);
            }

            return subscriptionTypes;
        }

        private void AddNewSubscriptionType(Subscriptions subscriptions)
        {
            var newType = new Subscription_Type
            {
                Subscription_Type_Name = subscriptions.SubscriptionTypeToAdd.Subscription_Type_Name,
                Price = subscriptions.SubscriptionTypeToAdd.Price,
                isAvailable = true,
                Duration = 30
            };
            
            using DataBaseContext db = new DataBaseContext();
            db.Subscription_Types.Add(newType);
            db.SaveChanges();

            int newSubscriptionTypeId = db.Subscription_Types
                .Where(t => t.Subscription_Type_Name == newType.Subscription_Type_Name)
                .Select(t => t.Subscription_Type_ID)
                .FirstOrDefault();
            
            foreach (var classId in subscriptions.SubscriptionTypeToAdd.ClassesInSubscription)
            {
                try
                {
                    db.Subscription_Classes.Add(new SubscriptionClass
                    {
                        Subscription_ID = newSubscriptionTypeId,
                        Class_ID = classId
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            db.SaveChanges();
        }

        private void ClearUnavailableScheduleItems()
        {
            using DataBaseContext db = new DataBaseContext();

            var UnavailableScheduleItems = db.Schedule
                .Where(s => s.IsAvailable == false && s.Date_Time < DateTime.Now)
                .ToList();

            if (UnavailableScheduleItems != null && UnavailableScheduleItems.Count != 0)
            {
                db.Schedule.RemoveRange(UnavailableScheduleItems);
                db.SaveChanges();   
            }
        }

        private void DeleteClassFromDB(int classId)
        {
            using DataBaseContext db = new DataBaseContext();

            var classToDelete = db.Classes.FirstOrDefault(c => c.Class_ID == classId);
            db.Remove(classToDelete);
            db.SaveChanges();
        }

        private void DeleteSubscriptionFromDB(int id)
        {
            using DataBaseContext db = new DataBaseContext();

            var subscriptionRelations = db.Subscription_Classes
                .Where(s => s.Subscription_ID == id)
                .ToList();
            
            if (subscriptionRelations != null && subscriptionRelations.Count != 0)
            {
                db.Subscription_Classes.RemoveRange(subscriptionRelations);

                var subscriptionTypeToRemove = db.Subscription_Types
                    .FirstOrDefault(s => s.Subscription_Type_ID == id);

                if (subscriptionTypeToRemove != null)
                {
                    db.Subscription_Types.Remove(subscriptionTypeToRemove);
                    db.SaveChanges();
                }
            }
        }
    }
}

