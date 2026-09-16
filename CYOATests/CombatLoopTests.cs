// CYOATests/CombatLoopTests.cs
using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// The round loop is Console-driven, so it is covered the way the Encounters are: tests
// assert it runs to completion rather than probing its logic. The logic that can be
// asserted — which spelling means which action — lives in Combat.ParseCombatChoice and is
// tested directly, the way Game's pure validators are.
//
// Console redirection is process-global and this assembly parallelizes by method, so the
// whole class sits in its own serial phase, as MerchantEncounterTests does.
[TestClass]
[DoNotParallelize]
public class CombatLoopTests
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
            MaximumHealthPoints = 30,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private Dragon CreateTestDragon()
    {
        return new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };
    }

    // The fight's length depends on rolls that cannot be seeded, so the script is longer
    // than any run needs. Retreat ends it at the first prompt; a run that ends sooner
    // simply stops reading.
    private static void ScriptAnswersSilencingOutput(params string[] answers)
    {
        Console.SetIn(new StringReader(string.Join(Environment.NewLine, answers) + Environment.NewLine));
        Console.SetOut(TextWriter.Null);
    }

    private static StringWriter ScriptAnswersCapturingOutput(params string[] answers)
    {
        Console.SetIn(new StringReader(string.Join(Environment.NewLine, answers) + Environment.NewLine));
        StringWriter captured = new StringWriter();
        Console.SetOut(captured);
        return captured;
    }

    // --- ParseCombatChoice: the one place the accepted spellings live ---

    [TestMethod]
    [DataRow("a")]
    [DataRow("attack")]
    [DataRow("A")]
    [DataRow("Attack")]
    [DataRow("  attack  ")]
    public void ParseCombatChoice_ReadsEverySpellingOfAttack(string input)
    {
        Assert.AreEqual(CombatAction.Attack, Combat.ParseCombatChoice(input));
    }

    [TestMethod]
    [DataRow("r")]
    [DataRow("retreat")]
    [DataRow("R")]
    [DataRow("Retreat")]
    [DataRow("  retreat  ")]
    public void ParseCombatChoice_ReadsEverySpellingOfRetreat(string input)
    {
        Assert.AreEqual(CombatAction.Retreat, Combat.ParseCombatChoice(input));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("fight")]
    public void ParseCombatChoice_RejectsAnythingElse(string? input)
    {
        Assert.IsNull(Combat.ParseCombatChoice(input));
    }

    // Game's validator and the round loop must never disagree about a spelling, so they
    // read the same list. This is the test that fails if they are ever split apart again.
    [TestMethod]
    [DataRow("a", true)]
    [DataRow("attack", true)]
    [DataRow("r", true)]
    [DataRow("retreat", true)]
    [DataRow("fight", false)]
    public void IsValidCombatChoice_AgreesWithParseCombatChoice(string input, bool expected)
    {
        Game game = new Game();

        Assert.AreEqual(expected, game.IsValidCombatChoice(input));
        Assert.AreEqual(expected, Combat.ParseCombatChoice(input) != null);
    }

    // --- The round loop itself ---

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void StartCombat_RunsToCompletion_WhenThePlayerRetreats(string language)
    {
        ScriptAnswersSilencingOutput(Enumerable.Repeat("r", 50).ToArray());

        Combat combat = new Combat(CreateTestPlayer(), CreateTestDragon(), GetMessages(language));

        combat.StartCombat();
    }

    [TestMethod]
    public void StartCombat_RunsToCompletion_WhenThePlayerAttacks()
    {
        ScriptAnswersSilencingOutput(Enumerable.Repeat("a", 200).ToArray());

        Combat combat = new Combat(CreateTestPlayer(), CreateTestDragon(), GetMessages("English"));

        combat.StartCombat();
    }

    // A slip must not cost a round: the prompt asks again rather than letting the
    // unrecognised answer fall through into an action.
    [TestMethod]
    public void StartCombat_RunsToCompletion_WhenAnAnswerIsMistyped()
    {
        string[] answers = new[] { "fight", "", "xyzzy" }.Concat(Enumerable.Repeat("r", 50)).ToArray();
        ScriptAnswersSilencingOutput(answers);

        Combat combat = new Combat(CreateTestPlayer(), CreateTestDragon(), GetMessages("English"));

        combat.StartCombat();
    }

    [TestMethod]
    public void StartCombat_SetsPlayerRetreated_WhenTheAnswerIsRetreat()
    {
        ScriptAnswersSilencingOutput(Enumerable.Repeat("r", 50).ToArray());

        Player player = CreateTestPlayer();
        player.HealthPoints = 500;
        player.MaximumHealthPoints = 500;

        Dragon dragon = CreateTestDragon();
        dragon.HealthPoints = 500;

        Combat combat = new Combat(player, dragon, GetMessages("English"));

        bool playerWon = combat.StartCombat();

        Assert.IsFalse(playerWon);
        Assert.IsTrue(combat.PlayerRetreated);
    }

    // The guard on the restructure: the opening round must still attack before anything is
    // asked, so carrying the action did not introduce a prompt ahead of the first blow.
    [TestMethod]
    public void StartCombat_AttacksBeforeItPromptsForTheFirstTime()
    {
        StringWriter captured = ScriptAnswersCapturingOutput(Enumerable.Repeat("r", 50).ToArray());

        Player player = CreateTestPlayer();
        player.HealthPoints = 500;
        player.MaximumHealthPoints = 500;

        Dragon dragon = CreateTestDragon();
        dragon.HealthPoints = 500;

        Messages messages = GetMessages("English");

        new Combat(player, dragon, messages).StartCombat();

        string output = captured.ToString();

        string attackIntro = string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(player.Occupation),
            messages.TranslateWeaponForDisplay(player.Weapon!.Type));

        int firstAttack = output.IndexOf(attackIntro, StringComparison.Ordinal);
        int firstPrompt = output.IndexOf(messages.GetMessage("attack_prompt"), StringComparison.Ordinal);

        Assert.AreNotEqual(-1, firstAttack, "The Character never attacked.");
        Assert.AreNotEqual(-1, firstPrompt, "The prompt was never shown.");
        Assert.IsTrue(firstAttack < firstPrompt, "The prompt appeared before the opening attack.");
    }

    [TestMethod]
    public void StartCombat_PrintsTheRetreatNarrative_WhenThePlayerRetreats()
    {
        StringWriter captured = ScriptAnswersCapturingOutput(Enumerable.Repeat("r", 50).ToArray());

        Player player = CreateTestPlayer();
        player.HealthPoints = 500;
        player.MaximumHealthPoints = 500;

        Dragon dragon = CreateTestDragon();
        dragon.HealthPoints = 500;

        Messages messages = GetMessages("English");

        new Combat(player, dragon, messages).StartCombat();

        StringAssert.Contains(captured.ToString(), messages.GetMessage("retreat_combat"));
    }

    // Without an end-of-input guard the prompt would re-ask forever with nothing left to
    // read, which is a hang rather than a failure: this test would never return.
    [TestMethod]
    public void StartCombat_EndsTheFight_WhenTheInputRunsOut()
    {
        Console.SetIn(new StringReader(string.Empty));
        Console.SetOut(TextWriter.Null);

        Player player = CreateTestPlayer();
        player.HealthPoints = 500;
        player.MaximumHealthPoints = 500;

        Dragon dragon = CreateTestDragon();
        dragon.HealthPoints = 500;

        Combat combat = new Combat(player, dragon, GetMessages("English"));

        bool playerWon = combat.StartCombat();

        Assert.IsFalse(playerWon);
        Assert.IsTrue(combat.PlayerRetreated);
    }
}
