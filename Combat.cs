public class Combat
{
    public Player player { get; set; }
    public Dragon dragon { get; set; }
    public Messages messages { get; set; }

    public bool PlayerRetreated { get; private set; }

    public Combat(Player player, Dragon dragon, Messages messages)
    {
        this.player = player;
        this.dragon = dragon;
        this.messages = messages;
        PlayerRetreated = false;
    }

    public bool StartCombat()
    {
        Console.WriteLine(GetCombatStatsDisplay());

        // The opening round attacks without asking: the fight was entered by choosing to
        // attack at the Encounter, and each prompt then chooses the following round's action.
        CombatAction action = CombatAction.Attack;

        while (true)
        {
            if (action == CombatAction.Attack)
                PlayerAttacksDragonSequence();

            if (dragon.HealthPoints <= 0)
                return true;

            DragonAttacksPlayerSequence();

            if (player.HealthPoints <= 0)
                return false;

            action = PromptForAction();

            if (action == CombatAction.Retreat)
            {
                PlayerRetreated = true;
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return false;
            }
        }
    }

    // Asks until the answer is one it recognises, so a slip costs nothing. Returning the
    // action rather than acting on it keeps this the thing that prompts and prints, and
    // lets the round decide what to do with the answer.
    private CombatAction PromptForAction()
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("attack_prompt"));

            string? input = Console.ReadLine();

            // End of input, so there is no one left to re-prompt and asking again would
            // spin forever. Retreating is the outcome that ends the fight without further
            // harm, so it is the safe one, as Game.PromptForMerchantChoice does with null.
            if (input == null)
                return CombatAction.Retreat;

            CombatAction? action = ParseCombatChoice(input);

            if (action != null)
                return action.Value;

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    // The one place the combat prompt's accepted spellings live. Game.IsValidCombatChoice
    // answers off this same list, so a spelling can never be accepted by the validator and
    // go unrecognised by the round, the way Merchant choices are parsed and validated once.
    public static CombatAction? ParseCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        switch (input.Trim().ToLower())
        {
            case "a":
            case "attack":
                return CombatAction.Attack;
            case "r":
            case "retreat":
                return CombatAction.Retreat;
            default:
                return null;
        }
    }

    public string GetCombatStatsDisplay()
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("combat_stats_header"));
        output.Add(messages.GetMessage("combat_player_header"));
        output.Add(messages.GetMessage("stats_name") + player.Name);
        output.Add(messages.GetMessage("stats_strength") + player.Strength);
        output.Add(messages.GetMessage("stats_agility") + player.Agility);
        output.Add(string.Format(messages.GetMessage("stats_health_of_max"), player.HealthPoints, player.MaximumHealthPoints));

        if (player.Weapon != null)
        {
            output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(player.Weapon.Type));
            output.Add(messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        }

        if (player.Armor != null)
        {
            output.Add(messages.GetMessage("stats_armor") + messages.TranslateArmorForDisplay(player.Armor.Type));
            output.Add(messages.GetMessage("stats_damage_reduction") + player.Armor.DamageReduction);
        }

        if (player.Potion != null)
        {
            output.Add(messages.GetMessage("stats_potion") + messages.TranslatePotionForDisplay(player.Potion.Type));
            output.Add(messages.GetMessage("stats_potion_restores") + player.Potion.HealthPointsRestored);
        }

        output.Add("");
        output.Add(messages.GetMessage("combat_dragon_header"));
        output.Add(messages.GetMessage("stats_name") + dragon.Name);
        output.Add(messages.GetMessage("stats_strength") + dragon.Strength);
        output.Add(messages.GetMessage("stats_agility") + dragon.Agility);
        output.Add(messages.GetMessage("stats_health") + dragon.HealthPoints);
        output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(dragon.Weapon.Type));
        output.Add(messages.GetMessage("stats_damage") + dragon.Weapon.MaxDamage);

        return string.Join(Environment.NewLine, output);
    }

    public void PlayerAttacksDragonSequence()
    {
        Console.WriteLine(player.Attack(dragon, messages));
    }

    public void DragonAttacksPlayerSequence()
    {
        Console.WriteLine(dragon.Attack(player, messages));
    }
}