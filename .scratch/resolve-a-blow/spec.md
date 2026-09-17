# Resolve a Blow in One Place

Status: ready-for-agent

## Problem Statement

A Blow, meaning one attacker's Attack Roll, Defense Roll and Damage against one target, is written out twice. The Character's attack on the Dragon and the Dragon's attack on the Character each roll, compare, subtract and narrate in their own method. A developer who wants to know how a Blow works has to read both, plus the Combat round, and a change to the rules has to be made twice and kept in step by hand.

The two copies have already drifted apart. The Character floors its Health Points at zero, and the Dragon does not. The Character's attack also has one English sentence written into the code instead of taken from the message catalogue. Some differences are correct: only the Character wears Armor, so only the Character's Blow passes through Damage Reduction, and each side's narration is its own.

ADR-0001 requires every source of Damage aimed at the Character to go through its reduction step. Today that holds only because the Dragon's copy remembers to call it. A future attacker written the same way could skip it, and nothing would catch it.

Nothing can check either copy precisely. The Die can't be seeded, and the Character builds its own privately, so tests can only assert loose bounds, such as "Health Points did not rise". The Character's attack on the Dragon has no test at all.

## Solution

This is a behaviour-preserving refactor. A player sees no difference in any language, in any roll, or in any line of narration.

It happens in three steps:

1. **Tests can hand the Character a Die.** The Die can be built around a supplied source of random numbers, so tests can script every roll. The Character can be built with a Die, and it still builds its own when it isn't given one.
2. **Characterization tests pin down every outcome of a Blow in both directions,** using scripted rolls, before any combat code moves. They record today's behaviour exactly, including the Dragon's unfloored Health Points and the English no-Weapon sentence.
3. **One module resolves a Blow.** Both attacks hand their rolls, comparisons and Damage to it and keep only their own narration. The characterization tests pass unchanged before and after.

## User Stories

1. As a player, I want every Attack Roll, Defense Roll and Damage to follow the same rules after the refactor, so that the fight I know is the fight I get.
2. As a player, I want every line of combat narration unchanged in English, Spanish and French, so that nothing I read changes.
3. As a player whose attack misses, I want to be told I missed and the Dragon's Health Points left untouched, as today.
4. As a player whose blow the Dragon dodges, I want the Dragon's taunt naming my Race, as today.
5. As a player who wounds the Dragon, I want to see the Damage and the Dragon's remaining Health Points, followed by its reply, as today.
6. As a player who lands a killing blow, I want the Dragon-defeated narrative instead of a reply, as today.
7. As a player whose killing blow overshoots, I want the Dragon's Health Points line to read exactly as it does today, even when it shows a number below zero.
8. As a player the Dragon misses, I want to be told so, with my Health Points untouched.
9. As a player who dodges the Dragon, I want to be told I defended, with my Health Points untouched.
10. As a player wearing no Armor, I want the Dragon's Damage to come off my Health Points in full.
11. As a player wearing Armor, I want the Dragon's Damage reduced by my Armor's Damage Reduction before it reaches my Health Points (ADR-0001).
12. As a player whose Armor stops a blow completely, I want to be told the Armor absorbed it, never that I dodged.
13. As a player taking a lethal blow, I want my Health Points to stop at zero and the defeat narrative to follow.
14. As a player whose roll exactly equals Strength or Agility, I want it to count the way it does today: a hit, or a dodge.
15. As a player, I want the Dragon's taunts and replies chosen the same way as today, so their variety doesn't change.
16. As a developer, I want to hand a scripted Die to the Character and to the Dragon, so that I can pin down an exact combat outcome in a test.
17. As a developer, I want a scripted Die to fail the test when it is asked for more rolls than I scripted, or leaves some unused, so that a change in how many rolls a Blow uses cannot slip through.
18. As a developer, I want every existing construction of a Character to keep compiling and keep rolling its own Die, so that the new seam costs the rest of the suite nothing.
19. As a developer, I want the characterization tests written and green before any combat code moves, so that they prove the refactor preserved behaviour instead of describing the new code.
20. As a developer, I want the characterization tests to pass unchanged after the refactor, so that "behaviour-preserving" is checked, not claimed.
21. As a developer, I want the Character's attack on the Dragon covered by tests for the first time, so that the least-tested half of the fight stops being blind.
22. As a developer, I want the rules of a Blow to live in one module, so that a change to them is made once.
23. As a developer, I want each side's narration to stay with that side, so that the Blow module holds rules and no "if the attacker is the Dragon" branches.
24. As a developer, I want Damage aimed at the Character to reach its reduction step through the Blow module itself, so that ADR-0001 holds by structure and not by remembering.
25. As a developer, I want each combatant's own response to Damage (the Character's reduction and floor at zero, the Dragon's plain subtraction) to stay with that combatant, so that the difference is stated in one visible place instead of being hidden in a copy.
26. As a developer, I want every public member of the Character, the Dragon and Combat to keep its current behaviour, so that the existing suite passes unchanged.
27. As a developer, I want **Blow** defined in the glossary, so that tests, code and conversation use one word for it.

