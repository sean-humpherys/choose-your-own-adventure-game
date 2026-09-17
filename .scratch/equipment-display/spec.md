# Equipment Display at the Fork

Status: ready-for-agent

## Problem Statement

The player can see their Character exactly once: at the prompt straight after creation, before any Equipment has been bought. From the moment they step onto the fork, the Character becomes invisible. A player who travels south and buys Armor or a Healing Potion is told about the purchase in one line of narrative, then returned to the fork with no way to confirm what they now carry, how much Gold is left, or what their Equipment actually does.

That matters because the fork is where the one irreversible decision of the Adventure is made. Taking the north Path always ends the Adventure, whether the fight is won, lost or fled, so the player never sees the fork again afterwards. They must commit to the Dragon without being able to look at the Character they are committing.

The character summary itself also hides information. A slot with nothing in it is left out entirely, so a Character with no Armor shows no Armor line at all. A player cannot tell "I carry no Armor" apart from "this game has no Armor", and nothing on the screen hints that the Merchant sold something they passed up.

## Solution

The fork gains a fourth option, beside north, south and exit: `c` (or `character`) shows the same Character summary offered after creation. Viewing it changes nothing. It can be opened as often as the player likes, it returns to the same fork prompt, and it does not count as meeting the Merchant.

The summary now shows every Equipment slot, whether filled or not. A filled slot shows the piece's name and its number line as today. An empty slot shows a single "none" line in its place, worded to agree with the slot's noun in each language.

While the fork's input handling is being touched, its accepted spellings move into a single parser, following the Merchant prompt's precedent, and running out of input at the fork is treated as exiting rather than looping forever.

## User Stories

1. As a player at the fork, I want to view my Character, so that I can see what I carry before choosing a Path.
2. As a player who has just bought from the Merchant, I want to view my Character back at the fork, so that I can confirm what I bought and what it does.
3. As a player, I want to see my Gold at the fork, so that I know what the purchase cost me.
4. As a player, I want to see my Health Points against my Maximum Health Points at the fork, so that I know the state of the Character I am about to take north.
5. As a player, I want the fork's view to be the same Character summary I saw after creation, so that I only have one screen to learn.
6. As a player, I want to keep the option to view my Character straight after creation, so that the existing flow is unchanged.
7. As a player, I want to open the Character view with `c` or `character`, so that it is answered the same way as every other prompt in the game.
8. As a player, I want my answer accepted regardless of capitalisation or stray spaces, so that I am not punished for typing naturally.
9. As a player, I want the fork prompt to tell me about the new option, so that I can discover it without guessing.
10. As a player who mistypes at the fork, I want the invalid-choice message to list every option including `c`, so that I know what I can type.
11. As a player who mistypes at the fork, I want to be asked again rather than sent down a Path, so that a slip costs nothing.
12. As a player, I want viewing my Character to return me to the same fork prompt, so that looking never costs me anything.
13. As a player, I want to view my Character as many times as I like, so that I can check again after thinking it over.
14. As a player who views my Character before going south, I want the Merchant still to be waiting, so that looking does not use up the visit.
15. As a player who has already met the Merchant, I want viewing my Character not to bring him back, so that the once-per-Adventure rule still holds.
16. As a player who views my Character and then goes north, I want the Dragon Encounter to open exactly as it does today, so that looking changes nothing about the fight.
17. As a player carrying no Armor, I want the summary to say "Armor: none", so that I can tell an empty slot from a missing feature.
18. As a player carrying no Healing Potion, I want the summary to say "Healing Potion: none", so that I remember it was on offer.
19. As a player, I want an empty slot to show a single line with no numbers under it, so that it cannot be mistaken for a piece of Equipment worth zero.
20. As a player with a filled slot, I want its number line kept (maximum Damage, Damage Reduction, or Health Points restored), so that I can see what it does.
21. As a player, I want every Equipment slot, the Weapon included, to follow the same "none" rule, so that the summary is consistent.
22. As a player who bought a Magical Weapon, I want it shown by its translated name without an extra label, so that the summary stays short.
23. As a player straight after creation, I want the "none" lines there too, so that the summary looks the same wherever I open it.
24. As a Spanish-speaking player, I want the whole feature in Spanish, with "none" agreeing with each slot's noun, so that it reads as correct Spanish.
25. As a French-speaking player, I want the whole feature in French, with "none" agreeing with each slot's noun, so that it reads as correct French.
26. As a player in any language, I want never to see a bare message key in place of text, so that the game never looks broken.
27. As a player whose input stream ends at the fork, I want the game to end cleanly, so that it does not spin forever printing "invalid".
28. As a developer, I want the fork's accepted spellings to live in one parser, so that the validator and the prompt can never disagree.
29. As a developer, I want the fork loop reachable by a test with a Character I built myself, so that I can assert its behaviour for any Equipment without scripting character creation.
30. As a developer, I want each slot's "none" to be its own message key, so that a future slot whose noun has a different gender cannot inherit the wrong agreement.
31. As a developer, I want the feature to follow the conventions the Merchant Encounter and Healing Potion already established, so that the codebase stays legible.

