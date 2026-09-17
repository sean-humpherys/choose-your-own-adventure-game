# 04: Combat stats display still hides empty Equipment slots

**What to build:** Nothing yet; this needs a decision. After ticket 03, the Character summary shows "Armor: none" and "Healing Potion: none", but the combat stats display shown when the Dragon fight opens still leaves empty slots out. The same Character therefore looks different at the fork and in the fight.

Arguments to weigh:
- **Match the summary:** one rule for every display of Equipment, and a clear reminder when the fight opens that no Healing Potion is carried.
- **Leave it:** the combat display is shown every round, space there is tighter, and an empty slot is nothing the player can act on mid-fight.

If it is changed, reuse the per-slot "none" messages from ticket 03 rather than adding new ones, and amend story 11 of the Healing Potion spec, which the combat display still honours.

Raised during the Equipment display design and deliberately left out of that feature's scope.

**Blocked by:** 03 (Show "none" for empty Equipment slots)

**Status:** needs-triage
