using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float startMoveSpeed = 5;

    public Range aggroRange;

    public RuntimeAnimatorController topAnimator;
    public RuntimeAnimatorController bottomAnimator;

    private Collider2D playerCollider;
    private CharacterState playerCS;

    private CharacterState CS;
    private NormalAttack NA;
    private Animator A;
    private SpriteRenderer SR;
    private Rigidbody2D RB;

    private float currentMoveSpeed;
    private Vector2 targetPosition;
    private bool inRange;
    public void SetInRange(bool inRange) { this.inRange = inRange; }

    public void SetMoveSpeed(float newSpeed)
    {
        if (!CS.getImmunityToSlow() || newSpeed >= currentMoveSpeed)
        {
            currentMoveSpeed = newSpeed;
        }
    }
    public float GetMoveSpeed() { return currentMoveSpeed; }

    private void Start()
    {
        CS = GetComponent<CharacterState>();
        NA = GetComponent<NormalAttack>();
        A = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody2D>();

        playerCS = PlayerAbilities.instance.GetPlayerCharacterState();
        playerCollider = playerCS.GetComponent<NormalAttack>().meleeRange.collider;
        currentMoveSpeed = startMoveSpeed;
    }

    private void FixedUpdate()
    {
        if((Vector2)transform.position != targetPosition && !inRange)
        {
            //Move enemy
            Vector3 MoveVec = targetPosition - (Vector2)transform.position;

            //Prevent player from moving faster on diagonals
            if (MoveVec.magnitude > 1)
            {
                MoveVec = MoveVec.normalized;
            }

            RB.velocity = MoveVec * currentMoveSpeed;
        }
    }

    private void Update()
    {
        if (!A.GetBool("Dead"))
        {
            if (CS.getTarget() == null)
            {
                targetPosition = CS.GetStartPosition();
            }

            //Get direction enemy is moving in and normalize if necessary
            Vector3 MoveVec = Vector3.zero;
            if (!inRange)
            {
                MoveVec = targetPosition - (Vector2)transform.position;

                if (MoveVec.magnitude > 1)
                {
                    MoveVec = MoveVec.normalized;
                }
            }

            //Enemy targeting
            if (aggroRange.collider.IsTouching(playerCollider) && CS.getTarget() == null && !playerCS.tag.Equals("Dead"))
            {
                CS.setTarget(playerCS);
                UIManager.instance.UpdateTargetOfTarget(playerCS);
                if (playerCS.getTarget() == CS)
                {
                    UIManager.instance.ToggleTargetOfTargetPanel(true);
                }
                NA.EnableCombat();
            }

            //Enemy movement
            if(CS.getTarget() != null)
            {
                Transform target = CS.getTarget().transform;
                targetPosition = (Vector2)target.position;
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
            } else
            {
                //If a value is 0 then the character will stay facing the way they were last moving
                if (MoveVec.x < 0)
                {
                    //Moving Left
                    SR.flipX = false;
                }
                else if (MoveVec.x > 0)
                {
                    //Moving Right
                    SR.flipX = true;
                }

                if (MoveVec.y < 0)
                {
                    //Going up
                    A.runtimeAnimatorController = bottomAnimator;
                }
                else if (MoveVec.y > 0)
                {
                    //Going down
                    A.runtimeAnimatorController = topAnimator;
                }
            }

            A.SetFloat("Speed", MoveVec.magnitude);
        }
    }
}
