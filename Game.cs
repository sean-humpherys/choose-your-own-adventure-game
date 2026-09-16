public class Game
{
    public void StartGame()
    {
        // Title screen
        Console.WriteLine("=====================================");
        Console.WriteLine("Welcome to Choose Your Adventure Game");
        Console.WriteLine("=====================================\n");

        // Instantiate Die first (per sequence diagram)
        Die die = new Die();

        Dragon dragon = new Dragon(die);

        // Language selection shown before Messages dictionary is loaded
        Console.WriteLine("Select language / Selecciona idioma / Choisissez la langue:");
        Console.WriteLine("1. English  2. Espanol  3. Francais");
        Console.Write("Enter choice: ");
        string? langInput = Console.ReadLine()?.Trim();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();
        // Display welcome message
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("welcome"));
        Console.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            Console.WriteLine(messages.GetMessage("menu"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                Player player = new Player();
                player.CreateCharacter(messages);

                // Tripped when the Merchant is met, not when something is bought, so
                // declining consumes the visit. Scoped to the Adventure, so a new
                // Character meets him afresh.
                bool merchantMet = false;

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    string selectedPath = PromptForPath(messages);

                    if (selectedPath == "exit")
                    {
                        Console.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        if (merchantMet)
                        {
                            Console.WriteLine(messages.GetMessage("south_path_empty"));
                        }
                        else
                        {
                            HandleMerchantEncounter(player, messages);
                            merchantMet = true;
                        }
                    }
                    else if (selectedPath == "north")
                    {
                        HandleDragonEncounter(player, dragon, messages);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    public void EndGame(bool playerWon, bool playerRetreated, Messages messages)
    {
        if (playerRetreated)
            Console.WriteLine(messages.GetMessage("retreat"));
        else if (playerWon)
            Console.WriteLine(messages.GetMessage("victory"));
        else
            Console.WriteLine(messages.GetMessage("defeat"));
    }

    public bool IsValidMenuChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2";
    }

    public bool IsValidPathChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "e" || trimmed == "exit";
    }

    // Answers off Combat's parser rather than repeating the spellings, so the validator and
    // the round loop cannot drift apart, exactly as IsValidMerchantChoice does below.
    public bool IsValidCombatChoice(string? input)
    {
        return Combat.ParseCombatChoice(input) != null;
    }

    public bool IsValidMerchantChoice(string? input)
    {
        return ParseMerchantChoice(input) != null;
    }

    // The one place the accepted spellings live. Validating and parsing off the same
    // list means a spelling can never be accepted by one and unrecognised by the other.
    private string? ParseMerchantChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        switch (input.Trim().ToLower())
        {
            case "a":
            case "armor":
                return "armor";

            case "w":
            case "weapon":
                return "weapon";

            case "n":
            case "nothing":
                return "nothing";

            default:
                return null;
        }
    }

    private string PromptForPath(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("path_prompt_full"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "north" || input == "n")
                return "north";

            if (input == "south" || input == "s")
                return "south";

            if (input == "exit" || input == "e")
                return "exit";

            Console.WriteLine(messages.GetMessage("path_invalid"));
        }
    }

    private void HandleMerchantEncounter(Player player, Messages messages)
    {
        Merchant merchant = new Merchant();

        Console.WriteLine(messages.GetMessage("south_path_narrative"));
        Console.WriteLine(merchant.GetOffer(player, messages));

        string choice = PromptForMerchantChoice(messages);
        string narrative = merchant.ApplyChoice(player, choice, messages);

        // Every answer the prompt can return has something to say; the guard covers only
        // the unparseable choice the seam refuses to narrate.
        if (narrative.Length > 0)
            Console.WriteLine(narrative);
    }

    private string PromptForMerchantChoice(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("merchant_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine();

            // End of input, so there is no one left to re-prompt. Declining is the
            // outcome that leaves the Character untouched, so it is the safe one.
            if (input == null)
                return "nothing";

            string? choice = ParseMerchantChoice(input);

            if (choice != null)
                return choice;

            Console.WriteLine(messages.GetMessage("merchant_invalid"));
        }
    }

    private void HandleDragonEncounter(Player player, Dragon dragon, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("north_path_narrative"));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages);
        while (true)
        {
            Console.WriteLine(messages.GetMessage("dragon_encounter_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "a" || input == "attack")
            {
                Combat combat = new Combat(player, dragon, messages);
                bool playerWon = combat.StartCombat();

                if (combat.PlayerRetreated)
                {
                    return; //combat already handled retreat message
                }
                else
                {
                   EndGame(playerWon, false, messages); 
                }
                
                return;
            }

        if (input == "r" || input == "retreat")
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return;
        }

        Console.WriteLine(messages.GetMessage("invalid"));
    }
}
}
