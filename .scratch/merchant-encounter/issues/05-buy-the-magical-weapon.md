# 05: Buy the Magical Weapon

**What to build:** The first thing the player can actually buy. Choosing the Magical Weapon replaces the Weapon their Occupation gave them and deducts the price from their Gold, and the new Weapon shows up wherever the old one did — on the character sheet and when the Dragon fight opens.

The Magical Weapon suits the Occupation: a Magician is not handed a sword. Its maximum Damage is the replaced Weapon's plus a fixed increment, the same increment for everyone, so the ordering established at character creation survives the purchase — a Thief's upgrade and a Fighter's upgrade are worth the same amount.

This ticket also lands the purchase plumbing that ticket 06 builds on: applying a choice, checking the purse can cover the price, and deducting the Gold.

**Blocked by:** 04 (the Encounter, the offer and the input validator must exist).

**Status:** ready-for-agent (implemented)

- [x] Buying the Magical Weapon replaces the carried Weapon and reduces Gold by the price.
- [x] The new Weapon's maximum Damage is the replaced Weapon's plus the fixed increment, for each of the four Occupations.
- [x] Each Occupation receives a Magical Weapon whose name fits it, translated in all three languages wherever Weapons are displayed.
- [x] The purchase is confirmed in the narrative, in all three languages.
- [x] Both the character sheet and the combat stats display show the new Weapon and its maximum Damage.
- [x] A purse that cannot cover the price is refused, including at the boundary where it falls one short — asserted at the purchase seam, since the once-per-Adventure rule means a player cannot reach a second purchase in play.
- [x] A Character carrying no Weapon is offered the Armor only, and never the Magical Weapon, since a relative increment has no base to build on.
- [x] Buying the Magical Weapon leaves Armor unset. Asserted in ticket 06, once Armor existed.
- [x] Tests assert the Weapon replacement, the Gold deduction, the per-Occupation Damage values, the affordability boundary and the missing-Weapon case, all without driving the Console.
- [x] The parity test from ticket 02 passes.

## Comments

`Merchant.ApplyChoice` now implements the `weapon` branch, and with it the purchase plumbing
ticket 06 builds on: the affordability guard, the Gold deduction and the missing-Weapon case all
live behind that one seam, so none of them needed a public surface of their own.

The Magical Weapon is looked up by Occupation from a table of name-and-art records — deliberately
not `Weapon` objects, since only the name and the art vary and a `Weapon` would need a meaningless
maximum Damage that a later reader could take for a real one. Its
maximum Damage is the replaced Weapon's plus `Merchant.MagicalDamageIncrement` (6). Because the
increment is uniform, the ordering the Occupations set at creation survives: Fighter 12 to 18,
Magician 12 to 18, Thief 6 to 12, Archer 8 to 14. A Thief's upgrade is worth exactly what a
Fighter's is.

Both stats displays already read `player.Weapon` and translate through the weapon display map, so
replacing the Weapon was enough to make the new one appear on the character sheet and when the
Dragon fight opens; tests assert both rather than assuming it.

A Character carrying no Weapon has nothing for a relative increment to build on, so `GetOffer`
omits the Magical Weapon line for them and the seam refuses the purchase with its own narrative.
The same refusal covers an Occupation with no Magical Weapon of its own, which validation makes
unreachable but the type still permits, since the Occupation-to-Weapon assignment has no default
branch either.

**The deferred checkbox.** "Buying the Magical Weapon leaves Armor unset" cannot be asserted here:
`Armor` and `Player.Armor` are ticket 06's "New module: Armor", and nothing in this ticket should
introduce them. Buying the Weapon touches only `Player.Weapon` and `Player.Gold`, so the
behaviour is already correct; it is the assertion that has to wait. Ticket 06 should add it when
Armor exists.

**Update:** ticket 06 introduced `Armor` and added that assertion, plus the matching one for
declining. The box is now ticked.

One rough edge left alone: when a Character carries no Weapon the offer correctly omits the
Magical Weapon, but the prompt still lists 'w' as an answer. Typing it gets the refusal narrative
rather than a re-prompt. Validation makes a weaponless Character unreachable in play, so this
costs a message key in three languages to fix a state no player can reach.

Verified by hand: a Thief buys a shadowfang dagger at 6 to 12 Damage with 100 Gold becoming 25,
and a Spanish Magician takes a "hechizo de rayo encadenado" into combat at 18 Damage. Full suite
173/173, parity included.
