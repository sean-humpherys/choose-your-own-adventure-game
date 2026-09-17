using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// A Die whose rolls are written by the test, in order. It fails the test when asked
// for a roll it wasn't given, or when a scripted roll can't come from the Die requested.
// Unused rolls fail the test that calls AssertAllRollsUsed, and are caught regardless at
// the end of the run, so a change in how many rolls something uses cannot slip through.
public class ScriptedDie : Die
{
    private static readonly ConcurrentBag<ScriptedDie> EveryDie = new ConcurrentBag<ScriptedDie>();

    private readonly ScriptedRolls _rolls;
    private readonly string _builtByTest;
    private bool _checked;

    public ScriptedDie(params int[] rolls)
        : this(new ScriptedRolls(rolls))
    {
    }

    private ScriptedDie(ScriptedRolls rolls)
        : base(rolls.Next)
    {
        _rolls = rolls;
        _builtByTest = FindCallingTest();
        EveryDie.Add(this);
    }

    // Scripts the three rolls a new Dragon consumes for its statistics, then the rest.
    public static ScriptedDie ForDragon(int strength, int agility, int healthPoints, params int[] rolls)
    {
        return new ScriptedDie([strength, agility, healthPoints, .. rolls]);
    }

    // Skips past a new Dragon's statistics with placeholder rolls, for tests that set
    // the statistics afterwards.
    public static ScriptedDie AfterDragonStats(params int[] rolls)
    {
        return ForDragon(1, 1, 1, rolls);
    }

    public void AssertAllRollsUsed()
    {
        _checked = true;
        _rolls.AssertAllUsed();
    }

    // Fails once for every die whose test never checked it and left rolls unused, naming
    // those tests. A die a test already checked has had its failure reported there.
    public static void AssertNoRollsLeftUnused(IEnumerable<ScriptedDie> dice)
    {
        List<string> leftovers = new List<string>();

        foreach (ScriptedDie die in dice.Where(die => !die._checked))
        {
            die._checked = true;

            if (die._rolls.Remaining.Count > 0)
                leftovers.Add($"{die._builtByTest} left roll(s) unused: {string.Join(", ", die._rolls.Remaining)}");
        }

        if (leftovers.Count > 0)
            Assert.Fail(string.Join(Environment.NewLine, leftovers));
    }

    internal static void AssertNoDieLeftRollsUnused()
    {
        AssertNoRollsLeftUnused(EveryDie);
    }

    private static string FindCallingTest()
    {
        MethodBase? test = new StackTrace()
            .GetFrames()
            .Select(frame => frame.GetMethod())
            .FirstOrDefault(method => method?.GetCustomAttribute<TestMethodAttribute>() != null);

        return test == null ? "A test" : $"{test.DeclaringType?.Name}.{test.Name}";
    }

    private sealed class ScriptedRolls
    {
        public Queue<int> Remaining { get; }

        public ScriptedRolls(int[] rolls)
        {
            Remaining = new Queue<int>(rolls);
        }

        public int Next(int sides)
        {
            if (Remaining.Count == 0)
                Assert.Fail($"The Die was asked for a d{sides} roll, but no scripted rolls remain.");

            int roll = Remaining.Dequeue();

            if (roll < 1 || roll > sides)
                Assert.Fail($"Scripted roll {roll} can't come from a d{sides}.");

            return roll;
        }

        public void AssertAllUsed()
        {
            if (Remaining.Count > 0)
                Assert.Fail($"{Remaining.Count} scripted roll(s) were never used: {string.Join(", ", Remaining)}.");
        }
    }
}

[TestClass]
public class ScriptedDieSweep
{
    [AssemblyCleanup]
    public static void FailTheRunIfAnyScriptedRollWentUnused()
    {
        ScriptedDie.AssertNoDieLeftRollsUnused();
    }
}
