using Microsoft.Extensions.Logging;
using ValidationLoggingBebugging;
using ValidationLoggingBebugging.Services;

public class UserServiceFixture : IDisposable
{
	public IUserService UserService { get; }

	public UserServiceFixture()
	{
		var logger = new LoggerFactory().CreateLogger<UserService>();
		UserService = new UserService(logger);
	}

	public void Dispose()
	{
		UsersCollection.Users.Clear();
	}
}
