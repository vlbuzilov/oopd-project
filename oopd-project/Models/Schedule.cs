using System;
namespace oopd_project.Models
{
	public class Schedule
	{
		public List<ScheduleItem> _Schedule { get; set; }
		public List<Class> _AvailableClasses { get; set; }
		public AddToScheduleModel _ClassToAdd { get; set; }
	}
}

