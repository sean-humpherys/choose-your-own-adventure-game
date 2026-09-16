public class Merchant
{
    // All three wares cost the same, so the choice is about fit rather than budget.
    // Priced against Player.StartingGold to afford exactly one item, never two.
    public const int ItemPrice = 75;

    // The single Armor the Merchant stocks. The reduction is calibrated in ADR-0001
    // against the Dragon's claws and the Character's Health Points: 3 nearly halves the
    // average blow, while values near 5 make the fight close to unloseable.
    public const string ArmorType = "magical armor";
    public const int ArmorDamageReduction = 3;
    private const string ArmorAsciiArt = "[#####]";

    // Added to the replaced Weapon's maximum Damage rather than replacing it, so the
    // ordering the Occupations set at creation survives: every Occupation's upgrade is
    // worth the same, and a Thief's dagger is not levelled up to a Fighter's sword.
    public const int MagicalDamageIncrement = 6;

    // The single Healing Potion the Merchant stocks. The restore is calibrated in ADR-0002
    // against the Dragon's claws and what the Armor saves across a fight: 8 is a little over
    // one average blow. Holding it until well wounded is the right play because the clamp
    // wastes an early drink and the lost round punishes a safe one, not because of its size.
    public const string PotionType = "healing potion";
    public const int PotionHealthPointsRestored = 8;
    private const string PotionAsciiArt = "(~~~)";

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

        output.Add(string.Format(
            messages.GetMessage("merchant_offer_potion"),
            ItemPrice,
            PotionHealthPointsRestored));

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

            case "armor":
                return BuyArmor(player, messages);

            case "potion":
                return BuyPotion(player, messages);

            default:
                return "";
        }
    }

    private string BuyArmor(Player player, Messages messages)
    {
        if (player.Gold < ItemPrice)
            return messages.GetMessage("merchant_cannot_afford");

        player.Armor = new Armor(ArmorType, ArmorDamageReduction, ArmorAsciiArt);
        player.Gold -= ItemPrice;

        return string.Format(
            messages.GetMessage("merchant_armor_bought"),
            messages.TranslateArmorForDisplay(player.Armor.Type),
            player.Armor.DamageReduction,
            player.Gold
        );
    }

    private string BuyPotion(Player player, Messages messages)
    {
        if (player.Gold < ItemPrice)
            return messages.GetMessage("merchant_cannot_afford");

        player.Potion = new Potion(PotionType, PotionHealthPointsRestored, PotionAsciiArt);
        player.Gold -= ItemPrice;

        return string.Format(
            messages.GetMessage("merchant_potion_bought"),
            messages.TranslatePotionForDisplay(player.Potion.Type),
            player.Potion.HealthPointsRestored,
            player.Gold
        );
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
