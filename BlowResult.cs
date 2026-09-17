// What a resolved Blow reports back to the side narrating it. DamageTaken is what the
// target actually lost after its own response to the Damage, zero unless the Blow landed.
public record BlowResult(BlowOutcome Outcome, int DamageTaken, bool TargetDefeated);
