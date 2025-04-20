using ValidationLoggingBebugging.Models;

namespace ValidationLoggingBebugging
{
	public static class UsersCollection
	{

		public static List<User> Users = new List<User>()
		{
			 new User
			 {
				 Id = 1,
				 Username = "john_doe",
				 Email = "john.doe@example.com",
				 Password = "Password123!",
				 DateOfBirth = new DateTime(1990, 5, 12),
				 Quantity = 3,
				 Price = "19.99m",
				 Amount = 59.97m
			 },
			 new User
			 {
				 Id = 2,
				 Username = "jane_smith",
				 Email = "jane.smith@example.com",
				 Password = "Secure*456",
				 DateOfBirth = new DateTime(1985, 8, 22),
				 Quantity = 2,
				 Price = "25.50m",
				 Amount = 51.00m
			 },
			 new User
			 {
				 Id = 3,
				 Username = "max_muster",
				 Email = "max.muster@example.com",
				 Password = "MyPass!789",
				 DateOfBirth = new DateTime(2000, 1, 1),
				 Quantity = 1,
				 Price = "99.99m",
				 Amount = 99.99m
			 }
		};
	}
}
