using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class PeopleControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetPeoplePaginated_ShouldReturnPaginatedResult()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            BuildPeopleTestDatabase(databaseName);

            var testContext = BuildContext(databaseName);
            var mapper = BuildMapper();

            var httpContext1 = BuildHttpContext("GET");
            var controller1 = new PeopleController(testContext, mapper, null);
            controller1.ControllerContext.HttpContext = httpContext1;

            var httpContext2 = BuildHttpContext("GET");
            var controller2 = new PeopleController(testContext, mapper, null);
            controller2.ControllerContext.HttpContext = httpContext2;

            // Act
            var responsePage1 = await controller1.Get(new PaginationDTO() { Page = 1, RecordsPerPage = 2 });
            var peoplePage1 = responsePage1.Value;

            var responsePage2 = await controller2.Get(new PaginationDTO() { Page = 2, RecordsPerPage = 2 });
            var peoplePage2 = responsePage2.Value;

            // Assert
            Assert.AreEqual(2, peoplePage1.Count);
            Assert.AreEqual(2, Int32.Parse(controller1.Response.Headers["page-count"]));
            Assert.AreEqual(1, peoplePage2.Count);
        }

        [TestMethod]
        public async Task CreateNewPerson_WithoutPicture_ShouldCreateDBEntry()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var testContext = BuildContext(databaseName);
            var verifyContext = BuildContext(databaseName);
            var mapper = BuildMapper();

            var newPerson = new CreatePersonDTO() { Name = "New Genre", Biography = "abc", DateOfBirth = DateTime.Now };

            var fileStorageServiceMock = new Mock<IFileStorageService>();
            fileStorageServiceMock
                .Setup(x => x.SaveFile(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.FromResult("url"));

            var controller = new PeopleController(testContext, mapper, fileStorageServiceMock.Object);

            // Act
            var response = await controller.Post(newPerson);
            var result = response.Result as CreatedAtRouteResult;
            var dbEntry = verifyContext.Person.Find(1);

            // Assert
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual(1, verifyContext.Person.ToList().Count);
            Assert.IsNull(dbEntry.Picture);
        }

        [TestMethod]
        public async Task CreateNewPerson_WithPicture_ShouldCreateDBEntry()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var testContext = BuildContext(databaseName);
            var verifyContext = BuildContext(databaseName);
            var mapper = BuildMapper();

            var pictureContent = Encoding.UTF8.GetBytes("This is a dummy image");
            var file = new FormFile(new MemoryStream(pictureContent), 0, pictureContent.Length, "Data", "test.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";


            var newPerson = new CreatePersonDTO()
            {
                Name = "New Genre",
                Biography = "abc",
                DateOfBirth = DateTime.Now,
                Picture = file
            };

            var fileStorageServiceMock = new Mock<IFileStorageService>();
            fileStorageServiceMock
                .Setup(x => x.SaveFile(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.FromResult("url"));

            var controller = new PeopleController(testContext, mapper, fileStorageServiceMock.Object);

            // Act
            var response = await controller.Post(newPerson);
            var result = response.Result as CreatedAtRouteResult;
            var dbEntry = verifyContext.Person.Find(1);

            // Assert
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual(1, verifyContext.Person.ToList().Count);
            Assert.AreEqual("url", dbEntry.Picture);
            Assert.AreEqual(1, fileStorageServiceMock.Invocations.Count);
        }

        private void BuildPeopleTestDatabase(string databaseName)
        {
            var seedContext = BuildContext(databaseName);

            seedContext.Person.Add(new Person() { Name = "Person 1", Biography = "Biography 1" });
            seedContext.Person.Add(new Person() { Name = "Person 2", Biography = "Biography 2" });
            seedContext.Person.Add(new Person() { Name = "Person 3", Biography = "Biography 3" });

            seedContext.SaveChanges();
        }

    }
}