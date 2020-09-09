using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class GenresControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetAllGenres_ShouldReturnAllGenres()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            BuildGenreTestDatabase(databaseName);

            var dbContext = BuildContext(databaseName);
            var mapper = BuildMapper();

            var controller = new GenresController(dbContext, mapper);

            // Act
            var response = await controller.Get();
            var genres = response.Value;

            // Assert
            Assert.AreEqual(2, genres.Count);
        }

        [TestMethod]
        public async Task GetGenreById_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var dbContext = BuildContext(databaseName);
            var mapper = BuildMapper();

            var controller = new GenresController(dbContext, mapper);

            // Act
            var response = await controller.Get(1);
            var result = response.Result as StatusCodeResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetGenreById_ValidId_ShouldReturnGenre()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            BuildGenreTestDatabase(databaseName);

            var dbContext = BuildContext(databaseName);
            var mapper = BuildMapper();
            var expectedId = 1;

            var controller = new GenresController(dbContext, mapper);

            // Act
            var response = await controller.Get(expectedId);
            var actualId = response.Value.Id;

            // Assert
            Assert.AreEqual(expectedId, actualId);
        }

        [TestMethod]
        public async Task CreateGenre_ShouldReturn201AndAddEntryInDb()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            BuildGenreTestDatabase(databaseName);

            var createDbContext = BuildContext(databaseName);
            var verifyDbContext = BuildContext(databaseName);
            var mapper = BuildMapper();
            var expectedName = "Test Genre";
            var expectedCount = createDbContext.Genres.Count() + 1;

            var createGenreDTO = new CreateGenreDTO() { Name = expectedName };

            var controller = new GenresController(createDbContext, mapper);

            // Act
            var response = await controller.Post(createGenreDTO);
            var result = response as CreatedAtRouteResult;
            var actualName = verifyDbContext.Genres.Find(expectedCount).Name;
            var actualCount = verifyDbContext.Genres.Count();

            // Assert
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual(expectedName, actualName);
            Assert.AreEqual(expectedCount, actualCount);
        }


        [TestMethod]
        public async Task ReplaceGenre_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var dbContext = BuildContext(databaseName);
            var mapper = BuildMapper();

            var testGenresCount = dbContext.Genres.Count();
            var expectedName = "New Genre Name";

            var createGenreDTO = new CreateGenreDTO() { Name = expectedName };

            var controller = new GenresController(dbContext, mapper);

            // Act
            var response = await controller.Put(testGenresCount + 1, createGenreDTO);
            var result = response as StatusCodeResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task ReplaceGenre_ValidId_ShouldReplaceGenre()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            BuildGenreTestDatabase(databaseName);
            var testDbContext = BuildContext(databaseName);
            var verifyDbContext = BuildContext(databaseName);

            var mapper = BuildMapper();

            var expectedGenresCount = verifyDbContext.Genres.Count();
            var expectedName = "New Genre Name";

            var createGenreDTO = new CreateGenreDTO() { Name = expectedName };

            var controller = new GenresController(testDbContext, mapper);

            // Act
            var response = await controller.Put(1, createGenreDTO);
            var result = response as StatusCodeResult;
            var actualName = verifyDbContext.Genres.Find(1).Name;

            // Assert
            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            Assert.AreEqual(expectedName, actualName);
        }



        private void BuildGenreTestDatabase(string databaseName)
        {
            var seedContext = BuildContext(databaseName);

            seedContext.Genres.Add(new Genre() { Name = "TestGenre 1" });
            seedContext.Genres.Add(new Genre() { Name = "TestGenre 2" });
            seedContext.SaveChanges();
        }

    }
}