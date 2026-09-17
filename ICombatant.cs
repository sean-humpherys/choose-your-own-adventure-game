// The smallest shape a Blow needs from either side of the fight, so the Character and the
// Dragon can be attacker or target alike. Rolls come from each combatant's own Die.
public interface ICombatant
{
    int Strength { get; }
    int Agility { get; }
    int HealthPoints { get; }

    int RollAttack();

    int RollDamage();

    // The target's Defense Roll against this combatant's Blow. It is rolled on the
    // attacker's Die, as it always has been, so it belongs to the attacker.
    int RollTargetDefense();

    // Takes the Damage of a landed Blow and returns the Damage actually taken. Each
    // combatant's own response lives here, so no Blow can bypass it.
    int TakeDamage(int damage);
}
