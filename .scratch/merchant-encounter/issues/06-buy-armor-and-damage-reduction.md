# 06: Buy Armor and Damage Reduction

**What to build:** The other half of the Merchant's stock, and the first thing in the game that makes a Character harder to kill. Buying the Armor deducts the Gold and equips it, and from then on every blow the Dragon lands is reduced by a flat amount before it reaches the player's Health Points.

Damage never goes below zero, and a blow reduced to nothing gets its own narrative: the Armor absorbed it. That is not the same event as dodging and must not reuse the dodge message — the player did not evade, their Armor stopped the blow, and telling them otherwise misrepresents how their purchase works.

Read `docs/adr/0001-armor-as-flat-damage-reduction.md` first. Armor must not touch the Attack Roll or the Defense Roll: the glossary defines Agility as evasion only, and raising it was considered and rejected.

**Blocked by:** 05 (the purchase seam, the affordability guard and the Gold deduction land there).

**Status:** ready-for-agent (implemented)

- [x] Buying the Armor equips it, reduces Gold by the price, and leaves the carried Weapon untouched.
- [x] Each blow the Dragon lands is reduced by the Armor's flat Damage Reduction before Health Points are subtracted.
- [x] Reduced Damage floors at zero and can never restore Health Points.
- [x] A blow reduced to nothing prints its own absorbed-blow narrative, distinct from the existing dodge message, in all three languages.
- [x] The Attack Roll and the Defense Roll are unchanged; wearing Armor makes the Character neither harder to hit nor quicker to dodge.
- [x] Damage is applied through a single pure reduction step, so any future source of Damage routes through it rather than subtracting from Health Points directly.
- [x] Both the character sheet and the combat stats display show the Armor and its Damage Reduction, and show neither when no Armor is carried.
- [x] The Armor is named in all three languages wherever it is displayed.
- [x] Tests assert: a blow larger than the reduction loses exactly the reduction; a blow equal to it becomes zero; a blow smaller than it becomes zero and never negative; with no Armor the blow passes through unchanged.
- [x] Tests assert the purchase equips the Armor, deducts the Gold, and leaves the Weapon alone.
- [x] The parity test from ticket 02 passes.

## Comments

New `Armor` module mirroring `Weapon`: a type name, a Damage Reduction value and ASCII art. It is
a class rather than a bare integer on the Character because the name is needed for display and
translation regardless, so an integer would have saved nothing and split one concept in two.

`Player.ReduceDamage` is the single pure step the spec and ADR-0001 ask for: it takes incoming
Damage, subtracts the worn Armor's Damage Reduction, floors the result at zero, and touches
nothing else. `Dragon.Attack` now routes its rolled Damage through it instead of subtracting from
Health Points directly, so any future source of Damage that does the same keeps Armor working —
and one that does not will silently bypass it, which is the trap ADR-0001 records.

A blow reduced to nothing gets `player_armor_absorbed`, its own key in all three languages,
distinct from the dodge message. Reusing the dodge line would have told the player they evaded a
blow their Armor actually stopped, which misrepresents what they bought. A test asserts the two
messages differ in each language rather than just that the key exists.

Nothing touches the Attack Roll or the Defense Roll. Tests assert that wearing Armor leaves
Strength, Agility and `AttackHits` unchanged, which is the property ADR-0001 rejected the
alternative design to protect.

The Damage Reduction is 3, the value ADR-0001 calibrates: it nearly halves the Dragon's average
blow, where values near 5 would make the fight close to unloseable.

Both stats displays gained Armor and Damage Reduction lines, conditional on ownership like the
Weapon lines, and the Armor is named through a new `displayArmorMap` parallel to the weapon one.
Ticket 02's parity test was extended to cover that new section, and verified by hand: removing the
French entry turns it red naming the language and the key.

Ticket 05's deferred checkbox is now closed — buying the Magical Weapon, and declining, are both
asserted to leave Armor unset.

Verified by hand: the Armor is bought and buckled on for 75 Gold, both displays show it and its
Damage Reduction of 3, and an absorbed blow narrates as the Armor taking all of it with Health
Points unchanged. Full suite 208/208.
