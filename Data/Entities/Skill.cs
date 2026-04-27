namespace CvApp.Data.Entities;

public class Skill
{
    public int Id { get; set; }
    public string SkillName { get; set; }
   
    public ICollection<Candidate>  Candidates { get; set; } = new List<Candidate>();




}