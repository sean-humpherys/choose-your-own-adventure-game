# 01: Give the fork loop its own seam

**What to build:** Make the fork testable before anything is added to it. Today the fork loop is written inline in the game's entry point, beside language selection, the main menu and character creation, so no test can reach it without scripting all of those. Its accepted spellings are also written out twice, once in the public validator and once in the prompt, which is the drift the Merchant's parser was introduced to prevent.

Move the fork loop into its own method in the Game module, taking the Character, the Dragon and the message catalogue, and owning the once-per-Adventure Merchant flag. The entry point creates the Character and calls it. Introduce a single fork parser returning `north`, `south`, `exit`, or nothing for unrecognised input, and have both the public validator and the prompt answer off it.

One behaviour changes, deliberately: when the input stream ends at the fork, the prompt today prints the invalid-choice message forever. Treat end of input as `exit`, the outcome that changes nothing, mirroring the Merchant prompt treating end of input as declining.

Otherwise the player sees no difference. This ticket is where a regression to north, south or exit would hide, so the new tests pin those down first. Follow the Merchant Encounter tests: drive the method with scripted console input and a hand-built Character, and assert on captured output. No test goes north, because the Dragon fight depends on unseeded rolls.

Verifiable on its own: the fork tests below pass, and the whole existing suite still passes unchanged.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] The fork loop is its own method taking the Character, the Dragon and the message catalogue; the entry point calls it.
- [ ] The once-per-Adventure Merchant flag is owned by that method and starts unset for each new Character.
- [ ] One parser holds the fork's spellings; the public validator and the prompt both use it.
- [ ] Every existing north, south and exit spelling is accepted as before, trimmed and case-insensitive; a digit, null, empty, whitespace-only and junk are rejected as before.
- [ ] An empty input stream at the fork ends the Adventure promptly instead of looping.
- [ ] In English, Spanish and French, tests drive the fork loop: `s` then declining shows the Merchant's offer; `s`, decline, `s` shows the empty-clearing narrative; junk prints the invalid-choice message and asks again; `e` ends the Adventure.
- [ ] The existing test suite passes unchanged.
