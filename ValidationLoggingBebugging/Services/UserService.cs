using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using ValidationLoggingBebugging.Helpers;
using ValidationLoggingBebugging.Models;

namespace ValidationLoggingBebugging.Services
{
	public class UserService : IUserService	
	{
		private readonly ILogger<UserService> logger;

		public UserService(ILogger<UserService> logger)
		{
			this.logger = logger;
		}

		public IEnumerable<User> GetUsers()
		{
			logger.LogDebug("Retrieving all users.");
			return UsersCollection.Users;
		}

		public User? GetUserById(int id)
		{
			logger.LogDebug("Looking up user with ID {Id}", id);
			return UsersCollection.Users.FirstOrDefault(u => u.Id == id);
		}

		public User CreateUser(UserRequest userRequest)
		{
			logger.LogDebug("Creating user with username {Username}", userRequest.Username);

			if (UsersCollection.Users.Any(u => u.Username == userRequest.Username))
			{
				throw new DuplicateUsernameException();
			}

			var lastUser = UsersCollection.Users.OrderBy(u => u.Id).LastOrDefault();
			var newId = lastUser != null ? ++lastUser.Id : 1;

			var newUser = new User
			{
				Id = newId,
				Username = userRequest.Username,
				Email = userRequest.Email,
				Password = userRequest.Password,
				DateOfBirth = userRequest.DateOfBirth,
				Quantity = userRequest.Quantity,
				Price = userRequest.Price,
				Amount = userRequest.Amount
			};

			UsersCollection.Users.Add(newUser);
			logger.LogInformation("User created: {@User}", newUser);

			return newUser;
		}

		public void UpdateUser(int id, UserRequest userRequest)
		{
			var user = UsersCollection.Users.FirstOrDefault(u => u.Id == id);
			if (user is null)
			{
				throw new KeyNotFoundException($"User with id={id} not found.");
			}

			if (UsersCollection.Users.Any(u => u.Username == userRequest.Username && u.Id != id))
			{
				throw new DuplicateUsernameException();
			}

			user.Username = userRequest.Username;
			user.Password = userRequest.Password;
			user.DateOfBirth = userRequest.DateOfBirth;
			user.Email = userRequest.Email;
			user.Quantity = userRequest.Quantity;
			user.Price = userRequest.Price;
			user.Amount = userRequest.Amount;

			logger.LogInformation("User updated: {@User}", user);
		}

		public void PatchUser(int id, JsonPatchDocument<UserRequest> patchDoc)
		{
			var user = UsersCollection.Users.FirstOrDefault(u => u.Id == id);
			if (user is null)
			{
				throw new KeyNotFoundException($"User with id={id} not found.");
			}

			var isUsernameBeingUpdated = patchDoc.Operations.Any(op =>
				op.path.Equals("/Username", StringComparison.OrdinalIgnoreCase) &&
				op.OperationType is OperationType.Replace or OperationType.Add);

			if (isUsernameBeingUpdated)
			{
				var newUsername = patchDoc.Operations
					.First(op => op.path.Equals("/Username", StringComparison.OrdinalIgnoreCase))
					.value?.ToString();

				if (UsersCollection.Users.Any(u => u.Username == newUsername && u.Id != id))
				{
					throw new DuplicateUsernameException();
				}
			}

			patchDoc.ApplyTo(user);
			logger.LogInformation("User patched: {@User}", user);
		}
	}
}