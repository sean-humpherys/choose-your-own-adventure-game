# Merchant Encounter on the South Path

Status: ready-for-agent

## Problem Statement

The south Path is a dead end. Choosing it prints a single line telling the player that nothing adventurous happens and that they return home poor and bored, then drops them back at the fork. Nothing about the Character changes, so the only meaningful decision in the Adventure is whether to fight the Dragon at all.

That leaves the player with no way to prepare. The Character they rolled at creation is the Character they take into combat, and the fight is decided almost entirely by the Attack Roll and Defense Roll landing against statistics the player had no hand in choosing. The south Path advertises a choice that carries no consequence.

## Solution

The south Path now holds an Encounter. The player meets a Merchant who sells exactly one piece of Magical equipment: Armor that grants Damage Reduction, or a Magical Weapon that deals more Damage than the one their Occupation gave them.

The Character carries a fixed purse of Gold from creation, sized to afford one item and not two, so the player must choose between surviving longer and hitting harder. They may also decline and keep their Gold, which buys them nothing. The Merchant is met once per Adventure, so the choice is final: the player then returns to the fork and may take the north Path to the Dragon, better equipped than before.

## User Stories

1. As a player, I want the south Path to contain an Encounter, so that both directions at the fork are worth exploring.
2. As a player, I want to meet a Merchant when I travel south, so that the Path has a reason to exist in the story.
3. As a player, I want to be told what the Merchant is selling and what each item costs, so that I can make an informed choice.
4. As a player, I want to see how much Gold I am carrying before I decide, so that I know what I can afford.
5. As a player, I want to see my Gold on my character sheet, so that I learn my purse exists before I reach the Merchant.
6. As a player, I want to buy Armor, so that the Dragon's blows take fewer Health Points from me.
7. As a player, I want to buy a Magical Weapon, so that I deal more Damage to the Dragon when I land a blow.
8. As a player, I want the Magical Weapon to suit my Occupation, so that my Magician is not handed a sword and my Fighter is not handed a spell.
9. As a player, I want the Magical Weapon to be a clear improvement on the one my Occupation gave me, so that spending my Gold feels worthwhile.
10. As a Thief with a weak starting Weapon, I want my Magical Weapon to improve on my dagger by the same margin a Fighter's improves on their long sword, so that the Occupation I chose is not erased by one purchase.
11. As a player, I want to decline the Merchant's offer, so that I can attempt the Dragon with the Character I rolled.
12. As a player, I want to be told plainly that declining ends my business with the Merchant, so that I am not surprised when he is gone.
13. As a player, I want my purchase confirmed in the narrative, so that I know the transaction succeeded.
14. As a player, I want my Gold reduced by the price when I buy, so that the purse behaves like money.
15. As a player, I want to be unable to buy a second item after my first purchase, so that the choice between Armor and a Magical Weapon is a real one.
16. As a player, I want the Magical Weapon to replace the Weapon I was carrying, so that I am never asked which Weapon to attack with.
17. As a player, I want my Armor and its Damage Reduction shown on my character sheet, so that I can confirm what I bought.
18. As a player, I want my Armor and its Damage Reduction shown when the Dragon fight opens, so that I can judge my chances before committing.
19. As a player, I want my new Magical Weapon and its Damage shown in both stats displays, so that the upgrade is visible where the old Weapon was.
20. As a player with no Armor, I want no empty Armor line cluttering my stats, so that the display stays readable.
21. As a player wearing Armor, I want a flat amount subtracted from each blow the Dragon lands, so that Armor protects me consistently rather than occasionally.
22. As a player wearing Armor, I want a blow reduced to nothing to be narrated as my Armor absorbing it, so that I can tell it apart from dodging.
23. As a player wearing Armor, I want a reduced blow to never restore Health Points, so that Armor cannot heal me.
24. As a player, I want Armor to leave the Attack Roll and Defense Roll untouched, so that wearing it does not make me harder to hit or quicker to dodge.
25. As a player, I want to visit the Merchant once per Adventure, so that I cannot shop repeatedly and trivialise the Dragon.
26. As a player who returns south after meeting the Merchant, I want a narrative explaining that the road is empty now, so that the Path does not silently repeat itself.
27. As a player, I want to return to the fork after the Encounter, so that I can still choose the north Path and fight the Dragon.
28. As a player, I want to be able to exit the game from the fork after shopping, so that the Encounter does not trap me in the Adventure.
29. As a player, I want a mistyped answer at the Merchant to re-prompt me, so that a slip does not cost me the Encounter.
30. As a player, I want to answer the Merchant with either a single letter or the whole word, so that the prompt behaves like every other prompt in the game.
31. As a player, I want my answer accepted regardless of capitalisation or stray spaces, so that I am not punished for typing naturally.
32. As a player who submits nothing at all, I want to be re-prompted rather than dropped, so that an accidental return key is harmless.
33. As a Spanish-speaking player, I want the whole Encounter in Spanish, so that the feature is as usable to me as to an English speaker.
34. As a French-speaking player, I want the whole Encounter in French, so that no part of the game falls back to another language.
35. As a player in any language, I want the Magical equipment named in my language wherever it is displayed, so that my stats do not mix languages.
36. As a player in any language, I want never to see a bare message key in place of text, so that the game never looks broken.
37. As a developer, I want the Merchant's purchase logic to be assertable without driving the Console, so that Gold, price, and Damage arithmetic are covered by real tests.
38. As a developer, I want the armor arithmetic to be a pure function, so that it can be tested despite the Die being non-injectable.
39. As a developer, I want both stats displays to return strings, so that what the player sees can be asserted rather than eyeballed.
40. As a developer, I want a test that fails when a message key is missing from any language, so that a half-translated change cannot ship green.
41. As a developer, I want the Merchant to follow the conventions the Dragon Encounter already established, so that the codebase stays legible.

