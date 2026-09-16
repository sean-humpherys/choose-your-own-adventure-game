# 04: Buy the Healing Potion from the Merchant

**What to build:** A third ware on the Merchant's shelf. Until now he sells two permanent upgrades and the leftover Gold in the purse has no use, so a player who wants insurance rather than a slow advantage has nothing to spend on.

The Healing Potion costs the same as the Armor and the Magical Weapon, so the purse still affords exactly one thing and the choice becomes a genuine three-way one: hit harder, take less, or buy one reprieve. The once-per-Adventure rule is untouched — the flag trips when the Merchant is met, so buying a Potion consumes the visit exactly as buying Armor does. A Character carries at most one Potion, in a single slot, mirroring Armor.

The purchase goes through the Merchant's existing single seam, which already owns affordability, the Gold deduction and the purchase narrative. Do not add a second purchase path; a divergent one would drift from the first. The offer text gains a line naming the Potion and what it restores, so the player can weigh it against the Armor's Damage Reduction before choosing.

Both stats displays gain a Potion line, conditional on carrying one exactly as the Armor lines are, and the Potion is named through a display map parallel to the existing weapon and armor maps rather than a single message key — the same mechanism, so the same parity test protects it.

Demoable on its own: travel south, read the offer, buy the Potion, watch the Gold fall, and see it on the character sheet and again when the Dragon fight opens. Drinking it comes in ticket 05.

**Blocked by:** 02. This is collision avoidance rather than a logical dependency — both tickets extend the same two stats displays.

**Status:** ready-for-agent

- [x] A Potion module exists mirroring Armor: a type name for display, the Health Points it restores, and ASCII art.
- [x] The Character carries at most one Potion, in a single slot.
- [x] The Merchant's offer lists the Healing Potion, its price, and what it restores.
- [x] Buying the Potion equips it, reduces Gold by the price, and leaves the Weapon and Armor untouched.
- [x] Buying Armor or the Magical Weapon leaves the Potion unset, and declining leaves all three untouched.
- [x] The price matches the other wares, so the purse affords exactly one ware and never two.
- [x] A second purchase with the depleted purse is refused, including at the exact boundary where the remaining Gold is one short of the price.
- [x] Buying the Potion consumes the visit to the Merchant, exactly as the other purchases do.
- [x] The purchase goes through the Merchant's existing purchase seam; no second purchase path is added.
- [x] The Potion is accepted at the Merchant prompt by a single letter or the full word, trimmed and case-insensitive, through the existing parser.
- [x] The existing Armor, Weapon and decline spellings are still accepted unchanged.
- [x] Both the character sheet and the combat stats display show the Potion when one is carried, and neither shows a Potion line when none is.
- [x] The Potion is named in all three languages through a display map parallel to the weapon and armor maps.
- [x] The language parity test is extended to cover the new display map and passes.
- [x] Tests assert the purchase through the Merchant's seam: the Potion is set, the Gold falls by the price, and the other equipment is untouched.
