# 01: Prefactor: the character sheet returns a string

**What to build:** No change the player can see. The character sheet is currently written straight to the Console line by line, while the combat stats display builds its text and returns it. Bring the two into the same shape: the character sheet builds and returns its text, and the method that displays it does nothing but write that text out. This is groundwork — every later ticket adds lines to this display, and until it returns a value none of them can be asserted.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

- [x] Viewing the character sheet from character creation prints exactly what it printed before, in all three languages: header, Name, Race, Occupation, Strength, Agility, Health Points, and the Weapon lines.
- [x] The Weapon and maximum Damage lines remain conditional on a Weapon being carried, as before.
- [x] The text is available as a returned string without driving the Console.
- [x] A test asserts the returned string contains the Character's Name, Race, Occupation and statistics, following the way the combat stats display is already tested.
- [x] A test asserts the Weapon lines are absent when the Character carries no Weapon.
- [x] The existing test suite still passes.

## Comments

**Implemented** on `feature/merchant-encounter`.

`Player.GetStatsDisplay(Messages)` now builds the character sheet as a `List<string>` joined with `Environment.NewLine`, mirroring `Combat.GetCombatStatsDisplay`. `Player.DisplayStats` is reduced to writing that string out, so its caller in `PromptForNextAction` is unchanged. The `if (Weapon != null)` guard is preserved verbatim. `Dragon.DisplayStats` was deliberately left alone — nothing in this feature asserts the Dragon's sheet.

Nine tests added to `CYOATests/PlayerTests.cs`, following the helper-and-`StringAssert` pattern in `CYOATests/CombatTests.cs`. Full suite: 103 passed, 0 failed (94 before).

Behaviour verified unchanged in the running game: the character sheet was captured from character creation in English, Spanish and French, before and after the refactor, and the outputs are identical once the random roll values are normalised.

Reviewed on both axes via `/code-review`. Standards: no hard violations. Spec: all six acceptance criteria met, no scope creep.
