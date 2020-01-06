using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Range aggroRange;

    public RuntimeAnimatorController topAnimator;
    public RuntimeAnimatorController bottomAnimator;

    private Collider2D playerCollider;
    private CharacterState playerCS;

    private CharacterState CS;
    private NormalAttack NA;
    private Animator A;
    private SpriteRenderer SR;

    private void Start()
    {
        CS = GetComponent<CharacterState>();
        NA = GetComponent<NormalAttack>();
        A = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();

        playerCS = PlayerAbilities.instance.GetPlayerCharacterState();
        playerCollider = playerCS.GetComponent<NormalAttack>().meleeRange.collider;
    }

    private void Update()
    {
        //Enemy targeting
        if (aggroRange.collider.IsTouching(playerCollider) && CS.getTarget() == null) {
            CS.setTarget(playerCS);
            UIManager.instance.UpdateTargetOfTarget(playerCS);
            if (playerCS.getTarget() == CS)
            {
                UIManager.instance.ToggleTargetOfTargetPanel(true);
            }
            NA.EnableCombat();
        }

        //Enemy movement
        if (CS.getTarget() == null)
        {
            //Return to start position
        }
        else
        {
            Transform target = CS.getTarget().transform;
            bool attackMode = A.GetBool("AttackMode");

            if (target.position.x < transform.position.x && target.position.y < transform.position.y)
            {
                //Enemy to bottom left
                SR.flipX = false;
                A.runtimeAnimatorController = bottomAnimator;
            }
            else if (target.position.x > transform.position.x && target.position.y < transform.position.y)
            {
                //Enemy to bottom right
                SR.flipX = true;
                A.runtimeAnimatorController = bottomAnimator;
            }
            else if (target.position.x < transform.position.x && target.position.y > transform.position.y)
            {
                //Enemy to top left
                SR.flipX = false;
                A.runtimeAnimatorController = topAnimator;
            }
            else if (target.position.x > transform.position.x && target.position.y > transform.position.y)
            {
                //Enemy to top right
                SR.flipX = true;
                A.runtimeAnimatorController = topAnimator;
            }

            A.SetBool("AttackMode", attackMode);
        }

        //Set speed in animator - A.SetFloat("Speed", MoveVec.magnitude);
    }
}
