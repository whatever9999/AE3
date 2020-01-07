using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuffHandler : MonoBehaviour
{
    public GameObject[] possibleBuffObjects;

    public CharacterState CS;
    public PlayerMovement PM;

    private Buff[] possibleBuffs;
    public Buff[] GetPossibleBuffs() { return possibleBuffs; }
    private Text[] buffTexts;

    private void Start()
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

    public void ActivateBuff(Buff b)
    {
        foreach (BuffEffect be in b.effects)
        {
            switch (be.buffEffectName)
            {
                case BuffEffectName.SlowMovement:
                    PM.startMoveSpeed *= (be.buffPower / 100.0f);
                    break;
                case BuffEffectName.SlowAttack:
                    CS.setAttackSpeed(CS.getAttackSpeed() * (be.buffPower / 100.0f));
                    break;
                case BuffEffectName.Stun:
                    CS.setAttackSpeed(0);
                    if (PM != null)
                    {
                        PM.SetMoveSpeed(0);
                    }
                    break;
                case BuffEffectName.DeflectDamage:
                    break;
                case BuffEffectName.NoMovement:
                    break;
                case BuffEffectName.ReducedDamageTaken:
                    break;
                case BuffEffectName.ImmuneToDamage:
                    break;
                case BuffEffectName.NoMelee:
                    break;
                case BuffEffectName.ImmuneToStun:
                    break;
                case BuffEffectName.ImmuneToSlow:
                    break;
                case BuffEffectName.ReduceDamageGiven:
                    break;
                case BuffEffectName.RestoreHealthToAttacker:
                    CS.setPercentageHealthRestoredToAttacker(be.buffPower);
                    break;
                case BuffEffectName.RestoreManaToAttacker:
                    break;
                case BuffEffectName.IncreaseMeleeDamage:
                    break;
                case BuffEffectName.RecoverManaByAmount:
                    break;
                case BuffEffectName.IncreaseAttackSpeed:
                    break;
                case BuffEffectName.IncreaseCritChance:
                    break;
                case BuffEffectName.IncreaseMaxHeath:
                    break;
                case BuffEffectName.IncreaseMaxMana:
                    break;
                case BuffEffectName.ReduceMeleeDamage:
                    break;
                case BuffEffectName.IncreaseBaseWeaponDamage:
                    break;
                default:
                    break;
            }
        }

        for (int i = 0; i < possibleBuffs.Length; i++)
        {
            if (possibleBuffs[i].name == b.name)
            {
                if(b.infiniteLength != true)
                {
                    StartCoroutine(BuffTimer(i, b.lengthInSeconds));
                    break;
                } else
                {
                    ActivateBuffIcon(i);
                }
            }
        }
    }

    public void DeactivateBuff(Buff b)
    {
        foreach (BuffEffect be in b.effects)
        {
            switch (be.buffEffectName)
            {
                case BuffEffectName.SlowMovement:
                    PM.startMoveSpeed /= (be.buffPower / 100.0f);
                    break;
                case BuffEffectName.SlowAttack:
                    CS.setAttackSpeed(CS.getAttackSpeed() / (be.buffPower / 100.0f));
                    break;
                case BuffEffectName.Stun:
                    CS.setAttackSpeed(CS.startAttackSpeed);
                    if (PM != null)
                    {
                        PM.SetMoveSpeed(PM.startMoveSpeed);
                    }
                    break;
                case BuffEffectName.DeflectDamage:
                    break;
                case BuffEffectName.NoMovement:
                    break;
                case BuffEffectName.ReducedDamageTaken:
                    break;
                case BuffEffectName.ImmuneToDamage:
                    break;
                case BuffEffectName.NoMelee:
                    break;
                case BuffEffectName.ImmuneToStun:
                    break;
                case BuffEffectName.ImmuneToSlow:
                    break;
                case BuffEffectName.ReduceDamageGiven:
                    break;
                case BuffEffectName.RestoreHealthToAttacker:
                    CS.setPercentageHealthRestoredToAttacker(CS.getPercentageHealthRestoredToAttacker() - be.buffPower);
                    break;
                case BuffEffectName.RestoreManaToAttacker:
                    break;
                case BuffEffectName.IncreaseMeleeDamage:
                    break;
                case BuffEffectName.RecoverManaByAmount:
                    break;
                case BuffEffectName.IncreaseAttackSpeed:
                    break;
                case BuffEffectName.IncreaseCritChance:
                    break;
                case BuffEffectName.IncreaseMaxHeath:
                    break;
                case BuffEffectName.IncreaseMaxMana:
                    break;
                case BuffEffectName.ReduceMeleeDamage:
                    break;
                case BuffEffectName.IncreaseBaseWeaponDamage:
                    break;
                default:
                    break;
            }
        }

        for (int i = 0; i < possibleBuffs.Length; i++)
        {
            if (possibleBuffs[i].name == b.name)
            {
                DeactivateBuffIcon(i);
            }
        }
    }

    private IEnumerator BuffTimer(int buff , int seconds)
    {
        possibleBuffObjects[buff].SetActive(true);
        CS.AddBuff(possibleBuffObjects[buff].GetComponent<Buff>());

        for(float i = seconds; i >= 0; i--)
        {
            yield return new WaitForSeconds(1);
            buffTexts[buff].text = UIManager.instance.MinutesAndSecondsFormat((int)System.Math.Ceiling(i));
        }

        possibleBuffObjects[buff].SetActive(false);
        Buff buffToRemove = possibleBuffObjects[buff].GetComponent<Buff>();
        DeactivateBuff(buffToRemove);
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