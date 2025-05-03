using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using ValidationLoggingBebugging;
using ValidationLoggingBebugging.Helpers;
using ValidationLoggingBebugging.Models;
using ValidationLoggingBebugging.Services;

public class UserServiceTests : IClassFixture<UserServiceFixture>	
{
	private readonly IUserService userService;

	public UserServiceTests(UserServiceFixture fixture)
	{
		userService = fixture.UserService;
		UsersCollection.Users.Clear();
	}

	[Fact]
	public void GetUsers_ReturnsAllUsers()
	{
		var user = new User { Id = 1, Username = "test" };
		UsersCollection.Users.Add(user);

		var result = userService.GetUsers();

		Assert.Single(result);
		Assert.Equal(user, result.First());
	}

	[Fact]
	public void GetUserById_ReturnsCorrectUser()
	{
		var user = new User { Id = 1, Username = "test" };
		UsersCollection.Users.Add(user);

		var result = userService.GetUserById(1);

		Assert.Equal(user, result);
	}

	[Fact]
	public void GetUserById_ReturnsNull_IfNotFound()
	{
		var result = userService.GetUserById(99);

		Assert.Null(result);
	}

	[Fact]
	public void CreateUser_AddsNewUser()
	{
		var request = new UserRequest { Username = "new", Email = "a@a.com", Password = "123" };

		var created = userService.CreateUser(request);

		Assert.NotNull(created);
		Assert.Equal("new", created.Username);
		Assert.Single(UsersCollection.Users);
	}

	[Fact]
	public void CreateUser_ThrowsDuplicateUsernameException()
	{
		UsersCollection.Users.Add(new User { Id = 1, Username = "dup" });

		var request = new UserRequest { Username = "dup" };

		Assert.Throws<DuplicateUsernameException>(() => userService.CreateUser(request));
	}

	[Fact]
	public void UpdateUser_UpdatesUserSuccessfully()
	{
		UsersCollection.Users.Add(new User { Id = 1, Username = "old" });

		var request = new UserRequest { Username = "new", Password = "pass" };

		userService.UpdateUser(1, request);

		var user = UsersCollection.Users.First();
		Assert.Equal("new", user.Username);
	}

	[Fact]
	public void UpdateUser_ThrowsKeyNotFoundException_IfUserDoesNotExist()
	{
		var request = new UserRequest { Username = "missing" };

		Assert.Throws<KeyNotFoundException>(() => userService.UpdateUser(1, request));
	}

	[Fact]
	public void UpdateUser_ThrowsDuplicateUsernameException_IfUsernameExists()
	{
		UsersCollection.Users.Add(new User { Id = 1, Username = "one" });
		UsersCollection.Users.Add(new User { Id = 2, Username = "two" });

		var request = new UserRequest { Username = "two" };

		Assert.Throws<DuplicateUsernameException>(() => userService.UpdateUser(1, request));
	}

	[Fact]
	public void PatchUser_UpdatesUserField()
	{
		UsersCollection.Users.Add(new User { Id = 1, Username = "old" });

		var patch = new JsonPatchDocument<UserRequest>();
		patch.Operations.Add(new Operation<UserRequest>("replace", "/Username", null, "new"));

		userService.PatchUser(1, patch);

		Assert.Equal("new", UsersCollection.Users.First().Username);
	}

	[Fact]
	public void PatchUser_ThrowsKeyNotFoundException_WhenUserDoesNotExist()
	{
		var patch = new JsonPatchDocument<UserRequest>();

		Assert.Throws<KeyNotFoundException>(() => userService.PatchUser(99, patch));
	}

	[Fact]
	public void PatchUser_ThrowsDuplicateUsernameException_WhenUsernameExists()
	{
		UsersCollection.Users.Add(new User { Id = 1, Username = "one" });
		UsersCollection.Users.Add(new User { Id = 2, Username = "two" });

		var patch = new JsonPatchDocument<UserRequest>();
		patch.Operations.Add(new Operation<UserRequest>("replace", "/Username", null, "two"));

		Assert.Throws<DuplicateUsernameException>(() => userService.PatchUser(1, patch));
	}
}
