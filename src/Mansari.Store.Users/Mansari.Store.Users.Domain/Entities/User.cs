using Mansari.Store.Users.Domain.Common;
using Mansari.Store.Users.Domain.Enums;
using Mansari.Store.Users.Domain.ValueObjects;

namespace Mansari.Store.Users.Domain.Entities;

public sealed class User : Entity, IAggregateRoot
{
    public PersonName FirstName { get; private set; } = default!;
    public PersonName LastName { get; private set; } = default!;
    public NationalCode NationalCode { get; private set; } = default!;
    public MobileNumber MobileNumber { get; private set; } = default!;
    public Username Username { get; private set; } = default!;
    public EmailAddress? Email { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public UserStatus Status { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public string FullName => $"{FirstName.Value} {LastName.Value}";

    private User()
    {
    }

    private User(
        Guid id,
        PersonName firstName,
        PersonName lastName,
        NationalCode nationalCode,
        MobileNumber mobileNumber,
        Username username,
        EmailAddress? email,
        DateOnly birthDate,
        Gender gender)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        NationalCode = nationalCode;
        MobileNumber = mobileNumber;
        Username = username;
        Email = email;
        BirthDate = birthDate;
        Gender = gender;
        Status = UserStatus.Active;
        IsDeleted = false;
    }

    public static User Create(
        string firstName,
        string lastName,
        string nationalCode,
        string mobileNumber,
        string username,
        string? email,
        DateOnly birthDate,
        Gender gender)
    {
        return new User(
            Guid.NewGuid(),
            PersonName.Create(firstName, "First name"),
            PersonName.Create(lastName, "Last name"),
            NationalCode.Create(nationalCode),
            MobileNumber.Create(mobileNumber),
            Username.Create(username),
            string.IsNullOrWhiteSpace(email) ? null : EmailAddress.Create(email),
            birthDate,
            gender);
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        string nationalCode,
        string mobileNumber,
        string username,
        string? email,
        DateOnly birthDate,
        Gender gender,
        UserStatus status)
    {
        FirstName = PersonName.Create(firstName, "First name");
        LastName = PersonName.Create(lastName, "Last name");
        NationalCode = NationalCode.Create(nationalCode);
        MobileNumber = MobileNumber.Create(mobileNumber);
        Username = Username.Create(username);
        Email = string.IsNullOrWhiteSpace(email) ? null : EmailAddress.Create(email);
        BirthDate = birthDate;
        Gender = gender;
        Status = status;

        MarkAsUpdated();
    }

    public void Delete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        Status = UserStatus.Inactive;

        MarkAsUpdated();
    }

    public void Activate()
    {
        if (IsDeleted)
            throw new DomainException("USER_DELETED", "Deleted users cannot be activated.");

        Status = UserStatus.Active;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        if (IsDeleted)
            throw new DomainException("USER_DELETED", "Deleted users cannot be deactivated.");

        Status = UserStatus.Inactive;
        MarkAsUpdated();
    }
}
