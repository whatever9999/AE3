﻿using UnityEngine;
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

    //Health and Power
    public int startHealth;
    public PowerTypes powerType;
    public int startPower;
    
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
    public PowerRegen[] startPowerRegen;

    /*
     * RuntimeData
     */

    //Health and Mana
    private int currentHealth;
    private int currentPower;
    private int maxHealth;
    private int maxPower;

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
    public int getPower() { return currentPower; }
    public void setMana(int newMana) { currentPower = newMana; }
    public int getMaxPower() { return maxPower; }
    public void setMaxMana(int newMaxMana) { maxPower = newMaxMana; }
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
        currentPower = startPower;
        currentAttackSpeed = startAttackSpeed;
        currentAttackDamage = startAttackDamage;
        currentChanceToHit = startChanceToHit;
        currentchanceToCrit = startChanceToCrit;
        currentPhysicalDamageReduction = startPhysicalDamageReduction;
        currentCriticalDamageMultiplier = startCriticalDamageMultiplier;
        currentSpellCritChance = startSpellCritChance;
        currentSpellCriticalDamageMultiplier = startSpellCriticalDamageMultiplier;

        maxHealth = startHealth;
        maxPower = startPower;
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
            CharacterState playerTarget = PlayerAbilities.instance.GetPlayerCharacterState().getTarget();
            CharacterState targetOfTarget = playerTarget.getTarget();
            if (playerTarget == this)
            {
                UIM.UpdateTargetHealth(playerTarget.getHealth(), playerTarget.getMaxHealth());
            } else if (targetOfTarget == this)
            {
                UIM.UpdateTargetOfTargetHealth(targetOfTarget.getHealth(), targetOfTarget.getMaxHealth());
            }
        }
    }

    public void DepletePower(float percentageOfPower)
    {
        int powerToDeplete = (int)((maxPower / 100.0) * percentageOfPower);
        currentPower -= powerToDeplete;

        if (tag.Equals("Player"))
        {
            UIM.UpdatePlayerPower(currentPower, maxPower);
        }
        else if (tag.Equals("Enemy"))
        {
            CharacterState playerTarget = PlayerAbilities.instance.GetPlayerCharacterState().getTarget();
            CharacterState targetOfTarget = playerTarget.getTarget();
            if (playerTarget == this)
            {
                UIM.UpdateTargetPower(playerTarget.getPower(), playerTarget.getMaxPower());
            }
            else if (targetOfTarget == this)
            {
                UIM.UpdateTargetOfTargetPower(targetOfTarget.getPower(), targetOfTarget.getMaxPower());
            }
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

public enum PowerRegenCircumstance
{
    CASTING,
    NOTCASTING,
    NOTINCOMBAT
}

[System.Serializable]
public struct PowerRegen
{
    public PowerRegenCircumstance circumstance;
    public int regenAmount;
}

public enum PowerTypes
{
    MANA,
    ENERGY,
    RAGE
}