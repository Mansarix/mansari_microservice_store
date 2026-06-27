using Mansari.Store.Users.Application.DTOs;
using Mansari.Store.Users.Domain.Entities;

namespace Mansari.Store.Users.Application.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            NationalCode = user.NationalCode.Value,
            MobileNumber = user.MobileNumber.Value,
            Username = user.Username.Value,
            Email = user.Email?.Value,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            Status = user.Status,
            IsDeleted = user.IsDeleted,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
            DeletedAtUtc = user.DeletedAtUtc,
            FullName = user.FullName
        };
    }
}
