using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities instance;

    public GameObject[] abilityObjects;

    public Range[] abilityRanges;

    private CharacterState CS;
    private Animator A;
    private NormalAttack NA;

    private ParticleSystem endSpellcastingEffect;
    private ParticleSystem[] spellcastingEffect;

    private Ability[] abilities;
    private Text[] abilityTexts;

    private Buff[] targetBuffs;

    public CharacterState GetPlayerCharacterState() { return CS; }

    private void Awake()
    {
        instance = this;

        CS = GetComponent<CharacterState>();
        A = GetComponent<Animator>();
        NA = GetComponent<NormalAttack>();

        ParticleSystem[] effects = GetComponentsInChildren<ParticleSystem>();

        int count = 0;
        spellcastingEffect = new ParticleSystem[2];
        foreach(ParticleSystem ps in effects)
        {
            if(ps.name.Equals("EndSpellcastingEffect"))
            {
                endSpellcastingEffect = ps;
            } else if (ps.name.Equals("SpellcastingEffect"))
            {
                spellcastingEffect[count] = ps;
                count++;
            }
        }
    }

    private void Start()
    {
        targetBuffs = GameObject.Find("TargetBuffPanel").GetComponent<BuffHandler>().GetPossibleBuffs();
        GameObject.Find("Target").SetActive(false);

        abilityTexts = new Text[abilityObjects.Length];
        abilities = new Ability[abilityObjects.Length];
        for (int i = 0; i < abilityObjects.Length; i++)
        {
            abilityTexts[i] = abilityObjects[i].GetComponentInChildren<Text>();
            abilities[i] = abilityObjects[i].GetComponent<Ability>();
        }
    }

    public void UseAbility(Ability a)
    {
        bool inRange = true;
        if (a.targetTag != "")
        {
            inRange = CheckIfInRange(a.requiredRange);
        }

        //If the ability isn't on cooldown, the player has enough mana to use it, the target is in range and it is intended for use on the current target or has no target then it will be cast
        if (!a.getCoolingDown() && a.getUseable() && inRange && (CS.getTarget().tag == a.targetTag || a.targetTag == ""))
        {
            //Switch case ability effects and add buffs
            StartCoroutine(Cast(a));
        } else
        {
            //Player cannot use ability
        }
    }

    private bool CheckIfInRange(RangeType range)
    {
        bool inRange = false;

        foreach (Range r in abilityRanges)
        {
            if(r.range == range)
            {
                inRange = r.collider.IsTouching(CS.getTarget().GetComponent<NormalAttack>().meleeRange.collider);
            }
        }

        if(CS.getTarget() == CS)
        {
            inRange = true;
        }

        return inRange;
    }

    private IEnumerator Cast(Ability a)
    {
        //Casting animation and castbar
        //Instant casts are treated as melee attacks
        if (!a.instantCast)
        {
            CS.setActiveRegen(PowerRegenCircumstance.CASTING);
            A.SetBool("Casting", true);
            UIManager.instance.SetPlayerCastbar(a.name.ToString(), a.secondsToCast);
            foreach (ParticleSystem ps in spellcastingEffect)
            {
                ps.Play();
            }
            SFXManager.instance.PlayEffect(SoundEffectNames.SPELLCASTING, a.secondsToCast);
            yield return new WaitForSeconds(a.secondsToCast);
            foreach (ParticleSystem ps in spellcastingEffect)
            {
                ps.Stop();
            }
            A.SetBool("Casting", false);
            CS.setActiveRegen(PowerRegenCircumstance.NOTCASTING);
            endSpellcastingEffect.Play();
        } else
        {
            //Instant cast abilities
            //If target isn't self and combat mode isn't enabled enable combat mode
            //Treat as melee attack if target isn't self
            //Spell if target is self
            if (!a.targetTag.Equals(tag) && !a.targetTag.Equals("") && !NA.GetCombatMode())
            {
                NA.EnableCombat();
                SFXManager.instance.PlayEffect(SoundEffectNames.ATTACK);
            }
            else if (!a.targetTag.Equals(tag) && !a.targetTag.Equals(""))
            {
                SFXManager.instance.PlayEffect(SoundEffectNames.ATTACK);
            }
            else
            {
                SFXManager.instance.PlayEffect(SoundEffectNames.SPELLCASTEND);
            }
            endSpellcastingEffect.Play();
        }
        

        //Update UI and start timer
        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i].name == a.name)
            {
                a.setCoolingDown(true);
                a.UpdateAbilityColour();
                StartCoroutine(AbilityTimer(i, a.secondsToCooldown));
            }
        }

        int damageCaused = 0;
        bool spellEffectsOccured = true;

        //Effects
        foreach (AbilityEffect e in a.effects)
        {
            switch (e.name)
            {
                case AbilityEffectName.PercentageOfPhysicalDamage:
                    //Deal a random percentage of a random amount of physical damage
                    int damageToDeal = (int)(Random.Range(e.abilityPowerRange[0], e.abilityPowerRange[1]) * (Random.Range(CS.startAttackDamage[0], CS.startAttackDamage[0]) / 100));
                    damageCaused = CS.getTarget().HitCritAndResult(damageToDeal, UIManager.ResultType.PHYSICALDAMAGE);
                    break;
                case AbilityEffectName.HealByPercentageOfDamageCaused:
                    CS.Heal((int)((damageCaused / 100.0) * Random.Range(e.abilityPowerRange[0], e.abilityPowerRange[1])));
                    break;
                case AbilityEffectName.HealTargetByAmount:
                    int amountToHeal = Random.Range((int)e.abilityPowerRange[0], (int)e.abilityPowerRange[1]);
                    CS.getTarget().HitCritAndResult(amountToHeal, UIManager.ResultType.HEALING);
                    break;
                case AbilityEffectName.MagicalDamageInRange:
                    //Deal a random amount of magical damage between two amounts
                    CS.getTarget().HitCritAndResult((int)(Random.Range(e.abilityPowerRange[0], e.abilityPowerRange[1])), UIManager.ResultType.MAGICALDAMAGE);
                    break;
                case AbilityEffectName.ConcecrateLand:
                    break;
                case AbilityEffectName.HealToMax:
                    break;
                case AbilityEffectName.BreakFreeOfEffects:
                    break;
                case AbilityEffectName.HealForPercentageOfMissingHealth:
                    break;
                case AbilityEffectName.UseJudgement:
                    foreach (Buff b in CS.GetCurrentBuffs())
                    {
                        if(b.name == BuffName.JudgementOfRighteousness || b.name == BuffName.JudgementOfWisdom || b.name == BuffName.JudgementOfWeakness)
                        {
                            foreach(Buff targetB in targetBuffs)
                            {
                                if(b.name == targetB.name)
                                {
                                    targetB.GetBuffHandler().ActivateBuff(targetB);
                                    break;
                                }
                            }
                            break;
                        }
                        else
                        { 
                            spellEffectsOccured = false; 
                        }
                    }
                    break;
                case AbilityEffectName.PercentageOfPhysicalDamageAsMagical:
                    //Deal a random percentage of a random amount of physical damage
                    damageToDeal = (int)(Random.Range(e.abilityPowerRange[0], e.abilityPowerRange[1]) * (Random.Range(CS.startAttackDamage[0], CS.startAttackDamage[0]) / 100));
                    CS.getTarget().DealDamage(damageToDeal, UIManager.ResultType.MAGICALDAMAGE);
                    break;
            }
        }

        //For example, if enemy has a debuff that restores health to its attacker
        CheckEffects();

        //Add buffs
        foreach(GameObject go in a.buffs)
        {
            Buff thisBuff = go.GetComponent<Buff>();
            thisBuff.GetBuffHandler().ActivateBuff(thisBuff);
        }

        //Deplete Power
        if (spellEffectsOccured)
        {
            CS.DepletePower(a.percentagePowerCost);
        }
    }

    public void CheckEffects()
    {
        int healthToRestore = (int)((CS.getMaxHealth() / 100) * CS.getTarget().getPercentageHealthRestoredToAttacker());

        if (healthToRestore != 0)
        {
            CS.Heal(healthToRestore);
        }
    }

    private IEnumerator AbilityTimer(int a, float seconds)
    {
        abilityTexts[a].text = UIManager.instance.MinutesAndSecondsFormat((int)System.Math.Ceiling(seconds));
        for (float i = seconds; i > 0; i--)
        {
            abilityTexts[a].text = UIManager.instance.MinutesAndSecondsFormat((int)System.Math.Ceiling(i));
            yield return new WaitForSeconds(1);
        }
        abilityTexts[a].text = "";

        abilities[a].setCoolingDown(false);
        abilities[a].UpdateAbilityColour();
    }
}
