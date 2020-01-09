using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    public float startMoveSpeed = 5;
    public RuntimeAnimatorController topAnimator;
    public RuntimeAnimatorController bottomAnimator;
    public Sprite topIdle;
    public Sprite bottomIdle;

    private float currentMoveSpeed;
    public void SetMoveSpeed(float newSpeed)
    {
        if(!CS.getImmunityToSlow() || newSpeed >= currentMoveSpeed)
        {
            currentMoveSpeed = newSpeed;
            print(currentMoveSpeed);
        }
    }
    public float GetMoveSpeed() { return currentMoveSpeed; }

    private Rigidbody2D RB;
    private Animator A;
    private SpriteRenderer SR;
    private CharacterState CS;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        A = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
        CS = GetComponent<CharacterState>();

        currentMoveSpeed = startMoveSpeed;
    }

    private void FixedUpdate()
    {
        Vector3 MoveVec = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));

        //Prevent player from moving faster on diagonals
        if (MoveVec.magnitude > 1)
        {
            MoveVec = MoveVec.normalized;
        }

        RB.velocity = MoveVec * currentMoveSpeed;
    }

    private void Update()
    {
        if (!A.GetBool("Dead"))
        {
            //Get direction player is moving in and normalize if necessary
            Vector2 MoveVec = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));

            if (MoveVec.magnitude > 1)
            {
                MoveVec = MoveVec.normalized;
            }

            if (CS.getTarget() == null || CS.getTarget().tag.Equals("Player"))
            {
                //If a value is 0 then the character will stay facing the way they were last moving
                if (MoveVec.x < 0)
                {
                    //Moving Left
                    SR.flipX = true;
                }
                else if (MoveVec.x > 0)
                {
                    //Moving Right
                    SR.flipX = false;
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
            else
            {
                Transform target = CS.getTarget().transform;
                bool attackMode = A.GetBool("AttackMode");

                if (target.position.x < transform.position.x && target.position.y < transform.position.y)
                {
                    //Enemy to bottom left
                    SR.flipX = true;
                    A.runtimeAnimatorController = bottomAnimator;
                }
                else if (target.position.x > transform.position.x && target.position.y < transform.position.y)
                {
                    //Enemy to bottom right
                    SR.flipX = false;
                    A.runtimeAnimatorController = bottomAnimator;
                }
                else if (target.position.x < transform.position.x && target.position.y > transform.position.y)
                {
                    //Enemy to top left
                    SR.flipX = true;
                    A.runtimeAnimatorController = topAnimator;
                }
                else if (target.position.x > transform.position.x && target.position.y > transform.position.y)
                {
                    //Enemy to top right
                    SR.flipX = false;
                    A.runtimeAnimatorController = topAnimator;
                }

                A.SetBool("AttackMode", attackMode);
            }

            A.SetFloat("Speed", MoveVec.magnitude);
        }
    }
}
