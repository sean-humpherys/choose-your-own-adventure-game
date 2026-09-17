// CYOATests/ForkTests.cs
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// The fork loop is Console-driven, so it is covered the way the Merchant Encounter is: the
// private method is reached by reflection with a hand-built Character and scripted input,
// and the tests assert on captured output. No test goes north, because the Dragon fight
// depends on rolls the Die cannot be seeded for.
//
// Console redirection is process-global, so the whole class runs serially.
[TestClass]
[DoNotParallelize]
public class ForkTests
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

    private static Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    private static Player CreateTestPlayer()
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

    // Runs the fork loop to its end with the given input and returns everything it printed.
    private static string RunFork(Player player, Messages messages, string input)
    {
        MethodInfo? method = typeof(Game).GetMethod(
            "PlayFromFork",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        Assert.IsNotNull(method, "PlayFromFork method was not found.");

        Console.SetIn(new StringReader(input));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        method!.Invoke(new Game(), new object[] { player, new Dragon(new Die()), messages });

        return writer.ToString();
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void South_ThenDeclining_MeetsTheMerchant(string language)
    {
        Messages messages = GetMessages(language);

        string output = RunFork(CreateTestPlayer(), messages, "s\nn\ne\n");

        StringAssert.Contains(output, messages.GetMessage("merchant_greeting"));
        StringAssert.Contains(output, messages.GetMessage("merchant_declined"));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void South_Twice_FindsTheClearingEmpty(string language)
    {
        Messages messages = GetMessages(language);

        string output = RunFork(CreateTestPlayer(), messages, "s\nn\ns\ne\n");

        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("merchant_greeting")));
        StringAssert.Contains(output, messages.GetMessage("south_path_empty"));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Junk_PrintsTheInvalidChoiceMessage_AndAsksAgain(string language)
    {
        Messages messages = GetMessages(language);

        string output = RunFork(CreateTestPlayer(), messages, "west\ne\n");

        StringAssert.Contains(output, messages.GetMessage("path_invalid"));
        Assert.AreEqual(2, CountOccurrences(output, messages.GetMessage("path_prompt_full")));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Exit_EndsTheAdventure(string language)
    {
        Messages messages = GetMessages(language);

        string output = RunFork(CreateTestPlayer(), messages, "e\n");

        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
        Assert.IsFalse(output.Contains(messages.GetMessage("merchant_greeting")));
    }

    // --- Viewing the Character at the fork ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Character_PrintsTheSummary_ThenAsksAgain(string language)
    {
        Messages messages = GetMessages(language);
        Player player = CreateTestPlayer();

        string output = RunFork(player, messages, "c\ne\n");

        StringAssert.Contains(output, player.GetStatsDisplay(messages));
        Assert.AreEqual(2, CountOccurrences(output, messages.GetMessage("path_prompt_full")));
        Assert.IsFalse(output.Contains(messages.GetMessage("path_invalid")));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Character_BeforeGoingSouth_LeavesTheMerchantWaiting(string language)
    {
        Messages messages = GetMessages(language);

        string output = RunFork(CreateTestPlayer(), messages, "c\ns\nn\ne\n");

        StringAssert.Contains(output, messages.GetMessage("merchant_greeting"));
        Assert.IsFalse(output.Contains(messages.GetMessage("south_path_empty")));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Character_AfterMeetingTheMerchant_DoesNotBringHimBack(string language)
    {
        Messages messages = GetMessages(language);

        string output = RunFork(CreateTestPlayer(), messages, "s\nn\nc\ns\ne\n");

        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("merchant_greeting")));
        StringAssert.Contains(output, messages.GetMessage("south_path_empty"));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Character_Twice_ThenExit_PrintsTheSummaryTwice_AndEndsTheAdventure(string language)
    {
        Messages messages = GetMessages(language);
        Player player = CreateTestPlayer();

        string output = RunFork(player, messages, "c\n  CHARACTER  \ne\n");

        Assert.AreEqual(2, CountOccurrences(output, player.GetStatsDisplay(messages)));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    // A purchase shows up the next time the Character is viewed, Gold included.
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void Character_AfterBuyingArmor_ShowsTheArmorAndTheLowerGold(string language)
    {
        Messages messages = GetMessages(language);
        Player player = CreateTestPlayer();

        string output = RunFork(player, messages, "s\na\nc\ne\n");

        StringAssert.Contains(output, messages.GetMessage("stats_armor") + messages.TranslateArmorForDisplay(Merchant.ArmorType));
        StringAssert.Contains(output, messages.GetMessage("stats_gold") + 25);
    }

    [TestMethod]
    [DataRow("English", "Enter 'c' to view your character.", "Invalid choice. Please enter 'n', 's', 'e', or 'c'.")]
    [DataRow("Spanish", "Escribe 'c' para ver tu personaje.", "Opcion invalida. Por favor escribe 'n', 's', 'e' o 'c'.")]
    [DataRow("French", "Tapez 'c' pour voir votre personnage.", "Choix invalide. Veuillez taper 'n', 's', 'e' ou 'c'.")]
    public void ForkWording_OffersTheCharacterLast_AndListsItWhenTheChoiceIsInvalid(
        string language, string closingSentence, string invalidChoice)
    {
        Messages messages = GetMessages(language);

        Assert.IsTrue(messages.GetMessage("path_prompt_full").EndsWith(" " + closingSentence),
            $"The {language} fork prompt does not end with the Character sentence.");
        Assert.AreEqual(invalidChoice, messages.GetMessage("path_invalid"));
    }

    // Run on a task with a deadline, so a regression back to re-prompting forever fails
    // the test instead of hanging the run.
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void EndOfInput_EndsTheAdventure_InsteadOfLooping(string language)
    {
        Messages messages = GetMessages(language);
        string output = "";

        Task run = Task.Run(() => output = RunFork(CreateTestPlayer(), messages, ""));

        Assert.IsTrue(run.Wait(TimeSpan.FromSeconds(5)), "The fork kept asking after its input ended.");
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }
}
