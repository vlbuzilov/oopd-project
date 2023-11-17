using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oopd_project.Models;

namespace oopd_project.Controllers
{
    public class CoachMainPageController : Controller
    {
        private static int? _coachId;

        public IActionResult Index()
        {
            var coachSchedule = getCoachSchedule();
            return View("Index", coachSchedule);
        }
        
        private Schedule getCoachSchedule()
        {
            Schedule coachSchedule = new Schedule
            {
                _Schedule = new List<ScheduleItem>()
            };
            _coachId = HttpContext.Session.GetInt32("UserId");
            using (DataBaseContext db = new DataBaseContext())
            {
                var coachCurrentList = db.Schedule
                    .Include(s => s.Class)
                    .Where(s => s.Class.Coach_ID == _coachId.Value && s.Date_Time > DateTime.Now && s.Date_Time < DateTime.Now.AddDays(7))  
                    .ToList();

                foreach (var item in coachCurrentList)
                {
                    var scheduleItem = new ScheduleItem()
                    {
                        Class = new Class()
                        {
                            Id = item.Schedule_Item_ID,
                            Name = item.Class.Class_Name,
                            Duration =TimeSpan.FromHours(item.Class.Duration),
                            Description = item.Class.Description,
                            MaxNumberOfPeople = item.Class.Max_Participants
                        },
                        DateTime = item.Date_Time
                    };
                    
                    scheduleItem.Class.CurrentNumberOfPeople = db.Clients_Classes
                        .Where(cc => cc.Schedule_Item_Id == item.Schedule_Item_ID)
                        .Select(cc => cc.Current_Clients_Number)
                        .FirstOrDefault(); 

                    
                    coachSchedule._Schedule.Add(scheduleItem);
                }

                return coachSchedule;
            }
        }
       
    }
}

