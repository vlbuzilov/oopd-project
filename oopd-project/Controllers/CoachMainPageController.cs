﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
            var coachSchedule = GetCoachSchedule();
            return View("Index", coachSchedule);
        }

        public IActionResult Classes()
        {
            var availableClasses = GetAvailableClasses();
            return View("Classes", availableClasses);
        }
        
        public IActionResult TakeClass(int id)
        {
            AssureClassFromDB(id);
            var availableClasses = GetAvailableClasses();
            return View("Classes", availableClasses);
        }

        public IActionResult DeclineClass(int id, string name, string description)
        {
            DeclineClassFromDB(id, name, description);
            var coachSchedule = GetCoachSchedule();
            return View("Index", coachSchedule);
        }
        
        private Schedule GetCoachSchedule()
        {
            Schedule coachSchedule = new Schedule
            {
                _Schedule = new List<ScheduleItem>()
            };
            _coachId = HttpContext.Session.GetInt32("UserId");

            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                try
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {
                        var coachCurrentList = db.Schedule
                            .Include(s => s.Class)
                            .Where(s => s.Class.Coach_ID == _coachId && s.Date_Time > DateTime.Now && s.Date_Time < DateTime.Now.AddDays(7))
                            .ToList();

                        foreach (var item in coachCurrentList)
                        {
                            var scheduleItem = new ScheduleItem()
                            {
                                Class = new Class()
                                {
                                    Id = item.Class_ID,
                                    Name = item.Class.Class_Name,
                                    Duration = TimeSpan.FromHours(item.Class.Duration),
                                    Description = item.Class.Description,
                                    MaxNumberOfPeople = item.Class.Max_Participants
                                },
                                DateTime = item.Date_Time
                            };

                            coachSchedule._Schedule.Add(scheduleItem);
                        }
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return coachSchedule;
        }
        
        private List<Class> GetAvailableClasses()
        {
            List<Class> classes = new List<Class>();
            _coachId = HttpContext.Session.GetInt32("UserId");

            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                try
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {
                        var availableClasses = db.Classes.Where(c => c.Coach_ID == null).ToList();

                        foreach (var item in availableClasses)
                        {
                            var _class = new Class()
                            {
                                Id = item.Class_ID,
                                Name = item.Class_Name,
                                Duration = TimeSpan.FromHours(item.Duration),
                                Description = item.Description,
                                MaxNumberOfPeople = item.Max_Participants
                            };
                            classes.Add(_class);
                        }
                    }
                    scope.Complete();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return classes;
        }

        private void AssureClassFromDB(int id)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {
                        var existingClass = db.Classes.Find(id);
                        if (existingClass != null)
                        {
                            existingClass.Coach_ID = _coachId;
                            db.SaveChanges();
                        }
                        
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        
        private void DeclineClassFromDB(int id, string name, string description)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {
                        var existingClass = db.Classes.FirstOrDefault(c => c.Class_ID == id && c.Class_Name == name && c.Description == description);
                        if (existingClass != null)
                        {
                            existingClass.Coach_ID = null;
                            db.SaveChanges();
                        }
                        
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}

