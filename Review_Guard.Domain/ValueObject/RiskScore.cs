using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.ValueObject;

public sealed class RiskScore : ValueObject
{
    public const decimal MinValue = 0m;
    public const decimal MaxValue = 100m;

    public const decimal LowRiskThreshold = 30m;
    public const decimal MediumRiskThreshold = 60m;
    public const decimal HighRiskThreshold = 80m;

    public decimal Value { get; }

    private RiskScore(decimal value)
    {
        Value = Math.Clamp(value, MinValue, MaxValue);
    }

    public static RiskScore Zero() => new(0m);

    public static RiskScore Create(decimal value) => new(value);

    // ── Level (Enum بدل string) ───────────────────────────────
    public RiskLevel Level => Value switch
    {
        < LowRiskThreshold => RiskLevel.Low,
        < MediumRiskThreshold => RiskLevel.Medium,
        < HighRiskThreshold => RiskLevel.High,
        _ => RiskLevel.Critical
    };

    // ── Flags ────────────────────────────────────────────────
    public bool IsLow => Level == RiskLevel.Low;
    public bool IsMedium => Level == RiskLevel.Medium;
    public bool IsHigh => Level == RiskLevel.High;
    public bool IsCritical => Level == RiskLevel.Critical;

    public bool RequiresManualReview => Level != RiskLevel.Low;

    // ── Equality ─────────────────────────────────────────────
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    // ── Debugging / Logging ──────────────────────────────────
    public override string ToString() => $"{Value:F1} ({Level})";
}