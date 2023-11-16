using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class User
	{
        [Key]
        public int User_ID { get; set; }
		public int User_Role_ID { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Phone_Number { get; set; }
		public DateTime Birthdate { get; set; }

        [ForeignKey("User_Role_ID")]
        public virtual UserRole UserRole { get; set; }
    }
}

