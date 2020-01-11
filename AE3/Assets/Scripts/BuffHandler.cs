using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuffHandler : MonoBehaviour
{
    public GameObject[] possibleBuffObjects;

    public CharacterState CS;
    public PlayerMovement PM;
    public EnemyAI EAI;

    private Buff[] possibleBuffs;
    public Buff[] GetPossibleBuffs() { return possibleBuffs; }
    private Text[] buffTexts;

    private void Awake()
    {
        buffTexts = new Text[possibleBuffObjects.Length];
        possibleBuffs = new Buff[possibleBuffObjects.Length];
        for(int i = 0; i < possibleBuffObjects.Length; i++)
        {
            buffTexts[i] = possibleBuffObjects[i].GetComponentInChildren<Text>();
            possibleBuffs[i] = possibleBuffObjects[i].GetComponent<Buff>();
            possibleBuffs[i].SetBuffHandler(this);

            //Ensure that initial auras, judgements, blessings and seals are active
        }
    }

    public void UpdateBuffs()
    {
        List<Buff> characterBuffs = CS.GetCurrentBuffs();

        for (int i = 0; i < possibleBuffObjects.Length; i++)
        {
            GameObject possibleGO = possibleBuffObjects[i];
            Buff possibleB = possibleBuffs[i];
            if (possibleGO.activeInHierarchy && !characterBuffs.Contains(possibleB))
            {
                possibleGO.SetActive(false);
            } else if(!possibleGO.activeInHierarchy && characterBuffs.Contains(possibleB))
            {
                possibleGO.SetActive(true);
            }
        }
    }

    public void ActivateBuff(Buff b)
    {
        bool buffAvailable = true;

        foreach(Buff currentB in CS.GetCurrentBuffs())
        {
            if (b.name == currentB.name && (b.name == BuffName.JudgementOfRighteousness || b.name == BuffName.JudgementOfWeakness || b.name == BuffName.JudgementOfWisdom || b.name == BuffName.DevotionAura || b.name == BuffName.MagicalAura || b.name == BuffName.RetributionAura || b.name == BuffName.SealOfJustice || b.name == BuffName.SealOfLight || b.name == BuffName.SealOfRighteousness || b.name == BuffName.BlessingOfKings || b.name == BuffName.DivineStrength || b.name == BuffName.DivineWisdom))
            {
                buffAvailable = false;
                DeactivateBuff(b);
                break;
            } else if (b.name == currentB.name)
            {
                buffAvailable = false;
                break;
            }
        }

        if(buffAvailable)
        {
            foreach (Effect e in b.effects)
            {
                e.ActivateEffect(CS, PM, EAI);
            }

            for (int i = 0; i < possibleBuffs.Length; i++)
            {
                if (possibleBuffs[i].name == b.name)
                {
                    if (b.infiniteLength != true)
                    {
                        StartBuffTimer(i, b.lengthInSeconds);
                        break;
                    }
                    else
                    {
                        ActivateBuffIcon(i);
                    }
                }
            }
        }
    }

    public void DeactivateBuff(Buff b)
    {
        
        foreach (Effect e in b.effects)
        {
            e.DeactivateEffect(CS, PM, EAI);
        }

        for (int i = 0; i < possibleBuffs.Length; i++)
        {
            if (possibleBuffs[i].name == b.name)
            {
                DeactivateBuffIcon(i);
            }
        }
    }

    private void StartBuffTimer(int buff , int seconds)
    {
        possibleBuffObjects[buff].SetActive(true);
        CS.AddBuff(possibleBuffs[buff]);

        possibleBuffs[buff].SetToRemove(buff, seconds);
    }

    public void UpdateBuffText(int buff, string time)
    {
        buffTexts[buff].text = time;
    }

    public void RemoveBuff(int buff)
    {
        possibleBuffObjects[buff].SetActive(false);
        DeactivateBuff(possibleBuffs[buff]);
        CS.RemoveBuff(possibleBuffObjects[buff].GetComponent<Buff>());
    }

    private void ActivateBuffIcon(int buff)
    {
        possibleBuffObjects[buff].SetActive(true);
        CS.AddBuff(possibleBuffObjects[buff].GetComponent<Buff>());
    }

    private void DeactivateBuffIcon(int buff)
    {
        possibleBuffObjects[buff].SetActive(false);
        CS.RemoveBuff(possibleBuffObjects[buff].GetComponent<Buff>());
    }
}

