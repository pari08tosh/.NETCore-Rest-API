using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using Newtonsoft.Json;

namespace MoviesAPI.Tests.IntegrationTests
{
    [TestClass]
    public class GenresControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetAllGenres_NoDbSeeds_ShouldReturnEmptyList()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);

            var client = factory.CreateClient();
            var url = "/api/genres";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            var genres = JsonConvert.DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(0, genres.Count);
        }

        [TestMethod]
        public async Task GetAllGenres_SeededDB_ShouldReturnGenresList()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);
            BuildGenreTestDatabase(databaseName);

            var client = factory.CreateClient();
            var url = "/api/genres";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            var genres = JsonConvert.DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(2, genres.Count);
        }

        [TestMethod]
        public async Task DeleteGenre_NotAuthenticated_ShouldReturnUnAuthorized()
        {
            var databaseName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(databaseName);
            BuildGenreTestDatabase(databaseName);

            var client = factory.CreateClient();
            var url = "/api/genres/1";

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteGenre_AuthenticatedAsAdmin_ShouldDeleteEntry()
        {
            var databaseName = Guid.NewGuid().ToString();
            var verifyDBContext = BuildContext(databaseName);
            var factory = BuildWebApplicationFactory(databaseName);
            BuildGenreTestDatabase(databaseName);

            var dummyAdminToken = GenerateDummyToken(new List<string>() { "Admin" });
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", dummyAdminToken);
            var url = "/api/genres/1";

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.AreEqual(1, verifyDBContext.Genres.ToList().Count);
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