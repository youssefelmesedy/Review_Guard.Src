using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.ValueObject;

public sealed class EmailAddress : ValueObject
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessRuleViolationException(nameof(EmailAddress), "Email cannot be empty.");

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            throw new BusinessRuleViolationException(nameof(EmailAddress), $"'{email}' is not a valid email address.");

        return new EmailAddress(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch { return false; }
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}