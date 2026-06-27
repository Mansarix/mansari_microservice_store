using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Application.Mappings;
using Mansari.Store.Users.Domain.Entities;
using Mansari.Store.Users.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByNationalCodeAsync(request.NationalCode, cancellationToken: cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "National code already exists.");

        if (await _repository.ExistsByMobileAsync(request.MobileNumber, cancellationToken: cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "Mobile number already exists.");

        if (await _repository.ExistsByUsernameAsync(request.Username, cancellationToken: cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "Username already exists.");

        if (!string.IsNullOrWhiteSpace(request.Email) && await _repository.ExistsByEmailAsync(request.Email, cancellationToken: cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "Email already exists.");

        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.NationalCode,
            request.MobileNumber,
            request.Username,
            request.Email,
            request.BirthDate,
            request.Gender);

        await _repository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserDto>.Success(user.ToDto());
    }
}
