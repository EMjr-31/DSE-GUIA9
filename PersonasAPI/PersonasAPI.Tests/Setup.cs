using Microsoft.EntityFrameworkCore;
using PersonasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonasAPI.Tests
{
    public static class Setup
    {
        public static PersonaDbContext GetInMemoryDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<PersonaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new PersonaDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }

}
