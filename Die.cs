using System;

public class Die
{
    private readonly Func<int, int> _rollSource;

    public Die()
    {
        Random random = new Random();
        _rollSource = sides => random.Next(1, sides + 1);
    }

    // Lets tests supply the rolls, so a combat outcome can be pinned exactly. The
    // source is given the number of sides and returns the roll.
    public Die(Func<int, int> rollSource)
    {
        _rollSource = rollSource;
    }

    public int Roll(int sides)
    {
        return _rollSource(sides);
    }
}
