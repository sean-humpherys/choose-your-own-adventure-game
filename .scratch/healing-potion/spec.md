# Healing Potion from the Merchant

Status: ready-for-agent

## Problem Statement

Once the Dragon fight opens, the Character the player walks in with is the Character they must win with. Health Points only ever fall. There is no decision left to make about survival — the player picks attack or retreat, and everything else is decided by rolls they cannot influence.

That makes a bad opening roll unrecoverable. A Character who rolls low Health Points at creation is very likely dead in two landed blows, and nothing bought on the south Path changes that: Armor reduces each blow, but it cannot give back what has already been taken. The player watches the number fall and has no move to make.

The Merchant compounds this. He sells two permanent upgrades, both of which pay off gradually across the fight, and the leftover Gold in the purse has no use at all. A player who wants insurance rather than a slow advantage has nothing to spend on.

## Solution

The Merchant now stocks a third ware: a Healing Potion, priced the same as the Armor and the Magical Weapon. The purse still affords exactly one thing, so the Potion competes with the permanent upgrades rather than supplementing them — the player chooses between hitting harder, taking less, and getting one reprieve.

A Character carries at most one Healing Potion, and may drink it during the Dragon fight. Drinking restores a flat amount of Health Points, never above the Maximum Health Points recorded at creation, and consumes the Potion. It also consumes the round: the Character drinks instead of attacking, and the Dragon still swings. The player therefore has a real decision about *when* — too early and the cap wastes most of it, too late and the blow that kills them lands first.

To make that decision possible, both stats displays now show the Character's Health Points against their maximum, so the player can see how much of a Potion would actually be used before they spend a round on it.

## User Stories

1. As a player, I want the Merchant to sell a Healing Potion, so that my Gold can buy insurance rather than only a permanent upgrade.
2. As a player, I want the Healing Potion offered alongside the Armor and the Magical Weapon, so that I can see all my options before deciding.
3. As a player, I want the Healing Potion to cost the same as the other wares, so that my choice is about what suits me rather than about what I can afford.
4. As a player, I want my Gold reduced by the price when I buy a Healing Potion, so that the purse behaves like money.
5. As a player, I want to be unable to buy a second ware after buying a Healing Potion, so that the three-way choice is a real one.
6. As a player, I want buying a Healing Potion to consume my one visit to the Merchant, so that the Encounter works the same way whatever I buy.
7. As a player, I want my purchase of a Healing Potion confirmed in the narrative, so that I know the transaction succeeded.
8. As a player, I want to decline the Merchant and still have the Potion have been a genuine option, so that declining remains a deliberate choice rather than the only interesting one.
9. As a player carrying a Healing Potion, I want it shown on my character sheet, so that I can confirm what I bought.
10. As a player carrying a Healing Potion, I want it shown when the Dragon fight opens, so that I remember I have it when it matters.
11. As a player carrying no Healing Potion, I want no empty Potion line cluttering either display, so that the displays stay readable.
12. As a player, I want to know how much a Healing Potion restores before I buy it, so that I can weigh it against the Armor's Damage Reduction.
13. As a player, I want my Maximum Health Points recorded when my Character is created, so that healing has a ceiling that reflects the Character I rolled.
14. As a player, I want my Health Points shown against my Maximum Health Points on my character sheet, so that I can see how much I have lost.
15. As a player, I want my Health Points shown against my Maximum Health Points when the Dragon fight opens, so that I can judge whether a Potion would be wasted.
16. As a player, I want the Dragon's Health Points shown as they are today, without a maximum, so that the display does not claim the Dragon has a ceiling it does not have.
17. As a player in combat, I want to drink my Healing Potion as an alternative to attacking, so that I have a move to make when I am losing.
18. As a player, I want drinking to restore a predictable amount of Health Points, so that I can decide whether it will save me.
19. As a player, I want drinking to never raise my Health Points above my Maximum Health Points, so that the Potion cannot make me stronger than the Character I rolled.
20. As a player who drinks while lightly wounded, I want the restore capped at what I have actually lost, so that the game does not pretend to give me more than it did.
21. As a player, I want drinking to consume the Healing Potion, so that it is a one-time reprieve rather than a repeatable heal.
22. As a player, I want drinking to cost me my attack for that round, so that healing is a tactical decision rather than a free action.
23. As a player who drinks, I want the Dragon to still take its turn, so that the fight does not pause while I recover.
24. As a player, I want the amount restored stated in the narrative, so that I can see what the Potion did.
25. As a player, I want my Health Points after drinking stated in the narrative, so that I know where I stand before the Dragon swings.
26. As a player carrying no Healing Potion, I want the drink option absent from the combat prompt, so that I am not offered something I cannot do.
27. As a player carrying no Healing Potion who types the drink option anyway, I want to be re-prompted rather than losing my turn, so that a mistyped choice is harmless.
28. As a player already at my Maximum Health Points, I want drinking refused and my Potion kept, so that I cannot waste it on nothing.
29. As a player refused a drink at full Health Points, I want to be told why and re-prompted, so that I understand what happened and can choose again.
30. As a player, I want the drink option offered only once the fight has begun, so that I am not invited to waste the Potion before anything has hit me.
31. As a player, I want the pre-combat prompt to keep offering only attack and retreat, so that the Encounter opens exactly as it does today.
32. As a player, I want to answer the combat prompt with either a single letter or the whole word, so that the prompt behaves like every other prompt in the game.
33. As a player, I want my answer accepted regardless of capitalisation or stray spaces, so that I am not punished for typing naturally.
34. As a player, I want a mistyped answer at the combat prompt to re-prompt me, so that a slip does not cost me a round.
35. As a player, I want to answer the Merchant with either a single letter or the whole word for the Potion, so that the new ware is bought the same way as the others.
36. As a player, I want the existing attack and retreat options unchanged, so that adding a third option does not alter how I already play.
37. As a player, I want retreating while carrying a Healing Potion to work exactly as it does today, so that the new ware does not change the exit.
38. As a Spanish-speaking player, I want the whole feature in Spanish, so that it is as usable to me as to an English speaker.
39. As a French-speaking player, I want the whole feature in French, so that no part of the game falls back to another language.
40. As a player in any language, I want the Healing Potion named in my language wherever it is displayed, so that my stats do not mix languages.
41. As a player in any language, I want never to see a bare message key in place of text, so that the game never looks broken.
42. As a developer, I want the drinking logic behind a seam that returns a narrative, so that healing arithmetic can be asserted without driving the Console.
43. As a developer, I want the restore and the clamp to be deterministic, so that they can be tested despite the Die being non-injectable.
44. As a developer, I want the Potion purchase to go through the Merchant's existing purchase seam, so that no second purchase path exists to diverge from the first.
45. As a developer, I want the combat prompt's accepted spellings validated by the existing pure validator, so that a spelling cannot be accepted by one place and unrecognised by another.
46. As a developer, I want a test that fails when a message key is missing from any language, so that a half-translated change cannot ship green.
47. As a developer, I want the feature to follow the conventions the Merchant Encounter already established, so that the codebase stays legible.

