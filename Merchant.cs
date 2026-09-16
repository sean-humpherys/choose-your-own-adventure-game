public class Merchant
{
    // Both items cost the same, so the choice is about fit rather than budget.
    // Priced against Player.StartingGold to afford exactly one item, never two.
    public const int ItemPrice = 75;

    public string GetOffer(Player player, Messages messages)
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("merchant_greeting"));
        output.Add(string.Format(messages.GetMessage("merchant_offer_armor"), ItemPrice));
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
            case "nothing":
                return messages.GetMessage("merchant_declined");

            // Buying is wired up in tickets 05 and 06. Until then either item is a valid
            // answer that leaves the Character untouched and says nothing.
            case "armor":
            case "weapon":
            default:
                return "";
        }
    }
}
