using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace Review_Guard.API.Localization;

public sealed class JsonLocalizationStore
{
    private const string FallbackCulture = "en";

    private readonly string _resourcesPath;
    private readonly ILogger<JsonLocalizationStore> _logger;
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _cache = new();

    public JsonLocalizationStore(
        IWebHostEnvironment environment,
        ILogger<JsonLocalizationStore> logger)
    {
        _resourcesPath = Path.Combine(environment.ContentRootPath, "Resources");
        _logger = logger;
    }

    public string GetString(CultureInfo culture, string key, params object?[] args)
    {
        var template = FindRawValue(culture, key);
        if (template is null)
            return SafeFormat(key, args);

        return SafeFormat(template, args);
    }

    public IEnumerable<KeyValuePair<string, string>> GetAll(CultureInfo culture, bool includeParentCultures)
    {
        var orderedCultures = includeParentCultures
            ? BuildCultureFallbackChain(culture).Reverse().ToArray()
            : [NormalizeCulture(culture.Name)];

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in orderedCultures)
        {
            var entries = LoadCulture(name);
            foreach (var entry in entries)
                map[entry.Key] = entry.Value;
        }

        return map;
    }

    private string? FindRawValue(CultureInfo culture, string key)
    {
        foreach (var cultureName in BuildCultureFallbackChain(culture))
        {
            var entries = LoadCulture(cultureName);
            if (entries.TryGetValue(key, out var value))
                return value;
        }

        return null;
    }

    private IReadOnlyDictionary<string, string> LoadCulture(string cultureName)
        => _cache.GetOrAdd(cultureName, LoadCultureCore);

    private IReadOnlyDictionary<string, string> LoadCultureCore(string cultureName)
    {
        var filePath = Path.Combine(_resourcesPath, $"{cultureName}.json");
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Localization file not found: {FilePath}", filePath);
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        using var stream = File.OpenRead(filePath);
        using var document = JsonDocument.Parse(stream);

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        FlattenJsonObject(document.RootElement, prefix: null, map);
        return map;
    }

    private static string[] BuildCultureFallbackChain(CultureInfo culture)
    {
        var values = new List<string>(3)
        {
            NormalizeCulture(culture.Name)
        };

        var twoLetter = NormalizeCulture(culture.TwoLetterISOLanguageName);
        if (!values.Contains(twoLetter, StringComparer.OrdinalIgnoreCase))
            values.Add(twoLetter);

        if (!values.Contains(FallbackCulture, StringComparer.OrdinalIgnoreCase))
            values.Add(FallbackCulture);

        return values.ToArray();
    }

    private static string NormalizeCulture(string value)
        => string.IsNullOrWhiteSpace(value)
            ? FallbackCulture
            : value.Trim().ToLowerInvariant();

    private static void FlattenJsonObject(
        JsonElement element,
        string? prefix,
        IDictionary<string, string> output)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return;

        foreach (var property in element.EnumerateObject())
        {
            var key = string.IsNullOrEmpty(prefix)
                ? property.Name
                : $"{prefix}.{property.Name}";

            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                FlattenJsonObject(property.Value, key, output);
                continue;
            }

            if (property.Value.ValueKind == JsonValueKind.String)
            {
                output[key] = property.Value.GetString() ?? string.Empty;
                continue;
            }

            output[key] = property.Value.ToString();
        }
    }

    private static string SafeFormat(string template, object?[] args)
    {
        if (args.Length == 0)
            return template;

        try
        {
            return string.Format(CultureInfo.CurrentCulture, template, args);
        }
        catch (FormatException)
        {
            return template;
        }
    }
}
