[TestClass]
public class CombatTests
{
    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
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

    private Dragon CreateTestDragon()
    {
        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return dragon;
    }

    [TestMethod]
    public void Combat_Constructor_InitializesPlayerAndDragon()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.AreSame(player, combat.player);
        Assert.AreSame(dragon, combat.dragon);
    }

    [TestMethod]
    public void Combat_PlayerRetreated_DefaultsToFalse()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.IsFalse(combat.PlayerRetreated);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesCombatHeaders()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_player_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_dragon_header"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesPlayerCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
        StringAssert.Contains(result, player.Weapon!.Type);
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesDragonCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, dragon.Name);
        StringAssert.Contains(result, dragon.Strength.ToString());
        StringAssert.Contains(result, dragon.Agility.ToString());
        StringAssert.Contains(result, dragon.HealthPoints.ToString());
        StringAssert.Contains(result, dragon.Weapon.Type);
        StringAssert.Contains(result, dragon.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
    }
}

[TestClass]
[DoNotParallelize]
public class CombatInputTests
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

    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    // Strength 0 means every attack roll (1-20) misses, so nobody takes damage
    // and the number of rounds depends only on the player's input.
    private Combat CreateCombatWhereEveryAttackMisses(Messages messages)
    {
        Player player = new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 0,
            Agility = 0,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 0,
            Agility = 0,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return new Combat(player, dragon, messages);
    }

    private string RunCombat(Combat combat, string input, out bool playerWon)
    {
        Console.SetIn(new StringReader(input));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        playerWon = combat.StartCombat();

        return writer.ToString();
    }

    private int CountOccurrences(string text, string value)
    {
        int count = 0;
        int index = text.IndexOf(value, StringComparison.Ordinal);

        while (index >= 0)
        {
            count++;
            index = text.IndexOf(value, index + value.Length, StringComparison.Ordinal);
        }

        return count;
    }

    private int CountPlayerAttacks(string output, Messages messages)
    {
        return CountOccurrences(output, string.Format(messages.GetMessage("player_missed_dragon"), "Smolderfang"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void StartCombat_InvalidInput_DoesNotStartAnotherRound()
    {
        Messages messages = CreateEnglishMessages();
        Combat combat = CreateCombatWhereEveryAttackMisses(messages);

        string output = RunCombat(combat, "x\nr\n", out bool playerWon);

        Assert.AreEqual(1, CountPlayerAttacks(output, messages));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("invalid")));
        Assert.IsFalse(playerWon);
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    [Timeout(5000)]
    public void StartCombat_RepeatedInvalidInput_KeepsPromptingWithoutAttacking()
    {
        Messages messages = CreateEnglishMessages();
        Combat combat = CreateCombatWhereEveryAttackMisses(messages);

        string output = RunCombat(combat, "x\n\nre\nfoo\nr\n", out bool playerWon);

        Assert.AreEqual(1, CountPlayerAttacks(output, messages));
        Assert.AreEqual(4, CountOccurrences(output, messages.GetMessage("invalid")));
        Assert.AreEqual(5, CountOccurrences(output, messages.GetMessage("attack_prompt")));
        Assert.IsFalse(playerWon);
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    [Timeout(5000)]
    public void StartCombat_InvalidInputThenAttack_StartsExactlyOneMoreRound()
    {
        Messages messages = CreateEnglishMessages();
        Combat combat = CreateCombatWhereEveryAttackMisses(messages);

        string output = RunCombat(combat, "x\na\nr\n", out bool playerWon);

        Assert.AreEqual(2, CountPlayerAttacks(output, messages));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("invalid")));
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    [Timeout(5000)]
    public void StartCombat_ValidInputIsTrimmedAndCaseInsensitive()
    {
        Messages messages = CreateEnglishMessages();
        Combat combat = CreateCombatWhereEveryAttackMisses(messages);

        string output = RunCombat(combat, "  ATTACK  \n  R  \n", out bool playerWon);

        Assert.AreEqual(2, CountPlayerAttacks(output, messages));
        Assert.AreEqual(0, CountOccurrences(output, messages.GetMessage("invalid")));
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    [Timeout(5000)]
    public void StartCombat_EndOfInput_RetreatsInsteadOfFightingOn()
    {
        Messages messages = CreateEnglishMessages();
        Combat combat = CreateCombatWhereEveryAttackMisses(messages);

        string output = RunCombat(combat, "", out bool playerWon);

        Assert.AreEqual(1, CountPlayerAttacks(output, messages));
        StringAssert.Contains(output, messages.GetMessage("retreat_combat"));
        Assert.IsFalse(playerWon);
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    [Timeout(5000)]
    public void StartCombat_EndOfInputAfterInvalidInput_Retreats()
    {
        Messages messages = CreateEnglishMessages();
        Combat combat = CreateCombatWhereEveryAttackMisses(messages);

        string output = RunCombat(combat, "x\n", out bool playerWon);

        Assert.AreEqual(1, CountPlayerAttacks(output, messages));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("invalid")));
        Assert.IsFalse(playerWon);
        Assert.IsTrue(combat.PlayerRetreated);
    }
}
