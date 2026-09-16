// What the Character does with its half of a round. The round carries one of these from
// the previous prompt rather than hard-coding an attack, so an action can be added without
// moving the Dragon's turn or either defeat check.
public enum CombatAction
{
    Attack,
    Retreat
}
