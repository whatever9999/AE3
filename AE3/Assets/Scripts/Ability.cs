﻿using System.Collections;
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
    public Sprite image;
    public new AbilityName name;
    public string description;
    public AbilityEffect[] effects;
    public BuffName[] buffs;
    public float percentageManaCost;
    public bool instantCast;
    public float secondsToCast;
    public float secondsToCooldown;
    public bool singleTargetEffect;

    private Image tint;
    private Text timer;

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

    private void Start()
    {
        Image[] images = GetComponentsInChildren<Image>();

        foreach(Image i in images)
        {
            if(i.name.Equals("Image"))
            {
                tint = i;
            }
        }
        timer = GetComponentInChildren<Text>();

        tint.gameObject.SetActive(false);
        timer.text = "";

        redTint = Color.red;
        redTint.a = 0.5f;

        blackTint = Color.black;
        blackTint.a = 0.5f;
    }

    private void OnEnable()
    {
        //UpdateAbilityColour();
    }

    public void UpdateAbilityColour()
    {
        CheckIfUseable();

        if (coolingDown && tint.color != Color.black) {
            tint.color = blackTint;
            tint.gameObject.SetActive(true);
        }
        else if (useable && !coolingDown && tint.gameObject.activeInHierarchy)
        {
            tint.gameObject.SetActive(false);
        }
        else if (!useable && tint.color != Color.red)
        {
            tint.color = redTint;
            tint.gameObject.SetActive(true);
        }
    }

    private void CheckIfUseable()
    {
        //If in range and affordable
        int manaCost = (int)((CS.getMaxMana() / 100.0) * percentageManaCost);

        if(CS.getMana() >= manaCost && !coolingDown)
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