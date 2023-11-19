namespace oopd_project.Models;

public class AddToScheduleModel
{
    public int ClassId { get; set; }
    public DateTime SelectedDate { get; set; }
    public TimeSpan SelectedTime { get; set; }
}