using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    private bool combatMode = false;
    private bool isAttacking = false;

    private Animator A;
    private CharacterState CS;

    private void Start()
    {
        A = GetComponent<Animator>();
        CS = GetComponent<CharacterState>();
    }

    public void NormalAttackButton()
    {
        combatMode = !combatMode;
        A.SetBool("AttackMode", combatMode);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(combatMode && !isAttacking)
        {
            if (tag.Equals("Player") && collision.tag.Equals("Enemy") || tag.Equals("Enemy") && collision.tag.Equals("Player"))
            {
                isAttacking = true;
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        A.speed = CS.getAttackSpeed();
        A.SetTrigger("Attack");
        yield return new WaitForSeconds(A.speed);
        A.speed = 1;
        isAttacking = false;
    }

    public void AttackEvent()
    {
        print("Here");
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
