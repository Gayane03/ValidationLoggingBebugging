namespace ValidationLoggingBebugging.Models
{
	public class UserRequest
	{
        public string Username { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }	
		public DateTime DateOfBirth { get; set; }

		public int Quantity { get; set; }

		//price is string type for can check type is decimal or not
		public string Price { get; set; }	

		public decimal Amount { get; set; }
    }
}
