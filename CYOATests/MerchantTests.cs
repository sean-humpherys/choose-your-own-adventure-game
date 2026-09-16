// CYOATests/MerchantTests.cs
[TestClass]
public class MerchantTests
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

    // --- The offer ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetOffer_StatesBothPricesAndThePlayersGold(string language)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);

        string offer = merchant.GetOffer(player, messages);

        StringAssert.Contains(offer, Merchant.ItemPrice.ToString(), $"The {language} offer omits the price.");
        StringAssert.Contains(offer, player.Gold.ToString(), $"The {language} offer omits the player's Gold.");
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetOffer_NeverShowsARawMessageKey(string language)
    {
        Merchant merchant = new Merchant();

        string offer = merchant.GetOffer(CreateTestPlayer(), GetMessages(language));

        Assert.IsFalse(offer.Contains("[merchant"), $"The {language} offer fell back to a raw key: {offer}");
    }

    [TestMethod]
    public void GetOffer_LeavesTheCharacterUntouched()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();

        merchant.GetOffer(player, GetMessages("English"));

        Assert.AreEqual(Player.StartingGold, player.Gold);
        Assert.AreEqual("long sword", player.Weapon!.Type);
    }

    [TestMethod]
    public void ItemPrice_IsAffordableOnceButNotTwice()
    {
        Assert.IsTrue(Merchant.ItemPrice <= Player.StartingGold, "A new Character cannot afford a single item.");
        Assert.IsTrue(Merchant.ItemPrice * 2 > Player.StartingGold, "A new Character can afford both items.");
    }

    // --- Declining ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void ApplyChoice_Nothing_ReturnsTheDeclineNarrative(string language)
    {
        Merchant merchant = new Merchant();
        Messages messages = GetMessages(language);

        string narrative = merchant.ApplyChoice(CreateTestPlayer(), "nothing", messages);

        Assert.AreEqual(messages.GetMessage("merchant_declined"), narrative);
        Assert.IsFalse(narrative.Contains("[merchant"), $"The {language} decline narrative fell back to a raw key.");
    }

    [TestMethod]
    public void ApplyChoice_Nothing_ChangesNothingAboutTheCharacter()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Weapon carried = player.Weapon!;

        merchant.ApplyChoice(player, "nothing", GetMessages("English"));

        Assert.AreEqual(Player.StartingGold, player.Gold);
        Assert.AreSame(carried, player.Weapon);
    }

    // --- Buying the Armor ---

    [TestMethod]
    public void ApplyChoice_Armor_EquipsTheArmorAndDeductsThePrice()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();

        merchant.ApplyChoice(player, "armor", GetMessages("English"));

        Assert.IsNotNull(player.Armor);
        Assert.AreEqual(Merchant.ArmorType, player.Armor!.Type);
        Assert.AreEqual(Merchant.ArmorDamageReduction, player.Armor.DamageReduction);
        Assert.AreEqual(Player.StartingGold - Merchant.ItemPrice, player.Gold);
    }

    [TestMethod]
    public void ApplyChoice_Armor_LeavesTheCarriedWeaponUntouched()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Weapon carried = player.Weapon!;

        merchant.ApplyChoice(player, "armor", GetMessages("English"));

        Assert.AreSame(carried, player.Weapon);
        Assert.AreEqual(12, player.Weapon!.MaxDamage);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void ApplyChoice_Armor_ConfirmsThePurchaseInTheNarrative(string language)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);

        string narrative = merchant.ApplyChoice(player, "armor", messages);

        Assert.IsFalse(string.IsNullOrWhiteSpace(narrative), $"The {language} purchase said nothing.");
        Assert.IsFalse(narrative.Contains("[merchant"), $"The {language} confirmation fell back to a raw key: {narrative}");
        StringAssert.Contains(narrative, messages.TranslateArmorForDisplay(Merchant.ArmorType),
            $"The {language} confirmation does not name the Armor in that language.");
    }

    [TestMethod]
    public void ApplyChoice_Armor_SucceedsWhenTheGoldExactlyCoversThePrice()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Gold = Merchant.ItemPrice;

        merchant.ApplyChoice(player, "armor", GetMessages("English"));

        Assert.IsNotNull(player.Armor);
        Assert.AreEqual(0, player.Gold);
    }

    [TestMethod]
    public void ApplyChoice_Armor_IsRefusedWhenTheGoldFallsOneShort()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Gold = Merchant.ItemPrice - 1;

        merchant.ApplyChoice(player, "armor", GetMessages("English"));

        Assert.IsNull(player.Armor);
        Assert.AreEqual(Merchant.ItemPrice - 1, player.Gold);
    }

    // A weaponless Character is offered the Armor, so buying it must still work for them.
    [TestMethod]
    public void ApplyChoice_Armor_WorksForACharacterCarryingNoWeapon()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Weapon = null;

        merchant.ApplyChoice(player, "armor", GetMessages("English"));

        Assert.IsNotNull(player.Armor);
        Assert.IsNull(player.Weapon);
        Assert.AreEqual(Player.StartingGold - Merchant.ItemPrice, player.Gold);
    }

    // Deferred from ticket 05, which had no Armor to assert against.
    [TestMethod]
    public void ApplyChoice_Weapon_LeavesArmorUnset()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();

        merchant.ApplyChoice(player, "weapon", GetMessages("English"));

        Assert.IsNull(player.Armor);
    }

    [TestMethod]
    public void ApplyChoice_Nothing_LeavesArmorUnset()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();

        merchant.ApplyChoice(player, "nothing", GetMessages("English"));

        Assert.IsNull(player.Armor);
    }

    // --- Buying the Magical Weapon ---

    private Player CreatePlayerWith(string occupation, Weapon weapon)
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = occupation,
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = weapon
        };
    }

    [TestMethod]
    public void ApplyChoice_Weapon_ReplacesTheCarriedWeapon()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Weapon carried = player.Weapon!;

        merchant.ApplyChoice(player, "weapon", GetMessages("English"));

        Assert.AreNotSame(carried, player.Weapon);
        Assert.AreNotEqual(carried.Type, player.Weapon!.Type);
    }

    [TestMethod]
    public void ApplyChoice_Weapon_DeductsThePriceFromTheGold()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();

        merchant.ApplyChoice(player, "weapon", GetMessages("English"));

        Assert.AreEqual(Player.StartingGold - Merchant.ItemPrice, player.Gold);
    }

    // The increment is uniform, so the Damage ordering set at character creation survives.
    [TestMethod]
    [DataRow("Fighter", "long sword", 12, "runed long sword")]
    [DataRow("Magician", "lightning bolt spell", 12, "chain lightning spell")]
    [DataRow("Thief", "dagger", 6, "shadowfang dagger")]
    [DataRow("Archer", "long bow", 8, "windsong long bow")]
    public void ApplyChoice_Weapon_FitsTheOccupationAndAddsTheFixedIncrement(
        string occupation, string startingType, int startingDamage, string expectedType)
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayerWith(occupation, new Weapon(startingType, startingDamage, "art"));

        merchant.ApplyChoice(player, "weapon", GetMessages("English"));

        Assert.AreEqual(expectedType, player.Weapon!.Type);
        Assert.AreEqual(startingDamage + Merchant.MagicalDamageIncrement, player.Weapon.MaxDamage);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void ApplyChoice_Weapon_ConfirmsThePurchaseInTheNarrative(string language)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);

        string narrative = merchant.ApplyChoice(player, "weapon", messages);

        Assert.IsFalse(string.IsNullOrWhiteSpace(narrative), $"The {language} purchase said nothing.");
        Assert.IsFalse(narrative.Contains("[merchant"), $"The {language} confirmation fell back to a raw key: {narrative}");
        StringAssert.Contains(narrative, messages.TranslateWeaponForDisplay(player.Weapon!.Type),
            $"The {language} confirmation does not name the new Weapon in that language.");
    }

    // English maps each Weapon name to itself, so only the other two can show a mapping
    // by differing from the canonical name. Parity (ticket 02) covers the English keys.
    [TestMethod]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void ApplyChoice_Weapon_NamesTheNewWeaponInEveryLanguage(string language)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);

        merchant.ApplyChoice(player, "weapon", messages);

        string translated = messages.TranslateWeaponForDisplay(player.Weapon!.Type);
        Assert.AreNotEqual(player.Weapon.Type, translated,
            $"'{player.Weapon.Type}' is missing from the {language} weapon display map.");
    }

    [TestMethod]
    public void ApplyChoice_Weapon_ShowsUpOnTheCharacterSheet()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages("English");

        merchant.ApplyChoice(player, "weapon", messages);
        string sheet = player.GetStatsDisplay(messages);

        StringAssert.Contains(sheet, messages.TranslateWeaponForDisplay(player.Weapon!.Type));
        StringAssert.Contains(sheet, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void ApplyChoice_Weapon_ShowsUpWhenTheDragonFightOpens()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages("English");

        merchant.ApplyChoice(player, "weapon", messages);
        Combat combat = new Combat(player, new Dragon(new Die()), messages);

        string display = combat.GetCombatStatsDisplay();

        StringAssert.Contains(display, messages.TranslateWeaponForDisplay(player.Weapon!.Type));
        StringAssert.Contains(display, player.Weapon.MaxDamage.ToString());
    }

    // --- Affordability ---
    // Asserted at the seam: the once-per-Adventure rule means no player reaches a second purchase.

    [TestMethod]
    public void ApplyChoice_Weapon_SucceedsWhenTheGoldExactlyCoversThePrice()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Gold = Merchant.ItemPrice;

        merchant.ApplyChoice(player, "weapon", GetMessages("English"));

        Assert.AreEqual("runed long sword", player.Weapon!.Type);
        Assert.AreEqual(0, player.Gold);
    }

    [TestMethod]
    public void ApplyChoice_Weapon_IsRefusedWhenTheGoldFallsOneShort()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Weapon carried = player.Weapon!;
        player.Gold = Merchant.ItemPrice - 1;

        merchant.ApplyChoice(player, "weapon", GetMessages("English"));

        Assert.AreSame(carried, player.Weapon);
        Assert.AreEqual(Merchant.ItemPrice - 1, player.Gold);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void ApplyChoice_Weapon_SaysSoWhenThePurseIsTooLight(string language)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Gold = 0;

        string narrative = merchant.ApplyChoice(player, "weapon", GetMessages(language));

        Assert.IsFalse(string.IsNullOrWhiteSpace(narrative), $"The {language} refusal said nothing.");
        Assert.IsFalse(narrative.Contains("[merchant"), $"The {language} refusal fell back to a raw key: {narrative}");
    }

    // --- A Character carrying no Weapon ---
    // A relative increment has no base to build on, so only the Armor is on offer.

    [TestMethod]
    public void ApplyChoice_Weapon_IsRefusedWhenNoWeaponIsCarried()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Weapon = null;

        string narrative = merchant.ApplyChoice(player, "weapon", GetMessages("English"));

        Assert.IsNull(player.Weapon);
        Assert.AreEqual(Player.StartingGold, player.Gold);
        Assert.IsFalse(string.IsNullOrWhiteSpace(narrative));
        Assert.IsFalse(narrative.Contains("[merchant"));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void GetOffer_OmitsTheMagicalWeapon_WhenNoWeaponIsCarried(string language)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        player.Weapon = null;
        Messages messages = GetMessages(language);

        string offer = merchant.GetOffer(player, messages);

        string weaponLine = string.Format(messages.GetMessage("merchant_offer_weapon"), Merchant.ItemPrice);
        Assert.IsFalse(offer.Contains(weaponLine), $"The {language} offer still sells a Magical Weapon to a Character carrying none.");
        StringAssert.Contains(offer, string.Format(messages.GetMessage("merchant_offer_armor"), Merchant.ItemPrice));
    }
}
