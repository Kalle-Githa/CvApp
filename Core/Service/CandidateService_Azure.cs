

//// AZURE VERSION - används när Azure Storage och SQL är uppsatt
//// Se CandidateService.cs för lokal testversionusing Azure.Storage.Blobs;


//using CvApp.Core.Interface;
//using CvApp.Data;
//using CvApp.Data.Entities;
//using CvApp.Models;
//using Microsoft.EntityFrameworkCore;

//namespace CvApp.Core.Service;

//public class CandidateService_Azure : ICandidateService
//{
//    private readonly CvAppDbContext _context;
//    private readonly IConfiguration _configuration;

//    // IConfiguration injiceras via DI så vi kan läsa från appsettings.json
//    // utan att hårdkoda känsliga värden som connection strings i koden
//    public CandidateService_Azure(CvAppDbContext context, IConfiguration configuration)
//    {
//        _context = context;
//        _configuration = configuration;
//    }

//    public async Task<List<CandidateSearchResultDto>> GetCandidatesBySkillsAsync(string skills)
//    {
//        // Splitta den kommaseparerade strängen, t.ex. "Azure, C#, React"
//        // .Trim() tar bort mellanslag runt varje ord så "Azure" och " Azure" behandlas lika
//        var skillList = skills.Split(",").Select(s => s.Trim()).ToList();

//        // Hämta kandidater från databasen
//        // .Include(c => c.Skills) – laddar in relaterade Skills för varje kandidat (eager loading)
//        // Utan Include skulle c.Skills vara en tom lista – EF laddar inte relaterad data automatiskt
//        var candidates = await _context.Candidates
//            .Include(c => c.Skills)
//            // .Any() returnerar true om minst en skill hos kandidaten finns i vår söklista
//            // Kandidater som inte har NÅGON av de sökta skills filtreras bort
//            .Where(c => c.Skills.Any(s => skillList.Contains(s.SkillName)))
//            .ToListAsync();

//        // Mappa varje Candidate-entitet till CandidateSearchResultDto
//        // Vi exponerar aldrig entiteter direkt – DTOs skyddar vår databasstruktur
//        return candidates.Select(c => new CandidateSearchResultDto
//        {
//            Name = c.Name,
//            // Mappa varje Skill-entitet till SkillDto
//            // s.SkillName plockar ut just namnet från det aktuella skill-objektet i loopen
//            Skills = c.Skills.Select(s => new SkillDto { Skill = s.SkillName }).ToList()
//        }).ToList();
//    }


//    public async Task UploadCandidateAsync(UploadViewModel model)
//    {
//        // 1. Anslut till Storage Account via connection string från appsettings.json
//        // BlobServiceClient är ingångspunkten till hela Storage Account
//        var blobServiceClient = new BlobServiceClient(
//            _configuration.GetConnectionString("AzureStorage"));

//        // 2. Hämta containern – en container är som en mapp i Blob Storage
//        var containerClient = blobServiceClient.GetBlobContainerClient("cv-files");

//        // 3. Skapa ett unikt filnamn med Guid för att undvika kollisioner
//        // Om två kandidater laddar upp "cv.pdf" skulle den ena skriva över den andra
//        // Guid.NewGuid() genererar ett globalt unikt ID, t.ex: "3f2504e0-cv.pdf"
//        var fileName = $"{Guid.NewGuid()}_{model.Cv.FileName}";
//        var blobClient = containerClient.GetBlobClient(fileName);

//        // 4. Ladda upp filen som en stream
//        // OpenReadStream() öppnar filens innehåll för läsning utan att ladda hela filen i minnet
//        await blobClient.UploadAsync(model.Cv.OpenReadStream());

//        // 5. Bygg upp kandidatobjektet som ska sparas i databasen
//        var candidate = new Candidate
//        {
//            Name = model.Name,
//            Email = model.Email,
//            PhoneNumber = model.PhoneNumber,

//            // blobClient.Uri ger oss den publika URL:en till filen i Blob Storage
//            // Denna URL sparas i databasen så vi kan länka till filen senare
//            CvUrl = blobClient.Uri.ToString(),

//            // model.Skills är en kommaseparerad sträng, t.ex. "Azure, C#, React"
//            // Split(",")  → delar upp strängen till en array: ["Azure", " C#", " React"]
//            // .Trim()     → tar bort mellanslag i början/slutet av varje element: ["Azure", "C#", "React"]
//            // Select(...)  → omvandlar varje sträng till ett Skill-objekt (LINQ-transformation)
//            // .ToList()   → konverterar resultatet till List<Skill> som Candidate.Skills förväntar sig
//            Skills = model.Skills
//                .Split(",")
//                .Select(s => new Skill { SkillName = s.Trim() })
//                .ToList()
//        };

//        // 6. Lägg till kandidaten i DbContext (spåras nu av EF Core)
//        await _context.Candidates.AddAsync(candidate);

//        // 7. SaveChangesAsync skriver alla spårade ändringar till databasen i en transaktion
//        // Utan detta anropet sparas ingenting – EF Core väntar tills du explicit ber om det
//        await _context.SaveChangesAsync();
//    }
//}