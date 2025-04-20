using FluentValidation;

namespace ValidationLoggingBebugging.Helpers
{
	public static class RuleBuilderExtension
	{
		public static IRuleBuilderOptions<TModel, TField> Required<TModel, TField>(this IRuleBuilder<TModel, TField> ruleBuilder, string fieldName)
		{
			return ruleBuilder.NotNull()
							  .NotEmpty()
							  .WithMessage($"{fieldName} is required.");
		}
	}
}