## Implementation Decisions

**Behaviour is frozen, drift included.** The Dragon's Health Points stay unfloored and can be shown below zero. The Character's no-Weapon sentence stays exactly as today's English literal, returned before any roll. Neither is "fixed" here. Each would be a separate, deliberate change.

**Step 1: the Die accepts a source of numbers.** The Die gains a way to be built around a supplied source of random numbers. Building it with no arguments behaves exactly as today. Tests supply a scripted source. This is only the slice of the "dice seam" architecture candidate that this refactor needs. How the Die is shared across the Adventure is untouched.

**Step 1: the Character accepts a Die.** The Character gains a way to be built with a Die, and building it with no arguments still makes its own. The Dragon already takes a Die and doesn't change here. No other caller changes.

**Each attacker keeps its own Die.** Which Die makes which roll stays exactly as today:
- **The Character's Blow:** the Attack Roll, Defense Roll and Damage come from the Character's Die.
- **The Dragon's taunt or reply** to the Character's Blow is chosen with the Dragon's Die.
- **The Dragon's Blow:** all three rolls come from the Dragon's Die.

Characterization tests give each combatant its own scripted Die, so a change in this routing fails them.

**The order and count of rolls are frozen.** A Blow rolls the Attack Roll, then the Defense Roll only if the attack landed, then Damage only if it wasn't dodged. A miss consumes one roll and a dodge two. The Dragon's taunt roll happens only on a dodge, and its reply roll only when the Dragon survives Damage.

**Comparisons are frozen.** A Blow lands when the Attack Roll is at most the attacker's Strength. It is dodged when the Defense Roll is at most the target's Agility. Damage is rolled against the attacker's Weapon maximum.

**Step 3: the Blow module.** A new module resolves one Blow between an attacker and a target. It performs the rolls and comparisons, has the target take the Damage, and returns what happened as data, not text:
- whether the Blow missed, was dodged, or landed
- the Damage actually taken after the target's own response
- whether the target is defeated

It produces no narration and doesn't touch the Console.

**Combatant shape.** The Character and the Dragon both become combatants the Blow module can treat alike. The shape stays as small as the Blow needs:
- Strength
- Agility
- Health Points
- the roll of an Attack Roll and of Damage from its own Die
- a Defense Roll from the attacker's Die
- a single "take this Damage" step that returns the Damage actually taken

**Taking Damage is per combatant.** The Character's step runs its Damage through its reduction step (ADR-0001), subtracts the result, and floors Health Points at zero. The Dragon's step subtracts the Damage with no floor, which is today's behaviour, stated there on purpose. Because the Blow module always goes through this step, no Blow can skip the Character's Armor.

**Narration stays per side.** Both attack methods keep their current public form: they take their opponent and the message catalogue and return narrative text. Each prints its own intro and Weapon art as today, asks the Blow module to resolve the Blow, and turns the result into its own lines:
- **The Character's side:** miss, hit, the Dragon's taunt on a dodge, Damage dealt and the Dragon's Health Points, then the defeat narrative or the Dragon's reply.
- **The Dragon's side:** miss, hit, the Character defended, "Armor absorbed" when the Damage taken is zero or Damage dealt otherwise, the Character's Health Points, and the defeat narrative.

Today's small asymmetries stay: the Character always adds its Weapon art line, while the Dragon adds its art only when it isn't blank.

**Existing public members keep working.** Attack Roll, attack-lands, Damage and Damage Reduction members that exist on the Character or the Dragon today stay, with the same results, and may delegate. Removing any of them is out of scope.

**Combat is untouched.** The round still calls each side's attack and prints what it returns.

**Glossary.** **Blow** is added to the domain glossary: one attacker's attempt to hurt one target, made of an Attack Roll, a Defense Roll and Damage. "Hit", "strike", "turn" and "exchange" are on its avoid list. No ADR is written. The refactor changes no rule of the game, and ADR-0001 and ADR-0002 are unchanged.

