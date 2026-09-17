// CYOATests/MerchantEncounterTests.cs
using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// The Encounter's prompt loop is Console-driven, so it is covered the way the Dragon
// Encounter already is: the private method is reached by reflection rather than widened
// to public, and the tests assert it runs and prints its flow rather than probing logic.
// The Encounter's real logic lives behind Merchant.ApplyChoice and IsValidMerchantChoice,
// which are public and asserted directly.
//
// Console redirection is process-global and this assembly parallelizes by method, so the
// whole class sits in its own serial phase, as GamePbi18Tests does.
[TestClass]
[DoNotParallelize]
public class MerchantEncounterTests
{
    private TextReader? _originalIn;
    private TextWriter? _originalOut;

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalIn != null)
            Console.SetIn(_originalIn);

        if (_originalOut != null)
            Console.SetOut(_originalOut);
    }

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

    private static void HandleMerchantEncounter(Game game, Player player, Messages messages)
    {
        MethodInfo? method = typeof(Game).GetMethod(
            "HandleMerchantEncounter",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        Assert.IsNotNull(method, "HandleMerchantEncounter method was not found.");

        method!.Invoke(game, new object[] { player, messages });
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void HandleMerchantEncounter_DeclinePath_DisplaysEncounterFlowText(string language)
    {
        Game game = new Game();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);

        // "n" declines, which ends the Encounter.
        Console.SetIn(new StringReader("n\n"));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        HandleMerchantEncounter(game, player, messages);

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("south_path_narrative"));
        StringAssert.Contains(output, messages.GetMessage("merchant_greeting"));
        StringAssert.Contains(output, messages.GetMessage("merchant_prompt"));
        StringAssert.Contains(output, messages.GetMessage("merchant_declined"));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void HandleMerchantEncounter_PurchasePath_DoesNotThrow(string language)
    {
        Game game = new Game();
        Player player = CreateTestPlayer();
        Messages messages = GetMessages(language);

        Console.SetIn(new StringReader("a\n"));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        HandleMerchantEncounter(game, player, messages);

        Assert.IsNotNull(player.Armor);
    }

    [TestMethod]
    public void HandleMerchantEncounter_RePromptsOnJunkAndEmptyAnswers()
    {
        Game game = new Game();
        Messages messages = GetMessages("English");

        Console.SetIn(new StringReader("banana\n\n   \nn\n"));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        HandleMerchantEncounter(game, CreateTestPlayer(), messages);

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("merchant_invalid"));
        StringAssert.Contains(output, messages.GetMessage("merchant_declined"));
    }
}
