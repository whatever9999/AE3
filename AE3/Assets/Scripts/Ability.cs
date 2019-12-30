using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityName
{
    CrusaderStrike,
    HammerOfJustice,
    DivineStorm,
    Judgement,
    HolyLight,
    Exorcism,
    Concecration,
    LayOnHands,
    DivineShield,
    HandOfFreedom,
    ArdentDefender,
    LightOfTheProtector,
    Aura,
    Seal,
    JudgementSetup,
    Blessing,
    DevotionAura,
    MagicalAura,
    RetributionAura,
    SealOfRighteousness,
    SealOfLight,
    SealOfJustice,
    JudgementOfRighteousness,
    JudgementOfWisdom,
    JudgementOfWeakness,
    BlessingOfMight,
    BlessingOfWisdom,
    BlessingOfKings
}

public enum AbilityGroupName
{
    Retribution,
    Holy,
    Protection,
    Utilities,
    Aura,
    Seal,
    Judgement,
    Blessing
}

public enum AbilityEffectName
{
    BaseDamage,
    HealByDamageCaused,
    HealTargetByAmount,
    DamageTargetByAmount,
    ConcecrateLand,
    HealToMax,
    BreakFreeOfEffects,
    HealForPercentageOfMissingHealth,
}

[System.Serializable]
public class Ability : MonoBehaviour
{
    public AbilityName abilityName;
    public AbilityEffect[] abilityEffects;
    public BuffName[] abilityBuffs;
    public float percentageManaCost;
    public bool instantCast;
    public float secondsToCast;
    public float secondsToCooldown;
    public bool singleTargetEffect;
}

[System.Serializable]
public struct AbilityEffect
{
    public AbilityEffectName name;
    public Vector2 abilityPowerRange;
}

[System.Serializable]
public struct AbilityGroup
{
    public AbilityGroupName abilityGroupName;
    public AbilityName[] abilities;
    public GameObject abilityPanel;
}