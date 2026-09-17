# Armor reduces damage by a flat amount rather than raising Agility

The game had no armor concept: combat resolved as an Attack Roll against Strength, then a Defense Roll against Agility, then full Weapon damage off Health Points. Adding Merchant-sold Armor meant inventing a mechanic. We chose **Damage Reduction**: a flat value subtracted from incoming Damage, floored at zero, applied by `Player.ReduceDamage`.

## Considered Options

- **Raise Agility.** Nearly free: the Defense Roll already exists, so armor would just make the existing dodge succeed more often. Rejected because it conflates evasion with protection. `CONTEXT.md` now defines Agility as evasion only, and plate mail making a character harder to hit *by dodging* is fiction the rest of the game contradicts.
- **A third roll against a Protection stat**, negating the blow entirely. Rejected as a third d20 in an already three-roll exchange, for an all-or-nothing outcome that is harder to reason about and to balance than a flat subtraction.
- **Flat Damage Reduction (chosen).** One new value, one changed line in `Dragon.Attack`, and a pure function that can be asserted against directly — which matters because `Die` is instantiated internally by `Player` and cannot be seeded from a test.

## Consequences

Damage Reduction is calibrated against the Dragon's claws (MaxDamage 12, averaging 6.5 per blow) and player Health Points (averaging about 10.5). A reduction of 3 nearly halves the average blow; values near 5 make the fight close to unloseable. Any future Weapon or Encounter that damages the player must route through `ReduceDamage` rather than subtracting from `HealthPoints` directly, or armor silently stops working for that source.
