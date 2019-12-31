using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities instance;

    private CharacterState CS;

    public CharacterState GetPlayerCharacterState() { return CS; }

    private void Start()
    {
        instance = this;

        CS = GetComponent<CharacterState>();
    }

    public void UseAbility(Ability a)
    {
        //Switch case ability effects and add buffs
        a.UpdateAbilityColour();
    }
}
