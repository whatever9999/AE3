using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Range aggroRange;

    private Collider2D playerCollider;

    private CharacterState CS;
    private NormalAttack NA;

    private void Start()
    {
        CS = GetComponent<CharacterState>();
        NA = GetComponent<NormalAttack>();

        playerCollider = PlayerAbilities.instance.GetPlayerCharacterState().GetComponent<NormalAttack>().meleeRange.collider;
    }

    private void Update()
    {
        if (aggroRange.collider.IsTouching(playerCollider)) {
            CS.setTarget(PlayerAbilities.instance.GetPlayerCharacterState());
            UIManager.instance.UpdateTargetOfTarget(PlayerAbilities.instance.GetPlayerCharacterState());
            UIManager.instance.ToggleTargetOfTargetPanel(true);
            NA.EnableCombat();
        }
    }
}
