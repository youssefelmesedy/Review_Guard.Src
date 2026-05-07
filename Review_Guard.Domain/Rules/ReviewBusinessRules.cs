using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;
using Review_Guard.Domain.ValueObject;

namespace Review_Guard.Domain.Rules;

/// <summary>
/// Domain-level business rules for the Review aggregate.
/// Pure static methods, fully unit-testable, independent of Application Layer.
/// </summary>
public static class ReviewBusinessRules
{
    //── Guard: ownership ───────────────────────────────────────────────
    public static void UserCannotReviewOwnBusiness(User user, Business business)
    {
        if (business.IsOwnedBy(user.Id))
            throw new DomainException("You cannot review your own business.");
    }

    // ── Guard: submission eligibility (daily limit, account status) ──
    public static void UserMustBeEligibleToReview(User user, int maxPerDay = 5)
    {
        user.ValidateCanSubmitReview(maxPerDay);
    }

    // ── Guard: duplicate review ───────────────────────────────────────
    public static void UserHasNotAlreadyReviewedBusiness(bool hasExisting)
    {
        if (hasExisting)
            throw new DomainException("You have already submitted a review for this business.");
    }

    // ── Guard: proof ownership / business match ───────────────────────
    public static void ProofMustBelongToUser(Proof proof, Guid userId)
    {
        if (proof.UserId != userId)
            throw new DomainException("The provided proof does not belong to you.");
    }

    public static void ProofMustMatchBusiness(Proof proof, Guid businessId)
    {
        if (proof.BranchId != businessId)
            throw new DomainException("The provided proof does not match this business.");
    }

    public static void ProofMustBeVerified(Proof proof)
    {
        if (!proof.VerifiedByAdminId.HasValue)
            throw new DomainException("Your proof of purchase must be verified before submitting a review.");
    }

    // ── Guard: UserLevel-based proof requirement ──────────────────────
    /// <summary>
    /// LowTrust and Normal users MUST supply a verified proof.
    /// Trusted users may review without proof (recommended but optional).
    /// </summary>
    public static void ProofRequiredForUserLevel(User user, Proof? proof)
    {
        if (!user.TrustScore.RequiresProof) return; // Trusted → no proof required

        if (proof is null)
            throw new DomainException(
                $"Users at {user.Level} level must supply proof of purchase (invoice, receipt, or order ID).");

        ProofMustBeVerified(proof);
    }

    // ── Determination: should this review need admin approval? ───────
    /// <summary>
    /// Returns true when the review must go to Pending state regardless of RiskScore.
    ///   - LowTrust users: always requires manual approval
    ///   - Normal users: requires manual approval if RiskScore ≥ Low threshold
    ///   - Trusted users: requires manual approval if RiskScore ≥ High threshold
    /// </summary>
    public static bool RequiresAdminApproval(User user, decimal riskScoreValue)
    {
        return user.Level switch
        {
            UserLevel.LowTrust => true,
            UserLevel.Normal => riskScoreValue >= RiskScore.LowRiskThreshold,
            UserLevel.Trusted => riskScoreValue >= RiskScore.HighRiskThreshold,
            _ => true
        };
    }

    // ── Convenience helper ──────────────────────────────────────────
    public static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");
    }
}
