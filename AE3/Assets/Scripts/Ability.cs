using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    PercentageOfPhysicalDamage,
    HealByDamageCaused,
    HealTargetByAmount,
    MagicalDamageInRange,
    ConcecrateLand,
    HealToMax,
    BreakFreeOfEffects,
    HealForPercentageOfMissingHealth,
}

[System.Serializable]
public class Ability : MonoBehaviour
{
    public Sprite image;
    public new AbilityName name;
    public string description;
    public AbilityEffect[] effects;
    public GameObject[] buffs;
    public float percentagePowerCost;
    public bool instantCast;
    public float secondsToCast;
    public float secondsToCooldown;
    public bool singleTargetEffect;
    public PowerTypes powerType;
    public RangeType requiredRange;
    public string targetTag;

    private Image tint;
    private Text timer;

    private bool setupComplete = false;
    private bool useable = true;
    private bool coolingDown = false;
    public void setCoolingDown(bool changeTo) { coolingDown = changeTo; }
    public bool getCoolingDown() { return coolingDown; }
    public bool getUseable() { return useable; }

    private Color redTint;
    private Color blackTint;

    private CharacterState CS;

    private void Awake()
    {
        CS = PlayerAbilities.instance.GetPlayerCharacterState();
    }

    private void OnEnable()
    {
        if(!setupComplete)
        {
            Image[] images = GetComponentsInChildren<Image>();

            foreach (Image i in images)
            {
                if (i.name.Equals("Image"))
                {
                    tint = i;
                }
            }
            timer = GetComponentInChildren<Text>();

            tint.enabled = false;
            timer.text = "";

            redTint = Color.red;
            redTint.a = 0.5f;

            blackTint = Color.black;
            blackTint.a = 0.5f;
        }

        UpdateAbilityColour();
    }

    public void UpdateAbilityColour()
    {
        CheckIfUseable();

        if (coolingDown && tint.color != Color.black) {
            tint.color = blackTint;
            tint.enabled = true;
        }
        else if (useable && !coolingDown && tint.enabled)
        {
            tint.enabled = false;
        }
        else if (!useable && tint.color != Color.red)
        {
            tint.color = redTint;
             tint.enabled = true;
        }
    }

    private void CheckIfUseable()
    {
        //If in range and affordable
        int manaCost = (int)((CS.getMaxPower() / 100.0) * percentagePowerCost);

        if(CS.getPower() >= manaCost && !coolingDown && (CS.getTarget().tag == targetTag || targetTag == ""))
        {
            useable = true;
        } else
        {
            useable = false;
        }
    }
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

public enum RangeType
{
    MELEE,
    MODERATE,
    LONG,
    AGGRO
}