using System;
namespace oopd_project.Models
{
	public class Subscription
	{
		public string SubscriptionType { get; set; }
		public DateTime StartingDate { get; set; }
		public int Duration { get; set; }
		public DateTime EndingDate { get; set; }
		public bool IsAvailabel { get; set; }
		public bool IsActive { get; set; }
	}
}

