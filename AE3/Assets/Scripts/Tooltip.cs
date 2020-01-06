using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    private Image image;
    private new Text name;
    private Text description;
    private GameObject removeButton;

    private BuffHandler activeBH;
    private Buff activeBuff;

    private Ability activeAbility;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);

        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image i in images)
        {
            if (i.name.Equals("TooltipImage"))
            {
                image = i;
            }
        }

        Text[] texts = GetComponentsInChildren<Text>();
        foreach (Text t in texts)
        {
            if (t.name.Equals("TooltipName"))
            {
                name = t;
            }
            else if (t.name.Equals("TooltipDescription"))
            {
                description = t;
            }
        }

        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button b in buttons)
        {
            if (b.name.Equals("RemoveBuffButton"))
            {
                removeButton = b.gameObject;
            }
        }
    }

    public void RemoveBuff()
    {
        SFXManager.instance.PlayEffect(SoundEffectNames.BUTTON);
        activeBH.DeactivateBuff(activeBuff);
        CloseTooltip();
    }

    public void OpenTooltip(Buff b)
    {
        image.sprite = b.image;
        description.text = b.description;

        string buffName = b.name.ToString();
        string newName = buffName[0].ToString();
        for(int i = 1; i < buffName.Length; i++)
        {
            if(char.IsUpper(buffName[i]))
            {
                newName += " " + buffName[i];
            } else
            {
                newName += buffName[i];
            }
        }
        name.text = newName;

        if(b.tag.Equals("Buff"))
        {
            removeButton.SetActive(true);
        } else
        {
            removeButton.SetActive(false);
        }

        activeBH = b.GetBuffHandler();
        activeBuff = b;

        gameObject.SetActive(true);
    }

    public void OpenTooltip(Ability a)
    {
        image.sprite = a.image;
        description.text = a.description;

        string abilityName = a.name.ToString();
        string newName = abilityName[0].ToString();
        for (int i = 1; i < abilityName.Length; i++)
        {
            if (char.IsUpper(abilityName[i]))
            {
                newName += " " + abilityName[i];
            }
            else
            {
                newName += abilityName[i];
            }
        }
        name.text = newName;

        if (a.tag.Equals("Ability"))
        {
            removeButton.SetActive(true);
        }
        else
        {
            removeButton.SetActive(false);
        }

        activeAbility = a;

        gameObject.SetActive(true);
    }

    public void CloseTooltip()
    {
        SFXManager.instance.PlayEffect(SoundEffectNames.BUTTON);
        gameObject.SetActive(false);
        activeBH = null;
        activeBuff = null;
    }
}
