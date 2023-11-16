using System;
using System.ComponentModel.DataAnnotations;

namespace oopd_project.DBContext.DBModels
{
	public class Subscription_Type
	{
        [Key]
        public int Subscription_Type_ID { get; set; }
		public string Subscription_Type_Name { get; set; }
		public decimal Price { get; set; }
		public double Duration { get; set; }
		public bool isAvailable { get; set; }
    }
}

