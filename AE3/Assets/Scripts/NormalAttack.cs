using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    public Range meleeRange;

    private bool combatMode = false;
    private bool isAttacking = false;
    private bool inRange = false;

    private Animator A;
    private CharacterState CS;

    private void Start()
    {
        A = GetComponent<Animator>();
        CS = GetComponent<CharacterState>();
    }

    private void Update()
    {
        if(combatMode)
        {
            CheckIfInRange();

            try
            {
                if (CS.getTarget().tag == "Dead")
                {
                    DisableCombat();
                    CS.setTarget(null);
                    UIManager.instance.ToggleTargetPanel(false);
                    UIManager.instance.ToggleTargetOfTargetPanel(false);
                }
            }
            catch (System.NullReferenceException e)
            {
                //There is no target
            }


            if (inRange && !isAttacking)
            {
                isAttacking = true;
                StartCoroutine(Attack());
            }
        }
    }

    public void NormalAttackButton()
    {
        SFXManager.instance.PlayEffect(SoundEffectNames.BUTTON);
        combatMode = !combatMode;
        A.SetBool("AttackMode", combatMode);
    }

    public void DisableCombat()
    {
        combatMode = false;
        inRange = false;
        A.SetBool("AttackMode", combatMode);
    }

    public void EnableCombat()
    {
        combatMode = true;
        A.SetBool("AttackMode", combatMode);
    }

    public void CheckIfInRange()
    {
        if (combatMode)
        {
            bool targetInRange = meleeRange.collider.IsTouching(CS.getTarget().GetComponent<NormalAttack>().meleeRange.collider);
            if (targetInRange)
            {
                inRange = true;
            } else
            {
                inRange = false;
            }
        }
    }

    private IEnumerator Attack()
    {
        float currentAttackSpeed = CS.getAttackSpeed();
        A.speed = 1 / currentAttackSpeed;
        A.SetTrigger("Attack");
        yield return new WaitForSeconds(currentAttackSpeed);
        A.speed = 1;
        isAttacking = false;
    }

    //Use the normal melee attack effects to deal damage
    public void AttackEvent()
    {
        //Successful hit?
        int rand = Random.Range(0, 100);
        if(rand <= CS.getChanceToHit())
        {
            //Calculate damage done in range
            Vector2 attackDamage = CS.getAttackDamage();
            int damageToDeal = Random.Range((int)attackDamage[0], (int)attackDamage[1]);

            //Critical hit
            rand = Random.Range(0, 100);
            if(rand <= CS.getChanceToCrit())
            {
                damageToDeal = (int)(damageToDeal * CS.getCritDamageMultiplier());
            }

            //Deal Damage
            CS.getTarget().DealDamage(damageToDeal, UIManager.ResultType.PHYSICALDAMAGE);
            SFXManager.instance.PlayEffect(SoundEffectNames.ATTACK);
        } else
        {
            SFXManager.instance.PlayEffect(SoundEffectNames.WHOOSH);
        }
    }
}

[System.Serializable]
public struct Range
{
    public RangeType range;
    public CircleCollider2D collider;
}