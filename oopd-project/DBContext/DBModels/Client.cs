using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oopd_project.DBContext.DBModels
{
	public class Client
	{
        [Key]
        public int Client_ID { get; set; }
		public string Name { get; set; }
		public string Last_Name { get; set; }
        public DateTime Registration_Date { get; set; }
        public int? Subscription_ID { get; set; }

        [ForeignKey("Client_ID")]
        public virtual User User { get; set; }
    }
}

