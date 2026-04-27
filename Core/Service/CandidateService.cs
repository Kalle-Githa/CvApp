

// Test lokalt - 


using CvApp.Core.Interface;
using CvApp.Data;
using CvApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CvApp.Core.Service;

public class CandidateService : ICandidateService
{
    private readonly CvAppDbContext _context;
    private readonly IConfiguration _configuration;

    // IConfiguration injiceras via DI så vi kan läsa från appsettings.json
    // utan att hårdkoda känsliga värden som connection strings i koden
    public CandidateService(CvAppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<List<CandidateSearchResultDto>> GetCandidatesBySkillsAsync(string skills)
    {
        // Splitta den kommaseparerade strängen, t.ex. "Azure, C#, React"
        // .Trim() tar bort mellanslag runt varje ord så "Azure" och " Azure" behandlas lika
        var skillList = skills.Split(",").Select(s => s.Trim()).ToList();

        // Hämta kandidater från databasen
        // .Include(c => c.Skills) – laddar in relaterade Skills för varje kandidat (eager loading)
        // Utan Include skulle c.Skills vara en tom lista – EF laddar inte relaterad data automatiskt
        var candidates = await _context.Candidates
            .Include(c => c.Skills)
            // .Any() returnerar true om minst en skill hos kandidaten finns i vår söklista
            // Kandidater som inte har NÅGON av de sökta skills filtreras bort
            .Where(c => c.Skills.Any(s => skillList.Contains(s.SkillName)))
            .ToListAsync();

        // Mappa varje Candidate-entitet till CandidateSearchResultDto
        // Vi exponerar aldrig entiteter direkt – DTOs skyddar vår databasstruktur
        return candidates.Select(c => new CandidateSearchResultDto
        {
            Name = c.Name,
            // Mappa varje Skill-entitet till SkillDto
            // s.SkillName plockar ut just namnet från det aktuella skill-objektet i loopen
            Skills = c.Skills.Select(s => new SkillDto { Skill = s.SkillName }).ToList()
        }).ToList();
    }


    public async Task UploadCandidateAsync(UploadViewModel model)
    {
        // Skapa en Uploads-mapp i projektets rot om den inte finns
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Hämta filnamn och extension separat
        var originalFileName = Path.GetFileNameWithoutExtension(model.Cv.FileName);
        var extension = Path.GetExtension(model.Cv.FileName);

        // Skapa unikt filnamn med Guid för att undvika kollisioner
        var uniqueFileName = $"{originalFileName}_{Guid.NewGuid()}{extension}";

        // Bygg fullständig sökväg
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Spara filen till disk
        // using säkerställer att stream stängs och resurser frigörs efteråt
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.Cv.CopyToAsync(stream);
        }

        // TODO: Spara kandidatdata till databas (implementeras när Azure är uppsatt)
    }
}