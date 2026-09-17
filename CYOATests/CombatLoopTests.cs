// CYOATests/CombatLoopTests.cs
using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

    private static Player CreateUnkillablePlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 100,
            MaximumHealthPoints = 500,
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Potion = new Potion("healing potion", 8, "(~~~)")
        };
    }

    private static Dragon CreateUnkillableDragon()
    {
        return new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 5000,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };
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

    // --- Drinking, in the round ---

    // The drink is taken in the round the prompt chose it, which is what "drinking replaces
    // the attack" means: the Potion is gone by the time the fight ends.
    [TestMethod]
    public void StartCombat_DrinksThePotion_WhenTheAnswerIsDrink()
    {
        ScriptAnswersSilencingOutput("d", "r", "r", "r");

        Player player = CreateUnkillablePlayer();

        Combat combat = new Combat(player, CreateUnkillableDragon(), GetMessages("English"));

        combat.StartCombat();

        Assert.IsNull(player.Potion, "The Potion was not drunk.");
        Assert.IsTrue(combat.PlayerRetreated);
    }

    // Typing the drink option while carrying none must cost nothing at all: the Character
    // is untouched and the prompt asks again rather than the round being spent. Counting the
    // attacks is what proves the round was not spent — asserting the retreat alone would
    // pass just as well if the mistyped answer had cost a round first.
    [TestMethod]
    public void StartCombat_RePromptsWithoutSpendingTheRound_WhenNoPotionIsCarried()
    {
        StringWriter captured = ScriptAnswersCapturingOutput("d", "r", "r", "r");

        Player player = CreateUnkillablePlayer();
        player.Potion = null;
        Messages messages = GetMessages("English");

        Combat combat = new Combat(player, CreateUnkillableDragon(), messages);

        combat.StartCombat();

        string output = captured.ToString();

        string attack = string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(player.Occupation),
            messages.TranslateWeaponForDisplay(player.Weapon!.Type));

        StringAssert.Contains(output, string.Format(messages.GetMessage("potion_none_carried"), player.Name));
        Assert.AreEqual(1, CountOccurrences(output, attack),
            "The refused drink cost a round: the Character attacked more than the opening round.");
        Assert.IsTrue(combat.PlayerRetreated);
    }

    private static int CountOccurrences(string haystack, string needle)
    {
        int count = 0;
        int at = haystack.IndexOf(needle, StringComparison.Ordinal);

        while (at != -1)
        {
            count++;
            at = haystack.IndexOf(needle, at + needle.Length, StringComparison.Ordinal);
        }

        return count;
    }

    // At Maximum Health Points the drink is refused and the Potion kept, so a ware worth
    // the whole purse is never silently spent on nothing.
    //
    // Agility 20 is what makes this deterministic rather than a coin toss: the Dragon's
    // opening blow would otherwise wound the Character before the prompt is reached, and
    // the drink would be allowed. Every Defense Roll is a d20, so 20 always defends.
    [TestMethod]
    public void StartCombat_KeepsThePotion_WhenTheDrinkIsRefusedAtMaximumHealthPoints()
    {
        ScriptAnswersSilencingOutput("d", "r", "r", "r");

        Player player = CreateUnkillablePlayer();
        player.Agility = 20;
        player.HealthPoints = player.MaximumHealthPoints;
        Potion carried = player.Potion!;

        Combat combat = new Combat(player, CreateUnkillableDragon(), GetMessages("English"));

        combat.StartCombat();

        Assert.AreEqual(player.MaximumHealthPoints, player.HealthPoints);
        Assert.AreSame(carried, player.Potion, "The refused Potion was consumed.");
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void StartCombat_RunsToCompletion_WhenThePlayerDrinks(string language)
    {
        ScriptAnswersSilencingOutput(new[] { "d" }.Concat(Enumerable.Repeat("a", 200)).ToArray());

        Combat combat = new Combat(CreateUnkillablePlayer(), CreateTestDragon(), GetMessages(language));

        combat.StartCombat();
    }

    // The fight is entered by choosing to attack, and that prompt is not the combat one:
    // a Character at Maximum Health Points has nothing to gain from a drink there.
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void PreCombatPrompt_DoesNotOfferTheDrinkInItsText(string language)
    {
        Messages messages = GetMessages(language);

        string prompt = messages.GetMessage("dragon_encounter_prompt");

        Assert.IsFalse(prompt.Contains("'d'"), $"The {language} pre-combat prompt offered the drink.");
    }

    // The text not mentioning the drink is not enough: the prompt must not accept it either.
    // Adding the spelling to the shared parser must not reach this loop, which reads its own.
    // 'd' is answered with the invalid message and the prompt asks again, so the fight is
    // never entered and the retreat that follows is what ends the Encounter.
    [TestMethod]
    public void PreCombatPrompt_RejectsTheDrink_AndNeverEntersTheFight()
    {
        StringWriter captured = ScriptAnswersCapturingOutput("d", "r");

        Player player = CreateUnkillablePlayer();
        Dragon dragon = CreateUnkillableDragon();
        Messages messages = GetMessages("English");

        MethodInfo? method = typeof(Game).GetMethod(
            "HandleDragonEncounter",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, "HandleDragonEncounter method was not found.");

        method!.Invoke(new Game(), new object[] { player, dragon, messages });

        string output = captured.ToString();

        StringAssert.Contains(output, messages.GetMessage("invalid"));
        StringAssert.Contains(output, messages.GetMessage("retreat"));

        // Entering the fight would have printed the combat stats display and an attack.
        Assert.IsFalse(output.Contains(messages.GetMessage("combat_stats_header")),
            "The drink was accepted at the pre-combat prompt and the fight began.");
        Assert.IsNotNull(player.Potion, "The Potion was drunk before the fight began.");
    }

    // The round cost is the whole reason drinking is a decision, so it is asserted rather
    // than assumed: after the drink the Character does not swing, and the Dragon does.
    [TestMethod]
    public void StartCombat_SpendsTheRoundOnTheDrink_AndLetsTheDragonAnswer()
    {
        StringWriter captured = ScriptAnswersCapturingOutput("d", "r", "r", "r");

        Player player = CreateUnkillablePlayer();
        Dragon dragon = CreateUnkillableDragon();
        Messages messages = GetMessages("English");

        new Combat(player, dragon, messages).StartCombat();

        string output = captured.ToString();

        string playerAttack = string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(player.Occupation),
            messages.TranslateWeaponForDisplay(player.Weapon!.Type));

        string dragonAttack = string.Format(
            messages.GetMessage("dragon_attack_intro"),
            dragon.Name,
            player.Name,
            messages.TranslateWeaponForDisplay(dragon.Weapon.Type));

        string drink = string.Format(
            messages.GetMessage("potion_drunk"),
            player.Name,
            messages.TranslatePotionForDisplay("healing potion"));

        int drinkAt = output.IndexOf(drink, StringComparison.Ordinal);
        Assert.AreNotEqual(-1, drinkAt, "The Potion was never drunk.");

        string afterTheDrink = output.Substring(drinkAt + drink.Length);

        StringAssert.Contains(afterTheDrink, dragonAttack, "The Dragon did not answer the drink.");
        Assert.IsFalse(afterTheDrink.Contains(playerAttack),
            "The Character attacked in the round it spent drinking.");
    }
}
