using System;
using System.ComponentModel.DataAnnotations;

namespace oopd_project.DBContext.DBModels
{
	public class UserRole
	{
        [Key]
        public int Role_ID { get; set; }
		public string Role { get; set; }
	}
}

