using Microsoft.EntityFrameworkCore;

namespace PersonasAPI.Models
{
    public class PersonaDbContext : DbContext
    {
        public PersonaDbContext(DbContextOptions<PersonaDbContext> options) : base(options) { }

        public DbSet<Persona> Personas { get; set; }
    }
}
