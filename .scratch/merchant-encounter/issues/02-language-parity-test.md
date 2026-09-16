# 02: Language parity test

**What to build:** A safety net for every string added from here on. Player-facing text is looked up by key from a per-language catalogue, and a missing key does not throw — it renders as the literal key name in brackets. A half-translated change therefore ships green and a Spanish or French player sees something like `[merchant_greeting]` on screen. This ticket adds the test that makes that impossible.

Write it against the catalogue in general, not against this feature's keys, so it keeps paying off for every string added later.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent (implemented)

- [x] A test asserts English, Spanish and French expose an identical set of dictionary keys.
- [x] A test asserts the same parity for the race, occupation and weapon display maps.
- [x] Failure output names the missing keys and the language missing them, so the fix is obvious without debugging.
- [x] The test passes against the catalogue as it stands today.
- [x] Removing any single key from any one language turns the test red — verify this by hand before finishing, then restore the key.

## Comments

Implemented as `CYOATests/LanguageParityTests.cs`: four tests, one per catalogue section
(`dictionary`, `displayRaceMap`, `displayOccupationMap`, `displayWeaponMap`), each asserting
that every key appearing in any language appears in all three. Written against whatever the
catalogue holds rather than a hardcoded key list, so keys added later are covered for free.

`raceMap` and `occupationMap` are deliberately excluded: they are localized *input* alias maps
(Spanish accepts `humano`, French `humain`), so their keys differ across languages by design.
The ticket's second checkbox names the *display* maps, which are keyed by the canonical English
value and are genuinely in parity.

Red-verify done by hand: removing `goodbye` from Spanish `dictionary` and `dagger` from French
`displayWeaponMap` turned two tests red, each naming the language and the missing key. The
catalogue was then restored byte-identical.
