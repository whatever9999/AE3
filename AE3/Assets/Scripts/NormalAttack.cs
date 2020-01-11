using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    public Range meleeRange;

    public Buff enemyStunBuff;

    private bool combatMode = false;
    public bool GetCombatMode() { return combatMode; }
    private bool isAttacking = false;
    private bool inRange = false;

    private Animator A;
    private CharacterState CS;
    private EnemyAI EnemyAI;

    private void Start()
    {
        A = GetComponent<Animator>();
        CS = GetComponent<CharacterState>();

        if (tag.Equals("Enemy"))
        {
            EnemyAI = GetComponent<EnemyAI>();
        }
    }

    private void Update()
    {
        if (combatMode)
        {
            CheckIfInRange();

            if ((CS.getTarget() != null && CS.getTarget().tag == "Dead") || tag.Equals("Dead"))
            {
                DisableCombat();
                CS.setTarget(null);
                UIManager.instance.ToggleTargetPanel(false);
                UIManager.instance.ToggleTargetOfTargetPanel(false);
            }
            
            if (inRange && !isAttacking)
            {
                isAttacking = true;
                StartCoroutine(Attack());
            }

            if (tag.Equals("Enemy"))
            {
                EnemyAI.SetInRange(inRange);
            }
        }
    }

    public void NormalAttackButton()
    {
        SFXManager.instance.PlayEffect(SoundEffectNames.BUTTON);
        combatMode = !combatMode;
        if (combatMode)
        {
            CS.setActiveRegen(PowerRegenCircumstance.NOTCASTING);
        }
        else
        {
            CS.setActiveRegen(PowerRegenCircumstance.NOTINCOMBAT);
        }
        A.SetBool("AttackMode", combatMode);
    }

    public void DisableCombat()
    {
        CS.setActiveRegen(PowerRegenCircumstance.NOTINCOMBAT);
        combatMode = false;
        inRange = false;
        isAttacking = false;
        A.SetBool("AttackMode", combatMode);
    }

    public void EnableCombat()
    {
        CS.setActiveRegen(PowerRegenCircumstance.NOTCASTING);
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
            }
            else
            {
                inRange = false;
            }
        }
    }

    private IEnumerator Attack()
    {
        float currentAttackSpeed = CS.getAttackSpeed();
        if (currentAttackSpeed != 0)
        {
            A.speed = 1 / currentAttackSpeed;
            A.SetTrigger("Attack");
            yield return new WaitForSeconds(currentAttackSpeed);
            A.speed = 1;
        }
        isAttacking = false;
    }

    //Use the normal melee attack effects to deal damage
    public void AttackEvent()
    {
        //Successful hit?
        int rand = Random.Range(0, 100);
        if (rand <= CS.getChanceToHit() && inRange)
        {
            //Calculate damage done in range
            Vector2 attackDamage = CS.getAttackDamage();
            int damageToDeal = Random.Range((int)attackDamage[0], (int)attackDamage[1]);

            //Critical hit
            rand = Random.Range(0, 100);
            if (rand <= CS.getChanceToCrit())
            {
                damageToDeal = (int)(damageToDeal * CS.getCritDamageMultiplier());
            }

            //Deal Damage and check if damage is deflected off of target
            CharacterState target = CS.getTarget();
            target.DealDamage(damageToDeal, UIManager.ResultType.PHYSICALDAMAGE);
            float damageDeflectedByTarget = target.getDamageDeflected();
            if (damageDeflectedByTarget > 0)
            {
                Vector2 targetAttackDamage = target.getAttackDamage();
                int damageDealt = (int)((Random.Range(targetAttackDamage[0], targetAttackDamage[1]) / 100) * damageDeflectedByTarget);
                //Make sure that if 2% of base damage is 0 the damage isn't shown
                if (damageDealt > 0)
                {
                    CS.DealDamage(damageDealt, UIManager.ResultType.MAGICALDAMAGE);
                }
            }
            SFXManager.instance.PlayEffect(SoundEffectNames.ATTACK);

            //Account for buffs on the enemy
            if (tag.Equals("Player"))
            {
                PlayerAbilities.instance.CheckEffects();
            }

            //Heal if there's a melee heal
            rand = Random.Range(0, 100);
            if (rand < CS.getMeleeHealChance())
            {
                int amountToHeal = (int)((damageToDeal / 100.0) * CS.getHealingFromMelee());
                CS.Heal(amountToHeal);
            }

            //Check if enemy gets stunned
            float chanceToStun = CS.getChanceToStun();
            if (chanceToStun > 0)
            {
                rand = Random.Range(0, 100);

                if (rand < chanceToStun)
                {
                    enemyStunBuff.lengthInSeconds = CS.getSecondsToStun();
                    enemyStunBuff.GetBuffHandler().ActivateBuff(enemyStunBuff);
                }
            }
            else
            {
                SFXManager.instance.PlayEffect(SoundEffectNames.WHOOSH);
            }
        }
    }

}

[System.Serializable]
public struct Range
{
    public RangeType range;
    public CircleCollider2D collider;
}