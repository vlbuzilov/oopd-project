using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class SubscriptionClass
	{
        [Key]
        public int Subscription_Class_ID { get; set; }
        public int Subscription_ID { get; set; }
		public int Class_ID { get; set; }

        [ForeignKey("Subscription_ID")]
        public virtual Subscription_Type Subscription_Type { get; set; }
        [ForeignKey("Class_ID")]
        public virtual Class Class { get; set; }
    }
}

