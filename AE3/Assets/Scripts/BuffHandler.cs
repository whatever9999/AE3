using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuffHandler : MonoBehaviour
{
    public GameObject[] possibleBuffObjects;

    public CharacterState CS;
    public PlayerMovement PM;

    private Buff[] possibleBuffs;
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
        }
    }

    private void ActivateBuff(Buff b)
    {
        foreach (BuffEffect be in b.effects)
        {
            switch (be.buffEffectName)
            {
                case BuffEffectName.SlowMovement:
                    PM.moveSpeed *= (be.buffPower / 100.0f);
                    break;
                case BuffEffectName.SlowAttack:
                    CS.setAttackSpeed(CS.getAttackSpeed() * (be.buffPower / 100.0f));
                    break;
                case BuffEffectName.Stun:
                    CS.setAttackSpeed(0);
                    PM.moveSpeed = 0;
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
                    PM.moveSpeed /= (be.buffPower / 100.0f);
                    break;
                case BuffEffectName.SlowAttack:
                    CS.setAttackSpeed(CS.getAttackSpeed() / (be.buffPower / 100.0f));
                    break;
                case BuffEffectName.Stun:
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

        for(int i = seconds; i >= 0; i--)
        {
            yield return new WaitForSeconds(1);
            buffTexts[buff].text = i + "s";
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