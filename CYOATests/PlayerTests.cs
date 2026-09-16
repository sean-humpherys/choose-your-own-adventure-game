[TestClass]
public class PlayerTests
{
    private Messages CreateMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    private Messages CreateEnglishMessages()
    {
        return CreateMessages("English");
    }

    private Player CreateTestPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);

        Assert.IsNull(player.Weapon);
    }

    // The amount is written out rather than read from Player.StartingGold: comparing the
    // constant with itself could never fail. Retuning the purse updates this line too, on purpose.
    [TestMethod]
    public void NewPlayer_CarriesTheFixedStartingGold()
    {
        Player player = new Player();

        Assert.AreEqual(100, player.Gold);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetStatsDisplay_IncludesGoldLine(string language)
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateMessages(language);

        string result = player.GetStatsDisplay(messages);

        string label = messages.GetMessage("stats_gold");
        Assert.AreNotEqual("[stats_gold]", label, $"'stats_gold' is missing from the {language} dictionary.");
        StringAssert.Contains(result, label + player.Gold, $"The {language} character sheet is missing its Gold line.");
    }

    [TestMethod]
    public void GetStatsDisplay_IncludesGoldLine_EvenWithNoWeaponCarried()
    {
        Player player = CreateTestPlayer();
        player.Weapon = null;
        Messages messages = CreateEnglishMessages();

        string result = player.GetStatsDisplay(messages);

        StringAssert.Contains(result, messages.GetMessage("stats_gold") + player.Gold);
    }

    [TestMethod]
    public void GetStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateEnglishMessages();

        string result = player.GetStatsDisplay(messages);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetStatsDisplay_IncludesCharacterDetails()
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateEnglishMessages();

        string result = player.GetStatsDisplay(messages);

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, messages.TranslateRaceForDisplay(player.Race));
        StringAssert.Contains(result, messages.TranslateOccupationForDisplay(player.Occupation));
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
    }

    [TestMethod]
    public void GetStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateEnglishMessages();

        string result = player.GetStatsDisplay(messages);

        StringAssert.Contains(result, messages.GetMessage("stats_header"));
        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_race"));
        StringAssert.Contains(result, messages.GetMessage("stats_occupation"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
    }

    [TestMethod]
    public void GetStatsDisplay_IncludesWeaponLines_WhenWeaponCarried()
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateEnglishMessages();

        string result = player.GetStatsDisplay(messages);

        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
        StringAssert.Contains(result, messages.TranslateWeaponForDisplay(player.Weapon!.Type));
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetStatsDisplay_OmitsWeaponLines_WhenNoWeapon()
    {
        Player player = CreateTestPlayer();
        player.Weapon = null;
        Messages messages = CreateEnglishMessages();

        string result = player.GetStatsDisplay(messages);

        Assert.IsFalse(result.Contains(messages.GetMessage("stats_weapon")));
        Assert.IsFalse(result.Contains(messages.GetMessage("stats_damage")));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetStatsDisplay_RendersInAllThreeLanguages(string language)
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateMessages(language);

        string result = player.GetStatsDisplay(messages);

        StringAssert.Contains(result, messages.GetMessage("stats_header"));
        Assert.IsFalse(result.Contains("[stats_"));
    }

    // Console redirection is process-global and this assembly parallelizes by method,
    // so every test that captures the Console must sit in the same serial phase.
    [TestMethod]
    [DoNotParallelize]
    public void DisplayStats_WritesTheStatsDisplayToTheConsole()
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateEnglishMessages();

        TextWriter originalOut = Console.Out;

        try
        {
            using StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            player.DisplayStats(messages);

            StringAssert.Contains(writer.ToString(), player.GetStatsDisplay(messages));
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
