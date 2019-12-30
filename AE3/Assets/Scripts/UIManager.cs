using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public AbilityGroup[] abilityGroups;
    public AbilityGroup[] subAbilityGroups;

    private bool abilityPanelOpen = false;
    private AbilityGroupName activeAbilityGroup = AbilityGroupName.Retribution;
    private GameObject activeSubAbilityPanel;

    /*
     * Change between ability group with swipe variables
     */
    private Vector3 FirstTouchPos;   //First touch position
    private Vector3 LastTouchPos;   //Last touch position
    public int ScreenPercentageForSwipe;
    private float dragDistance;  //minimum distance for a swipe to be registered

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

    /*
     * Normal Attack variables
     */
    public GameObject normalAttackButton;

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

        dragDistance = Screen.height * ScreenPercentageForSwipe / 100; //dragDistance is N% height of the screen
    }

    private void Update()
    {
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                FirstTouchPos = touch.position;
                LastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                LastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                LastTouchPos = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than N% of the screen height
                //It's a drag
                if (Mathf.Abs(LastTouchPos.x - FirstTouchPos.x) > dragDistance || Mathf.Abs(LastTouchPos.y - FirstTouchPos.y) > dragDistance)
                {
                    //check if the drag is vertical or horizontal
                    if (Mathf.Abs(LastTouchPos.x - FirstTouchPos.x) > Mathf.Abs(LastTouchPos.y - FirstTouchPos.y))
                    {

                        //If the horizontal movement is greater than the vertical movement...
                        if ((LastTouchPos.x > FirstTouchPos.x))  //If the movement was to the right)
                        {
                            //Right swipe
                            ChangeAbilityPanel(AbilityGroupName.Retribution);
                        }
                        else
                        {
                            //Left swipe
                            ChangeAbilityPanel(AbilityGroupName.Holy);
                        }
                    }
                    else
                    {
                        //the vertical movement is greater than the horizontal movement
                        if (LastTouchPos.y > FirstTouchPos.y)  //If the movement was up
                        {
                            //Up swipe
                            ChangeAbilityPanel(AbilityGroupName.Protection);
                        }
                        else
                        {
                            //Down swipe
                            ChangeAbilityPanel(AbilityGroupName.Utilities);
                        }
                    }
                }
            }
        }
    }

    public void ToggleTargetPanel(bool to)
    {
        normalAttackButton.SetActive(to);
        targetPanel.SetActive(to);
        ToggleAbilityPanel(to);
        if (!to && activeSubAbilityPanel != null)
        {
            SwitchPanels(activeSubAbilityPanel);
        }
    }

    public void ChangeAbilityPanel(AbilityGroupName panelToOpen)
    {
        if(abilityPanelOpen && activeAbilityGroup != panelToOpen)
        {
            foreach (AbilityGroup ag in abilityGroups)
            {
                if (ag.abilityGroupName == activeAbilityGroup)
                {
                    ag.abilityPanel.SetActive(false);
                }
                else if (ag.abilityGroupName == panelToOpen)
                {
                    ag.abilityPanel.SetActive(true);
                }
            }

            activeAbilityGroup = panelToOpen;
        }
    }

    public void ToggleAbilityPanel(bool to)
    {
        abilityPanelOpen = to;

        if (abilityPanelOpen)
        {
            foreach (AbilityGroup ag in abilityGroups)
            {
                if (ag.abilityGroupName == activeAbilityGroup)
                {
                    ag.abilityPanel.SetActive(true);
                }
            }
        }
        else
        {
            foreach (AbilityGroup ag in abilityGroups)
            {
                if (ag.abilityGroupName == activeAbilityGroup)
                {
                    ag.abilityPanel.SetActive(false);
                }
            }
        }
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

    public void SwitchPanels(GameObject panelToOpen)
    {
        //Check if the panel clicked on is already open (close it if it is, otherwise open it)
        if (panelToOpen != activeSubAbilityPanel)
        {
            panelToOpen.SetActive(true);
            activeSubAbilityPanel = panelToOpen;
        } else
        {
            panelToOpen.SetActive(false);
            activeSubAbilityPanel = null;
        }

        //Turn off all other panels
        foreach(AbilityGroup ag in subAbilityGroups)
        {
            if(ag.abilityPanel != panelToOpen)
            {
                ag.abilityPanel.SetActive(false);
            }
        }
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
}