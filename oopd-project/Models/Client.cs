using System;
namespace oopd_project.Models
{
	public class Client : User
	{
		public DateTime RegisteredDate { get; set; }
		public Subscription Subscription { get; set; }
        public List<ScheduleItem> PersonalSchedule { get; set; }
	}
}