## Implementation Decisions

**One view, not two.** The fork shows the existing Character summary, the one the post-creation prompt already shows. A shorter Equipment-and-Gold view was considered and rejected: it would be a second display to keep in step with the first, and the Health Points line is exactly what a player wants to see before committing to the Dragon.

**The fork loop becomes its own method in the Game module.** Today the fork loop is written inline in the game's entry point, alongside language selection, the main menu and character creation, so nothing can reach it without scripting all of those. It moves into a method of its own that takes the Character, the Dragon and the message catalogue, and owns the once-per-Adventure Merchant flag. The entry point creates the Character and calls it. This is the one new seam the feature introduces, and it sits at the same level as the Merchant Encounter method that tests already drive. The move must not change the behaviour of north, south or exit.

**The fork gains a parser.** The fork's accepted spellings are written out twice today, once in the public validator and once inside the prompt loop, which is exactly the drift the Merchant's parser was introduced to prevent. A single parser becomes the one place the spellings live, returning `north`, `south`, `exit`, `character`, or nothing for unrecognised input. The public validator answers off it, and the prompt uses it. The fork loop gains one branch for `character` that prints the Character's summary and goes round again.

**The key is `c` / `character`, in every language.** No letter is naturally shared across English, Spanish and French, and the project already uses English letters throughout (`n`/`s`/`e` at the fork, `w` for the Weapon at the Merchant). The letter follows that. A digit was rejected because the fork already rejects digits, and that is asserted.

**End of input at the fork means exit.** When the input stream ends, the fork prompt today prints the invalid-choice message forever. It now returns `exit`, which is the outcome that changes nothing, mirroring the Merchant prompt treating end of input as declining.

**Viewing changes no state.** The `character` branch touches neither the Merchant flag nor anything on the Character. Only north, south and exit move the Adventure forward.

**Every Equipment slot is always shown.** The Character's summary method shows the Weapon, Armor and Healing Potion slots whether or not they are filled. A filled slot keeps its name line and number line. An empty slot shows its label followed by "none", and no number line: a "Damage Reduction: 0" under an absent Armor would read as a piece of Armor with no effect. This deliberately reverses story 11 of the Healing Potion spec ("no empty Potion line cluttering either display"), for the character summary only.

**The Weapon slot follows the rule too.** In practice a Character always carries a Weapon, because the Occupation is validated before one is assigned. The rule still applies, so the summary has one behaviour for every slot and never silently drops one.

**No Magical label.** The translated name of a piece already says what it is. No flag is added to Weapon or Armor to mark a bought piece as Magical.

**Localisation.** Every new or changed player-facing string is changed in all three languages in the same change. Each slot has its own "none" key, holding only the word, which follows the existing slot label:

| Key | English | Spanish | French |
| --- | --- | --- | --- |
| `stats_weapon_none` | none | ninguna | aucune |
| `stats_armor_none` | none | ninguna | aucune |
| `stats_potion_none` | none | ninguna | aucune |

All three nouns happen to be feminine in both Spanish (*el arma*, *la armadura*, *la pocion*) and French (*l'arme*, *l'armure*, *la potion*), so one shared key would work today. It is rejected because it would only be right by coincidence, and gendered agreement has already had to be fixed once in this project. Spellings follow the catalogue's existing unaccented convention.

The fork prompt gains a closing sentence and the invalid-choice message lists the new letter:

