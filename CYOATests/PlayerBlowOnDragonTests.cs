// CYOATests/PlayerBlowOnDragonTests.cs
// Characterization tests: they record what the Character's attack on the Dragon does
// today, drift included, so the Blow refactor can prove it changed nothing. Each
// combatant holds its own scripted Die, pinning which Die makes each roll.
[TestClass]
public class PlayerBlowOnDragonTests
{
    private Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Strength 10, so an Attack Roll of 10 lands and 11 misses.
    private Player CreateCharacter(ScriptedDie die)
    {
        return new Player(die)
        {
            Name = "TestHero",
            Race = "Elf",
            Occupation = "Fighter",
            Strength = 10,
            Agility = 5,
            HealthPoints = 20,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private string[] Opening(Messages messages)
    {
        return
        [
            string.Format(
                messages.GetMessage("player_attack_intro"),
                messages.TranslateOccupationForDisplay("Fighter"),
                messages.TranslateWeaponForDisplay("long sword")),
            "-)=====>"
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
        ScriptedDie characterDie = new ScriptedDie(11);
        ScriptedDie dragonDie = ScriptedDie.ForDragon(8, 7, 15);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = player.Attack(dragon, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("player_missed_dragon"), "Smolderfang")]),
            result);
        Assert.AreEqual(15, dragon.HealthPoints);
        Assert.AreEqual(20, player.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_EqualToStrength_Lands_AndDefenseEqualToAgility_IsDodgedWithATaunt(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie(10, 7);
        ScriptedDie dragonDie = ScriptedDie.ForDragon(8, 7, 15, 3);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = player.Attack(dragon, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("player_hit_dragon"), "Smolderfang"),
                string.Format(messages.GetMessage("dragon_defended_player_attack"), "Smolderfang"),
                string.Format(messages.GetMessage("dragon_defense_taunt_3"), messages.TranslateRaceForDisplay("Elf"))]),
            result);
        Assert.AreEqual(15, dragon.HealthPoints);
        Assert.AreEqual(20, player.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_ThatWounds_NarratesDamageAndRemainingHealthPoints_ThenTheDragonsReply(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie(10, 8, 6);
        ScriptedDie dragonDie = ScriptedDie.ForDragon(8, 7, 15, 2);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = player.Attack(dragon, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("player_hit_dragon"), "Smolderfang"),
                string.Format(messages.GetMessage("player_damage_dealt"), 6),
                string.Format(messages.GetMessage("dragon_health_now"), "Smolderfang", 9),
                messages.GetMessage("dragon_damage_reply_2")]),
            result);
        Assert.AreEqual(9, dragon.HealthPoints);
        Assert.AreEqual(20, player.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_ThatBringsTheDragonToExactlyZero_NarratesItsDefeat_WithNoReplyRoll(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie(10, 8, 6);
        ScriptedDie dragonDie = ScriptedDie.ForDragon(8, 7, 6);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = player.Attack(dragon, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("player_hit_dragon"), "Smolderfang"),
                string.Format(messages.GetMessage("player_damage_dealt"), 6),
                string.Format(messages.GetMessage("dragon_health_now"), "Smolderfang", 0),
                messages.GetMessage("dragon_defeated_narrative")]),
            result);
        Assert.AreEqual(0, dragon.HealthPoints);
        Assert.AreEqual(20, player.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    // Today's drift, recorded rather than fixed: the Dragon's Health Points aren't floored.
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_ThatOverkills_LeavesAndNarratesTheDragonsHealthPointsBelowZero(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie(10, 8, 9);
        ScriptedDie dragonDie = ScriptedDie.ForDragon(8, 7, 4);
        Player player = CreateCharacter(characterDie);
        Dragon dragon = new Dragon(dragonDie);

        string result = player.Attack(dragon, messages);

        Assert.AreEqual(
            Lines([.. Opening(messages),
                string.Format(messages.GetMessage("player_hit_dragon"), "Smolderfang"),
                string.Format(messages.GetMessage("player_damage_dealt"), 9),
                string.Format(messages.GetMessage("dragon_health_now"), "Smolderfang", -5),
                messages.GetMessage("dragon_defeated_narrative")]),
            result);
        Assert.AreEqual(-5, dragon.HealthPoints);
        Assert.AreEqual(20, player.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }

    // Today's drift, recorded rather than fixed: the sentence is English in every language.
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Attack_WithNoWeapon_ReturnsTheEnglishSentence_AndRollsNothing(string language)
    {
        Messages messages = GetMessages(language);
        ScriptedDie characterDie = new ScriptedDie();
        ScriptedDie dragonDie = ScriptedDie.ForDragon(8, 7, 15);
        Player player = CreateCharacter(characterDie);
        player.Weapon = null;
        Dragon dragon = new Dragon(dragonDie);

        string result = player.Attack(dragon, messages);

        Assert.AreEqual("You have no weapon to attack with.", result);
        Assert.AreEqual(15, dragon.HealthPoints);
        Assert.AreEqual(20, player.HealthPoints);
        characterDie.AssertAllRollsUsed();
        dragonDie.AssertAllRollsUsed();
    }
}
