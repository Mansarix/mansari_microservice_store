using FluentValidation;

namespace Mansari.Store.Users.Application.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NationalCode).NotEmpty().Length(10).Matches("^[0-9۰-۹٠-٩]+$");
        RuleFor(x => x.MobileNumber).NotEmpty().MinimumLength(11).MaximumLength(16);
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4).MaximumLength(32);
        RuleFor(x => x.Email).MaximumLength(254).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.BirthDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
