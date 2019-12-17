using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5;

    private Rigidbody2D RB;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector3 MoveVec = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));

        //Prevent player from moving faster on diagonals
        if (MoveVec.magnitude > 1)
        {
            MoveVec = MoveVec.normalized;
        }

        RB.velocity = MoveVec * moveSpeed;

        //Rotate player
        if (CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0)
        {
            transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(CrossPlatformInputManager.GetAxis("Vertical"), CrossPlatformInputManager.GetAxis("Horizontal")) * 180 / Mathf.PI);
        }
    }
}
