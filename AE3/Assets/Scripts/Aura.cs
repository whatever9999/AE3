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
    }

    public void SetAura(AuraName newAura)
    {
        //Get rid of previous aura effect
        foreach(CharacterState cs in playersInAura)
        {
            switch (currentAura)
            {
                case AuraName.DEVOTION:

                    break;
                case AuraName.MAGICAL:

                    break;
                case AuraName.RETRIBUTION:

                    break;
                default:
                    break;
            }
        }

        switch (newAura)
        {
            case AuraName.DEVOTION:

                break;
            case AuraName.MAGICAL:

                break;
            case AuraName.RETRIBUTION:

                break;
            default:
                break;
        }
    }

    private void TriggerEffect(CharacterState cs)
    {
        switch (currentAura)
        {
            case AuraName.DEVOTION:

                break;
            case AuraName.MAGICAL:

                break;
            case AuraName.RETRIBUTION:

                break;
            default:
                break;
        }
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

            TriggerEffect(thisCharacter);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            playersInAura.Remove(collision.GetComponent<CharacterState>());
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
    public BuffEffect[] auraEffects;

}