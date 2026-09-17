using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DieTests
{
    [TestMethod]
    public void Roll_ReturnsValueWithinRange_ForTwentySidedDie()
    {
        Die die = new Die();
        int result = die.Roll(20);

        Assert.IsTrue(result >= 1 && result <= 20);
    }

    [TestMethod]
    public void Roll_TakesItsRollsFromTheSuppliedSource()
    {
        ScriptedDie die = new ScriptedDie(7, 20, 1);

        Assert.AreEqual(7, die.Roll(20));
        Assert.AreEqual(20, die.Roll(20));
        Assert.AreEqual(1, die.Roll(6));
        die.AssertAllRollsUsed();
    }
}
