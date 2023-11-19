namespace oopd_project.Models;

public class Subscriptions
{
    public List<SubscriptionType> SubscriptionTypes { get; set; }
    public SubscriptionType SubscriptionTypeToAdd { get; set; }
    public List<Class> AvailableClasses { get; set; }
}