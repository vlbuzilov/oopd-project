using System;
namespace oopd_project.Models
{
	public class Class
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Coach Coach { get; set; }
		public int MaxNumberOfPeople { get; set; }
		public TimeSpan Duration { get; set; }
		public List<Client> Clients { get; set; }
	}
}

