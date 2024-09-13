using LibrosAPI.Controllers;
using LibrosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LibrosAPI.Tests
{
    public class LibrosControllerTests
    {
        [Fact]
        public async Task PostLibro_AgregaLibro_CuandoLibroEsValido()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new LibrosController(context);
            var nuevoLibro = new Libro
            {
                Titulo = "El Principito",
                Autor = "Antoine de Saint-Exupéry",
                FechaPublicacion = new DateTime(1943, 4, 6)
            };

            // Act
            var result = await controller.PostLibro(nuevoLibro);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var libro = Assert.IsType<Libro>(createdResult.Value);
            Assert.Equal("El Principito", libro.Titulo);
        }

        [Fact]
        public async Task GetLibro_RetornaLibro_CuandoIdEsValido()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new LibrosController(context);
            var libro = new Libro
            {
                Titulo = "1984",
                Autor = "George Orwell",
                FechaPublicacion = new DateTime(1949, 6, 8)
            };
            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetLibro(libro.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Libro>>(result);
            var returnValue = Assert.IsType<Libro>(actionResult.Value);
            Assert.Equal("1984", returnValue.Titulo);
        }

        [Fact]
        public async Task GetLibro_RetornaNotFound_CuandoIdNoExiste()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new LibrosController(context);

            // Act
            var result = await controller.GetLibro(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostLibro_NoAgregaLibro_CuandoTituloEsNulo()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new LibrosController(context);
            var nuevoLibro = new Libro
            {
                Titulo = null,
                Autor = "Autor",
                FechaPublicacion = new DateTime(2000, 1, 1)
            };

            // Act
            var result = await controller.PostLibro(nuevoLibro);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostLibro_IncrementaConteo_CuandoSeAgregaNuevoLibro()
        {
            // Arrange
            var context = Setup.GetInMemoryDatabaseContext();
            var controller = new LibrosController(context);
            var libroInicial = new Libro
            {
                Titulo = "Libro 1",
                Autor = "Autor 1",
                FechaPublicacion = DateTime.Now
            };
            await controller.PostLibro(libroInicial);

            var nuevoLibro = new Libro
            {
                Titulo = "Libro 2",
                Autor = "Autor 2",
                FechaPublicacion = DateTime.Now
            };

            // Act
            await controller.PostLibro(nuevoLibro);
            var libros = await context.Libros.ToListAsync();

            // Assert
            Assert.Equal(2, libros.Count);
        }
    }
}
