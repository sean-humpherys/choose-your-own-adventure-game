public class Dragon : ICombatant
{
    private readonly Die _die;

    public string Name { get; set; } = "";
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    // Weapon is now a real shared class in the repo.
    public Weapon Weapon { get; set; }

    // Example use of taunts:
    // string taunt = dragon.GetRandomDefenseTaunt().Replace("{race}", player.Race);
    private readonly List<string> _defenseTauntKeys = new()
{
    "dragon_defense_taunt_1",
    "dragon_defense_taunt_2",
    "dragon_defense_taunt_3",
    "dragon_defense_taunt_4"
};

    private readonly List<string> _damageReplyKeys = new()
{
    "dragon_damage_reply_1",
    "dragon_damage_reply_2",
    "dragon_damage_reply_3",
    "dragon_damage_reply_4"
};

    public Dragon(Die die)
    {
        _die = die;

        Name = "Smolderfang";

        // Dragon stats are initialized using Die so the class aligns better
        // with the sequence/start-game diagrams and shared repo structure.
        Strength = _die.Roll(20);
        Agility = _die.Roll(20);
        HealthPoints = _die.Roll(20);

        // The dragon uses a placeholder weapon object for now.
        // This can be refined later if combat requirements expand.
        Weapon = new Weapon("claws", 12, "<<< claws >>>");
    }

    public int RollAttack()
    {
        return _die.Roll(20);
    }

    public bool AttackHits(int attackRoll)
    {
        return attackRoll <= Strength;
    }

    public int RollDamage()
    {
        return _die.Roll(Weapon.MaxDamage);
    }

    // The target's Defense Roll against the Dragon's Blow, rolled on the Dragon's Die.
    public int RollDefense()
    {
        return _die.Roll(20);
    }

    // The Dragon wears no Armor and its Health Points are not floored, so a Blow that
    // overshoots leaves them below zero. That is today's behaviour, stated here on purpose.
    public int TakeDamage(int damage)
    {
        HealthPoints -= damage;
        return damage;
    }

    public string Attack(Player player, Messages messages)
    {
        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("dragon_attack_intro"),
            Name,
            player.Name,
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(Weapon.AsciiArt))
            output.Add(Weapon.AsciiArt);

        BlowResult blow = Blow.Resolve(this, player);

        if (blow.Outcome == BlowOutcome.Missed)
        {
            output.Add(string.Format(messages.GetMessage("dragon_missed_player"), Name, player.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("dragon_hit_player"), Name, player.Name));

        if (blow.Outcome == BlowOutcome.Dodged)
        {
            output.Add(string.Format(messages.GetMessage("player_defended_dragon_attack"), player.Name));
            return string.Join(Environment.NewLine, output);
        }

        // The Armor stopped the blow. This is not a dodge and must not say it was one:
        // the Character did not evade, and conflating the two misrepresents the purchase.
        if (blow.DamageTaken == 0)
            output.Add(string.Format(messages.GetMessage("player_armor_absorbed"), player.Name));
        else
            output.Add(string.Format(messages.GetMessage("dragon_damage_dealt"), Name, blow.DamageTaken));

        output.Add(string.Format(messages.GetMessage("player_health_now"), player.Name, player.HealthPoints));

        if (blow.TargetDefeated)
            output.Add(string.Format(messages.GetMessage("player_defeated_narrative"), player.Name));

        return string.Join(Environment.NewLine, output);
    }


    public string GetRandomDefenseTaunt(Messages messages)
    {
        int index = _die.Roll(_defenseTauntKeys.Count) - 1;
        return messages.GetMessage(_defenseTauntKeys[index]);
    }

    public string GetRandomDamageReply(Messages messages)
    {
        int index = _die.Roll(_damageReplyKeys.Count) - 1;
        return messages.GetMessage(_damageReplyKeys[index]);
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage("dragon_stats_header"));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);
        Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
        Console.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
    }
}