using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace oopd_project.Controllers
{
    public class CoachMainPageController : Controller
    {
        public IActionResult Index()
        {
            
            return View("Index");
        }

       
    }
}

