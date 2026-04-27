using CvApp.Data.Entities;
using CvApp.Models;

namespace CvApp.Core.Interface;

public interface ICandidateService
{
    Task<List<CandidateSearchResultDto>> GetCandidatesBySkillsAsync(string skills);
    Task UploadCandidateAsync(UploadViewModel model);
    
    
}