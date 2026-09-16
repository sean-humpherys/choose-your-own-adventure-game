[TestClass]
public class CombatTests
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

    private Dragon CreateTestDragon()
    {
        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return dragon;
    }

    [TestMethod]
    public void Combat_Constructor_InitializesPlayerAndDragon()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.AreSame(player, combat.player);
        Assert.AreSame(dragon, combat.dragon);
    }

    [TestMethod]
    public void Combat_PlayerRetreated_DefaultsToFalse()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.IsFalse(combat.PlayerRetreated);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesCombatHeaders()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_player_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_dragon_header"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesPlayerCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
        StringAssert.Contains(result, player.Weapon!.Type);
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesDragonCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, dragon.Name);
        StringAssert.Contains(result, dragon.Strength.ToString());
        StringAssert.Contains(result, dragon.Agility.ToString());
        StringAssert.Contains(result, dragon.HealthPoints.ToString());
        StringAssert.Contains(result, dragon.Weapon.Type);
        StringAssert.Contains(result, dragon.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        // 'stats_health' is now the Dragon's label alone. The Character's line is a format
        // string, covered by GetCombatStatsDisplay_ShowsThePlayersHealthAgainstTheMaximum.
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
    }

    // --- The two health lines, now rendered separately ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetCombatStatsDisplay_ShowsThePlayersHealthAgainstTheMaximum(string language)
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateMessages(language);

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.AreNotEqual("[stats_health_of_max]", messages.GetMessage("stats_health_of_max"),
            $"'stats_health_of_max' is missing from the {language} dictionary.");
        StringAssert.Contains(result, string.Format(
            messages.GetMessage("stats_health_of_max"),
            player.HealthPoints,
            player.MaximumHealthPoints));
    }

    // The Dragon has no ceiling and its line must not grow one, which is the reason the
    // two health lines were separated rather than the shared key being changed in place.
    [TestMethod]
    public void GetCombatStatsDisplay_ShowsTheDragonsHealthWithoutAMaximum()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        // Asserted as a whole line rather than a substring: the Character's line shares the
        // label, so a Contains check would still pass if the Dragon's line grew a maximum.
        string[] lines = result.Split(Environment.NewLine);

        CollectionAssert.Contains(lines, messages.GetMessage("stats_health") + dragon.HealthPoints);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_KeepsItsWeaponAndArmorContent()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(player.Weapon!.Type));
        StringAssert.Contains(result, messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        StringAssert.Contains(result, messages.GetMessage("stats_armor") + messages.TranslateArmorForDisplay(player.Armor.Type));
        StringAssert.Contains(result, messages.GetMessage("stats_damage_reduction") + player.Armor.DamageReduction);
    }
}
