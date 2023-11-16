using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class Class
	{
        [Key]
        public int Class_ID { get; set; }
		public int Coach_ID { get; set; }
		public string Class_Name { get; set; }
		public double Duration { get; set; }
		public string Description { get; set; }
		public int Max_Participants { get; set; }

        [ForeignKey("Coach_ID")]
        public virtual Coach Coach { get; set; }
    }
}

