using System;
namespace oopd_project.Models
{
	public class Coach : User
	{
		public string Specialization { get; set; }
		public int Experience { get; set; }
		public List<Schedule> MyClasses { get; set; }
	}
}