public enum EffectName
{
    SlowMovementByPercentage,
    SlowAttackByPercentage,
    HealByPercentageOfHealth,
    Stun,
    MeleeHeal,
    NoMovement,
    ReducedDamageTaken,
    ImmuneToDamage,
    NoMelee,
    ImmuneToStun,
    ImmuneToSlow,
    ReduceDamageGiven,
    RestoreHealthToAttacker,
    RestoreManaToAttacker,
    IncreaseInstantSpellDamage,
    RecoverManaByAmount,
    IncreaseAttackSpeed,
    IncreaseCritChance,
    IncreaseMaxHealthByPercentage,
    IncreaseMaxManaByPercentage,
    ReduceMeleeDamageByPercentage,
    IncreaseMeleeDamageByPercentage,
    AdditionalDamageForSingleTargetAttack,
    ChanceToStun,
    RecoverManaByPercentage,
    AttackAttackers,
    DevotionAura,
    MagicalAura,
    RetributionAura
}

[System.Serializable]
public class Effect
{
    public EffectName name;
    public float power;
    public float chance = 100;

    public void ActivateEffect(CharacterState CS, PlayerMovement PM, EnemyAI EAI)
    {
        switch (name)
        {
            case EffectName.SlowMovementByPercentage:
                if(PM != null)
                {
                    PM.SetMoveSpeed((PM.GetMoveSpeed() / 100.0f) * (100 + power));
                } else
                {
                    EAI.SetMoveSpeed((EAI.GetMoveSpeed() / 100.0f) * (100 + power));
                }
                break;
            case EffectName.SlowAttackByPercentage:
                CS.setAttackSpeed((CS.getAttackSpeed() / 100.0f) * (100 + power));
                break;
            case EffectName.Stun:
                //Make sure character is only stunned if not immune
                if (!CS.getImmunityToStun())
                {
                    CS.setAttackSpeed(0);
                    if (PM != null)
                    {
                        PM.SetMoveSpeed(0);
                    } else if(EAI != null)
                    {
                        EAI.SetMoveSpeed(0);
                    }
                }
                break;
            case EffectName.MeleeHeal:
                CS.setHealingFromMelee(CS.getHealingFromMelee() + power);
                CS.setMeleeHealChance(CS.getMeleeHealChance() + chance);
                break;
            case EffectName.NoMovement:
                break;
            case EffectName.ReducedDamageTaken:
                CS.setAllDamageReduction(CS.getAllDamageReduction() + power);
                break;
            case EffectName.ImmuneToDamage:
                CS.setImmunity(true);
                break;
            case EffectName.NoMelee:
                break;
            case EffectName.ImmuneToStun:
                CS.setImmunityToStun(true);
                break;
            case EffectName.ImmuneToSlow:
                CS.setImmunityToSlow(true);
                break;
            case EffectName.ReduceDamageGiven:
                break;
            case EffectName.RestoreHealthToAttacker:
                CS.setPercentageHealthRestoredToAttacker(CS.getPercentageHealthRestoredToAttacker() + power);
                break;
            case EffectName.RestoreManaToAttacker:
                CS.setPercentagePowerRestoredToAttacker(CS.getPercentagePowerRestoredToAttacker() + power);
                break;
            case EffectName.IncreaseInstantSpellDamage:
                CS.setAdditionalSpellDamage(CS.getAdditionalSpellDamage() + power);
                break;
            case EffectName.RecoverManaByAmount:
                CS.changeRegenPower((int)power);
                break;
            case EffectName.IncreaseAttackSpeed:
                CS.setAttackSpeed(CS.getAttackSpeed() + power);
                break;
            case EffectName.IncreaseCritChance:
                CS.setChanceToCrit(CS.getChanceToCrit() + (int)power);
                CS.setSpellChanceToCrit(CS.getSpellChanceToCrit() + (int)power);
                break;
            case EffectName.IncreaseMaxHealthByPercentage:
                CS.setMaxHealth((int)((CS.getMaxHealth() / 100.0f) * (100 + power)));
                break;
            case EffectName.IncreaseMaxManaByPercentage:
                CS.setMaxPower((int)((CS.getMaxPower() / 100.0f) * (100 + power)));
                break;
            case EffectName.ReduceMeleeDamageByPercentage:
                Vector2 currentAttackDamage = CS.getAttackDamage();
                Vector2 newAttackDamage = new Vector2((currentAttackDamage[0] / 100.0f) * (100 - power), (currentAttackDamage[1] / 100.0f) * (100 - power));
                CS.setAttackDamage(newAttackDamage);
                break;
            case EffectName.IncreaseMeleeDamageByPercentage:
                currentAttackDamage = CS.getAttackDamage();
                CS.setAttackDamage(new Vector2((currentAttackDamage[0] / 100.0f) * (100 + power), (currentAttackDamage[1] / 100.0f) * (100 + power)));
                break;
            case EffectName.DevotionAura:
                CS.getAura().SetAura(AuraName.DEVOTION);
                break;
            case EffectName.MagicalAura:
                CS.getAura().SetAura(AuraName.MAGICAL);
                break;
            case EffectName.RetributionAura:
                CS.getAura().SetAura(AuraName.RETRIBUTION);
                break;
            case EffectName.RecoverManaByPercentage:
                CS.changeRegenPower(CS.getRegenPower() + power);
                break;
            case EffectName.AttackAttackers:
                CS.setDamageDeflected(CS.getDamageDeflected() + power);
                break;
            case EffectName.ChanceToStun:
                CS.setSecondsToStun((int)power);
                CS.setChanceToStun(CS.getChanceToStun() + chance);
                break;
        }
    }

