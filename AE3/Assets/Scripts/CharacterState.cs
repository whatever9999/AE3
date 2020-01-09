using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
    private PowerRegen[] currentPowerRegen;
    private PowerRegenCircumstance activeRegen = PowerRegenCircumstance.NOTINCOMBAT;
    public void changeRegenPower(float percentageOfCurrent) { 
        for(int i = 0; i < currentPowerRegen.Length; i++)
        {
            currentPowerRegen[i].regenAmount = (int)((currentPowerRegen[i].regenAmount / 100) * percentageOfCurrent);
        }
    }
    private float currentRegenInterval = 0;
    public float regenInterval = 5;

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

    //Buffs
    private List<Buff> currentBuffs = new List<Buff>();
    public List<Buff> GetCurrentBuffs() { return currentBuffs; }

    //Targeting
    private CharacterState target;

    //Animation
    private Animator A;

    //Realtime Effects
    private float percentageHealthRestoredToAttacker = 0;
    private float percentagePowerRestoredToAttacker = 0;
    private bool immuneToAllDamage;
    private bool immuneToStun;
    private bool immuneToSlow;
    private float allDamageReduction = 0;

    #region GettersAndSetters
    public CharacterState getTarget() { return target; }
    public void setTarget(CharacterState CS) { target = CS; }
    public int getHealth() { return currentHealth; }
    public void setHealth(int newHealth) { currentHealth = newHealth; }
    public int getMaxHealth() { return maxHealth; }
    public void setMaxHealth(int newMaxHealth) { maxHealth = newMaxHealth; }
    public int getCurrentHealth() { return currentHealth; }
    public void setCurrentHealth(int newHealth) { currentHealth = newHealth; }
    public int getPower() { return currentPower; }
    public void setMana(int newMana) { currentPower = newMana; }
    public int getMaxPower() { return maxPower; }
    public void setMaxMana(int newMaxMana) { maxPower = newMaxMana; }
    public float getAttackSpeed() { return currentAttackSpeed; }
    public void setAttackSpeed(float newAttackSpeed) { 
        currentAttackSpeed = newAttackSpeed;
    }
    public int getChanceToHit() { return currentChanceToHit; }
    public void setChanceToHit(int newChanceToHit) { currentChanceToHit = newChanceToHit; }
    public Vector2 getAttackDamage() { return currentAttackDamage; }
    public void setAttackDamage(Vector2 newAttackDamage) { print(currentAttackDamage);  currentAttackDamage = newAttackDamage; print(currentAttackDamage); }
    public int getChanceToCrit() { return currentchanceToCrit; }
    public void setChanceToCrit(int newChanceToCrit) { currentchanceToCrit = newChanceToCrit; }
    public int getSpellChanceToCrit() { return currentSpellCritChance; }
    public void setSpellChanceToCrit(int newChanceToCrit) { currentSpellCritChance = newChanceToCrit; }
    public float getCritDamageMultiplier() { return currentCriticalDamageMultiplier; }
    public void setCritDamageMultiplier(float newCriticalDamageMultiplier) { currentCriticalDamageMultiplier = newCriticalDamageMultiplier; }
    public float getPercentageHealthRestoredToAttacker() { return percentageHealthRestoredToAttacker; }
    public void setPercentageHealthRestoredToAttacker(float newPercentage) { percentageHealthRestoredToAttacker = newPercentage; }
    public void setActiveRegen(PowerRegenCircumstance newRegen) { activeRegen = newRegen; }
    public float getPercentagePowerRestoredToAttacker() { return percentagePowerRestoredToAttacker; }
    public void setPercentagePowerRestoredToAttacker(float newPower) { percentagePowerRestoredToAttacker = newPower; }
    public void setImmunity(bool newImmunity) { immuneToAllDamage = newImmunity; }
    public void setImmunityToStun(bool newImmunity) { immuneToStun = newImmunity; }
    public bool getImmunityToStun() { return immuneToStun; }
    public void setImmunityToSlow(bool newImmunity) { immuneToSlow = newImmunity; }
    public bool getImmunityToSlow() { return immuneToSlow; }
    public void setAllDamageReduction(float percentage) { allDamageReduction = percentage; }
    public float getAllDamageReduction() { return allDamageReduction; }

    #endregion

    /*
     * Other variables
     */
    private UIManager UIM;
    private PlayerAbilities PA;

    private void Start()
    {
        UIM = UIManager.instance;

        InstantiateCharacter();

        if(tag.Equals("Player"))
        {
            UIM.UpdatePlayer(this);
        }

        A = GetComponent<Animator>();
        if (tag == "Player")
        {
            PA = GetComponent<PlayerAbilities>();
        }
    }

    private void Update()
    {
        if(maxPower != 0 && currentPower != maxPower)
        {
            if(currentRegenInterval > regenInterval)
            {
                foreach (PowerRegen pr in currentPowerRegen)
                {
                    if (activeRegen == pr.circumstance)
                    {
                        currentPower += pr.regenAmount;

                        if(currentPower > maxPower)
                        {
                            currentPower = maxPower;
                        }

                        if (tag.Equals("Player"))
                        {
                            UIM.UpdatePlayerPower(currentPower, maxPower);
                            UIM.UpdateTargetOfTargetPower(currentPower, maxPower);
                        }
                        else if (tag.Equals("Enemy"))
                        {
                            CharacterState playerTarget = PlayerAbilities.instance.GetPlayerCharacterState().getTarget();
                            if (playerTarget == this)
                            {
                                UIM.UpdateTargetPower(playerTarget.getPower(), playerTarget.getMaxPower());
                            }
                        }

                        currentRegenInterval = 0;

                        PA.UpdateAbilityColours();
                        break;
                    }
                }
            }

            currentRegenInterval += Time.deltaTime;
        }
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
        currentPowerRegen = startPowerRegen;

        maxHealth = startHealth;
        maxPower = startPower;
    }

    public int HitCritAndResult(int amount, UIManager.ResultType resultType)
    {
        switch (resultType)
        {
            case UIManager.ResultType.PHYSICALDAMAGE:
                //Critical hit
                float rand = Random.Range(0, 100);
                if (rand <= currentchanceToCrit)
                {
                    amount = (int)(amount * currentCriticalDamageMultiplier);
                }

                //Deal Damage
                DealDamage(amount, UIManager.ResultType.PHYSICALDAMAGE);
                SFXManager.instance.PlayEffect(SoundEffectNames.ATTACK);
                break;
            case UIManager.ResultType.HEALING:
                //Critical hit
                rand = Random.Range(0, 100);
                if (rand <= currentSpellCritChance)
                {
                    amount = (int)(amount * currentSpellCriticalDamageMultiplier);
                }

                //Deal Damage
                Heal(amount);
                SFXManager.instance.PlayEffect(SoundEffectNames.SPELLCASTEND);
                break;
            case UIManager.ResultType.MAGICALDAMAGE:
                //Critical hit
                rand = Random.Range(0, 100);
                if (rand <= currentSpellCritChance)
                {
                    amount = (int)(amount * currentSpellCriticalDamageMultiplier);
                }

                //Deal Damage
                DealDamage(amount, UIManager.ResultType.MAGICALDAMAGE);
                SFXManager.instance.PlayEffect(SoundEffectNames.SPELLCASTEND);
                break;
            default:
                break;
        }

        return amount;
    }

    public void DealDamage(int damage, UIManager.ResultType damageType)
    {
        if(!immuneToAllDamage)
        {
            //Apply damage reduction
            if (damageType == UIManager.ResultType.PHYSICALDAMAGE)
            {
                damage = (int)((damage / 100.0) * (100 - allDamageReduction - currentPhysicalDamageReduction));
            }
            else
            {
                damage = (int)((damage / 100.0) * (100 - allDamageReduction));
            }

            currentHealth -= damage;
            UIM.SpawnResultText(transform.position, damage, damageType);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                if (tag.Equals("Player"))
                {
                    A.SetBool("Dead", true);
                    //End Game
                }
                else if (tag.Equals("Enemy"))
                {
                    A.SetBool("Dead", true);
                    tag = "Dead";
                }
            }

            if (tag.Equals("Player"))
            {
                UIM.UpdatePlayerHealth(currentHealth, maxHealth);
                UIM.UpdateTargetOfTargetHealth(currentHealth, maxHealth);
            }
            else if (tag.Equals("Enemy") || tag.Equals("Dead"))
            {
                CharacterState playerTarget = PlayerAbilities.instance.GetPlayerCharacterState().getTarget();
                if (playerTarget == this)
                {
                    UIM.UpdateTargetHealth(playerTarget.getHealth(), playerTarget.getMaxHealth());
                }
            }
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        UIM.SpawnResultText(transform.position, amount, UIManager.ResultType.HEALING);

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (tag.Equals("Player"))
        {
            UIM.UpdatePlayerHealth(currentHealth, maxHealth);
            UIM.UpdateTargetOfTargetHealth(currentHealth, maxHealth);
        }
        else if (tag.Equals("Enemy"))
        {
            CharacterState playerTarget = PlayerAbilities.instance.GetPlayerCharacterState().getTarget();
            if (playerTarget == this)
            {
                UIM.UpdateTargetHealth(playerTarget.getHealth(), playerTarget.getMaxHealth());
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
            UIM.UpdateTargetOfTargetPower(currentPower, maxPower);
        }
        else if (tag.Equals("Enemy"))
        {
            CharacterState playerTarget = PlayerAbilities.instance.GetPlayerCharacterState().getTarget();
            if (playerTarget == this)
            {
                UIM.UpdateTargetPower(playerTarget.getPower(), playerTarget.getMaxPower());
            }
        }
    }

    public void IncreasePower(float percentageOfPower)
    {
        int powerToIncrease = (int)((maxPower / 100.0) * percentageOfPower);
        currentPower += powerToIncrease;

        if(currentPower > maxPower)
        {
            currentPower = maxPower;
        }

        if (tag.Equals("Player"))
        {
            UIM.UpdatePlayerPower(currentPower, maxPower);
            UIM.UpdateTargetOfTargetPower(currentPower, maxPower);
        }
        else if (tag.Equals("Enemy"))
        {
            CharacterState playerTarget = PlayerAbilities.instance.GetPlayerCharacterState().getTarget();
            if (playerTarget == this)
            {
                UIM.UpdateTargetPower(playerTarget.getPower(), playerTarget.getMaxPower());
            }
        }
    }

    public void AddBuff(Buff b)
    {
        bool notAlreadyInUse = true;

        foreach(Buff currentB in currentBuffs)
        {
            if(b.name == currentB.name)
            {
                notAlreadyInUse = false;
            } else if((b.name == BuffName.JudgementOfRighteousness || b.name == BuffName.JudgementOfWeakness || b.name == BuffName.JudgementOfWisdom) && (currentB.name == BuffName.JudgementOfRighteousness || currentB.name == BuffName.JudgementOfWeakness || currentB.name == BuffName.JudgementOfWisdom))
            {
                currentB.GetBuffHandler().DeactivateBuff(currentB);
                break;
            }
        }

        if (notAlreadyInUse)
        {
            currentBuffs.Add(b);
        }
    }

    public void RemoveBuff(Buff b)
    {
        currentBuffs.Remove(b);
    }

    public void Footstep()
    {
        SFXManager.instance.PlayEffect(SoundEffectNames.FOOTSTEP);
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