# 01: Let tests script the rolls of both combatants

**What to build:** A way for tests to decide every roll a combatant makes, without changing anything a player sees. Today no combat outcome can be pinned: the Die wraps an unseeded source of random numbers, and the Character builds its own Die privately, so tests can only assert loose bounds.

The Die gains a way to be built around a supplied source of random numbers. Building it with no arguments behaves exactly as today. The Character gains a way to be built with a Die. Building it with no arguments still makes its own, so every existing construction of a Character keeps compiling and rolling as before. The Dragon already takes a Die and doesn't change.

On the test side, add a scripted Die adapter. It is built from an ordered list of rolls and fails the test in any of these cases:
- it is asked for more rolls than were scripted
- a scripted roll is outside the number of sides requested
- any scripted roll is still unused at the end

Building a Dragon consumes three rolls for its statistics. The adapter makes it easy to script those three or skip past them.

This is only the part of the dice-seam architecture candidate that the characterization tests need. How Dice are created or shared across the Adventure is untouched.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] A Die built with no arguments rolls as it does today.
- [ ] A Die can be built around a supplied source of random numbers, and its rolls come from that source.
- [ ] A Character can be built with a Die and rolls from it. A Character built with no arguments still makes its own.
- [ ] The scripted Die adapter fails the test on too many roll requests, on an out-of-range scripted roll, and on unused rolls.
- [ ] A test shows a Character given a scripted Die produces exactly the scripted Attack Roll.
- [ ] No gameplay, narration or public behaviour changes. The existing test suite passes unchanged.