    public void DeactivateEffect(CharacterState CS, PlayerMovement PM, EnemyAI EAI)
    {
        switch (name)
        {
            case EffectName.SlowMovementByPercentage:
                if (PM != null)
                {
                    PM.SetMoveSpeed((PM.GetMoveSpeed() / (100 + power)) * 100);
                } else
                {
                    EAI.SetMoveSpeed((EAI.GetMoveSpeed() / (100 + power) * 100));
                }
                break;
            case EffectName.SlowAttackByPercentage:
                CS.setAttackSpeed((CS.getAttackSpeed() / (100 + power)) * 100);
                break;
            case EffectName.Stun:
                CS.setAttackSpeed(CS.startAttackSpeed);
                if (PM != null)
                {
                    PM.SetMoveSpeed(PM.startMoveSpeed);
                } else if(EAI != null)
                {
                    EAI.SetMoveSpeed(EAI.startMoveSpeed);
                }
                break;
            case EffectName.MeleeHeal:
                CS.setHealingFromMelee(CS.getHealingFromMelee() - power);
                CS.setMeleeHealChance(CS.getMeleeHealChance() - chance);
                break;
            case EffectName.NoMovement:
                break;
            case EffectName.ReducedDamageTaken:
                CS.setAllDamageReduction(CS.getAllDamageReduction() - power);
                break;
            case EffectName.ImmuneToDamage:
                CS.setImmunity(false);
                break;
            case EffectName.NoMelee:
                break;
            case EffectName.ImmuneToStun:
                CS.setImmunityToStun(false);
                break;
            case EffectName.ImmuneToSlow:
                CS.setImmunityToSlow(false);
                break;
            case EffectName.ReduceDamageGiven:
                break;
            case EffectName.RestoreHealthToAttacker:
                CS.setPercentageHealthRestoredToAttacker(CS.getPercentageHealthRestoredToAttacker() - power);
                break;
            case EffectName.RestoreManaToAttacker:
                CS.setPercentagePowerRestoredToAttacker(CS.getPercentagePowerRestoredToAttacker() - power);
                break;
            case EffectName.IncreaseInstantSpellDamage:
                CS.setAdditionalSpellDamage(CS.getAdditionalSpellDamage() - power);
                break;
            case EffectName.RecoverManaByAmount:
                CS.changeRegenPower(-(int)power);
                break;
            case EffectName.IncreaseAttackSpeed:
                CS.setAttackSpeed(CS.getAttackSpeed() - power);
                break;
            case EffectName.IncreaseCritChance:
                CS.setChanceToCrit(CS.getChanceToCrit() - (int)power);
                CS.setSpellChanceToCrit(CS.getSpellChanceToCrit() - (int)power);
                break;
            case EffectName.IncreaseMaxHealthByPercentage:
                CS.setMaxHealth((int)((CS.getMaxHealth() / (100 + power)) * 100));
                break;
            case EffectName.IncreaseMaxManaByPercentage:
                CS.setMaxPower((int)((CS.getMaxPower() / (100 + power)) * 100));
                break;
            case EffectName.ReduceMeleeDamageByPercentage:
                Vector2 currentAttackDamage = CS.getAttackDamage();
                Vector2 newAttackDamage = new Vector2((currentAttackDamage[0] / (100 + power)) * 100, (currentAttackDamage[0] / (100 + power)) * 100);
                CS.setAttackDamage(newAttackDamage);
                break;
            case EffectName.IncreaseMeleeDamageByPercentage:
                currentAttackDamage = CS.getAttackDamage();
                newAttackDamage = new Vector2((currentAttackDamage[0] / (100 - power)) * 100, (currentAttackDamage[0] / (100 - power)) * 100);
                CS.setAttackDamage(newAttackDamage);
                break;
            case EffectName.DevotionAura:
                CS.getAura().SetAura(AuraName.NONE);
                break;
            case EffectName.MagicalAura:
                CS.getAura().SetAura(AuraName.NONE);
                break;
            case EffectName.RetributionAura:
                CS.getAura().SetAura(AuraName.NONE);
                break;
            case EffectName.RecoverManaByPercentage:
                CS.changeRegenPower(CS.getRegenPower() - power);
                break;
            case EffectName.AttackAttackers:
                CS.setDamageDeflected(CS.getDamageDeflected() - power);
                break;
            case EffectName.ChanceToStun:
                CS.setChanceToStun(CS.getChanceToStun() - chance);
                break;
        }
    }
}