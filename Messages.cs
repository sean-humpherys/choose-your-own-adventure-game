using System.Diagnostics;
using System.Text.Json;

public class Messages
{
    private const string DefaultLanguage = "English";

    private readonly string _jsonPath;
    private bool _isLoaded;

    // Read-only from outside: change it through SetCurrentLanguage so the
    // loaded text always matches the language.
    public string CurrentLanguage { get; private set; } = DefaultLanguage;

    // All maps start empty (never null), so every method is safe to call
    // even before ReadDictionary() runs.
    private Dictionary<string, string> _dictionary = new();

    // Player-input maps ignore case, so "ELF", "elf" and "Elf" all match.
    private Dictionary<string, string> _raceMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _occupationMap = new(StringComparer.OrdinalIgnoreCase);

    // Display maps are keyed by the canonical English values ("Elf", "Fighter", "dagger").
    private Dictionary<string, string> _displayRaceMap = new();
    private Dictionary<string, string> _displayOccupationMap = new();
    private Dictionary<string, string> _displayWeaponMap = new();

    /// <param name="jsonPath">
    /// Optional path to the language file. Defaults to language_data.json next to the app.
    /// </param>
    public Messages(string? jsonPath = null)
    {
        _jsonPath = jsonPath ?? Path.Combine(AppContext.BaseDirectory, "language_data.json");
    }

    /// <summary>
    /// Sets the language. If text was already loaded, it is reloaded in the new
    /// language so CurrentLanguage and the messages never get out of sync.
    /// </summary>
    public void SetCurrentLanguage(string language)
    {
        CurrentLanguage = language;

        if (_isLoaded)
            ReadDictionary();
    }

    /// <summary>
    /// Loads all text for CurrentLanguage from the JSON file.
    /// Falls back to English if the language is not in the file.
    /// Throws InvalidOperationException with a clear message if the file is
    /// missing, malformed, or incomplete.
    /// </summary>
    public void ReadDictionary()
    {
        if (!File.Exists(_jsonPath))
            throw new InvalidOperationException($"Language file not found: '{_jsonPath}'.");

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(File.ReadAllText(_jsonPath));
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Language file '{_jsonPath}' is not valid JSON: {ex.Message}", ex);
        }

        using (doc)
        {
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty(CurrentLanguage, out JsonElement langSection))
            {
                Debug.WriteLine($"[Messages] Language '{CurrentLanguage}' not found; falling back to {DefaultLanguage}.");
                CurrentLanguage = DefaultLanguage;

                if (!root.TryGetProperty(DefaultLanguage, out langSection))
                    throw new InvalidOperationException($"Language file has no '{DefaultLanguage}' section to fall back to.");
            }

            _dictionary = LoadMap(langSection, "dictionary", StringComparer.Ordinal);
            _raceMap = LoadMap(langSection, "raceMap", StringComparer.OrdinalIgnoreCase);
            _occupationMap = LoadMap(langSection, "occupationMap", StringComparer.OrdinalIgnoreCase);
            _displayRaceMap = LoadMap(langSection, "displayRaceMap", StringComparer.Ordinal);
            _displayOccupationMap = LoadMap(langSection, "displayOccupationMap", StringComparer.Ordinal);
            _displayWeaponMap = LoadMap(langSection, "displayWeaponMap", StringComparer.Ordinal);
        }

        _isLoaded = true;
    }

    /// <summary>
    /// Reads one named section (e.g. "raceMap") into a dictionary.
    /// </summary>
    private Dictionary<string, string> LoadMap(JsonElement langSection, string sectionName, StringComparer comparer)
    {
        if (!langSection.TryGetProperty(sectionName, out JsonElement section) ||
            section.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException(
                $"Language '{CurrentLanguage}' is missing the '{sectionName}' section in '{_jsonPath}'.");
        }

        var map = new Dictionary<string, string>(comparer);
        foreach (JsonProperty entry in section.EnumerateObject())
        {
            if (entry.Value.ValueKind != JsonValueKind.String)
            {
                throw new InvalidOperationException(
                    $"Value for '{sectionName}.{entry.Name}' in language '{CurrentLanguage}' must be text.");
            }

            map[entry.Name] = entry.Value.GetString()!;
        }
        return map;
    }

    /// <summary>
    /// Returns the message for a key. If the key is missing, returns "[key]"
    /// so the game keeps running, and writes a warning to the debug output.
    /// </summary>
    public string GetMessage(string key)
    {
        if (_dictionary.TryGetValue(key, out string? text))
            return text;

        Debug.WriteLine($"[Messages] Missing key '{key}' for language '{CurrentLanguage}'.");
        return $"[{key}]";
    }

    public bool IsValidRace(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) && _raceMap.ContainsKey(input.Trim());
    }

    public string NormalizeRace(string input)
    {
        string trimmed = input.Trim();
        return _raceMap.TryGetValue(trimmed, out string? canonical) ? canonical : trimmed;
    }

    public bool IsValidOccupation(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) && _occupationMap.ContainsKey(input.Trim());
    }

    public string NormalizeOccupation(string input)
    {
        string trimmed = input.Trim();
        return _occupationMap.TryGetValue(trimmed, out string? canonical) ? canonical : trimmed;
    }

    public string TranslateRaceForDisplay(string race)
    {
        return _displayRaceMap.TryGetValue(race, out string? translated) ? translated : race;
    }

    public string TranslateOccupationForDisplay(string occupation)
    {
        return _displayOccupationMap.TryGetValue(occupation, out string? translated) ? translated : occupation;
    }

    public string TranslateWeaponForDisplay(string weaponType)
    {
        return _displayWeaponMap.TryGetValue(weaponType, out string? translated) ? translated : weaponType;
    }
}
