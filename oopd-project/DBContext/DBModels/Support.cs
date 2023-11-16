using System;
using System.ComponentModel.DataAnnotations;

namespace oopd_project.DBContext.DBModels
{
	public class Support
	{
        [Key]
        public int Question_ID { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
	}
}

