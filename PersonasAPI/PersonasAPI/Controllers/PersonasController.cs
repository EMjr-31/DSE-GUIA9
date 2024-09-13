using Microsoft.AspNetCore.Mvc;
using PersonasAPI.Models;
using System.Text.RegularExpressions;

namespace PersonasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly PersonaDbContext _context;

        public PersonasController(PersonaDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Persona>> PostPersona(Persona persona)
        {
            if (string.IsNullOrEmpty(persona.PrimerNombre) || string.IsNullOrEmpty(persona.PrimerApellido))
            {
                return BadRequest("El primer nombre y el primer apellido son obligatorios.");
            }

            if (persona.PrimerNombre.Length > 100 || persona.PrimerApellido.Length > 100 ||
                (persona.SegundoNombre?.Length ?? 0) > 100 || (persona.SegundoApellido?.Length ?? 0) > 100)
            {
                return BadRequest("Ningún campo puede exceder los 100 caracteres.");
            }

            if (persona.FechaNacimiento == default(DateTime) || persona.FechaNacimiento > DateTime.Now)
            {
                return BadRequest("Fecha de nacimiento inválida.");
            }

            if (!Regex.IsMatch(persona.DUI, @"^\d{8}-\d$"))
            {
                return BadRequest("El formato del DUI es inválido.");
            }

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPersona), new { id = persona.Id }, persona);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Persona>> GetPersona(int id)
        {
            var persona = await _context.Personas.FindAsync(id);

            if (persona == null)
            {
                return NotFound();
            }

            return persona;
        }
    }
}
