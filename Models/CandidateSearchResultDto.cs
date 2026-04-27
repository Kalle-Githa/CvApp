using CvApp.Data.Entities;

namespace CvApp.Models;

public class CandidateSearchResultDto
{
    public string Name { get; set; }
    public List<SkillDto> Skills { get; set; } = new();
    
}