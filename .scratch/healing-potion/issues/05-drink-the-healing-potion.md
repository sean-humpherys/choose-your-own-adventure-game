# 05: Drink the Healing Potion in combat

**What to build:** The feature itself. A player losing the Dragon fight finally has a move to make.

Drinking is offered as a third option at the combat prompt, beside attack and retreat, and only once the fight has begun — never at the prompt before it. Before combat the Character is at Maximum Health Points by definition, so a drink there would restore nothing; offering it would be a trap that only ever wastes the purchase.

Drinking restores a flat amount of Health Points, never above Maximum Health Points, and consumes the Potion. It also consumes the round: the Character drinks instead of attacking and the Dragon still swings. That is what makes the timing a decision — too early and the cap wastes most of it, too late and the killing blow lands first.

Two refusals, both of which keep the Potion. A Character carrying none is not offered the option at all, following the Merchant's precedent of not offering a Magical Weapon to a Character with nothing to better; typing it anyway re-prompts rather than costing a round. A Character already at Maximum Health Points is told why and re-prompted, because silently consuming a ware worth the entire purse for no effect reads as a defect.

The drinking logic belongs behind a single method on the Character that applies the restore, clamps, clears the Potion slot and returns its narrative — mirroring the existing reduction step, which is pure and assertable, and the existing attack methods, which return their narrative rather than writing to the Console. The fight prompts and prints; it holds no healing arithmetic.

Read `docs/adr/0002-*` from ticket 01 first. The clamp is the rule it protects: any path that raises Health Points while skipping the ceiling silently breaks the cap.

**Blocked by:** 01 (the ADR), 02 (the ceiling to clamp against), 03 (a round that can perform something other than an attack), 04 (a Potion to drink).

**Status:** ready-for-agent

- [ ] Drinking is offered at the combat prompt, alongside attack and retreat, accepted as a single letter or the full word, trimmed and case-insensitive, through the existing pure validator.
- [ ] Drinking is not offered at the prompt before combat begins, which still offers only attack and retreat.
- [ ] The drink option is absent from the prompt text when no Potion is carried.
- [ ] Drinking restores a flat amount of Health Points and consumes the Potion.
- [ ] The restore never raises Health Points above Maximum Health Points.
- [ ] A Character wounded by less than the restore returns to exactly Maximum Health Points and never exceeds it.
- [ ] Drinking replaces the Character's attack for that round, and the Dragon still takes its turn.
- [ ] Drinking at Maximum Health Points is refused, the Potion is kept, Health Points are unchanged, and the prompt re-prompts.
- [ ] Typing the drink option while carrying no Potion re-prompts and does not cost a round.
- [ ] The narrative states the amount restored and the resulting Health Points.
- [ ] The narratives for a successful drink, a refusal at full health, and having no Potion are distinct, so the player can tell the three apart.
- [ ] All new text reads correctly in English, Spanish and French, and the language parity test passes.
- [ ] The healing arithmetic lives behind a single method on the Character that returns its narrative, so it is asserted without driving the Console.
- [ ] Tests assert the restore across its boundaries: wounded by more than the restore, by exactly the restore, and by less than it.
- [ ] Tests assert a successful drink clears the Potion, and that both refusals leave the Potion and Health Points untouched.
- [ ] The existing combat tests pass unchanged.