## Implementation Decisions

**Damage Reduction, not protection.** Armor grants Damage Reduction: a flat value subtracted from incoming Damage, floored at zero. This is recorded in ADR-0001, which rejects the cheaper alternative of raising Agility. Agility is defined in `CONTEXT.md` as evasion only, and nothing in this feature may change the Attack Roll or the Defense Roll. Player-facing text uses the term Damage Reduction; "protection" is listed under `_Avoid_` in the glossary.

**New module: Armor.** A small class mirroring `Weapon` — a type name, a Damage Reduction value, and ASCII art. A Character carries at most one. Chosen over a bare integer on the Character because the type name is needed for display and translation regardless, so the integer saves nothing and splits one concept across two fields.

**New module: Merchant.** Holds the Encounter. It takes no `Die`: nothing about the Merchant is random, and the Encounter is fully deterministic. Its narrative is assembled as a list of lines and returned joined, the convention `Player.Attack` and `Dragon.Attack` already follow, rather than written to the Console from inside.

**The Merchant's single seam.** One method takes the Character, an already-parsed choice, and the message catalogue; applies the purchase; and returns the narrative. It owns affordability, the Gold deduction, replacing the Weapon, fitting the Armor, the missing-Weapon case, and the decline. Affordability is deliberately not exposed as a separate public surface — the exact-price boundary is asserted through this method instead, keeping the seam count at the highest point possible.

**Input validation.** A pure validator, named and shaped after the existing `IsValidPathChoice` and `IsValidCombatChoice`. It accepts a single letter or the full word for each of the three outcomes — buy Armor, buy the Magical Weapon, buy nothing — trimmed and case-insensitive, and rejects everything else so the prompt loop re-prompts. It stays separate from the purchase method because the loop must distinguish "re-prompt" from "proceed", which a narrative string cannot express.

**Gold.** A new property on the Character, set to a fixed amount at creation rather than rolled. A rolled purse would let variance produce Adventures in which nothing is affordable, turning the Encounter into a no-op that would need its own narrative. Both items are priced identically at an amount the purse covers once and not twice, so the decision is about fit rather than budget, and the leftover Gold has no further use by design.

**Magical Weapon damage is relative, not flat.** Starting Weapon Damage is uneven across Occupations by design at creation. The Magical Weapon's maximum Damage is the replaced Weapon's plus a fixed increment, preserving that ordering; a flat value for all Occupations would hand the weakest Occupation a far larger gain than the strongest. Each Occupation gets its own Magical Weapon name so the fiction holds.

**Weapon replacement.** Buying the Magical Weapon overwrites the single Weapon slot. No inventory is introduced and no Weapon-selection prompt is added to combat.

**Missing Weapon.** The Character's Weapon is nullable and the Occupation-to-Weapon assignment has no default branch, so the type permits a Character with no Weapon even though validation makes it unreachable in practice. A relative Damage increment needs a base, so the Merchant offers only the Armor in that case. This matches the existing guard in the Character's attack path rather than assuming validation elsewhere holds.

**Applying Damage Reduction.** The Character exposes a pure method taking incoming Damage and returning the reduced amount, floored at zero. The Dragon's attack calls it instead of subtracting rolled Damage from Health Points directly. Any future Damage source must route through the same method or Armor will silently stop working for it, as noted in ADR-0001.

**Absorbed blows.** A blow reduced to zero gets its own message key. It must not reuse the existing dodge message: the Character did not evade, the Armor stopped it, and conflating the two would misrepresent the mechanic to the player.

**Once per Adventure.** A single boolean on the Encounter's owner, tripped when the Merchant is *met*, not when a purchase is made. Declining therefore consumes the visit, which makes the decision weighty and needs no further state. A later trip south prints a different narrative describing an empty road.

**Both stats displays return strings.** The combat stats display already returns a string; the character sheet writes directly to the Console. The character sheet is refactored to build and return a string, with the printing method reduced to writing it out — mirroring the combat display exactly. This is a refactor of existing behaviour with no change to what the player sees, and it makes the new Gold and Armor lines assertable. Armor and Weapon lines remain conditional on ownership, following the existing guard.

