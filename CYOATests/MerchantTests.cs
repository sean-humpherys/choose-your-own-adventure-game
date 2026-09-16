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

    // Buying is wired up in tickets 05 and 06; until then either item is accepted and does nothing.
    [TestMethod]
    [DataRow("armor")]
    [DataRow("weapon")]
    public void ApplyChoice_BuyingIsAcceptedButChangesNothingYet(string choice)
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer();
        Weapon carried = player.Weapon!;

        merchant.ApplyChoice(player, choice, GetMessages("English"));

        Assert.AreEqual(Player.StartingGold, player.Gold);
        Assert.AreSame(carried, player.Weapon);
    }
}
