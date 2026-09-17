using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// A Die whose rolls are written by the test, in order. It fails the test when asked
// for a roll it wasn't given, when a scripted roll can't come from the Die requested,
// or, once AssertAllRollsUsed is called, when a scripted roll was never asked for.
public class ScriptedDie : Die
{
    private readonly ScriptedRolls _rolls;

    public ScriptedDie(params int[] rolls)
        : this(new ScriptedRolls(rolls))
    {
    }

    private ScriptedDie(ScriptedRolls rolls)
        : base(rolls)
    {
        _rolls = rolls;
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
        _rolls.AssertAllUsed();
    }

    private sealed class ScriptedRolls : Random
    {
        private readonly Queue<int> _remaining;

        public ScriptedRolls(int[] rolls)
        {
            _remaining = new Queue<int>(rolls);
        }

        public override int Next(int minValue, int maxValue)
        {
            int sides = maxValue - 1;

            if (_remaining.Count == 0)
                Assert.Fail($"The Die was asked for a d{sides} roll, but no scripted rolls remain.");

            int roll = _remaining.Dequeue();

            if (roll < minValue || roll >= maxValue)
                Assert.Fail($"Scripted roll {roll} can't come from a d{sides}.");

            return roll;
        }

        public void AssertAllUsed()
        {
            if (_remaining.Count > 0)
                Assert.Fail($"{_remaining.Count} scripted roll(s) were never used: {string.Join(", ", _remaining)}.");
        }
    }
}
