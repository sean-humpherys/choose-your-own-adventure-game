# 02: Maximum Health Points, shown against current Health Points

**What to build:** A ceiling for the Character's health, and the display that makes it visible. Today Health Points are rolled once at creation and only ever fall, and nothing records what they started at — so there is nothing for healing to be capped against.

The Character now records Maximum Health Points at creation, equal to the Health Points rolled there, and it never changes for the rest of the Adventure. It reflects the Character actually rolled rather than an idealised maximum: a Halfling's negative Strength modifier already lowers the rolled Health Points, and the ceiling must lower with it.

Both the character sheet and the combat stats display now show the Character's Health Points together with that maximum, so a player can see how much they have lost. This is what later makes the decision to drink possible — a player needs to know how much of a Potion would actually be used before spending a round on it.

The Dragon has no maximum and its health line must not gain one; `CONTEXT.md` says so explicitly. Both displays currently render the Character's health and the Dragon's through the same shared message key, so this ticket separates those two lines. That separation is most of the work here and is where a regression would show.

Demoable on its own: create a Character, view the sheet, take the north Path, and see the maximum standing beside the current value in both places and holding steady as the Dragon lands blows.

**Blocked by:** 01 (the ADR records the cap this ticket introduces).

**Status:** ready-for-agent

- [ ] The Character records Maximum Health Points at creation, equal to the Health Points rolled there.
- [ ] Maximum Health Points is unchanged by Damage; only Health Points fall.
- [ ] A Halfling whose Strength modifier lowers rolled Health Points gets a correspondingly lower maximum.
- [ ] The character sheet shows the Character's Health Points together with the maximum.
- [ ] The combat stats display shows the Character's Health Points together with the maximum.
- [ ] The Dragon's health line is unchanged and shows no maximum.
- [ ] The Character's health line and the Dragon's are rendered separately rather than through one shared message key.
- [ ] The blow-by-blow combat narration is unchanged and does not repeat the maximum after every swing.
- [ ] The new text reads correctly in English, Spanish and French.
- [ ] Tests assert the maximum is set at creation, survives Damage untouched, and appears in both displays' returned strings.
- [ ] Tests assert both displays keep their existing Weapon, Armor and Gold content unchanged.
- [ ] The language parity test passes.
