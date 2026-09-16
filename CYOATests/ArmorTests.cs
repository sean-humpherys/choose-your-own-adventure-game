// CYOATests/ArmorTests.cs
[TestClass]
public class ArmorTests
{
    private Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
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

    // --- ReduceDamage: the one pure step every blow goes through ---

    [TestMethod]
    public void ReduceDamage_TakesExactlyTheReduction_FromABlowLargerThanIt()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");

        Assert.AreEqual(7, player.ReduceDamage(10));
    }

    [TestMethod]
    public void ReduceDamage_ReturnsZero_ForABlowEqualToTheReduction()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");

        Assert.AreEqual(0, player.ReduceDamage(3));
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(1)]
    [DataRow(0)]
    public void ReduceDamage_FloorsAtZero_AndNeverGoesNegative(int blow)
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");

        Assert.AreEqual(0, player.ReduceDamage(blow));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(7)]
    [DataRow(12)]
    public void ReduceDamage_PassesTheBlowThroughUnchanged_WhenNoArmorIsWorn(int blow)
    {
        Player player = CreateTestPlayer();

        Assert.IsNull(player.Armor);
        Assert.AreEqual(blow, player.ReduceDamage(blow));
    }

    [TestMethod]
    public void ReduceDamage_LeavesHealthPointsAlone()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");
        int before = player.HealthPoints;

        player.ReduceDamage(10);

        Assert.AreEqual(before, player.HealthPoints);
    }

    // An absorbed blow must not heal: subtracting the reduced Damage can only lower Health Points.
    [TestMethod]
    public void ReduceDamage_CannotRestoreHealthPoints()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");

        int reduced = player.ReduceDamage(1);
        int before = player.HealthPoints;
        player.HealthPoints -= reduced;

        Assert.IsTrue(player.HealthPoints <= before);
        Assert.AreEqual(before, player.HealthPoints);
    }

    // --- Armor does not touch the rolls (ADR-0001) ---

    [TestMethod]
    public void WearingArmor_ChangesNeitherStrengthNorAgility()
    {
        Player player = CreateTestPlayer();
        int strength = player.Strength;
        int agility = player.Agility;

        player.Armor = new Armor("magical armor", 3, "[###]");

        Assert.AreEqual(strength, player.Strength);
        Assert.AreEqual(agility, player.Agility);
    }

    [TestMethod]
    public void AttackHits_IsUnaffectedByArmor()
    {
        Player player = CreateTestPlayer();
        bool before = player.AttackHits(player.Strength);

        player.Armor = new Armor("magical armor", 3, "[###]");

        Assert.AreEqual(before, player.AttackHits(player.Strength));
    }

    // --- Both stats displays ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void CharacterSheet_ShowsTheArmorAndItsDamageReduction_WhenWorn(string language)
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");
        Messages messages = GetMessages(language);

        string sheet = player.GetStatsDisplay(messages);

        StringAssert.Contains(sheet, messages.GetMessage("stats_armor"));
        StringAssert.Contains(sheet, messages.GetMessage("stats_damage_reduction") + player.Armor.DamageReduction);
        StringAssert.Contains(sheet, messages.TranslateArmorForDisplay(player.Armor.Type));
        Assert.IsFalse(sheet.Contains("[stats_"), $"The {language} sheet fell back to a raw key: {sheet}");
    }

    [TestMethod]
    public void CharacterSheet_ShowsNoArmorLines_WhenNoneIsWorn()
    {
        Player player = CreateTestPlayer();
        Messages messages = GetMessages("English");

        string sheet = player.GetStatsDisplay(messages);

        Assert.IsFalse(sheet.Contains(messages.GetMessage("stats_armor")));
        Assert.IsFalse(sheet.Contains(messages.GetMessage("stats_damage_reduction")));
    }

    [TestMethod]
    public void CombatStatsDisplay_ShowsTheArmorAndItsDamageReduction_WhenWorn()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor("magical armor", 3, "[###]");
        Messages messages = GetMessages("English");

        Combat combat = new Combat(player, new Dragon(new Die()), messages);
        string display = combat.GetCombatStatsDisplay();

        StringAssert.Contains(display, messages.GetMessage("stats_armor"));
        StringAssert.Contains(display, messages.GetMessage("stats_damage_reduction") + player.Armor.DamageReduction);
        StringAssert.Contains(display, messages.TranslateArmorForDisplay(player.Armor.Type));
    }

    [TestMethod]
    public void CombatStatsDisplay_ShowsNoArmorLines_WhenNoneIsWorn()
    {
        Player player = CreateTestPlayer();
        Messages messages = GetMessages("English");

        Combat combat = new Combat(player, new Dragon(new Die()), messages);
        string display = combat.GetCombatStatsDisplay();

        Assert.IsFalse(display.Contains(messages.GetMessage("stats_armor")));
        Assert.IsFalse(display.Contains(messages.GetMessage("stats_damage_reduction")));
    }

    // English maps each name to itself, so only the other two can show a mapping by differing.
    [TestMethod]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void ArmorIsNamedInEveryLanguage(string language)
    {
        Messages messages = GetMessages(language);

        string translated = messages.TranslateArmorForDisplay(Merchant.ArmorType);

        Assert.AreNotEqual(Merchant.ArmorType, translated,
            $"'{Merchant.ArmorType}' is missing from the {language} armor display map.");
    }

    // --- The absorbed blow ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void AbsorbedBlowMessage_IsDistinctFromTheDodgeMessage(string language)
    {
        Messages messages = GetMessages(language);

        string absorbed = messages.GetMessage("player_armor_absorbed");
        string dodged = messages.GetMessage("player_defended_dragon_attack");

        Assert.AreNotEqual($"[player_armor_absorbed]", absorbed,
            $"'player_armor_absorbed' is missing from the {language} dictionary.");
        Assert.AreNotEqual(dodged, absorbed,
            $"The {language} absorbed-blow narrative reuses the dodge message.");
    }

    // The Die cannot be seeded, so the Dragon's blow is driven through the pure step it
    // now routes through, rather than depending on a roll.
    [TestMethod]
    public void ArmorAbsorbsTheSmallestPossibleBlowEntirely()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor(Merchant.ArmorType, Merchant.ArmorDamageReduction, "[###]");
        int before = player.HealthPoints;

        player.HealthPoints -= player.ReduceDamage(1);

        Assert.AreEqual(before, player.HealthPoints);
    }

    [TestMethod]
    public void ArmorLetsTheDragonsHardestBlowThroughMinusTheReduction()
    {
        Player player = CreateTestPlayer();
        player.Armor = new Armor(Merchant.ArmorType, Merchant.ArmorDamageReduction, "[###]");
        Dragon dragon = new Dragon(new Die());
        int hardest = dragon.Weapon.MaxDamage;

        Assert.AreEqual(hardest - Merchant.ArmorDamageReduction, player.ReduceDamage(hardest));
    }
}
