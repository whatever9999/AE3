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

    private ParticleSystem endSpellcastingEffect;
    private ParticleSystem[] spellcastingEffect;

    private Ability[] abilities;
    private Text[] abilityTexts;

    public CharacterState GetPlayerCharacterState() { return CS; }

    private void Awake()
    {
        instance = this;

        CS = GetComponent<CharacterState>();
        A = GetComponent<Animator>();

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
        bool inRange = CheckIfInRange(a.requiredRange);

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
        A.SetBool("Casting", true);
        UIManager.instance.SetPlayerCastbar(a.name.ToString(), a.secondsToCast);
        foreach(ParticleSystem ps in spellcastingEffect)
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
        endSpellcastingEffect.Play();

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
        
        //Effects
        foreach (AbilityEffect e in a.effects)
        {
            switch (e.name)
            {
                case AbilityEffectName.PercentageOfPhysicalDamage:
                    //Deal a random percentage of a random amount of physical damage
                    int damageToDeal = (int)(Random.Range(e.abilityPowerRange[0], e.abilityPowerRange[1]) * (Random.Range(CS.startAttackDamage[0], CS.startAttackDamage[0]) / 100));
                    CS.getTarget().HitCritAndResult(damageToDeal, UIManager.ResultType.PHYSICALDAMAGE);
                    break;
                case AbilityEffectName.HealByDamageCaused:
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
                default:
                    break;
            }
        }

        //Add buffs
        foreach(GameObject go in a.buffs)
        {
            CS.getTarget().AddBuff(go.GetComponent<Buff>());
        }

        //Deplete Power
        CS.DepletePower(a.percentagePowerCost);
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
