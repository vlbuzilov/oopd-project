namespace oopd_project.Models;

public class SubscriptionType
{
    public int Subscription_Type_ID { get; set; }
    public string Subscription_Type_Name { get; set; }
    public decimal Price { get; set; }
    public double Duration { get; set; }
    public bool IsAvailable { get; set; }
    
    public List<Class> Classes { get; set; }
    public List<int> ClassesInSubscription { get; set; }
}