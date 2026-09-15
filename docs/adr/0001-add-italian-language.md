# ADR 0001: Add Italian as a Fourth Language

## Status
Proposed. Implemented on branch `feature/italian-language` and awaiting review and merge into `main`.

## Date
2026-09-15

## Context
The game supports three languages: English, Spanish, and French. The player picks one at startup, and every message after that is shown in the chosen language.

The existing language support works like this:

- `language_data.json` holds one top-level section per language. Each section has six parts:
  - `dictionary`: every message the game prints, looked up by key (69 keys).
  - `raceMap` and `occupationMap`: turn what the player types (for example `elfo`) into the internal English name (`Elf`).
  - `displayRaceMap`, `displayOccupationMap`, and `displayWeaponMap`: turn internal English names back into the chosen language for display.
- `Messages.cs` has no language-specific code. `ReadDictionary()` loads whichever section matches `CurrentLanguage`.
- `Game.cs` has a hard-coded language menu (`1. English  2. Espanol  3. Francais`) and a `switch` that turns the number into a language name.
- The existing translations are plain ASCII with accents removed (for example `Espanol`, `dano`, `etre`).
- Command inputs are checked directly in C# and are the same in every language: `a`/`r` in combat, `n`/`s`/`e` for paths, and `roll` for stat rolls.
- Tests in `MessagesTests.cs` and `GameTests.cs` repeat the same checks for each language.

We decided to offer Italian as an additional language.

## Decision
Add Italian as a fourth language by following the existing pattern exactly:

1. Add an `"Italian"` section to `language_data.json` with all 69 dictionary keys and all five lookup maps. Use the same `{0}`/`{1}` placeholders as English.
2. Add `4. Italiano` to the language menu in `Game.cs` and `"4" => "Italian"` to the language `switch`.
3. Add Italian versions of the per-language tests in `MessagesTests.cs` and `GameTests.cs`.

Italian text follows the existing conventions:

- Plain ASCII with no accents (for example `Qual e`, `Agilita`, `piu`).
- Command inputs stay `a`, `r`, `n`, `s`, `e`, and `roll`. These happen to match Italian words: **a**ttaccare, **r**itirarsi, **n**ord, **s**ud, **e**sci.
- Race input words are `umano`, `elfo`, `nano`, `mezzuomo`. Occupation input words are `guerriero`, `mago`, `ladro`, `arciere`.
- Where an Italian article would depend on a noun's gender, the text avoids the article. For example, `Da {0}, impugni l'arma ({1})` reads correctly for both `spada lunga` (feminine) and `pugnale` (masculine).

## Alternatives Considered

### 1. Keep only English, Spanish, and French
No code or data changes, and no new translations to maintain.
**Rejected** because it doesn't meet the goal of offering Italian.

### 2. Build the language menu from `language_data.json`
Read the top-level language names from the JSON file and generate the menu automatically, so adding a language would need no C# change.
**Rejected for now.** It would change how `Game.cs` starts the game, needs a display name for each language (for example "Italiano"), and goes beyond adding one language. It is still a good future improvement (see Consequences).

### 3. Switch to .NET resource files (`.resx`) with `CultureInfo`
Use the standard .NET localization system instead of a custom JSON file.
**Rejected.** It would mean rewriting `Messages.cs` and moving every existing translation. That's a much larger change than one new language justifies, and it would break the project's existing, working approach.

### 4. Use proper Italian accents (UTF-8)
Write `Qual è`, `Agilità`, `più`, and so on.
**Rejected** to stay consistent with the other languages, which are all ASCII-only. Accented characters can also display incorrectly in some terminals unless the console encoding is set. If accents are wanted, all languages should switch together in a separate change.

### 5. Translate the command letters
Use Italian-specific command letters or words.
**Rejected.** The commands are hard-coded in `Game.cs`, `Combat.cs`, and `Player.cs`, so this would need code changes. The existing letters already match Italian words, so no change is needed.

## Rationale
- **Low cost, low risk.** The language system is data-driven, so Italian is almost entirely a data change: one JSON section, a three-line language menu change in `Game.cs`, and tests. `Messages.cs`, `Player.cs`, `Dragon.cs`, and `Combat.cs` are unchanged.
- **Consistency.** Following the existing structure and conventions exactly means Italian behaves like the other three languages, and anyone who understands one language section understands them all.
- **Verifiable.** A script confirmed that the Italian section has the same dictionary keys, map structure, and placeholders as English. The new tests pass (100/100), and a scripted play-through in Italian showed no missing-key `[placeholders]`.

## Consequences

### Positive
- Players can play the full game in Italian: menus, character creation, stats, paths, the dragon encounter, combat, and endings.
- Existing languages and game behavior are unaffected.
- It shows that the JSON-based design scales to more languages with very little code.

### Negative / Trade-offs
- **Unreviewed translations.** The Italian text has not been reviewed by a native speaker. It should be before release.
- **No accents.** Some words are ambiguous without accents. For example, `e` can mean "and" (`e`) or "is" (`è`). This is inherited from the existing ASCII convention.
- **More maintenance.** Every new or changed message key must now be added in four languages instead of three.
- **Hard-coded menu.** Each new language still needs a manual edit to the menu and `switch` in `Game.cs`. Alternative 2 would remove this.
- **Partial test coverage of keys.** The per-language "required keys" tests check only 11 of the 69 dictionary keys. A missing Italian key would not fail a test and would appear in the game as `[key_name]`. A future test could compare every language's keys against English.
- **Missing sections still crash.** `Messages.ReadDictionary()` throws if a language section or map is missing from `language_data.json`, so a malformed Italian section would crash the game when Italian is selected.

## Related
- Branch: `feature/italian-language` (commit `28aa8c9`, "Add Italian as a fourth language")
- Files changed by the decision: `language_data.json`, `Game.cs`, `CYOATests/MessagesTests.cs`, `CYOATests/GameTests.cs`
