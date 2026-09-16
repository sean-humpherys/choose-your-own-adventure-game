# 01: Record ADR-0002 for capped, flat healing that costs the round

**What to build:** The decision record for how healing works, written before the code that depends on it. A future reader looking at the Healing Potion will ask three questions — why can't I heal past the Character I rolled, why is the amount fixed rather than rolled, and why does drinking cost me my attack — and this is the document that answers them.

It records healing as a flat restore, capped at Maximum Health Points, that consumes the Character's action for the round. It lists what was rejected and why: uncapped healing, which lets a Potion push a Character past the Character they rolled at creation; a rolled restore, which cannot be asserted in a test because the Die is constructed privately inside the Character and cannot be seeded; and drinking as a free action, which makes the Potion strictly dominant and the fight close to unloseable while one is carried.

It needs a calibration note in the style of ADR-0001's, because the balance is deliberately fine. The restore is set close to what Armor mitigates across a typical fight, which produces an intended optimal play: hold the Potion until well wounded, because the cap punishes drinking early and the lost round punishes drinking while safe. A future reader should be able to recognise that as the design rather than a flaw to correct.

This is ticket 01 rather than a note appended to the implementation because its decisions are split across two tickets — the cap lands in 02, the round cost in 05 — and neither of those owns the rule on its own.

Read `docs/adr/0001-armor-as-flat-damage-reduction.md` first and follow its shape. Its closing warning applies here in mirror image: ADR-0001 records that any new source of Damage bypassing the Character's reduction step silently breaks Armor, and the same class of trap exists for healing — a second way to raise Health Points that skips the ceiling silently breaks the cap.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

- [x] An ADR numbered 0002 exists alongside ADR-0001, following its structure: the decision, the considered options, and the consequences.
- [x] It states the decision as a flat restore, clamped at Maximum Health Points, consuming the Character's action for the round.
- [x] It records uncapped healing as considered and rejected, with the reason.
- [x] It records a rolled restore as considered and rejected, naming the non-injectable Die as the reason, consistent with how ADR-0001 reached the same conclusion for Damage Reduction.
- [x] It records free-action drinking as considered and rejected, with the reason.
- [x] It carries a calibration note explaining the chosen restore relative to the Dragon's average blow, the Character's average Health Points, and what Armor mitigates across a fight.
- [x] It warns that any future source of healing must route through the same clamp, or the ceiling silently stops working for that source.
- [x] It uses the vocabulary in `CONTEXT.md`: Healing Potion, Maximum Health Points, Health Points, Damage Reduction.
