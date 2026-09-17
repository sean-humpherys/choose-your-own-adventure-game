// Mirrors Armor: a type name for display, the value it contributes, and ASCII art.
// A Character carries at most one. This is a class rather than a bare integer on the
// Character because the type name is needed for display and translation regardless,
// so an integer would save nothing and split one concept across two fields.
public class Potion
{
    public string Type { get; set; }
    public int HealthPointsRestored { get; set; }
    public string AsciiArt { get; set; }

    public Potion(string type, int healthPointsRestored, string asciiArt)
    {
        Type = type;
        HealthPointsRestored = healthPointsRestored;
        AsciiArt = asciiArt;
    }
}
