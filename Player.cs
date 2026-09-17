public class Player
{
    private readonly Die _die;

    public Player()
        : this(new Die())
    {
    }

    // Lets tests hand the Character a scripted Die, so its rolls can be pinned.
    public Player(Die die)
    {
        _die = die;
    }

    public string Name { get; set; } = "";
    public string Race { get; set; } = "";
    public string Occupation { get; set; } = "";

    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    // The ceiling healing is capped against, recorded at creation and fixed for the
    // Adventure. The Dragon has no equivalent and gains none, as CONTEXT.md states.
    public int MaximumHealthPoints { get; set; }

    // Fixed at creation rather than rolled, so no Adventure can begin with a purse
    // too small to matter. Sized to afford one piece of the Merchant's stock, not two.
    public const int StartingGold = 100;

    public int Gold { get; set; } = StartingGold;

    public Weapon? Weapon { get; set; }

    public Armor? Armor { get; set; }

    public Potion? Potion { get; set; }

    public void CreateCharacter(Messages messages)
    {
        Race = PromptForRace(messages);
        Name = PromptForName(messages);
        Occupation = PromptForOccupation(messages);

        AssignWeaponByOccupation();

        int strengthModifier = RollStrength(messages);
        RollAgility(messages);
        RollHealthPoints(messages, strengthModifier);

        PromptForNextAction(messages);
    }

    private string PromptForRace(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("race_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (messages.IsValidRace(input))
                return messages.NormalizeRace(input!);

            Console.WriteLine(messages.GetMessage("race_invalid"));
        }
    }

    private string PromptForName(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("name_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine(messages.GetMessage("name_invalid"));
        }
    }

    private string PromptForOccupation(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("occupation_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (messages.IsValidOccupation(input))
                return messages.NormalizeOccupation(input!);

            Console.WriteLine(messages.GetMessage("occupation_invalid"));
        }
    }

    private void AssignWeaponByOccupation()
    {
        switch (Occupation.ToLower())
        {
            case "fighter":
                Weapon = new Weapon("long sword", 12, "-)=====>");
                break;
            case "magician":
                Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~");
                break;
            case "thief":
                Weapon = new Weapon("dagger", 6, "-)==>");
                break;
            case "archer":
                Weapon = new Weapon("long bow", 8, "}    -->");
                break;
        }
    }

    private int RollStrength(Messages messages)
    {
        PromptRoll(messages.GetMessage("roll_strength"), messages);
        Strength = _die.Roll(20);

        int modifier = _die.Roll(4);

        if (Race == "Halfling")
        {
            Strength -= modifier;
            return -modifier;
        }

        Strength += modifier;
        return modifier;
    }

    private void RollAgility(Messages messages)
    {
        PromptRoll(messages.GetMessage("roll_agility"), messages);
        Agility = _die.Roll(20);

        if (Race == "Halfling" || Race == "Elf")
            Agility += _die.Roll(4);
    }

    private void RollHealthPoints(Messages messages, int strengthModifier)
    {
        PromptRoll(messages.GetMessage("roll_health"), messages);
        SetHealthPointsAtCreation(_die.Roll(20), strengthModifier);
    }


    private void PromptForNextAction(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("next_action"));
            string? input = Console.ReadLine()?.Trim();

            if (input == "1")
                DisplayStats(messages);
            else if (input == "2")
                return;
            else
                Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(GetStatsDisplay(messages));
    }

    public string GetStatsDisplay(Messages messages)
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("stats_header"));
        output.Add(messages.GetMessage("stats_name") + Name);
        output.Add(messages.GetMessage("stats_race") + messages.TranslateRaceForDisplay(Race));
        output.Add(messages.GetMessage("stats_occupation") + messages.TranslateOccupationForDisplay(Occupation));
        output.Add(messages.GetMessage("stats_strength") + Strength);
        output.Add(messages.GetMessage("stats_agility") + Agility);
        output.Add(string.Format(messages.GetMessage("stats_health_of_max"), HealthPoints, MaximumHealthPoints));
        output.Add(messages.GetMessage("stats_gold") + Gold);

        // Every Equipment slot is shown, filled or not, so a player can tell "I carry no Armor"
        // from "this game has no Armor". An empty slot gets no number line: "Damage Reduction: 0"
        // would read as a piece worth nothing. Each slot has its own "none" message so its
        // wording can agree with that slot's noun. The combat display still omits empty slots.
        if (Weapon != null)
        {
            output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
            output.Add(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
        }
        else
        {
            output.Add(messages.GetMessage("stats_weapon") + messages.GetMessage("stats_weapon_none"));
        }

        if (Armor != null)
        {
            output.Add(messages.GetMessage("stats_armor") + messages.TranslateArmorForDisplay(Armor.Type));
            output.Add(messages.GetMessage("stats_damage_reduction") + Armor.DamageReduction);
        }
        else
        {
            output.Add(messages.GetMessage("stats_armor") + messages.GetMessage("stats_armor_none"));
        }

        if (Potion != null)
        {
            output.Add(messages.GetMessage("stats_potion") + messages.TranslatePotionForDisplay(Potion.Type));
            output.Add(messages.GetMessage("stats_potion_restores") + Potion.HealthPointsRestored);
        }
        else
        {
            output.Add(messages.GetMessage("stats_potion") + messages.GetMessage("stats_potion_none"));
        }

        return string.Join(Environment.NewLine, output);
    }

    // Split from the roll so the arithmetic can be asserted without seeding Die, which is
    // constructed privately here and cannot be injected. Maximum Health Points is recorded
    // from the same figure rather than derived later, so a Halfling's subtracted Strength
    // modifier lowers the ceiling along with the roll: the cap reflects the Character
    // actually rolled. See docs/adr/0002-healing-as-a-capped-flat-restore-that-costs-the-round.md.
    public void SetHealthPointsAtCreation(int roll, int strengthModifier)
    {
        HealthPoints = roll + strengthModifier;
        MaximumHealthPoints = HealthPoints;
    }

    // Whether the round may be spent on a drink at all. The prompt asks before offering the
    // option and before spending a round on it, and DrinkPotion refuses off this same answer,
    // so the two rules are stated once and a caller that forgets to ask is still refused.
    public bool CanDrinkPotion()
    {
        return Potion != null && HealthPoints < MaximumHealthPoints;
    }

    // The one place Health Points are raised. The restore is flat and clamped at Maximum
    // Health Points, so the Potion can never make a Character stronger than the one rolled
    // at creation; any future source of healing must come through here or the ceiling
    // silently stops working for it, as recorded in
    // docs/adr/0002-healing-as-a-capped-flat-restore-that-costs-the-round.md.
    //
    // It owns both refusals as well, returning their narrative and leaving the Character
    // untouched, so the fight has no healing arithmetic of its own to get wrong. The
    // narrative is returned rather than printed, the way the attack methods return theirs.
    public string DrinkPotion(Messages messages)
    {
        if (!CanDrinkPotion())
            return Potion == null
                ? string.Format(messages.GetMessage("potion_none_carried"), Name)
                : string.Format(messages.GetMessage("potion_refused_at_maximum"), Name, MaximumHealthPoints);

        int restored = Math.Min(Potion.HealthPointsRestored, MaximumHealthPoints - HealthPoints);
        HealthPoints += restored;

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("potion_drunk"),
            Name,
            messages.TranslatePotionForDisplay(Potion.Type)
        ));

        if (!string.IsNullOrWhiteSpace(Potion.AsciiArt))
            output.Add(Potion.AsciiArt);

        output.Add(string.Format(
            messages.GetMessage("potion_restored"),
            restored,
            HealthPoints,
            MaximumHealthPoints
        ));

        Potion = null;

        return string.Join(Environment.NewLine, output);
    }

    // The single step every blow aimed at this Character goes through. Armor is the only
    // source of Damage Reduction, and the result floors at zero so a blow can never heal.
    // Any future source of Damage must route through here or Armor stops working for it,
    // as recorded in docs/adr/0001-armor-as-flat-damage-reduction.md.
    public int ReduceDamage(int damage)
    {
        if (Armor == null)
            return damage;

        int reduced = damage - Armor.DamageReduction;

        return reduced < 0 ? 0 : reduced;
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
        if (Weapon == null)
            return 0;

        return _die.Roll(Weapon.MaxDamage);
    }

    public string Attack(Dragon dragon, Messages messages)
    {
        if (Weapon == null)
            return "You have no weapon to attack with.";

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(Occupation),
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        // If this fails to compile, change AsciiArt to whatever exists in Weapon.cs
        output.Add(Weapon.AsciiArt);

        int attackRoll = RollAttack();

        // MISS
        if (!AttackHits(attackRoll))
        {
            output.Add(string.Format(messages.GetMessage("player_missed_dragon"), dragon.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("player_hit_dragon"), dragon.Name));

        int defenseRoll = _die.Roll(20);

        // DEFENSE
        if (defenseRoll <= dragon.Agility)
        {
            output.Add(string.Format(messages.GetMessage("dragon_defended_player_attack"), dragon.Name));
            output.Add(string.Format(
                dragon.GetRandomDefenseTaunt(messages),
                messages.TranslateRaceForDisplay(Race)
            ));
            return string.Join(Environment.NewLine, output);
        }

        // DAMAGE
        int damage = RollDamage();
        dragon.HealthPoints -= damage;

        output.Add(string.Format(messages.GetMessage("player_damage_dealt"), damage));
        output.Add(string.Format(messages.GetMessage("dragon_health_now"), dragon.Name, dragon.HealthPoints));

        // END
        if (dragon.HealthPoints <= 0)
            output.Add(messages.GetMessage("dragon_defeated_narrative"));
        else
            output.Add(dragon.GetRandomDamageReply(messages));

        return string.Join(Environment.NewLine, output);
    }

    private void PromptRoll(string prompt, Messages messages)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "roll")
                return;

            Console.WriteLine(messages.GetMessage("roll_invalid"));
        }
    }
}