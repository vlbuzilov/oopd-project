using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class Schedule
	{
		[Key]
		public int Schedule_Item_ID { get; set; }
		public int Class_ID { get; set; }
		public DateTime Date_Time { get; set; }
		public bool IsAvailable { get; set; }

		[ForeignKey("Class_ID")]
		public virtual Class Class { get; set; }
	}
}

