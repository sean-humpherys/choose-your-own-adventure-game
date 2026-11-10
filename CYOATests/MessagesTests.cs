// CYOATests/MessagesTests.cs
[TestClass]
public class MessagesTests
{
    [TestMethod]
    public void Constructor_DefaultLanguageIsEnglish()
    {
        Messages messages = new Messages();
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        Assert.AreEqual("Spanish", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        Assert.AreEqual("French", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.SetCurrentLanguage("English");
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void ReadDictionary_LoadsEnglishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Welcome to Choose Your Adventure Game", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsSpanishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenido al Juego de Elige Tu Aventura", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsFrenchWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenue dans Choisissez Votre Aventure", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsFallbackForUnknownKey()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("unknown_key");
        Assert.AreEqual("[unknown_key]", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsDifferentMessageAfterLanguageSwitch()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string english = messages.GetMessage("welcome");

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string spanish = messages.GetMessage("welcome");

        Assert.AreNotEqual(english, spanish);
    }

    [TestMethod]
    public void ReadDictionary_LoadsGoodbyeMessageInEachLanguage()
    {
        Messages messages = new Messages();

        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Adventure ends. Bye.", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        Assert.AreEqual("Adios!", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        Assert.AreEqual("Au revoir!", messages.GetMessage("goodbye"));
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from English dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from Spanish dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from French dictionary.");
        }
    }

    // --- IsValidRace ---

    [TestMethod]
    public void IsValidRace_ReturnsTrue_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("human"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("HUMAN"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForInvalidRace()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace("orc"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace(null));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("human"));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("HUMAN"));
    }

    // --- IsValidOccupation ---

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("fighter"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("FIGHTER"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForInvalidOccupation()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation("wizard"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation(null));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("fighter"));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("FIGHTER"));
    }

    // --- Safety before loading ---

    [TestMethod]
    public void TranslateMethods_BeforeReadDictionary_ReturnInputInsteadOfCrashing()
    {
        Messages messages = new Messages();
        Assert.AreEqual("Elf", messages.TranslateRaceForDisplay("Elf"));
        Assert.AreEqual("Fighter", messages.TranslateOccupationForDisplay("Fighter"));
        Assert.AreEqual("dagger", messages.TranslateWeaponForDisplay("dagger"));
    }

    // --- Language fallback and reload ---

    [TestMethod]
    public void ReadDictionary_UnknownLanguage_FallsBackToEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Klingon");
        messages.ReadDictionary();

        Assert.AreEqual("English", messages.CurrentLanguage);
        Assert.AreEqual("Welcome to Choose Your Adventure Game", messages.GetMessage("welcome"));
    }

    [TestMethod]
    public void SetCurrentLanguage_AfterLoading_ReloadsTextAutomatically()
    {
        Messages messages = new Messages();
        messages.ReadDictionary();                 // English
        messages.SetCurrentLanguage("Spanish");    // no second ReadDictionary call

        Assert.AreEqual("Bienvenido al Juego de Elige Tu Aventura", messages.GetMessage("welcome"));
    }

    // --- Input handling ---

    [TestMethod]
    public void IsValidRace_IgnoresSurroundingSpaces()
    {
        Messages messages = new Messages();
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("  elf  "));
    }

    [TestMethod]
    public void NormalizeRace_UnknownInput_ReturnsTrimmedInput()
    {
        Messages messages = new Messages();
        messages.ReadDictionary();
        Assert.AreEqual("Orc", messages.NormalizeRace("  Orc  "));
    }

    [TestMethod]
    public void NormalizeOccupation_UnknownInput_ReturnsTrimmedInput()
    {
        Messages messages = new Messages();
        messages.ReadDictionary();
        Assert.AreEqual("Wizard", messages.NormalizeOccupation("  Wizard  "));
    }

    // --- Clear errors for a broken language file ---

    [TestMethod]
    public void ReadDictionary_MissingFile_ThrowsClearError()
    {
        Messages messages = new Messages(Path.Combine(Path.GetTempPath(), "does_not_exist_cyoa.json"));
        var ex = Assert.ThrowsException<InvalidOperationException>(() => messages.ReadDictionary());
        StringAssert.Contains(ex.Message, "not found");
    }

    [TestMethod]
    public void ReadDictionary_MalformedJson_ThrowsClearError()
    {
        string path = WriteTempJson("{ this is not json");
        try
        {
            var ex = Assert.ThrowsException<InvalidOperationException>(() => new Messages(path).ReadDictionary());
            StringAssert.Contains(ex.Message, "not valid JSON");
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void ReadDictionary_MissingSection_ThrowsClearError()
    {
        string path = WriteTempJson("""{ "English": { "dictionary": { "welcome": "Hi" } } }""");
        try
        {
            var ex = Assert.ThrowsException<InvalidOperationException>(() => new Messages(path).ReadDictionary());
            StringAssert.Contains(ex.Message, "raceMap");
        }
        finally { File.Delete(path); }
    }

    // --- Every language has the same keys as English ---

    [TestMethod]
    public void LanguageFile_EveryLanguageHasSameKeysAsEnglish()
    {
        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "language_data.json"));
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;

        string[] sections = { "dictionary", "raceMap", "occupationMap",
                              "displayRaceMap", "displayOccupationMap", "displayWeaponMap" };

        foreach (string section in sections)
        {
            // raceMap/occupationMap keys are what the player types, so they differ by
            // language; only the number of entries needs to match for those.
            bool compareKeyNames = section != "raceMap" && section != "occupationMap";
            var englishKeys = root.GetProperty("English").GetProperty(section)
                                  .EnumerateObject().Select(p => p.Name).ToHashSet();

            foreach (var language in root.EnumerateObject().Where(l => l.Name != "English"))
            {
                var keys = language.Value.GetProperty(section)
                                   .EnumerateObject().Select(p => p.Name).ToHashSet();

                if (compareKeyNames)
                {
                    var missing = englishKeys.Except(keys).ToList();
                    Assert.AreEqual(0, missing.Count,
                        $"{language.Name}.{section} is missing: {string.Join(", ", missing)}");
                }
                else
                {
                    Assert.AreEqual(englishKeys.Count, keys.Count,
                        $"{language.Name}.{section} has a different number of entries than English.");
                }
            }
        }
    }

    private static string WriteTempJson(string content)
    {
        string path = Path.Combine(Path.GetTempPath(), $"cyoa_test_{Guid.NewGuid():N}.json");
        File.WriteAllText(path, content);
        return path;
    }
}
