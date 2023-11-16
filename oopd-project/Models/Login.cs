using System;
namespace oopd_project.Models
{
	public class Login
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string SecretKey { get; set; }
		public bool IsAdmin { get; set; } = false;
	}
}

