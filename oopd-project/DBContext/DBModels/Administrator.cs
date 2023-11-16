using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class Administrator
	{
        [Key]
        public int Admin_ID { get;set; }
		public string Name { get; set; }
		public string Last_Name { get; set; }
        public string Secret_Key { get; set; }

        [ForeignKey("Admin_ID")]
        public virtual User User { get; set; }
    }
}

