# 02: Pin down the Character's Blow on the Dragon

**What to build:** Characterization tests that record exactly what the Character's attack on the Dragon does today, before any combat code moves. That attack has no tests at all right now, so this is where a refactor could most easily change behaviour unnoticed.

The tests call the Character's existing public attack method with a hand-built Character and Dragon. Each holds its own scripted Die from ticket 01, so the tests pin which combatant's Die makes each roll:
- **the Character's Die:** the Attack Roll, the Defense Roll and Damage
- **the Dragon's Die:** its taunt or reply

They assert:
- the returned narration, against the message catalogue formatted with literal values
- both combatants' Health Points afterwards, as literal numbers worked out from the scripted rolls
- that every scripted roll was consumed and no more

They record today's drift as it is, without fixing it:
- the Dragon's Health Points are not floored and can be narrated below zero
- the no-Weapon sentence is an English literal in every language

This ticket changes no production code. If a case surprises you, record what the code does, not what it should do.

**Blocked by:** 01 (Let tests script the rolls of both combatants)

**Status:** ready-for-agent

- [ ] **Miss:** an Attack Roll one above the Character's Strength gives the missed narrative, leaves the Dragon's Health Points unchanged, and uses one roll.
- [ ] **Dodge at the boundary:** an Attack Roll equal to Strength lands, and a Defense Roll equal to the Dragon's Agility dodges. The result is the dodge narrative plus the taunt picked by the Dragon's Die, with the Character's translated Race. Health Points are unchanged.
- [ ] **Wound:** the Damage dealt, the Dragon's exact remaining Health Points, and the reply picked by the Dragon's Die.
- [ ] **Killing blow** to exactly zero: the Dragon-defeated narrative, with no reply roll made.
- [ ] **Overkill:** the Dragon's Health Points and its narrated Health Points line show the negative number.
- [ ] **No Weapon:** exactly "You have no weapon to attack with.", with no roll taken from either Die and both Health Points unchanged.
- [ ] Every case above runs in English, Spanish and French.
- [ ] No production code changes, and the whole suite passes.
