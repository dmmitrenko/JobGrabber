namespace WebScraperFunction.Domain.Models;
public class Vacancy
{
    public string Title { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public string Link { get; set; }
    public string? Salary { get; set; }
    public string? Description { get; set; }
    public DateTime PostedDate { get; set; }
}
