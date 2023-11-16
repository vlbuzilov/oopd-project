using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class Coach
	{
        [Key]
        public int Coach_ID { get; set; }
		public string Name { get; set; }
		public string Last_Name { get; set; }
		public string Specialization { get; set; }
		public int Experience { get; set; }

        [ForeignKey("Coach_ID")]
        public virtual User User { get; set; }
    }
}

