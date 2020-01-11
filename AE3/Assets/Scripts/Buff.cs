using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Buff : MonoBehaviour
{
    public Sprite image;
    public new BuffName name;
    public string description;
    public Effect[] effects;
    public bool infiniteLength;
    public int lengthInSeconds;
    public int chance = 100;

    private BuffHandler BH;

    private int toRemove = -1;
    private float timeToRemoveBuff;
    private float currentTimePassed;
    private float lastTimeOpen;

    public void SetBuffHandler(BuffHandler buffHandler) { BH = buffHandler; }
    public BuffHandler GetBuffHandler() { return BH; }
    public void SetToRemove(int buffID, int seconds) { 
        toRemove = buffID; 
        timeToRemoveBuff = seconds; 
    }

    private void Start()
    {
        currentTimePassed -= Time.time;
    }

    private void Update()
    {
        if (toRemove > -1)
        {
            currentTimePassed += Time.time - lastTimeOpen;
            BH.UpdateBuffText(toRemove, UIManager.instance.MinutesAndSecondsFormat((int)System.Math.Ceiling(timeToRemoveBuff - currentTimePassed)));

            if (currentTimePassed > timeToRemoveBuff)
            {
                BH.RemoveBuff(toRemove);
                toRemove = -1;
                currentTimePassed = 0;
            }
        }

        lastTimeOpen = Time.time;
    }
}

public enum BuffName
{
    NONE,
    BlackFathomHamstring,
    NagaSpirit,
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
    BlessingOfKings,
    SealOfRighteousness,
    SealOfLight,
    SealOfJustice,
    DevotionAura,
    MagicalAura,
    RetributionAura
}