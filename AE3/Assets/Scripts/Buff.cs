using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Buff : MonoBehaviour
{
    public Sprite image;
    public new BuffName name;
    public string description;
    public BuffEffect[] effects;
    public bool infiniteLength;
    public int lengthInSeconds;
    public int chance = 100;

    private BuffHandler BH;

    public void SetBuffHandler(BuffHandler buffHandler) { BH = buffHandler; }
    public BuffHandler GetBuffHandler() { return BH; }
}

public enum BuffName
{
    NONE,
    BlackFathomHamstring,
    NagaSpirit,
    Bash,
    VengefulStance,
    Chilled,
    FrozenSolid,
    Stun,
    Forbearance,
    DivineShield,
    HandOfFreedom,
    ArdentDefender,
    JudgementOfRighteousness,
    JudgementOfWisdom,
    JudgementOfWeakness,
    DivineStrength,
    DivineWisdom,
    BlessingOfKings,
    SealOfRighteousness,
    SealOfLight,
    SealOfJustice,
    DevotionAura,
    MagicalAura,
    RetributionAura

}

public enum BuffEffectName
{
    SlowMovement,
    SlowAttack,
    HealByPercentageOfHealth,
    Stun,
    DeflectDamage,
    NoMovement,
    ReducedDamageTaken,
    ImmuneToDamage,
    NoMelee,
    ImmuneToStun,
    ImmuneToSlow,
    ReduceDamageGiven,
    RestoreHealthToAttacker,
    RestoreManaToAttacker,
    IncreaseMeleeDamage,
    RecoverManaByAmount,
    IncreaseAttackSpeed,
    IncreaseCritChance,
    IncreaseMaxHeath,
    IncreaseMaxMana,
    ReduceMeleeDamage,
    IncreaseBaseWeaponDamage,
    AdditionalDamageForSingleTargetAttack,
    ChanceToStun,
    RecoverManaByPercentage,
    AttackAttackers
}

[System.Serializable]
public struct BuffEffect
{
    public BuffEffectName buffEffectName;
    public float buffPower;
}