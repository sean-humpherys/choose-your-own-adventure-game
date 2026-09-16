# 03: Gold on the Character

**What to build:** The Character starts the Adventure carrying money. A new Gold property is set to a fixed amount at creation — not rolled, so no Adventure can begin with a purse too small to matter — and the character sheet shows it in all three languages.

Nothing can be bought yet. This ticket exists so the purse is visible and established before the player meets anyone selling anything.

**Blocked by:** 01 (the character sheet must return a string before the Gold line can be asserted).

**Status:** ready-for-agent (implemented)

- [x] Every newly created Character carries the same fixed amount of Gold.
- [x] The character sheet shows the Gold, always — not conditionally like the Weapon lines.
- [x] The Gold label is translated in English, Spanish and French.
- [x] A test asserts a newly created Character's Gold is the expected fixed amount.
- [x] A test asserts the character sheet's returned string contains the Gold line in each of the three languages.
- [x] The language parity test (ticket 02), if already merged, still passes.

## Comments

`Player.Gold` is a new property initialised to `Player.StartingGold` (100), a `const` on the
Character so the purse is the one-line tuning change the spec describes. Being a property
initialiser rather than something `CreateCharacter` assigns, every Character carries it from
construction, whichever path built it.

The character sheet gains a `stats_gold` line straight after Health Points, outside the
`if (Weapon != null)` guard, so it always shows. The label is translated in all three languages
(`Gold: ` / `Oro: ` / `Or: `), following the catalogue's existing unaccented-ASCII convention.

Nothing was wired up to spend it: the price, the Merchant and the purchase seam belong to
tickets 04-06. 100 is chosen to afford exactly one item once those tickets price the stock.

Tests added to `PlayerTests`: a new Character's Gold is 100, two new Characters carry the same
amount, the sheet contains the Gold line in each of the three languages (asserting the label is
not the `[stats_gold]` fallback), and the Gold line survives when no Weapon is carried, which is
what distinguishes it from the conditional Weapon lines. Full suite 111/111, ticket 02's parity
test included.
