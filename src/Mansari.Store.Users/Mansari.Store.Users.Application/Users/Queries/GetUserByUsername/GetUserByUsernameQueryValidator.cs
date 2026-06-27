using FluentValidation;

namespace Mansari.Store.Users.Application.Users.Queries.GetUserByUsername;

public sealed class GetUserByUsernameQueryValidator : AbstractValidator<GetUserByUsernameQuery>
{
    public GetUserByUsernameQueryValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4).MaximumLength(32);
    }
}
