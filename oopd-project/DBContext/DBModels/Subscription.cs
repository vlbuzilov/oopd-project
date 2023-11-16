using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class Subscription
	{
		[Key]
		public int Subscription_ID { get; set; }
		public int Subscription_Type_ID { get; set; }
		public int Client_ID { get; set; }
		public DateTime Starting_Date { get; set; }
		public bool isActive { get; set; }

        [ForeignKey("Subscription_Type_ID")]
        public virtual Subscription_Type Subscription_Type { get; set; }
        [ForeignKey("Client_ID")]
        public virtual Client Client { get; set; }

    }
}

