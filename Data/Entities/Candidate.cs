namespace CvApp.Data.Entities;

public class Candidate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string CvUrl { get; set; }

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();

}