**Localisation.** Every new player-facing string is added to all three language dictionaries in the same change. The Magical Weapon names are added to each language's weapon display map, and a parallel display map is introduced for Armor. The existing south Path narrative is rewritten in all three languages rather than supplemented: its current text states that nothing happens and that the player returns home poor, which this feature makes false.

## Testing Decisions

**What makes a good test here.** Assert what the player or the caller can observe: the narrative returned, the Character's Gold, Weapon, Armor, and Health Points after an action, and whether an input is accepted. Do not assert on the order of internal calls, on private fields, or on the shape of intermediate lists. Prefer driving a seam that returns a value over capturing Console output. Because `Die` is constructed internally and cannot be seeded, no test may depend on a roll's outcome — every assertion below is over arithmetic or validation that is deterministic by construction.

**Prior art.** The pure validators in the Game module are tested directly with valid, invalid, empty, and null inputs; follow that pattern for the Merchant's validator. The combat stats display is exercised through its returned string; follow that for both stats displays. The message catalogue tests instantiate the catalogue per language and read it; follow that for the parity test. The Console-driven Dragon Encounter is covered only by tests asserting it does not throw; the Merchant's prompt loop gets the same treatment and no more.

**Damage Reduction.** A blow larger than the reduction is reduced by exactly the reduction. A blow equal to the reduction becomes zero. A blow smaller than the reduction becomes zero and never negative. With no Armor, the blow passes through unchanged. Health Points after an absorbed blow are unchanged, and never increase.

**Purchases, through the Merchant's seam.** Buying Armor sets the Armor, reduces Gold by the price, and leaves the Weapon untouched. Buying the Magical Weapon replaces the Weapon, sets its maximum Damage to the old maximum plus the increment, reduces Gold, and leaves Armor unset. Declining changes nothing — not Gold, not Weapon, not Armor. A second purchase attempt with the depleted purse is refused, including at the exact boundary where the remaining Gold is one short of the price. Each of the four Occupations yields the correct Magical Weapon name and Damage value. A Character with no Weapon is offered the Armor and not the Magical Weapon.

**Input validation.** Every accepted spelling — the letter and the full word for each of the three outcomes — is accepted, including in mixed case and padded with whitespace. Null, empty, whitespace-only, and an arbitrary junk string are all rejected.

**Stats displays.** The character sheet's returned string contains the Gold line always, and the Armor and Damage Reduction lines only when Armor is owned. The combat stats display likewise gains the Armor lines only when Armor is owned. Both continue to show the Weapon lines as before, and the refactor changes neither display's existing content.

**Language parity.** One test asserting that all three languages expose an identical set of dictionary keys, and likewise for the weapon and armor display maps. This is the highest-value test in the change: a missing key does not throw, it renders as the literal key name in brackets, so without this test a half-translated change passes every other test. It is written against the catalogue generally, not against this feature's keys, so it pays off for every string added later.

**The Encounter flow.** The prompt loop itself is covered only by tests asserting it does not throw, in each of the three languages, matching how the Dragon Encounter is covered today. Its logic lives behind the seams above, which are asserted properly.

## Out of Scope

- **Inventory.** The Character carries one Weapon and at most one Armor. No multi-item carrying, no equip or unequip, no Weapon choice during combat.
- **Armor for the Dragon.** Damage Reduction applies only to the Character. The Dragon's Health Points continue to take rolled Damage directly.
- **Spending leftover Gold.** The remaining Gold after a purchase has no use. No second shop, no healing, no haggling, no selling the replaced Weapon back.
- **Making the Die injectable.** The randomness remains untestable. This feature works around it by keeping its own arithmetic pure rather than refactoring combat's dependencies.
- **Rebalancing the Dragon.** The Dragon's Health Points remain a single twenty-sided roll, so a Magical Weapon will often end the fight in two landed blows. The Adventure gets easier, and that is accepted here; changing the Dragon's statistics is a separate decision.
- **New Paths or Encounters.** The fork still offers north, south, and exit.
- **Additional languages.** English, Spanish, and French only.
- **Persistence.** Nothing is saved between Adventures; Gold and equipment do not carry over.

## Further Notes

The vocabulary in this spec is defined in `CONTEXT.md` at the repo root, which was written alongside the design. Use those terms in code, in tests, and in player-facing text — in particular Damage Reduction rather than protection, Gold rather than coins or money, and Merchant rather than shop or trader.

ADR-0001 records why Armor subtracts a flat amount instead of raising Agility, and lists the rejected alternatives. Read it before changing anything about how Armor interacts with combat; its calibration note explains why the Damage Reduction value is what it is relative to the Dragon's Damage and the Character's Health Points.

The concrete numbers — purse size, item price, Damage Reduction value, and the Magical Weapon's Damage increment — were each settled deliberately but are one-line changes. They are tuning, not architecture, and may be adjusted without revisiting this spec, provided the purse continues to afford exactly one item and the Damage increment stays uniform across Occupations.
