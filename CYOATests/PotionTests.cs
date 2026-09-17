// CYOATests/PotionTests.cs
// The Potion's purchase is asserted through Merchant.ApplyChoice, the Encounter's single
// seam, the way the Armor's and the Magical Weapon's already are. Nothing here drives the
// Console, and nothing depends on a roll.
[TestClass]
public class PotionTests
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
            MaximumHealthPoints = 30,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // --- The Potion itself ---

    [TestMethod]
    public void Potion_CarriesItsTypeRestoreAndArt()
    {
        Potion potion = new Potion("healing potion", 8, "(~)");

        Assert.AreEqual("healing potion", potion.Type);
        Assert.AreEqual(8, potion.HealthPointsRestored);
        Assert.AreEqual("(~)", potion.AsciiArt);
    }

    [TestMethod]
    public void NewPlayer_CarriesNoPotion()
    {
        Assert.IsNull(new Player().Potion);
    }

    // --- Buying it, through the Merchant's seam ---

    [TestMethod]
    public void ApplyChoice_Potion_EquipsThePotionAndDeductsThePrice()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();

        merchant.ApplyChoice(player, "potion", GetMessages("English"));

        Assert.IsNotNull(player.Potion);
        Assert.AreEqual(Merchant.PotionType, player.Potion!.Type);
        Assert.AreEqual(Merchant.PotionHealthPointsRestored, player.Potion.HealthPointsRestored);
        Assert.AreEqual(Player.StartingGold - Merchant.ItemPrice, player.Gold);
    }

    [TestMethod]
    public void ApplyChoice_Potion_LeavesTheWeaponAndArmorUntouched()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Weapon carried = player.Weapon!;

        merchant.ApplyChoice(player, "potion", GetMessages("English"));

        Assert.AreSame(carried, player.Weapon);
        Assert.IsNull(player.Armor);
    }

    [TestMethod]
    [DataRow("armor")]
    [DataRow("weapon")]
    [DataRow("nothing")]
    public void ApplyChoice_OtherChoices_LeaveThePotionUnset(string choice)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();

        merchant.ApplyChoice(player, choice, GetMessages("English"));

        Assert.IsNull(player.Potion);
    }

    [TestMethod]
    [DataRow("English", "healing potion")]
    [DataRow("Spanish", "pocion curativa")]
    [DataRow("French", "potion de soin")]
    public void ApplyChoice_Potion_ConfirmsThePurchaseInTheNarrative(string language, string expectedName)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);

        string narrative = merchant.ApplyChoice(player, "potion", messages);

        Assert.IsFalse(string.IsNullOrWhiteSpace(narrative));
        Assert.IsFalse(narrative.StartsWith("["), $"The {language} narrative fell back to a message key: {narrative}");
        StringAssert.Contains(narrative, expectedName);
        StringAssert.Contains(narrative, Merchant.PotionHealthPointsRestored.ToString());
    }

    // The purse affords exactly one ware. Both boundaries are asserted because the refusal
    // is the thing that keeps the three-way choice real.
    [TestMethod]
    public void ApplyChoice_Potion_SucceedsWhenTheGoldExactlyCoversThePrice()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Gold = Merchant.ItemPrice;

        merchant.ApplyChoice(player, "potion", GetMessages("English"));

        Assert.IsNotNull(player.Potion);
        Assert.AreEqual(0, player.Gold);
    }

    [TestMethod]
    public void ApplyChoice_Potion_IsRefusedWhenTheGoldFallsOneShort()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Gold = Merchant.ItemPrice - 1;

        string narrative = merchant.ApplyChoice(player, "potion", GetMessages("English"));

        Assert.IsNull(player.Potion);
        Assert.AreEqual(Merchant.ItemPrice - 1, player.Gold);
        Assert.AreEqual(GetMessages("English").GetMessage("merchant_cannot_afford"), narrative);
    }

    // The purse cannot stretch to a second ware, whatever was bought first.
    [TestMethod]
    public void ApplyChoice_Potion_IsRefusedAfterTheArmorHasBeenBought()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages("English");

        merchant.ApplyChoice(player, "armor", messages);
        merchant.ApplyChoice(player, "potion", messages);

        Assert.IsNull(player.Potion);
        Assert.AreEqual(Player.StartingGold - Merchant.ItemPrice, player.Gold);
    }

    [TestMethod]
    public void ApplyChoice_Armor_IsRefusedAfterThePotionHasBeenBought()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages("English");

        merchant.ApplyChoice(player, "potion", messages);
        merchant.ApplyChoice(player, "armor", messages);

        Assert.IsNull(player.Armor);
        Assert.IsNotNull(player.Potion);
        Assert.AreEqual(Player.StartingGold - Merchant.ItemPrice, player.Gold);
    }

    // --- The offer ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetOffer_ListsThePotionItsPriceAndWhatItRestores(string language)
    {
        Merchant merchant = new Merchant();
        Messages messages = GetMessages(language);

        string offer = merchant.GetOffer(CreateTestPlayer(), messages);

        string expected = string.Format(
            messages.GetMessage("merchant_offer_potion"),
            Merchant.ItemPrice,
            Merchant.PotionHealthPointsRestored);

        Assert.AreNotEqual("[merchant_offer_potion]", messages.GetMessage("merchant_offer_potion"),
            $"'merchant_offer_potion' is missing from the {language} dictionary.");
        StringAssert.Contains(offer, expected);
    }

    // --- The spellings, through the existing parser ---

    [TestMethod]
    [DataRow("p")]
    [DataRow("potion")]
    [DataRow("P")]
    [DataRow("Potion")]
    [DataRow("  potion  ")]
    public void IsValidMerchantChoice_AcceptsEverySpellingOfThePotion(string input)
    {
        Assert.IsTrue(new Game().IsValidMerchantChoice(input));
    }

    [TestMethod]
    [DataRow("a")]
    [DataRow("armor")]
    [DataRow("w")]
    [DataRow("weapon")]
    [DataRow("n")]
    [DataRow("nothing")]
    public void IsValidMerchantChoice_StillAcceptsTheExistingSpellings(string input)
    {
        Assert.IsTrue(new Game().IsValidMerchantChoice(input));
    }

    // --- Both stats displays ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetStatsDisplay_ShowsThePotion_WhenOneIsCarried(string language)
    {
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);
        player.Potion = new Potion(Merchant.PotionType, Merchant.PotionHealthPointsRestored, "(~)");

        string result = player.GetStatsDisplay(messages);

        StringAssert.Contains(result, messages.GetMessage("stats_potion") + messages.TranslatePotionForDisplay(player.Potion.Type));
        StringAssert.Contains(result, messages.GetMessage("stats_potion_restores") + player.Potion.HealthPointsRestored);
    }

    // The character sheet names the empty slot, reversing story 11 of the Healing Potion
    // spec for this display only; the combat display below still leaves it out.
    [TestMethod]
    [DataRow("English", "Healing Potion: none")]
    [DataRow("Spanish", "Pocion Curativa: ninguna")]
    [DataRow("French", "Potion de Soin: aucune")]
    public void GetStatsDisplay_ShowsPotionNone_WhenNoneIsCarried(string language, string expectedLine)
    {
        Messages messages = GetMessages(language);

        string result = CreateTestPlayer().GetStatsDisplay(messages);

        CollectionAssert.Contains(result.Split(Environment.NewLine), expectedLine);
        Assert.IsFalse(result.Contains(messages.GetMessage("stats_potion_restores")),
            "The character sheet showed a restore line for an empty Potion slot.");
        Assert.IsFalse(result.Contains("[stats_"), $"The {language} sheet fell back to a raw key: {result}");
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetCombatStatsDisplay_ShowsThePotion_WhenOneIsCarried(string language)
    {
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);
        player.Potion = new Potion(Merchant.PotionType, Merchant.PotionHealthPointsRestored, "(~)");

        Combat combat = new Combat(player, new Dragon(new Die()), messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_potion") + messages.TranslatePotionForDisplay(player.Potion.Type));
        StringAssert.Contains(result, messages.GetMessage("stats_potion_restores") + player.Potion.HealthPointsRestored);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ShowsNoPotionLine_WhenNoneIsCarried()
    {
        Messages messages = GetMessages("English");

        Combat combat = new Combat(CreateTestPlayer(), new Dragon(new Die()), messages);

        Assert.IsFalse(combat.GetCombatStatsDisplay().Contains(messages.GetMessage("stats_potion")),
            "The combat display showed an empty Potion line.");
    }
}
