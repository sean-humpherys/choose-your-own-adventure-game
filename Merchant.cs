public class Merchant
{
    // Both items cost the same, so the choice is about fit rather than budget.
    // Priced against Player.StartingGold to afford exactly one item, never two.
    public const int ItemPrice = 75;

    // Added to the replaced Weapon's maximum Damage rather than replacing it, so the
    // ordering the Occupations set at creation survives: every Occupation's upgrade is
    // worth the same, and a Thief's dagger is not levelled up to a Fighter's sword.
    public const int MagicalDamageIncrement = 6;

    // Only the name and the art vary by Occupation; the Damage is always derived from the
    // Weapon being replaced, so this deliberately is not a Weapon. A Weapon here would need
    // a meaningless maximum Damage that a later reader could mistake for a real one.
    private sealed record MagicalWeapon(string Type, string AsciiArt);

    // Keyed by the lowercased Occupation, the way Player.AssignWeaponByOccupation matches.
    private static readonly Dictionary<string, MagicalWeapon> _magicalWeaponsByOccupation = new()
    {
        ["fighter"] = new MagicalWeapon("runed long sword", "-)==***==>"),
        ["magician"] = new MagicalWeapon("chain lightning spell", "zap~~*~~zap"),
        ["thief"] = new MagicalWeapon("shadowfang dagger", "-)=*>"),
        ["archer"] = new MagicalWeapon("windsong long bow", "}   ==*-->")
    };

    public string GetOffer(Player player, Messages messages)
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("merchant_greeting"));
        output.Add(string.Format(messages.GetMessage("merchant_offer_armor"), ItemPrice));

        // A relative increment needs a Weapon to build on, so a Character carrying none
        // is never offered one.
        if (FindMagicalWeaponFor(player) != null)
            output.Add(string.Format(messages.GetMessage("merchant_offer_weapon"), ItemPrice));

        output.Add(string.Format(messages.GetMessage("merchant_your_gold"), player.Gold));

        return string.Join(Environment.NewLine, output);
    }

    // The Encounter's single seam: it takes an already-parsed choice, applies it to the
    // Character, and returns the narrative rather than writing to the Console.
    public string ApplyChoice(Player player, string choice, Messages messages)
    {
        switch (choice)
        {
            case "weapon":
                return BuyMagicalWeapon(player, messages);

            case "nothing":
                return messages.GetMessage("merchant_declined");

            // Buying the Armor is wired up in ticket 06. Until then it is a valid answer
            // that leaves the Character untouched and says nothing.
            case "armor":
            default:
                return "";
        }
    }

    private string BuyMagicalWeapon(Player player, Messages messages)
    {
        MagicalWeapon? offered = FindMagicalWeaponFor(player);

        if (offered == null)
            return messages.GetMessage("merchant_no_weapon_to_better");

        if (player.Gold < ItemPrice)
            return messages.GetMessage("merchant_cannot_afford");

        player.Weapon = new Weapon(
            offered.Type,
            player.Weapon!.MaxDamage + MagicalDamageIncrement,
            offered.AsciiArt
        );
        player.Gold -= ItemPrice;

        return string.Format(
            messages.GetMessage("merchant_weapon_bought"),
            messages.TranslateWeaponForDisplay(player.Weapon.Type),
            player.Weapon.MaxDamage,
            player.Gold
        );
    }

    // Null when there is nothing to better: no Weapon carried, or an Occupation with no
    // Magical Weapon of its own. Validation makes the second unreachable, but the
    // Occupation-to-Weapon assignment has no default branch either, so the type permits it.
    private MagicalWeapon? FindMagicalWeaponFor(Player player)
    {
        if (player.Weapon == null)
            return null;

        return _magicalWeaponsByOccupation.TryGetValue(player.Occupation.ToLower(), out MagicalWeapon? magical)
            ? magical
            : null;
    }
}