## Implementation Decisions

**Maximum Health Points is a new quantity on the Character.** It is recorded at creation, equal to the Health Points rolled there, and never changes during the Adventure. It exists only on the Character; the Dragon has no ceiling and gains no such property, which `CONTEXT.md` states explicitly. Recording it at creation rather than deriving it later is what makes the cap meaningful: a Halfling's negative Strength modifier already lowers the rolled Health Points, and the ceiling must reflect the Character actually rolled rather than an idealised maximum.

**New module: Potion.** A small class mirroring `Weapon` and `Armor` — a type name for display, the Health Points it restores, and ASCII art. A Character carries at most one, held in a single nullable slot exactly as Armor is. This is a class rather than a bare integer for the same reason `Armor` is one: the type name is needed for display and translation regardless, so an integer would save nothing and split one concept across two fields.

**The restore is flat, not rolled.** The Potion restores a fixed number of Health Points. A rolled restore was rejected: `Die` is constructed privately inside the Character and cannot be seeded from a test, so a rolled amount would be untestable by construction. ADR-0001 made the same trade for Damage Reduction and for the same reason. This also keeps the amount legible to the player, which matters because they must decide whether it is enough to survive the next blow.

**The restore is capped.** Drinking raises Health Points by the flat amount or to Maximum Health Points, whichever is lower. The cap is what stops the Potion from being a way to exceed the Character rolled at creation, and it is what makes the timing of the drink a decision rather than a formality. Recorded in ADR-0002 along with the rejected alternative of uncapped healing.

**Drinking consumes the round.** The Character drinks instead of attacking, and the Dragon still takes its turn. A free drink was considered and rejected: it makes the Potion strictly dominant and the fight close to unloseable while one is held, which is the failure mode ADR-0001 already identified for over-generous Damage Reduction. Also recorded in ADR-0002.

