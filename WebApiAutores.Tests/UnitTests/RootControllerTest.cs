using Microsoft.AspNetCore.Authorization;
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

    }
}