## Testing Decisions

**What makes a good test here.** Assert what a caller of an attack method can observe:
- the narrative text it returns
- both combatants' Health Points afterwards
- that exactly the scripted rolls were consumed

Don't assert on the Blow module, private members, or the order of internal calls. The rolls are the one exception: their count and which combatant's Die supplied them are behaviour this refactor promises to keep.

**The seam is the existing public attack methods.** Characterization tests call the Character's attack on the Dragon and the Dragon's attack on the Character, with hand-built combatants that each hold a scripted Die. No test calls the Blow module directly, so the same tests prove the behaviour before and after the refactor.

**Scripted Die.** This is a test-side adapter. It is built from an ordered list of rolls and fails the test if asked for a roll it doesn't have, if asked for a roll outside the requested number of sides, or if a roll is left unused at the end. It's handed to a combatant through the step 1 seam. Building a Dragon consumes three rolls for its statistics, so tests either script those three or set the statistics afterwards.

**Expected values are literals.** Health Points after a Blow are written as numbers worked out from the scripted rolls, not recomputed the way the code does. Narration is asserted against the message catalogue, formatted with those literal values, in English, Spanish and French. The no-Weapon sentence is asserted as its exact English literal in every language, because that is today's behaviour.

**Cases for the Character's Blow on the Dragon:**
- **Miss** (Attack Roll one above Strength): missed narrative, Dragon's Health Points unchanged, one roll consumed.
- **Boundary hit** (Attack Roll equal to Strength) that the Dragon dodges (Defense Roll equal to its Agility): the dodge narrative and the taunt chosen by the Dragon's Die with the Character's translated Race, Health Points unchanged.
- **Wound:** Damage dealt, the Dragon's exact remaining Health Points, and the reply chosen by the Dragon's Die.
- **Killing blow** to exactly zero: the defeat narrative and no reply roll.
- **Overkill** below zero: the Dragon's Health Points and its narrated line show the negative number.
- **No Weapon:** the exact English sentence, no rolls consumed from either Die, Health Points unchanged.

**Cases for the Dragon's Blow on the Character:**
- **Miss.**
- **Dodge** (Defense Roll equal to the Character's Agility).
- **Wound without Armor:** exact remaining Health Points.
- **Wound with Armor:** Damage minus Damage Reduction.
- **Armor absorbs all:** the absorbed narrative, not the dodge narrative, and Health Points unchanged.
- **Lethal blow:** Health Points floored at zero and the defeat narrative.

**Existing tests stay.** The whole suite passes unchanged at every step, including the loose Dragon attack tests and the Armor tests on Damage Reduction.

**Prior art.**
- The Armor tests and Drink Potion tests build combatants by hand and assert exact Health Points.
- The Merchant tests assert returned narrative against the catalogue in all three languages.
- The Dragon tests already hand the Dragon a Die.

## Out of Scope

- **Flooring the Dragon's Health Points,** or any change to the Dragon's Health Points line.
- **Translating or rewording the no-Weapon sentence.**
- **Sharing one Die across the Adventure,** having the Adventure own the Die, or changing how Combat or the Game builds Dice (the rest of the dice-seam candidate).
- **Changing Combat,** including its round order, its shallow sequence methods, or its stats display.
- **Moving Console input and output** behind a seam (a separate architecture candidate).
- **Removing or renaming** any public member of the Character, the Dragon or Combat.
- **The drink action and healing.** Drinking the Healing Potion isn't a Blow, and ADR-0002 is untouched.
- **New combat rules** such as critical hits, a Dragon with Damage Reduction, or new attackers.
- **The stats displays,** including ticket 04 of the Equipment display.

## Further Notes

This spec comes from candidate 02 of the architecture review, "One module resolves a blow, whoever strikes it", which the user chose to keep behaviour-preserving and tightly scoped. Step 1 takes only the part of candidate 01 (the dice seam) that the characterization tests need.

The design decisions follow the recommended answers from the design interview:
- tests can script the Die through a small constructor change
- each attacker keeps its own Die
- the Blow module returns data, not text
- the concept is named Blow
- the work is recorded as a spec with tickets

The test seam, the existing public attack methods with no test seam on the Blow module, was confirmed by the user.

The vocabulary follows `CONTEXT.md`: Character, Dragon, Attack Roll, Defense Roll, Damage, Damage Reduction, Health Points, Armor, Weapon, and now Blow.
