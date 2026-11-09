using System.Text.Json;

/// <summary>
/// Loads the text for the chosen language from language_data.json and handles
/// lookups: messages, race/occupation input, and translated display names.
/// </summary>
public class Messages
{
    public const string DefaultLanguage = "English";
    private const string DefaultFileName = "language_data.json";

    // Language is changed through SetCurrentLanguage so it can't be changed by accident elsewhere.
    public string CurrentLanguage { get; private set; } = DefaultLanguage;

    // All maps start empty (never null) so every lookup is safe even before ReadDictionary() runs.
    // Case-insensitive keys mean "ELF", "elf" and "Elf" all match without calling ToLower().
    private Dictionary<string, string> _dictionary = NewMap();
    private Dictionary<string, string> _raceMap = NewMap();
    private Dictionary<string, string> _occupationMap = NewMap();
    private Dictionary<string, string> _displayRaceMap = NewMap();
    private Dictionary<string, string> _displayOccupationMap = NewMap();
    private Dictionary<string, string> _displayWeaponMap = NewMap();

    public void SetCurrentLanguage(string language)
    {
        CurrentLanguage = string.IsNullOrWhiteSpace(language) ? DefaultLanguage : language.Trim();
    }

    /// <summary>
    /// Loads the current language from language_data.json. A different file path can be
    /// passed in (useful for tests). If the language isn't in the file, English is used.
    /// If the file is missing or broken, an error is shown and messages display as [key].
    /// </summary>
    public void ReadDictionary(string? jsonPath = null)
    {
        jsonPath ??= Path.Combine(AppContext.BaseDirectory, DefaultFileName);

        try
        {
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty(CurrentLanguage, out JsonElement langSection))
            {
                Console.Error.WriteLine($"Language '{CurrentLanguage}' not found. Using {DefaultLanguage}.");
                CurrentLanguage = DefaultLanguage;

                if (!root.TryGetProperty(DefaultLanguage, out langSection))
                {
                    Console.Error.WriteLine($"Language '{DefaultLanguage}' not found in {jsonPath}.");
                    return;
                }
            }

            _dictionary = LoadMap(langSection, "dictionary");
            _raceMap = LoadMap(langSection, "raceMap");
            _occupationMap = LoadMap(langSection, "occupationMap");
            _displayRaceMap = LoadMap(langSection, "displayRaceMap");
            _displayOccupationMap = LoadMap(langSection, "displayOccupationMap");
            _displayWeaponMap = LoadMap(langSection, "displayWeaponMap");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            Console.Error.WriteLine($"Could not load language file '{jsonPath}': {ex.Message}");
        }
    }

    /// <summary>Returns the message for a key, or "[key]" if it is missing.</summary>
    public string GetMessage(string key)
    {
        return _dictionary.TryGetValue(key, out string? message) ? message : $"[{key}]";
    }

    public bool IsValidRace(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) && _raceMap.ContainsKey(input.Trim());
    }

    /// <summary>Converts input (e.g. "elfo") to the English race name. Call IsValidRace first.</summary>
    public string NormalizeRace(string input)
    {
        return Lookup(_raceMap, input.Trim());
    }

    public bool IsValidOccupation(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) && _occupationMap.ContainsKey(input.Trim());
    }

    /// <summary>Converts input (e.g. "ladron") to the English occupation name. Call IsValidOccupation first.</summary>
    public string NormalizeOccupation(string input)
    {
        return Lookup(_occupationMap, input.Trim());
    }

    public string TranslateRaceForDisplay(string race) => Lookup(_displayRaceMap, race);

    public string TranslateOccupationForDisplay(string occupation) => Lookup(_displayOccupationMap, occupation);

    public string TranslateWeaponForDisplay(string weaponType) => Lookup(_displayWeaponMap, weaponType);

    // ---------- helpers ----------

    private static Dictionary<string, string> NewMap() => new(StringComparer.OrdinalIgnoreCase);

    // Returns the mapped value, or the original text if there is no match.
    private static string Lookup(Dictionary<string, string> map, string key)
    {
        return map.TryGetValue(key, out string? value) ? value : key;
    }

    // Reads one section (e.g. "raceMap") into a dictionary. A missing section gives an
    // empty map, and non-string values are skipped instead of crashing the game.
    private static Dictionary<string, string> LoadMap(JsonElement langSection, string sectionName)
    {
        var map = NewMap();

        if (!langSection.TryGetProperty(sectionName, out JsonElement section) ||
            section.ValueKind != JsonValueKind.Object)
        {
            Console.Error.WriteLine($"Language section '{sectionName}' is missing.");
            return map;
        }

        foreach (JsonProperty entry in section.EnumerateObject())
        {
            if (entry.Value.ValueKind == JsonValueKind.String)
                map[entry.Name] = entry.Value.GetString()!;
        }

        return map;
    }
}
