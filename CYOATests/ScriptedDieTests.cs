using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ScriptedDieTests
{
    [TestMethod]
    public void Roll_FailsTheTest_WhenAskedForMoreRollsThanScripted()
    {
        ScriptedDie die = new ScriptedDie(4);
        die.Roll(20);

        Assert.ThrowsException<AssertFailedException>(() => die.Roll(20));
    }

    [TestMethod]
    public void Roll_FailsTheTest_WhenTheScriptedRollIsAboveTheSidesRequested()
    {
        ScriptedDie die = new ScriptedDie(13);

        Assert.ThrowsException<AssertFailedException>(() => die.Roll(12));
    }

    [TestMethod]
    public void Roll_FailsTheTest_WhenTheScriptedRollIsBelowOne()
    {
        ScriptedDie die = new ScriptedDie(0);

        Assert.ThrowsException<AssertFailedException>(() => die.Roll(20));
    }

    [TestMethod]
    public void AssertAllRollsUsed_FailsTheTest_WhenAScriptedRollIsUnused()
    {
        ScriptedDie die = new ScriptedDie(5, 6);
        die.Roll(20);

        Assert.ThrowsException<AssertFailedException>(() => die.AssertAllRollsUsed());
    }

    [TestMethod]
    public void ForDragon_ScriptsTheDragonsStatistics_BeforeTheRest()
    {
        ScriptedDie die = ScriptedDie.ForDragon(14, 9, 17, 3);
        Dragon dragon = new Dragon(die);

        Assert.AreEqual(14, dragon.Strength);
        Assert.AreEqual(9, dragon.Agility);
        Assert.AreEqual(17, dragon.HealthPoints);
        Assert.AreEqual(3, dragon.RollAttack());
        die.AssertAllRollsUsed();
    }

    [TestMethod]
    public void AfterDragonStats_SkipsPastTheDragonsStatistics()
    {
        ScriptedDie die = ScriptedDie.AfterDragonStats(11);
        Dragon dragon = new Dragon(die);

        Assert.AreEqual(11, dragon.RollAttack());
        die.AssertAllRollsUsed();
    }
}
