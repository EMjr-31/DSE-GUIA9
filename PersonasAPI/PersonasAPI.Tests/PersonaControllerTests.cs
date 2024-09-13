using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonasAPI.Controllers;
using PersonasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonasAPI.Tests
{
    public class PersonasControllerTests
    {
        [Fact]
        public async Task PostPersona_AgregaPersona_CuandoPersonaEsValida()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new PersonasController(context);
            var nuevaPersona = new Persona
            {
                PrimerNombre = "Juan",
                SegundoNombre = "Carlos",
                PrimerApellido = "Pérez",
                SegundoApellido = "Martínez",
                DUI = "01234567-8",
                FechaNacimiento = new DateTime(1990, 5, 15)
            };

            // Act
            var result = await controller.PostPersona(nuevaPersona);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var persona = Assert.IsType<Persona>(createdResult.Value);
            Assert.Equal("Juan", persona.PrimerNombre);
        }

        [Fact]
        public async Task GetPersona_RetornaPersona_CuandoIdEsValido()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new PersonasController(context);
            var persona = new Persona
            {
                PrimerNombre = "Maria",
                SegundoNombre = "Luisa",
                PrimerApellido = "García",
                SegundoApellido = "Lopez",
                DUI = "12345678-9",
                FechaNacimiento = new DateTime(1985, 7, 10)
            };
            context.Personas.Add(persona);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetPersona(persona.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Persona>>(result);
            var returnValue = Assert.IsType<Persona>(actionResult.Value);
            Assert.Equal("Maria", returnValue.PrimerNombre);
        }

        [Fact]
        public async Task PostPersona_NoAgregaPersona_CuandoPrimerNombreEsNulo()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new PersonasController(context);
            var nuevaPersona = new Persona
            {
                PrimerNombre = null, // No puede ser nulo
                PrimerApellido = "Pérez",
                DUI = "01234567-8",
                FechaNacimiento = new DateTime(1990, 5, 15)
            };

            // Act
            var result = await controller.PostPersona(nuevaPersona);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostPersona_NoAgregaPersona_CuandoDUIEsInvalido()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new PersonasController(context);
            var nuevaPersona = new Persona
            {
                PrimerNombre = "Luis",
                PrimerApellido = "Ramirez",
                DUI = "12345-678", // DUI no válido
                FechaNacimiento = new DateTime(1992, 3, 22)
            };

            // Act
            var result = await controller.PostPersona(nuevaPersona);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostPersona_NoAgregaPersona_CuandoFechaNacimientoEsInvalida()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new PersonasController(context);   
            var nuevaPersona = new Persona
            {
                PrimerNombre = "Ana",
                PrimerApellido = "Sanchez",
                DUI = "98765432-1",
                FechaNacimiento = new DateTime(1, 1, 1) // Fecha inválida
            };

            // Act
            var result = await controller.PostPersona(nuevaPersona);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPersona_RetornaNotFound_CuandoIdNoExiste()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new PersonasController(context);

            // Act
            var result = await controller.GetPersona(999); // Id que no existe

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostPersona_IncrementaConteo_CuandoSeAgregaNuevaPersona()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new PersonasController(context);
            var personaInicial = new Persona
            {
                PrimerNombre = "Pedro",
                PrimerApellido = "Lopez",
                DUI = "76543210-9",
                FechaNacimiento = new DateTime(2000, 1, 1)
            };
            await controller.PostPersona(personaInicial);

            var nuevaPersona = new Persona
            {
                PrimerNombre = "Laura",
                PrimerApellido = "Gomez",
                DUI = "87654321-0",
                FechaNacimiento = new DateTime(1995, 12, 15)
            };

            // Act
            await controller.PostPersona(nuevaPersona);
            var personas = await context.Personas.ToListAsync();

            // Assert
            Assert.Equal(2, personas.Count);
        }
    }
}