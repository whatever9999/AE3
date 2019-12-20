using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    /*
     * Player Panel
     */
    public GameObject playerPanel;
    private Image playerImage;
    private Slider playerHealthSlider;
    private Slider playerManaSlider;
    private Text playerHealthText;
    private Text playerManaText;

    /*
     * Target Panel
     */
    public GameObject targetPanel;
    private Image targetImage;
    private Slider targetHealthSlider;
    private Slider targetManaSlider;
    private Text targetHealthText;
    private Text targetManaText;

    /*
     * Target of Target Panel
     */
    public GameObject targetOfTargetPanel;
    private Image targetOfTargetImage;
    private Slider targetOfTargetHealthSlider;
    private Slider targetOfTargetManaSlider;

    private void Start()
    {
        instance = this;

        //Get player UI components
        Image[] images = playerPanel.GetComponentsInChildren<Image>();
        Slider[] sliders = playerPanel.GetComponentsInChildren<Slider>();
        Text[] texts = playerPanel.GetComponentsInChildren<Text>();
        for(int i = 0; i < sliders.Length; i++)
        {
            if(sliders[i].tag.Equals("HealthSlider"))
            {
                playerHealthSlider = sliders[i];
                playerHealthText = texts[i];
            } else if(sliders[i].tag.Equals("ManaSlider"))
            {
                playerManaSlider = sliders[i];
                playerManaText = texts[i];
            } 
        }

        foreach(Image i in images)
        {
            if (i.tag.Equals("CharacterImage"))
            {
                playerImage = i;
            }
        }

        //Get target UI components
        images = targetPanel.GetComponentsInChildren<Image>();
        sliders = targetPanel.GetComponentsInChildren<Slider>();
        texts = targetPanel.GetComponentsInChildren<Text>();
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].tag.Equals("HealthSlider"))
            {
                targetHealthSlider = sliders[i];
                targetHealthText = texts[i];
            }
            else if (sliders[i].tag.Equals("ManaSlider"))
            {
                targetManaSlider = sliders[i];
                targetManaText = texts[i];
            }
        }

        foreach (Image i in images)
        {
            if (i.tag.Equals("CharacterImage"))
            {
                targetImage = i;
            }
        }

        //Get target of target UI components
        images = targetOfTargetPanel.GetComponentsInChildren<Image>();
        sliders = targetOfTargetPanel.GetComponentsInChildren<Slider>();
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].tag.Equals("HealthSlider"))
            {
                targetOfTargetHealthSlider = sliders[i];
            }
            else if (sliders[i].tag.Equals("ManaSlider"))
            {
                targetOfTargetManaSlider = sliders[i];
            }
        }

        foreach (Image i in images)
        {
            if (i.tag.Equals("CharacterImage"))
            {
                targetOfTargetImage = i;
            }
        }
    }

    public void ToggleTargetPanel(bool to)
    {
        targetPanel.SetActive(to);
    }

    public void ToggleTargetOfTargetPanel(bool to)
    {
        targetOfTargetPanel.SetActive(to);
    }

    public void UpdatePlayer(CharacterState CS)
    {
        playerImage.sprite = CS.characterSprite;
        UpdatePlayerHealth(CS.getHealth(), CS.getMaxHealth());
        UpdatePlayerMana(CS.getMana(), CS.getMaxMana());
    }

    public void UpdatePlayerHealth(int currentHealth, int maxHealth)
    {
        float sliderValue = (float)currentHealth / (float)maxHealth;
        playerHealthSlider.value = sliderValue;

        playerHealthText.text = currentHealth + "/" + maxHealth;
    }

    public void UpdatePlayerMana(int currentMana, int maxMana)
    {
        float sliderValue = (float)currentMana / (float)maxMana;
        playerManaSlider.value = sliderValue;

        playerManaText.text = currentMana + "/" + maxMana;
    }

    public void SetTarget(CharacterState CS)
    {
        ToggleTargetPanel(true);
        targetImage.sprite = CS.characterSprite;

        UpdateTargetHealth(CS.getHealth(), CS.getMaxHealth());

        int targetMaxMana = CS.getMaxMana();
        if(targetMaxMana > 0)
        {
            targetManaSlider.gameObject.SetActive(true);
            UpdateTargetMana(CS.getMana(), CS.getMaxMana());
        } else
        {
            targetManaSlider.gameObject.SetActive(false);
        }
    }

    public void UpdateTarget(CharacterState CS)
    {
        UpdateTargetHealth(CS.getHealth(), CS.getMaxHealth());
        UpdateTargetMana(CS.getMana(), CS.getMaxMana());
    }

    public void UpdateTargetHealth(int currentHealth, int maxHealth)
    {
        float sliderValue = (float)currentHealth / (float)maxHealth;
        targetHealthSlider.value = sliderValue;

        targetHealthText.text = currentHealth + "/" + maxHealth;
    }

    public void UpdateTargetMana(int currentMana, int maxMana)
    {
        float sliderValue = (float)currentMana / (float)maxMana;
        targetManaSlider.value = sliderValue;

        targetManaText.text = currentMana + "/" + maxMana;
    }

    public void SetTargetOfTarget(CharacterState CS)
    {
        ToggleTargetOfTargetPanel(true);
        targetOfTargetImage.sprite = CS.characterSprite;

        UpdateTargetOfTargetHealth(CS.getHealth(), CS.getMaxHealth());

        int targetMaxMana = CS.getMaxMana();
        if (targetMaxMana > 0)
        {
            targetManaSlider.gameObject.SetActive(true);
            UpdateTargetOfTargetMana(CS.getMana(), CS.getMaxMana());
        }
        else
        {
            targetManaSlider.gameObject.SetActive(false);
        }
    }

    public void UpdateTargetOfTarget(CharacterState CS)
    {
        UpdateTargetOfTargetHealth(CS.getHealth(), CS.getMaxHealth());
        UpdateTargetOfTargetMana(CS.getMana(), CS.getMaxMana());
    }

    public void UpdateTargetOfTargetHealth(int currentHealth, int maxHealth)
    {
        float sliderValue = (float)currentHealth / (float)maxHealth;
        targetOfTargetHealthSlider.value = sliderValue;
    }

    public void UpdateTargetOfTargetMana(int currentMana, int maxMana)
    {
        float sliderValue = (float)currentMana / (float)maxMana;
        targetOfTargetManaSlider.value = sliderValue;
    }
}
