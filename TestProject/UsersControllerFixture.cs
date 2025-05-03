using Microsoft.Extensions.Logging;
using Moq;
using ValidationLoggingBebugging.Controllers;
using ValidationLoggingBebugging.Services;

namespace TestProject
{
	public class UsersControllerFixture
	{
		public Mock<IUserService> MockUserService { get; }
		public Mock<ILogger<UsersController>> MockLogger { get; }
		public UsersController UserController { get; }

		public UsersControllerFixture()
		{
			MockUserService = new Mock<IUserService>();
			MockLogger = new Mock<ILogger<UsersController>>();
			UserController = new UsersController(MockLogger.Object, MockUserService.Object);
		}
	}
}
