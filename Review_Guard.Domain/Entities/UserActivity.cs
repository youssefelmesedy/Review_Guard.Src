using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class UserActivity : BaseEntity
{
    private UserActivity() { }

    public Guid UserId { get; private set; }
    public ActivityType Type { get; private set; }
    public string Description { get; private set; } = default!;
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Location { get; private set; }
    public bool IsSuspicious { get; private set; }
    public string? SuspicionReason { get; private set; }
    private readonly Dictionary<string, string> _metadata = new();
    public IReadOnlyDictionary<string, string> Metadata => _metadata;

    // Navigation
    public User User { get; private set; } = default!;

    public static UserActivity Create(Guid userId, ActivityType type, string description,
        string? ipAddress = null, string? userAgent = null, string? location = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Activity description is required.");

        return new UserActivity
        {
            UserId = userId,
            Type = type,
            Description = description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Location = location
        };
    }

    public void MarkSuspicious(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Suspicion reason is required.");

        IsSuspicious = true;
        SuspicionReason = reason;
    }

    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Metadata key is required.");

        _metadata[key] = value;
    }
}
