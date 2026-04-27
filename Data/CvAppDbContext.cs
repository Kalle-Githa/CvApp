using CvApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvApp.Data;

public class CvAppDbContext: DbContext
{
    public CvAppDbContext(DbContextOptions<CvAppDbContext>options)
        :base(options){}
    
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Skill> Skills => Set<Skill>();
    
}



