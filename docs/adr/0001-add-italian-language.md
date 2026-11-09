# ADR 0001: Add Italian as a Supported Language

- **Status:** Accepted
- **Date:** 2026-11-09
- **Decision maker:** Ryan Halford
- **Related files:** `language_data.json`, `Game.cs`, `Messages.cs`, `CYOATests/MessagesTests.cs`, `CYOATests/GameTests.cs`

## Context

The game supports English, Spanish, and French. All player-facing text lives in
`language_data.json`, one section per language. `Messages.cs` loads the section
for the language the player picks at startup, and `Game.cs` shows a numbered
language menu.

We want the game to reach more players and to show that the localization
design can take a new language without changes to the game logic. Italian was
chosen as the fourth language.

## Decision

Add Italian as a fourth language using the existing JSON-based localization
design:

1. **`language_data.json`:** add an `"Italian"` section with all 69 message keys
   plus the five lookup maps (`raceMap`, `occupationMap`, `displayRaceMap`,
   `displayOccupationMap`, `displayWeaponMap`).
2. **`Game.cs`:** add `4. Italiano` to the language menu. The menu-number-to-
   language mapping moves into a public `GetLanguageFromChoice` method so it can
   be unit tested.
3. **`Messages.cs`:** no changes needed. The class already loads any language
   section by name.
4. **Tests:** add Italian tests, and add one test that checks *every* non-English
   language has every English message key.

### Translation conventions (kept consistent with Spanish and French)

- **No accented characters.** The existing languages avoid accents (e.g. `Adios`,
  `epee`), which keeps output readable in Windows consoles that are not set to
  UTF-8. For Italian, a final accented vowel is written with an apostrophe, the
  usual Italian plain-keyboard spelling: `e'` (è), `piu'` (più), `agilita'` (agilità).
- **Commands stay in English.** The typed commands `roll`, `a`, `r`, `n`, `s`
  and `e` are hard-coded in C#. As in Spanish and French, the Italian prompts
  tell the player to type these English commands.
- **Translated input only.** Like the other languages, the Italian race and
  occupation lists accept only Italian names (`Umano`, `Elfo`, `Nano`,
  `Mezzuomo`; `Guerriero`, `Mago`, `Ladro`, `Arciere`).
- **"Halfling" is `Mezzuomo`,** the term used in Italian Tolkien translations.
- **Gendered grammar:** Italian articles and possessives change with the noun's
  gender (*la spada*, *il pugnale*). Weapon names are filled into one shared
  sentence, so the attack line avoids a possessive: *"impugni l'arma (spada
  lunga)"* instead of *"impugni il tuo spada lunga"*, which is wrong for
  feminine weapons.

## Alternatives Considered

| Option | Why not chosen |
|---|---|
| **Choose a different fourth language** (e.g. German, Portuguese) | Any of these would fit the design equally well. Italian was chosen for this assignment, and it is a good test of the design because, unlike English, its articles and possessives depend on grammatical gender. |
| **Use .NET `.resx` resource files** instead of JSON | The standard .NET approach, with compile-time checks, but it means moving all existing languages to a new format. That is too big a change for adding one language. |
| **Keep accented characters (UTF-8)** | More correct Italian, but inconsistent with the other languages, and risks garbled text in consoles not set to UTF-8. Could be revisited for all languages together. |
| **Translate the typed commands too** (e.g. `tira` for roll) | Needs C# changes in `Game`, `Player` and `Combat` to read commands from the language file. Out of scope; it should be done for all languages at once. |
| **Keep the inline `switch` in `StartGame`** | It works, but cannot be unit tested without faking console input. A small public method is easy to test. |

## Consequences

### Positive
- Italian-speaking players can play the full game.
- Shows the localization design is extensible: a new language needed only a JSON
  section and one menu option, with no changes to `Messages`, `Player`, `Dragon` or `Combat`.
- The new key-coverage test catches missing translations in *any* language, not
  just Italian.
- All tests pass (114 total, 20 new).

### Negative / Trade-offs
- **More text to keep in sync:** every new message must now be translated four
  times. The key-coverage test catches *missing* keys, but not wrong or stale translations.
- **Mixed-language commands:** Italian players still type English commands
  (`roll`, `a`, `r`, `n`, `s`, `e`).
- **Spelling without accents** (`e'` instead of `è`) is readable but not standard printed Italian.
- **Translation quality** was produced with AI help and has not been reviewed by
  a native speaker.
- **The menu is still hard-coded:** each new language means editing both the
  JSON file and the menu in `Game.cs`.

## Follow-up Ideas
- Build the language menu from the language sections in `language_data.json`, so
  adding a language needs no C# changes.
- Move typed commands into the language file so they can be localized.
- Decide project-wide whether to support UTF-8 accented characters
  (`Console.OutputEncoding = Encoding.UTF8`).
- Have a native Italian speaker review the translations.
