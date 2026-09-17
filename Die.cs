using System;

public class Die
{
    private readonly Random _random;

    public Die()
        : this(new Random())
    {
    }

    // Lets tests supply the numbers, so a combat outcome can be pinned exactly.
    public Die(Random random)
    {
        _random = random;
    }

    public int Roll(int sides)
    {
        return _random.Next(1, sides + 1);
    }
}
