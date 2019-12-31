using UnityEngine;
using System.Collections.Generic;

public class CharacterState : MonoBehaviour
{
    /*
     * Start Data
     */

    //Appearance
    public Sprite characterSprite;

    //AttackTypes
    public AttackType[] attackTypes;

    //Health and Mana
    public int startHealth;
    public int startMana;
    
    //Attack chances and values
    public float startAttackSpeed;
    public Vector2 startAttackDamage;
    public int startChanceToHit;

    //Defense
    public int startPhysicalDamageReduction;

    //Attack crit chances and effect
    public int startChanceToCrit;
    public float startCriticalDamageMultiplier;

    //Spell crit chances and effect
    public int startSpellCritChance;
    public float startSpellCriticalDamageMultiplier;

    //Regen
    public ManaRegen[] startManaRegen;

    /*
     * RuntimeData
     */

    //Health and Mana
    private int currentHealth;
    private int currentMana;
    private int maxHealth;
    private int maxMana;

    //Attack chances and values
    private float currentAttackSpeed;
    private Vector2 currentAttackDamage;
    private int currentChanceToHit;

    //Defense
    private int currentPhysicalDamageReduction;

    //Attack crit chances and effect
    private int currentchanceToCrit;
    private float currentCriticalDamageMultiplier;

    //Spell crit chances and effect
    private int currentSpellCritChance;
    private float currentSpellCriticalDamageMultiplier;

    //Casting
    private bool casting;
    private float timeToCast;
    private float timeCasting;

    //Buffs
    private List<Buff> currentBuffs = new List<Buff>();

    //Targeting
    private CharacterState target;

    //Animation
    private Animator A;

    #region GettersAndSetters
    public CharacterState getTarget() { return target; }
    public void setTarget(CharacterState CS) { target = CS; }
    public int getHealth() { return currentHealth; }
    public void setHealth(int newHealth) { currentHealth = newHealth; }
    public int getMaxHealth() { return maxHealth; }
    public void setMaxHealth(int newMaxHealth) { maxHealth = newMaxHealth; }
    public int getMana() { return currentMana; }
    public void setMana(int newMana) { currentMana = newMana; }
    public int getMaxMana() { return maxMana; }
    public void setMaxMana(int newMaxMana) { maxMana = newMaxMana; }
    public float getAttackSpeed() { return currentAttackSpeed; }
    public void setAttackSpeed(float newAttackSpeed) { 
        currentAttackSpeed = newAttackSpeed; 
        //Set animation speed
    }
    public int getChanceToHit() { return currentChanceToHit; }
    public void setChanceToHit(int newChanceToHit) { currentChanceToHit = newChanceToHit; }
    public Vector2 getAttackDamage() { return currentAttackDamage; }
    public void setAttackDamage(Vector2 newAttackDamage) { currentAttackDamage = newAttackDamage; }
    public int getChanceToCrit() { return currentchanceToCrit; }
    public void setChanceToCrit(int newChanceToCrit) { currentchanceToCrit = newChanceToCrit; }
    public float getCritDamageMultiplier() { return currentCriticalDamageMultiplier; }
    public void setCritDamageMultiplier(float newCriticalDamageMultiplier) { currentCriticalDamageMultiplier = newCriticalDamageMultiplier; }
#endregion

    /*
     * Other variables
     */
    private UIManager UIM;

    private void Start()
    {
        UIM = UIManager.instance;

        InstantiateCharacter();

        if(tag.Equals("Player"))
        {
            UIM.UpdatePlayer(this);
        } else
        {
            target = GameObject.Find("Isilion").GetComponent<CharacterState>();
        }

        A = GetComponent<Animator>();
    }

    private void InstantiateCharacter()
    {
        currentHealth = startHealth;
        currentMana = startMana;
        currentAttackSpeed = startAttackSpeed;
        currentAttackDamage = startAttackDamage;
        currentChanceToHit = startChanceToHit;
        currentPhysicalDamageReduction = startPhysicalDamageReduction;
        currentCriticalDamageMultiplier = startCriticalDamageMultiplier;
        currentSpellCritChance = startSpellCritChance;
        currentSpellCriticalDamageMultiplier = startSpellCriticalDamageMultiplier;

        maxHealth = startHealth;
        maxMana = startMana;
    }

    public void DealDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            if(tag.Equals("Player"))
            {
                A.SetBool("Dead", true);
                //End Game
            } else if(tag.Equals("Enemy"))
            {
                A.SetBool("Dead", true);
            }
        }

        if (tag.Equals("Player"))
        {
            UIM.UpdatePlayerHealth(currentHealth, maxHealth);
        }
        else if (tag.Equals("Enemy"))
        {
            //Update target or target of target
        }
    }

    public void AddBuff(Buff b)
    {
        currentBuffs.Add(b);
    }

    public void RemoveBuff(Buff b)
    {
        currentBuffs.Remove(b);
    }

    
}

public enum AttackType
{
    MELEE,
    SPELLS
}

public enum ManaRegenCircumstance
{
    CASTING,
    NOTCASTING,
    NOTINCOMBAT
}

[System.Serializable]
public struct ManaRegen
{
    public ManaRegenCircumstance circumstance;
    public int manaRegenAmount;
}