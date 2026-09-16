// CYOATests/GameTests.cs
[TestClass]
public class GameTests
{
    private Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerWinsInEnglish()
    {
        Game game = new Game();
        game.EndGame(true, false, GetMessages("English"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerLosesInEnglish()
    {
        Game game = new Game();
        game.EndGame(false, false, GetMessages("English"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerWinsInSpanish()
    {
        Game game = new Game();
        game.EndGame(true, false, GetMessages("Spanish"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerWinsInFrench()
    {
        Game game = new Game();
        game.EndGame(true, false, GetMessages("French"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerLosesInSpanish()
    {
        Game game = new Game();
        game.EndGame(false, false, GetMessages("Spanish"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerLosesInFrench()
    {
        Game game = new Game();
        game.EndGame(false, false, GetMessages("French"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerRetreatsInEnglish()
    {
        Game game = new Game();
        game.EndGame(false, true, GetMessages("English"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerRetreatsInSpanish()
    {
        Game game = new Game();
        game.EndGame(false, true, GetMessages("Spanish"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerRetreatsInFrench()
    {
        Game game = new Game();
        game.EndGame(false, true, GetMessages("French"));
    }

    // --- IsValidMenuChoice ---

    [TestMethod]
    public void IsValidMenuChoice_ReturnsTrue_ForOne()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidMenuChoice("1"));
    }

    [TestMethod]
    public void IsValidMenuChoice_ReturnsTrue_ForTwo()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidMenuChoice("2"));
    }

    [TestMethod]
    public void IsValidMenuChoice_ReturnsFalse_ForThree()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMenuChoice("3"));
    }

    [TestMethod]
    public void IsValidMenuChoice_ReturnsFalse_ForNull()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMenuChoice(null));
    }

    [TestMethod]
    public void IsValidMenuChoice_ReturnsFalse_ForEmpty()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMenuChoice(""));
    }

    [TestMethod]
    public void IsValidMenuChoice_ReturnsFalse_ForLetters()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMenuChoice("abc"));
    }

    // --- IsValidPathChoice ---

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForN()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("n"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForNorth()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("north"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForS()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("s"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForSouth()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("south"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForE()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("e"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForExit()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("exit"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsFalse_ForInvalidInput()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidPathChoice("west"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsFalse_ForNull()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidPathChoice(null));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsFalse_ForEmpty()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidPathChoice(""));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsFalse_ForWhitespace()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidPathChoice("   "));
    }

    // --- IsValidMenuChoice: padded input ---

    [TestMethod]
    public void IsValidMenuChoice_ReturnsTrue_ForPaddedOne()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidMenuChoice("  1  "));
    }

    [TestMethod]
    public void IsValidMenuChoice_ReturnsTrue_ForPaddedTwo()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidMenuChoice("  2  "));
    }

    // --- IsValidPathChoice: case insensitivity ---

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForUppercaseN()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("N"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForUppercaseNorth()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("NORTH"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForUppercaseS()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("S"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForUppercaseE()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("E"));
    }

    // --- IsValidPathChoice: padded input ---

    [TestMethod]
    public void IsValidPathChoice_ReturnsTrue_ForPaddedNorth()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidPathChoice("  north  "));
    }

    // --- IsValidPathChoice: numbers should be invalid ---

    [TestMethod]
    public void IsValidPathChoice_ReturnsFalse_ForOne()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidPathChoice("1"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsFalse_ForTwo()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidPathChoice("2"));
    }

    [TestMethod]
    public void IsValidPathChoice_ReturnsFalse_ForThree()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidPathChoice("3"));
    }

    // --- IsValidCombatChoice ---

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForA()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("a"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForAttack()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("attack"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForR()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("r"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForRetreat()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("retreat"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsFalse_ForInvalidInput()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidCombatChoice("fight"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsFalse_ForNull()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidCombatChoice(null));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsFalse_ForEmpty()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidCombatChoice(""));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsFalse_ForWhitespace()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidCombatChoice("   "));
    }

    // --- IsValidCombatChoice: case insensitivity ---

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForUppercaseA()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("A"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForUppercaseAttack()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("ATTACK"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForUppercaseR()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("R"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForUppercaseRetreat()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("RETREAT"));
    }

    // --- IsValidCombatChoice: numbers should be invalid (old prompt said "1. Attack 2. Retreat") ---

    [TestMethod]
    public void IsValidCombatChoice_ReturnsFalse_ForOne()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidCombatChoice("1"));
    }

    [TestMethod]
    public void IsValidCombatChoice_ReturnsFalse_ForTwo()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidCombatChoice("2"));
    }

    // --- IsValidCombatChoice: padded input ---

    [TestMethod]
    public void IsValidCombatChoice_ReturnsTrue_ForPaddedA()
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidCombatChoice("  a  "));
    }

    // --- IsValidMerchantChoice ---

    [TestMethod]
    [DataRow("a")]
    [DataRow("armor")]
    [DataRow("w")]
    [DataRow("weapon")]
    [DataRow("n")]
    [DataRow("nothing")]
    public void IsValidMerchantChoice_ReturnsTrue_ForEveryAcceptedSpelling(string input)
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidMerchantChoice(input));
    }

    [TestMethod]
    [DataRow("A")]
    [DataRow("ARMOR")]
    [DataRow("Weapon")]
    [DataRow("NOTHING")]
    public void IsValidMerchantChoice_ReturnsTrue_RegardlessOfCase(string input)
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidMerchantChoice(input));
    }

    [TestMethod]
    [DataRow("  a  ")]
    [DataRow("  armor  ")]
    [DataRow("  w  ")]
    [DataRow(" WEAPON ")]
    [DataRow("  n  ")]
    [DataRow("  nothing  ")]
    public void IsValidMerchantChoice_ReturnsTrue_WhenPaddedWithWhitespace(string input)
    {
        Game game = new Game();
        Assert.IsTrue(game.IsValidMerchantChoice(input));
    }

    [TestMethod]
    public void IsValidMerchantChoice_ReturnsFalse_ForNull()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMerchantChoice(null));
    }

    [TestMethod]
    public void IsValidMerchantChoice_ReturnsFalse_ForEmpty()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMerchantChoice(""));
    }

    [TestMethod]
    public void IsValidMerchantChoice_ReturnsFalse_ForWhitespace()
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMerchantChoice("   "));
    }

    [TestMethod]
    [DataRow("banana")]
    [DataRow("buy")]
    [DataRow("1")]
    public void IsValidMerchantChoice_ReturnsFalse_ForJunk(string input)
    {
        Game game = new Game();
        Assert.IsFalse(game.IsValidMerchantChoice(input));
    }

    // --- The Merchant Encounter ---
    // Console-driven, so the ticket asks only that it not throw; its real logic lives
    // behind Merchant.ApplyChoice and IsValidMerchantChoice, which are asserted properly.
    // HandleMerchantEncounter is public only so these can drive it.

    // Console redirection is process-global and this assembly parallelizes by method,
    // so these two must not run alongside anything else or they lose their fake stdin.
    [TestMethod]
    [DoNotParallelize]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void HandleMerchantEncounter_DoesNotThrow_WhenThePlayerDeclines(string language)
    {
        RunWithConsoleInput("n", () =>
        {
            Game game = new Game();
            game.HandleMerchantEncounter(NewPlayer(), GetMessages(language));
        });
    }

    [TestMethod]
    [DoNotParallelize]
    public void HandleMerchantEncounter_DoesNotThrow_WhenAnInvalidAnswerIsRePrompted()
    {
        RunWithConsoleInput("banana\n\n   \nn", () =>
        {
            Game game = new Game();
            game.HandleMerchantEncounter(NewPlayer(), GetMessages("English"));
        });
    }

    private static Player NewPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // Drives the prompt loop from a canned answer and swallows its output, so the
    // Console-driven Encounter can be exercised without a human at the keyboard.
    private static void RunWithConsoleInput(string input, Action action)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        try
        {
            Console.SetIn(new StringReader(input));
            Console.SetOut(new StringWriter());
            action();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}
