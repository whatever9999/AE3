using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities instance;

    public GameObject[] abilityObjects;

    private CharacterState CS;
    private Animator A;

    private ParticleSystem spellcastingEffect;

    private Ability[] abilities;
    private Text[] abilityTexts;

    public CharacterState GetPlayerCharacterState() { return CS; }

    private void Awake()
    {
        instance = this;

        CS = GetComponent<CharacterState>();
        A = GetComponent<Animator>();

        spellcastingEffect = GetComponentInChildren<ParticleSystem>();
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
        if(!a.getCoolingDown() && a.getUseable())
        {
            //Switch case ability effects and add buffs
            StartCoroutine(Cast(a));
        } else
        {
            //Player cannot use ability
        }
    }

    private IEnumerator Cast(Ability a)
    {
        //Casting animation and castbar
        A.SetBool("Casting", true);
        UIManager.instance.SetPlayerCastbar(a.name.ToString(), a.secondsToCast);
        yield return new WaitForSeconds(a.secondsToCast);
        A.SetBool("Casting", false);
        spellcastingEffect.Play();

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
                case AbilityEffectName.BaseDamage:
                    int damageToDeal = Random.Range((int)e.abilityPowerRange[0], (int)e.abilityPowerRange[1]);
                    CS.getTarget().DealDamage(damageToDeal);
                    break;
                case AbilityEffectName.HealByDamageCaused:
                    break;
                case AbilityEffectName.HealTargetByAmount:
                    break;
                case AbilityEffectName.DamageTargetByAmount:
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

        //Deplete Mana
        CS.DepleteMana(a.percentageManaCost);
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
