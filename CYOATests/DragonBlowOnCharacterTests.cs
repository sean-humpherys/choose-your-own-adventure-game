// CYOATests/DragonBlowOnCharacterTests.cs
// Characterization tests: they record what the Dragon's attack on the Character does
// today, so the Blow refactor can prove it changed nothing. The Dragon's scripted Die
// makes every roll; the Character holds an empty one, so any roll routed to it fails.
[TestClass]
public class DragonBlowOnCharacterTests
{
    private Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Agility 5, so a Defense Roll of 5 dodges and 6 does not.
    private Player CreateCharacter(ScriptedDie die, int healthPoints = 20)
    {
        return new Player(die)
        {
            Name = "TestHero",
            Race = "Elf",
            Occupation = "Fighter",
            Strength = 10,
            Agility = 5,
            HealthPoints = healthPoints,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // Strength 8, so an Attack Roll of 8 lands and 9 misses. The Dragon's statistics
    // take its Die's first three rolls; the Blow's rolls follow.
    private ScriptedDie DragonDie(params int[] blowRolls)
    {
        return ScriptedDie.ForDragon(8, 7, 15, blowRolls);
    }

    private string[] Opening(Messages messages)
    {
        return
        [
            string.Format(
                messages.GetMessage("dragon_attack_intro"),
                "Smolderfang",
                "TestHero",
                messages.TranslateWeaponForDisplay("claws")),
            "<<< claws >>>"
        ];
    }

    private static string Lines(params string[] lines)
    {
        return string.Join(Environment.NewLine, lines);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_OneAboveStrength_Misses_AndUsesOneRoll(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie();
        ScriptedDie dragonDie = DragonDie(9);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("dragon_missed_player"), "Smolderfang", "TestHero")]),
            result);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(15, dragon.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_EqualToStrength_Lands_AndDefenseEqualToAgility_IsDodged_UsingTwoRolls(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie();
        ScriptedDie dragonDie = DragonDie(8, 5);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("dragon_hit_player"), "Smolderfang", "TestHero"),
                string.Format(messages.GetMessage("player_defended_dragon_attack"), "TestHero")]),
            result);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(15, dragon.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_ThatWounds_WithoutArmor_TakesTheFullDamage(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie();
        ScriptedDie dragonDie = DragonDie(8, 6, 7);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("dragon_hit_player"), "Smolderfang", "TestHero"),
                string.Format(messages.GetMessage("dragon_damage_dealt"), "Smolderfang", 7),
                string.Format(messages.GetMessage("player_health_now"), "TestHero", 13)]),
            result);
        Assert.AreEqual(13, player.HealthPoints);
        Assert.AreEqual(15, dragon.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    // ADR-0001: the Damage passes through the Character's Damage Reduction.
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_ThatWounds_WithArmor_TakesTheDamageLessTheDamageReduction(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie();
        ScriptedDie dragonDie = DragonDie(8, 6, 7);
        Player player = CreateCharacter(characterDie);
        player.Armor = new Armor("magical armor", 3, "[###]");
        Dragon dragon = new Dragon(dragonDie);

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("dragon_hit_player"), "Smolderfang", "TestHero"),
                string.Format(messages.GetMessage("dragon_damage_dealt"), "Smolderfang", 4),
                string.Format(messages.GetMessage("player_health_now"), "TestHero", 16)]),
            result);
        Assert.AreEqual(16, player.HealthPoints);
        Assert.AreEqual(15, dragon.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    // The Armor stopping the whole blow is narrated as absorbed, never as a dodge. A blow
    // below the Damage Reduction must not heal the Character or be narrated as negative.
    [TestMethod]
    [DataRow("English", 3)]
    [DataRow("Spanish", 3)]
    [DataRow("French", 3)]
    [DataRow("English", 2)]
    [DataRow("Spanish", 2)]
    [DataRow("French", 2)]
    public void Attack_WhoseDamageReductionCancelsTheDamage_IsNarratedAsAbsorbed_NotDodged(string language, int damageRoll)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie();
        ScriptedDie dragonDie = DragonDie(8, 6, damageRoll);
        Player player = CreateCharacter(characterDie);
        player.Armor = new Armor("magical armor", 3, "[###]");
        Dragon dragon = new Dragon(dragonDie);

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("dragon_hit_player"), "Smolderfang", "TestHero"),
                string.Format(messages.GetMessage("player_armor_absorbed"), "TestHero"),
                string.Format(messages.GetMessage("player_health_now"), "TestHero", 20)]),
            result);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(15, dragon.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_ThatIsLethal_StopsHealthPointsAtZero_AndNarratesTheDefeat(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie();
        ScriptedDie dragonDie = DragonDie(8, 6, 12);
        Player player = CreateCharacter(characterDie, healthPoints: 5);
        Dragon dragon = new Dragon(dragonDie);

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("dragon_hit_player"), "Smolderfang", "TestHero"),
                string.Format(messages.GetMessage("dragon_damage_dealt"), "Smolderfang", 12),
                string.Format(messages.GetMessage("player_health_now"), "TestHero", 0),
                string.Format(messages.GetMessage("player_defeated_narrative"), "TestHero")]),
            result);
        Assert.AreEqual(0, player.HealthPoints);
        Assert.AreEqual(15, dragon.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }
}
