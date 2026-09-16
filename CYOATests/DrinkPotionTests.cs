// CYOATests/DrinkPotionTests.cs
// The healing arithmetic is asserted through Player.DrinkPotion, which returns its
// narrative rather than writing to the Console, the way the attack methods return theirs.
// Nothing here depends on a roll: the restore is flat by design, which is the trade
// ADR-0002 records and the reason these boundaries can be asserted at all.
[TestClass]
public class DrinkPotionTests
{
    private Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Maximum Health Points 30, restore 8, so each boundary can be reached exactly.
    private Player CreateWoundedPlayer(int healthPoints)
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = healthPoints,
            MaximumHealthPoints = 30,
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Potion = new Potion("healing potion", 8, "(~~~)")
        };
    }

    // --- The restore, across its boundaries ---

    [TestMethod]
    public void DrinkPotion_RestoresTheFlatAmount_WhenWoundedByMoreThanIt()
    {
        Player player = CreateWoundedPlayer(10);

        player.DrinkPotion(GetMessages("English"));

        Assert.AreEqual(18, player.HealthPoints);
    }

    [TestMethod]
    public void DrinkPotion_ReturnsToTheMaximum_WhenWoundedByExactlyTheRestore()
    {
        Player player = CreateWoundedPlayer(22);

        player.DrinkPotion(GetMessages("English"));

        Assert.AreEqual(30, player.HealthPoints);
    }

    // The clamp is the rule ADR-0002 protects: the Potion can never make a Character
    // stronger than the one rolled at creation.
    [TestMethod]
    public void DrinkPotion_StopsAtTheMaximum_WhenWoundedByLessThanTheRestore()
    {
        Player player = CreateWoundedPlayer(27);

        player.DrinkPotion(GetMessages("English"));

        Assert.AreEqual(30, player.HealthPoints);
        Assert.AreEqual(30, player.MaximumHealthPoints);
    }

    [TestMethod]
    public void DrinkPotion_LeavesTheMaximumUntouched()
    {
        Player player = CreateWoundedPlayer(10);

        player.DrinkPotion(GetMessages("English"));

        Assert.AreEqual(30, player.MaximumHealthPoints);
    }

    [TestMethod]
    public void DrinkPotion_ConsumesThePotion()
    {
        Player player = CreateWoundedPlayer(10);

        player.DrinkPotion(GetMessages("English"));

        Assert.IsNull(player.Potion);
    }

    // --- The two refusals, both of which keep the Potion ---

    [TestMethod]
    public void DrinkPotion_IsRefusedAtMaximumHealthPoints_AndKeepsThePotion()
    {
        Player player = CreateWoundedPlayer(30);
        Potion carried = player.Potion!;

        player.DrinkPotion(GetMessages("English"));

        Assert.AreEqual(30, player.HealthPoints);
        Assert.AreSame(carried, player.Potion);
    }

    [TestMethod]
    public void DrinkPotion_WithNoPotion_ChangesNothing()
    {
        Player player = CreateWoundedPlayer(10);
        player.Potion = null;

        player.DrinkPotion(GetMessages("English"));

        Assert.AreEqual(10, player.HealthPoints);
        Assert.IsNull(player.Potion);
    }

    // --- The narratives, which the player tells the three cases apart by ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void DrinkPotion_StatesTheAmountRestoredAndTheResultingHealthPoints(string language)
    {
        Player player = CreateWoundedPlayer(10);
        Messages messages = GetMessages(language);

        string narrative = player.DrinkPotion(messages);

        Assert.IsFalse(narrative.Contains("["), $"The {language} narrative fell back to a message key: {narrative}");

        // The whole line, not the bare numbers: "8" is a substring of "18", so asserting
        // the two separately would have cost nothing to pass.
        StringAssert.Contains(narrative, string.Format(
            messages.GetMessage("potion_restored"), 8, 18, 30));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void DrinkPotion_TellsTheThreeCasesApart(string language)
    {
        Messages messages = GetMessages(language);

        Player drinker = CreateWoundedPlayer(10);
        Player full = CreateWoundedPlayer(30);
        Player empty = CreateWoundedPlayer(10);
        empty.Potion = null;

        string drunk = drinker.DrinkPotion(messages);
        string refused = full.DrinkPotion(messages);
        string none = empty.DrinkPotion(messages);

        Assert.AreNotEqual(drunk, refused);
        Assert.AreNotEqual(drunk, none);
        Assert.AreNotEqual(refused, none);

        foreach (string narrative in new[] { drunk, refused, none })
            Assert.IsFalse(string.IsNullOrWhiteSpace(narrative) || narrative.Contains("["),
                $"A {language} narrative was missing or fell back to a message key: {narrative}");
    }

    // --- Whether the round may be spent on a drink at all ---

    [TestMethod]
    public void CanDrinkPotion_IsTrue_ForAWoundedCharacterCarryingOne()
    {
        Assert.IsTrue(CreateWoundedPlayer(10).CanDrinkPotion());
    }

    [TestMethod]
    public void CanDrinkPotion_IsFalse_AtMaximumHealthPoints()
    {
        Assert.IsFalse(CreateWoundedPlayer(30).CanDrinkPotion());
    }

    [TestMethod]
    public void CanDrinkPotion_IsFalse_WithNoPotionCarried()
    {
        Player player = CreateWoundedPlayer(10);
        player.Potion = null;

        Assert.IsFalse(player.CanDrinkPotion());
    }

    // --- The spelling, through the same parser the round reads ---

    [TestMethod]
    [DataRow("d")]
    [DataRow("drink")]
    [DataRow("D")]
    [DataRow("Drink")]
    [DataRow("  drink  ")]
    public void ParseCombatChoice_ReadsEverySpellingOfDrink(string input)
    {
        Assert.AreEqual(CombatAction.Drink, Combat.ParseCombatChoice(input));
    }

    [TestMethod]
    public void IsValidCombatChoice_AcceptsDrink()
    {
        Assert.IsTrue(new Game().IsValidCombatChoice("drink"));
    }

    // --- The prompt text, which omits what the Character cannot do ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void CombatPrompt_OffersTheDrink_WhenAPotionIsCarried(string language)
    {
        Player player = CreateWoundedPlayer(10);
        Messages messages = GetMessages(language);

        Combat combat = new Combat(player, new Dragon(new Die()), messages);

        Assert.AreEqual(messages.GetMessage("attack_prompt_with_potion"), combat.GetPromptText());
        Assert.AreNotEqual("[attack_prompt_with_potion]", combat.GetPromptText(),
            $"'attack_prompt_with_potion' is missing from the {language} dictionary.");
    }

    [TestMethod]
    public void CombatPrompt_IsUnchanged_WhenNoPotionIsCarried()
    {
        Player player = CreateWoundedPlayer(10);
        player.Potion = null;
        Messages messages = GetMessages("English");

        Combat combat = new Combat(player, new Dragon(new Die()), messages);

        Assert.AreEqual(messages.GetMessage("attack_prompt"), combat.GetPromptText());
    }
}
