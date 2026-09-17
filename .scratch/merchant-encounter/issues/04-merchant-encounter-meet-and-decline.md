# 04: Merchant Encounter: meet him, decline, once per Adventure

**What to build:** The south Path stops being a dead end. Travelling south, the player meets a Merchant who tells them what he sells, what each item costs, and how much Gold they are carrying. The player answers, and for now the only outcome that does anything is declining: they keep their Gold and walk away.

Declining still costs them the Encounter. The Merchant is met once per Adventure, and the flag trips on meeting him, not on buying — so a player who says no and comes back finds an empty road. After the Encounter the player returns to the fork and may still take the north Path to the Dragon or leave the game.

The existing south narrative — which tells the player nothing happens and that they return home poor — is now false and must be rewritten, not merely added to, in all three languages.

**Blocked by:** 03 (the offer states the player's purse, so Gold must exist).

**Status:** ready-for-agent (implemented)

- [x] Choosing the south Path presents the Merchant: a greeting, the two items with their prices, and the player's current Gold.
- [x] The player can answer with either a single letter or the whole word, for each of the three outcomes — buy the Armor, buy the Magical Weapon, buy nothing.
- [x] Answers are accepted regardless of capitalisation or surrounding whitespace.
- [x] Anything else — including an empty answer or bare whitespace — re-prompts rather than leaving the Encounter.
- [x] Declining prints a narrative making clear the player's business with the Merchant is over.
- [x] Choosing either item is accepted as valid input but has no effect yet; it is wired up in tickets 05 and 06.
- [x] After the Encounter, the player is returned to the fork and can still travel north or exit.
- [x] A second trip south prints a narrative describing an empty road, and does not present the Merchant again.
- [x] The old south Path narrative is replaced in English, Spanish and French.
- [x] Every new string exists in all three languages; the parity test from ticket 02 passes.
- [x] A test asserts the input validator accepts all six spellings, in mixed case and whitespace-padded.
- [x] A test asserts the validator rejects null, empty, whitespace-only, and an arbitrary junk string.
- [x] A test asserts the Encounter runs without throwing in each of the three languages, matching how the Dragon Encounter is covered today.

## Comments

New `Merchant` module, following the spec's decisions: it takes no `Die`, and it assembles its
narrative as a list of lines returned joined, the convention `Player.Attack` and `Dragon.Attack`
already follow, rather than writing to the Console itself. `GetOffer` states the greeting, both
items at `Merchant.ItemPrice` (75) and the player's current Gold. `ApplyChoice` is the single
seam the spec describes, taking an already-parsed choice; the decline branch is real, while
`armor` and `weapon` are accepted and do nothing, which is what tickets 05 and 06 fill in.

`Game.IsValidMerchantChoice` is a pure validator shaped after the existing `IsValidPathChoice`
and `IsValidCombatChoice`, accepting a letter or the whole word for each of the three outcomes,
trimmed and case-insensitive. It stays separate from `ApplyChoice` because the prompt loop must
tell "re-prompt" from "proceed", which a narrative string cannot express. The accepted
spellings live in one private `ParseMerchantChoice`, which both the validator and the prompt
loop go through, so a spelling can never be accepted by one and unrecognised by the other.

Once per Adventure is a `merchantMet` local scoped to the Adventure loop, tripped when the
Merchant is met rather than when something is bought, so declining consumes the visit. A second
trip south prints `south_path_empty` instead. Scoping it to the loop rather than to a field means
a newly created Character meets him afresh without anything having to reset it.

The old `south_path_narrative` was rewritten in all three languages rather than supplemented: it
told the player nothing happens and they return home poor, which this ticket makes false.

Two things surfaced that the ticket did not anticipate:

- The prompt loop spun forever when input ran out, because `Console.ReadLine` returns null at end
  of input and a null is not a valid answer. It now treats end of input as declining, the outcome
  that leaves the Character untouched. The older prompt loops in the codebase share this flaw;
  they were left alone as out of scope.
- `MSTestSettings.cs` parallelizes by method, and Console redirection is process-global, so the
  Console-driven Encounter tests raced with the existing `DisplayStats` capture test and the
  suite failed intermittently. Every test that captures the Console is now `[DoNotParallelize]`,
  which includes marking that one pre-existing test. The suite then passed five runs in a row.

**Later correction: the method is private and the test exists after all.**
The first whole-branch review flagged that `HandleMerchantEncounter` was public only so a test
could drive it, breaking the spec's requirement that the Merchant follow the conventions the
Dragon Encounter established. The method was made private and the test dropped, on the mistaken
premise that the two were mutually exclusive. They are not: `DragonTests` reaches the private
`HandleDragonEncounter` by reflection, so that — not a public method, and not an untested loop —
is the convention the spec points at. `MerchantEncounterTests` now mirrors it exactly: the method
stays private, the tests invoke it reflectively with canned input, and the class sits in its own
serial phase because Console redirection is process-global.

The tests cover the decline path and the purchase path in all three languages, plus re-prompting
on junk, empty and whitespace-only answers. Verified to bite: removing the offer from the
Encounter turns the decline tests red in all three languages.

The `[DoNotParallelize]` marker stays on the `DisplayStats` test. `DragonTests` also redirects the
Console, so the race it guards against predates this feature and outlives the dropped tests.

Verified by hand in the real game: the offer renders with prices and purse, junk and empty
answers re-prompt, declining ends the business and returns to the fork, a second trip south finds
an empty road, and north still reaches the Dragon afterwards. Full suite 152/152, parity included.
