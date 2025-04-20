using FluentValidation;
using ValidationLoggingBebugging.Models;

namespace ValidationLoggingBebugging.Helpers
{
	public class UserRequestValidator : AbstractValidator<UserRequest>
	{
		private const int UsernameMinLength = 3;
		private const int PasswordMinLength = 6;
		private const decimal AmountLessThenNumber = 50;
		public UserRequestValidator()
		{
			RuleFor(u => u.Username)
				.Required(nameof(UserRequest.Username))
				.MinimumLength(UsernameMinLength)
				  .WithMessage($"Username must be least {UsernameMinLength} characters.");

			RuleFor(u => u.Email)
				.Required(nameof(UserRequest.Email))
				.EmailAddress()
				  .WithMessage("Email is not valid.");

			RuleFor(u => u.Password)
				.Required(nameof(UserRequest.Password))
				.MinimumLength(PasswordMinLength)
				   .WithMessage($"Password must be least {PasswordMinLength} characters.")
				.Must((request, password) => !password.Contains(request.Username, StringComparison.OrdinalIgnoreCase))
				   .WithMessage("Password must not contain the username.")
				.Matches("[A-Z]")
				   .WithMessage("Password must contain at least one uppercase letter.")
				.Matches("[a-z]")
				   .WithMessage("Password must contain at least one lowercase letter.")
				.Must(p => p.Any(char.IsDigit))
				   .WithMessage("Password must contain at least one number.")
				.Matches("[!@#$%^&*(),.?\":{}|<>]")
				   .WithMessage("Password must contain at least one special character.")
				.Matches("[ա-ֆԱ-Ֆև]+")
				   .WithMessage("Password must contain at least one Armenian letter.");

			RuleFor(u => u.DateOfBirth)
				.Required(nameof(UserRequest.DateOfBirth))
				.Must(day => day.Date < DateTime.UtcNow.Date)
				  .WithMessage("Date of birth must be in the pass.");


			RuleFor(u => u.Quantity)
				.Required(nameof(UserRequest.Quantity))
				.GreaterThan(0)
				  .WithMessage("Quantity must be a positive number.");

			RuleFor(x => x.Price)
				.Required(nameof(UserRequest.Price))
				.Must(value => decimal.TryParse(value, out _))
				  .WithMessage("Price must be a decimal value");


			RuleFor(x => x.Amount)
				.Required(nameof(UserRequest.Amount))
				.LessThan(AmountLessThenNumber)
				   .WithMessage($"Amount must be less than {AmountLessThenNumber}.");
		}


		public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
		{
			var result = await ValidateAsync(ValidationContext<UserRequest>.CreateWithOptions((UserRequest)model, x => x.IncludeProperties(propertyName)));
			if (result.IsValid)
				return Array.Empty<string>();
			return result.Errors.Select(e => e.ErrorMessage);
		};
	}
}
