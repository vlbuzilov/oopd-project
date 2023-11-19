using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oopd_project.Models;

namespace oopd_project.Controllers
{
    public class AdministratorMainPageController : Controller
    {
        public IActionResult Index()
        {
            var schedule = GetCurrentClubSchedule();
            schedule._AvailableClasses = GetAvailableClasses();
            return View("Index", schedule);
        }
        
        public IActionResult Classes()
        {
            return View("Classes");
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
            schedule._AvailableClasses = GetAvailableClasses();
            return Index();
        }

        private List<Class> GetAvailableClasses()
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
                        Name = item.Class_Name
                    };  
                    availableClasses.Add(availableClass);
                }
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

                    scheduleItem.Class.CurrentNumberOfPeople = db.Clients_Classes
                        .Where(cc => cc.Schedule_Item_Id == item.Schedule_Item_ID)
                        .Select(cc => cc.Current_Clients_Number)
                        .FirstOrDefault();


                    Schedule._Schedule.Add(scheduleItem);
                }

                return Schedule;
            }
        }
        
        
    }
}

