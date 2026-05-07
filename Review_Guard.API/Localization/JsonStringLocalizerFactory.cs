using Microsoft.Extensions.Localization;

namespace Review_Guard.API.Localization;

public sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly JsonStringLocalizer _localizer;

    public JsonStringLocalizerFactory(JsonLocalizationStore store)
        => _localizer = new JsonStringLocalizer(store);

    public IStringLocalizer Create(Type resourceSource) => _localizer;

    public IStringLocalizer Create(string baseName, string location) => _localizer;
}