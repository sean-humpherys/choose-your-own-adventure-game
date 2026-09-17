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
            MaximumHealthPoints = 30,
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
        // The health line is a format string rather than a label, so it cannot be
        // asserted here; GetStatsDisplay_ShowsHealthPointsAgainstTheMaximum covers it.
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
    [DataRow("English", "Weapon: long sword", "Weapon's Maximum Damage Points: 12")]
    [DataRow("Spanish", "Arma: espada larga", "Puntos Maximos de Dano del Arma: 12")]
    [DataRow("French", "Arme: epee longue", "Points de Dommages Maximum: 12")]
    public void GetStatsDisplay_ShowsTheWeaponAndItsMaximumDamage_WhenCarried(
        string language, string nameLine, string damageLine)
    {
        Messages messages = CreateMessages(language);

        string[] lines = CreateTestPlayer().GetStatsDisplay(messages).Split(Environment.NewLine);

        CollectionAssert.Contains(lines, nameLine);
        CollectionAssert.Contains(lines, damageLine);
    }

    // Validation means a Character always carries a Weapon in play, but the slot follows the
    // same "none" rule as the others so the summary never silently drops one.
    [TestMethod]
    [DataRow("English", "Weapon: none")]
    [DataRow("Spanish", "Arma: ninguna")]
    [DataRow("French", "Arme: aucune")]
    public void GetStatsDisplay_ShowsWeaponNone_WhenNoWeapon(string language, string expectedLine)
    {
        Player player = CreateTestPlayer();
        player.Weapon = null;
        Messages messages = CreateMessages(language);

        string result = player.GetStatsDisplay(messages);

        CollectionAssert.Contains(result.Split(Environment.NewLine), expectedLine);
        Assert.IsFalse(result.Contains(messages.GetMessage("stats_damage")));
        Assert.IsFalse(result.Contains("[stats_"), $"The {language} sheet fell back to a raw key: {result}");
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

    // --- Maximum Health Points: the ceiling healing is capped against ---

    // The roll and the modifier are passed in rather than rolled, because Die is
    // constructed privately inside the Player and cannot be seeded from a test. The
    // arithmetic is the whole point of the seam, so it is asserted directly.
    [TestMethod]
    public void SetHealthPointsAtCreation_RecordsTheMaximumEqualToTheHealthPointsRolled()
    {
        Player player = new Player();

        player.SetHealthPointsAtCreation(14, 3);

        Assert.AreEqual(17, player.HealthPoints);
        Assert.AreEqual(17, player.MaximumHealthPoints);
    }

    // A Halfling's Strength modifier is subtracted, so the ceiling must come down with
    // the roll: the cap reflects the Character actually rolled, not an idealised one.
    [TestMethod]
    public void SetHealthPointsAtCreation_LowersTheMaximum_ForANegativeStrengthModifier()
    {
        Player player = new Player();

        player.SetHealthPointsAtCreation(14, -3);

        Assert.AreEqual(11, player.HealthPoints);
        Assert.AreEqual(11, player.MaximumHealthPoints);
    }

    [TestMethod]
    public void MaximumHealthPoints_IsUntouchedByDamage()
    {
        Player player = new Player();
        player.SetHealthPointsAtCreation(20, 0);

        player.HealthPoints -= player.ReduceDamage(8);

        Assert.AreEqual(12, player.HealthPoints);
        Assert.AreEqual(20, player.MaximumHealthPoints);
    }

    [TestMethod]
    public void NewPlayer_HasNoMaximumHealthPoints()
    {
        Player player = new Player();

        Assert.AreEqual(0, player.MaximumHealthPoints);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetStatsDisplay_ShowsHealthPointsAgainstTheMaximum(string language)
    {
        Player player = CreateTestPlayer();
        Messages messages = CreateMessages(language);

        string result = player.GetStatsDisplay(messages);

        string line = string.Format(
            messages.GetMessage("stats_health_of_max"),
            player.HealthPoints,
            player.MaximumHealthPoints);

        Assert.AreNotEqual("[stats_health_of_max]", messages.GetMessage("stats_health_of_max"),
            $"'stats_health_of_max' is missing from the {language} dictionary.");
        StringAssert.Contains(result, line);
    }

    // The maximum is new; the rest of the sheet is not. This is the guard against the
    // separated health line disturbing the lines around it.
    [TestMethod]
    public void GetStatsDisplay_KeepsItsWeaponArmorAndGoldContent()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");
        Messages messages = CreateEnglishMessages();

        string result = player.GetStatsDisplay(messages);

        StringAssert.Contains(result, messages.GetMessage("stats_gold") + player.Gold);
        StringAssert.Contains(result, messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(player.Weapon!.Type));
        StringAssert.Contains(result, messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        StringAssert.Contains(result, messages.GetMessage("stats_armor") + messages.TranslateArmorForDisplay(player.Armor.Type));
        StringAssert.Contains(result, messages.GetMessage("stats_damage_reduction") + player.Armor.DamageReduction);
    }
}