**The combat loop is restructured, not merely extended.** Today the Character's attack fires at the top of the loop and the prompt sits at the bottom, so the first attack happens before any choice is offered and each prompt governs the following pass. Drinking cannot therefore be a third branch beside attack and retreat — the player's choice must gate which action the Character takes within the round, while leaving the Dragon's turn and both defeat checks where they are. This is the only place the feature touches existing control flow and is where a regression would hide; the existing behaviour for attack and retreat must be preserved exactly.

**The Character's single drinking seam.** One method on the Character takes the message catalogue, applies the restore, clamps at Maximum Health Points, clears the Potion slot, and returns the narrative. It owns the no-Potion case and the already-full case as well, returning the appropriate narrative and leaving the Character untouched in both. Combat calls it and prints the result, which keeps Combat as the thing that prompts and prints — all it does today. The seam mirrors both `ReduceDamage`, which is pure and assertable, and the existing attack methods, which return their narrative as a joined list of lines rather than writing to the Console.

**Refusals keep the Potion.** Drinking at Maximum Health Points is refused rather than allowed and wasted, and the prompt re-prompts. Silently consuming a ware worth the entire purse for no effect reads as a defect. Likewise the drink option is omitted from the prompt text entirely when no Potion is carried, following the Merchant's precedent of not offering a Magical Weapon to a Character with none to better.

**The Potion is sold through the existing purchase seam.** The Merchant's single seam gains a case for the Potion; it already owns affordability, the Gold deduction and the purchase narrative, and no second purchase path is introduced. The Potion is priced identically to the Armor and the Magical Weapon, so the purse continues to afford exactly one ware and the `Gold` glossary entry stays true as written. The once-per-Adventure flag is untouched: it trips when the Merchant is met, so buying a Potion consumes the visit exactly as buying Armor does.

**Input validation follows the existing validators.** The Merchant's parser gains one accepted spelling pair for the Potion, and the combat validator gains one for drinking, each accepting a single letter and the full word, trimmed and case-insensitive. Both remain pure and separate from the action methods, because the prompt loops must distinguish "re-prompt" from "proceed", which a narrative string cannot express. The combat prompt's parsing continues to live in one place so that a spelling cannot be accepted by the validator and unrecognised by the loop.

**Health Points are displayed against the maximum in the stats displays only.** Both the character sheet and the combat stats display show the Character's Health Points together with the Maximum Health Points. The Dragon's health line in the combat display is unchanged, since the Dragon has no maximum — this requires the Character's health line to be rendered separately from the Dragon's rather than through the single shared message key both use today. The blow-by-blow combat narration keeps its current form and is not changed to include the maximum; the cap is something the player consults before acting, not something to repeat after every swing.

**Localisation.** Every new player-facing string is added to all three language dictionaries in the same change. A display map for the Potion is introduced parallel to the existing weapon and armor maps, rather than a single message key, so that the Potion is named through the same mechanism as the other wares and is covered by the same parity test. The Merchant's offer text is extended in all three languages to list the Potion and state what it restores.

## Testing Decisions

**What makes a good test here.** Assert what the player or the caller can observe: the narrative returned, and the Character's Health Points, Maximum Health Points, Gold, Potion, Weapon and Armor after an action. Do not assert on the order of internal calls, on private fields, or on the shape of intermediate lists. Prefer driving a seam that returns a value over capturing Console output. Because `Die` is constructed internally and cannot be seeded, no test may depend on a roll's outcome — every assertion below is over arithmetic or validation that is deterministic by construction, which is precisely why the restore is flat.

**Prior art.** The pure validators in the Game module are tested directly with valid, invalid, empty and null inputs; follow that for the new spellings. `ReduceDamage` is tested as pure arithmetic across its boundaries; follow that for the restore and its clamp. Both stats displays are exercised through their returned strings; follow that for the new lines. The message catalogue tests instantiate the catalogue per language and read it; follow that for the parity test. The Console-driven Encounters are covered only by tests asserting they do not throw; the restructured combat loop gets the same treatment and no more.

**Drinking, through the Character's seam.** A Character wounded by more than the restore gains exactly the restore. A Character wounded by exactly the restore returns to Maximum Health Points. A Character wounded by less than the restore returns to Maximum Health Points and never exceeds it. Drinking clears the Potion slot in every case where it succeeds. Drinking at Maximum Health Points leaves Health Points unchanged and leaves the Potion in place. Drinking with no Potion leaves Health Points unchanged. The narrative differs between a successful drink, a refusal at full health, and having no Potion, so the player can tell the three apart.

