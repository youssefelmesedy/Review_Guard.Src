using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Review_Guard.API.Localization;

public sealed class JsonStringLocalizer : IStringLocalizer
{
    private readonly JsonLocalizationStore _store;
    private readonly CultureInfo? _culture;

    public JsonStringLocalizer(JsonLocalizationStore store, CultureInfo? culture = null)
    {
        _store = store;
        _culture = culture;
    }

    private CultureInfo Culture => _culture ?? CultureInfo.CurrentUICulture;

    public LocalizedString this[string name]
    {
        get
        {
            var value = _store.GetString(Culture, name);
            var resourceNotFound = string.Equals(value, name, StringComparison.Ordinal);
            return new LocalizedString(name, value, resourceNotFound);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var args = arguments.Cast<object?>().ToArray();
            var value = _store.GetString(Culture, name, args);
            var resourceNotFound = string.Equals(value, name, StringComparison.Ordinal);
            return new LocalizedString(name, value, resourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => _store.GetAll(Culture, includeParentCultures)
            .Select(x => new LocalizedString(x.Key, x.Value, resourceNotFound: false));

    public IStringLocalizer WithCulture(CultureInfo culture)
        => new JsonStringLocalizer(_store, culture);
}
