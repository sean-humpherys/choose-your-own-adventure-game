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
        List<int> sidesRequested = new List<int>();
        Die die = new Die(sides =>
        {
            sidesRequested.Add(sides);
            return 4;
        });

        Assert.AreEqual(4, die.Roll(20));
        Assert.AreEqual(4, die.Roll(12));
        CollectionAssert.AreEqual(new List<int> { 20, 12 }, sidesRequested);
    }
}