**Maximum Health Points.** It is set at creation and equals the Health Points the Character starts with. Damage lowers Health Points and leaves the maximum untouched. Nothing in the feature raises the maximum.

**Purchases, through the Merchant's seam.** Buying the Potion sets the Potion, reduces Gold by the price, and leaves the Weapon and Armor untouched. Buying Armor or the Magical Weapon leaves the Potion unset. Declining changes nothing, including the Potion. A second purchase attempt with the depleted purse is refused, including at the exact boundary where the remaining Gold is one short of the price.

**Input validation.** Every accepted spelling for drinking and for buying the Potion — the letter and the full word — is accepted, including in mixed case and padded with whitespace. Null, empty, whitespace-only and an arbitrary junk string are all rejected. The existing attack, retreat, armor, weapon and decline spellings continue to be accepted unchanged.

**Stats displays.** The character sheet's returned string shows the Character's Health Points against the maximum, and shows the Potion line only when one is carried. The combat stats display does likewise for the Character, and continues to show the Dragon's health without a maximum. The refactor changes neither display's existing Weapon, Armor or Gold content.

**Language parity.** The existing parity test is extended to cover the new display map, so that all three languages expose an identical set of dictionary keys and an identical set of entries in every display map. This remains the highest-value test in the change: a missing key does not throw, it renders as the literal key name, so without this test a half-translated change passes every other test.

**The combat loop.** Covered by tests asserting it does not throw, in each of the three languages, matching how the Encounters are covered today. Its logic lives behind the seams above, which are asserted properly. The existing combat tests must continue to pass unchanged, which is the guard against the restructuring altering attack or retreat.

## Out of Scope

- **Finding Potions.** The Potion is bought from the Merchant and acquired no other way. No loot, no new Encounter, no Potion on the north Path.
- **Inventory.** A Character carries at most one Potion in a single slot. No stockpiling, no counts, no carrying a Potion alongside a second one bought later.
- **A second Merchant visit.** The once-per-Adventure rule is unchanged, and buying a Potion consumes the visit. The Potion competes with the Armor and the Magical Weapon; it does not supplement them.
- **Drinking outside combat.** No drinking at the fork, at the Merchant, at the pre-combat prompt, or after the fight resolves.
- **Healing the Dragon.** The Potion applies only to the Character.
- **Rebalancing the Dragon.** The Dragon's statistics are untouched. The Adventure gets a little more survivable, and that is accepted here.
- **Making the Die injectable.** The randomness remains untestable. This feature works around it by keeping its own arithmetic flat and pure, exactly as the Armor work did.
- **Persistence.** Nothing is saved between Adventures. An undrunk Potion is simply gone when the Adventure ends.
- **Additional languages.** English, Spanish and French only.
- **New Paths or Encounters.** The fork still offers north, south and exit.

## Further Notes

The vocabulary in this spec is defined in `CONTEXT.md` at the repo root, which was extended alongside this design: **Healing Potion** and **Maximum Health Points** are new terms, **Merchant** and **Magical** were rewritten because the Merchant no longer sells equipment exclusively, and **Gold** was deliberately left as it stands because pricing the Potion identically keeps it true. Use those terms in code, in tests and in player-facing text — in particular Healing Potion rather than elixir or cure, and Maximum Health Points rather than full health.

ADR-0001 records why Armor subtracts a flat amount rather than raising Agility, and its closing note is binding here: any future source of Damage must route through the Character's reduction step. This feature adds a source of *healing* rather than Damage, so it does not route through that step, but it must not bypass the Maximum Health Points clamp for the same class of reason — a second way to change Health Points that skips the ceiling would silently break the cap.

ADR-0002 is to be written with this feature. It records healing as a capped, flat restore that consumes the round, and lists the rejected alternatives: uncapped healing, a rolled restore, and drinking as a free action. It needs a calibration note in ADR-0001's style, because the balance here is deliberately fine: the restore is set close to what Armor mitigates across a typical fight, which produces an intended optimal play — hold the Potion until well wounded, because the cap punishes drinking early and the round cost punishes drinking while safe. A future reader should recognise that as the design rather than a flaw to correct.

The concrete numbers — the amount restored and the Potion's price — are tuning rather than architecture and may be adjusted without revisiting this spec, provided the price continues to let the purse afford exactly one ware and the restore stays large enough to be worth a lost round.
