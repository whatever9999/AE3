using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour
{
    public AuraType[] auras;

    private List<CharacterState> playersInAura;

    private AuraName currentAura = AuraName.NONE;

    private void Start()
    {
        playersInAura = new List<CharacterState>();
        playersInAura.Add(transform.parent.GetComponent<CharacterState>());
    }

    public void SetAura(AuraName newAura)
    {
        //Get aura effects;
        Effect[] currentAuraEffects = null;
        Effect[] newAuraEffects = null;
        foreach (AuraType a in auras)
        {
            if(a.name == currentAura)
            {
                currentAuraEffects = a.effects;
            } else if (a.name == newAura)
            {
                newAuraEffects = a.effects;
            }
        }

        //Get rid of previous aura effect and change to new effect
        foreach (CharacterState cs in playersInAura)
        {
            PlayerMovement thisPM = cs.GetComponent<PlayerMovement>();

            if (currentAuraEffects != null)
            {
                foreach (Effect e in currentAuraEffects)
                {
                    e.DeactivateEffect(cs, thisPM, null);
                }
            }
            
            if(newAuraEffects != null)
            {
                foreach (Effect e in newAuraEffects)
                {
                    e.ActivateEffect(cs, thisPM, null);
                }
            }
        }

        //Update aura
        currentAura = newAura;
    }

    

    private void Update()
    {
        for (int i = 0; i < playersInAura.Count; i++)
        {
            if (playersInAura[i].tag.Equals("Dead"))
            {
                playersInAura.Remove(playersInAura[i]);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            CharacterState thisCharacter = collision.GetComponent<CharacterState>();

            bool canAdd = true;
            foreach (CharacterState cs in playersInAura)
            {
                if (cs == thisCharacter)
                {
                    canAdd = false;
                }
            }

            if (canAdd)
            {
                playersInAura.Add(thisCharacter);
            }

            foreach(AuraType a in auras)
            {
                if(a.name == currentAura)
                {
                    PlayerMovement thisPM = thisCharacter.GetComponent<PlayerMovement>();
                    foreach(Effect e in a.effects)
                    {
                        e.ActivateEffect(thisCharacter, thisPM, null);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            CharacterState thisCharacter = collision.GetComponent<CharacterState>();
            playersInAura.Remove(thisCharacter);

            foreach (AuraType a in auras)
            {
                if (a.name == currentAura)
                {
                    PlayerMovement thisPM = thisCharacter.GetComponent<PlayerMovement>();
                    foreach (Effect e in a.effects)
                    {
                        e.ActivateEffect(thisCharacter, thisPM, null);
                    }
                }
            }
        }
    }
}

public enum AuraName
{
    NONE,
    DEVOTION,
    MAGICAL,
    RETRIBUTION
}

[System.Serializable]
public struct AuraType
{
    public AuraName name;
    public Effect[] effects;

}