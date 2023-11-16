using System;
namespace oopd_project.Models
{
	public abstract class User
	{
		public int UserId { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
        public string ConfirmationPassword { get; set; }
        public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime Birthdate { get; set; }
	}
}

