# 03: Pin down the Dragon's Blow on the Character

**What to build:** Characterization tests that record exactly what the Dragon's attack on the Character does today, before any combat code moves. Today's Dragon tests only check loose bounds ("Health Points did not rise", "not below zero"), so they would not catch a Blow that swapped a comparison or skipped Damage Reduction.

The tests call the Dragon's existing public attack method with a hand-built Dragon holding a scripted Die from ticket 01. The Dragon's Die makes all three rolls. They assert:
- the returned narration, against the message catalogue formatted with literal values
- the Character's Health Points afterwards, as literal numbers
- that every scripted roll was consumed and no more

Armor cases must show that Damage passes through the Character's Damage Reduction (ADR-0001). The case where Armor stops the whole blow must be narrated as absorbed, never as a dodge.

This ticket changes no production code, and it can be worked alongside ticket 02.

**Blocked by:** 01 (Let tests script the rolls of both combatants)

**Status:** ready-for-agent (implemented)

- [ ] **Miss:** an Attack Roll one above the Dragon's Strength gives the missed narrative, leaves the Character's Health Points unchanged, and uses one roll.
- [ ] **Dodge at the boundary:** a Defense Roll equal to the Character's Agility gives the defended narrative, with Health Points unchanged and two rolls used.
- [ ] **Wound without Armor:** the full Damage roll comes off, with exact remaining Health Points narrated.
- [ ] **Wound with Armor:** Damage minus the Armor's Damage Reduction comes off, with exact remaining Health Points.
- [ ] **Armor absorbs all:** the Armor-absorbed narrative, not the defended narrative, with Health Points unchanged.
- [ ] **Lethal blow:** Health Points stop at zero, not below, and the defeat narrative follows.
- [ ] Every case above runs in English, Spanish and French.
- [ ] No production code changes, and the whole suite passes.
