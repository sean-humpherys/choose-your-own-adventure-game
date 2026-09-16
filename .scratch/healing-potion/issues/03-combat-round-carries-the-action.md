# 03: Prefactor — the combat round carries the chosen action

**What to build:** Nothing the player can see. This is the "make the change easy" step that lets a later ticket add drinking without rewriting the fight.

The round loop today hard-codes the Character's attack at the top and puts the prompt at the bottom, so the opening attack happens before any choice is offered and each prompt governs the following pass. That shape cannot express "drink instead of attacking": a third branch beside attack and retreat would fire in the wrong pass.

After this ticket the loop carries the action the previous prompt chose and performs it at the top of the round, defaulting to attack for the opening round. The observable sequence is identical — the Character attacks, the Dragon answers, the prompt decides what the Character does next — and no extra prompt appears before the first blow.

This is the one place the feature touches existing control flow, so the guard is that the existing combat behaviour is provably unchanged. Retreat must still end the fight where it does now, both defeat checks must stay where they are, and a mistyped answer must still re-prompt without costing a round.

**Blocked by:** None (can start immediately). Independent of 01 and 02 and may run alongside them.

**Status:** ready-for-agent

- [ ] The round performs an action carried from the previous prompt rather than a hard-coded attack.
- [ ] The opening round still attacks, with no prompt shown before the first blow.
- [ ] The Dragon's turn and both defeat checks stay in their current positions in the round.
- [ ] Retreat ends the fight exactly as it does today, with the same narrative.
- [ ] An unrecognised answer still re-prompts without advancing the round.
- [ ] No player-facing text changes in any of the three languages.
- [ ] The existing combat tests pass unchanged, with no edits to their assertions.
