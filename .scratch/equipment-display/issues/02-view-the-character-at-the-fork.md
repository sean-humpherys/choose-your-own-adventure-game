# 02: View the Character at the fork

**What to build:** A fourth option at the fork. Today the player sees their Character only straight after creation, before any Equipment has been bought, and must then commit to the Dragon blind. Taking the north Path always ends the Adventure, so the fork is the last moment a look is useful.

Typing `c` or `character` at the fork shows the same Character summary the post-creation prompt shows, then asks the fork question again. The letter is English in all three languages, like `n`/`s`/`e` and the Merchant's `w`. Viewing changes no state: it can be done any number of times, it does not count as meeting the Merchant, and it does not bring him back once met. It is a view, not a Path, so the new prompt sentence comes last.

Wording, in all three languages:

| Message | English | Spanish | French |
| --- | --- | --- | --- |
| Fork prompt, appended sentence | Enter 'c' to view your character. | Escribe 'c' para ver tu personaje. | Tapez 'c' pour voir votre personnage. |
| Invalid choice at the fork | Invalid choice. Please enter 'n', 's', 'e', or 'c'. | Opcion invalida. Por favor escribe 'n', 's', 'e' o 'c'. | Choix invalide. Veuillez taper 'n', 's', 'e' ou 'c'. |

Add the spelling to the fork parser from ticket 01, and one branch to the fork loop. Test through the fork loop seam from ticket 01.

Demoable on its own: create a Character, type `c` at the fork, go south and buy something, type `c` again and see the purchase and the lower Gold. Empty slots stay hidden until ticket 03 lands.

**Blocked by:** 01 (Give the fork loop its own seam)

**Status:** ready-for-agent

- [ ] The fork accepts `c` and `character`, trimmed and case-insensitive, and prints the Character summary followed by the fork prompt again.
- [ ] Viewing before going south leaves the Merchant waiting: `c`, `s` reaches the Merchant's offer.
- [ ] Viewing after meeting the Merchant does not bring him back: `s`, decline, `c`, `s` shows the empty clearing.
- [ ] `c` twice then `e` prints the summary twice and ends the Adventure.
- [ ] The fork prompt ends with the new sentence and the invalid-choice message lists `c`, in English, Spanish and French.
- [ ] A digit, null, empty, whitespace-only and junk are still rejected at the fork.
- [ ] The post-creation option to list the Character's stats is unchanged.
- [ ] Tests cover the above through the fork loop in all three languages, and the validator tests are extended with `c` and `character`, uppercase and padded.
- [ ] The language parity test passes.
