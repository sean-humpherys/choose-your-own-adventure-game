// CYOATests/LanguageParityTests.cs
using System.Text;
using System.Text.Json;

// Parity is asserted for the catalogue sections that are keyed by a language-neutral
// value, so every language must carry every key. The remaining two sections, raceMap and
// occupationMap, are deliberately excluded: they map the words a player may *type* to the
// canonical value, so Spanish keys on "humano" and French on "humain". Their keys differ
// across languages by design, and asserting parity there would be wrong, not stricter.
[TestClass]
public class LanguageParityTests
{
    private static readonly string[] Languages = { "English", "Spanish", "French" };

    [TestMethod]
    public void Dictionary_HasIdenticalKeysInEveryLanguage()
    {
        AssertSectionKeysMatchAcrossLanguages("dictionary");
    }

    [TestMethod]
    public void DisplayRaceMap_HasIdenticalKeysInEveryLanguage()
    {
        AssertSectionKeysMatchAcrossLanguages("displayRaceMap");
    }

    [TestMethod]
    public void DisplayOccupationMap_HasIdenticalKeysInEveryLanguage()
    {
        AssertSectionKeysMatchAcrossLanguages("displayOccupationMap");
    }

    [TestMethod]
    public void DisplayWeaponMap_HasIdenticalKeysInEveryLanguage()
    {
        AssertSectionKeysMatchAcrossLanguages("displayWeaponMap");
    }

    [TestMethod]
    public void DisplayArmorMap_HasIdenticalKeysInEveryLanguage()
    {
        AssertSectionKeysMatchAcrossLanguages("displayArmorMap");
    }

    [TestMethod]
    public void DisplayPotionMap_HasIdenticalKeysInEveryLanguage()
    {
        AssertSectionKeysMatchAcrossLanguages("displayPotionMap");
    }

    // Every key that appears in any language is expected in all of them, so a key
    // added to one language alone fails no matter which language it was added to.
    private static void AssertSectionKeysMatchAcrossLanguages(string section)
    {
        Dictionary<string, HashSet<string>> keysByLanguage = new Dictionary<string, HashSet<string>>();
        foreach (string language in Languages)
            keysByLanguage[language] = ReadSectionKeys(language, section);

        HashSet<string> expected = new HashSet<string>();
        foreach (HashSet<string> keys in keysByLanguage.Values)
            expected.UnionWith(keys);

        StringBuilder failures = new StringBuilder();
        foreach (string language in Languages)
        {
            List<string> missing = expected.Except(keysByLanguage[language]).OrderBy(key => key).ToList();
            if (missing.Count > 0)
                failures.AppendLine($"  {language} is missing {missing.Count} key(s) from '{section}': {string.Join(", ", missing)}");
        }

        if (failures.Length > 0)
            Assert.Fail($"Language catalogue is out of parity in '{section}'.{Environment.NewLine}{failures}"
                      + $"Add the key(s) above to language_data.json for each language listed.");
    }

    private static HashSet<string> ReadSectionKeys(string language, string section)
    {
        string jsonPath = Path.Combine(AppContext.BaseDirectory, "language_data.json");
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(jsonPath));

        Assert.IsTrue(doc.RootElement.TryGetProperty(language, out JsonElement langSection),
            $"language_data.json has no '{language}' section.");
        Assert.IsTrue(langSection.TryGetProperty(section, out JsonElement sectionElement),
            $"language_data.json has no '{section}' section under '{language}'.");

        HashSet<string> keys = new HashSet<string>();
        foreach (JsonProperty entry in sectionElement.EnumerateObject())
            keys.Add(entry.Name);
        return keys;
    }
}
