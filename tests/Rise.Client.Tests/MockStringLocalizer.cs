using Microsoft.Extensions.Localization;

namespace Rise.Client;
public class MockStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly Dictionary<string, string> _translations = new();

    public LocalizedString this[string name] => 
        new LocalizedString(name, _translations.GetValueOrDefault(name, name), !_translations.ContainsKey(name));
    
    public LocalizedString this[string name, params object[] arguments] => 
        new LocalizedString(name, string.Format(_translations.GetValueOrDefault(name, name), arguments));
    
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => 
        _translations.Select(kvp => new LocalizedString(kvp.Key, kvp.Value));

    public void AddTranslation(string key, string value) => _translations[key] = value;
}