# 03: Show "none" for empty Equipment slots

**What to build:** Make every Equipment slot visible on the Character summary. Today an empty slot is left out altogether, so a Character with no Armor has no Armor line. A player cannot tell "I carry no Armor" from "this game has no Armor", and nothing hints that the Merchant sold something they passed up.

The summary always shows the Weapon, Armor and Healing Potion slots. A filled slot keeps its translated name and number line exactly as today. An empty slot shows its label followed by "none", and no number line: "Damage Reduction: 0" under an absent Armor would read as a piece worth nothing. A Character always carries a Weapon in practice, but that slot follows the same rule so no slot is ever silently dropped. No Magical label is added.

Each slot has its own "none" message, holding only the word:

| Slot | English | Spanish | French |
| --- | --- | --- | --- |
| Weapon | none | ninguna | aucune |
| Armor | none | ninguna | aucune |
| Healing Potion | none | ninguna | aucune |

All three nouns are feminine in Spanish and French today, but one shared message would only be right by coincidence, and gendered agreement has already had to be fixed once in this project. Spellings follow the catalogue's unaccented convention.

This deliberately reverses story 11 of the Healing Potion spec, for the character summary only. The combat stats display is untouched; that is ticket 04. The existing test asserting that no Armor lines appear when none is worn encodes the old behaviour: rewrite it to assert the "none" line, do not delete it.

Test through the summary's returned string, as the Armor and Player tests already do. It needs neither ticket 01 nor 02.

Demoable on its own: create a Character, list its stats after creation, and see "Armor: none" and "Healing Potion: none" in each language.

**Blocked by:** None (can start immediately). It edits the same language file as ticket 02 but different messages, so expect at most a merge conflict.

**Status:** ready-for-agent

- [ ] A Character with no Armor shows the Armor label followed by its "none", and no Damage Reduction line.
- [ ] A Character with no Healing Potion shows the Potion label followed by its "none", and no restore line.
- [ ] A Character with no Weapon shows the Weapon label followed by its "none", and no maximum Damage line.
- [ ] Filled slots show their translated name and number line exactly as before.
- [ ] Each slot has its own "none" message in English, Spanish and French with the wording above.
- [ ] No Magical label or flag is added to Weapon or Armor.
- [ ] The combat stats display is unchanged.
- [ ] The existing "no Armor lines when none is worn" test is rewritten to assert the "none" line.
- [ ] Tests assert each empty and filled slot in all three languages, with no raw message key in the output.
- [ ] The language parity test passes.
