using Mansari.Store.Users.Application.Common;
using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Application.Mappings;
using Mansari.Store.Users.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Users.Application.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);

        if (user is null)
            return Result<UserDto>.Failure(ErrorCodes.NotFound, "User not found.");

        if (await _repository.ExistsByNationalCodeAsync(request.NationalCode, request.Id, cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "National code already exists.");

        if (await _repository.ExistsByMobileAsync(request.MobileNumber, request.Id, cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "Mobile number already exists.");

        if (await _repository.ExistsByUsernameAsync(request.Username, request.Id, cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "Username already exists.");

        if (!string.IsNullOrWhiteSpace(request.Email) && await _repository.ExistsByEmailAsync(request.Email, request.Id, cancellationToken))
            return Result<UserDto>.Failure(ErrorCodes.Conflict, "Email already exists.");

        user.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.NationalCode,
            request.MobileNumber,
            request.Username,
            request.Email,
            request.BirthDate,
            request.Gender,
            request.Status);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserDto>.Success(user.ToDto());
    }
}
