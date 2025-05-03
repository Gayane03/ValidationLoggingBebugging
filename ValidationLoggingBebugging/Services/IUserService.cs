using Microsoft.AspNetCore.JsonPatch;
using ValidationLoggingBebugging.Models;

namespace ValidationLoggingBebugging.Services
{
	public interface IUserService
	{
		IEnumerable<User> GetUsers();
		User? GetUserById(int id);
		User CreateUser(UserRequest userRequest);
		void UpdateUser(int id, UserRequest userRequest);
		void PatchUser(int id, JsonPatchDocument<UserRequest> patchDoc);
	}
}
