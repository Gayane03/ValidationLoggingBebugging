using Microsoft.AspNetCore.Mvc;
using ValidationLoggingBebugging.Models;
using Microsoft.AspNetCore.JsonPatch;
using TestProject;

namespace ValidationLoggingBebugging.Tests.Controllers
{
	public class UsersControllerTests : IClassFixture<UsersControllerFixture>
	{
		private readonly UsersControllerFixture fixture;

		public UsersControllerTests(UsersControllerFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Get_Users()
		{
			var expected = new List<User> { new User { Id = 1, Username = "Test" } };
			var users = fixture.MockUserService.Setup(s => s.GetUsers()).Returns(expected);

			var result = fixture.UserController.Get();

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			Assert.Equal(expected, okResult.Value);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		public void GetUserById_UserController_OK(int id)
		{
			var user = new User { Id = id, Username = "Test" };
			fixture.MockUserService.Setup(s => s.GetUserById(id)).Returns(user);

			var result = fixture.UserController.Get(id);

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			Assert.Equal(user, okResult.Value);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		public void GetUserById_UserController_ReturnsNotFound(int id)
		{
			fixture.MockUserService.Setup(s => s.GetUserById(id)).Returns((User)null);

			var result = fixture.UserController.Get(id);

			Assert.IsType<NotFoundResult>(result.Result);
		}


		[Theory]
		[InlineData(1,"Test")]
		[InlineData(2, "Test2")]
		public void Post_ReturnsCreatedUser(int id, string username)
		{
			var userRequest = new UserRequest { Username = username };
			var user = new User { Id = id, Username = username };
			fixture.MockUserService.Setup(s => s.CreateUser(userRequest)).Returns(user);

			var result = fixture.UserController.Post(userRequest);

			var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			Assert.Equal(user, createdResult.Value);
		}

		[Fact]
		public void Put_UpdatesUserSuccessfully()
		{
			var userRequest = new UserRequest { Username = "Updated" };

			var result = fixture.UserController.Put(1, userRequest);

			Assert.IsType<OkResult>(result);
		}

		[Fact]
		public void Put_ReturnsNotFound_WhenUserNotExists()
		{
			var userRequest = new UserRequest { Username = "Updated" };
			fixture.MockUserService.Setup(s => s.UpdateUser(1, userRequest)).Throws<KeyNotFoundException>();

			var result = fixture.UserController.Put(1, userRequest);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public void Patch_ReturnsBadRequest_WhenPatchIsNull()
		{
			var result = fixture.UserController.Patch(1, null);

			var badRequest = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid patch document.", badRequest.Value);
		}

		[Fact]
		public void Patch_ReturnsOk_WhenUserPatched()
		{
			var patchDoc = new JsonPatchDocument<UserRequest>();

			var result = fixture.UserController.Patch(1, patchDoc);

			Assert.IsType<OkResult>(result);
		}

		[Fact]
		public void Patch_ReturnsNotFound_WhenUserNotExists()
		{
			var patchDoc = new JsonPatchDocument<UserRequest>();
			fixture.MockUserService.Setup(s => s.PatchUser(1, patchDoc)).Throws<KeyNotFoundException>();

			var result = fixture.UserController.Patch(1, patchDoc);

			Assert.IsType<NotFoundResult>(result);
		}
	}
}
