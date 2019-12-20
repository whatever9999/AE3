using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    UIManager UIM;

    private void Start()
    {
        UIM = UIManager.instance;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector3 Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 TouchPos = new Vector2(Pos.x, Pos.y);

                var Hit = Physics2D.OverlapPoint(TouchPos);

                if (Hit)
                {
                    if (Hit.transform.tag.Equals("Player") || Hit.transform.tag.Equals("Enemy"))
                    {
                        CharacterState currentTarget = GetComponent<CharacterState>().getTarget();
                        CharacterState targetCS = Hit.transform.GetComponent<CharacterState>();
                        if (currentTarget != null && currentTarget.tag.Equals(targetCS.tag))
                        {
                            GetComponent<CharacterState>().setTarget(null);
                            UIM.ToggleTargetPanel(false);
                            UIM.ToggleTargetOfTargetPanel(false);
                        } else
                        {
                            GetComponent<CharacterState>().setTarget(targetCS);
                            UIM.SetTarget(targetCS);
                            if(targetCS.tag.Equals("Enemy") && targetCS.getTarget() != null)
                            {
                                UIM.SetTargetOfTarget(targetCS.getTarget());
                            } else
                            {
                                UIM.ToggleTargetOfTargetPanel(false);
                            }
                        }
                    }
                }
            }
        }
    }
}