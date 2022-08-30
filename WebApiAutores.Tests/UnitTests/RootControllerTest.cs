using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using WebApiAutores.Controllers.V1;
using WebApiAutores.Tests.Mocks;

namespace WebApiAutores.Tests.UnitTests
{

    [TestClass]
    public class RootControllerTest
    {

        [TestMethod]
        public async Task IfUserIsAdminGetResponsWith4Links()
        {
            // Preparation
            var authorizationServiceMock = new AuthorizationServiceMock();
            authorizationServiceMock.Result = AuthorizationResult.Success();
            var rootController = new RootController(authorizationServiceMock);
            rootController.Url = new URLHelperMock();

            // Execution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(5, result.Value.Count());
        }

        [TestMethod]
        public async Task IfUserIsNotAdminGetResponsWith4Links()
        {
            // Preparation
            var authorizationServiceMock = new AuthorizationServiceMock();
            authorizationServiceMock.Result = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationServiceMock);
            rootController.Url = new URLHelperMock();

            // Execution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(4, result.Value.Count());
        }

        [TestMethod]
        public async Task IfUserIsNotAdminGetResponsWith4LinksUsingMoqLibrary()
        {
            // Preparation
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService
                .Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));
            mockAuthorizationService
                .Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper
                .Setup(x => x.Link(It.IsAny<String>(), It.IsAny<object>()))
                .Returns(String.Empty);

            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockUrlHelper.Object;

            // Execution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(4, result.Value.Count());
        }

    }
}
