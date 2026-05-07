using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Business : BaseEntity
{
    private Business() { }

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; } = default!;

    public string? AdminNote { get; private set; }
    public Guid? ReviewedByAdminId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    public string? LogoUrl { get; private set; }

    // Navigation properties 

    private readonly List<Proof> _proofs = new();
    public IReadOnlyCollection<Proof> Proofs => _proofs.AsReadOnly();

    public Guid BusinessCategoryId { get; private set; }
    public BusinessCategory BusinessCategory { get; private set; } = default!;

    private readonly List<Branch> _branches = new();
    public IReadOnlyCollection<Branch> Branches => _branches.AsReadOnly();

    public BusinessStatus Status { get; private set; } = BusinessStatus.PendingApproval;


    public bool IsActive { get; private set; } = false;

    public bool IsVerified => Status == BusinessStatus.Active;

    public bool IsOwnedBy(Guid userId) => OwnerId == userId;

    public void Deactivate() { IsActive = false; Status = BusinessStatus.Inactive; SetUpdatedAt(); }
    public void Activate() { IsActive = true; Status = BusinessStatus.Active; SetUpdatedAt(); }
    public void UpdateLogo(string logoUrl) { LogoUrl = logoUrl; SetUpdatedAt(); }

    // ── Factory ─────────────────────────────
    public static Business Create(
        Guid ownerId,
        string name,
        string description,
        Guid categoryId,
        string commercialRegistrationNumber,
        string taxNumber)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Business name is required.");

        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Business description is required.");

        if (string.IsNullOrWhiteSpace(commercialRegistrationNumber)) throw new DomainException("Commercial registration number is required.");

        if (string.IsNullOrWhiteSpace(taxNumber)) throw new DomainException("Tax number is required.");

        return new Business
        {
            OwnerId = ownerId,
            Name = name.Trim(),
            Description = description.Trim(),
            BusinessCategoryId = categoryId,
            Status = BusinessStatus.PendingApproval,
            IsActive = false
        };
    }

    // ── Branch Management ─────────────────────
    public Branch AddBranch(Guid currentUserId, string address, string city, string country, string phone, Guid managerId)
    {
        if (!IsOwnedBy(currentUserId)) throw new DomainException("Only owner can add branches.");
        var branch = Branch.Create(Id, address, city, country, phone, managerId);
        _branches.Add(branch);
        return branch;
    }

    // ── Admin Actions ──────────────────────
    public void Approve(Guid adminId, string? note = null)
    {
        Status = BusinessStatus.Active;
        IsActive = true;
        ReviewedByAdminId = adminId;
        ReviewedAt = DateTime.UtcNow;
        AdminNote = note;
        SetUpdatedAt();
    }

    public void Reject(Guid adminId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Rejection reason is required.");
        Status = BusinessStatus.Rejected;
        ReviewedByAdminId = adminId;
        ReviewedAt = DateTime.UtcNow;
        AdminNote = reason;
        SetUpdatedAt();
    }
    public Proof AddOrderProof(Guid branchId, Guid userId, string orderId)
    {
        var branch = Branches.FirstOrDefault(b => b.Id == branchId)
            ?? throw new DomainException("Branch not found.");

        var proof = Proof.CreateFromOrder(userId, branchId, orderId);
        _proofs.Add(proof);
        return proof;
    }
}