| Key | English | Spanish | French |
| --- | --- | --- | --- |
| `path_prompt_full` (appended) | Enter 'c' to view your character. | Escribe 'c' para ver tu personaje. | Tapez 'c' pour voir votre personnage. |
| `path_invalid` | Invalid choice. Please enter 'n', 's', 'e', or 'c'. | Opcion invalida. Por favor escribe 'n', 's', 'e' o 'c'. | Choix invalide. Veuillez taper 'n', 's', 'e' ou 'c'. |

The new sentence comes last, because viewing the Character is not a Path.

## Testing Decisions

**What makes a good test here.** Assert what the player or the caller can observe: the text printed or returned, and the Character and Adventure state afterwards. Do not assert on private fields, the order of internal calls, or the exact layout of lines beyond what the player reads. Because the Die cannot be seeded, no test may depend on a roll; nothing in this feature needs one, and no test goes north.

**Seam 1: the fork loop.** Driven with scripted input and a Character built directly in the test, so its Equipment can be set without the Merchant. It covers:
- `c` prints the Character summary and then the fork prompt again.
- `c`, `s`, then declining reaches the Merchant's offer, so viewing did not use up the visit.
- `s`, declining, `c`, `s` shows the empty-clearing narrative, so viewing did not bring the Merchant back.
- `c` twice then `e` prints the summary twice and ends the Adventure.
- A junk answer prints the invalid-choice message, which lists `c`, and asks again.
- An empty input stream returns promptly and ends the Adventure instead of looping.
- Each of these runs in English, Spanish and French.

**Seam 2: the Character summary.** The existing method that returns the summary as a string. In each of the three languages:
- A Character with no Armor shows the Armor label followed by that language's Armor "none", and no Damage Reduction line.
- A Character with no Potion shows the Potion label followed by its "none", and no restore line.
- A Character with no Weapon shows the Weapon label followed by its "none", and no maximum Damage line.
- A Character with each piece shows its translated name and number line exactly as before.
- No raw message key appears.

The existing test asserting that no Armor lines appear when none is worn encodes the old behaviour. It is rewritten to assert the "none" line, not deleted.

**The fork's spellings.** The existing public validator is extended rather than a new test seam added. `c`, `character`, uppercase and padded forms are accepted. Every existing north, south and exit spelling is still accepted. A digit, null, empty, whitespace-only and a junk string are still rejected.

**Language parity.** The existing parity test already asserts that every language has the same keys. It needs no change, and it is what catches a "none" key added to one language and forgotten in another.

**Prior art.** The Merchant Encounter tests drive a prompt method with scripted console input and a hand-built Character, asserting on captured output in each language; follow that for the fork loop. The Armor and Player tests assert on the summary's returned string; follow that for the "none" lines. The Game tests exercise the fork validator with valid, invalid, empty, null, mixed-case and padded input; extend them in place.

## Out of Scope

- **The combat stats display.** It keeps leaving out empty slots, so it and the summary disagree after this change. That inconsistency is recorded as a separate follow-up issue awaiting triage.
- **Viewing during combat, at the Merchant, or at the pre-combat prompt.** The summary is reachable after creation and at the fork only. Viewing in combat would raise whether it costs the round, which ADR-0002 makes a real question.
- **A separate Equipment screen.** One summary serves both places.
- **A Magical label or flag** on Weapon or Armor.
- **Inventory.** The Character still carries one Weapon, at most one Armor and at most one Healing Potion. Nothing here adds carrying capacity.
- **Tidying unused message keys.** The old Health Points label appears unused since the maximum was added, but removing it is unrelated.
- **Making the Die injectable.** No part of this feature depends on a roll.
- **Additional languages.** English, Spanish and French only.
- **New Paths or Encounters.** The fork still leads north and south. The new option is a view, not a Path.

## Further Notes

The vocabulary in this spec is defined in `CONTEXT.md`. **Equipment** was added alongside this design: the Weapon, the Armor and the Healing Potion a Character carries, with Gold explicitly excluded and "inventory" and "items" on the avoid list. Call the feature the Equipment display, and use Equipment rather than inventory in code, tests and player-facing text.

No ADR was written. Showing the summary at the fork is cheap to reverse, unsurprising to a future reader, and involved no real trade-off, so it does not meet the bar ADR-0001 and ADR-0002 cleared. Neither existing ADR is touched: nothing here changes Damage, Damage Reduction or healing.

Taking the north Path always ends the Adventure, even on retreat. That is why an empty Potion slot at the fork only ever means the Potion was never bought, never that it was drunk, and why the summary needs no "used" state.
