using UnityEngine;

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
    private Buff[] buffs;

    //Targeting
    private CharacterState target;

    /*
     * Getters and Setters
     */
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

public enum BuffName
{
    BlackFathomHamstring,
    Bash,
    VengefulStance,
    Chilled,
    FrozenSolid,
    Stun,
    Forbearance,
    DivineShield,
    HandOfFreedom,
    ArdentDefender,
    JudgementOfRighteousness,
    JudgementOfWisdom,
    JudgementOfWeakness,
    DivineStrength,
    DivineWisdom,
    BlessingOfKings

}

public enum BuffEffectName
{
    WeaponDamageMultipier,
    SlowMovement,
    SlowAttack,
    Stun,
    DeflectDamage,
    NoMovement,
    ReducedDamageTaken,
    ImmuneToDamage,
    NoMelee,
    ImmuneToStun,
    ImmuneToSlow,
    ReduceDamageGiven,
    RestoreHealthToAttacker,
    RestoreManaToAttacker,
    IncreaseMeleeDamage,
    RecoverMana,
    IncreaseAttackSpeed,
    CritChance,
    IncreaseMaxHeath,
    IncreaseMaxMana
}

public struct BuffEffect
{
    public BuffEffectName buffEffectName;
    public int buffPower;
    public int buffChance;
}

public struct Buff
{
    public BuffName buffName;
    public BuffEffect[] buffEffect;
    public bool infiniteLength;
    public int buffLengthInSeconds;
}