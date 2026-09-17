// The rules of a Blow, written once for either attacker. It rolls and compares, has the
// target take the Damage, and reports what happened; each side narrates it its own way.
public static class Blow
{
    public static BlowResult Resolve(ICombatant attacker, ICombatant target)
    {
        int attackRoll = attacker.RollAttack();

        if (attackRoll > attacker.Strength)
            return new BlowResult(BlowOutcome.Missed, 0, false);

        int defenseRoll = attacker.RollDefense();

        if (defenseRoll <= target.Agility)
            return new BlowResult(BlowOutcome.Dodged, 0, false);

        // Always through the target's own step, so the Character's Damage Reduction
        // applies to every Blow aimed at it (docs/adr/0001-armor-as-flat-damage-reduction.md).
        int damageTaken = target.TakeDamage(attacker.RollDamage());

        return new BlowResult(BlowOutcome.Landed, damageTaken, target.HealthPoints <= 0);
    }
}

public enum BlowOutcome
{
    Missed,
    Dodged,
    Landed
}

public record BlowResult(BlowOutcome Outcome, int DamageTaken, bool TargetDefeated);
