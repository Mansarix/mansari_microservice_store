using FluentValidation;

namespace Mansari.Store.Users.Application.Users.Queries.GetUserByNationalCode;

public sealed class GetUserByNationalCodeQueryValidator : AbstractValidator<GetUserByNationalCodeQuery>
{
    public GetUserByNationalCodeQueryValidator()
    {
        RuleFor(x => x.NationalCode).NotEmpty().Length(10).Matches("^[0-9۰-۹٠-٩]+$");
    }
}
