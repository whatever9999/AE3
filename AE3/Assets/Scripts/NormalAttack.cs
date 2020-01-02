﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
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
        if (inRange && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine(Attack());
        }
    }

    public void NormalAttackButton()
    {
        combatMode = !combatMode;
        A.SetBool("AttackMode", combatMode);
    }

    public void DisableCombat()
    {
        combatMode = false;
        inRange = false;
        A.SetBool("AttackMode", combatMode);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (combatMode)
        {
            if (collision.GetComponent<CharacterState>() == CS.getTarget())
            {
                inRange = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (combatMode)
        {
            if (collision.GetComponent<CharacterState>() == CS.getTarget())
            {
                inRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (combatMode)
        {
            if (collision.GetComponent<CharacterState>() == CS.getTarget())
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
            CS.getTarget().DealDamage(damageToDeal);
        } else
        {
            //Hit failed
        }
    }
}
