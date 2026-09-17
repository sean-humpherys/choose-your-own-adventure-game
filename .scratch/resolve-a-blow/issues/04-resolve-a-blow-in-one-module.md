# 04: Resolve a Blow in one module

**What to build:** Move the rules of a Blow into one module, with no change in behaviour. Today the Character's attack and the Dragon's attack each write out the Attack Roll, Defense Roll and Damage separately. The copies have drifted, and ADR-0001's "every blow reaches the Character's reduction step" holds only because one copy remembers.

A new module resolves one Blow between an attacker and a target:
- it takes the Attack Roll, Defense Roll and Damage from the attacker's own Die, in today's order, stopping at a miss or a dodge
- it has the target take the Damage
- it returns what happened as data: missed, dodged or landed, the Damage actually taken, and whether the target is defeated

It produces no narration and doesn't touch the Console.

The Character and the Dragon become combatants the module can treat alike, through the smallest shape the Blow needs. Taking Damage stays with each combatant:
- **The Character** runs Damage through its reduction step and floors its Health Points at zero.
- **The Dragon** subtracts with no floor. That is today's behaviour, now stated in one visible place.

Because the module always goes through this step, no Blow can skip the Character's Armor.

Both attack methods keep their public form and their own narration. Each prints its intro and Weapon art as today, asks the module to resolve the Blow, and turns the result into its own lines, including the Dragon's taunts and replies picked with the Dragon's Die. Combat doesn't change.

Keep behaviour frozen. The Dragon's Health Points stay unfloored, and the no-Weapon sentence stays an English literal returned before any roll. **Blow** is already defined in the glossary; use that word.

**Blocked by:** 02 (Pin down the Character's Blow on the Dragon), 03 (Pin down the Dragon's Blow on the Character)

**Status:** ready-for-agent (implemented)

- [ ] One module resolves a Blow for either attacker. Neither attack method still performs its own Attack Roll, Defense Roll or Damage arithmetic.
- [ ] The module returns data and produces no narration or Console output.
- [ ] Each combatant's own "take Damage" step holds its difference: the Character applies Damage Reduction and floors at zero, and the Dragon doesn't floor.
- [ ] Every existing public member of the Character, the Dragon and Combat still exists and behaves as before.
- [ ] The characterization tests from tickets 02 and 03 pass without being edited.
- [ ] The whole existing test suite passes without being edited.
- [ ] No narration, roll order, roll count or choice of Die changes in any language.
